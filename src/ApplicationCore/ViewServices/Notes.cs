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
	public static class NotesViewService
	{
		public static NoteViewModel MapViewModel(this Note note, IMapper mapper, ICollection<UploadFile> attachmentsList = null)
		{
			if (attachmentsList.HasItems()) note.LoadAttachments(attachmentsList);

			var model = mapper.Map<NoteViewModel>(note);

			if (!String.IsNullOrEmpty(model.Highlight)) model.Highlights =  JsonConvert.DeserializeObject<ICollection<string>>(model.Highlight);
			if (!String.IsNullOrEmpty(model.Reference)) model.References = JsonConvert.DeserializeObject<ICollection<ReferenceViewModel>>(model.Reference);

			return model;
		}

		public static List<NoteViewModel> MapViewModelList(this IEnumerable<Note> notes, IMapper mapper, ICollection<UploadFile> attachmentsList = null)
			=> notes.Select(item => MapViewModel(item, mapper, attachmentsList)).ToList();

		public static Note MapEntity(this NoteViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<NoteViewModel, Note>(model);
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			if (!entity.Text.HasHtmlTag()) entity.Text = entity.Text.ReplaceNewLine();

			entity.Highlight = model.Highlights.HasItems() ? JsonConvert.SerializeObject(model.Highlights) : "";
			entity.Reference = model.References.HasItems() ? JsonConvert.SerializeObject(model.References) : "";

			return entity;
		}

		public static PagedList<Note, NoteViewModel> GetPagedList(this IEnumerable<Note> notes, IMapper mapper,
			ICollection<UploadFile> attachmentsList = null, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Note, NoteViewModel>(notes, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper, attachmentsList);

			pageList.List = null;

			return pageList;
		}

		public static IEnumerable<Note> GetOrdered(this IEnumerable<Note> notes)
			=> notes.OrderBy(item => item.Order);

		public static IEnumerable<Note> FilterByKeyword(this IEnumerable<Note> notes, ICollection<string> keywords)
			=> notes.Where(item => keywords.Any(item.HasKeyword)).ToList();

		
	}
}
