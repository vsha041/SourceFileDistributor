using System;
using System.Collections.Generic;
using System.IO;

namespace Library
{
    public class PathSorter : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var first = Convert.ToInt32(Path.GetFileName(x));
            var second = Convert.ToInt32(Path.GetFileName(y));

            if (first < second)
                return -1;

            if (first > second)
                return 1;

            return 0;
        }
    }

}
