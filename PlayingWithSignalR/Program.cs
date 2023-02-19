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

            services.AddAuthorization(options =>
            {
                // https://docs.microsoft.com/en-ie/aspnet/core/migration/22-to-30?view=aspnetcore-3.0&tabs=visual-studio#authorization
                // FallbackPolicy is initially configured to allow requests without authorization.
                // Override it to always require authentication on all endpoints except when [AllowAnonymous].
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
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
