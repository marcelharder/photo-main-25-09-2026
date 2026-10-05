namespace fotoservice.data;

public class ApplicationDbContext : IdentityDbContext<
    AppUser,
    AppRole,
    int,
    IdentityUserClaim<int>,
    AppUserRole,
    IdentityUserLogin<int>,
    IdentityRoleClaim<int>,
    IdentityUserToken<int>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(b =>
        {
            b.HasMany(e => e.UserRoles)
             .WithOne(e => e.User)
             .HasForeignKey(ur => ur.UserId)
             .IsRequired();
        });

        builder.Entity<AppRole>(b =>
        {
            b.HasMany(e => e.UserRoles)
             .WithOne(e => e.Role)
             .HasForeignKey(ur => ur.RoleId)
             .IsRequired();
        });
    }
}

