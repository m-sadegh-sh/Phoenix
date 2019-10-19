namespace Phoenix.Domain.LabProps {
    using System;
    using System.Data.Linq.Mapping;

    using Phoenix.Infrastructure.Extensions;

    [Table(Name = "Phoenix.LabProps")]
    public sealed class LabProp {
        public LabProp() {
            AssignedOn = DateTime.Now;
        }

        [Column(IsPrimaryKey = true)]
        public Guid PropID { get; set; }

        [Column(IsPrimaryKey = true)]
        public Guid LabID { get; set; }

        [Column]
        public DateTime AssignedOn { get; set; }

        public string StringAssignedOn {
            get { return AssignedOn.ToLocalized(); }
        }

        public string StringLabID { get; set; }

        public string StringPropID { get; set; }

        public string StringStatus { get; set; }

        public string StringSerialNo { get; set; }

        public string StringPropNo { get; set; }
    }
}