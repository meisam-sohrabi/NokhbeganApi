using System.ComponentModel.DataAnnotations;

namespace NokhbeganApi.Model
{
    public class CheckNationalIdVM
    {
        [StringLength(10)]
        public string NationalId { get; set; }
    }
}
