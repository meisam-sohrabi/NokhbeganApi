using System.ComponentModel.DataAnnotations;

namespace NokhbeganApi.Model
{
    public class UserInfoUpdateVM
    {

        [StringLength(25)]
        public string FirstName { get; set; }

        [StringLength(25)]
        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }
}
