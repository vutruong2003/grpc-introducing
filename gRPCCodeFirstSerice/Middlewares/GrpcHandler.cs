using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;

namespace gRPCCodeFirstSerice.Middlewares
{
    public class GrpcRequirement : IAuthorizationRequirement
    {

    }

    public class GrpcHandler : AuthorizationHandler<GrpcRequirement>
    {
        private const string SecretKeyField = "secretkey";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public GrpcHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GrpcRequirement requirement)
        {
            var requiredKey = _configuration["SecretKey"];
            if (!string.IsNullOrEmpty(requiredKey))
            {
                var headers = _httpContextAccessor.HttpContext.Request.Headers;
                var hasKey = headers.TryGetValue(SecretKeyField, out var secretkey);

                if (!hasKey || !secretkey.Any(x => x == requiredKey))
                {
                    context.Fail(new AuthorizationFailureReason(this, "Unauthorization"));
                    return;
                }
            }

            context.Succeed(requirement);

        }
    }
}
