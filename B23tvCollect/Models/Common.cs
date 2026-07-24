using B23tvCollect.Common.Helpers;

namespace B23tvCollect.Models
{
    public class Common
    {
        public class TargetObject
        {
            public int targetId { get; set; }
            public string target { get; set; }
            public int targetType { get; set; }
            public long submitTime { get; set; }
        }
        public class InnerTargetObject : TargetObject
        {
        
        }
    }
}
