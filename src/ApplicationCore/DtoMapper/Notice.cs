using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class NoticeMappingProfile : Profile
	{
		public NoticeMappingProfile()
		{
			CreateMap<Notice, NoticeViewModel>();

			CreateMap<NoticeViewModel, Notice>();
		}
	}
}
