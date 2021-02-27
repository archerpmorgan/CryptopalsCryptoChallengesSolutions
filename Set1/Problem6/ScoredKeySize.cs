using System;

namespace Problem6 {
    public class ScoredKeySize : IComparable
    {

        public double score { get; set; }
        public int keySize { get; set; }

        int IComparable.CompareTo(object obj)
        {
            ScoredKeySize o = (ScoredKeySize) obj;
            if (o.score == this.score)
            {
                return 0;
            }
            else
            {
                return this.score < o.score ? -1 : 1;
            }
        }
    }
}
