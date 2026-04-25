using System;
using System.Collections.Generic;

namespace RollPunk.Modding
{
    public partial class ModMetadata
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public List<string> Dependencies { get; set; }
    }
}
