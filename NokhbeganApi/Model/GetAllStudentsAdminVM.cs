using System.ComponentModel.DataAnnotations;

namespace NokhbeganApi.Model
{
    public class GetAllStudentsAdminVM
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public string ReferralCode { get; set; }

        public ICollection<GetAllStudentsAdminVM> InvitedUsers { get; set; } = new List<GetAllStudentsAdminVM>();
    }
}
