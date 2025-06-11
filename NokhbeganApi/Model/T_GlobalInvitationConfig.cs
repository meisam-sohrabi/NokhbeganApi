using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NokhbeganApi.Model
{
    public class T_GlobalInvitationConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID_Global {  get; set; }
        public double MaxGlobalDiscountPercent { get; set; }

    }
}
