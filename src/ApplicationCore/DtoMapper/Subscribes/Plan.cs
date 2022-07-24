﻿using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper
{
	public class PlanMappingProfile : Profile
	{
		public PlanMappingProfile()
		{
			CreateMap<Plan, PlanViewModel>();

			CreateMap<PlanViewModel, Plan>();
		}
	}
}
