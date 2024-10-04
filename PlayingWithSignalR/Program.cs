using Microsoft.AspNetCore.Authorization;
using PlayingWithSignalR.Hubs;

namespace PlayingWithSignalR;

public sealed class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container
        {
            services.AddControllers();

            services.AddJwtAuthentication();

            services.AddSignalR(options => options.EnableDetailedErrors = true);

            // --> Configure: Authorization
            // FallbackPolicy, by default, is configured to allow requests without authorization.
            // With the following configuration, requires authentication on all endpoints except when [AllowAnonymous].
            services.AddAuthorizationBuilder() // .AddAuthorization() is applied with this line
                .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build());

            // OpenTelemetry: AddAspNetCoreInstrumentation and tracing.AddSource("Microsoft.AspNetCore.SignalR.Server")
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            //app.UseSignalRClientMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<MessageHub>(MessageHub.Path);
        }

        app.Run();
    }
}
