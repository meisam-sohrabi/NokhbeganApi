using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NokhbeganApi.Model
{
    public class T_Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ID_Notification { get; set; }
        [StringLength(100)]
        public string Title { get; set; }
        [StringLength(500)]
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string T_User_ID { get; set; }
        [ForeignKey("T_User_ID")]
        public T_CustomUser? User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
