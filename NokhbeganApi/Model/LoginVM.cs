using System.ComponentModel.DataAnnotations;

namespace NokhbeganApi.Model
{
    public class LoginVM
    {
        [StringLength(10)]
        public string NationalId { get; set; }

        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
