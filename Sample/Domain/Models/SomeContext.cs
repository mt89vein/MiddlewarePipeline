namespace Domain.Models
{
    public class SomeContext
    {
        public SomeRequest Request { get; set; }

        public SomeResponse Response { get; set; }

        public SomeContext(SomeRequest someRequest)
        {
            Request = someRequest;
            Response = new SomeResponse();
        }
    }
}