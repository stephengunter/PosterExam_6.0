using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApplicationCore.Helpers;

namespace ApplicationCore.Services
{
	public interface IPlansService
	{
		Task<IEnumerable<Plan>> FetchAsync(bool active);
		Task<IEnumerable<Plan>> FetchAllAsync();
		Task<Plan> GetByIdAsync(int id);
		Task<Plan> CreateAsync(Plan plan);
		Task UpdateAsync(Plan plan);
		Task UpdateAsync(Plan existingEntity, Plan model);
		Task RemoveAsync(Plan plan);
	}

	public class PlansService : IPlansService
	{
		private readonly IDefaultRepository<Plan> _planRepository;

		public PlansService(IDefaultRepository<Plan> planRepository)
		{
			this._planRepository = planRepository;
		}

		public async Task<Plan> CreateAsync(Plan plan) => await _planRepository.AddAsync(plan);

		public async Task<IEnumerable<Plan>> FetchAsync(bool active)
		{
			var plans = await FetchAllAsync();
			if (plans.IsNullOrEmpty()) return null;

			return plans.Where(x => x.Active == active);
		}


		public async Task<IEnumerable<Plan>> FetchAllAsync() => await _planRepository.ListAsync(new PlanFilterSpecification());

		public async Task<Plan> GetByIdAsync(int id) => await _planRepository.GetByIdAsync(id);

		public async Task RemoveAsync(Plan plan)
		{
			plan.Removed = true;
			await _planRepository.UpdateAsync(plan);
		}

		public async Task UpdateAsync(Plan plan) => await _planRepository.UpdateAsync(plan);

		public async Task UpdateAsync(Plan existingEntity, Plan model) => await _planRepository.UpdateAsync(existingEntity, model);
	}
}
