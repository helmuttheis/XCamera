using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCamera.UWP
{
    public class ManagerUwp : IManager
    {
        public async Task Open()
        {
            await Task.Delay(100);
        }
    }
}
