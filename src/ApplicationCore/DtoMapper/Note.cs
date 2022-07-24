using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DtoMapper
{
	public class NoteMappingProfile : Profile
	{
		public NoteMappingProfile()
		{
			CreateMap<Note, NoteViewModel>();

			CreateMap<NoteViewModel, Note>();
		}
	}
}
