using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Repositories;
using UserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configure PostgreSQL with Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1",
        Description = "A CRUD REST API for user management with audit logging"
    });
});

// CORS policy for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1"));
    app.UseCors("AllowAll");
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseAuditLogMiddleware();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            // Seed data if database tables are empty
            if (!context.Users.Any())
            {
                // Create default admin user
                var adminAddress = new Address
                {
                    Street = "Admin Street 1",
                    PostalCode = "1000"
                };
                context.Addresses.Add(adminAddress);
                context.SaveChanges();

                var adminUser = new User
                {
                    Username = "admin",
                    FirstName = "Admin",
                    LastName = "User",
                    AddressId = adminAddress.Id,
                    Active = true
                };
                context.Users.Add(adminUser);
                context.SaveChanges();

                // Create admin role
                var adminRole = new Role
                {
                    RoleName = "Administrator",
                    Active = true
                };
                context.Roles.Add(adminRole);
                context.SaveChanges();

                // Assign admin role to admin user
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                };
                context.UserRoles.Add(userRole);

                // Create basic access rights
                var accessRights = new[]
                {
                    new AccessRight { Description = "Users:Read" },
                    new AccessRight { Description = "Users:Write" },
                    new AccessRight { Description = "Roles:Read" },
                    new AccessRight { Description = "Roles:Write" },
                    new AccessRight { Description = "AuditLogs:Read" }
                };
                context.AccessRights.AddRange(accessRights);
                context.SaveChanges();

                // Assign all access rights to admin role
                foreach (var right in accessRights)
                {
                    context.RoleAccessRights.Add(new RoleAccessRight
                    {
                        RoleId = adminRole.Id,
                        AccessRightId = right.Id
                    });
                }

                context.SaveChanges();

                // Log initial setup
                var auditLog = new AuditLog
                {
                    TableName = "system",
                    Operation = "INITIAL_SETUP",
                    ChangedAt = DateTime.UtcNow,
                    UserId = adminUser.Id
                };
                context.AuditLogs.Add(auditLog);
                context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}

app.Run();