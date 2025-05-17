using GirlScoutTroop41645Page.Models;

namespace GirlScoutTroop41645Page.Services;

public interface ITokenService
{
    Task<Token> GetTokenAsync(string code);
    Task<string> GetAccessTokenAsync();
}
