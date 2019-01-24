using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCameraManager
{
    public class JsonProject
    {
        public string szProjectName { get; set; }
        public long lSize { get; set; }
    }
    public class JsonError
    {
        public string szMessage{ get; set; }
    }
}
