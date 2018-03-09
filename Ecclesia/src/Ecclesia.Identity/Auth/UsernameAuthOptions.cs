using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecclesia.Identity.Auth
{
    public class UsernameAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "username";
        public const string DefaultChallengeScheme = "username";
        public string Scheme => DefaultScheme;
    }
}
