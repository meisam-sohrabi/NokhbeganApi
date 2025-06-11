using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NokhbeganApi.Model
{
    public class T_InvitationLevelDiscount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID_LevelDiscount {  get; set; }
        public int Level {  get; set; }
        public double DiscountPercent { get; set; }

    }
}
