using System;
using System.Collections.Generic;

namespace Yoyo.Entity.Models
{
    public partial class SystemTask
    {
        public int Id { get; set; }

        public int Sort { get; set; }

        public int TaskType { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDesc { get; set; }
        public decimal Reward { get; set; }
        public int Aims { get; set; }
        public string Unit { get; set; }
        public decimal Devote { get; set; }
        public int Status { get; set; }
    }
}
