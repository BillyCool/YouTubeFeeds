using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using YouTubeFeeds.Extension;
using YouTubeFeeds.Repository;

namespace YouTubeFeeds.Functions;

public class ChannelFunctions(IYoutubeFeedsRepository youtubeFeedsRepository, ILogger<ChannelFunctions> logger) : AbstractYouTubeFunctions(youtubeFeedsRepository, logger)
{
    [Function("channel")]
    public async Task<IActionResult> Channel([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "channel/{channelIdentifier}")]
        HttpRequest req, string channelIdentifier)
    {
        _ = req; // CA1801: Discard unused parameter
        try
        {
            // Get the channel
            Channel channel = await _youtubeFeedsRepository.GetChannelAsync(channelIdentifier);

            // Return the channel in JSON format
            return new JsonResult(channel);
        }
        catch (YoutubeExplodeException ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            return ex.ToActionResult();
        }
    }

    [Function("channel-videos")]
    public async Task<IActionResult> ChannelVideos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "channel/{channelIdentifier}/videos")]
    HttpRequest req, string channelIdentifier, int? count = default)
    {
        _ = req; // CA1801: Discard unused parameter
        try
        {
            // Get the channel's uploads
            IReadOnlyList<PlaylistVideo> videos = await _youtubeFeedsRepository.GetChannelVideosAsync(channelIdentifier, count);

            // Return the videos in JSON format
            return new JsonResult(videos);
        }
        catch (YoutubeExplodeException ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            return ex.ToActionResult();
        }
    }
}
