using JwtAuthenticationCoreApi.Persistence;
using JwtAuthenticationCoreApi.Core.Models.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JwtAuthenticationCoreApi.Core.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using JwtAuthenticationCoreApi.Helpers;
using JwtAuthenticationCoreApi.Auth;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using JwtAuthenticationCoreApi.Extensions;
using LinkedInClient.Models.OAuth;
using LinkedInClient.Services.OAuth;
using LinkedInClient.Services.SignIn;
using JwtAuthenticationCoreApi.Persistence.Repositories;
using JwtAuthenticationCoreApi.Core.Repositories;
using JwtAuthenticationCoreApi.Core;

namespace VirtualRecruitmentApi
{
    public class Startup
    {
        // TODO - You should get this from somewhere secure
        private const string SecretKey = "<your-super-secret-key>";

        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the DI container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("JwtAuthenticationCoreApi"))
            );

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            // linkedin oauth wire up
            // Get clientId / clientSecret from app settings
            var linkedInOAuthSettings = Configuration.GetSection(nameof(LinkedInOAuthSettings));

            services.Configure<LinkedInOAuthSettings>(creds =>
            {
                creds.ClientId = linkedInOAuthSettings[nameof(LinkedInOAuthSettings.ClientId)];
                creds.ClientSecret = linkedInOAuthSettings[nameof(LinkedInOAuthSettings.ClientSecret)];
            });
            
            // Some more middleware code which introduces JWT authentication to the request pipeline, 
            // specified the validation parameters to dictate how we want received tokens validated 
            // and finally, created an authorization policy to guard our API controllers and actions
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
            )
            .AddJwtBearer(
                configureOptions =>
                {
                    configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                    configureOptions.TokenValidationParameters = tokenValidationParameters;
                    configureOptions.SaveToken = true;
                }
            );

            // api user claim policy
            // With this role stashed in our token, 
            // we can use a claims-based authorization check to give the role access to certain controllers 
            // and actions so that only users possessing the role claim may access those resources.
            // We already enabled claims based authorization as part of the JWT setup we did earlier.
            // The specific code to do that was this bit in ConfigureServices() in Startup.cs where we build 
            // and register a policy called ApiUser which checks for the presence of the Rol 
            // claim with a value of ApiAccess.
            // Usage in controller / actions: [Authorize(Policy = "ApiUser")]
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });

            // add identity
            var builder = services.AddIdentityCore<ApplicationUser>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddCors();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add application services Section
            
            // Extension Method: Like doing services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // For accessing HttpContext as a dependency inside controllers / services / anywhere that needs to be used in common
            services.AddHttpContextAccessor();
            services.AddSingleton<IJwtFactory, JwtFactory>();

            services.AddScoped<IOAuthService, OAuthService>();
            services.AddScoped<IUserSignInService, UserSignInService>();

            // Services that use the database context should use the scoped lifetime.
            services.AddScoped<IGenericRepository<JwtAuthUser>, GenericRepository<JwtAuthUser>>();
            services.AddScoped<IJwtAuthUserRepository, JwtAuthUserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // When the app runs in the Development environment:
                //   - Use the Developer Exception Page to report app runtime errors.
                //   - Use the Database Error Page to report database runtime errors.
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Use the HTTP Strict Transport Security Protocol (HSTS) Middleware.
                app.UseHsts();
            }

            // Enable the Exception Handler Middleware to catch exceptions
            app.UseExceptionHandler(
                builder =>
                {
                    builder.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                            var error = context.Features.Get<IExceptionHandlerFeature>();

                            if (error != null)
                            {
                                context.Response.AddApplicationError(error.Error.Message);

                                await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                            }
                        }
                    );
                }
            );

            // Use HTTPS Redirection Middleware to redirect HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // Authenticate before the user accesses secure resources.
            app.UseAuthentication();

            app.UseDefaultFiles();

            // Return static files and end the pipeline.
            app.UseStaticFiles();

            app.UseCors(
                builder =>
                    builder
                        .WithOrigins("http://localhost:4200") // where your angular6 client web app runs
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
            );

            // Add MVC to the request pipeline.
            app.UseMvc();
        }
    }
}