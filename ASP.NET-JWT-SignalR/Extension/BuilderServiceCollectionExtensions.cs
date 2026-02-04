namespace LlamaEngineHost.Extension
{
    using System.Net.Http.Headers;

    using System.Text.Json.Serialization;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Net.Http.Headers;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;




    using Ardalis.GuardClauses;
    using LlamaEngineHost.Utilities;
    using LlamaEngineHost.Models;

    public static class BuilderServiceCollectionExtensions
    {

        static BuilderServiceCollectionExtensions()
        {


        }


        public static IServiceCollection AddHosting(
                    this IServiceCollection services,
                    IConfiguration configuration
                )
        {
            services
                .AddDataProtection()
                .PersistKeysToFileSystem(
                    new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys")
                );

            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddAuthorization();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            })
             .AddJsonOptions(opt =>
                 opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
                
             );
            return services;


        }

        public static IServiceCollection AddSignalR(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services
                .AddSignalR()
                .AddNewtonsoftJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                });
            return services;
        }

        public static IServiceCollection AddScope(
                    this IServiceCollection services,
                    IConfiguration configuration
                )
        {
            services.AddScoped<IManageUser, ManageUser>();
            services.AddScoped<IAuthService, AuthService>();


            return services;
        }

         public static IServiceCollection AddJWTAuth(this IServiceCollection services,
                    IConfiguration configuration, string key)
        {
            

            services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Read token from cookie
                        var token = context.Request.Cookies["LlamaEngineHostToken"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["jwtOptions:Issuer"],
                    ValidAudience = configuration["jwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key))
                };
            });

            return services;



        }



        public static IServiceCollection AddJWTAuth2(this IServiceCollection services,
                    IConfiguration configuration)
        {

            JwtOptions jwtOptions = Guard.Against.Null(
                       configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()
                   );

            services
               .AddAuthentication(o =>
               {
                   o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                   o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
               })
               .AddJwtBearer(o =>
               {
                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       //NameClaimType = "name",
                       RoleClaimType = "role",
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = false,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = jwtOptions.Issuer,
                       ValidAudience = jwtOptions.Audience,
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(jwtOptions.Key)
                       ),
                   };
                   o.Events = new JwtBearerEvents
                   {
                       OnMessageReceived = context =>
                       {
                           var accessToken = context.Request.Query["access_token"];

                           // For SignalR requests
                           var path = context.HttpContext.Request.Path;
                           if (
                               !string.IsNullOrEmpty(accessToken)
                               && (path.StartsWithSegments("/chatHub"))
                           )
                           {
                               context.Token = accessToken;
                           }

                           return Task.CompletedTask;
                       },
                       OnAuthenticationFailed = context =>
                       {
                           Console.WriteLine(
                               "Authentication failed: " + context.Exception.Message
                           );
                           return Task.CompletedTask;
                       },
                   };
               });

            return services;
        }


        
        /// <summary>
        /// For calling outsource
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClient(this IServiceCollection services,
                    IConfiguration configuration)
        {

            services.AddHttpClient(
                "LlamaEngineHost",
                httpClient =>
                {
                    httpClient.BaseAddress = new Uri(configuration["ApiUrls:Base"]);
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json")
                    );
                }
            );

            return services;

        }

        public static IServiceCollection AddSwagger(this IServiceCollection services,
                    IConfiguration configuration)
        {

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v2",
                    new OpenApiInfo
                    {
                        Title = "FLSCB API",
                        Version = "v2",
                        Description = "The FLSCB API provides methods for UI.",
                    }
                );

                //string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "bearer",
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                            },
                            new List<string>()
                        },
                    }
                );
            });


            return services;
        }

        
    
    }
}