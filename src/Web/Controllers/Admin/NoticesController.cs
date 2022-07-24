using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;

namespace Web.Controllers.Admin
{
	public class NoticesController : BaseAdminController
	{
		private readonly INoticesService _noticesService;
		private readonly IMapper _mapper;

		public NoticesController(INoticesService noticesService, IMapper mapper)
		{
			
			_noticesService = noticesService;
			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index(int open, int active, int page = 1, int pageSize = 10)
		{
			if (page < 1) page = 1;

			// 1 = public 公開, 0 = 私人
			var notices = await _noticesService.FetchAsync(open.ToBoolean());

			notices = notices.Where(x => x.Active == active.ToBoolean());

			notices = notices.GetOrdered().ToList();

			return Ok(notices.GetPagedList(_mapper, page, pageSize));
		}

		[HttpGet("create")]
		public ActionResult Create()
		{
			return Ok(new NoticeViewModel() { Public = true, Active = false, Order = -1 });
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] NoticeViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var notice = model.MapEntity(_mapper, CurrentUserId);
			notice.Order = model.Active ? 0 : - 1;

			notice = await _noticesService.CreateAsync(notice);

			return Ok(notice.MapViewModel(_mapper));
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var notice = await _noticesService.GetByIdAsync(id);
			if (notice == null) return NotFound();

			var model = notice.MapViewModel(_mapper);

			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] NoticeViewModel model)
		{
			var existingEntity = await _noticesService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var notice = model.MapEntity(_mapper, CurrentUserId);
			notice.Order = model.Active ? 0 : -1;

			await _noticesService.UpdateAsync(existingEntity, notice);

			return Ok(notice.MapViewModel(_mapper));
		}

		[HttpPost("off")]
		public async Task<ActionResult> Off([FromBody] NoticeViewModel model)
		{
			var notice = await _noticesService.GetByIdAsync(model.Id);
			if (notice == null) return NotFound();

			notice.Order = -1;
			await _noticesService.UpdateAsync(notice);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var notice = await _noticesService.GetByIdAsync(id);
			if (notice == null) return NotFound();

			notice.Order = -1;
			notice.SetUpdated(CurrentUserId);
			await _noticesService.RemoveAsync(notice);

			return Ok();
		}

		void ValidateRequest(NoticeViewModel model)
		{
			if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫標題");

			if (String.IsNullOrEmpty(model.Content)) ModelState.AddModelError("content", "必須填寫內容");

			if (!model.Public) ModelState.AddModelError("public", "不支持儲存個人訊息");

		}


	}
}
