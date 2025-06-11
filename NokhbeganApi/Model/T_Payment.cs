using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NokhbeganApi.Model
{
    public class T_Payment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid PaymentId { get; set; }
        public string? Message { get; set; }

        public long? TrackId { get; set; }

        public string? Description { get; set; }

        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public long Amount { get; set; }
        public long PayWithDiscount { get; set; }
        public string? paymentStatusCode { get; set; }
        public bool? IsSuccess { get; set; }
        public string? OrderId { get; set; }

        public string UserPaidId { get; set; }

        public Guid TermId { get; set; }
    }
}
