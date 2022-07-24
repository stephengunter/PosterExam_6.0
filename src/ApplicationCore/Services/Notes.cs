using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApplicationCore.Helpers;
using Infrastructure.DataAccess;

namespace ApplicationCore.Services
{
	public interface INotesService
	{
		Task<IEnumerable<Note>> FetchAsync(IList<int> termIds);
		Task<Note> GetByIdAsync(int id);
		Task<Note> CreateAsync(Note note);
		Task UpdateOrderAsync(int target, int replace, bool up);
		Task UpdateAsync(Note note);
		Task UpdateAsync(Note existingEntity, Note model);
		Task RemoveAsync(Note note);

		Task<IEnumerable<Note>> FetchAsync(BaseSpecification<Note> spec);
		Note GetById(int id);
		void LoadSubItems(IEnumerable<Note> list);
		Task LoadParentIdsAsync(Note note);

		Task<int> GetMaxOrderAsync(Term term, int parentId);

		List<int> ResolveSelectedIds(int[] selectedIds);
	}

	public class NotesService : BaseCategoriesService<Note>, INotesService
	{
		private readonly IDefaultRepository<Note> _noteRepository;

		public NotesService(IDefaultRepository<Note> noteRepository)
		{
			this._noteRepository = noteRepository;
		}

		public async Task<IEnumerable<Note>> FetchAsync(BaseSpecification<Note> spec) => await _noteRepository.ListAsync(spec);

		public async Task<IEnumerable<Note>> FetchAsync(IList<int> termIds) => await _noteRepository.ListAsync(new NoteTermFilterSpecification(termIds));

		
		public async Task<Note> GetByIdAsync(int id) => await _noteRepository.GetByIdAsync(id);

		public async Task<Note> CreateAsync(Note note) => await _noteRepository.AddAsync(note);

		public async Task UpdateAsync(Note note) => await _noteRepository.UpdateAsync(note);

		public async Task UpdateAsync(Note existingEntity, Note model) => await _noteRepository.UpdateAsync(existingEntity, model);

		public async Task UpdateOrderAsync(int target, int replace, bool up)
		{
			var targetEntity = await _noteRepository.GetByIdAsync(target);
			int targetOrder = targetEntity.Order;

			var replaceEntity = await _noteRepository.GetByIdAsync(replace);
			int replaceOrder = replaceEntity.Order;

			targetEntity.Order = replaceOrder;
			replaceEntity.Order = targetOrder;

			if (targetEntity.Order == replaceEntity.Order)
			{
				if (up) replaceEntity.Order += 1;
				else targetEntity.Order += 1;
			}

			_noteRepository.UpdateRange(new List<Note> { targetEntity, replaceEntity });
		}

		public async Task RemoveAsync(Note note)
		{
			note.Removed = true;
			await _noteRepository.UpdateAsync(note);
		}

		public Note GetById(int id)
		{
			var note = _noteRepository.GetById(id);
			if (note == null) return null;

			LoadSubItems(note);

			return note;

		}
		

		public async Task LoadParentIdsAsync(Note note) => note.LoadParentIds(await _noteRepository.ListAllAsync());

		public async Task<int> GetMaxOrderAsync(Term term, int parentId)
		{
			var spec = new NoteTermFilterSpecification(term, parentId);
			var list = await _noteRepository.ListAsync(spec);

			if (list.IsNullOrEmpty()) return 0;
			return list.Max(item => item.Order);
		}

		public List<int> ResolveSelectedIds(int[] selectedIds) => ResolveSelectedIds(selectedIds, _noteRepository.DbSet);

		public void LoadSubItems(IEnumerable<Note> list)
		{
			if (list.IsNullOrEmpty()) return;

			var subItems = AllSubItems(_noteRepository.DbSet);


			foreach (var entity in list)
			{
				entity.LoadSubItems(subItems.ToList());
			}
		}

		void LoadSubItems(Note entity)
		{
			var subItems = AllSubItems(_noteRepository.DbSet);

			entity.LoadSubItems(subItems.ToList());
		}

		
	}
}
