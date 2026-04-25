using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootLoader
{
    internal class AppLoader
    {
        public void LoadClient()
        {
            AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
