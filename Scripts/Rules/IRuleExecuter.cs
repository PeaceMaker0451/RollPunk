using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollPunk.Rules
{
    public interface IRuleExecuter
    {
        public object[] Execute(string eventName, params object[] args);
    }
}
