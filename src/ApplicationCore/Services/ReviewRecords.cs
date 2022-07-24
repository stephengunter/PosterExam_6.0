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
	public interface IReviewRecordsService
	{
		Task<ReviewRecord> GetByIdAsync(int id);
		Task<ReviewRecord> CreateAsync(ReviewRecord record);
		Task UpdateAsync(ReviewRecord record);
		Task UpdateAsync(ReviewRecord existingEntity, ReviewRecord record);
		Task RemoveAsync(ReviewRecord record);

	}

	public class ReviewRecordsService : IReviewRecordsService
	{
		private readonly IDefaultRepository<ReviewRecord> _recordRepository;

		public ReviewRecordsService(IDefaultRepository<ReviewRecord> recordRepository)
		{
			this._recordRepository = recordRepository;
		}


		public async Task<ReviewRecord> GetByIdAsync(int id) => await _recordRepository.GetByIdAsync(id);

		public async Task<ReviewRecord> CreateAsync(ReviewRecord record) => await _recordRepository.AddAsync(record);

		public async Task UpdateAsync(ReviewRecord record) => await _recordRepository.UpdateAsync(record);

		public async Task UpdateAsync(ReviewRecord existingEntity, ReviewRecord record) => await _recordRepository.UpdateAsync(existingEntity, record);

		public async Task RemoveAsync(ReviewRecord record)
		{
			record.Removed = true;
			await _recordRepository.UpdateAsync(record);
		}

	}
}
