namespace NetBoilerplate.Domain.Identity;

public static class ApplicationPermissions
{
    public static class Identity
    {
        public const string Roles = "NetBoilerplate.Identity.Roles";
        public const string CreateRoles = "NetBoilerplate.Identity.Roles.Create";
        public const string ManageRolePermissions = "NetBoilerplate.Identity.Roles.ManagePermissions";
        public const string Users = "NetBoilerplate.Identity.Users";
        public const string CreateUsers = "NetBoilerplate.Identity.Users.Create";
        public const string UpdateUsers = "NetBoilerplate.Identity.Users.Update";
        public const string DeleteUsers = "NetBoilerplate.Identity.Users.Delete";
        public const string ManageUserRoles = "NetBoilerplate.Identity.Users.ManageRoles";
        public const string ManageUserPassword = "NetBoilerplate.Identity.Users.ManagePassword";
    }

    public static class Smtp
    {
        public const string Test = "NetBoilerplate.Smtp.Test";
    }

    public static readonly IReadOnlyList<PermissionDefinition> All =
    [
        new(Identity.Roles, "Roles", "Identity"),
        new(Identity.CreateRoles, "Create roles", "Identity", Identity.Roles),
        new(Identity.ManageRolePermissions, "Manage role permissions", "Identity", Identity.Roles),
        new(Identity.Users, "View users", "Identity"),
        new(Identity.CreateUsers, "Create users", "Identity", Identity.Users),
        new(Identity.UpdateUsers, "Update users", "Identity", Identity.Users),
        new(Identity.DeleteUsers, "Delete users", "Identity", Identity.Users),
        new(Identity.ManageUserRoles, "Manage user roles", "Identity", Identity.Users),
        new(Identity.ManageUserPassword, "Manage user passwords", "Identity", Identity.Users),
        new(Smtp.Test, "Send SMTP test emails", "SMTP")
    ];

    public static readonly IReadOnlyList<string> DefaultUser =
    [];

    public static bool IsDefined(string permissionName)
    {
        return All.Any(permission => permission.Name == permissionName);
    }
}

public sealed record PermissionDefinition(
    string Name,
    string DisplayName,
    string GroupName,
    string? ParentName = null);
