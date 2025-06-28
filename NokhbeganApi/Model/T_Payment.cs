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
        [StringLength(300)]
        public string? PaymentType { get; set; } // نقدی ،کارت خوان، کارت به کارت
        public DateTime? PaidAt { get; set; }
        public bool IsPaid { get; set; } = false;
        public long Amount { get; set; }
        public long AmountWithDiscount { get; set; }
        public string UserPaidId { get; set; }
        public Guid TermId { get; set; }

        //public string? Message { get; set; }
        //public long? TrackId { get; set; }
        //public string? paymentStatusCode { get; set; }
        //public bool? IsSuccess { get; set; }
        //public string? OrderId { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public string? Description { get; set; }

    }
}
