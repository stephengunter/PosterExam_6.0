using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Auth;
using ApplicationCore.Services;
namespace Web.Controllers
{
	public class AuthController : BaseController
	{
		private readonly IUsersService _usersService;
		private readonly IAuthService _authService;


		public AuthController(IUsersService usersService, IAuthService authService)
		{
			_usersService = usersService;
			_authService = authService;
		}

		[HttpPost("")]
		public async Task<ActionResult> Login([FromBody] LoginRequest model)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var user = await _usersService.FindUserByEmailAsync(model.Username);
			if (user != null)
			{
				if (await _usersService.CheckPasswordAsync(user, model.Password))
				{
					var roles = await _usersService.GetRolesAsync(user);
					var responseView = await _authService.CreateTokenAsync(RemoteIpAddress, user, roles);

					return Ok(responseView);
				}
			}

			ModelState.AddModelError("", "身分驗證失敗. 請重新登入");
			return BadRequest(ModelState);

		}

		//POST api/auth/refreshtoken
		[HttpPost("refreshtoken")]
		public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
		{
			var cp = _authService.ResolveClaimsFromToken(model.AccessToken);
			string userId = cp.GetUserId();
			OAuthProvider oauthProvider = cp.GetOAuthProvider();

			ValidateRequest(model, userId);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var user = await _usersService.FindUserByIdAsync(userId);
			var oauth = _authService.FindOAuthByProvider(userId, oauthProvider);
			var roles = await _usersService.GetRolesAsync(user);

			var responseView = await _authService.CreateTokenAsync(RemoteIpAddress, user, oauth, roles);

			return Ok(responseView);

		}

		void ValidateRequest(RefreshTokenRequest model, string userId)
		{
			bool isValid = _authService.IsValidRefreshToken(model.RefreshToken, userId);
			if(!isValid) ModelState.AddModelError("token", "身分驗證失敗. 請重新登入");

		}



	}

}