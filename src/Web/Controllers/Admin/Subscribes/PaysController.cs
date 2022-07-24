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
	public class PaysController : BaseAdminController
	{
		private readonly IPaysService _paysService;
		private readonly IPlansService _plansService;
		private readonly IBillsService _billsService;
		private readonly IMapper _mapper;

		public PaysController(IPaysService paysService, IPlansService plansService, IBillsService billsService, IMapper mapper)
		{
			_paysService = paysService;
			_plansService = plansService;
			_billsService = billsService;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int plan = 0, string start = "", string end = "", int page = 1, int pageSize = 10)
		{
            var model = new PaysAdminModel();

			if (page < 0) //首次載入
			{
				page = 1;
				var payways = await _paysService.FetchAllPayWaysAsync();
				payways = payways.GetOrdered();
				model.Payways = payways.MapViewModelList(_mapper);
			}

			IEnumerable<Pay> pays;
			if (plan > 0)
			{
				var planSelected = await _plansService.GetByIdAsync(plan);
				if (planSelected == null)
				{
					ModelState.AddModelError("plan", "方案不存在");
					return BadRequest(ModelState);
				}

				var billIds = await _billsService.FetchIdsByPlanAsync(planSelected);
				if (billIds.HasItems()) pays = await _paysService.FetchByBillAsync(billIds);
				else pays = new List<Pay>();
			}
			else
			{
				pays = await _paysService.FetchAllAsync();
			}

			if (start.HasValue() || end.HasValue())
			{
				var startDate = start.ToStartDate();
				if (!startDate.HasValue) startDate = DateTime.MinValue;

				var endDate = end.ToEndDate();
				if (!endDate.HasValue) endDate = DateTime.MaxValue;


				pays = pays.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);
			}



			pays = pays.GetOrdered();

            model.PagedList = pays.GetPagedList(_mapper, null ,page, pageSize);
            return Ok(model);
		}

		[HttpGet("{id}")]
		public ActionResult Details(int id)
		{
			var pay = _paysService.GetById(id);
			if (pay == null) return NotFound();

			var model = pay.MapViewModel(_mapper, null);

			return Ok(model);
		}

	}
}
