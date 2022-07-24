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
using Web.Controllers;
using Web.Models;

namespace Web.Controllers.Admin
{
	public class ExceptionsController : BaseAdminController
	{
		private readonly ApplicationCore.Logging.IAppLogger _logger;

		public ExceptionsController(ApplicationCore.Logging.IAppLogger logger)
		{
			_logger = logger;
		}

		[HttpGet("")]
		public ActionResult Index(string start = "", string end = "", string type = "" , int page = 1, int pageSize = 10)
		{
			var model = new ExceptionsAdminModel();

			var records = _logger.FetchAllExceptions();
			if (records.IsNullOrEmpty()) return Ok(model);

			if (page < 0) //首次載入
			{
				page = 1;
				string startDate = records.Select(item => item.DateTime).Min().ToDateString();
				string endDate = records.Select(item => item.DateTime).Max().ToDateString();

				model.Period = new List<string> { startDate , endDate  };
			}

			if (start.HasValue() || end.HasValue())
			{
				var startDate = start.ToStartDate();
				if (!startDate.HasValue) startDate = DateTime.MinValue;

				var endDate = end.ToEndDate();
				if (!endDate.HasValue) endDate = DateTime.MaxValue;


				records = records.Where(x => x.DateTime >= startDate && x.DateTime <= endDate);
			}

			if(type.HasValue()) records = records.Where(x => x.TypeName.EqualTo(type));

			records = records.GetOrdered().ToList();

			var types = records.Select(item => item.TypeName).Distinct();

			model.Types = types.ToList();

			model.PagedList = records.GetPagedList(page, pageSize);

			return Ok(model);
		}

	}
}
