namespace Identity.Application.Authorization
{
    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
        public const string CanDeleteAnyPost = "CanDeleteAnyPost";
        public const string CanModerate = "CanModerate";
    }
}
