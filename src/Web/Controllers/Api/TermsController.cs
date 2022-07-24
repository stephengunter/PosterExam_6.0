using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.ViewServices;
using Microsoft.AspNetCore.Authorization;
using Web.Controllers;

namespace Web.Controllers.Api
{
	[Authorize]
	public class TermsController : BaseApiController
	{
		private readonly IMapper _mapper;
		private readonly ITermsService _termsService;

		public TermsController(ITermsService termsService, IMapper mapper)
		{
			_mapper = mapper;
			_termsService = termsService;
		}

		[HttpGet("{id}")]
		public ActionResult Details(int id)
		{
			var term = _termsService.GetById(id);
			if (term == null) return NotFound();


			return Ok(term.MapViewModel(_mapper));
		}
	}
}
