namespace B23tvCollect.Models
{
    public class LinkRequest
    {
        public class NewB23tv
        {
            public string b23tvCode { get; set; }
            public string target { get; set; }
        }

        public class GetTarget
        {
             public string b23tvCode { get; set; }
        }
    }
}
