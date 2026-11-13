using Microsoft.AspNetCore.Identity;

namespace QueuEase.Data.Seeding;

public static class RoleSeed
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Employee = "Employee";

    // Ids and stamps are fixed: if they were new Guids on every build,
    // each new migration would delete and re-insert the roles.
    public static IdentityRole[] Roles =>
    [
        Role("8d7c1f3e-2a4b-4c6d-9e01-5f2a3b4c5d61", SuperAdmin, "1a2b3c4d-0001-4000-8000-000000000001"),
        Role("3b5e7a9c-1d2f-4a6b-8c0d-2e4f6a8b0c12", Admin, "1a2b3c4d-0002-4000-8000-000000000002"),
        Role("6f8a0b2c-4d6e-4f1a-9b3c-5d7e9f1a3b23", Manager, "1a2b3c4d-0003-4000-8000-000000000003"),
        Role("9c1d3e5f-7a9b-4c2d-8e4f-6a8b0c2d4e34", Employee, "1a2b3c4d-0004-4000-8000-000000000004"),
    ];

    private static IdentityRole Role(string id, string name, string stamp) => new()
    {
        Id = id,
        Name = name,
        NormalizedName = name.ToUpperInvariant(),
        ConcurrencyStamp = stamp,
    };
}
