using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class ReviewRecordMappingProfile : Profile
	{
		public ReviewRecordMappingProfile()
		{
			CreateMap<ReviewRecord, ReviewRecordViewModel>();

			CreateMap<ReviewRecordViewModel, ReviewRecord>();
		}
	}
}
