using System.Text;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
      // ??? CHECK: MigrationAssembly(typeof(AppDbContext).Assembly.FullName)
      services.AddDbContext<AppDbContext>(options =>
      {
        options.UseLazyLoadingProxies();
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
      });

      services.AddDefaultIdentity<AppUser>(options =>
      {
        options.Password.RequireDigit = false;
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
      })
          .AddRoles<IdentityRole>()
          .AddEntityFrameworkStores<AppDbContext>();

      services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());
      services.AddScoped<IJwtGenerator, JwtGenerator>();

      services.AddTransient<IDateTime, DateTimeService>();
      services.AddTransient<IIdentityService, IdentityService>();

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
            options.TokenValidationParameters = new TokenValidationParameters
            {
              ValidateIssuerSigningKey = true,
              IssuerSigningKey = key,
              ValidateAudience = false,
              ValidateIssuer = false,
              // ValidateLifetime = true,
              // ClockSkew = TimeSpan.Zero
            };
          });

      return services;
    }
  }
}