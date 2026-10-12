using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MyEntityMembershipService.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return System.Text.Json.JsonSerializer.Deserialize<T>(dataAsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = false });
        }
        public static async Task<T> GetDataAsync<T>(this HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);
            return await response.ReadContentAs<T>();
        }

        
        
        public static async Task<dynamic> GetDataAsync(this HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);
            return await response.ReadContentAs<dynamic>();
        }




        public static async Task<dynamic> PostAsJson<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // content.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.GetAuth().access_token);

            var response = await httpClient.PostAsync(url, content);

            return await response.ReadContentAs<dynamic>();
        }
        public static async Task<TResult> PostAsJson<T, TResult>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // content.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.GetAuth().access_token);

            var response = await httpClient.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();



            return JsonConvert.DeserializeObject<TResult>(result)!;
        }
        public static async Task<TResult> PutAsJson<T, TResult>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.PutAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(result)!;
        }

        public static Task<HttpResponseMessage> PutAsJson<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PutAsync(url, content);
        }


        public static async Task<dynamic> ReadContentAsDynamic<T>(this HttpResponseMessage response)
        {
            //if (!response.IsSuccessStatusCode)
            //    return false;

            var dataAsString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(dataAsString);
        }
        public static async Task<dynamic> GetDataAsyncDynamic(this HttpClient httpClient, string url)
        {
            var response = await httpClient.GetAsync(url);
            return await response.ReadContentAsDynamic<dynamic>();
        }



        public static async Task<dynamic> PostAsJsonDynamic<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // content.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.GetAuth().access_token);

            var response = await httpClient.PostAsync(url, content);

            return await response.ReadContentAsDynamic<dynamic>();
        }
    }
}
