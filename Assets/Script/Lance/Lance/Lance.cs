using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class Lance {

    #region paramters
    public struct information
    {
        public int index;
        public string name;

        public information(int _index, string _name)
        {
            index = _index;
            name = _name;
        }
    }

    public struct stats
    {
        public float power;
        public float max_power;

        public float destructive;
        public float max_destructive;

        public float penetration;
        public float max_penetration;

        public float parring;
        public float max_parring;

        public float weight;
        public float max_weight;

        public stats(float _power, float _destructive, float _penetration, float _parring, float _weight, float max)
        {
            power = _power;
            destructive = _destructive;
            penetration = _penetration;
            parring = _parring;
            weight = _weight;

            max_power = max;
            max_destructive = max;
            max_penetration = max;
            max_parring = max;
            max_weight = max;
        }

        public float getstat(int index)
        {
            if (index == 0)
                return power;
            else if (index == 1)
                return destructive;
            else if (index == 2)
                return penetration;
            else if (index == 3)
                return parring;
            else if (index == 4)
                return weight;
            else
                return -1.0f;
        }

        public float getmaxstat(int index)
        {
            if (index == 0)
                return max_power;
            else if (index == 1)
                return max_destructive;
            else if (index == 2)
                return max_penetration;
            else if (index == 3)
                return max_parring;
            else if (index == 4)
                return max_weight;
            else
                return -1.0f;
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
    protected LanceSkill skill;
    public LanceSkill Skill
    {
        get { return skill; }
    }
    protected int skill_Lv;

    protected bool isused;
    public bool Isused
    {
        get { return isused; }
        set { isused = value; }
    }
    #endregion

    public Lance(int index)
    {
        XmlManager xmlmanager = new XmlManager();
        XmlNode node = xmlmanager.LoadNode(XmlManager.eType.Lance, index);

        info = new information(index, node.Attributes.GetNamedItem("name").Value);

        stat = new stats(int.Parse(node.Attributes.GetNamedItem("power").Value),
            int.Parse(node.Attributes.GetNamedItem("destructive").Value),
            int.Parse(node.Attributes.GetNamedItem("penetration").Value),
            int.Parse(node.Attributes.GetNamedItem("parring").Value),
            int.Parse(node.Attributes.GetNamedItem("weigth").Value),
            150);

        skill = new LanceSkill(int.Parse(node.Attributes.GetNamedItem("skillid").Value) - 1,
            int.Parse(node.Attributes.GetNamedItem("skilllv").Value));
    }
}
