using System.Collections.Generic;

namespace Library
{
    public class FilePatternFilter : IFilter
    {
        public List<string> ToIgnore { get; set; }

        public Dictionary<string, string> ToIgnoreDict { get; set; }

        public void AppendExtension(string extension)
        {
            
        }
    }
}
