using Microsoft.AspNetCore.Mvc;

namespace YouTubeFeeds.Extension;
public static class YouTubeExplodeExtensions
{
    public static IActionResult ToActionResult(this YoutubeExplodeException exception)
    {
        // Return a not found result with the exception message
        return new NotFoundObjectResult(exception.Message);
    }
}
