﻿using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper
{
	public class PayMappingProfile : Profile
	{
		public PayMappingProfile()
		{
			CreateMap<Pay, PayViewModel>();

			CreateMap<PayViewModel, Pay>();
		}
	}
}
