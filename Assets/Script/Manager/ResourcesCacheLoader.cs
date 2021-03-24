using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;
using System.Xml;

public class ResourcesCacheLoader : Singleton<ResourcesCacheLoader>
{
    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    public void LoadXml(string _content)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(_content);

        prefabs.Clear(); //.. 메모리 깔끔하게 비우기

        var rootNode = doc.SelectSingleNode("Root");


    }

    public GameObject GetPrefab(string _key)
    {
        return prefabs[_key];
    }
}
