using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Item {

    #region parameters
    public struct information
    {
        public int index;
        public string name;
        public string description1;
        public string description2;

        public information(int _index, string _name, string _description1, string _description2)
        {
            index = _index;
            name = _name;
            description1 = _description1;
            description2 = _description2;
        }
    }

    public struct stats
    {
        public float degree;

        public stats(float _degree)
        {
            degree = _degree;
        }
    }

    protected information info;
    public information Info
    {
        get { return info; }
    }
    protected stats stat;
    public stats Stat
    {
        get { return stat; }
    }
    protected bool isused;
    public bool Isused
    {
        get { return isused; }
        set { isused = value; }
    }
    #endregion

    public Item(int index)
    {
        XmlManager xmlmanager = new XmlManager();
        XmlNode node = xmlmanager.LoadNode(XmlManager.eType.Item, index);

        info = new information(index,
            node.Attributes.GetNamedItem("name").Value,
            node.Attributes.GetNamedItem("text1").Value,
            node.Attributes.GetNamedItem("text2").Value);
    }

    public virtual void Use(Player player)
    {

    }
}
