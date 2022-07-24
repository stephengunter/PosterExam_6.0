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
	public interface IArticlesService
	{
		Task<IEnumerable<Article>> FetchAsync(int category);
		Task<Article> GetByIdAsync(int id);
		Task<Article> CreateAsync(Article article);

		Task<IEnumerable<Article>> FetchAllAsync();
		Task UpdateAsync(Article article);
		Task UpdateAsync(Article existingEntity, Article model);
		Task RemoveAsync(Article article);


		#region  Categories
		Task<IEnumerable<Category>> FetchCategoriesAsync();

		Category FindCategoryByKey(string key);

		#endregion
	}

	public class ArticlesService : IArticlesService
	{
		private readonly IDefaultRepository<Article> _articleRepository;
		private readonly IDefaultRepository<Category> _categoryRepository;
		public ArticlesService(IDefaultRepository<Article> articleRepository, IDefaultRepository<Category> categoryRepository)
		{
			_articleRepository = articleRepository;
			_categoryRepository = categoryRepository;
		}

		
		public async Task<IEnumerable<Article>> FetchAsync(int category)
			=> await _articleRepository.ListAsync(new ArticleFilterSpecification(category));

		public async Task<IEnumerable<Article>> FetchAllAsync()
			=> await _articleRepository.ListAsync(new ArticleFilterSpecification());

		public async Task<Article> GetByIdAsync(int id) => await _articleRepository.GetByIdAsync(id);
		
		public async Task<Article> CreateAsync(Article article) => await _articleRepository.AddAsync(article);


		public async Task UpdateAsync(Article article) => await _articleRepository.UpdateAsync(article);

		public async Task UpdateAsync(Article existingEntity, Article model) => await _articleRepository.UpdateAsync(existingEntity, model);

		public async Task RemoveAsync(Article article)
		{
			
			article.Removed = true;
			await _articleRepository.UpdateAsync(article);
		}

		#region  Categories
		public async Task<IEnumerable<Category>> FetchCategoriesAsync() 
			=> await _categoryRepository.ListAsync(new CategoryFilterSpecification());

		public Category FindCategoryByKey(string key)
			=> _categoryRepository.GetSingleBySpec(new CategoryFilterSpecification(key));

		#endregion
	}
}
