using System.Collections.Generic;
using System.Linq;

namespace Library
{
    public class FileFilter : IFilter
    {
        public List<string> ToIgnore { get; set; }

        public Dictionary<string, string> ToIgnoreDict { get; set; }

        public void AppendExtension(string extension)
        {
            ToIgnore = ToIgnore.Select(x => x + extension.Replace("*", string.Empty)).ToList();
        }
    }
}
