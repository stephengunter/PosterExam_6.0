using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Authorization
{
	public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
	{

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
		{
			Permissions permissione = requirement.Permission;

			if (permissione == Permissions.Subscriber)
			{
				if(context.User.Claims.IsSubscriber())
				{
					context.Succeed(requirement);
					return Task.CompletedTask;
				} 
			}
			else if(permissione == Permissions.Admin)
			{
				if(context.User.Claims.IsBoss() || context.User.Claims.IsDev())
				{
					context.Succeed(requirement);
					return Task.CompletedTask;
				}
				
			}

			context.Fail();
			return Task.CompletedTask;
		}
	}
}
