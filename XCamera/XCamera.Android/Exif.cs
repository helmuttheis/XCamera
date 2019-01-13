using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace XCamera.Droid
{
    class Exif : XCamera.IExif
    {
        public async Task<string> GetComment(string filename)
        {
            return "";
        }
        public async Task<string> SetComment(string filename, string newComment)
        {
            return "";
        }
    }
}