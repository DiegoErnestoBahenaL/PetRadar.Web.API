using Microsoft.AspNetCore.Mvc;

namespace PetRadar.Web.API
{
    public static class Constants
    {

        public const string ASPNetcoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        public const string DevEnvironment = "Development";
        public const string LocalIntegrationEnvironment = "LocalIntegration";

        public const string MigrationsAssembly = "PetRadar.DbMigrations";

        public const string NotFoundTitle = "Not found";
        public const string NotFoundDetail = "The requested resource was not found.";

        public const string BadRequestTitle = "Bad request";

        public static ProblemDetails NotFoundProblemDetails => new()
        {
            Title = NotFoundTitle,
            Detail = NotFoundDetail,
            Status = StatusCodes.Status404NotFound
        };

        public static ProblemDetails BadRequestProblemDetails(string detail) => new()
        {
            Title = BadRequestTitle,
            Detail = detail,
            Status = StatusCodes.Status400BadRequest
        };

        public static ProblemDetails InternalServerErrorProblemDetails(string detail) => new()
        {
            Title = "Internal Server Error",
            Detail = detail,
            Status = StatusCodes.Status500InternalServerError
        };


    }
}
