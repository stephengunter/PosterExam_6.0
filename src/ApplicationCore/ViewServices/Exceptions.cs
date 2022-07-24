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

namespace ApplicationCore.ViewServices
{
	public static class ExceptionsViewService
	{
		public static PagedList<ExceptionViewModel> GetPagedList(this IEnumerable<ExceptionViewModel> records, int page = 1, int pageSize = 999)
			=> new PagedList<ExceptionViewModel>(records, page, pageSize);

		public static IEnumerable<ExceptionViewModel> GetOrdered(this IEnumerable<ExceptionViewModel> list)
			=> list.OrderByDescending(item => item.DateTime);
	}
}
