namespace WebApplication1.Extensions;

public static class RoleNames
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);

    public static readonly IReadOnlyCollection<string> All = [Admin, User];
}
