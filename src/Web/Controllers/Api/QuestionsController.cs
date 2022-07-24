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
using Web.Controllers;

namespace Web.Controllers.Api
{
	[Authorize]
	public class QuestionsController : BaseApiController
	{
		private readonly IMapper _mapper;
		private readonly IDataService _dataService;
		private readonly ITermsService _termsService;
		private readonly INotesService _notesService;
		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;

		public QuestionsController(IDataService dataService, ITermsService termsService, INotesService notesService,
			IQuestionsService questionsService, IRecruitsService recruitsService,
			IAttachmentsService attachmentsService, IMapper mapper)
		{
			_mapper = mapper;
			_dataService = dataService;
			_termsService = termsService;
			_notesService = notesService;
			_questionsService = questionsService;
			_attachmentsService = attachmentsService;
			_recruitsService = recruitsService;
		}

        [HttpGet("")]
        public async Task<ActionResult> Index(int term = 0, int subject = 0)
        {
			var qIds = new List<int>();

			if (term > 0)
			{
				var termNotesView = _dataService.FindTermNotesViewByTerm(term);
				if (termNotesView == null) return NotFound();
				
				qIds.AddRange(termNotesView.RQIds.SplitToIds());
				qIds.AddRange(termNotesView.QIds.SplitToIds());
			}
			else if (subject > 0)
			{
				var termNotesViews = _dataService.FetchTermNotesViewBySubject(subject);
				
				foreach (var termNotesView in termNotesViews) qIds.AddRange(termNotesView.RQIds.SplitToIds());
				foreach (var termNotesView in termNotesViews) qIds.AddRange(termNotesView.QIds.SplitToIds());

				qIds = qIds.OrderByDescending(x => x).ToList();
			}
			else
			{
				ModelState.AddModelError("params", "錯誤的查詢參數");
				return BadRequest(ModelState);
			}

			var result = new List<QuestionViewModel>();
			if (qIds.IsNullOrEmpty()) return Ok(result);


			qIds = qIds.Distinct().ToList();
			var questions = await _questionsService.FetchByIdsAsync(qIds);
			var viewList = await LoadQuestionViewsAsync(questions);
			
			foreach (var qId in qIds) result.Add(viewList.FirstOrDefault(x => x.Id == qId));

			return Ok(result);

		}


        async Task<List<QuestionViewModel>> LoadQuestionViewsAsync(IEnumerable<Question> questions)
		{
			var allRecruits = await _recruitsService.GetAllAsync();
			List<Term> allTerms = null;

			var types = new List<PostType> { PostType.Question, PostType.Option };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);

			var viewList = questions.MapViewModelList(_mapper, allRecruits.ToList(), attachments.ToList(), allTerms);

			var sources = viewList.SelectMany(q => q.Resolves).SelectMany(r => r.Sources);
			foreach (var item in sources)
			{
				item.MapContent(_notesService, _termsService);
			}


			return viewList;
		}
	}
}
