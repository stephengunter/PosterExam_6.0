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
using ApplicationCore.Specifications;
using Web.Helpers;
using Web.Controllers;
using Infrastructure.Views;

namespace Web.Controllers.Admin
{
	public class ManualsController : BaseAdminController
	{
		private readonly IManualsService _manualsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IMapper _mapper;

		public ManualsController(IManualsService manualsService, IAttachmentsService attachmentsService,
			 IMapper mapper)
		{
			_manualsService = manualsService;
			_attachmentsService = attachmentsService;
			_mapper = mapper;
		}

		

		[HttpGet("")]
		public async Task<ActionResult> Index(int active = 1)
		{
			var manuals = await _manualsService.FetchAsync(active.ToBoolean());

			foreach (var item in manuals)
			{
				item.Features = item.Features.Where(x => !x.Removed).ToList();
			}

			manuals = manuals.GetOrdered();

			return Ok(manuals.MapViewModelList(_mapper));
		}

		[HttpGet("create")]
		public async Task<ActionResult> Create()
		{
			int parentId = 0;
			var rootItems = await _manualsService.FetchAsync(parentId);

			var form = new ManualEditForm
			{
				Parents = rootItems.Select(item => new BaseOption<int>(item.Id, item.Title)).ToList(),
				Manual = new ManualViewModel() { Order = -1 }
			};

			return Ok(form);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] ManualViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.Order = model.Active ? 0 : -1;

			var manual = model.MapEntity(_mapper, CurrentUserId);

			manual = await _manualsService.CreateAsync(manual);

			return Ok(manual.Id);
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var manual = await _manualsService.GetByIdAsync(id);
			if (manual == null) return NotFound();

			int parentId = 0;
			var rootItems = await _manualsService.FetchAsync(parentId);

			var form = new ManualEditForm
			{
				Parents = rootItems.Select(item => new BaseOption<int>(item.Id, item.Title)).ToList(),
				Manual = manual.MapViewModel(_mapper)
			};

			return Ok(form);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] ManualViewModel model)
		{
			var existingEntity = await _manualsService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var notice = model.MapEntity(_mapper, CurrentUserId);
			notice.Order = model.Active ? 0 : -1;

			await _manualsService.UpdateAsync(existingEntity, notice);

			return Ok();
		}


		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var manual = await _manualsService.GetByIdAsync(id);
			if (manual == null) return NotFound();

			manual.SetUpdated(CurrentUserId);
			await _manualsService.RemoveAsync(manual);

			return Ok();
		}

		void ValidateRequest(ManualViewModel model)
		{
			if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "請填寫標題");
		}
	}
}
