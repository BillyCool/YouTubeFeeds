namespace YouTubeFeeds.Common.Settings;

public class CacheSettings
{
    public CacheSettings()
    {
        UseRedisCache = false;
        Expiration = 5;
        LocalCacheExpiration = 1;
    }

    public bool UseRedisCache { get; set; }

    public int Expiration { get; set; }

    public int LocalCacheExpiration { get; set; }
}
