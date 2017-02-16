using System.Collections.Generic;

namespace Library
{
    public class FilePatternFilter
    {
        public List<string> FilePatternsToIgnore { get; set; }

        public Dictionary<string, string> FilePatternToIgnoreDict { get; set; }
    }

}
