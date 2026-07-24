namespace B23tvCollect.Common.Helpers
{
    public static class UriHelper
    {
        public static bool IsValidUrl(string url)
        {
            if (!url.StartsWith("http"))
                url = "https://" + url;
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)
                && uriResult.Host.Contains('.');
        }
        public static Uri GetUri(string url)
        {
            if (!url.StartsWith("http"))
            url = "https://" + url;
            return new Uri(url);
        }
    }
}
