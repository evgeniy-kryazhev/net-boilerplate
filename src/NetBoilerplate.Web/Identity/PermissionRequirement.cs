using Microsoft.AspNetCore.Authorization;

namespace NetBoilerplate.Web.Identity;

public sealed record PermissionRequirement(string PermissionName) : IAuthorizationRequirement;
