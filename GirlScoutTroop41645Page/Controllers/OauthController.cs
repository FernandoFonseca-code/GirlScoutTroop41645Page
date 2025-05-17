using GirlScoutTroop41645Page.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace GirlScoutTroop41645Page.Controllers;

public class OauthController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    public OauthController(IConfiguration configuration, ITokenService tokenService)
    {
        _configuration = configuration;
        _tokenService = tokenService;
    }
    public IActionResult Authorize()
    {
        var url = $"{_configuration.GetValue<string>("GoogleCalendar:AuthorizationEndpoint")}" +
            $"scope={_configuration.GetValue<string>("GoogleCalendar:Scope")}" +
            $"&access_type=offline" +
            $"&response_type=code" +
            $"&state=TroopWebMaster" +
            $"&redirect_uri={_configuration.GetValue<string>("GoogleCalendar:RedirectUri")}" +
            $"&client_id={_configuration.GetValue<string>("GoogleCalendar:ClientId")}";
        
        return Redirect(url);
    }
    public async Task Callback(string code, string state)
    {
        await _tokenService.GetTokenAsync(code);
    }
}
