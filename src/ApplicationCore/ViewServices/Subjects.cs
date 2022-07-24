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
	public static class SubjectsViewService
	{
		public static SubjectViewModel MapViewModel(this Subject subject, IMapper mapper)
			=> mapper.Map<SubjectViewModel>(subject);

		public static List<SubjectViewModel> MapViewModelList(this IEnumerable<Subject> subjects, IMapper mapper)
			=> subjects.Select(item => MapViewModel(item, mapper)).ToList();

		public static Subject MapEntity(this SubjectViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<SubjectViewModel, Subject>(model);

			if (model.Id == 0) entity.SetCreated(currentUserId);
			else entity.SetUpdated(currentUserId);

			return entity;
		}
		public static IEnumerable<Subject> GetOrdered(this IEnumerable<Subject> subjects)
			=> subjects.OrderBy(item => item.Order);

		public static BaseOption<int> ToOption(this Subject subject)
			=> new BaseOption<int>(subject.Id, subject.Title);
	}
}
