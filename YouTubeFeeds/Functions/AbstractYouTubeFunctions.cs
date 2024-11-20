using Microsoft.Extensions.Logging;
using YouTubeFeeds.Repository;

namespace YouTubeFeeds.Functions;
public abstract class AbstractYouTubeFunctions(IYoutubeFeedsRepository youtubeFeedsRepository, ILogger<AbstractYouTubeFunctions> logger)
{
    protected readonly IYoutubeFeedsRepository _youtubeFeedsRepository = youtubeFeedsRepository;

    protected readonly ILogger<AbstractYouTubeFunctions> _logger = logger;
}
