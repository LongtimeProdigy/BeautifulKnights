using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class XmlManager {

    public enum eType
    {
        Character,
        Skill,
        Lance,
        Item,
        Debuff
    }

    public XmlNode LoadNode(eType type, int index)
    {
        XmlDocument document = new XmlDocument();
        document.Load(string.Format("{0}{1}{2}{3}", Application.dataPath, "/Resources/Xml/", type.ToString(), ".xml"));
        XmlNodeList nodelist = document[type.ToString()].ChildNodes;
        XmlNode node = nodelist.Item(index);

        return node;
    }

    public string LoadAttribute(eType type, int index, string attributename)
    {
        XmlDocument document = new XmlDocument();
        document.Load(string.Format("{0}{1}{2}{3}", Application.dataPath, "/Resources/Xml/", type.ToString(), ".xml"));
        XmlNodeList nodelist = document[type.ToString()].ChildNodes;
        XmlNode node = nodelist.Item(index);

        return node.Attributes.GetNamedItem(attributename).Value;
    }
}
