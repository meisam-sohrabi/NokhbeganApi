using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NokhbeganApi.Model;

namespace NokhbeganApi.Context
{
    public class NokhbeganDbContext : IdentityDbContext<T_CustomUser>
    {
        public NokhbeganDbContext(DbContextOptions options) : base(options)
        {
           
        }


        public DbSet<T_StudentTerm> studentTerms { get; set; }
        public DbSet<T_Notification> notifications { get; set; }
        public DbSet<T_InvitationLevelDiscount> discount { get; set; }
        public DbSet<T_Payment> payments { get; set; }
        public DbSet<T_GlobalInvitationConfig> GlobalConfig { get; set; }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    //Seeding a  'admin' role to AspNetRoles table

        //    builder.Entity<IdentityRole>()
        //          .HasData(new IdentityRole
        //          {
        //              Id = "6c5e174e-9b0e-446f-86af-483k56fd7210",
        //              Name = "admin",
        //              NormalizedName = "ADMIN".ToUpper(),
        //          });

        //    //a hasher to hash the password before seeding the user to the db

        //    var hasher = new PasswordHasher<CustomUser>();

        //    //Seeding the User to AspNetUsers table

        //    builder.Entity<CustomUser>()
        //        .HasData(new CustomUser
        //        {
        //            Id = "5e443865-a24d-4543-b6c6-9443d048cdb9",
        //            UserName = "admin@example.com",
        //            NormalizedUserName = "ADMIN".ToUpper(),
        //            PasswordHash = hasher.HashPassword(null, "Admin@12345"),
        //            FirstName = "admin",
        //            LastName = "admin",
        //            PhoneNumber = "",
        //            Email = "admin@example.com",
        //            NormalizedEmail = "ADMIN@EXAMPLE.COM",

        //        });


        //    //Seeding the relation between our user and role to AspNetUserRoles table

        //    builder.Entity<IdentityUserRole<string>>()
        //        .HasData(new IdentityUserRole<string>
        //        {
        //            RoleId = "6c5e174e-9b0e-446f-86af-483k56fd7210",
        //            UserId = "5e443865-a24d-4543-b6c6-9443d048cdb9"
        //        });
        //}
    }
}
