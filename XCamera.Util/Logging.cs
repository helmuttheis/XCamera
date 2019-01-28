using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XCamera.Util
{
    public static class Logging
    {
        private static string szErrFile { get; set; }
        public static string szLogFile { get; set; }

        public static void Reset()
        {
            try
            {
                if( File.Exists(szLogFile))
                {
                    File.Delete(szLogFile);
                }
            }
            catch (Exception)
            {
            }
        }
        private static void EnsureErrFile()
        {
            if (string.IsNullOrWhiteSpace(szLogFile))
                return ;
            if (string.IsNullOrWhiteSpace(szErrFile))
            {
                szErrFile = szLogFile + ".err";
            }

        }
        public static Boolean WasLastError()
        {
            Boolean bRet = false;
            if (string.IsNullOrWhiteSpace(szLogFile))
                return bRet;
            try
            {
                EnsureErrFile();
                bRet = File.Exists(szErrFile);
            }
            catch (Exception)
            {

                throw;
            }
            return bRet;
        }
        public static string GetLastLog()
        {
            if (string.IsNullOrWhiteSpace(szLogFile))
                return "";
            try
            {
                return File.ReadAllText(szLogFile);
            }
            catch (Exception)
            {

                throw;
            }
            return "";
        }
        public static void AddMsg(string szMsg)
        {
            try
            {
                File.AppendAllText(szLogFile, szMsg + Environment.NewLine);
            }
            catch (Exception)
            {
            }
        }
        public static void AddError(string szMsg)
        {
            AddMsg("ERROR:" + szMsg);
            try
            {
                EnsureErrFile();
                if ( !File.Exists(szErrFile))
                {
                    File.WriteAllText(szErrFile, szMsg);
                }
            }
            catch (Exception)
            {
            }
        }
        public static void AddInfo(string szMsg)
        {
            AddMsg("INFO :" + szMsg);
        }
    }
}
