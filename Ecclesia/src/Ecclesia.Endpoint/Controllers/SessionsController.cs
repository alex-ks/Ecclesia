﻿using Ecclesia.DataAccessLayer;
using Ecclesia.DataAccessLayer.Models;
using Ecclesia.Endpoint.Models;
using Ecclesia.Identity.Models;
using Ecclesia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecclesia.Endpoint.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SessionsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SessionManager _sessionManager;

        private Task<ApplicationUser> AppUser() => _userManager.GetUserAsync(HttpContext.User);

        public SessionsController(UserManager<ApplicationUser> userManager, SessionManager sessionManager)
        {
            _userManager = userManager;
            _sessionManager = sessionManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSessionAsync([FromBody] SessionStartRequest startRequest)
        {
            var user = await AppUser();
            var sessionId = await _sessionManager.StartSessionAsync(user, startRequest);

            return Ok(new { SessionId = sessionId });
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetSessionStatusAsync(long id)
        {
            try
            {
                return Ok(await _sessionManager.GetSessionStatusAsync(await AppUser(), id));
            }
            catch (KeyNotFoundException)
            {
                return BadRequest(EndpointResources.SessionNotFoundMsg);
            }
        }

        [HttpGet]
        public async Task<IEnumerable<ResponseSessionStatus>> GetSessionsAsync()
        {
            return _sessionManager.GetSessions(await AppUser());
        }
    }
}
