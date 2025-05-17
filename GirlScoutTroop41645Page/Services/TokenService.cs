using GirlScoutTroop41645Page.Models;
using RestSharp;
using System.Text.Json;

namespace GirlScoutTroop41645Page.Services;

public class TokenService : ITokenService
{
    private readonly IRestClient _restClient;
    private readonly IConfiguration _configuration;
    private readonly string tokenFilePath = "D:\\Fernando Fonseca\\GirlScoutTroop41645Page\\GirlScoutTroop41645Page\\AppData\\Token.json";
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _restClient = new RestClient("https://oauth2.googleapis.com/token");
        
    }
    public async Task<string> GetAccessTokenAsync()
    {
        var token = GetToken();
        if (token.IsTokenExpired)
        {
            token = await RefreshTokenAsync();
        }
        return token.access_token;
    }

    public async Task<Token> GetTokenAsync(string code)
    {
        var restRequest = new RestRequest();
        restRequest.AddQueryParameter("code", code);
        restRequest.AddQueryParameter("client_id", _configuration.GetValue<string>("GoogleCalendar:ClientId"));
        restRequest.AddQueryParameter("client_secret", _configuration.GetValue<string>("GoogleCalendar:ClientSecret"));
        restRequest.AddQueryParameter("redirect_uri", _configuration.GetValue<string>("GoogleCalendar:RedirectUri"));
        restRequest.AddQueryParameter("grant_type", "authorization_code");

        var response = await _restClient.PostAsync<Token>(restRequest);
        SaveToken(response);
        return response;
    }

    private async Task<Token> RefreshTokenAsync()
    {
        var token = GetToken();
        var restRequest = new RestRequest();
        restRequest.AddQueryParameter("refresh_token", token.refresh_token);
        restRequest.AddQueryParameter("client_id", _configuration.GetValue<string>("GoogleCalendar:ClientId"));
        restRequest.AddQueryParameter("client_secret", _configuration.GetValue<string>("GoogleCalendar:ClientSecret"));
        restRequest.AddQueryParameter("grant_type", "refresh_token");

        var response = await _restClient.PostAsync<Token>(restRequest);
        response.refresh_token = token.refresh_token;
        SaveToken(response);
        return response;
    }
    private void SaveToken(Token token)
    {
        System.IO.File.WriteAllText(tokenFilePath, JsonSerializer.Serialize(token));
    }

    private Token GetToken()
    {
        var tokenContent = System.IO.File.ReadAllText(tokenFilePath);
        return JsonSerializer.Deserialize<Token>(tokenContent);
    }
}

