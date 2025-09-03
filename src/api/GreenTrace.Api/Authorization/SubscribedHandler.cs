using System.Security.Claims;
using GreenTrace.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace GreenTrace.Api.Authorization;

public class SubscribedHandler(ISubscriptionService subscriptions) : AuthorizationHandler<SubscribedRequirement>
{
    private readonly ISubscriptionService _subs = subscriptions;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SubscribedRequirement requirement)
    {
        // Admins bypass subscription requirement
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        var subClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ?? context.User.FindFirst("sub");
        if (subClaim == null) return;
        if (!Guid.TryParse(subClaim.Value, out var userId)) return;

        if (await _subs.HasActiveAsync(userId) || await _subs.HasValidThroughAsync(userId))
        {
            context.Succeed(requirement);
        }
    }
}
