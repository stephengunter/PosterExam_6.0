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
	public static class UsersViewService
	{
		public static UserViewModel MapViewModel(this User user, IMapper mapper)
			=> mapper.Map<UserViewModel>(user);

		public static User MapEntity(this UserViewModel model, IMapper mapper)
			=> mapper.Map<UserViewModel, User>(model);

		public static List<UserViewModel> MapViewModelList(this IEnumerable<User> users, IMapper mapper)
			=> users.Select(item => MapViewModel(item, mapper)).ToList();

		public static PagedList<User, UserViewModel> GetPagedList(this IEnumerable<User> users, IMapper mapper, int page = 1, int pageSize = -1)
		{
			var pageList = new PagedList<User, UserViewModel>(users, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}


		public static IEnumerable<User> GetOrdered(this IEnumerable<User> users) => users.OrderByDescending(u => u.CreatedAt);

		public static IEnumerable<User> FilterByKeyword(this IEnumerable<User> users, ICollection<string> keywords)
			=> users.Where(item => keywords.Any(item.UserName.CaseInsensitiveContains)).ToList();

	}
}
