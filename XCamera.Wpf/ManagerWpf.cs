using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCamera.Wpf
{
    public class ManagerWpf:IManager
    {
        public async Task Open()
        {
            await Task.Delay(100);
        }
    }
}
