

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
using ApplicationCore.Models;
using Newtonsoft.Json;
using ApplicationCore.Exceptions;
using System;

namespace Web.Controllers.Tests
{
	public class ASubsrcibesTestController : BaseTestController
	{
		private readonly AdminSettings _adminSettings;
		private readonly IUsersService _usersService;
		private readonly ISubscribesService _subscribesService;
		private readonly IBillsService _billsService;
		private readonly IAuthService _authService;

		
		public ASubsrcibesTestController(IOptions<AdminSettings> adminSettings, IUsersService usersService, 
			ISubscribesService subscribesService, IBillsService billsService, IAuthService authService)			
		{
			_adminSettings = adminSettings.Value;
			_usersService = usersService;
			_subscribesService = subscribesService;
			_billsService = billsService;
			_authService = authService;

			
		}

		User _testUser = null;
		User TestUser
		{
			get
			{
				if (_testUser == null) _testUser = _usersService.FindUserByEmailAsync(_adminSettings.Email).Result;
				return _testUser;
			}
		}

		[HttpPost]
		public async Task<ActionResult> Test([FromBody] AdminRequest model)
		{
			if (model.Key != _adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");
			if (string.IsNullOrEmpty(model.Cmd)) ModelState.AddModelError("cmd", "指令錯誤");
			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (model.Cmd.EqualTo("remove-subsrcibes"))
			{
				await RemoveSubsrcibesFromUserAsync();

				await RemoveBillsFromUserAsync();
			}
			else if (model.Cmd.EqualTo("remove-bill"))
			{
				var billView = JsonConvert.DeserializeObject<BillViewModel>(model.Data);
				RemoveBill(billView.Id);
			}
			else if (model.Cmd.EqualTo("login"))
			{
				var responseView = await LoginAsync(RemoteIpAddress);

				return Ok(responseView);
			}
			else
			{
				ModelState.AddModelError("cmd", "指令錯誤");
				return BadRequest(ModelState);
			}

			return Ok($"{model.Cmd} - OK");
		}

		async Task RemoveSubsrcibesFromUserAsync()
		{
			var subscribes = await _subscribesService.FetchByUserAsync(TestUser.Id);
			foreach (var subscribe in subscribes)
			{
				await RemoveSubsrcibeAsync(subscribe);
			}
		}

		async Task RemoveSubsrcibeAsync(Subscribe subscribe)
		{
			string userId = subscribe.UserId;

			await _usersService.RemoveSubscriberRoleAsync(userId);

			await _subscribesService.RemoveAsync(subscribe);

		}

		async Task RemoveBillsFromUserAsync()
		{
			string userId = TestUser.Id;
			var bills = await _billsService.FetchByUserAsync(userId);

			foreach (var bill in bills)
			{
				_billsService.Remove(bill);
			}

		}

		async Task<AuthResponse> LoginAsync(string remoteIp)
		{
			var user = TestUser;
			var roles = await _usersService.GetRolesAsync(user);
			var oAuth = _authService.FindOAuthByProvider(user.Id, OAuthProvider.Google);
			return await _authService.CreateTokenAsync(remoteIp, user, oAuth, roles);
		}

		void RemoveBill(int id)
		{
			var bill = _billsService.GetById(id);
			if (bill == null) throw new EntityNotFoundException(new Bill { Id = id });
			if (bill.UserId != TestUser.Id) throw new Exception();

			_billsService.Remove(bill);
		}

	}

}