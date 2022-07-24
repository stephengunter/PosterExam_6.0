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
	public static class AttachmentsViewService
	{
		public static AttachmentViewModel MapViewModel(this UploadFile attachment, IMapper mapper)
			=> mapper.Map<AttachmentViewModel>(attachment);

		public static List<AttachmentViewModel> MapViewModelList(this IEnumerable<UploadFile> attachments, IMapper mapper)
			=> attachments.Select(item => MapViewModel(item, mapper)).ToList();

		public static PagedList<UploadFile, AttachmentViewModel> GetPagedList(this IEnumerable<UploadFile> attachments, IMapper mapper, int page = 1, int pageSize = 99)
		{
			var pageList = new PagedList<UploadFile, AttachmentViewModel>(attachments, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}

		public static UploadFile MapEntity(this AttachmentViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<AttachmentViewModel, UploadFile>(model);

			if (model.Id == 0) entity.SetCreated(currentUserId);
			else entity.SetUpdated(currentUserId);

			return entity;
		}

		public static IEnumerable<UploadFile> GetOrdered(this IEnumerable<UploadFile> attachments)
			=> attachments.OrderBy(item => item.Order);

		
	}
}
