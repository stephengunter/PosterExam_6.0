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
	public class PlansController : BaseAdminController
	{
		private readonly IPlansService _plansService;
		private readonly ISubscribesService _subscribesService;
		private readonly IUsersService _usersService;
		private readonly IMapper _mapper;

		public PlansController(IPlansService plansService, ISubscribesService subscribesService, IUsersService usersService, IMapper mapper)
		{
			_plansService = plansService;
			_subscribesService = subscribesService;
			_usersService = usersService;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int active = -1)
		{
			if (active < 0)
			{
				var allPlans = await _plansService.FetchAllAsync();
				allPlans = allPlans.GetOrdered();
				return Ok(allPlans.MapViewModelList(_mapper));
			}
			else {
				bool isAcive = active.ToBoolean();
				var plans = await _plansService.FetchAsync(isAcive);
				plans = plans.GetOrdered();
				return Ok(plans.MapViewModelList(_mapper));
			}
			
		}

		[HttpGet("create")]
		public ActionResult Create()
		{
			var model = new PlanViewModel
			{
				Price = 360,
				Discount = 50,
				Description = "<ul><li>第二次訂閱者半價優惠</li></ul>"
			};
			return Ok(model);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] PlanViewModel model)
		{
			var plan = model.MapEntity(_mapper, CurrentUserId);
			plan.Discount = 50;   //固定五折

			await ValidateAsync(plan);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			plan = await _plansService.CreateAsync(plan);

			return Ok(plan.Id);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var plan = await _plansService.GetByIdAsync(id);
			if (plan == null) return NotFound();

			var model = plan.MapViewModel(_mapper);
			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] PlanViewModel model)
		{
			var existingEntity = await _plansService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			var plan = model.MapEntity(_mapper, CurrentUserId);
			plan.Discount = 50;   //固定五折

			await ValidateAsync(plan);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			await _plansService.UpdateAsync(existingEntity, plan);

			var subscribes = await _subscribesService.FetchByPlanAsync(plan.Id);
			if(subscribes.HasItems()) _subscribesService.SetEndDateMany(subscribes, plan.EndDate);


			return Ok();
		}

		[HttpPut("clear/{id}")]
		public async Task<ActionResult> Clear(int id)
		{
			var plan = await _plansService.GetByIdAsync(id);
			if (plan == null) return NotFound();

			if (!plan.CanClear)
			{
				ModelState.AddModelError("canClear", "此方案無法結算");
				return BadRequest(ModelState);
			}

			var subscribes = await _subscribesService.FetchByPlanAsync(plan.Id);
			if (subscribes.HasItems())
			{
				var userIds = subscribes.Select(x => x.UserId);
                foreach (var userId in userIds)
                {
					await _usersService.RemoveSubscriberRoleAsync(userId);
				}
			}

			plan.ClearDate = DateTime.Now;
			await _plansService.UpdateAsync(plan);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var plan = await _plansService.GetByIdAsync(id);
			if (plan == null) return NotFound();

			if (plan.Active)
			{
				ModelState.AddModelError("endDate", "方案上架中無法刪除");
				return BadRequest(ModelState);
			}

			plan.SetUpdated(CurrentUserId);
			await _plansService.RemoveAsync(plan);

			return Ok();
		}

		async Task ValidateAsync(Plan plan)
		{
			if (!plan.StartDate.HasValue)
			{
				ModelState.AddModelError("startDate", "必須填寫開始日期");
			}
			else
			{
				if (plan.EndDate.HasValue)
				{
					if (plan.EndDate <= plan.StartDate)
					{
						ModelState.AddModelError("endDate", "結束日期錯誤");
					}
					else
					{
						var existingPlans = (await _plansService.FetchAllAsync()).Where(x => x.Id != plan.Id).ToList();
						var hasDateConflict = existingPlans.Where(x => x.HasDateConflict(plan));

						if (hasDateConflict.HasItems())
						{
							ModelState.AddModelError("endDate", "日期與其他方案衝突");
						}
					}
				}

			}
			if (plan.Money < 150 || plan.Discount > 500)
			{
				ModelState.AddModelError("money", "金額錯誤");
			}
			if (plan.Discount < 50 || plan.Discount > 95)
			{
				ModelState.AddModelError("discount", "折扣錯誤");
			}
			
		}

	}
}
