using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Exceptions;
using Web.Models;
using Web.Helpers;
using Web.Controllers;

namespace Web.Controllers.Api
{
	[Authorize]
	public class ExamsController : BaseApiController
	{
		private readonly UserManager<User> _userManager;

		private readonly IDataService _dataService;

		private readonly IQuestionsService _questionsService;
		private readonly IResolvesService _resolvesService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IExamsService _examsService;
		private readonly ISubjectsService _subjectsService;
		private readonly IRecruitsService _recruitsService;

		private readonly IMapper _mapper;

		public ExamsController(UserManager<User> userManager, IDataService dataService, 
			IAttachmentsService attachmentsService, IQuestionsService questionsService, IResolvesService resolvesService,
			IExamsService examsService, ISubjectsService subjectsService, IRecruitsService recruitsService,
			IMapper mapper)
		{
			_userManager = userManager;

			_dataService = dataService;

			_questionsService = questionsService;
			_resolvesService = resolvesService;
			_attachmentsService = attachmentsService;
			_examsService = examsService;
			_subjectsService = subjectsService;
			_recruitsService = recruitsService;

			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject = 0, int status = -1, string sortBy = "lastUpdated", bool desc = true, int page = 1, int pageSize = 10)
		{
			var user = await _userManager.FindByIdAsync(CurrentUserId);
			if (user == null) throw new UserNotFoundException(CurrentUserId);
			

			var model = new ExamIndexViewModel();

			if (page < 1) //初次載入頁面
			{
				model.LoadExamTypeOptions();
				model.LoadRecruitExamTypeOptions();

				var yearRecruits = _dataService.FetchYearRecruits();
				model.YearRecruits = yearRecruits.ToList();


				model.LoadStatusOptions();

				//考試科目
				var examSubjects = await _subjectsService.FetchExamSubjectsAsync();
				model.LoadSubjectOptions(examSubjects, "全部");

				page = 1;
			}

			var exams = await _examsService.FetchAsync(user);
			

			if (exams.HasItems())
			{
				exams = exams.FilterByStatus(status.ToExamStaus());

				var subjects = await _subjectsService.FetchAsync();

				if (subject > 0) exams = FilterBySubject(exams, subjects, subject);


				foreach (var exam in exams) exam.LoadSubject(subjects);

				exams = exams.GetOrdered(sortBy, desc);
			}
			
			model.PagedList = exams.GetPagedList(_mapper, page, pageSize, sortBy, desc);

			return Ok(model);
		}

		[HttpGet("init")]
		public async Task<ActionResult> Init()
		{
			var model = new ExamIndexViewModel();

			model.LoadExamTypeOptions();
			model.LoadRecruitExamTypeOptions();

			var yearRecruits = _dataService.FetchYearRecruits();
			model.YearRecruits = yearRecruits.ToList();

			//考試科目
			var examSubjects = await _subjectsService.FetchExamSubjectsAsync();
			model.LoadSubjectOptions(examSubjects, "全部");

			return Ok(model);
		}

		#region  Create
		[HttpGet("create")]
		public async Task<ActionResult> Create(int recruit = 0, int type = -1, int rtype = -1, int subject = 0)
		{
			Exam exam = null;
			string examTitle = "";

			if (recruit > 0)   
			{
				//歷屆試題模式, 完全相同
				var yearRecruits = _dataService.FetchYearRecruits().ToList();
				var recruitsViews = yearRecruits.SelectMany(item => item.SubItems);

				var selectedRecruitView = recruitsViews.FirstOrDefault(x => x.Id == recruit);
				if (selectedRecruitView == null)
				{
					ModelState.AddModelError("recruit", "年度不存在");
					return BadRequest(ModelState);
				}


				exam = await InitByRecruitAsync(selectedRecruitView, yearRecruits);

				var selectedYear = yearRecruits.FirstOrDefault(x => x.Id == selectedRecruitView.ParentId);
				var selectedSubject = await _subjectsService.GetByIdAsync(selectedRecruitView.SubjectId);

				examTitle = $"歷屆試題_{selectedYear.Title}_{selectedSubject.Title}_{DateTime.Today.ToDateNumber()}";

			}
			else 
			{
				//限訂閱會員
				if (!CurrentUserIsSubscriber) return Forbid();
				
				ExamType examType = type.ToExamType();
				if (examType == ExamType.Unknown)
				{
					ModelState.AddModelError("type", "題庫來源錯誤");
					return BadRequest(ModelState);
				}

				//取得測驗設定
				var examSettingsView = _dataService.FindExamSettings(subject);
				if (examSettingsView == null)
				{
					throw new Exception("ExamSettings Not Found. subject = " + subject);
				}

				//取得SubjectQuestions
				var subjectQuestionsList = _dataService.FindSubjectQuestions(subject);
				if (subjectQuestionsList.IsNullOrEmpty())
				{
					throw new Exception("SubjectQuestions Not Found. subject = " + subject);
				}

				//一定是CrossYears, 差別在 examType
				exam = new Exam
				{ 
					ExamType = examType,
					RecruitExamType = RecruitExamType.CrossYears,
					SubjectId = subject,
					OptionType = examSettingsView.Parts.First().OptionType.ToOptionType()
				};
				
				await InitExamPartsAsync(exam, examSettingsView, subjectQuestionsList);

				var selectedSubject = await _subjectsService.GetByIdAsync(subject);

				if (examType == ExamType.Recruit)
				{
					examTitle = $"{examType.GetDisplayName()}_{RecruitExamType.CrossYears.GetDisplayName()}_{selectedSubject.Title}_{DateTime.Today.ToDateNumber()}";
				}
				else if (examType == ExamType.System)
				{
					examTitle = $"{examType.GetDisplayName()}_{selectedSubject.Title}_{DateTime.Today.ToDateNumber()}";
				}
					
			}


			//begin 測試題目有無重複
			var qids = exam.QuestionIds;
			var dis = qids.Distinct().ToList();
			if (qids.Count != dis.Count) throw new ExamQuestionDuplicated();
			//end 測試題目有無重複

			exam.LoadPartTitles();

			await _examsService.CreateAsync(exam, CurrentUserId);

			var types = new List<PostType> { PostType.Question, PostType.Option, PostType.Resolve };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);

			var examView = exam.MapExamViewModel(_mapper, attachments.ToList());
			examView.Title = examTitle;

			return Ok(examView);
		}

		async Task<Exam> InitByRecruitAsync(RecruitViewModel recruitView, List<RecruitViewModel> recruitViewList)
		{
			var rootRecruitView = recruitViewList.FirstOrDefault(x => x.Id == recruitView.ParentId);
			var exam = new Exam()
			{
				RecruitId = rootRecruitView.Id,
				SubjectId = recruitView.SubjectId,

				ExamType = ExamType.Recruit,
				RecruitExamType = RecruitExamType.Exactly,
				OptionType = recruitView.OptionType.ToOptionType()
			};

			var parts = recruitView.SubItems;
			if (parts.HasItems())
			{
				foreach (var part in parts)
				{
					var examPart = await InitExamPartAsync(part);
					exam.Parts.Add(examPart);
				}
			}
			else
			{
				var examPart = await InitExamPartAsync(recruitView);
				exam.Parts.Add(examPart);
			}

			exam.LoadPartTitles();
			return exam;

		}

		async Task<ExamPart> InitExamPartAsync(RecruitViewModel recruitView)
		{
			var examPart = new ExamPart
			{
				Points = recruitView.Points,
				MultiAnswers = recruitView.MultiAnswers,
				Order = recruitView.Order,
				OptionCount = recruitView.OptionCount,
				Title = recruitView.Title
			};


			var questions = (await _questionsService.FetchByIdsAsync(recruitView.QuestionIds)).ToList();

			examPart.LoadExamQuestions(questions);

			return examPart;
		}

		async Task InitExamPartsAsync(Exam exam, ExamSettingsViewModel examSettingsView, IEnumerable<SubjectQuestionsViewModel> subjectQuestionsList)
		{
			List<int> recruitQuestionIds = new List<int>();
			if (exam.ExamType == ExamType.Recruit)
			{
				//只要歷屆試題
				recruitQuestionIds = _questionsService.FetchAllRecruitQuestionIds().ToList();
			}

			var pickedQids = new List<int>();

			foreach (var examPartSettings in examSettingsView.Parts)
			{
				// 依照設定, 隨機出題
				var examPart = new ExamPart
				{
					Points = examPartSettings.Points,
					MultiAnswers = examPartSettings.MultiAnswers,
					Order = examPartSettings.Order,
					OptionCount = examPartSettings.OptionCount,
					Title = examPartSettings.Title
				};

				var qids = examPartSettings.PickQuestionIds(subjectQuestionsList, pickedQids, recruitQuestionIds);
				pickedQids.AddRange(qids);

				var questions = (await _questionsService.FetchByIdsAsync(qids)).ToList();

				examPart.LoadExamQuestions(questions);

				exam.Parts.Add(examPart);
				
			}

			//測試是否全為歷屆試題
			if (recruitQuestionIds.HasItems())
			{
				var examQids = exam.QuestionIds;
				var notIn = examQids.Where(x => !recruitQuestionIds.Contains(x));

				if (notIn.HasItems()) throw new ExamNotRecruitQuestionSelected();
			}
			
		}

		#endregion

		[HttpPut("{id}/{action}")]
		public async Task<ActionResult> Save(int id, [FromBody] ExamViewModel model)
		{
			var existingEntity = await _examsService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			var exam = model.MapEntity(_mapper, CurrentUserId);

			ValidateSaveRequest(exam);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			_examsService.SaveExam(existingEntity, exam);

			return Ok();
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] ExamViewModel model)
		{
			var exam = await _examsService.GetByIdAsync(id);
			if (exam == null) return NotFound();

			//只更新 title
			exam.Title = model.Title;
			exam.Reserved = true;
			exam.SetUpdated(CurrentUserId);

			ValidateSaveRequest(exam);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			await _examsService.UpdateAsync(exam);

			return Ok();
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			bool withOptions = true;
			var exam = _examsService.GetById(id, withOptions);

			if (exam == null) return NotFound();
			ValidateEditRequest(exam);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var types = new List<PostType> { PostType.Question, PostType.Option, PostType.Resolve };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);

			return Ok(exam.MapExamViewModel(_mapper, attachments.ToList()));
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] ExamViewModel model)
		{
			int id = model.Id;
			var existingEntity = await _examsService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			ValidateEditRequest(existingEntity);
			if (!ModelState.IsValid) return BadRequest(ModelState);


			var exam = model.MapEntity(_mapper, CurrentUserId);

			//先存檔
			_examsService.SaveExam(existingEntity, exam);

			//對答案
			bool withOptions = true;
			exam = _examsService.GetById(id, withOptions);

			exam.SetAnswers();
			exam.Finish();

			await _examsService.UpdateAsync(exam);

			return Ok();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult> Details(int id)
		{
			bool withOptions = true;
			var exam = _examsService.GetById(id, withOptions);
			if (exam == null) return NotFound();

			ValidateRequest(exam);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var resolves = await _resolvesService.FetchExamResolvesAsync(exam);

			var types = new List<PostType> { PostType.Question, PostType.Option, PostType.Resolve };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);

			return Ok(exam.MapExamViewModel(_mapper, attachments.ToList(), resolves.ToList()));
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var exam = await _examsService.GetByIdAsync(id);
			if (exam == null) throw new EntityNotFoundException(new Exam { Id = id });

			ValidateDeleteRequest(exam);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			await _examsService.DeleteAsync(exam);

			return Ok();
		}


		#region Helper

		IEnumerable<Exam> FilterBySubject(IEnumerable<Exam> exams, IEnumerable<Subject> subjects, int subjectId)
		{
			var selectedSubject = subjects.FirstOrDefault(x => x.Id == subjectId);
			if (selectedSubject == null) throw new EntityNotFoundException(new Subject { Id = subjectId });

			return exams.Where(x => x.SubjectId == subjectId);
		}

		#endregion

		#region Validate

		void ValidateRequest(Exam exam)
		{
			if (exam.UserId != CurrentUserId) ModelState.AddModelError("userId", "權限不足");
		}

		void ValidateSaveRequest(Exam exam)
		{
			ValidateRequest(exam);
		}

		void ValidateDeleteRequest(Exam exam)
		{
			ValidateRequest(exam);
		}

		void ValidateEditRequest(Exam exam)
		{
			ValidateRequest(exam);

			if (exam.IsComplete) ModelState.AddModelError("isComplete", "此測驗已經完成");

		}

		#endregion
	}
}
