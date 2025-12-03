namespace Bookify.Web.Core.Models
{
    public class Subscription
    {
        public int Id { get; set; }

        // Nav Property to Subscriber one-to-many (Many Subscriptions for one Subscriber)
        public int SubscriberId { get; set; }
        public Subscriber? Subscriber { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? CreatedById { get; set; }
        // Nav Property to ApplicationUser To Know Who Created This Subscription
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
