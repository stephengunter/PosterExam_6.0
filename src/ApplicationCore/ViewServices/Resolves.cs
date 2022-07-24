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

namespace ApplicationCore.ViewServices
{
	public static class ResolvesViewService
	{
		public static ResolveViewModel MapViewModel(this Resolve resolve, IMapper mapper, ICollection<UploadFile> attachmentsList = null)
		{
			if (attachmentsList.HasItems()) resolve.LoadAttachments(attachmentsList);

			var model = mapper.Map<ResolveViewModel>(resolve);

			if (!String.IsNullOrEmpty(model.Highlight)) model.Highlights =  JsonConvert.DeserializeObject<ICollection<string>>(model.Highlight);
			if (!String.IsNullOrEmpty(model.Source)) model.Sources = JsonConvert.DeserializeObject<ICollection<SourceViewModel>>(model.Source);

			return model;
		}

		public static List<ResolveViewModel> MapViewModelList(this IEnumerable<Resolve> resolves, IMapper mapper, ICollection<UploadFile> attachmentsList = null)
			=> resolves.Select(item => MapViewModel(item, mapper, attachmentsList)).ToList();

		public static Resolve MapEntity(this ResolveViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<ResolveViewModel, Resolve>(model);
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			if(!entity.Text.HasHtmlTag()) entity.Text = entity.Text.ReplaceNewLine();

			entity.Highlight = model.Highlights.HasItems() ? JsonConvert.SerializeObject(model.Highlights) : "";
			entity.Source = model.Sources.HasItems() ? JsonConvert.SerializeObject(model.Sources) : "";

			return entity;
		}

		public static PagedList<Resolve, ResolveViewModel> GetPagedList(this IEnumerable<Resolve> resolves, IMapper mapper,
			ICollection<UploadFile> attachmentsList = null, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Resolve, ResolveViewModel>(resolves, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper, attachmentsList);

			pageList.List = null;

			return pageList;
		}
	}
}
