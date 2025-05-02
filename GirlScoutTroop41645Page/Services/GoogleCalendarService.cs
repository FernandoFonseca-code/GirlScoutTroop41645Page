using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

public class GoogleCalendarService
{
    private readonly IConfiguration _configuration;
    private readonly string _tokenPath;
    private readonly string[] _scopes = { CalendarService.Scope.Calendar };
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _calendarId;
    private readonly string _applicationName;
    private readonly string _redirectUri = "https://127.0.0.1:8001/authorize/";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GoogleCalendarService> _logger;

    public GoogleCalendarService(
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<GoogleCalendarService> logger)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;

        // Use a directory in the app for tokens
        _tokenPath = _configuration["Google:calendar_token"];

        // Make sure the directory exists
        if (!Directory.Exists(_tokenPath))
        {
            Directory.CreateDirectory(_tokenPath);
        }

        _clientId = _configuration["Google:client_id"];
        _clientSecret = _configuration["Google:client_secret"];
        _applicationName = _configuration["GoogleCalendar:ApplicationName"];
        _calendarId = _configuration["GoogleCalendar:CalendarId"];
    }

    // Then modify GetAuthorizationUrl() and GetCalendarServiceAsync() to use
    // ClientSecrets directly instead of loading from a file
    public string GetAuthorizationUrl()
    {
        var clientSecrets = new ClientSecrets
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret
        };

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets,
            Scopes = _scopes,
            DataStore = new FileDataStore(_tokenPath, true)
        });

        // Generate the authorization URL
        var uri = flow.CreateAuthorizationCodeRequest(_redirectUri).Build();
        return uri.ToString();
    }

    public async Task<CalendarService> GetCalendarServiceAsync()
    {
        if (_calendarId == null)
        {
            throw new Exception("Calendar ID is not set in the configuration.");
        }

        var clientSecrets = new ClientSecrets
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret
        };

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets,
            Scopes = _scopes,
            DataStore = new FileDataStore(_tokenPath, true)
        });

        // Try to get stored credential
        var credential = await flow.LoadTokenAsync("user", CancellationToken.None);

        // If no stored token or token is expired, redirect to authorization
        if (credential == null || credential.IsExpired(flow.Clock))
        {
            // Store the current request path to redirect back after authorization
            _httpContextAccessor.HttpContext.Response.Redirect(GetAuthorizationUrl());
            return null;
        }

        // Create a user credential from the stored token
        var userCredential = new UserCredential(flow, "user", new TokenResponse
        {
            AccessToken = credential.AccessToken,
            RefreshToken = credential.RefreshToken,
            ExpiresInSeconds = credential.ExpiresInSeconds,
            TokenType = credential.TokenType
        });

        // Create Google Calendar API service
        var service = new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = userCredential,
            ApplicationName = _applicationName,
        });

        return service;
    }

    public async Task<List<Event>> GetUpcomingEventsAsync(int maxResults = 10)
    {
        try
        {
            var service = await GetCalendarServiceAsync();
            if (service == null) return new List<Event>();

            // Define parameters of the request
            var request = service.Events.List(_calendarId);
            request.TimeMinDateTimeOffset = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = maxResults;
            request.OrderBy = Google.Apis.Calendar.v3.EventsResource.ListRequest.OrderByEnum.StartTime;

            // Fetch events
            var events = await request.ExecuteAsync();
            return events.Items != null ? events.Items.ToList() : new List<Event>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching upcoming events");
            return new List<Event>();
        }
    }

    public async Task<Event> CreateEventAsync(Event newEvent)
    {
        var service = await GetCalendarServiceAsync();
        if (service == null) throw new InvalidOperationException("Calendar service not available");

        return await service.Events.Insert(newEvent, _calendarId).ExecuteAsync();
    }
}
