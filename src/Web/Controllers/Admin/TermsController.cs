using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Web.Models;
using Web.Controllers;

namespace Web.Controllers.Admin
{
	public class TermsController : BaseAdminController
	{
		private readonly IMapper _mapper;
		private readonly ITermsService _termsService;
		private readonly ISubjectsService _subjectsService;

		public TermsController(ITermsService termsService, ISubjectsService subjectsService, IMapper mapper)
		{
			_mapper = mapper;
			_termsService = termsService;
			_subjectsService = subjectsService;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject, int parent = -1, string keyword = "", bool subItems = true)
		{
			Subject selectedSubject = await _subjectsService.GetByIdAsync(subject);
			if (selectedSubject == null)
			{
				ModelState.AddModelError("subject", "科目不存在");
				return BadRequest(ModelState);
			}

			var terms = await _termsService.FetchAsync(selectedSubject, parent);

			if (terms.HasItems())
			{
				if (subItems) _termsService.LoadSubItems(terms);

				var keywords = keyword.GetKeywords();
				if(keywords.HasItems()) terms = terms.FilterByKeyword(keywords);
				
				terms = terms.GetOrdered();
			}

			
			return Ok(terms.MapViewModelList(_mapper));
		}




		[HttpGet("create")]
		public async Task<ActionResult> Create(int subject, int parent)
		{
			Subject selectedSubject = await _subjectsService.GetByIdAsync(subject);
			if (selectedSubject == null)
			{
				ModelState.AddModelError("subject", "科目不存在");
				return BadRequest(ModelState);
			}

			int maxOrder = await _termsService.GetMaxOrderAsync(selectedSubject, parent);
			int order = maxOrder + 1;
			var model = new TermViewModel()
			{
				Order = order,
				SubjectId = subject,
				ParentId = parent,
				Active = order >= 0
			};
		
			var terms = await _termsService.FetchAsync(selectedSubject);
			if (terms.HasItems())
			{
				_termsService.LoadSubItems(terms);
				terms = terms.GetOrdered();
			}

			var form = new TermEditForm()
			{
				Term = model,
				Parents = terms.MapViewModelList(_mapper)
			};
			return Ok(form);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] TermViewModel model)
		{
			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var term = model.MapEntity(_mapper, CurrentUserId);

			term = await _termsService.CreateAsync(term);

			return Ok(term.Id);
		}

		[HttpGet("{id}")]
		public ActionResult Details(int id)
		{
			var term = _termsService.GetById(id);
			if (term == null) return NotFound();

			
			return Ok(term.MapViewModel(_mapper));
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var term = await _termsService.GetByIdAsync(id);
			if (term == null) return NotFound();

			var model = term.MapViewModel(_mapper);
			model.Text = model.Text.ReplaceBrToNewLine();


			var selectedSubject = await _subjectsService.GetByIdAsync(term.SubjectId);
			var terms = await _termsService.FetchAsync(selectedSubject);
			if (terms.HasItems())
			{
				_termsService.LoadSubItems(terms);
				terms = terms.GetOrdered();
			}

			var form = new TermEditForm()
			{
				Term = model,
				Parents = terms.MapViewModelList(_mapper)
			};
			return Ok(form);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] TermViewModel model)
		{
			var existingEntity = _termsService.GetById(id);
			if (existingEntity == null) return NotFound();
			
			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var term = model.MapEntity(_mapper, CurrentUserId);

			await _termsService.UpdateAsync(existingEntity, term);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var term = await _termsService.GetByIdAsync(id);
			if (term == null) return NotFound();

			term.SetUpdated(CurrentUserId);
			await _termsService.RemoveAsync(term);

			return Ok();
		}

		async Task ValidateRequestAsync(TermViewModel model)
		{
			var subject = await _subjectsService.GetByIdAsync(model.SubjectId);
			if (subject == null) ModelState.AddModelError("subjectId", "科目不存在");

			if (model.ParentId > 0)
			{
				var parent = await _termsService.GetByIdAsync(model.ParentId);
				if(parent == null) ModelState.AddModelError("parentId", "主條文不存在");

				if (parent.Id == model.Id) ModelState.AddModelError("parentId", "主條文重疊.請選擇其他主條文");
			}
		}


	}
}
