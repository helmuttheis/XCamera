﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace XCamera.Util
{
    public class ProjectUtil
    {
        static private HttpClient _httpClient;

        public static HttpClient httpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }
                return _httpClient;
            }
        }

        public static string szBasePath { get; set; } = "";
        public static string szServer { get; set; }

        public static Boolean IsValidName(string szProjectName)
        {
            return !szProjectName.StartsWith("__");
        }


        public static List<string> GetProjectList()
        {
            List<string> projList = new List<string>();
            string[] projects = Directory.GetDirectories(ProjectUtil.szBasePath);
            foreach (var project in projects)
            {
                string szProjectName = project.Split(Path.DirectorySeparatorChar).LastOrDefault();
                if (ProjectUtil.IsValidName(szProjectName))
                {
                    projList.Add(szProjectName);
                }
            }

            return projList;
        }

        public static List<string> GetRemoteProjectList()
        {
            string szJson = "";
            List<string> projList = new List<string>();

            Task.Run(async () =>
            {
                szJson = await httpClient.GetStringAsync(szServer);
            }).Wait();
            //  [{"szProjectName":"Test1","lSize":11496036},{"szProjectName":"Test2","lSize":11496036}]
            List<JsonProject> remoteProjects = JsonConvert.DeserializeObject<List<JsonProject>>(szJson);
            foreach (var project in remoteProjects)
            {
                if (ProjectUtil.IsValidName(project.szProjectName))
                {
                    projList.Add(project.szProjectName);
                }
            }

            return projList;
        }
        public static Boolean DownloadFile(string szProjetName, string szFileName, string szDestFile)
        {
            Boolean bRet = false;
            byte[] byteArr = null;
            Task.Run(async () =>
            {
                byteArr = await httpClient.GetByteArrayAsync(szServer + "?project=" + szProjetName + "&file=" + szFileName);
            }).Wait();
            File.WriteAllBytes(szDestFile, byteArr);

            return bRet;
        }
        public static Boolean SendFile(string szProjectName, string szFileName)
        {
            Boolean bRet = false;
            byte[] byteArr = null;
            HttpResponseMessage response = null;

            try
            {
                Task.Run(async () =>
                {
                    string szSourceFile = Path.Combine(ProjectPath(szProjectName), szFileName);

                    byteArr = File.ReadAllBytes(szSourceFile);
                    HttpContent httpContent = new ByteArrayContent(byteArr);
                    response = await httpClient.PostAsync(szServer + "?project=" + szProjectName + "&file=" + szFileName, httpContent);
                }).Wait();

            }
            catch (Exception ex)
            {
                Logging.AddError("SendFile " + ex.ToString());
            }

            return bRet;
        }
        public static Boolean SendJson(string szProjectName, string szJson)
        {
            Boolean bRet = false;
            HttpResponseMessage response = null;

            try
            {
                Task.Run(async () =>
                {
                    HttpContent httpContent = new StringContent(szJson);
                    response = await httpClient.PostAsync(szServer + "?project=" + szProjectName + "&json=true", httpContent);
                }).Wait();

            }
            catch (Exception ex)
            {
                Logging.AddError("SendJson " + ex.ToString());
            }

            return bRet;
        }
        public static string ProjectPath(string szProjectName)
        {
            string szProjectPath = Path.Combine(szBasePath, szProjectName);
            if (!Directory.Exists(szProjectPath))
            {
                Directory.CreateDirectory(szProjectPath);
            }
            return szProjectPath;
        }
        public static string ProjectDbFile(string szProjectName)
        {
            string szProjectPath = ProjectPath(szProjectName);
            return Path.Combine(szProjectPath, szProjectName + ".db");
        }
        public static void MergeProject(string szRemoteProject, string szLocalProject)
        {

        }
        public static async void CopyProject(string szRemoteProject, string szLocalProject)
        {
            try
            {
                byte[] arr = await DownloadFileAsync(szServer + "?project=" + szRemoteProject + "&file=" + szRemoteProject + ".db");
                File.WriteAllBytes(ProjectDbFile(szLocalProject), arr);

                // load the DB file
                ProjectSql tmpProject = new ProjectSql(szLocalProject);
                // get all images
                List<Bild> bilder = tmpProject.GetBilder();
                foreach (var bild in bilder)
                {
                    string szImageName = bild.Name;
                    string szFullImageName = tmpProject.GetImageFullName(szImageName);
                    arr = await DownloadFileAsync(szServer + "?project=" + szRemoteProject + "&file=" + szImageName);
                    File.WriteAllBytes(szFullImageName, arr);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        static async Task<byte[]> DownloadFileAsync(string szFileUrl)
        {
            // var _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

            try
            {
                using (var httpResponse = await _httpClient.GetAsync(szFileUrl))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return await httpResponse.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        //Url is Invalid
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                //Handle Exception
                return null;
            }
        }
    }
}
