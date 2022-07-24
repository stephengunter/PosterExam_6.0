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
	public static class TermsViewService
	{
		public static TermViewModel MapViewModel(this Term term, IMapper mapper)
		{
			var model = mapper.Map<TermViewModel>(term);
			if (!String.IsNullOrEmpty(model.Highlight)) model.Highlights = JsonConvert.DeserializeObject<ICollection<string>>(model.Highlight);
			if (!String.IsNullOrEmpty(model.Reference)) model.References = JsonConvert.DeserializeObject<ICollection<ReferenceViewModel>>(model.Reference);

			if (term.SubItems.HasItems()) model.SubItems = term.SubItems.Select(item => item.MapViewModel(mapper)).ToList();
			
			return model;
		}

		public static List<TermViewModel> MapViewModelList(this IEnumerable<Term> terms, IMapper mapper)
			=> terms.Select(item => MapViewModel(item, mapper)).ToList();

		public static Term MapEntity(this TermViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<TermViewModel, Term>(model);
			if (!entity.Text.HasHtmlTag())  entity.Text = entity.Text.ReplaceNewLine();

			entity.Highlight = model.Highlights.HasItems() ? JsonConvert.SerializeObject(model.Highlights) : "";
			entity.Reference = model.References.HasItems() ? JsonConvert.SerializeObject(model.References) : "";


			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			return entity;
		}


		public static IEnumerable<Term> GetOrdered(this IEnumerable<Term> terms)
			=> terms.OrderBy(item => item.Order);

		public static IEnumerable<Term> FilterByKeyword(this IEnumerable<Term> terms, ICollection<string> keywords)
			=> terms.Where(item => keywords.Any(item.Text.Contains)).ToList();
	}
}
