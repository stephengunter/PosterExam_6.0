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
	public class OptionsController : BaseAdminController
	{
		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IMapper _mapper;

		public OptionsController(IQuestionsService questionsService, IMapper mapper, IAttachmentsService attachmentsService)
		{
			_questionsService = questionsService;
			_attachmentsService = attachmentsService;
			_mapper = mapper;
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] QuestionViewModel model)
		{
			var question = _questionsService.GetById(model.Id);

			if (question == null)
			{
				ModelState.AddModelError("question", "錯誤的QuestionId");
				return BadRequest(ModelState);
			}

			var correctOptions = question.Options.Where(x => x.Correct);
			int minCorrectCounts = correctOptions.Count() == 0 ? 1 : 0; 
			int maxCorrectCounts = question.MultiAnswers ? 999 : 1;

			ValidateRequest(model.Options, minCorrectCounts, maxCorrectCounts);
			if (!ModelState.IsValid) return BadRequest(ModelState);


			foreach (var optionModel in model.Options)
			{
				optionModel.QuestionId = question.Id;
				var option = optionModel.MapEntity(_mapper);
				option = await _questionsService.AddOptionAsync(option);

				var mediaIds = optionModel.Attachments.Select(x => x.Id).ToList();

				var attachments = await _attachmentsService.FetchByIdsAsync(mediaIds);

				foreach (var attachment in attachments)
				{
					attachment.PostType = PostType.Option;
					attachment.PostId = option.Id;
				}

				_attachmentsService.UpdateRange(attachments);

			}
			
			return Ok();
		}


		void ValidateRequest(ICollection<OptionViewModel> models, int minCorrectCounts, int maxCorrectCounts)
		{
			if (models.IsNullOrEmpty())
			{
				ModelState.AddModelError("options", "正確選項數量錯誤");
			}
			else
			{
				var correctOptions = models.Where(item => item.Correct);
				if (correctOptions.Count() < minCorrectCounts || correctOptions.Count() > maxCorrectCounts)
				{
					ModelState.AddModelError("options", "正確選項數量錯誤");
				}
			}
			

		}

	}
}
