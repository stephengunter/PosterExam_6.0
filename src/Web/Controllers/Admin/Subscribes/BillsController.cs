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
	public class BillsController : BaseAdminController
	{
		private readonly IBillsService _billsService;
		private readonly IPlansService _plansService;
		//private readonly IPaysService _paysService;
		private readonly IMapper _mapper;

		public BillsController(IBillsService billsService, IPlansService plansService, IMapper mapper)
		{
			_billsService = billsService;
			_plansService = plansService;
			//_paysService = paysService;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int plan, int payed = 1, int page = 1, int pageSize = 10)
		{
			Plan planSelected = null;
			if (plan > 0)
			{
				planSelected = await _plansService.GetByIdAsync(plan);
				if (planSelected == null)
				{
					ModelState.AddModelError("plan", "方案不存在");
					return BadRequest(ModelState);
				}
			}

			var bills = await _billsService.FetchAllAsync();

			if (planSelected != null) bills = bills.Where(x => x.PlanId == planSelected.Id);

			bills = bills.Where(x => x.Payed == payed.ToBoolean());

			bills = bills.GetOrdered();


			if(page < 1) page = 1;
			return Ok(bills.GetPagedList(_mapper, page, pageSize));
		}


		[HttpGet("{id}")]
		public ActionResult Details(int id)
		{
			var bill = _billsService.GetById(id);
			if (bill == null) return NotFound();

			var model = bill.MapViewModel(_mapper, null);

			return Ok(model);
		}

		[HttpPut("clear/{plan}")]
		public async Task<ActionResult> Clear(int plan)
		{
			Plan planSelected = null;
			if (plan > 0)
			{
				planSelected = await _plansService.GetByIdAsync(plan);
				if (planSelected == null)
				{
					ModelState.AddModelError("plan", "方案不存在");
					return BadRequest(ModelState);
				}
			}

			var bills = await _billsService.FetchAllAsync();
			if (planSelected != null) bills = bills.Where(x => x.PlanId == planSelected.Id);

			var targetBills = bills.Where(x => x.TotalPayed == 0).Where(x => x.DeadLine.HasValue && DateTime.Now > x.DeadLine);
            foreach (var target in targetBills) _billsService.Remove(target);

			return Ok();
		}

	}
}
