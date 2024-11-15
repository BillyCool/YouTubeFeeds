using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.ClosedCaptions;
using YoutubeExplode.Videos.Streams;

namespace YouTubeFeeds;

public class Functions(HybridCache cache, ILogger<Functions> logger)
{
    private readonly YoutubeClient _youtubeClient = new();

    private readonly HybridCache _cache = cache;

    private readonly ILogger<Functions> _logger = logger;

    [Function("channel")]
    public async Task<IActionResult> Channel([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "channel/{channelIdentifier}")]
        HttpRequest req, string channelIdentifier)
    {
        // Get the channel
        Channel? channel = await GetYouTubeChannelAsync(channelIdentifier);

        // Return the channel in JSON format
        return channel is not null ? new JsonResult(channel) : new NotFoundResult();
    }

    [Function("playlist")]
    public async Task<IActionResult> Playlist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "playlist/{playlistId}")]
        HttpRequest req, string playlistId)
    {
        try
        {
            // Get the playlist
            Playlist playlist = await _youtubeClient.Playlists.GetAsync(playlistId);

            // Return the playlist in JSON format
            return new JsonResult(playlist);
        }
        catch
        {
            _logger.LogError("Playlist not found: {PlaylistId}", playlistId);
            return new NotFoundResult();
        }
    }

    [Function("video")]
    public async Task<IActionResult> Video([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "video/{videoId}")]
        HttpRequest req, string videoId)
    {
        try
        {
            // Get the video
            Video video = await _youtubeClient.Videos.GetAsync(videoId);

            // Return the video in JSON format
            return new JsonResult(video);
        }
        catch
        {
            _logger.LogError("Video not found: {VideoId}", videoId);
            return new NotFoundResult();
        }
    }

    [Function("playlist-videos")]
    public async Task<IActionResult> PlaylistVideos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "playlist/{playlistId}/videos")] HttpRequest req, string playlistId)
    {
        try
        {
            // Get the playlist's videos
            IReadOnlyList<PlaylistVideo> videos = await _youtubeClient.Playlists.GetVideosAsync(playlistId);

            // Return the videos in JSON format
            return new JsonResult(videos);
        }
        catch
        {
            _logger.LogError("Playlist not found: {PlaylistId}", playlistId);
            return new NotFoundResult();
        }
    }

    [Function("channel-videos")]
    public async Task<IActionResult> ChannelVideos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "channel/{channelIdentifier}/videos")] HttpRequest req, string channelIdentifier)
    {
        try
        {
            // Get the channel ID
            string channelId = channelIdentifier.StartsWith(Constants.YouTubeChannelIdPrefix)
                ? channelIdentifier
                : (await _youtubeClient.Channels.GetByUserAsync(channelIdentifier)).Id;

            // Get the channel's uploads
            IReadOnlyList<PlaylistVideo> videos = await _youtubeClient.Channels.GetUploadsAsync(channelId).CollectAsync(10);

            // Return the videos in JSON format
            return new JsonResult(videos);
        }
        catch
        {
            _logger.LogError("Channel not found: {ChannelIdentifier}", channelIdentifier);
            return new NotFoundResult();
        }
    }

    [Function("video-streams")]
    public async Task<IActionResult> VideoStreams([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "video/{videoId}/streams")]
        HttpRequest req, string videoId)
    {
        try
        {
            // Get the stream manifest
            StreamManifest manifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoId);

            // Return the stream manifest in JSON format
            return new JsonResult(manifest);
        }
        catch
        {
            _logger.LogError("Video not found: {VideoId}", videoId);
            return new NotFoundResult();
        }
    }

    [Function("video-closed-captions")]
    public async Task<IActionResult> VideoClosedCaptions([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "video/{videoId}/cc")]
        HttpRequest req, string videoId)
    {
        try
        {
            // Get the closed captions manifest
            ClosedCaptionManifest manifest = await _youtubeClient.Videos.ClosedCaptions.GetManifestAsync(videoId);

            // Return the closed captions manifest in JSON format
            return new JsonResult(manifest);
        }
        catch
        {
            _logger.LogError("Video not found: {VideoId}", videoId);
            return new NotFoundResult();
        }
    }

    private async Task<Channel?> GetYouTubeChannelAsync(string channelIdentifier) =>
        await _cache.GetOrCreateAsync($"channel-{channelIdentifier}", async cancellationToken =>
            {
                if (channelIdentifier.StartsWith(Constants.YouTubeChannelIdPrefix))
                {
                    try
                    {
                        // Channel ID
                        return await _youtubeClient.Channels.GetAsync(channelIdentifier, cancellationToken);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        // User handle
                        return await _youtubeClient.Channels.GetByHandleAsync(channelIdentifier, cancellationToken);
                    }
                    catch { }

                    try
                    {
                        // User ID
                        return await _youtubeClient.Channels.GetByUserAsync(channelIdentifier, cancellationToken);
                    }
                    catch { }

                    try
                    {
                        // User Slug
                        return await _youtubeClient.Channels.GetBySlugAsync(channelIdentifier, cancellationToken);
                    }
                    catch { }
                }

                _logger.LogError("Channel not found: {ChannelIdentifier}", channelIdentifier);
                return null;
            }
        );
}
