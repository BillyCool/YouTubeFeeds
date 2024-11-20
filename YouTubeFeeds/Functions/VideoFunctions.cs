using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using YouTubeFeeds.Extension;
using YouTubeFeeds.Repository;

namespace YouTubeFeeds.Functions;

public class VideoFunctions(IYoutubeFeedsRepository youtubeFeedsRepository, ILogger<VideoFunctions> logger) : AbstractYouTubeFunctions(youtubeFeedsRepository, logger)
{
    [Function("video")]
    public async Task<IActionResult> Video([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "video/{videoId}")]
        HttpRequest req, string videoId)
    {
        _ = req; // CA1801: Discard unused parameter
        try
        {
            // Get the video
            Video video = await _youtubeFeedsRepository.GetVideoAsync(videoId);

            // Return the video in JSON format
            return new JsonResult(video);
        }
        catch (YoutubeExplodeException ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            return ex.ToActionResult();
        }
    }

    [Function("video-streams")]
    public async Task<IActionResult> VideoStreams([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "video/{videoId}/streams")]
        HttpRequest req, string videoId)
    {
        _ = req; // CA1801: Discard unused parameter
        try
        {
            // Get the stream manifest
            StreamManifest manifest = await _youtubeFeedsRepository.GetStreamManifestAsync(videoId);

            // Return the stream manifest in JSON format
            return new JsonResult(manifest);
        }
        catch (YoutubeExplodeException ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            return ex.ToActionResult();
        }
    }

    [Function("video-closed-captions")]
    public async Task<IActionResult> VideoClosedCaptions([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "video/{videoId}/cc")]
        HttpRequest req, string videoId)
    {
        _ = req; // CA1801: Discard unused parameter
        try
        {
            // Get the closed captions manifest
            ClosedCaptionManifest manifest = await _youtubeFeedsRepository.GetClosedCaptionManifestAsync(videoId);

            // Return the closed captions manifest in JSON format
            return new JsonResult(manifest);
        }
        catch (YoutubeExplodeException ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            return ex.ToActionResult();
        }
    }
}
