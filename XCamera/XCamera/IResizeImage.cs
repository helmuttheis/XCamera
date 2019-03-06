using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace XCamera
{
    public static class Global
    {
        public static IResizeImage resizeImage { get; set; }
    }
    public interface IResizeImage
    {
        Task<byte[]> ResizeImage(byte[] imageData, float width, float height);
    }
}
