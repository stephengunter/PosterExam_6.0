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
	public class SubscribesController : BaseAdminController
	{
		private readonly ISubscribesService _subscribesService;
		private readonly IPlansService _plansService;
		private readonly IMapper _mapper;

		public SubscribesController(ISubscribesService subscribesService, IPlansService plansService, IMapper mapper)
		{
			_subscribesService = subscribesService;
			_plansService = plansService;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int plan, int page = 1, int pageSize = 10)
		{
			Plan planSelected = null;
			if (plan > 0) planSelected = await _plansService.GetByIdAsync(plan);

			if (planSelected == null)
			{
				ModelState.AddModelError("plan", "方案不存在");
				return BadRequest(ModelState);
			}

			var subscribes = await _subscribesService.FetchByPlanAsync(plan);
			subscribes = subscribes.GetOrdered();

			if (page < 1) page = 1;
			return Ok(subscribes.GetPagedList(_mapper, page, pageSize));
		}

	}
}
