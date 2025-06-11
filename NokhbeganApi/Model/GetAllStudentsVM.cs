namespace NokhbeganApi.Model
{
    public class GetAllStudentsVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<GetAllStudentsVM> InvitedUsers { get; set; } = new List<GetAllStudentsVM>();
    }
}
