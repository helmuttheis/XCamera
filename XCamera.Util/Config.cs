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
    }
}
