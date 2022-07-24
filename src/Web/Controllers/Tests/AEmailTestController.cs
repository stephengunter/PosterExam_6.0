using ApplicationCore.Helpers;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using ApplicationCore.ViewServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Web.Controllers.Tests
{
	public class AEmailTestController : BaseTestController
	{
		private readonly AppSettings _appSettings;
		private readonly AdminSettings _adminSettings;
		private readonly IMailService _mailService;
		private readonly IWebHostEnvironment _environment;
		public AEmailTestController(IOptions<AppSettings> appSettings, IOptions<AdminSettings> adminSettings, 
			IMailService mailService, IWebHostEnvironment environment)
		{
			_appSettings = appSettings.Value;
			_adminSettings = adminSettings.Value;

			_mailService = mailService;
			_environment = environment;
		}

		

		[HttpPost]
		public async Task<ActionResult> Test([FromBody] AdminRequest model)
		{
			if (model.Key != _adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");
			if (string.IsNullOrEmpty(model.Cmd)) ModelState.AddModelError("cmd", "指令錯誤");
			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (model.Cmd.EqualTo("basic-email"))
			{
				string email = _adminSettings.Email;
				// send email
				string mailSubject = "Test Subject";
				string mailContent = "Test Content";

				await _mailService.SendAsync(email, mailSubject, mailContent);
			}
			else if(model.Cmd.EqualTo("subscribe-email"))
			{
				//create private notice
				string content = GetMailTemplate(_environment, _appSettings, Web.Consts.MailTemplates.Subscribe);				

				string email = _adminSettings.Email;
				// send email
				string mailSubject = "您的訂閱會員已經生效";
				string mailTemplate = GetMailTemplate(_environment, _appSettings);
				string mailContent = mailTemplate.Replace(Web.Consts.MAIL_CONTENT, content);

				await _mailService.SendAsync(email, mailSubject, mailContent);

			}
			else
			{
				ModelState.AddModelError("cmd", "指令錯誤");
				return BadRequest(ModelState);
			}

			return Ok($"{model.Cmd} - OK");

		}


	}

}