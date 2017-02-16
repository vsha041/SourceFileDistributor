using System.Collections.Generic;
using System.Linq;

namespace Library
{
    public class FileFilter
    {
        public List<string> FilesToIgnore { get; set; }

        public Dictionary<string, string> FilesToIgnoreDict { get; set; }

        public void AppendExtension(string extension)
        {
            FilesToIgnore = FilesToIgnore.Select(x => x + extension.Replace("*", string.Empty)).ToList();
        }
    }
}
