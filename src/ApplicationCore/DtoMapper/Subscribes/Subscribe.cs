using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper
{
	public class SubscribeMappingProfile : Profile
	{
		public SubscribeMappingProfile()
		{
			CreateMap<Subscribe, SubscribeViewModel>();

			CreateMap<SubscribeViewModel, Subscribe>();
		}
	}
}
