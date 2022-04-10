using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prolonger.Model
{
    public class STAN
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }
        public string Court { get; set; }
        public string Case { get; set; }
        public string SubCase { get; set; }
        public DateTime RegDate { get; set; }
        public string Judge { get; set; }
        public string SubJudge { get; set; }
        public string Littigans { get; set; }
        public DateTime Date { get; set; }
        public string Decision { get; set; }
        public string SubDecision { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

    }
}
