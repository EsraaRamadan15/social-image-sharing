using Microsoft.AspNetCore.Authorization;
using Posts.Domain.Entities;
using System.Security.Claims;

namespace Posts.Application.Authorization
{
    public sealed class PostOwnerOrAdminHandler
        : AuthorizationHandler<PostOwnerOrAdminRequirement, Post>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PostOwnerOrAdminRequirement requirement,
            Post post)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isOwner = Guid.TryParse(userId, out var currentUserId)
                          && post.UserId == currentUserId;

            var isAdmin = context.User.IsInRole("Admin");

            if (isOwner || isAdmin)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
