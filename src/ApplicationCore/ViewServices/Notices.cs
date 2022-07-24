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
	public static class NoticesViewService
	{
		public static NoticeViewModel MapViewModel(this Notice notice, IMapper mapper) 
			=> mapper.Map<NoticeViewModel>(notice);

		public static List<NoticeViewModel> MapViewModelList(this IEnumerable<Notice> notices, IMapper mapper) 
			=> notices.Select(item => MapViewModel(item, mapper)).ToList();

		public static PagedList<Notice, NoticeViewModel> GetPagedList(this IEnumerable<Notice> notices, IMapper mapper, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Notice, NoticeViewModel>(notices, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}

		public static Notice MapEntity(this NoticeViewModel model, IMapper mapper, string currentUserId)
		{ 
			var entity = mapper.Map<NoticeViewModel, Notice>(model);

			if (model.Id == 0) entity.SetCreated(currentUserId);
			else entity.SetUpdated(currentUserId);

			return entity;
		}

		public static IEnumerable<Notice> GetOrdered(this IEnumerable<Notice> notices)
			=> notices.OrderByDescending(item => item.Top).ThenByDescending(item => item.Order)
			.ThenByDescending(item => item.LastUpdated);



		public static ReceiverViewModel MapViewModel(this Receiver receiver, IMapper mapper)
		{ 
			var model = mapper.Map<ReceiverViewModel>(receiver);
			if (receiver.Notice != null) model.Notice = receiver.Notice.MapViewModel(mapper);

			return model;
		}

		public static List<ReceiverViewModel> MapViewModelList(this IEnumerable<Receiver> receivers, IMapper mapper)
			=> receivers.Select(item => MapViewModel(item, mapper)).ToList();


		public static IEnumerable<Receiver> GetOrdered(this IEnumerable<Receiver> receivers)
			=> receivers.OrderByDescending(item => item.Notice.LastUpdated);


		public static PagedList<Receiver, ReceiverViewModel> GetPagedList(this IEnumerable<Receiver> receivers, IMapper mapper, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Receiver, ReceiverViewModel>(receivers, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}
	}
}
