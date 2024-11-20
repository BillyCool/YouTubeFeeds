using Microsoft.Extensions.Caching.Hybrid;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace YouTubeFeeds.Repository;

internal class YoutubeFeedsRepository(HybridCache cache) : IYoutubeFeedsRepository
{
    private readonly YoutubeClient _youtubeClient = new();

    private readonly HybridCache _cache = cache;

    private const int DefaultMaxCount = 10;

    public Task<Channel> GetChannelAsync(string channelIdentifier)
    {
        return GetYouTubeChannelAsync(channelIdentifier);
    }

    public async Task<IReadOnlyList<PlaylistVideo>> GetChannelVideosAsync(string channelIdentifier, int? maxCount = default)
    {
        return await _cache.GetOrCreateAsync($"channel-videos-{channelIdentifier}", async cancellationToken =>
        {
            // Get the channel ID
            string channelId = channelIdentifier.StartsWith(Constants.YouTubeChannelIdPrefix)
                ? channelIdentifier
                : (await GetYouTubeChannelAsync(channelIdentifier, cancellationToken)).Id;

            // Get the channel's uploads
            return await _youtubeClient.Channels.GetUploadsAsync(channelId, cancellationToken).CollectAsync(maxCount ?? DefaultMaxCount);
        });
    }

    public async Task<ClosedCaptionManifest> GetClosedCaptionManifestAsync(string videoId)
    {
        return await _cache.GetOrCreateAsync($"video-cc-{videoId}", async cancellationToken =>
            await _youtubeClient.Videos.ClosedCaptions.GetManifestAsync(videoId, cancellationToken));
    }

    public async Task<Playlist> GetPlaylistAsync(string playlistId)
    {
        return await _cache.GetOrCreateAsync($"playlist-{playlistId}", async cancellationToken =>
            await _youtubeClient.Playlists.GetAsync(playlistId, cancellationToken));
    }

    public async Task<IReadOnlyList<PlaylistVideo>> GetPlaylistVideosAsync(string playlistId, int? maxCount = default)
    {
        return await _cache.GetOrCreateAsync($"playlist-videos-{playlistId}", async cancellationToken =>
            await _youtubeClient.Playlists.GetVideosAsync(playlistId, cancellationToken).CollectAsync(maxCount ?? DefaultMaxCount));
    }

    public async Task<StreamManifest> GetStreamManifestAsync(string videoId)
    {
        return await _cache.GetOrCreateAsync($"video-streams-{videoId}", async cancellationToken =>
            await _youtubeClient.Videos.Streams.GetManifestAsync(videoId, cancellationToken));
    }

    public async Task<Video> GetVideoAsync(string videoId)
    {
        return await _cache.GetOrCreateAsync($"video-streams-{videoId}", async cancellationToken =>
            await _youtubeClient.Videos.GetAsync(videoId, cancellationToken));
    }

    private async Task<Channel> GetYouTubeChannelAsync(string channelIdentifier, CancellationToken cancellationToken = default) =>
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

            throw new YoutubeExplodeException($"Channel not found: {channelIdentifier}");
        }, cancellationToken: cancellationToken);
}
