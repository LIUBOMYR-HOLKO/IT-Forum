using System;
using System.IO;
using System.Reflection;
using System.Text;
using IT_Forum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IT_Forum
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Configures Entity Framework DbContext for the project.
        /// </summary>
        public static void ConfigureEntityFramework(this IServiceCollection services, 
            IConfiguration configuration)
        {   
            services.AddDbContext<Context>(opt =>
                opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }
        
        /// <summary>
        /// Configures JSON Web Token authentication scheme.
        /// </summary>
        
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options => {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 1;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                }).AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();
        }
    }
}