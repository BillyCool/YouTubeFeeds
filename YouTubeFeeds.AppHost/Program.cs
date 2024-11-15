using Microsoft.Extensions.Configuration;
using YouTubeFeeds.Common;
using YouTubeFeeds.Common.Settings;

var builder = DistributedApplication.CreateBuilder(args);

if (builder.Configuration.GetSection(nameof(CacheSettings)).GetValue<bool>(nameof(CacheSettings.UseRedisCache)))
{
    var redis = builder.AddRedis(ServiceNames.Redis).WithRedisInsight();

    builder.AddAzureFunctionsProject<Projects.YouTubeFeeds>(ServiceNames.App)
        .WithReference(redis);
}
else
{
    builder.AddAzureFunctionsProject<Projects.YouTubeFeeds>(ServiceNames.App);
}

builder.Build().Run();
