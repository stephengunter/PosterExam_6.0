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
using Web.Models;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.Specifications;
using Web.Controllers;

namespace Web.Controllers.Admin
{
	public class SettingsController : BaseAdminController
	{
		private readonly IDataService _dataService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly RootSubjectSettings _rootSubjectSettings;
		private readonly IMapper _mapper;

		public SettingsController(IOptions<RootSubjectSettings> rootSubjectSettings, IDataService dataService, IRecruitsService recruitsService, 
			ISubjectsService subjectsService, ITermsService termsService, IMapper mapper)
		{
			_dataService = dataService;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_rootSubjectSettings = rootSubjectSettings.Value;
			_mapper = mapper;
		}

		[HttpGet("exam")]
		public async Task<ActionResult> FindExamSettings(int subject)
		{
			var rootSubject = _subjectsService.GetById(subject);
			if (rootSubject == null) return NotFound();
			if (rootSubject.ParentId > 0)
			{
				ModelState.AddModelError("subject", "錯誤的科目");
				return BadRequest(ModelState);
			}

			var model = _dataService.FindExamSettings(subject);
			if (model == null) return NotFound();

			var rootSubjectView = rootSubject.MapViewModel(_mapper);
			var subjectViews = rootSubject.SubItems.MapViewModelList(_mapper);

			var allTerms = await _termsService.FetchAsync(new TermFilterBySubjectsSpecification(rootSubject.GetSubIds().ToList()));
			var termViews = allTerms.MapViewModelList(_mapper);

			model.Subject = rootSubjectView;
			foreach (var part in model.Parts)
			{
				foreach (var subjectsSettings in part.Subjects)
				{
					subjectsSettings.Subject = subjectViews.FirstOrDefault(x => x.Id == subjectsSettings.SubjectId);
					foreach (var tqSettings in subjectsSettings.TermQuestions)
					{
						tqSettings.Term = termViews.FirstOrDefault(x => x.Id == tqSettings.TermId);
						foreach (var item in tqSettings.SubItems)
						{
							item.Term = termViews.FirstOrDefault(x => x.Id == item.TermId); ;
						}
					}
				}
			}


			return Ok(model);
		}

		[HttpPost("exam")]
		public ActionResult SaveExamSettings([FromBody] ExamSettingsViewModel model)
		{
			var subjectId = model.SubjectId;
			model.Subject = null;
			model.Recruit = null;

			//移除object資料
			foreach (var part in model.Parts)
			{
				foreach (var subjectsSettings in part.Subjects)
				{
					subjectsSettings.Subject = null;
					foreach (var tqSettings in subjectsSettings.TermQuestions)
					{
						tqSettings.Term = null;
						foreach (var item in tqSettings.SubItems)
						{
							item.Term = null;
						}
					}
				}
			}

			_dataService.SaveExamSettings(subjectId, model);

			

			return Ok();
		}
		
	}
}
