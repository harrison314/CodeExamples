using Microsoft.EntityFrameworkCore;
using TheStoryVault.Infrastructure.Interceptors;

namespace TheStoryVault;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddRazorComponents();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddCaseR(options =>
        {
#if DEBUG
            options.AddGenericInterceptor(typeof(LogTimeInterceptor<,>));
#endif
            options.AddGenericInterceptor(typeof(LogInterceptor<,>));
        });

        builder.Services.AddKeyedCaseR(CaseR.UseCaseRelation.Include, options =>
        {
            options.AddGenericInterceptor(typeof(LogIncludedInterceptor<,>));
        });

        builder.Services.AddCaseRInteractors();

        string? type = builder.Configuration.GetConnectionString("Type");

        if (type == "MsSql")
        {
            builder.Services.AddDbContext<TheStoryVault.Services.Data.StoryVaultContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        }
        else if (type == "Sqlite")
        {
            builder.Services.AddDbContext<TheStoryVault.Services.Data.StoryVaultContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection")));
        }
        else if (type == "MariaDb")
        {
            builder.Services.AddDbContext<TheStoryVault.Services.Data.StoryVaultContext>(options =>
               options.UseMySql(builder.Configuration.GetConnectionString("MariaDbConnection"),
               ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDbConnection"))));
        }
        else
        {
            throw new Exception("DB type is invalid");
        }

        builder.Services.AddScoped<Services.Contracts.ICookieApi, Services.Implementation.CookieApi>();
        builder.Services.AddScoped<Services.Contracts.IBasketApi, Services.Implementation.DbBasketApi>();
        builder.Services.AddScoped<Services.Contracts.IBookService, Services.Implementation.BookService>();
        builder.Services.AddScoped<Services.Contracts.IAuthorService, Services.Implementation.AuthorService>();
        builder.Services.AddScoped<Services.Contracts.ICategoryService, Services.Implementation.CategoryService>();
        builder.Services.AddScoped<Services.Contracts.ITrackingService, Services.Implementation.TrackingService>();

        builder.Services.AddOutputCache(options =>
        {
            options.AddPolicy(PolicyNames.Cache.Default, policy =>
            {
                policy.Expire(TimeSpan.FromMinutes(5));
            });
        });

        builder.Services.AddMemoryCache();

        builder.Services.AddHostedService<Infrastructure.Workers.InitializationWorker>();

        WebApplication app = builder.Build();

        // Simulate some processing delay for HTMX requests
        if (args.Contains("--slow"))
        {
            app.Use(async (ctx, next) =>
            {
                if (ctx.Request.Headers.ContainsKey("HX-Request"))
                {
                    await Task.Delay(500);
                }

                await next();
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseOutputCache();

        TheStoryVault.Pages.ErrorPage.AppErrorExtensions.UseExceptionCustomPage(app);
        // Or use local filter
        //RouteGroupBuilder global = app
        // .MapGroup(string.Empty)
        // .AddEndpointFilter<TheStoryVault.Pages.ErrorPage.EndpointFilter>();

        TheStoryVault.Pages.CommonEndpoints.MapEndpoints(app);

        TheStoryVault.Pages.Index.Endpoint.MapEndpoints(app);
        TheStoryVault.Pages.Book.Endpoint.MapEndpoints(app);
        TheStoryVault.Pages.Cart.Endpoint.MapEndpoints(app);
        TheStoryVault.Pages.Order.Endpoint.MapEndpoints(app);
        TheStoryVault.Pages.Author.Endpoint.MapEndpoints(app);
        TheStoryVault.Pages.Category.Endpoint.MapEndpoints(app);
        TheStoryVault.Pages.Search.Endpoint.MapEndpoints(app);

        TheStoryVault.Pages.EndOfDemo.Endpoint.MapEndpoints(app);

        app.Run();
    }
}
