using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XCamera.Util;

namespace XCamera.Util
{
    public class Config
    {
        // we crreate a singleton here
        static Config _current;
        public static Config current
        {
            get {
                if( _current == null )
                {
                    _current = new Config();
                }
                return _current;
            }
        }
        public static string szConfigFile;
        private XmlDocument xmlDoc;
        private XmlNode settingsNode;
        public Config()
        {

            xmlDoc = new XmlDocument();
            if( File.Exists(szConfigFile))
            {
                xmlDoc.Load(szConfigFile);
            }
            else
            {
                xmlDoc.LoadXml("<Settings></Settings>");
            }
            settingsNode = xmlDoc.SelectSingleNode("//Settings");
            SaveDefault();
        }
        public string szCurProject 
        {
            get
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "curproject");
                if (string.IsNullOrWhiteSpace(oneNode.InnerText))
                {
                    oneNode.InnerText = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                return oneNode.InnerText;
            }
            set
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "curproject");
                oneNode.InnerText = value;
                Save();
            }
        }
        public string szBasedir {
            get {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "basedir");
                if( string.IsNullOrWhiteSpace(oneNode.InnerText))
                {
                    oneNode.InnerText = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments);
                }
                return oneNode.InnerText;
            }
            set {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "basedir");
                oneNode.InnerText = value;
                Save();
            }
        }
        public string szPort
        {
            get
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "port");
                if( string.IsNullOrWhiteSpace(oneNode.InnerText))
                {
                    oneNode.InnerText = "8080";
                }
                return oneNode.InnerText;
            }
            set
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "port");
                oneNode.InnerText = value;
                Save();
            }
        }
        public string szIP
        {
            get
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "ip");
                return oneNode.InnerText;
            }
            set
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "ip");
                oneNode.InnerText = value;
                Save();
            }
        }
        public string szWordTemplate
        {
            get
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "wordtemplate");
                string szTemplate = oneNode.InnerText.Trim();
                if( string.IsNullOrWhiteSpace(szTemplate))
                {
                    szTemplate = "template.docx";
                }
                if (!Path.IsPathRooted(szTemplate))
                {
                    szTemplate = Path.Combine(Path.GetDirectoryName(szConfigFile), szTemplate);
                }
                return szTemplate;
            }
            set
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "wordtemplate");
                oneNode.InnerText = value;
                Save();
            }
        }

        public string szPicSuffix
        {
            get
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "pathPicSuffix");
                string szPicSuffix = oneNode.InnerText.Trim();
                if(string.IsNullOrEmpty(szPicSuffix))
                {
                    szPicSuffix = "Fotos";
                }
                return szPicSuffix;
            }
            set
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "pathPicSuffix");
                oneNode.InnerText = value;
                Save();
            }
        }
        public string szDbSuffix
        {
            get
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "pathDbSuffix");
                string szSuffix = oneNode.InnerText.Trim();
                if (string.IsNullOrEmpty(szSuffix))
                {
                    szSuffix = "";
                }
                return szSuffix;
            }
            set
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "pathDbSuffix");
                oneNode.InnerText = value;
                Save();
            }
        }
        public string szWordEmptySearch
        {
            get
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "szWordEmptySearch");
                string szWordEmptySearch = oneNode.InnerText.Trim();
                if (string.IsNullOrEmpty(szWordEmptySearch))
                {
                    szWordEmptySearch = "-";
                }
                return szWordEmptySearch;
            }
            set
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "szWordEmptySearch");
                oneNode.InnerText = value;
                Save();
            }
        }
        public string szWordEmptyInfo
        {
            get
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "szWordEmptyInfo");
                string szWordEmptyInfo = oneNode.InnerText.Trim();
                if (string.IsNullOrEmpty(szWordEmptySearch))
                {
                    szWordEmptyInfo = "-";
                }
                return szWordEmptyInfo;
            }
            set
            {
                XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "szWordEmptyInfo");
                oneNode.InnerText = value;
                Save();
            }
        }
        public void SetProjectStatus(string szProjectName,STATUS status)
        {
            XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "status","project",szProjectName);
            oneNode.InnerText = status.ToString();
            Save();
        }
        public string GetProjectStatus(string szProjectName)
        {
            XmlNode oneNode = XmlUtil.EnsureElement(settingsNode, "status", "project", szProjectName);
            return oneNode.InnerText;
        }
        public void Save()
        {
            xmlDoc.Save(szConfigFile);
        }

        public void SaveDefault()
        {
            var dummy = this.szWordTemplate;
            this.szWordTemplate = dummy;
            dummy = this.szBasedir;
            this.szBasedir = dummy;
            dummy = this.szCurProject;
            this.szCurProject = dummy;
            dummy = this.szIP;
            this.szIP = dummy;
            dummy = this.szPort;
            this.szPort = dummy;
            dummy = this.szPicSuffix;
            this.szPicSuffix = dummy;
            dummy = this.szDbSuffix;
            this.szDbSuffix = dummy;
            dummy = this.szWordEmptySearch;
            szWordEmptySearch = dummy;
            dummy = this.szWordEmptyInfo;
            szWordEmptyInfo = dummy;
            Save();
        }
    }
}
