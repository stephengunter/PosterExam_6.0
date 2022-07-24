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
	public static class BillsViewService
	{

		public static BillViewModel MapViewModel(this Bill bill, IMapper mapper, IEnumerable<PayWay> payWays = null)
		{ 
		    var model = mapper.Map<BillViewModel>(bill);

			if (bill.Plan != null) model.Plan = bill.Plan.MapViewModel(mapper, bill.HasDiscount);
			if (payWays.HasItems()) model.Pays = bill.Pays.MapViewModelList(mapper, payWays);

			return model;
		}

		public static List<BillViewModel> MapViewModelList(this IEnumerable<Bill> bills, IMapper mapper, IEnumerable<PayWay> payWays = null)
			=> bills.Select(item => MapViewModel(item, mapper, payWays)).ToList();

		public static PagedList<Bill, BillViewModel> GetPagedList(this IEnumerable<Bill> bills, IMapper mapper, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Bill, BillViewModel>(bills, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}


		public static Bill MapEntity(this BillViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<BillViewModel, Bill>(model);
			
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			return entity;
		}

		public static IEnumerable<Bill> GetOrdered(this IEnumerable<Bill> bills)
		{
			return bills.OrderByDescending(item => item.CreatedAt);

		}

	}
}
