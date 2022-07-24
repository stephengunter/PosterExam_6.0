

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
using ApplicationCore.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;
using ApplicationCore.Hubs;
using Hangfire;
using ApplicationCore.Exceptions;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System;
using ApplicationCore.Logging;

namespace Web.Controllers.Tests
{
	public class APaysTestController : BaseTestController
	{
		private readonly IAppLogger _logger;
		private readonly IWebHostEnvironment _environment;
		private readonly AppSettings _appSettings;
		private readonly AdminSettings _adminSettings;
		private readonly Web.Services.ISubscribesService _webSubscribesService;
		private readonly INoticesService _noticesService;
		private readonly IMailService _mailService;

		private readonly IHubContext<NotificationsHub> _notificationHubContext;
		private readonly IHubConnectionManager _userConnectionManager;
		public APaysTestController(IAppLogger logger, IWebHostEnvironment environment, IOptions<AppSettings> appSettings, IOptions<AdminSettings> adminSettings,
			Web.Services.ISubscribesService webSubscribesService, INoticesService noticesService, IMailService mailService,
			IHubContext<NotificationsHub> notificationHubContext, IHubConnectionManager userConnectionManager)
		{
			_logger = logger;
			_environment = environment;
			_appSettings = appSettings.Value;
			_adminSettings = adminSettings.Value;
			_webSubscribesService = webSubscribesService;
			_noticesService = noticesService;
			_mailService = mailService;

			_notificationHubContext = notificationHubContext;
			_userConnectionManager = userConnectionManager;
		}

		

		[HttpPost]
		public async Task<ActionResult> Test([FromBody] AdminRequest model)
		{
			if (model.Key != _adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");
			if (string.IsNullOrEmpty(model.Cmd)) ModelState.AddModelError("cmd", "指令錯誤");
			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (model.Cmd.EqualTo("fake-pay"))
			{
				var tradeResultModel = JsonConvert.DeserializeObject<TradeResultModel>(model.Data);

				var subscribe = await _webSubscribesService.StorePayAsync(tradeResultModel);
				if (subscribe != null)
				{
					//付款訂閱完成
					BackgroundJob.Enqueue(() => NotifyUserAsync(subscribe.UserId));
				}

				return Ok("1|OK");
			}
			else
			{
				ModelState.AddModelError("cmd", "指令錯誤");
				return BadRequest(ModelState);
			}
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task NotifyUserAsync(string userId)
		{

			try
			{
				var user = await _webSubscribesService.FindUserByIdAsync(userId);
				if (user == null) throw new UserNotFoundException(userId);

				//create private notice
				string content = GetMailTemplate(_environment, _appSettings, Web.Consts.MailTemplates.Subscribe);
				var notice = new Notice
				{
					Title = "您的訂閱會員已經生效",
					Content = content
				};
				await _noticesService.CreateUserNotificationAsync(notice, new List<string> { userId });

				// send email
				string mailSubject = notice.Title;
				string mailTemplate = GetMailTemplate(_environment, _appSettings);
				string mailContent = mailTemplate.Replace(Web.Consts.MAIL_CONTENT, content);

				await _mailService.SendAsync(user.Email, notice.Title, mailContent);


				// send hub notification
				var connections = _userConnectionManager.GetUserConnections(userId);
				if (connections.HasItems())
				{
					foreach (var connectionId in connections)
					{
						await _notificationHubContext.Clients.Client(connectionId).SendAsync(Web.Consts.Notifications, userId);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
			}

		}

	}

}