using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.UIFields
{
public enum ContainerType
{
    VBox,
    HBox,
    VFlow,
    HFlow,
    Scroll
}

    public static class ContainerTypeMapper
    {
        public static bool TryParse(string value, out ContainerType result)
            => Enum.TryParse(value, true, out result);
    }
}
