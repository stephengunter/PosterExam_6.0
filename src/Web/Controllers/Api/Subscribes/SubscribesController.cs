using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.Api
{
	[Authorize]
	public class SubscribesController : BaseApiController
	{
		private readonly Web.Services.ISubscribesService _subscribesService;

		public SubscribesController(Web.Services.ISubscribesService subscribesService)
		{
			_subscribesService = subscribesService;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			bool allData = true;
			var model = await _subscribesService.GetSubscribesIndexViewAsync(CurrentUserId, allData);

			return Ok(model);
		}

		[HttpGet("create")]
		public async Task<ActionResult> Create()
		{

			var form = await _subscribesService.GetCreateBillFormAsync(CurrentUserId);

			return Ok(form);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] BillViewModel model)
		{
			var createdBillView = await _subscribesService.StoreBillAsync(model, CurrentUserId);

			return Ok(createdBillView);
		}

		
	}

	
}
