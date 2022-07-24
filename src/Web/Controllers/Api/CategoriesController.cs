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
	public class CategoriesController : BaseApiController
	{
		private readonly IDataService _dataService;
		private readonly IMapper _mapper;

		public CategoriesController(IDataService dataService, IMapper mapper)
		{
			_dataService = dataService;
			_mapper = mapper;
		}

		[HttpGet("")]
		public ActionResult Index()
		{
			var categories = _dataService.FetchNoteCategories();
			return Ok(categories);
		}

	}

	
}
