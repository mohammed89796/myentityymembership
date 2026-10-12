namespace  MyEntityMembershipService.Errors
{
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiErrorResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessage(statusCode);
        }

        public string? GetDefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request!",
                401 => "UnAuthorized!",
                404 => "Resources not found!",
                500 => "There is server Error",
                _ => null
            };
        }
    }
}
