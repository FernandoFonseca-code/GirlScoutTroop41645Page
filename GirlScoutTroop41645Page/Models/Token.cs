using Azure.Core;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Plugins;
using System.Runtime.Intrinsics.X86;
namespace GirlScoutTroop41645Page.Models;

public class Token
{
    public Token()
    {
        GeneratedOn = DateTime.Now;
    }
    public string access_token { get; set; }
    public  int expires_in { get; set; }
    public string token_type { get; set; }
    public string scope { get; set; }
    public string refresh_token { get; set; }
    public DateTime GeneratedOn { get; set; }
    public bool IsTokenExpired 
    { get
        {
            return GeneratedOn.AddSeconds(expires_in) <= DateTime.Now;
        }
    }
}