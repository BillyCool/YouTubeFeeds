using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YouTubeFeeds.Common;
using YouTubeFeeds.Common.Settings;
using YouTubeFeeds.Repository;

namespace YouTubeFeeds;

public static class ServiceExtensions
{
    public static void AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        CacheSettings cacheSettings = configuration.GetRequiredSection(nameof(CacheSettings)).Get<CacheSettings>()!;

        if (cacheSettings.UseRedisCache)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString(ServiceNames.Redis);
                options.InstanceName = "YouTubeFeeds";
            });
        }

#pragma warning disable EXTEXP0018
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(cacheSettings.Expiration),
                LocalCacheExpiration = TimeSpan.FromMinutes(cacheSettings.LocalCacheExpiration)
            };
        });
#pragma warning restore EXTEXP0018
    }

    public static void AddYouTubeFeedsServices(this IServiceCollection services)
    {
        services.AddScoped<IYoutubeFeedsRepository, YoutubeFeedsRepository>();
    }
}
