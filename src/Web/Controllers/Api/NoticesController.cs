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
	public class NoticesController : BaseApiController
	{
		private readonly INoticesService _noticesService;
		private readonly IUsersService _usersService;
		private readonly IMapper _mapper;

		public NoticesController(INoticesService noticesService, IUsersService usersService, IMapper mapper)
		{
			_noticesService = noticesService;
			_usersService = usersService;
			_mapper = mapper;
		}
	

		[HttpGet("")]
		public async Task<ActionResult> Index(int page = 1, int pageSize = 10)
		{
			if (page < 1) page = 1;

			var notices = await _noticesService.FetchAsync();

			notices = notices.Where(x => x.Active);

			notices = notices.GetOrdered().ToList();

			return Ok(notices.GetPagedList(_mapper, page, pageSize));
		}


		[HttpGet("{id}/{user?}")]
		public async Task<ActionResult> Details(int id, string user = "")
		{
			var notice = await _noticesService.GetByIdAsync(id);
			if (notice == null) return NotFound();

			if (!notice.Active)
			{
				var existingUser = await _usersService.FindUserByIdAsync(user);
				if(existingUser == null) return NotFound();

				bool isAdmin = await _usersService.IsAdminAsync(existingUser);
				if(!isAdmin) return NotFound();
			}

			return Ok(notice.MapViewModel(_mapper));
		}



	}

	
}
