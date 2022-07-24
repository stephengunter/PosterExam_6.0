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
	public class RecruitQuestionsController : BaseAdminController
	{
		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public RecruitQuestionsController(IQuestionsService questionsService, IRecruitsService recruitsService,
			IAttachmentsService attachmentsService, ISubjectsService subjectsService, ITermsService termsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_attachmentsService = attachmentsService;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int recruit)
		{
			Recruit selectedRecruit = _recruitsService.GetById(recruit);
			if (selectedRecruit == null)
			{
				ModelState.AddModelError("recruit", "年度不存在");
				return BadRequest(ModelState);
			}

			if (selectedRecruit.RecruitEntityType == RecruitEntityType.Year) return Ok(new List<Question>().GetPagedList(_mapper));

			var allSubjects = await _subjectsService.FetchAsync();

			var subject = _recruitsService.FindSubject(selectedRecruit, allSubjects);

			if (subject == null)
			{
				ModelState.AddModelError("subject", "科目不存在");
				return BadRequest(ModelState);
			}

			_subjectsService.LoadSubItems(subject);

			var questions = await _questionsService.FetchByRecruitAsync(selectedRecruit, subject);

			List<Term> allTerms = null;
			List<Recruit> allRecruits = null;
			List<UploadFile> attachments = null;
			if (questions.HasItems())
			{
				var types = new List<PostType>() { PostType.Question, PostType.Option };
				attachments = (await _attachmentsService.FetchByTypesAsync(types)).ToList();
				allTerms = (await _termsService.FetchAllAsync()).ToList();
			}

			var pageList = questions.GetPagedList(_mapper, allRecruits, attachments, allTerms);
			foreach (var item in pageList.ViewList)
			{
				item.Options = item.Options.OrderByDescending(o => o.Correct).ToList();
			}
			

			return Ok(pageList);
		}

	}
}
