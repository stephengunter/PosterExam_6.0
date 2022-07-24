using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.Api
{
	[Authorize]
	public class NotificationsController : BaseApiController
	{
		private readonly INoticesService _noticesService;
		private readonly IMapper _mapper;

		public NotificationsController(INoticesService noticesService, IMapper mapper)
		{
			_noticesService = noticesService;
			_mapper = mapper;
		}
	

		[HttpGet("")]
		public async Task<ActionResult> Index(int page = 1, int pageSize = 99)
		{
			if (page < 1) return await NotificationsAsync();


			var notifications = await _noticesService.FetchUserNotificationsAsync(CurrentUserId);
			notifications = notifications.GetOrdered();

			return Ok(notifications.GetPagedList(_mapper, page, pageSize));
		}

		async Task<ActionResult> NotificationsAsync()
		{
			var notifications = await _noticesService.FetchUserNotificationsAsync(CurrentUserId);
			// 只要未讀的
			notifications = notifications.Where(item => !item.HasReceived);
			
			return Ok(notifications.GetPagedList(_mapper, 1, 50));
		}

		[HttpGet("{id}")]
		public ActionResult Details(int id)
		{
			var notification = _noticesService.GetUserNotificationById(id);
			if (notification == null) return NotFound();
			if (notification.UserId != CurrentUserId) return NotFound();

			return Ok(notification.MapViewModel(_mapper));
		}

		[HttpPost]
		public async Task<ActionResult> Clear([FromBody] CommonRequestViewModel model)
		{
			
			var idList = model.Data.SplitToIds();
			if (idList.HasItems())
			{
				await _noticesService.ClearUserNotificationsAsync(CurrentUserId, idList);
			}

			return await NotificationsAsync();
		}

	}

	
}
