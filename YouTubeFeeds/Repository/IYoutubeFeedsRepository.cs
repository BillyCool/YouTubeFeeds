namespace YouTubeFeeds.Repository;

public interface IYoutubeFeedsRepository
{
    Task<Channel> GetChannelAsync(string channelIdentifier);

    Task<IReadOnlyList<PlaylistVideo>> GetChannelVideosAsync(string channelIdentifier, int? maxCount = default);

    Task<ClosedCaptionManifest> GetClosedCaptionManifestAsync(string videoId);

    Task<Playlist> GetPlaylistAsync(string playlistId);

    Task<IReadOnlyList<PlaylistVideo>> GetPlaylistVideosAsync(string playlistId, int? maxCount = default);

    Task<StreamManifest> GetStreamManifestAsync(string videoId);

    Task<Video> GetVideoAsync(string videoId);
}
