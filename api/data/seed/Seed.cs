namespace fotoservice.data.seed;

public class Seed
{
    public static async Task SeedUsers(
        UserManager<AppUser> manager,
        RoleManager<AppRole> roleManager,
        IConfiguration configuration
    )
    {
        if (await manager.Users.AnyAsync())
            return;

        var adminEmail = configuration["SeedAdmin:Email"]?.Trim();
        var adminPassword = configuration["SeedAdmin:Password"];
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            throw new InvalidOperationException(
                "Set SeedAdmin:Email and SeedAdmin:Password before starting with an empty database."
            );
        }

        var roles = new List<AppRole>
        {
            new AppRole { Name = "Surgery" },
            new AppRole { Name = "Moderator" },
            new AppRole { Name = "Sponsor" },
            new AppRole { Name = "Refcard" },
            new AppRole { Name = "Admin" },
            new AppRole { Name = "Cardiologist" },
            new AppRole { Name = "Chef" }
        };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Could not create role '{role.Name}': {string.Join("; ", result.Errors.Select(error => error.Description))}"
                    );
                }
            }
        }

        var admin = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Gender = "male",
            PaidTill = new DateTime().AddYears(2250)
        };
        var adminResult = await manager.CreateAsync(admin, adminPassword);
        if (!adminResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Could not create seed admin: {string.Join("; ", adminResult.Errors.Select(error => error.Description))}"
            );
        }

        var roleResult = await manager.AddToRoleAsync(admin, "Admin");
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Could not assign the Admin role: {string.Join("; ", roleResult.Errors.Select(error => error.Description))}"
            );
        }
    }
}
