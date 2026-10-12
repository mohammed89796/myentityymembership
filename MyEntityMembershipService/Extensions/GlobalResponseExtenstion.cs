using MyEntityMembershipService.Repositories.Common;
using System.Net;

namespace MyEntityMembershipService.Extensions
{
    public static class GlobalResponseExtension
    {
        public static IResult GetResponse<T>(this Response<T> response) =>
         response.StatusCode switch
         {
             HttpStatusCode.OK => Results.Ok(response),
             HttpStatusCode.Created => Results.Created(string.Empty, response),
             HttpStatusCode.Unauthorized => Results.Unauthorized(),
             HttpStatusCode.BadRequest => Results.BadRequest(response),
             HttpStatusCode.NotFound => Results.NotFound(response),
             HttpStatusCode.Accepted => Results.Accepted(string.Empty, response),
             HttpStatusCode.UnprocessableEntity => Results.UnprocessableEntity(response),
             _ => Results.BadRequest(response)
         };
    }
}
