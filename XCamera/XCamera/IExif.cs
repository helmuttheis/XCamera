using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XCamera
{
    public interface IExif
    {
        Task<string> GetComment(string filename);
        Task<string> SetComment(string filename, string newComment);
    }
}
