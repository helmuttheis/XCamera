using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XCamera
{
    public enum ExifTag
    {
        XPComment = 40092
    }
    public interface IExif
    {
        Task<string> GetComment(Stream stream);
        Task<string> SetComment(Stream stream, string newComment);
    }
}
