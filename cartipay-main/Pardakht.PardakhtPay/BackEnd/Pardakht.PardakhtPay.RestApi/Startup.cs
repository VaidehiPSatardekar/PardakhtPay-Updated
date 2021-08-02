using BotDetect.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Text;
using Pardakht.PardakhtPay.Enterprise.Utilities.Services.User;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.SqlRepository;
using Microsoft.EntityFrameworkCore;

namespace Pardakht.PardakhtPay.RestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public AppSettings AppSettings { get; set; }
        public object Enterprise { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            AutoMapperConfig.Initialize();
            var connectionString = "Server=" + Configuration.GetSection("ConnectionString")["Server"].ToString() +
                                   ";User=" + Configuration.GetSection("ConnectionString")["User"].ToString() +
                                   ";Password=" + Configuration.GetSection("ConnectionString")["Password"].ToString() +
                                   ";Database=" + Configuration.GetSection("ConnectionString")["Database"].ToString();

            connectionString += ";Max Pool Size=1000";

            PardakhtPayAuthentication.CurrentConnectionString = connectionString;

            services.AddDbContext<PardakhtPayDbContext>(optionActions =>
            {
                optionActions.UseSqlServer(connectionString);
            });


            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            services.SetupConfigurations(Configuration);

            services.AddDependencyInjections(Configuration);

            services.AddCacheService();

            services.AddCors(options => options.AddPolicy("AllowAnyOrigins",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .WithExposedHeaders("Content-Type", "File-Name")
                ));

            AddJwtAuthentication(services);

            services.AddMvc(options =>
            {
                options.Filters.Add<PardakhtPayExceptionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllersWithViews().AddNewtonsoftJson(); 
            
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //services.AddScoped<ILoggingSettings, LoggingSettings>();
            //LoggingService.InitialiseLogger(services, Configuration);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });

            AppSettings = Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();

            if (AppSettings.AllowSwagger)
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1",
                        new OpenApiInfo
                        {
                            Title = "PardakhtPay API",
                            Version = "v1",
                            Description = "PardakhtPay Payments"
                        });
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            if (AppSettings.AllowSwagger)
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PardakhtPay API V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            // configures Session middleware 
            app.UseSession();

            //// configure your application pipeline to use Captcha middleware 
            app.UseCaptcha(Configuration);

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseCors("AllowAnyOrigins");

            app.UseRouting();
            app.UseAuthorization();

            app.UseMiddleware<PardakhtPayAuthentication>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void AddJwtAuthentication(IServiceCollection services)
        {
            var jwtAppSettingOptions = services.BuildServiceProvider().GetRequiredService<IOptions<JwtIssuerOptions>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtAppSettingOptions.Value.Issuer,
                        ValidAudience = jwtAppSettingOptions.Value.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtAppSettingOptions.Value.Key))
                    };
                    options.EventsType = typeof(UserValidation);
                });

        }
    }
}
