using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCMS.Data
{
    public class SecurityLevelRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SecurityLevelId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty;

        [ForeignKey(nameof(SecurityLevelId))]
        public SecurityLevel SecurityLevel { get; set; } = default!;
    }
}
