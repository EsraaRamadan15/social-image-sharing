using Microsoft.AspNetCore.Authorization;

namespace Posts.Application.Authorization
{
    public sealed class PostOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
