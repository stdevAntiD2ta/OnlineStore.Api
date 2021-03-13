using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineStore.Api.Data;
using OnlineStore.Api.Data.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OnlineStore.Api
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

            services.AddControllers();//.AddNewtonsoftJson();
            //services.AddCors();
            // services.AddDbContext<OnlineStoreContext>(options =>
            //     options.UseMySql(Configuration.GetConnectionString("DefaultConnection", new MySqlServerVersion(new Version(5, 7, 21)))));
            // services.AddDbContextPool<OnlineStoreContext>(options => options
            //     .UseMySql(Configuration.GetConnectionString("DefaultConnection"),
            //         new MySqlServerVersion(new Version(5, 7, 21)),
            //         mySqlOptions => mySqlOptions
            //             .CharSetBehavior(CharSetBehavior.NeverAppend))
            //     .EnableSensitiveDataLogging()
            //     .EnableDetailedErrors()
            //     );
            services.AddDbContext<OnlineStoreContext>(options =>
                options.UseInMemoryDatabase("OnlineStore"));

            services.AddIdentity<Usuario, IdentityRole>()
                .AddEntityFrameworkStores<OnlineStoreContext>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://localhost",
                        ValidAudience = "https://localhost",
                        IssuerSigningKey = new
                    SymmetricSecurityKey(Encoding.ASCII.GetBytes
                    ("7S79jvOkEdwoRqHx"))
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "OnlineStore API",
                    Version = "v1",
                    Description = "Example of a Online Store REST API using ASP .NET Core Web API",
                    Contact = new OpenApiContact
                    {
                        Name = "Miguel Tenorio",
                        Email = "mtpotro41@gmail.com",
                    }
                });

                // var security = new Dictionary<string, IEnumerable<string>>
                // {
                //     {"Bearer", new string[] { }},
                // };
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new SecurityRequirements());

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineStore.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class SecurityRequirements : OpenApiSecurityRequirement
    {
        Dictionary<string, IEnumerable<string>> Security = new Dictionary<string, IEnumerable<string>>
        {
            {"Bearer", new string[] { }},
        };
    }
}
