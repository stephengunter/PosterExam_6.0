using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class ExamQuestionMappingProfile : Profile
	{
		public ExamQuestionMappingProfile()
		{
			CreateMap<ExamQuestion, ExamQuestionViewModel>();

			CreateMap<ExamQuestionViewModel, ExamQuestion>();
		}
	}
}
