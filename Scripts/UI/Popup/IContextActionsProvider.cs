using System;
using System.Collections.Generic;

namespace RollPunk.Popup
{
    internal interface IContextActionsProvider
    {
        IEnumerable<ContextAction> GetContextActions();
    }
}
