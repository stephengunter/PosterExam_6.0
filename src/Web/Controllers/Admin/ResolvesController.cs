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
using Newtonsoft.Json;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Web.Controllers;

namespace Web.Controllers.Admin
{
	public class ResolvesController : BaseAdminController
	{
		private readonly IResolvesService _resolvesService;
		private readonly INotesService _notesService;
		private readonly ITermsService _termsService;
		private readonly IReviewRecordsService _reviewRecordsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly AdminSettings _adminSettings;
		private readonly IMapper _mapper;

		public ResolvesController(IResolvesService resolvesService, IReviewRecordsService reviewRecordsService,
			INotesService notesService, ITermsService termsService,
			IAttachmentsService attachmentsService, IOptions<AdminSettings> adminSettings, IMapper mapper)
		{
			_resolvesService = resolvesService;
			_reviewRecordsService = reviewRecordsService;
			_notesService = notesService;
			_termsService = termsService;
			_attachmentsService = attachmentsService;
			_adminSettings = adminSettings.Value;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int question = 0, int page = 1, int pageSize = 10)
		{
			var resolves = await _resolvesService.FetchAsync(question);

			List<UploadFile> attachments = null;

			if (resolves.IsNullOrEmpty())
			{
				if (question > 0) return Ok(new List<ResolveViewModel>());
				else return Ok(resolves.GetPagedList(_mapper, attachments, page, pageSize));
			}

			var postIds = resolves.Select(x => x.Id).ToList();

			attachments = (await _attachmentsService.FetchAsync(PostType.Resolve, postIds)).ToList();

			if (question > 0)
			{
				var viewList = resolves.MapViewModelList(_mapper, attachments.ToList());
				foreach (var view in viewList)
				{
					foreach (var item in view.Sources)
					{
						item.MapContent(_notesService, _termsService);
					}
				}
				return Ok(viewList);
			}
			else
			{
				var pageList = resolves.GetPagedList(_mapper, attachments.ToList(), page, pageSize);
				return Ok(pageList);
			}
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] ResolveViewModel model)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var resolve = model.MapEntity(_mapper, CurrentUserId);

			resolve.Reviewed = true;
			resolve = await _resolvesService.CreateAsync(resolve);

			if (model.Attachments.HasItems())
			{
				var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
				foreach (var attachment in attachments)
				{
					attachment.PostType = PostType.Resolve;
					attachment.PostId = resolve.Id;
				}

				_attachmentsService.CreateMany(attachments);

				resolve.Attachments = attachments;
			}

			var reviewRecord = new ReviewRecord { Reviewed = true, Type = ReviewableType.Resolve, PostId = resolve.Id };
			reviewRecord.SetCreated(CurrentUserId);
			await _reviewRecordsService.CreateAsync(reviewRecord);
			

			return Ok(resolve.MapViewModel(_mapper));
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] ResolveViewModel model)
		{
			var existingEntity = await _resolvesService.GetByIdAsync(id);
			if (existingEntity == null) return NotFound();

			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var resolve = model.MapEntity(_mapper, CurrentUserId);
			resolve.Reviewed = true;

			await _resolvesService.UpdateAsync(existingEntity, resolve);


			if (model.Attachments.HasItems())
			{
				var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
				foreach (var attachment in attachments)
				{
					attachment.PostType = PostType.Resolve;
					attachment.PostId = resolve.Id;
				}

				await _attachmentsService.SyncAttachmentsAsync(resolve, attachments);

				resolve.Attachments = attachments;
			}
			else
			{
				await _attachmentsService.SyncAttachmentsAsync(resolve, null);
			}

			var reviewRecord = new ReviewRecord { Reviewed = true, Type = ReviewableType.Resolve, PostId = resolve.Id };
			reviewRecord.SetCreated(CurrentUserId);
			await _reviewRecordsService.CreateAsync(reviewRecord);

			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			var resolve = await _resolvesService.GetByIdAsync(id);
			if (resolve == null) return NotFound();

			resolve.SetUpdated(CurrentUserId);
			await _resolvesService.RemoveAsync(resolve);

			return Ok();
		}

		[HttpPost("admin")]
		public async Task<ActionResult> Admin(AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			//同步更新解析中的參考(因Note TermId可能變動)
			var resolves = await _resolvesService.FetchAsync();

			resolves = resolves.Where(x => !String.IsNullOrEmpty(x.Source)).ToList();

			var viewList = resolves.MapViewModelList(_mapper, new List<UploadFile>());
			foreach (var view in viewList)
			{
				foreach (var item in view.Sources)
				{
					item.MapContent(_notesService, _termsService);
				}

				var entity = resolves.Where(x => x.Id == view.Id).FirstOrDefault();
				entity.Source = view.Sources.HasItems() ? JsonConvert.SerializeObject(view.Sources) : "";

				await _resolvesService.UpdateAsync(entity);
			}

			return Ok();
		}


		void ValidateRequest(ResolveViewModel model)
		{
			if (model.Sources.HasItems())
			{
				foreach (var item in model.Sources)
				{
					if (item.NoteId > 0)
					{
						var note = _notesService.GetById(item.NoteId);
						if (note == null)
						{
							ModelState.AddModelError("sources", $"錯誤的參考. Note Id: ${item.NoteId}");
							return;
						}
					}
					else
					{
						var term = _termsService.GetById(item.TermId);
						if (term == null)
						{
							ModelState.AddModelError("sources", $"錯誤的參考. Term Id: ${item.TermId}");
							return;
						}
					}
				}
			}
			else
			{
				if (String.IsNullOrEmpty(model.Text) && model.Attachments.IsNullOrEmpty())
				{
					ModelState.AddModelError("text", "必須填寫內容");
					return;
				}
			}
		}

	}
}
