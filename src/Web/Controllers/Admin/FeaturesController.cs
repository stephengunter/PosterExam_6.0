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

namespace Web.Controllers.Admin
{
	public class FeaturesController : BaseAdminController
	{
		private readonly IManualsService _manualsService;
		private readonly IMapper _mapper;

		public FeaturesController(IManualsService manualsService, IMapper mapper)

		{
			_manualsService = manualsService;
			_mapper = mapper;
		}


		[HttpGet("create")]
		public ActionResult Create()
		{
			return Ok(new FeatureViewModel() { Order = -1 });
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] FeatureViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.Order = model.Active ? 0 : -1;

			var feature = model.MapEntity(_mapper, CurrentUserId);

			feature = await _manualsService.CreateFeatureAsync(feature);

			return Ok(feature.Id);
		}

		[HttpGet("edit/{id}")]
		public ActionResult Edit(int id)
		{
			var feature = _manualsService.GetFeatureById(id);
			if (feature == null) return NotFound();

			var model = feature.MapViewModel(_mapper);
			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] FeatureViewModel model)
		{
			var existingEntity = await _manualsService.GetFeatureByIdAsync(id);
			if (existingEntity == null) return NotFound();

			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			model.Order = model.Active ? 0 : -1;
			var feature = model.MapEntity(_mapper, CurrentUserId);

			await _manualsService.UpdateFeatureAsync(existingEntity, feature);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var feature = await _manualsService.GetFeatureByIdAsync(id);
			if (feature == null) return NotFound();

			feature.SetUpdated(CurrentUserId);
			await _manualsService.RemoveFeatureAsync(feature);

			return Ok();
		}

		void ValidateRequest(FeatureViewModel model)
		{
			if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "請填寫標題");
		}
	}
}
