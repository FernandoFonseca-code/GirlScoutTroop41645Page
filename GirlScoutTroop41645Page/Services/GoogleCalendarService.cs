using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace GirlScoutTroop41645Page.Services;

public class GoogleCalendarService
{
    private readonly IConfiguration _configuration;
    public readonly string _tokenPath;
    private readonly string _credentialsPath;
    private readonly string[] _scopes = { CalendarService.Scope.Calendar };
    private readonly string _calendarId;
    private readonly string _applicationName;
    private readonly string _redirectUri = "http://127.0.0.1:8001/authorize/";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GoogleCalendarService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;

        // Set paths to credentials and tokens
        var appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
        _credentialsPath = Path.Combine(appDataPath, "GoogleCalendarCredentials.json");
        _tokenPath = Path.Combine(appDataPath, "GoogleCalendarToken");
        _applicationName = _configuration["GoogleCalendar:ApplicationName"];
        _calendarId = _configuration["GoogleCalendar:CalendarId"];
    }

    public string GetCalendarId()
    {
        return _calendarId;
    }
    public string GetAuthorizationUrl()
    {
        using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);
        var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

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

    public async Task<string> HandleAuthCallbackAsync(string code)
    {
        using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);
        var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = clientSecrets,
            Scopes = _scopes,
            DataStore = new FileDataStore(_tokenPath, true)
        });

        // Exchange authorization code for tokens
        var token = await flow.ExchangeCodeForTokenAsync(
            "user",
            code,
            _redirectUri,
            CancellationToken.None);

        return "Token received and stored successfully";
    }

    public async Task<CalendarService> GetCalendarServiceAsync()
    {
        if (_calendarId == null)
        {
            throw new Exception("Calendar ID is not set in the configuration.");
        }

        using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);
        var clientSecrets = GoogleClientSecrets.FromStream(stream).Secrets;
        
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

    public List<string> GetTokenFiles()
    {
        if (!Directory.Exists(_tokenPath))
        {
            return new List<string> { "Token directory does not exist" };
        }

        return new List<string>(Directory.GetFiles(_tokenPath));
    }

    public bool TestTokenDirectoryAccess()
    {
        if (!Directory.Exists(_tokenPath))
        {
            try
            {
                Directory.CreateDirectory(_tokenPath);

                // Test write access
                string testFile = Path.Combine(_tokenPath, "test.txt");
                File.WriteAllText(testFile, "Test");
                File.Delete(testFile);

                return true;
            }
            catch
            {
                return false;
            }
        }

        try
        {
            string testFile = Path.Combine(_tokenPath, "test.txt");
            File.WriteAllText(testFile, "Test");
            File.Delete(testFile);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> VerifyTokenWorks()
    {
        try
        {
            var service = await GetCalendarServiceAsync();

            // Try to list calendars as a simple test
            var calendarList = await service.CalendarList.List().ExecuteAsync();

            return calendarList != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Event>> GetUpcomingEventsAsync(int maxResults = 10)
    {
        var service = await GetCalendarServiceAsync();

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

    public async Task<Event> CreateEventAsync(Event newEvent)
    {
        var service = await GetCalendarServiceAsync();
        return await service.Events.Insert(newEvent, _calendarId).ExecuteAsync();
    }
}

public class FileDataStore : IDataStore
{
    public FileDataStore(string folder, bool fullPath = false)
    {
        if (fullPath)
        {
            folder = Path.GetFullPath(folder);
        }
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        Folder = folder;
    }

    public string Folder { get; }

    public Task ClearAsync()
    {
        try
        {
            // Delete all files in the folder
            foreach (var file in Directory.GetFiles(Folder))
            {
                File.Delete(file);
            }
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new IOException("Failed to clear the data store.", ex);
        }
    }

    public Task DeleteAsync<T>(string key)
    {
        try
        {
            string filePath = Path.Combine(Folder, key);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to delete data with key '{key}'", ex);
        }
    }

    public Task<T> GetAsync<T>(string key)
    {
        string filePath = Path.Combine(Folder, key);

        if (!File.Exists(filePath))
        {
            return Task.FromResult(default(T));
        }

        string json = File.ReadAllText(filePath);
        T value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        return Task.FromResult(value);
    }

    public Task StoreAsync<T>(string key, T value)
    {
        try
        {
            string filePath = Path.Combine(Folder, key);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            File.WriteAllText(filePath, json);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to store data with key '{key}'", ex);
        }
    }
}