using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace XCamera.iOS
{
    public class ManageriOS : IManager
    {
        public async Task Open()
        {
            await Task.Delay(100);
        }
    }
}