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
using Microsoft.AspNetCore.Http;

namespace Web.Controllers.Api
{
	
	public class MessagesController : BaseApiController
	{
		private readonly IHttpContextAccessor _accessor;
		private readonly IRecaptchaService _recaptchaService;
		private readonly IMessagesService _messagesService;
		private readonly IMapper _mapper;

		public MessagesController(IHttpContextAccessor accessor, IRecaptchaService recaptchaService,
			IMessagesService messagesService, IMapper mapper)
		{
			_accessor = accessor;
			_recaptchaService = recaptchaService;
			_messagesService = messagesService;
			_mapper = mapper;
		}


		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] MessageEditForm form)
		{
			if(!ModelState.IsValid) return BadRequest(ModelState);

			var ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
			bool recaptchaValid = await _recaptchaService.VerifyAsync(form.Token, ip);

			if (!recaptchaValid)
			{
				ModelState.AddModelError("recaptcha", "驗證失敗");
				return BadRequest(ModelState);
			}

			var message = form.Message.MapEntity(_mapper, CurrentUserId);
			message = await _messagesService.CreateAsync(message);

			return Ok(message.MapViewModel(_mapper));
		}

	}
}
