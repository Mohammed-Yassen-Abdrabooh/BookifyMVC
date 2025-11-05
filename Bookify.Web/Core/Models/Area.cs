namespace Bookify.Web.Core.Models
{
    [Index(nameof(Name),nameof(GovernorateId), IsUnique = true)]
    public class Area : BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        // Foreign Key to Governorate
        public int GovernorateId { get; set; }
        public Governorate? Governorate { get; set; }

    }
}
