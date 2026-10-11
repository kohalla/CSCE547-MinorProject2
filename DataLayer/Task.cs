namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Task
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Task()
        {
            TaskCompletions = new HashSet<TaskCompletion>();
        }

        public int TaskID { get; set; }

        public int UserID { get; set; }

        public int CategoryID { get; set; }

        public int? GoalID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(10)]
        public string Frequency { get; set; }

        [StringLength(9)]
        public string PreferredDay { get; set; }

        public TimeSpan? PreferredTime { get; set; }

        public int DurationMinutes { get; set; }

        public byte Priority { get; set; }

        public bool IsFlexible { get; set; }

        public bool IsActive { get; set; }

        public virtual Category Category { get; set; }

        public virtual Goal Goal { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaskCompletion> TaskCompletions { get; set; }

        public virtual User User { get; set; }
    }
}
