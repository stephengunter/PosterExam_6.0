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

namespace Web.Controllers.Admin
{
	public class SubjectsController : BaseAdminController
	{
		private readonly IMapper _mapper;
		private readonly ISubjectsService _subjectsService;

		public SubjectsController(ISubjectsService subjectsService, IMapper mapper)
		{
			_mapper = mapper;
			_subjectsService = subjectsService;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int parent = -1,  bool subItems = true)
		{
			var subjects = await _subjectsService.FetchAsync(parent);
			
			subjects = subjects.GetOrdered();

			if (subItems)
			{
				_subjectsService.LoadSubItems(subjects);
				
				foreach (var item in subjects) item.GetSubIds();
			}

			return Ok(subjects.MapViewModelList(_mapper));
		}

		[HttpGet("create")]
		public ActionResult Create() => Ok(new SubjectViewModel() { Order = -1 });
		

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] SubjectViewModel model)
		{
			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var subject = model.MapEntity(_mapper, CurrentUserId);

			subject = await _subjectsService.CreateAsync(subject);

			return Ok(subject.Id);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var subject = await _subjectsService.GetByIdAsync(id);
			if (subject == null) return NotFound();

			return Ok(subject.MapViewModel(_mapper));
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] SubjectViewModel model)
		{
			var existingEntity = _subjectsService.GetById(id);
			if (existingEntity == null) return NotFound();

			await ValidateRequestAsync(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var subject = model.MapEntity(_mapper, CurrentUserId);

			await _subjectsService.UpdateAsync(existingEntity, subject);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var subject = await _subjectsService.GetByIdAsync(id);
			if (subject == null) return NotFound();

			subject.SetUpdated(CurrentUserId);
			await _subjectsService.RemoveAsync(subject);

			return Ok();
		}

		async Task ValidateRequestAsync(SubjectViewModel model)
		{
			if (model.ParentId > 0)
			{
				var parent = await _subjectsService.GetByIdAsync(model.ParentId);
				if (parent == null) ModelState.AddModelError("parentId", "主科目不存在");

				if (parent.Id == model.Id) ModelState.AddModelError("parentId", "主科目重疊.請選擇其他主科目");
				
			}
		}


	}
}
