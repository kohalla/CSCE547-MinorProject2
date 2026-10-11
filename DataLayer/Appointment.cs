namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Appointment
    {
        public int AppointmentID { get; set; }

        public int UserID { get; set; }

        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(150)]
        public string Location { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime StartDateTime { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime EndDateTime { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public int ReminderMinutesBefore { get; set; }

        public virtual Category Category { get; set; }

        public virtual User User { get; set; }
    }
}
