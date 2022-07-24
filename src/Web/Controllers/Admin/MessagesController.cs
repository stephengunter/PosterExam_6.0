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
using ApplicationCore.Specifications;
using Web.Helpers;
using Web.Controllers;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;

namespace Web.Controllers.Admin
{
	public class MessagesController : BaseAdminController
	{
		private readonly IWebHostEnvironment _environment;
		private readonly AppSettings _appSettings;

		private readonly IMessagesService _messagesService;
		private readonly IMailService _mailService;
		private readonly IMapper _mapper;

		public MessagesController(IWebHostEnvironment environment, IOptions<AppSettings> appSettings, 
			IMessagesService messagesService, IMailService mailService, IMapper mapper)
		{
			_environment = environment;
			_appSettings = appSettings.Value;

			_messagesService = messagesService;
			_mailService = mailService;
			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index(int status = 0, string start = "", string end = "", int page = 1, int pageSize = 10)
		{
			if (page < 1) page = 1;

			var messages = await _messagesService.FetchAsync(status.ToBoolean());

			if (start.HasValue() || end.HasValue())
			{
				var startDate = start.ToStartDate();
				if (!startDate.HasValue) startDate = DateTime.MinValue;

				var endDate = end.ToEndDate();
				if (!endDate.HasValue) endDate = DateTime.MaxValue;


				messages = messages.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);
			}

			messages = messages.GetOrdered().ToList();

			return Ok(messages.GetPagedList(_mapper, page, pageSize));
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var message = await _messagesService.GetByIdAsync(id);
			if (message == null) return NotFound();

			var model = message.MapViewModel(_mapper);

			if (!(message.Returned))
			{
				if (String.IsNullOrEmpty(model.ReturnContentView.Template))
				{
					model.ReturnContentView.Template = GetMailTemplate(_environment, _appSettings);
				}
					
			}

			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] BaseMessageViewModel model)
		{
			var message = await _messagesService.GetByIdAsync(id);
			if (message == null) return NotFound();
			

			if (!model.Draft)
			{
				ValidateRequest(model);
				if (!ModelState.IsValid) return BadRequest(ModelState);

				//發送email, 如果失敗則拋出例外
				await _mailService.SendAsync(message.Email, model.Subject, model.Content, model.Text);


				//如果發送成功, 狀態為已回覆
				message.Returned = true;

			}

			model.UpdatedBy = CurrentUserId;
			model.LastUpdated = DateTime.Now;

			message.ReturnContent = JsonConvert.SerializeObject(model);
			await _messagesService.UpdateAsync(message);

			return Ok();
		}

		void ValidateRequest(BaseMessageViewModel model)
		{
			if (String.IsNullOrEmpty(model.Subject)) ModelState.AddModelError("subject", "必須填寫主旨");

			if (String.IsNullOrEmpty(model.Content)) ModelState.AddModelError("content", "必須填寫內容");
			
		}


	}
}
