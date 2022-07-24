using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Helpers;
using System.Security.Claims;

namespace ApplicationCore.Authorization
{
	public static class IdendityHelpers
	{
		public static string UserId(this IEnumerable<Claim> claims)
		{
			var entity = claims.FirstOrDefault(c => c.Type == ClaimsTypeKeys.Id);
			if (entity == null) return "";

			return entity.Value;
		}

		public static IEnumerable<string> Roles(this IEnumerable<Claim> claims)
		{
			var entity = claims.FirstOrDefault(c => c.Type == ClaimsTypeKeys.Roles);
			if (entity == null) return null;


			return entity.Value.Split(',');
		}

		public static string UserName(this IEnumerable<Claim> claims)
		{
			var entity = claims.FirstOrDefault(c => c.Type == ClaimsTypeKeys.Sub);
			if (entity == null) return "";

			return entity.Value;
		}

		public static bool IsDev(this IEnumerable<Claim> claims)
		{
			var roles = Roles(claims);
			if (roles.IsNullOrEmpty()) return false;

			string devRoleName = AppRoles.Dev.ToString();
			var match = roles.Where(r => r.EqualTo(devRoleName)).FirstOrDefault();

			return match != null;
		}
		public static bool IsBoss(this IEnumerable<Claim> claims)
		{
			var roles = Roles(claims);
			if (roles.IsNullOrEmpty()) return false;

			string bossRoleName = AppRoles.Boss.ToString();
			var match = roles.Where(r => r.EqualTo(bossRoleName)).FirstOrDefault();

			return match != null;
		}
		public static bool IsSubscriber(this IEnumerable<Claim> claims)
		{
			var roles = Roles(claims);
			if (roles.IsNullOrEmpty()) return false;

			string subscriberRoleName = AppRoles.Subscriber.ToString();
			var match = roles.Where(r => r.EqualTo(subscriberRoleName)).FirstOrDefault();

			return match != null;
		}

	}
}
