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
using Web.Controllers;

namespace Web.Controllers.Admin
{
	public class TermQuestionsController : BaseAdminController
	{
		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public TermQuestionsController(IQuestionsService questionsService, IRecruitsService recruitsService, IAttachmentsService attachmentsService,
			ISubjectsService subjectsService, ITermsService termsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_recruitsService = recruitsService;
			_attachmentsService = attachmentsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

			_mapper = mapper;
		}


		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] TermQuestion model)
		{
			var term = await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var qids = term.QIds.SplitToIds();
			if (!qids.Contains(model.QuestionId)) qids.Add(model.QuestionId);

			term.QIds = qids.JoinToStringIntegers();

			await _termsService.UpdateAsync(term);

			return Ok();
		}
		

		[HttpPost("remove")]
		public async Task<ActionResult> Remove([FromBody] TermQuestion model)
		{
			var term = await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var qids = term.QIds.SplitToIds();
			if (!qids.Contains(model.QuestionId)) return Ok();

			qids.RemoveAll(item => item == model.QuestionId);

			term.QIds = qids.JoinToStringIntegers();

			await _termsService.UpdateAsync(term);

			return Ok();
		}

		async Task<Term> ValidateRequestAsync(TermQuestion model)
		{
			var term = await _termsService.GetByIdAsync(model.TermId);
			if (term == null) ModelState.AddModelError("termId", "條文不存在");

			var question = await _questionsService.GetByIdAsync(model.QuestionId);
			if (question == null) ModelState.AddModelError("questionId", "試題不存在");

			return term;

		}


	}
}
