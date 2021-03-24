using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialInteraction : MonoBehaviour {

    public List<Material> materialList;
    public static GameObject[] previewObjects;
	
    // Can be moved to DI on more advanced build
     private ComboBox comboBox;

    // Use this for initialization
	void Start () {
       
        comboBox = gameObject.GetComponent<ComboBox>();

        foreach (var material in materialList)
        {
            comboBox.items.Add(material.name);
        }

        
        previewObjects = GameObject.FindGameObjectsWithTag("PreviewObject");
       
        
	}
	
}
