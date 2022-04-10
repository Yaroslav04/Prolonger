using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Prolonger.Model
{
    public class ERSR : IEquatable<ERSR>
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int N { get; set; }

        [Indexed(Name = "ListingID", Order = 1, Unique = true)]
        public string Id { get; set; }
        public string Court { get; set; }
        public string JusticeDecision { get; set; }
        public string JusticeKind { get; set; }
        public string Category { get; set; }
        public string Case { get; set; }
        public DateTime DecisionDate    { get; set; }
        public DateTime PublicDate { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string Text { get; set; }
        public string CriminalNumber { get; set; }
        public DateTime DownloadDate { get; set; }

        public bool Equals(ERSR ersr)
        {
            //Check whether the compared object is null.  
            if (Object.ReferenceEquals(ersr, null)) return false;

            //Check whether the compared object references the same data.  
            if (Object.ReferenceEquals(this, ersr)) return true;

            //Check whether the UserDetails' properties are equal.  
            return N.Equals(ersr.N);
        }

        // If Equals() returns true for a pair of objects   
        // then GetHashCode() must return the same value for these objects.  

        public override int GetHashCode()
        {

            //Get hash code for the UserName field if it is not null.  
            int hashN = N == null ? 0 : N.GetHashCode();

            //Calculate the hash code for the GPOPolicy.  
            return hashN;
        }

    }


}
