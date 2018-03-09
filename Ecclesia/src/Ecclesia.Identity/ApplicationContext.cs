using Ecclesia.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecclesia.Identity
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {

    }
}
