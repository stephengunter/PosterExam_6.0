using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Microsoft.AspNetCore.Authorization;
using Web.Models;

namespace Web.Controllers.Api
{
	public class ArticlesController : BaseApiController
	{
		private readonly IArticlesService _articlesService;
		private readonly IUsersService _usersService;
		private readonly IMapper _mapper;

		public ArticlesController(IArticlesService articlesService, IUsersService usersService, IMapper mapper)
		{
			_articlesService = articlesService;
			_usersService = usersService;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(string key)
		{
			if (String.IsNullOrEmpty(key)) return BadRequest();

			var category = ValidateRequest(key);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var articles = await _articlesService.FetchAsync(category.Id);
			if (articles.HasItems())
			{
				articles = articles.Where(x => x.Active);
				articles = articles.GetOrdered().ToList();
			}

			return Ok(articles.MapViewModelList(_mapper));
		}

		[HttpGet("{id}/{user?}")]
		public async Task<ActionResult> Details(int id, string user = "")
		{
			var article = await _articlesService.GetByIdAsync(id);
			if (article == null) return NotFound();

			if (!article.Active)
			{
				var existingUser = await _usersService.FindUserByIdAsync(user);
				if (existingUser == null) return NotFound();

				bool isAdmin = await _usersService.IsAdminAsync(existingUser);
				if (!isAdmin) return NotFound();
			}

			return Ok(article.MapViewModel(_mapper));
		}

		Category ValidateRequest(string key)
		{
			if (String.IsNullOrEmpty(key))
			{
				ModelState.AddModelError("key", "key不得空白");
				return null;
			} 

			var category = _articlesService.FindCategoryByKey(key);
			if (category == null)
			{
				ModelState.AddModelError("key", "錯誤的key");
				return null;
			}

			return category;

		}

	}

	
}
