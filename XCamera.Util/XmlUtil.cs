using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XCamera.Util
{
    public static class XmlUtil
    {
        public static XmlNode EnsureElement(XmlNode parentNode, string szElement, string szText = "")
        {
            XmlNode oneNode = parentNode.SelectSingleNode(szElement);
            if (oneNode == null)
            {
                oneNode = parentNode.OwnerDocument.CreateElement(szElement);

                parentNode.AppendChild(oneNode);
            }
            if (!string.IsNullOrWhiteSpace(szText))
            {
                oneNode.InnerText = szText;
            }
            return oneNode;
        }
        public static XmlNode AddElement(XmlNode parentNode, string szElement, string szText = "")
        {
            XmlNode oneNode = parentNode.OwnerDocument.CreateElement(szElement);
            parentNode.AppendChild(oneNode);

            oneNode.InnerText = szText;
            return oneNode;
        }
        public static XmlNode EnsureElement(XmlNode parentNode, string szElement, string szAttrName, string szAttrValue)
        {
            XmlNode oneNode = parentNode.SelectSingleNode(szElement + "[@" + szAttrName + " ='" + szAttrValue + "']");
            if (oneNode == null)
            {
                oneNode = parentNode.OwnerDocument.CreateElement(szElement);
                XmlNode attrNode = parentNode.OwnerDocument.CreateAttribute(szAttrName);
                attrNode.InnerText = szAttrValue;
                oneNode.Attributes.SetNamedItem(attrNode);
                parentNode.AppendChild(oneNode);
            }
            return oneNode;
        }
        public static XmlNode EnsureAttribute(XmlNode oneNode, string szAttrName, string szAttrValue)
        {
            XmlNode attrNode = oneNode.Attributes.GetNamedItem(szAttrName);
            if (attrNode == null)
            {
                attrNode = oneNode.OwnerDocument.CreateAttribute(szAttrName);
                attrNode.InnerText = szAttrValue;
                oneNode.Attributes.SetNamedItem(attrNode);
            }
            return oneNode;
        }
    }
}
