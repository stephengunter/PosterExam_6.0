using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper
{
	public class BillMappingProfile : Profile
	{
		public BillMappingProfile()
		{
			CreateMap<Bill, BillViewModel>();

			CreateMap<BillViewModel, Bill>();
		}
	}
}
