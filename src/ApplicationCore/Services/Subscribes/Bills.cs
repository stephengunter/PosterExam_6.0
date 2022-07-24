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
	public interface IBillsService
	{
		Task<IEnumerable<Bill>> FetchByUserAsync(string userId);
		Task<IEnumerable<Bill>> FetchByUserAsync(User user, Plan plan);
		Task<List<int>> FetchIdsByPlanAsync(Plan plan);
		Task<IEnumerable<Bill>> FetchAllAsync();
		Bill GetById(int id);
		Task UpdateAsync(Bill bill);
		Task<Bill> CreateAsync(Bill bill);
		void Remove(Bill bill);
	}

	public class BillsService : IBillsService
	{
		private readonly IDefaultRepository<Bill> _billRepository;

		public BillsService(IDefaultRepository<Bill> billRepository)
		{
			this._billRepository = billRepository;
		}

		public async Task<Bill> CreateAsync(Bill bill) => await _billRepository.AddAsync(bill);

		public async Task UpdateAsync(Bill bill) => await _billRepository.UpdateAsync(bill);

		public async Task<IEnumerable<Bill>> FetchByUserAsync(string userId)
		{
			var spec = new BillFilterSpecification(new User { Id = userId });
			return await _billRepository.ListAsync(spec);
		}

		public async Task<IEnumerable<Bill>> FetchByUserAsync(User user, Plan plan)
		{
			var spec = new BillFilterSpecification(user, plan);
			return await _billRepository.ListAsync(spec);
		}

		public async Task<List<int>> FetchIdsByPlanAsync(Plan plan)
		{
			var spec = new BillSimpleFilterSpecification(plan);
			var list = await _billRepository.ListAsync(spec);
			return list.Select(x => x.Id).ToList();
		}

		public async Task<IEnumerable<Bill>> FetchAllAsync() => await _billRepository.ListAsync(new BillFilterSpecification());

		public Bill GetById(int id)
		{
			var spec = new BillFilterSpecification(id);
			return _billRepository.GetSingleBySpec(spec);
		}

		public void Remove(Bill bill)
		{
			bill.Removed = true;

			foreach (var pay in bill.Pays)
			{
				pay.Removed = true;
			}

			_billRepository.Update(bill);
		}

		
	}
}
