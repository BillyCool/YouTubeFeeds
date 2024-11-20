using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using YouTubeFeeds;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureFunctionsWebApplication();

builder.Services.AddCaching(builder.Configuration);
builder.Services.AddYouTubeFeedsServices();

builder.Build().Run();
