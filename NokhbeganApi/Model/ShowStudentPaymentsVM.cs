namespace NokhbeganApi.Model
{
    public class ShowStudentPaymentsVM
    {
        public string? PaymentType { get; set; }
        public DateTime? PaidAt { get; set; }
        public bool IsPaid { get; set; }
        public long Amount { get; set; }
        public long AmountWithDiscount { get; set; }

    }
}
