using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NokhbeganApi.Context;
using NokhbeganApi.Model;

namespace NokhbeganApi.Helper
{
    public class DataSeeder
    {
        
        private readonly UserManager<T_CustomUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DataSeeder> _logger;
        private readonly IConfiguration _configuration;

        public DataSeeder(
            UserManager<T_CustomUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DataSeeder> logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {


            // Check if the admin role exists
            if (!await _roleManager.RoleExistsAsync("admin"))
            {
                var adminRole = new IdentityRole { Name = "admin", NormalizedName = "ADMIN" };
                var roleResult = await _roleManager.CreateAsync(adminRole);

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Admin role created successfully.");
                }
                else
                {
                    _logger.LogError("Failed to create admin role: {Errors}", roleResult.Errors);
                    return;
                }
            }

            // Check if the admin user exists
            var adminNationalId = "1234567890";
            var adminUser = await _userManager.Users.FirstOrDefaultAsync(c=> c.NationalId == adminNationalId);

            if (adminUser == null)
            {
                adminUser = new T_CustomUser
                {
                    UserName = Guid.NewGuid().ToString(),
                    NationalId = adminNationalId,
                    FirstName = "Admin",
                    LastName = "Admin",
                    PhoneNumber = string.Empty  // Replace with actual number if needed
                };

                // Use a secure default password or pull from environment/config
                var password = _configuration["SeedAdminPass:Pass"];  // For production, use Environment.GetEnvironmentVariable("ADMIN_PASSWORD")

                var userResult = await _userManager.CreateAsync(adminUser, password);

                if (userResult.Succeeded)
                {
                    _logger.LogInformation("Admin user created successfully with nationalId {AdminNationalId}.", adminNationalId);
                    var addToRoleResult = await _userManager.AddToRoleAsync(adminUser, "admin");

                    if (addToRoleResult.Succeeded)
                    {
                        _logger.LogInformation("Admin user assigned to 'admin' role.");
                    }
                    else
                    {
                        _logger.LogError("Failed to assign admin role: {Errors}", addToRoleResult.Errors);
                    }
                }
                else
                {
                    _logger.LogError("Failed to create admin user: {Errors}", userResult.Errors);
                }
            }
            else
            {
                _logger.LogInformation("Admin user already exists.");
            }
        }
    }
}
