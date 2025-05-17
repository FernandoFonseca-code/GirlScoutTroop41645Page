using GirlScoutTroop41645Page.Models;
using GirlScoutTroop41645Page.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;

namespace GirlScoutTroop41645Page.Controllers;

public class CalendarController : Controller
{
    private readonly ITokenService _tokenService;
    public CalendarController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
    [HttpGet("token")]
    public async Task<string> GetAccessTokenAsync()
    {
        return await _tokenService.GetAccessTokenAsync();
    }
}