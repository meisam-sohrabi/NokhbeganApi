using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NokhbeganApi.Model
{
    public class RegisterVM
    {

        [StringLength(10)]

        public string NationalId { get; set; }

        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [StringLength(8)]
        [DefaultValue(null)]
        public string? ReferralCode { get; set; }

        public IFormFile? ImageUrl { get; set; }

    }
}
