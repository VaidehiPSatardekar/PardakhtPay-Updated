using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.TimeZone;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Enterprise.Utilities.Services.GenericManagementApi;
using Pardakht.PardakhtPay.Enterprise.Utilities.Services.TimeZone;
using Pardakht.UserManagement.Application.JwtToken;
using Pardakht.UserManagement.Application.Role;
using Pardakht.UserManagement.Application.StaffUser;
using Pardakht.UserManagement.Application.TenantServiceApiAuth;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Domain.Platform;
using Pardakht.UserManagement.Domain.Role;
using Pardakht.UserManagement.Domain.StaffUser;
using Pardakht.UserManagement.Domain.UserPlatform;
using Pardakht.UserManagement.Infrastructure.Identity;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Infrastructure.SqlRepository;
using Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories;
using Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories.Account;
using Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData;
using Pardakht.UserManagement.Shared.Models.Configuration;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Web.RestService
{
    public class Startup
    {
       
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fff+00:00";
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            ////Settings found in the appsettings.json file
            services.Configure<JwtIssuerOptions>(Configuration.GetSection(nameof(JwtIssuerOptions)));
            services.Configure<CaptchaSettings>(Configuration.GetSection(nameof(CaptchaSettings)));
            services.Configure<LockoutSettings>(Configuration.GetSection(nameof(LockoutSettings)));
            services.Configure<PasswordSettings>(Configuration.GetSection(nameof(PasswordSettings)));
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.Configure<SeedDataSettings>(Configuration.GetSection(nameof(SeedDataSettings)));
            services.Configure<CorsSettings>(Configuration.GetSection(nameof(CorsSettings)));
            services.Configure<SendGridSettings>(Configuration.GetSection(nameof(SendGridSettings)));
            services.Configure<SecurityKeys>(Configuration.GetSection(nameof(SecurityKeys)));
            services.Configure<RoleSettings>(Configuration.GetSection(nameof(RoleSettings)));
            services.Configure<TenantManagementSettings>(Configuration.GetSection(nameof(TenantManagementSettings)));
            services.Configure<EmailNotification>(Configuration.GetSection(nameof(EmailNotification)));
          
            services.AddCors();
            services.AddOptions();

            services.SystemFunctions<TimeZoneCalendarManagementSettings>(Configuration);

            AutoMapperConfig.Initialize();
            // Register the Swagger generator, defining 1 or more Swagger documents

            services.AddScoped<ITimeZoneService, TimeZoneService>();
            services.AddScoped<IUserPlatformManager, UserPlatformManager>();
            services.AddScoped<IUserPlatformRepository, UserPlatformRepository>();

            services.AddScoped<IStaffUserService, StaffUserService>();
            services.AddScoped<IStaffUserManager, StaffUserManager>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleManager, RoleManager>();
            services.AddScoped<IAuditLogManager, AuditLogManager>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IConnectionStringManager, ConnectionStringManager>();
            services.AddTransient<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ISignInManager, SignInManagerWrapper>();
            //services.AddScoped<TenantConfig>();
            services.AddScoped<IStaffUserRepository, StaffUserRepository>();
            services.AddScoped<IPlatformManager, PlatformManager>();
            services.AddScoped<IPlatformRepository, PlatformRepository>();
            services.AddScoped<UserContext>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
            services.AddScoped<TenantServiceApiAuth>();
            services.AddSingleton<TenantServiceInfo>();
            
            services.AddTransient<TenantServiceApiTokenGenerator>();

            var connectionString = "Server=" + Configuration.GetSection("ConnectionStringOperational")["Server"] +
                                   ";User=" + Configuration.GetSection("ConnectionStringOperational")["User"] +
                                   ";Password=" + Configuration.GetSection("ConnectionStringOperational")["Password"] +
                                   ";Database=" + Configuration.GetSection("ConnectionStringOperational")["Database"];
            connectionString += ";MultipleActiveResultSets=True";

            var accountConnectionString = "Server=" + Configuration.GetSection("ConnectionStringAccount")["Server"] +
                                   ";User=" + Configuration.GetSection("ConnectionStringAccount")["User"] +
                                   ";Password=" + Configuration.GetSection("ConnectionStringAccount")["Password"] +
                                   ";Database=" + Configuration.GetSection("ConnectionStringAccount")["Database"];
            accountConnectionString += ";MultipleActiveResultSets=True";


            services.AddDbContext<ParadakhtUserManagementDbContext>(options => options.UseSqlServer(connectionString));

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<AccountRepository>(options => options.UseSqlServer(accountConnectionString));

            services.AddTransient<SeedDataInvoker>();
            services.Configure<EmailManagementSettings>(Configuration.GetSection(nameof(EmailManagementSettings)));
            services.AddScoped<Pardakht.PardakhtPay.Enterprise.Utilities.Models.User.UserContext>();
            //services.AddScoped<Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant.TenantConfig>();
            
            services.AddHealthChecks();

            //services.AddScoped<Enterprise.LoggingManagement.Services.ILoggingSettings, Enterprise.LoggingManagement.Services.LoggingSettings>();
            //Enterprise.Logging.Services.LoggingService.InitialiseLogger(services, Configuration);

            AddIdentity(services);
            AddJwtAuthentication(services);

            services.Configure<SeedDataSettings>(Configuration.GetSection(nameof(SeedDataSettings)));


            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "User Management Api",
                    Version = "v1",
                    Description = "User Management Api"
                });
            });
        }

        private void AddIdentity(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var passwordSettings = sp.GetService<IOptions<PasswordSettings>>().Value;
            var lockoutSettings = sp.GetService<IOptions<LockoutSettings>>().Value;

            //services.AddDbContext<AccountRepository>(x => x.UseInMemoryDatabase("TestDb")); // will need to swap out with actual db before deployment
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = passwordSettings.RequireDigit;
                options.Password.RequiredLength = passwordSettings.RequiredLength;
                options.Password.RequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric;
                options.Password.RequireUppercase = passwordSettings.RequireUppercase;
                options.Password.RequireLowercase = passwordSettings.RequireLowercase;
                options.Password.RequiredUniqueChars = passwordSettings.RequiredUniqueChars;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutSettings.DefaultLockoutTimeSpan);
                options.Lockout.MaxFailedAccessAttempts = lockoutSettings.MaxFailedAccessAttempts;
                options.Lockout.AllowedForNewUsers = true;
                options.User.RequireUniqueEmail = false;
            })
                .AddEntityFrameworkStores<AccountRepository>()
                .AddDefaultTokenProviders();
        }

        private void AddJwtAuthentication(IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection(nameof(JwtIssuerOptions))["Key"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SeedDataInvoker seeder, ILoggerFactory loggerFactory)
        {
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API V1");

                // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(x => x
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true) // allow any origin
               .AllowCredentials()); // allow credentials

            app.UseAuthentication();
            app.UseMiddleware<UserContextResolver>();
            app.UseRewriter(new RewriteOptions().Add(RewriteRouteRule.ReWriteRequests));
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            seeder.Seed();
        }
    }
}