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
	public interface ITermsService
	{
		Task<IEnumerable<Term>> FetchAllAsync();
		Task<IEnumerable<Term>> FetchAsync(Subject subject, int parentId = -1);
		Task<Term> GetByIdAsync(int id);
		Task<Term> CreateAsync(Term term);
		Task UpdateAsync(Term term);
		Task UpdateAsync(Term existingEntity, Term model);
		Task RemoveAsync(Term term);

		Task<IEnumerable<Term>> FetchAsync(BaseSpecification<Term> spec);
		Term GetById(int id);
		void LoadSubItems(IEnumerable<Term> list);
		Task LoadParentIdsAsync(Term term);

		//IEnumerable<Term> FilterByKeyword(ICollection<string> keywords);

		Task<int> GetMaxOrderAsync(Subject subject, int parentId);

		List<int> ResolveSelectedIds(int[] selectedIds);
	}

	public class TermsService : BaseCategoriesService<Term>, ITermsService
	{
		private readonly IDefaultRepository<Term> _termRepository;

		public TermsService(IDefaultRepository<Term> termRepository)
		{
			this._termRepository = termRepository;
		}

		public async Task<IEnumerable<Term>> FetchAsync(BaseSpecification<Term> spec) => await _termRepository.ListAsync(spec);

		public async Task<IEnumerable<Term>> FetchAllAsync() => await _termRepository.ListAsync(new TermFilterSpecification());
		
		public async Task<IEnumerable<Term>> FetchAsync(Subject subject, int parentId = -1)
		{
			TermFilterSpecification spec;
			if (parentId >= 0) spec = new TermFilterSpecification(subject, parentId);
			else spec = new TermFilterSpecification(subject);

			return await _termRepository.ListAsync(spec);
		}

		public async Task<Term> GetByIdAsync(int id) => await _termRepository.GetByIdAsync(id);

		public async Task<Term> CreateAsync(Term term) => await _termRepository.AddAsync(term);

		public async Task UpdateAsync(Term term) => await _termRepository.UpdateAsync(term);

		public async Task UpdateAsync(Term existingEntity, Term model) => await _termRepository.UpdateAsync(existingEntity, model);

		public async Task RemoveAsync(Term term)
		{
			term.Removed = true;
			await _termRepository.UpdateAsync(term);
		}

		public Term GetById(int id)
		{
			var term = _termRepository.GetSingleBySpec(new TermFilterSpecification(id));
			if (term == null) return null;

			LoadSubItems(term);

			return term;

		}
		

		public async Task LoadParentIdsAsync(Term term) => term.LoadParentIds(await _termRepository.ListAllAsync());

		public async Task<int> GetMaxOrderAsync(Subject subject, int parentId)
		{
			var spec = new TermFilterSpecification(subject, parentId);
			var list = await _termRepository.ListAsync(spec);

			if (list.IsNullOrEmpty()) return 0;
			return list.Max(item => item.Order);
		}

		public List<int> ResolveSelectedIds(int[] selectedIds) => ResolveSelectedIds(selectedIds, _termRepository.DbSet);

		public void LoadSubItems(IEnumerable<Term> list)
		{
			if (list.IsNullOrEmpty()) return;

			var subItems = AllSubItems(_termRepository.DbSet);


			foreach (var entity in list)
			{
				entity.LoadSubItems(subItems.ToList());
			}
		}

		void LoadSubItems(Term entity)
		{
			var subItems = AllSubItems(_termRepository.DbSet);

			entity.LoadSubItems(subItems.ToList());
		}

		
	}
}
