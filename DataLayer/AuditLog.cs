namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AuditLog")]
    public partial class AuditLog
    {
        [Key]
        public long AuditID { get; set; }

        public int? UserID { get; set; }

        [Required]
        [StringLength(20)]
        public string ActionType { get; set; }

        [StringLength(50)]
        public string TableAffected { get; set; }

        public int? RecordID { get; set; }

        [StringLength(500)]
        public string Details { get; set; }

        [StringLength(45)]
        public string IPAddress { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime LoggedAt { get; set; }

        public virtual User User { get; set; }
    }
}
