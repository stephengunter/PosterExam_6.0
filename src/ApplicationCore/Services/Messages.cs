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
	public interface IMessagesService
	{
		Task<IEnumerable<Message>> FetchAsync(bool returned);
		Task<Message> GetByIdAsync(int id);
		Task<Message> CreateAsync(Message message);

		Task<IEnumerable<Message>> FetchAllAsync();
		Task UpdateAsync(Message message);
		Task UpdateAsync(Message existingEntity, Message model);
		Task RemoveAsync(Message message);

		
	}

	public class MessagesService : IMessagesService
	{
		private readonly IDefaultRepository<Message> _messageRepository;

		public MessagesService(IDefaultRepository<Message> messageRepository)
		{
			this._messageRepository = messageRepository;
		}

		
		public async Task<IEnumerable<Message>> FetchAsync(bool returned)
			=> await _messageRepository.ListAsync(new MessageFilterSpecification(returned));

		public async Task<IEnumerable<Message>> FetchAllAsync()
			=> await _messageRepository.ListAsync(new MessageFilterSpecification());

		public async Task<Message> GetByIdAsync(int id) => await _messageRepository.GetByIdAsync(id);
		
		public async Task<Message> CreateAsync(Message message) => await _messageRepository.AddAsync(message);

		public async Task UpdateAsync(Message message) => await _messageRepository.UpdateAsync(message);

		public async Task UpdateAsync(Message existingEntity, Message model) => await _messageRepository.UpdateAsync(existingEntity, model);

		
		public async Task RemoveAsync(Message message)
		{
			message.Removed = true;
			await _messageRepository.UpdateAsync(message);
		}

		
	}
}
