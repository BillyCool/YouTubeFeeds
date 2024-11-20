using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using YouTubeFeeds.Extension;
using YouTubeFeeds.Repository;

namespace YouTubeFeeds;

public class Functions(IYoutubeFeedsRepository youtubeFeedsRepository, ILogger<Functions> logger)
{
    private readonly IYoutubeFeedsRepository _youtubeFeedsRepository = youtubeFeedsRepository;

    private readonly ILogger<Functions> _logger = logger;

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

    [Function("playlist")]
    public async Task<IActionResult> Playlist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "playlist/{playlistId}")]
        HttpRequest req, string playlistId)
    {
        _ = req; // CA1801: Discard unused parameter
        try
        {
            // Get the playlist
            Playlist playlist = await _youtubeFeedsRepository.GetPlaylistAsync(playlistId);

            // Return the playlist in JSON format
            return new JsonResult(playlist);
        }
        catch (YoutubeExplodeException ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            return ex.ToActionResult();
        }
    }

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

    [Function("playlist-videos")]
    public async Task<IActionResult> PlaylistVideos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "playlist/{playlistId}/videos")]
    HttpRequest req, string playlistId)
    {
        _ = req; // CA1801: Discard unused parameter
        try
        {
            // Get the playlist's videos
            IReadOnlyList<PlaylistVideo> videos = await _youtubeFeedsRepository.GetPlaylistVideosAsync(playlistId);

            // Return the videos in JSON format
            return new JsonResult(videos);
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
