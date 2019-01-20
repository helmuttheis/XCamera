using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace XCamera.Util
{
    public class Project
    {
        public static string szProjectName { get; set; } = "Sample";
        public string szProjectPath { get; set; }
        public string szProjectFile { get; set; }

        public string szTempProjectPath { get; set; }

        private string szBasePath { get; set; }
        private XmlDocument xmlDoc { get; set; }
        private XmlNode rootNode { get; set; }

        private XmlNode hierarchyNode { get; set; }
        private List<string> lstLevel { get; set; }

        private Dictionary<string, List<string>> dictLevel { get; set; }

        public Project(string szBasePath)
        {
            this.szBasePath = szBasePath;
            szProjectPath = Path.Combine(szBasePath, szProjectName);
            if (!Directory.Exists(szProjectPath))
            {
                Directory.CreateDirectory(szProjectPath);
            }
            szProjectFile = Path.Combine(szProjectPath, szProjectName + ".xml");
            xmlDoc = new XmlDocument();
            if (!File.Exists(szProjectFile))
            {
                xmlDoc.LoadXml("<XCamera></XCamera>");
            }
            else
            {
                xmlDoc.Load(szProjectFile);
            }
            rootNode = xmlDoc.SelectSingleNode("//XCamera");

            FillLevel();
        }
        private void FillLevel()
        {
            dictLevel = new Dictionary<string, List<string>>();
            hierarchyNode = EnsureElement(rootNode, "hierarchy");
            lstLevel = new List<string>();
            int levelCnt = lstLevel.Count;

            string szLevelName = "Gebäude";
            lstLevel.Add(szLevelName);
            levelCnt = lstLevel.Count;
            XmlNode levelNode = EnsureElement(hierarchyNode, "level" + levelCnt.ToString());
            EnsureAttribute(levelNode, "id",  levelCnt.ToString());
            EnsureAttribute(levelNode, "name", szLevelName);
            EnsureAttribute(AddElement(levelNode, "value", "Haus A"), "id", "1");
            EnsureAttribute(AddElement(levelNode, "value", "Haus B"), "id", "2");

            szLevelName = "Etage";
            lstLevel.Add(szLevelName);
            levelCnt = lstLevel.Count;
            levelNode = EnsureElement(hierarchyNode, "level" + levelCnt.ToString());
            EnsureAttribute(levelNode, "id", levelCnt.ToString());
            EnsureAttribute(levelNode, "name", szLevelName);
            EnsureAttribute(AddElement(levelNode, "value", "Etage 1"), "id", "1");
            EnsureAttribute(AddElement(levelNode, "value", "Etage 2"), "id", "2");

            szLevelName = "Wohnung";
            lstLevel.Add(szLevelName);
            levelCnt = lstLevel.Count;
            levelNode = EnsureElement(hierarchyNode, "level" + levelCnt.ToString());
            EnsureAttribute(levelNode, "id", levelCnt.ToString());
            EnsureAttribute(levelNode, "name", szLevelName);
            EnsureAttribute(AddElement(levelNode, "value", "Wohnung 1"), "id", "1");
            EnsureAttribute(AddElement(levelNode, "value", "Wohnung 2"), "id", "2");

            szLevelName = "Zimmer";
            lstLevel.Add(szLevelName);
            levelCnt = lstLevel.Count;
            levelNode = EnsureElement(hierarchyNode, "level" + levelCnt.ToString());
            EnsureAttribute(levelNode, "id", levelCnt.ToString());
            EnsureAttribute(levelNode, "name", szLevelName);
            EnsureAttribute(AddElement(levelNode, "value", "Flur"), "id", "1");
            EnsureAttribute(AddElement(levelNode, "value", "Küche"), "id", "2");
            EnsureAttribute(AddElement(levelNode, "value", "Wohnzimmer"), "id", "3");
            EnsureAttribute(AddElement(levelNode, "value", "Bad"), "id", "4");
        }
        private XmlNode EnsureElement(XmlNode parentNode,string szElement, string szText="")
        {
            XmlNode oneNode = parentNode.SelectSingleNode(szElement);
            if (oneNode == null)
            {
                oneNode = xmlDoc.CreateElement(szElement);

                parentNode.AppendChild(oneNode);
            }
            oneNode.InnerText = szText;
            return oneNode;
        }
        private XmlNode AddElement(XmlNode parentNode, string szElement, string szText = "")
        {
            XmlNode oneNode = xmlDoc.CreateElement(szElement);
            parentNode.AppendChild(oneNode);
            
            oneNode.InnerText = szText;
            return oneNode;
        }
        private XmlNode EnsureElement(XmlNode parentNode, string szElement, string szAttrName, string szAttrValue)
        {
            XmlNode oneNode = parentNode.SelectSingleNode(szElement + "[@" + szAttrName + " ='" + szAttrValue + "']");
            if (oneNode == null)
            {
                oneNode = xmlDoc.CreateElement(szElement);
                XmlNode attrNode = xmlDoc.CreateAttribute(szAttrName);
                attrNode.InnerText = szAttrValue;
                oneNode.Attributes.SetNamedItem(attrNode);
                parentNode.AppendChild(oneNode);
            }
            return oneNode;
        }
        private XmlNode EnsureAttribute(XmlNode oneNode, string szAttrName, string szAttrValue)
        {
            XmlNode attrNode = oneNode.Attributes.GetNamedItem(szAttrName);
            if( attrNode == null)
            { 
                attrNode = xmlDoc.CreateAttribute(szAttrName);
                attrNode.InnerText = szAttrValue;
                oneNode.Attributes.SetNamedItem(attrNode);
            }
            return oneNode;
        }

        public List<string> GetImages()
        {
            List<string> imgList = new List<string>();
            string[] images = Directory.GetFiles(szProjectPath, "*.jpg");
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
            for (int i = 0; i < imgNodes.Count; i++)
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
            if (imgNode == null)
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

            XmlNode tempDirNode = rootNode.SelectSingleNode("//tempdir");
            if (tempDirNode == null)
            {
                tempDirNode = xmlDoc.CreateElement("tempdir");
                rootNode.AppendChild(tempDirNode);
            }
            tempDirNode.InnerText = szTempProjectPath;

            xmlDoc.Save(szProjectFile);
        }
        public string GetTempDir()
        {
            XmlNode tempDirNode = rootNode.SelectSingleNode("//tempdir");
            if (tempDirNode == null)
            {
                return "";
            }
            return tempDirNode.InnerText.Trim();
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
        public List<string> GetLevelList()
        {
            List<string> lstLevel = new List<string>();
            lstLevel.Add("Gebäude");
            lstLevel.Add("Etage");
            lstLevel.Add("Wohnung");
            lstLevel.Add("Zimmer");

            return lstLevel;
        }
        public List<string> GetLevelValuesList(int iLevelId)
        {
            XmlNode levelNode = hierarchyNode.SelectSingleNode("child::level" + iLevelId.ToString());

            List<string> lstLevel = new List<string>();
            if (levelNode != null )
            {
                XmlNodeList valueNodes = levelNode.SelectNodes("child::value");
                foreach (XmlNode valueNode in valueNodes)
                {
                    lstLevel.Add(valueNode.InnerText);
                }
            }

            return lstLevel;
        }

        public static List<string> GetList()
        {
            List<string> projList = new List<string>();
            string[] projects = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            foreach (var project in projects)
            {
                string szProjectName = project.Split(Path.DirectorySeparatorChar).LastOrDefault();
                if (Project.IsValidName(szProjectName))
                {
                    projList.Add(szProjectName);
                }
            }

            return projList;
        }
        public static Boolean IsValidName(string szProjectName)
        {
            return !szProjectName.StartsWith("__");
        }
    }
}
