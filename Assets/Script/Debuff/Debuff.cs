using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Debuff {
    #region parameters
    public struct information
    {
        public int index;
        public string name;
        public string description1;
        public string description2;
        public string description3;
        public string description4;

        public information(int _index, string _name, string _description1, string _description2, string _description3, string _description4)
        {
            index = _index;
            name = _name;
            description1 = _description1;
            description2 = _description2;
            description3 = _description3;
            description4 = _description4;
        }
    }

    public struct stats
    {
        public float degree;
        public int stack;

        public stats(float _degree, int _stack)
        {
            degree = _degree;
            stack = _stack;
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

    public Debuff(int index)
    {
        XmlManager xmlmanager = new XmlManager();
        XmlNode node = xmlmanager.LoadNode(XmlManager.eType.Debuff, index);

        info = new information(index,
            node.Attributes.GetNamedItem("name").Value,
            node.Attributes.GetNamedItem("text1").Value,
            node.Attributes.GetNamedItem("text2").Value,
            node.Attributes.GetNamedItem("text3").Value,
            node.Attributes.GetNamedItem("text4").Value);
    }

    public virtual void Use(Player player)
    {
        if(stat.stack != 0)
        {
            stat.stack = stat.stack - 1;
        }
    }
}
