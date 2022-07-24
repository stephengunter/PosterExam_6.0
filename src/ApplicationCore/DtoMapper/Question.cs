using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ApplicationCore.DtoMapper
{
	public class QuestionMappingProfile : Profile
	{
		public QuestionMappingProfile()
		{
			CreateMap<Question, QuestionViewModel>();

			CreateMap<QuestionViewModel, Question>().ForMember(dest => dest.RecruitQuestions, opt => opt.MapFrom<QuestionRecruitsResolver>());
		}
	}

	public class QuestionRecruitsResolver : IValueResolver<QuestionViewModel, Question, ICollection<RecruitQuestion>>
	{
		public ICollection<RecruitQuestion> Resolve(QuestionViewModel source, Question destination, ICollection<RecruitQuestion> destMember, ResolutionContext context)
		{
			return source.Recruits.Select(item => new RecruitQuestion { RecruitId = item.Id }).ToList();
		}
	}
}
