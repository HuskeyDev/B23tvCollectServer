namespace B23tvCollect.Models
{
    public class LinkResponse
    {
        public class ReturnTarget
        {
            public ReturnTarget()
            {
                targets = new List<Common.TargetObject>();
            }
            public List<Common.TargetObject> targets { get; set; }
        }

    }
}
