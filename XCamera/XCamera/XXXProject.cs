using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace XCamera
{
    public class xXXXProject
    {


        public static string szProjectName { get; set; } = "Sample";
        public string szProjectPath { get; set; }
        public string szProjectFile { get; set; }
        private XmlDocument xmlDoc { get; set; }
        private XmlNode rootNode { get; set; }
        public xXXXProject()
        {
            // build the filename
            szProjectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), szProjectName);
            if( !Directory.Exists(szProjectPath))
            {
                Directory.CreateDirectory(szProjectPath);
            }
            szProjectFile =  Path.Combine(szProjectPath, szProjectName + ".xml");
            xmlDoc = new XmlDocument();
            if( !File.Exists(szProjectFile))
            {
                xmlDoc.LoadXml("<XCamera></XCamera>");
            }
            else
            {
                xmlDoc.Load(szProjectFile);
            }
            rootNode = xmlDoc.SelectSingleNode("//XCamera");
        }
        public List<string> GetImages()
        {
            List<string> imgList = new List<string>();
            string[] images = Directory.GetFiles(szProjectPath,"*.jpg");
            foreach (var image in images)
            {
                if (!IsDeleted(image))
                {
                    imgList.Add(image.Split(Path.DirectorySeparatorChar).LastOrDefault());
                }
            }

            return imgList;
        }
        public string GetImageFullName(string szImage)
        {
            return Path.Combine(szProjectPath, szImage);
        }
        
        public void Delete(string szFullImageName)
        {
            XmlNode imgNode = rootNode.SelectSingleNode("image[@name='" + szFullImageName + "']");
            if (imgNode == null)
            {
                imgNode = xmlDoc.CreateElement("image");
                XmlNode attrNode = xmlDoc.CreateAttribute("name");
                attrNode.InnerText = szFullImageName;
                imgNode.Attributes.SetNamedItem(attrNode);
                rootNode.AppendChild(imgNode);
            }
            XmlNode commentNode = imgNode.SelectSingleNode("deleted");
            if (commentNode == null)
            {
                commentNode = xmlDoc.CreateElement("deleted");

                imgNode.AppendChild(commentNode);
            }
            commentNode.InnerText = "1";
            Save();
        }
        public Boolean HasDeleted()
        {
            XmlNodeList imgNodes = rootNode.SelectNodes("image/deleted");
            return imgNodes.Count > 0;
        }
        public void ClearDeleted()
        {
            XmlNodeList imgNodes = rootNode.SelectNodes("image/deleted");
            for(int i=0; i<  imgNodes.Count;i++)
            {
                XmlNode imgNode = imgNodes[i].ParentNode;
                string szFullImageName = imgNode.Attributes.GetNamedItem("name").InnerText;
                System.IO.File.Delete(szFullImageName);
                imgNode.ParentNode.RemoveChild(imgNode);
            }
            Save();
        }
        public Boolean IsDeleted(string szFullImageName)
        {
            XmlNode imgNode = rootNode.SelectSingleNode("image[@name='" + szFullImageName + "']");
            if (imgNode == null)
            {
                return false;

            }
            XmlNode commentNode = imgNode.SelectSingleNode("deleted");
            if (commentNode == null)
            {
                return false;
            }
            return commentNode.InnerText.Equals("1");
        }
        public void SetComment(string szFullImageName, string szComment)
        {
            XmlNode imgNode = rootNode.SelectSingleNode("image[@name='" + szFullImageName + "']");
            if(imgNode == null )
            {
                imgNode = xmlDoc.CreateElement("image");
                XmlNode attrNode = xmlDoc.CreateAttribute("name");
                attrNode.InnerText = szFullImageName;
                imgNode.Attributes.SetNamedItem(attrNode);
                rootNode.AppendChild(imgNode);
            }
            XmlNode commentNode = imgNode.SelectSingleNode("comment");
            if (commentNode == null)
            {
                commentNode = xmlDoc.CreateElement("comment");
                
                imgNode.AppendChild(commentNode);
            }
            commentNode.InnerText = szComment;
        }
        
        public string GetComment(string szFullImageName)
        {
            XmlNode imgNode = rootNode.SelectSingleNode("image[@name='" + szFullImageName + "']");
            if (imgNode == null)
            {
                return "";
                
            }
            XmlNode commentNode = imgNode.SelectSingleNode("comment");
            if (commentNode == null)
            {
                return "";
            }
            return commentNode.InnerText;
        }
        public void Save()
        {
            XmlNode attrNode = rootNode.Attributes.GetNamedItem("isdirty");
            if (attrNode == null)
            {
                attrNode = xmlDoc.CreateAttribute("isdirty");
            }
            attrNode.InnerText = "1";
            rootNode.Attributes.SetNamedItem(attrNode);
            xmlDoc.Save(szProjectFile);
        }

        public Boolean IsDirty()
        {
            XmlNode attrNode = rootNode.Attributes.GetNamedItem("isdirty");
            if (attrNode == null)
            {
                return false;
            }
            return attrNode.InnerText.Equals("1");

        }

        public static List<string> GetList()
        {
            List<string> projList = new List<string>();
            string[] projects = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            foreach(var project in projects)
            {
                projList.Add(project.Split(Path.DirectorySeparatorChar).LastOrDefault());
            }

            return projList;
        }
    }
}
