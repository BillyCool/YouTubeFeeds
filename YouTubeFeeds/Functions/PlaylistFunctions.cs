using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using YouTubeFeeds.Extension;
using YouTubeFeeds.Repository;

namespace YouTubeFeeds.Functions;

public class PlaylistFunctions(IYoutubeFeedsRepository youtubeFeedsRepository, ILogger<PlaylistFunctions> logger) : AbstractYouTubeFunctions(youtubeFeedsRepository, logger)
{
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
}
