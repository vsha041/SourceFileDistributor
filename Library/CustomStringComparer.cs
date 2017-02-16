using System;
using System.Collections.Generic;

namespace Library
{
    public class CustomStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return String.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}
