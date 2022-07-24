using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class ManualMappingProfile : Profile
	{
		public ManualMappingProfile()
		{
			CreateMap<Manual, ManualViewModel>();

			CreateMap<ManualViewModel, Manual>();
		}
	}

	public class FeatureMappingProfile : Profile
	{
		public FeatureMappingProfile()
		{
			CreateMap<Feature, FeatureViewModel>();

			CreateMap<FeatureViewModel, Feature>();
		}
	}
}
