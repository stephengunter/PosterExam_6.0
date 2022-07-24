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
	public interface IResolvesService
	{
		Task<IEnumerable<Resolve>> FetchAsync(int questionId = 0);
		Task<Resolve> GetByIdAsync(int id);
		Task<Resolve> CreateAsync(Resolve resolve);
		Task UpdateAsync(Resolve resolve);
		Task UpdateAsync(Resolve existingEntity, Resolve resolve);
		Task RemoveAsync(Resolve resolve);

		Task<IEnumerable<Resolve>> FetchExamResolvesAsync(Exam exam);
	}

	public class ResolvesService : IResolvesService
	{
		private readonly IDefaultRepository<Resolve> _resolveRepository;

		public ResolvesService(IDefaultRepository<Resolve> resolveRepository)
		{
			this._resolveRepository = resolveRepository;
		}

		public async Task<IEnumerable<Resolve>> FetchAsync(int questionId = 0)
		{
			if (questionId > 0)
			{
				return await _resolveRepository.ListAsync(new ResolveFilterSpecification(questionId));
			}
			return await _resolveRepository.ListAsync(new ResolveFilterSpecification());
		}

		public async Task<Resolve> GetByIdAsync(int id) => await _resolveRepository.GetByIdAsync(id);

		public async Task<Resolve> CreateAsync(Resolve resolve) => await _resolveRepository.AddAsync(resolve);

		public async Task UpdateAsync(Resolve resolve) => await _resolveRepository.UpdateAsync(resolve);

		public async Task UpdateAsync(Resolve existingEntity, Resolve resolve) => await _resolveRepository.UpdateAsync(existingEntity, resolve);

		public async Task RemoveAsync(Resolve resolve)
		{
			resolve.Removed = true;
			await _resolveRepository.UpdateAsync(resolve);
		}

		public async Task<IEnumerable<Resolve>> FetchExamResolvesAsync(Exam exam)
			=> await _resolveRepository.ListAsync(new ResolveFilterSpecification(exam));

	}
}
