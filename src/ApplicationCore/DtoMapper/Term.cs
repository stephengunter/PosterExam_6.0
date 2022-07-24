using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class TermMappingProfile : Profile
	{
		public TermMappingProfile()
		{
			CreateMap<Term, TermViewModel>();

			CreateMap<TermViewModel, Term>();
		}
	}
}
