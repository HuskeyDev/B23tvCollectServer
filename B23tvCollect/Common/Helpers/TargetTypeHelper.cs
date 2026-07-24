using B23tvCollect.Common.Exceptions;

namespace B23tvCollect.Common.Helpers
{
    public static class TargetTypeHelper
    {
        //0=default
        //1=video
        //2=bangumi
        //3=live
        //4=opus
        //5=space
        //6=readlist
        public static int GetTargetType(string rawUrl)
        {
            var uri = UriHelper.GetUri(rawUrl);
            var host = uri.Host.ToLower();
            var path = uri.AbsolutePath.ToLower();

            if (host.Contains("live.bilibili.com"))
                return 3;
            if (path.StartsWith("/video/"))
                return 1;
            if (path.StartsWith("/bangumi/play/"))
                return 2;
            if (path.StartsWith("/opus/"))
                return 4;
            if (path.StartsWith("/space/"))
                return 5;
            if (path.StartsWith("/read/readlist/"))
                return 6;
            return 0;
        }
        public static bool IsTargetTypeLegal(int input)
        {
            if (input<=6 && input>=0)return true;
            return false;
        }
        public static string FormatStorageContent(int targetType, string rawUrl)
        {
            var target = rawUrl;
            var uri = UriHelper.GetUri(rawUrl);
            switch (targetType)
            {
                case 0:
                    break;
                case 1://video
                    target = uri.Segments.Last().TrimEnd('/');
                    break;
                case 2://bangumi
                    target = uri.Segments.Last().TrimEnd('/');
                    break;
                case 3://live
                    target = uri.Segments.Last().TrimEnd('/');
                    break;
                case 4://opus
                    target = uri.Segments.Last().TrimEnd('/');
                    break;
                case 5://space
                    target = uri.Segments.Last().TrimEnd('/');
                    break;
                case 6://readlist
                    target = uri.Segments.Last().TrimEnd('/');
                    break;
                default:
                    throw new BusinessException(602, "参数不合法：targetType");
            }
            return target;
        }
        public static string GetTargetUrl(int targetType,string storageContent)
        {
            var target = storageContent;
            switch (targetType)
            {
                case 0:
                    break;
                case 1://video
                    target = "https://www.bilibili.com/video/" + storageContent;
                    break;
                case 2://bangumi
                    target = "https://www.bilibili.com/bangumi/play/" + storageContent;
                    break;
                case 3://live
                    target = "https://live.bilibili.com/" + storageContent;
                    break;
                case 4://opus
                    target = "https://bilibili.com/opus/" + storageContent;
                    break;
                case 5://space
                    target = "https://space.bilibili.com/" + storageContent;
                    break;
                case 6://readlist
                    target = "https://www.bilibili.com/read/readlist/" + storageContent;
                    break;
                default:
                    throw new BusinessException(602, "参数不合法：targetType");
            }
            return target;
        }
    }
}
