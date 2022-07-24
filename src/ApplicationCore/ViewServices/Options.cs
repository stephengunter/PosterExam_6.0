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
	public static class OptionsViewService
	{
		public static OptionViewModel MapViewModel(this Option option, IMapper mapper)
			=> mapper.Map<OptionViewModel>(option);

		public static Option MapEntity(this OptionViewModel model, IMapper mapper)
			=> mapper.Map<OptionViewModel, Option>(model);
	}
}
