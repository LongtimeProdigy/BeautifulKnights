using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Skill {

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
        public int level;

        public stats(float _degree, int _level)
        {
            degree = _degree;
            level = _level;
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

    public Skill(int index, int level)
    {
        XmlManager xmlmanager = new XmlManager();
        XmlNode node = xmlmanager.LoadNode(XmlManager.eType.Skill, index);

        info = new information(index,
            node.Attributes.GetNamedItem("name").Value,
            node.Attributes.GetNamedItem("text1").Value,
            node.Attributes.GetNamedItem("text2").Value);

        stat = new stats(int.Parse(node.Attributes.GetNamedItem(string.Format("lv{0}", level)).Value), level);
    }

    public virtual void Use(Player player)
    {

    }
}
