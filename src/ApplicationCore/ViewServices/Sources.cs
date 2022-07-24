using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Views;
using ApplicationCore.Services;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Views;
using AutoMapper;
using Newtonsoft.Json;

namespace ApplicationCore.ViewServices
{
	public static class SourcesViewService
	{
		public static void MapContent(this SourceViewModel model, INotesService notesService, ITermsService termsService)
		{
			if (model.NoteId > 0)
			{
				var note = notesService.GetById(model.NoteId);
				model.TermId = note.TermId;
				model.Title = note.Title;
				model.Text = note.Text;
			}
			else if (model.TermId > 0)
			{
				var term = termsService.GetById(model.TermId);
				model.Title = $"{term.Subject.Title} {term.Title}" ;
				model.Text = term.Text;
				model.NoteId = 0;
			}
		}
		
	}
}
