using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace XCamera.Droid
{
    class ExifDroid : XCamera.IExif
    {
        public async Task<string> GetComment(System.IO.Stream stream)
        {
            string szValue = "";
            await Task.Run(() =>
            {

                ExifInterface newExif = new ExifInterface(stream);

                szValue = newExif.GetAttribute(ExifInterface.TagUserComment);
            });
            return szValue;
        }
        public async Task<string> SetComment(System.IO.Stream stream, string newComment)
        {
            string szValue = "";
            await Task.Run(() =>
            {

                ExifInterface newExif = new ExifInterface(stream);

                szValue = newExif.GetAttribute(ExifInterface.TagUserComment);

                newExif.SetAttribute(ExifInterface.TagUserComment, newComment);
                newExif.SaveAttributes();
            });
            return szValue;
        }
    }
}