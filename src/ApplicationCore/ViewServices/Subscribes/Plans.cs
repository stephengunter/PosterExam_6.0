using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Views;
using AutoMapper;

namespace ApplicationCore.ViewServices
{
	public static class PlansViewService
	{

		public static PlanViewModel MapViewModel(this Plan plan, IMapper mapper, bool discount = false)
		{ 
		    var model = mapper.Map<PlanViewModel>(plan);
			if (discount) 
			{
				model.Price = Convert.ToInt32(plan.Money * plan.Discount / 100);
			}
			else model.Price = Convert.ToInt32(plan.Money);

			model.StartDateText = plan.StartDate.ToDateString();
			model.EndDateText = plan.EndDate.ToDateString();

			return model;
		}

		public static List<PlanViewModel> MapViewModelList(this IEnumerable<Plan> plans, IMapper mapper)
			=> plans.Select(item => MapViewModel(item, mapper)).ToList();

		
		public static Plan MapEntity(this PlanViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<PlanViewModel, Plan>(model);
			entity.Money = Convert.ToDecimal(model.Price);
			entity.StartDate = model.StartDateText.ToStartDate();
			entity.EndDate = model.EndDateText.ToEndDate();

			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			return entity;
		}

		public static IEnumerable<Plan> GetOrdered(this IEnumerable<Plan> plans)
			=> plans.HasItems() ? plans.OrderByDescending(item => item.StartDate)
								: new List<Plan>();

	}
}
