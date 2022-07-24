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
	public interface INoticesService
	{
		Task<IEnumerable<Notice>> FetchAsync(bool isPublic = true);
		Task<Notice> GetByIdAsync(int id);
		Task<Notice> CreateAsync(Notice notice);

		Task<Notice> CreateUserNotificationAsync(Notice notice, IEnumerable<string> userIds);
		Task<IEnumerable<Receiver>> FetchUserNotificationsAsync(string userId);
		Task ClearUserNotificationsAsync(string userId, IEnumerable<int> ids);
		Receiver GetUserNotificationById(int id);

		Task<IEnumerable<Notice>> FetchAllAsync();
		Task UpdateAsync(Notice notice);
		Task UpdateAsync(Notice existingEntity, Notice model);
		void UpdateMany(IEnumerable<Notice> notices);
		Task RemoveAsync(Notice notice);

		
	}

	public class NoticesService : INoticesService
	{
		private readonly IDefaultRepository<Notice> _noticeRepository;
		private readonly IDefaultRepository<Receiver> _receiverRepository;

		public NoticesService(IDefaultRepository<Notice> noticeRepository, IDefaultRepository<Receiver> receiverRepository)
		{
			_noticeRepository = noticeRepository;
			_receiverRepository = receiverRepository;
		}

		
		public async Task<IEnumerable<Notice>> FetchAsync(bool isPublic = true)
			=> await _noticeRepository.ListAsync(new NoticeFilterSpecification(isPublic));

		public async Task<IEnumerable<Notice>> FetchAllAsync()
			=> await _noticeRepository.ListAsync(new NoticeFilterSpecification());

		public async Task<Notice> GetByIdAsync(int id) => await _noticeRepository.GetByIdAsync(id);
		
		public async Task<Notice> CreateAsync(Notice notice) => await _noticeRepository.AddAsync(notice);

		public async Task<Notice> CreateUserNotificationAsync(Notice notice, IEnumerable<string> userIds)
		{
			notice.Public = false;
			foreach (var userId in userIds)
			{
				notice.Receivers.Add(new Receiver
				{
					UserId = userId
				});
			}
			
			return await _noticeRepository.AddAsync(notice);
		}

		public async Task<IEnumerable<Receiver>> FetchUserNotificationsAsync(string userId)
			=> await _receiverRepository.ListAsync(new ReceiverFilterSpecification(userId));

		public async Task ClearUserNotificationsAsync(string userId, IEnumerable<int> ids)
		{
			var receivers = await FetchUserNotificationsAsync(userId);
			var items = receivers.Where(item => ids.Contains(item.Id)).ToList();

			if (items.HasItems())
			{
				foreach (var item in items) item.ReceivedAt = DateTime.Now;
				_receiverRepository.UpdateRange(items);
			} 
		}

		public Receiver GetUserNotificationById(int id)
			=> _receiverRepository.GetSingleBySpec(new ReceiverFilterSpecification(id));



		public async Task UpdateAsync(Notice notice) => await _noticeRepository.UpdateAsync(notice);

		public async Task UpdateAsync(Notice existingEntity, Notice model) => await _noticeRepository.UpdateAsync(existingEntity, model);

		public void UpdateMany(IEnumerable<Notice> notices) => _noticeRepository.UpdateRange(notices);


		public async Task RemoveAsync(Notice notice)
		{
			
			notice.Removed = true;
			await _noticeRepository.UpdateAsync(notice);
		}

		
	}
}
