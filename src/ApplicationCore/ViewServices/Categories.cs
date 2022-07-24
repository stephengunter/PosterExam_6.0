using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Views;
using AutoMapper;
using Newtonsoft.Json;
using System.Runtime.InteropServices.ComTypes;

namespace ApplicationCore.ViewServices
{
	public static class CategorysViewService
	{
		public static CategoryViewModel MapViewModel(this Category category, IMapper mapper) 
			=> mapper.Map<CategoryViewModel>(category);

		public static List<CategoryViewModel> MapViewModelList(this IEnumerable<Category> categories, IMapper mapper) 
			=> categories.Select(item => MapViewModel(item, mapper)).ToList();
	}
}
