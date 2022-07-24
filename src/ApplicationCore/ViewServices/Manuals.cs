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
	public static class ManualsViewService
	{
		public static ManualViewModel MapViewModel(this Manual manual, IMapper mapper)
		{
			var model = mapper.Map<ManualViewModel>(manual);
			if (model.SubItems.HasItems()) model.SubItems = model.SubItems.OrderBy(item => item.Order).ToList();
			if (model.Features.HasItems()) model.Features = model.Features.OrderBy(item => item.Order).ToList();
			return model;
		}

		public static List<ManualViewModel> MapViewModelList(this IEnumerable<Manual> manuals, IMapper mapper)
			=> manuals.Select(item => MapViewModel(item, mapper)).ToList();

		public static Manual MapEntity(this ManualViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<ManualViewModel, Manual>(model);
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			entity.Content = entity.Content.ReplaceNewLine("");
			return entity;
		}

		public static IEnumerable<Manual> GetOrdered(this IEnumerable<Manual> manuals)
			=> manuals.OrderBy(item => item.Order);


		public static FeatureViewModel MapViewModel(this Feature feature, IMapper mapper)
		{
			var model = mapper.Map<FeatureViewModel>(feature);
			return model;
		}

		public static List<FeatureViewModel> MapViewModelList(this IEnumerable<Feature> features, IMapper mapper)
			=> features.Select(item => MapViewModel(item, mapper)).ToList();

		public static Feature MapEntity(this FeatureViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<FeatureViewModel, Feature>(model);
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			entity.Content = entity.Content.ReplaceNewLine("");
			return entity;
		}

		public static IEnumerable<Feature> GetOrdered(this IEnumerable<Feature> features)
			=> features.OrderBy(item => item.Order);

	}
}
