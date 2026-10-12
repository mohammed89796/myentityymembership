using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Elastic.Apm.Api;
using MyEntityMembershipService.Authorization.Permissions.Dto;
using Serilog;

namespace MyEntityMembershipService.Authorization.AuthorizationService
{
    internal class AuthorizationService : IAuthorizationService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _resilientClient;

        public AuthorizationService(IConfiguration configuration, IHttpClientFactory factory)
        {
            _configuration = configuration;
            _resilientClient = factory.CreateClient("ResilientClient");

        }

        public async Task<bool> IsAuthAsync(AuthorizationRequestDto dto)
        {
            var baseUrl = _configuration["ApiSettings:Gateway"];
            var endpoint = $"{baseUrl}{_configuration["Authorization:CheckAuthorization"]}";
            try
            {
                var httpResponse = await _resilientClient.PostAsJsonAsync(endpoint, dto);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var error = await httpResponse.Content.ReadAsStringAsync();
                    Log.Error($"Error Check Authorization: {error}");

                    return false;
                }

                var json = await httpResponse.Content.ReadAsStringAsync();
                Log.Information($"Response: {json}");

                var result = JsonSerializer.Deserialize<Response<bool>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Data ?? false;
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occurred while registering service: {ex.Message}");

                return false;
            }

        }
        public async Task<bool> IsAuthorizedAsync(string userId, string permissionCode)
        {
            
            var baseUrl = _configuration["ApiSettings:Gateway"];
            var endpoint = $"{baseUrl}{_configuration["Authorization:CheckPermission"]}?userId={Uri.EscapeDataString(userId)}&permissionCode={Uri.EscapeDataString(permissionCode)}";
            try
            {
                var httpResponse = await _resilientClient.GetAsync(endpoint);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var error = await httpResponse.Content.ReadAsStringAsync();
                    Log.Error($"Error Check Authorization: {error}");

                    return false;
                }

                var json = await httpResponse.Content.ReadAsStringAsync();
                Log.Information($"Response: {json}");

                var result = JsonSerializer.Deserialize<Response<bool>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Data ?? false;
            }
            catch (Exception ex)
            {
                Log.Error($"Exception occurred while registering service: {ex.Message}");

                return false;
            }

        }

        public async Task RegisterServiceAsync(ServicePermissionsDto servicePermissionsDto)
        {
            var token = await GetTokenAsync();
            var endpoint = $"{_configuration["ApiSettings:Gateway"]}{_configuration["Authorization:RegisterService"]}";
            var json = JsonSerializer.Serialize(servicePermissionsDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _resilientClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var httpResponse = await _resilientClient.PostAsync(endpoint, content);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var error = await httpResponse.Content.ReadAsStringAsync();
                    Log.Error($"Error registering service: {error}");

                }

                var responseBody = await httpResponse.Content.ReadAsStringAsync();
                Log.Information($"Response: {responseBody}");


            }
            catch (Exception ex)
            {
                Log.Error($"Exception occurred while registering service: {ex.Message}");


            }
        }
        private async Task<string> GetTokenAsync()
        {

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["ApiSettings:IdentityUrl"]}/connect/token");

            var collection = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", "x_inity_mobile_app_react"),
                new KeyValuePair<string, string>("client_secret", "X-!n!ty_m0bile_@pP2o24&x"),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "x_inity_mobile_app")
            };

            request.Content = new FormUrlEncodedContent(collection);

            try
            {
                var response = await _resilientClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Log.Error($"Failed to obtain token: {content}");
                    return null!;

                }

                // Parse the response and extract the access_token
                var tokenData = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                if (tokenData != null && tokenData.ContainsKey("access_token"))
                {
                    return tokenData["access_token"].ToString(); // Return only the access token
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"Error while requesting token: {ex.Message}");
                return null;
            }

        }
    }
}
