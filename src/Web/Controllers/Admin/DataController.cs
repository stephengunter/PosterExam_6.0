using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Web.Controllers;
using ApplicationCore.ViewServices;
using Web.Helpers;

namespace Web.Controllers.Admin
{
	public class DataController : BaseAdminController
	{
		private readonly AdminSettings _adminSettings;
		private readonly IDataService _dataService;
		private readonly RootSubjectSettings _rootSubjectSettings;

		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly INotesService _notesService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;
		private readonly IQuestionsService _questionsService;

		private readonly IMapper _mapper;

		public DataController(IOptions<RootSubjectSettings> rootSubjectSettings, IOptions<AdminSettings> adminSettings,
			IDataService dataService, ISubjectsService subjectsService, ITermsService termsService, INotesService notesService, 
			IRecruitsService recruitsService, IQuestionsService questionsService, IAttachmentsService attachmentsService, IMapper mapper)
		{
			_adminSettings = adminSettings.Value;
			_rootSubjectSettings = rootSubjectSettings.Value;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_notesService = notesService;
			_recruitsService = recruitsService;
			_questionsService = questionsService;
			_attachmentsService = attachmentsService;

			_dataService = dataService;

			_mapper = mapper;
		}


		//儲存每個Subject, Term 底下的QuestionId(精選試題)
		#region subject-questions
		
		[HttpPost("subject-questions")]
		public async Task<ActionResult> StoreSubjectQuestions([FromBody] AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			//獲取所有歷屆試題Id
			var recruitQuestionIds = _questionsService.FetchAllRecruitQuestionIds();

			//專業科目(1)：臺灣自然及人文地理
			var firstRootSubject = _subjectsService.GetById(_rootSubjectSettings.FirstId);
			await SaveSubjectQuestionsAsync(firstRootSubject, recruitQuestionIds);


			//專業科目(2)：郵政法規大意及交通安全常識
			var secondRootSubject = _subjectsService.GetById(_rootSubjectSettings.SecondId);
			await SaveSubjectQuestionsAsync(secondRootSubject, recruitQuestionIds);


			return Ok();
		}

		async Task SaveSubjectQuestionsAsync(Subject rootSubject, IEnumerable<int> recruitQuestionIds)
		{
			var subjects = rootSubject.SubItems;

			var models = new List<SubjectQuestionsViewModel>();
			foreach (var subject in subjects)
			{
				int parentId = 0;
				var terms = await _termsService.FetchAsync(subject, parentId);
				_termsService.LoadSubItems(terms);

				var subjectQuestionsModel = new SubjectQuestionsViewModel { SubjectId = subject.Id };
				foreach (var term in terms)
				{
					var termQuestionsModel = new TermQuestionsViewModel
					{
						TermId = term.Id,
						QuestionIds = term.QIds.SplitToIds()
					};
					
					if (term.SubItems.HasItems())
					{
						termQuestionsModel.SubItems = term.SubItems.Select(subItem => new TermQuestionsViewModel
						{
							TermId = subItem.Id,
							QuestionIds = subItem.QIds.SplitToIds()
						}).ToList();
					}

					subjectQuestionsModel.TermQuestions.Add(termQuestionsModel);

				}

				models.Add(subjectQuestionsModel);
			}

			_dataService.SaveSubjectQuestions(rootSubject.Id, models);
		}
		#endregion



		
		[HttpPost("year-recruits")]
		public async Task<ActionResult> StoreYearRecruits([FromBody] AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			int parent = 0;
			var recruits = await _recruitsService.FetchAsync(parent);

			recruits = recruits.Where(x => x.Active).GetOrdered();

			_recruitsService.LoadSubItems(recruits);

			var recruitViews = recruits.MapViewModelList(_mapper);

			foreach (var yearView in recruitViews)
			{
				foreach (var recruitView in yearView.SubItems)
				{
					recruitView.QuestionIds = _questionsService.FetchQuestionIdsByRecruit(new Recruit { Id = recruitView.Id }).ToList();

					foreach (var partView in recruitView.SubItems)
					{
						partView.QuestionIds = _questionsService.FetchQuestionIdsByRecruit(new Recruit { Id = partView.Id }).ToList();
					}
				}

			}


			_dataService.SaveYearRecruits(recruitViews);

			return Ok();
		}

		[HttpPost("note-categories")]
		public async Task<ActionResult> NoteCategories([FromBody] AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var allSubjects = await _subjectsService.FetchAsync();
			allSubjects = allSubjects.Where(x => x.Active);

			var allTerms = await _termsService.FetchAllAsync();

			var rootSubjects = allSubjects.Where(x => x.ParentId < 1).GetOrdered();

			var categories = rootSubjects.Select(item => item.MapNoteCategoryViewModel()).ToList();
			foreach (var root in categories)
			{
				int parentId = root.Id;
				var subjects = allSubjects.Where(x => x.ParentId == parentId).GetOrdered();
				root.SubItems = subjects.Select(item => item.MapNoteCategoryViewModel(parentId)).ToList();
			}

			var subjectCategories = categories.SelectMany(x => x.SubItems);

			//只到ChapterTitle, 捨棄Hide項目
			foreach (var subjectCategory in subjectCategories)
			{
				var terms = allTerms.Where(item => item.SubjectId == subjectCategory.Id && item.ParentId == 0 && item.ChapterTitle && !item.Hide).GetOrdered();
				subjectCategory.SubItems = terms.Select(item => item.MapNoteCategoryViewModel()).ToList();
			}

			_dataService.SaveNoteCategories(categories);

			return Ok();
		}

		#region term-notes

		[HttpPost("term-notes")]
		public async Task<ActionResult> TermNotes([FromBody] AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			_dataService.CleanTermNotes();

			var categories = _dataService.FetchNoteCategories();
			var subjects = categories.SelectMany(x => x.SubItems).ToList();

			var yearRecruits = _dataService.FetchYearRecruits();
			var yearRecruitQids = new Dictionary<int, List<int>>();
			foreach (var yearRecruit in yearRecruits)
			{
				var questionIds = new List<int>();
				foreach (var recruit in yearRecruit.SubItems)
				{
					if (recruit.SubItems.HasItems())
					{
						foreach (var part in recruit.SubItems) questionIds.AddRange(part.QuestionIds);
					}
					else
					{
						questionIds.AddRange(recruit.QuestionIds);
					}
				}
				yearRecruitQids.Add(yearRecruit.Year, questionIds);
			}


			foreach (var subject in subjects)
			{
				if (subject.SubItems.HasItems())
				{
					foreach (var term in subject.SubItems)
					{
						var selectedTerm = _termsService.GetById(term.Id);
						await SaveTermNotes(selectedTerm, yearRecruitQids);
					}

				}
				else
				{
					Subject selectedSubject = await _subjectsService.GetByIdAsync(subject.Id);
					int parent = -1;
					//科目底下所有條文
					var terms = (await _termsService.FetchAsync(selectedSubject, parent)).Where(x => !x.ChapterTitle);
					var termIds = terms.Select(x => x.Id).ToList();

					if (terms.HasItems())
					{
						_termsService.LoadSubItems(terms);
						terms = terms.GetOrdered();

						foreach (var term in terms)
						{
							await SaveTermNotes(term, yearRecruitQids);
						}
					}
				}
			}

			return Ok();
		}

		async Task SaveTermNotes(Term term, Dictionary<int, List<int>> yearRecruitQids)
		{
			var termIds = new List<int>() { term.Id };
			if (term.SubItems.HasItems()) termIds.AddRange(term.GetSubIds());
			var notes = await _notesService.FetchAsync(termIds);

			var RQIds = new List<int>();
			foreach (KeyValuePair<int, List<int>> yearRecruitQid in yearRecruitQids)
			{
				foreach (int qid in term.GetQuestionIds())
				{
					if (yearRecruitQid.Value.Contains(qid)) RQIds.Add(qid);
				}
			}
			RQIds = RQIds.Distinct().ToList();

			var qids = new List<int>();
			foreach (int qid in term.GetQuestionIds())
			{
				if (!RQIds.Contains(qid)) qids.Add(qid);
			}
			qids = qids.Distinct().ToList();

			var postIds = notes.Select(x => x.Id).ToList();
			var attachments = (await _attachmentsService.FetchAsync(PostType.Note, postIds)).ToList();

			var noteViewList = notes.MapViewModelList(_mapper, attachments.ToList());

			var termViewModel = term.MapViewModel(_mapper);

			
			_dataService.SaveTermNotes(termViewModel, noteViewList, RQIds, qids);

		}
		#endregion

	}
}
