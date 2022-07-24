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
	public interface ISubscribesService
	{
		Task<IEnumerable<Subscribe>> FetchAsync(bool active);
		Task<IEnumerable<Subscribe>> FetchByUserAsync(string userId);
		Task<IEnumerable<Subscribe>> FetchByPlanAsync(int planId);

		Task<IEnumerable<Subscribe>> FetchAllAsync();
		Subscribe GetById(int id);
		Task<Subscribe> CreateAsync(Subscribe subscribe);

		Task<Subscribe> FindCurrentSubscribeAsync(string userId);
		Subscribe Find(Bill bill);
		Task RemoveAsync(Subscribe subscribe);
		void SetEndDateMany(IEnumerable<Subscribe> subscribes, DateTime? endDate);
	}

	public class SubscribesService : ISubscribesService
	{
		private readonly IDefaultRepository<Subscribe> _subscribeRepository;

		public SubscribesService(IDefaultRepository<Subscribe> subscribeRepository)
		{
			this._subscribeRepository = subscribeRepository;
		}

		public async Task<Subscribe> CreateAsync(Subscribe subscribe) => await _subscribeRepository.AddAsync(subscribe);

		public async Task<IEnumerable<Subscribe>> FetchAsync(bool active)
		{
			var subscribes = await FetchAllAsync();
			if (subscribes.IsNullOrEmpty()) return null;

			return subscribes.Where(x => x.Active == active);
		}

		public async Task<IEnumerable<Subscribe>> FetchByUserAsync(string userId)
		{
			var spec = new SubscribeUserFilterSpecification(userId);
			return await _subscribeRepository.ListAsync(spec);
		}

		public async Task<IEnumerable<Subscribe>> FetchByPlanAsync(int planId)
		{
			var spec = new SubscribePlanFilterSpecification(planId);
			return await _subscribeRepository.ListAsync(spec);
		}

		public async Task<IEnumerable<Subscribe>> FetchAllAsync() => await _subscribeRepository.ListAsync(new SubscribeFilterSpecification());

		public Subscribe GetById(int id)
		{
			var spec = new SubscribeFilterSpecification(id);
			return _subscribeRepository.GetSingleBySpec(spec);
		}

		public async Task<Subscribe> FindCurrentSubscribeAsync(string userId)
		{
			var subscribes = await FetchByUserAsync(userId);

			return subscribes.HasItems() ? subscribes.Where(x => x.Active).FirstOrDefault() : null;
		}

		public Subscribe Find(Bill bill)
		{
			var spec = new SubscribeFilterSpecification(bill);
			return _subscribeRepository.GetSingleBySpec(spec);
		}

		public async Task RemoveAsync(Subscribe subscribe)
		{
			subscribe.Removed = true;
			await _subscribeRepository.UpdateAsync(subscribe);
		}

		public void SetEndDateMany(IEnumerable<Subscribe> subscribes, DateTime? endDate)
		{
            foreach (var subscribe in subscribes) subscribe.EndDate = endDate;

			_subscribeRepository.UpdateRange(subscribes);
		}
	}
}
