using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NokhbeganApi.Model
{
    public  class T_StudentTerm
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid TermId { get; set; }
        [StringLength(50)]
        public string BookName { get; set; }
        public int CurrentLevel { get; set; }
        public int HistoryOfTerm { get; set; }
        public long Price { get; set; }
        public DateTime StartedAt { get; set; }  
        public DateTime EndedAt { get; set; }
        public bool IsActive { get; set; } // when the term is defined its true when its paid will be false.
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public T_CustomUser? User { get; set; }


    }
}
