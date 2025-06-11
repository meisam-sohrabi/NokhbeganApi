namespace NokhbeganApi.Model
{
    public class StudentInfoFiltered
    {
        public string Id { get; set; }
        public string NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string? ReferralCode { get; set; }
        public string? ImageUrl { get; set; }
    }
}
