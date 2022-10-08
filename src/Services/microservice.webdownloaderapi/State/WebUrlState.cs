namespace microservice.weburlapi.State
{
    public class WebUrlState
    {
        public DateTime CreatedOn { get; set; }

        public Guid CorrelationId { get; set; }

        public string Url { get; set; }
    }
}
