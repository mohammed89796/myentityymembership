using Microsoft.Extensions.Caching.Memory;
using MyEntityMembershipService.DTOs.Membership.UserDtos;
using MyEntityMembershipService.Extensions;
using MyEntityMembershipService.Repositories.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net.Http.Headers;

namespace MyEntityMembershipService.Services.Membership.PublishService
{
    public class PublishService : IPublishService
    {
        private readonly HttpClient _httpClient;
        private readonly string _gatewayUrl;
        private readonly string _orchestrationUrl;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache _cache;
        private const string TokenCacheKey = "STS:AccessToken";
        public PublishService(IHttpClientFactory clientFactory, IConfiguration configuration, IMemoryCache cache)
        {
            _httpClient = clientFactory.CreateClient("ResilientClientWithoutTimeOut");
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _gatewayUrl = configuration["ApiSettings:Gateway"] ?? throw new ArgumentNullException(nameof(configuration), "Gateway URL is not configured.");
            _orchestrationUrl = configuration["Orchestration:OrchestrationUrl"] ?? throw new ArgumentNullException(nameof(configuration), "OrchestrationUrl URL is not configured.");
            _clientFactory = clientFactory;
            this.configuration = configuration;
            _cache = cache;
        }

        public async Task<Response<string>> PublishUserData(UserDataDto userDataDto)
        {


            Log.Information($"Publish User Data request : {JsonConvert.SerializeObject(userDataDto)}");
            var token = await GetSTSTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"{_gatewayUrl}{_orchestrationUrl}";
            var requestData = await _httpClient.PostAsJson<UserDataDto, Response<object>>(url, userDataDto);
            Log.Information($"publish user response: {requestData?.Message ?? "No Message Found"}");
            if (!(requestData?.Succeeded ?? true))
                return new ResponseHandler().BadRequest<string>(requestData.Message);

            return new ResponseHandler().Success(requestData?.Message ?? "No Message Found");


        }

        private async Task<string> GetSTSTokenAsync()
        {
            if (_cache.TryGetValue(TokenCacheKey, out string? cachedToken) && !string.IsNullOrWhiteSpace(cachedToken))
            {
                return cachedToken;
            }

            var tokenRequest = new Dictionary<string, string>
            {
                { "client_id", "x_inity_mobile_app_react" },
                { "client_secret", "X-!n!ty_m0bile_@pP2o24&x" },
                { "grant_type", "client_credentials" },
                { "scope", "x_inity_mobile_app" }
            };

            var client = _clientFactory.CreateClient();
            var content = new FormUrlEncodedContent(tokenRequest);
            var response = await client.PostAsync($"{configuration["ApiSettings:IdentityUrl"]}/connect/token", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to obtain access token.");

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<JObject>(responseContent);

            var accessToken = tokenResponse["access_token"]?.ToString();
            var expiresIn = tokenResponse["expires_in"]?.ToObject<int>() ?? 3600; 

            _cache.Set(TokenCacheKey, accessToken, TimeSpan.FromSeconds(expiresIn - 30));

            return accessToken!;
        }

    }
}

