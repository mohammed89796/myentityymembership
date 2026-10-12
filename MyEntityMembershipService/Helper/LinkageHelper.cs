using MyEntityMembershipService.DTOs.Entity.BranchDtos;
using System.Net.Http.Headers;

namespace MyEntityMembershipService.Helper
{
    public static class LinkageHelper
    {
        public static async Task<bool> IsLinkedToProgramAsync(
            CreationCheckDto branchCheckDto,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            //FIXME: This method post package request authorizations from membership service
            //QA link: https://buildup-gateway-dev.buildup-egypt.com
            //Dev Link: https://buildup-gateway-dev.buildup-egypt.com
            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://buildup-gateway-dev.buildup-egypt.com");
            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Authorization token is missing.");
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            var response = await httpClient.PostAsJsonAsync("/MyProgram/1/Programs/IsLinkedToProgram", branchCheckDto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            else
            {
                throw new Exception("Failed to verify branch linkage status.");
            }
        }
    }

}
