using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Microsoft.AspNetCore.Authorization;
using Web.Models;

namespace Web.Controllers.Api
{
	[Authorize(Policy = "Subscriber")]
	public class NotesController : BaseApiController
	{
		private readonly IDataService _dataService;
		private readonly INotesService _notesService;
		private readonly IMapper _mapper;

		public NotesController(IDataService dataService, INotesService notesService, 
			 IMapper mapper)
		{
			_dataService = dataService;
			_notesService = notesService;
			_mapper = mapper;
		}

		[HttpGet("categories")]
		public ActionResult Categories()
		{
			var categories = _dataService.FetchNoteCategories();

			var paramsView = _dataService.FindNoteParams(CurrentUserId);

			var model = new NotesIndexModel
			{
				Categories = categories.ToList(),
				Params = paramsView
			};

			return Ok(model);
		}

		[HttpGet("")]
		public ActionResult Index(int mode = 0, int term = 0, int subject = 0, string keyword = "")
		{
			if (term > 0)
			{
				var termViewModel = _dataService.FindTermNotesByTerm(term);
				if (termViewModel == null) return NotFound();

				if (mode < 0 || mode > 1) mode = 0;
				_dataService.SaveNoteParams(CurrentUserId, new NoteParamsViewModel
				{
					Mode = mode,
					SubjectId = 0,
					TermId = term
				});

				if (termViewModel.SubItems.HasItems()) return Ok(termViewModel.SubItems);
				else return Ok(new List<TermViewModel> { termViewModel });
			}
			else if (subject > 0)
			{
				var keywords = keyword.GetKeywords();
				var termViewList = _dataService.FetchTermNotesBySubject(subject);

				if (keywords.IsNullOrEmpty())
				{
					if (mode < 0 || mode > 1) mode = 0;
					_dataService.SaveNoteParams(CurrentUserId, new NoteParamsViewModel
					{
						Mode = mode,
						SubjectId = subject,
						TermId = 0
					});

					return Ok(termViewList);
				} 


				var termsHasKeywords = FilterByKeywords(termViewList, keywords);

				var noteIds = FetchNoteIdsByKeywords(termViewList, keywords);

				var resultList = new List<TermViewModel>();
				if (termsHasKeywords.HasItems()) resultList.AddRange(termsHasKeywords);


				var terms = termViewList.SelectMany(x => x.SubItems).ToList();
				if (terms.IsNullOrEmpty()) terms = termViewList.ToList();

				foreach (var termView in terms)
				{
					termView.Notes = termView.Notes.Where(x => noteIds.Contains(x.Id)).ToList();
					if (termView.Notes.HasItems())
					{
						if (resultList.FirstOrDefault(x => x.Id == termView.Id) == null)
						{
							resultList.Add(termView);
						}
					}
				}

				return Ok(resultList);
			}

			ModelState.AddModelError("params", "錯誤的查詢參數");
			return BadRequest(ModelState);
		}

		List<TermViewModel> FilterByKeywords(IEnumerable<TermViewModel> termViewList, IList<string> keywords)
		{
			var terms = termViewList.SelectMany(x => x.SubItems).ToList();
			if (terms.HasItems())
			{
				return terms.Where(item => keywords.Any(item.Text.Contains)).ToList();
			}
			else
			{

				return termViewList.Where(item => keywords.Any(item.Text.Contains)).ToList();
			}
		}

		List<int> FetchNoteIdsByKeywords(IEnumerable<TermViewModel> termViewList, IList<string> keywords)
		{
			var terms = termViewList.SelectMany(x => x.SubItems).ToList();
			if (terms.HasItems())
			{
				return terms.SelectMany(x => x.Notes).Where(item => keywords.Any(item.HasKeyword)).Select(x => x.Id).ToList();
			}
			else
			{
				return termViewList.SelectMany(x => x.Notes).Where(item => keywords.Any(item.HasKeyword)).Select(x => x.Id).ToList();
			}


		}

		[HttpGet("{id}")]
		public ActionResult Details(int id)
		{
			var note = _notesService.GetById(id);
			if (note == null) return NotFound();

			return Ok(note.MapViewModel(_mapper));
		}



	}

	
}
