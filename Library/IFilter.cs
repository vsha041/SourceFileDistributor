using System.Collections.Generic;

namespace Library
{
    public interface IFilter
    {
        Dictionary<string, string> ToIgnoreDict { get; set; }

        List<string> ToIgnore { get; set; }

        void AppendExtension(string extension);
    }
}