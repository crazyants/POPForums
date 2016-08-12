﻿using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PopForums.Services;

namespace PopForums.Web
{
    public class PopForumsMiddleware
    {
	    private readonly RequestDelegate _next;

	    public PopForumsMiddleware(RequestDelegate next)
	    {
		    _next = next;
	    }

	    public async Task Invoke(HttpContext context)
	    {
		    var name = context.User.Identity.Name;
		    if (!string.IsNullOrWhiteSpace(name))
		    {
			    var userService = context.RequestServices.GetService<IUserService>();
			    var user = userService.GetUserByName(name);
			    if (user != null)
				{
					foreach (var role in user.Roles)
						((ClaimsIdentity)context.User.Identity).AddClaim(new Claim("ForumClaims", role));
					context.Items["PopForumsUser"] = user;
				    var profileService = context.RequestServices.GetService<IProfileService>();
				    var profile = profileService.GetProfile(user);
				    context.Items["PopForumsProfile"] = profile;
					// TODO: update last hit here on user record
					// TODO: update session
			    }
		    }
		    await _next(context);
	    }
    }
}
