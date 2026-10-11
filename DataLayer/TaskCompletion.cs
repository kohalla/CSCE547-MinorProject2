namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TaskCompletion
    {
        [Key]
        public int CompletionID { get; set; }

        public int TaskID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CompletedAt { get; set; }

        public int? ActualMinutes { get; set; }

        [StringLength(300)]
        public string Notes { get; set; }

        public virtual Task Task { get; set; }
    }
}
