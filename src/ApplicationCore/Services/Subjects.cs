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
	public interface ISubjectsService
	{
		Task<IEnumerable<Subject>> FetchAsync(int parentId = -1);
		Task<Subject> GetByIdAsync(int id);
		Task<Subject> CreateAsync(Subject subject);
		Task UpdateAsync(Subject subject);
		Task UpdateAsync(Subject existingEntity, Subject model);
		Task RemoveAsync(Subject subject);

		Task<IEnumerable<Subject>> FetchRootSubjectsAsync();
		IEnumerable<Subject> FetchRootItems();
		Task<IEnumerable<Subject>> FetchExamSubjectsAsync();
		void LoadSubItems(Subject entity, bool activeOnly = false);
		void LoadSubItems(IEnumerable<Subject> list, bool activeOnly = false);
		Subject GetById(int id);
	}

	public class SubjectsService : BaseCategoriesService<Subject>, ISubjectsService
	{
		private readonly IDefaultRepository<Subject> _subjectRepository;

		public SubjectsService(IDefaultRepository<Subject> subjectRepository)
		{
			this._subjectRepository = subjectRepository;
		}
		

		public async Task<IEnumerable<Subject>> FetchAsync(int parentId = -1)
		{
			SubjectFilterSpecification spec;
			if (parentId >= 0) spec = new SubjectFilterSpecification(parentId);
			else spec = new SubjectFilterSpecification();


			return await _subjectRepository.ListAsync(spec);
		}

		public async Task<IEnumerable<Subject>> FetchRootSubjectsAsync() => await FetchAsync(0);

		public async Task<Subject> GetByIdAsync(int id) => await _subjectRepository.GetByIdAsync(id);

		public async Task<Subject> CreateAsync(Subject subject) => await _subjectRepository.AddAsync(subject);

		public async Task UpdateAsync(Subject subject) => await _subjectRepository.UpdateAsync(subject);

		public async Task UpdateAsync(Subject existingEntity, Subject model) => await _subjectRepository.UpdateAsync(existingEntity, model);

		public async Task RemoveAsync(Subject subject)
		{
			subject.Removed = true;
			await _subjectRepository.UpdateAsync(subject);
		}

		public Subject GetById(int id)
		{
			var subject = _subjectRepository.GetById(id);
			if (subject == null) return null;

			LoadSubItems(subject);

			return subject;

		}


		public IEnumerable<Subject> FetchRootItems() => AllRootItems(_subjectRepository.DbSet);

		public async Task<IEnumerable<Subject>> FetchExamSubjectsAsync()
		{
			int parentId = 0;
			var spec = new SubjectFilterSpecification(parentId);
			return await _subjectRepository.ListAsync(spec);
		}

		public void LoadSubItems(IEnumerable<Subject> list, bool activeOnly = false)
		{
			if (list.IsNullOrEmpty()) return;

			var subItems = AllSubItems(_subjectRepository.DbSet).ToList();
			foreach (var entity in list) entity.LoadSubItems(subItems, activeOnly);
		}

		public void LoadSubItems(Subject entity, bool activeOnly = false)
		{
			var subItems = AllSubItems(_subjectRepository.DbSet).ToList();

			entity.LoadSubItems(subItems, activeOnly);
		}

		
	}
}
