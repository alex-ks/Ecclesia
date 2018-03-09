using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecclesia.Identity.Auth
{
    public static class UsernameAuthExtensions
    {
        public static AuthenticationBuilder AddUsernameScheme(
            this AuthenticationBuilder builder)
        {
            // Add custom authentication scheme with custom options and custom handler
            return builder.AddScheme<UsernameAuthOptions, UsernameAuthHandler>(
                UsernameAuthOptions.DefaultScheme,
                options => { });
        }
    }
}
