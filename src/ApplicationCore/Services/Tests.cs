using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Logging;
using ApplicationCore.Exceptions;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using System.Text;
using ApplicationCore;

namespace ApplicationCore.Services
{
    public interface ITestsService
    {
        Task<AuthResponse> LoginAsync(string remoteIp);
        Task RemoveSubsrcibesFromUserAsync();
        Task RemoveBillsFromUserAsync();

        void RemoveBill(int id);
    }

    public class TestsService : ITestsService
    {
        private readonly IAppLogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IBillsService _billsService;
        private readonly IPaysService _paysService;
        private readonly ISubscribesService _subscribesService;
        private readonly IUsersService _usersService;
        private readonly IAuthService _authService;

        public TestsService(IAppLogger logger, IOptions<AppSettings> appSettings, 
             IPaysService paysService, IBillsService billsService,
            ISubscribesService subscribesService, IUsersService usersService, IAuthService authService)
        {
            _logger = logger;
            _appSettings = appSettings.Value;

            
            _paysService = paysService;
            _billsService = billsService;
            _subscribesService = subscribesService;

            _usersService = usersService;
            _authService = authService;
        }

        User _testUser = null;
        User TestUser
        {
            get
            {
                if (_testUser == null) _testUser = _usersService.FindUserByEmailAsync("traders.com.tw@gmail.com").Result;
                return _testUser;
            }
        }

        public async Task Test()
        {
            var bills = await _billsService.FetchByUserAsync(TestUser.Id);
            foreach (var bill in bills)
            {
                bill.DeadLine = DateTime.Now.AddDays(-1);
                await _billsService.UpdateAsync(bill);

                _billsService.Remove(bill);
            }
        }

        public async Task<AuthResponse> LoginAsync(string remoteIp)
        {
            var user = TestUser;
            var roles = await _usersService.GetRolesAsync(user);
            var oAuth = _authService.FindOAuthByProvider(user.Id, OAuthProvider.Google);
            return await _authService.CreateTokenAsync(remoteIp, user, oAuth, roles);
        }

        public async Task RemoveSubsrcibesFromUserAsync()
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

        public async Task RemoveBillsFromUserAsync()
        {
            string userId = TestUser.Id;
            var bills = await _billsService.FetchByUserAsync(userId);

            foreach (var bill in bills)
            {
                _billsService.Remove(bill);
            }

        }

        public void RemoveBill(int id)
        {
            var bill = _billsService.GetById(id);
            if(bill == null) throw new EntityNotFoundException(new Bill { Id = id });
            if(bill.UserId != TestUser.Id) throw new Exception();

            _billsService.Remove(bill);
        }

    }
}
