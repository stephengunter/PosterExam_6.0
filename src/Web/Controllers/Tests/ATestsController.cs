using ApplicationCore.Helpers;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using ApplicationCore.ViewServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using ApplicationCore.DataAccess;
using System.IO;
using ApplicationCore.Models;
using Newtonsoft.Json;

namespace Web.Controllers.Tests
{
    public class ATestsController : BaseTestController
    {
        private readonly AppSettings _appSettings;
        public ATestsController(IOptions<AppSettings> appSettings)
        {

            _appSettings = appSettings.Value;
        }



        [HttpGet]
        public async Task<ActionResult>  Index()
        {
            
            return Ok();
        }


        [HttpGet("version")]
        public ActionResult Version()
        {
            return Ok(_appSettings.ApiVersion);
        }


        [HttpGet("ex")]
        public ActionResult Ex()
        {
            throw new System.Exception("Test Throw Exception");
        }
    }
}
