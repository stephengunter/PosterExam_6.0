using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class ExamMappingProfile : Profile
	{
		public ExamMappingProfile()
		{
			CreateMap<Exam, ExamViewModel>();

			CreateMap<ExamViewModel, Exam>();
		}
	}
}
