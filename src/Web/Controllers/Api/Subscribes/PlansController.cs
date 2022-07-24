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
using ApplicationCore.Logging;
using ApplicationCore.Exceptions;

namespace Web.Controllers.Api
{
	public class PlansController : BaseApiController
	{
		private readonly Web.Services.ISubscribesService _subscribesService;
		private readonly IMapper _mapper;

		public PlansController(Web.Services.ISubscribesService subscribesService, IMapper mapper)
		{
			_subscribesService = subscribesService;
			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var plan = await _subscribesService.FindActivePlanAsync();
			if (plan == null) return Ok();

			return Ok(plan.MapViewModel(_mapper));
			
		}

	}

	
}
