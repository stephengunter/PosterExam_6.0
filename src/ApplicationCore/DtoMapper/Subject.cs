using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class SubjectMappingProfile : Profile
	{
		public SubjectMappingProfile()
		{
			CreateMap<Subject, SubjectViewModel>();
			CreateMap<SubjectViewModel, Subject>();
		}
	}
}
