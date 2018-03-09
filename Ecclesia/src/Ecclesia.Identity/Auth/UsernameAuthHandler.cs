using Ecclesia.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Ecclesia.Identity.Auth
{
    class UsernameAuthHandler : AuthenticationHandler<UsernameAuthOptions>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _principalFactory;

        public UsernameAuthHandler(
            IOptionsMonitor<UsernameAuthOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> principalFactory)
            : base(options, logger, encoder, clock)
        {
            _userManager = userManager;
            _principalFactory = principalFactory;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get Authorization header value
            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            {
                return AuthenticateResult.Fail("Cannot read authorization header");
            }

            if (authorization.Count != 1)
            {
                return AuthenticateResult.Fail("Too many authorization headers");
            }

            try
            {
                var user = await _userManager.FindByNameAsync(authorization[0]);

                var claims = await _principalFactory.CreateAsync(user);
                var ticket = new AuthenticationTicket(claims, Options.Scheme);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail(e.Message);
            }
        }
    }
}
