using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace GiveandTake_API.Extensions
{
    public static class DependencyExtension
    {
        // AddServices method to register services
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
        {
            return services;
        }

        // AddJwtValidation method to configure JWT authentication
        public static IServiceCollection AddJwtValidation(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                };
            });
            return services;
        }


        // AddConfigSwagger method to configure Swagger
        public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Configure Swagger documentation
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "GiveAndTake", Version = "v1" });

                // Configure JWT authentication in Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                // Add JWT authentication requirement in Swagger
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { }
                        }
                });

                // Map TimeOnly type to string in Swagger
                options.MapType<TimeOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "time",
                    Example = OpenApiAnyFactory.CreateFromJson("\"13:45:42.0000000\"")
                });

                // Enable annotations in Swagger
                options.EnableAnnotations();

                // Resolve conflicting actions in Swagger
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            return services;
        }
    }
}
