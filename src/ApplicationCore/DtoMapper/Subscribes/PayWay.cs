using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper
{
	public class PayWayMappingProfile : Profile
	{
		public PayWayMappingProfile()
		{
			CreateMap<PayWay, PayWayViewModel>();

			CreateMap<PayWayViewModel, PayWay>();
		}
	}
}
