var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureFunctionsProject<Projects.YouTubeFeeds>("youtubefeeds");

builder.Build().Run();
