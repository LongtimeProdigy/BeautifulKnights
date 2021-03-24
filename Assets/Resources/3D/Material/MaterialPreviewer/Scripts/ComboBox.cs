using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboBox : MonoBehaviour {

    public List<string> items;
    
    private string selectedItem = "Select material";
    private bool isEditing = false;

    public float leftFloat = 10;
    public float topFloat = 25;
    public float boxWidth = Screen.width /6;
    public float boxHeight = 25;


    private void OnGUI()
    {
        boxWidth = Screen.width / 6;
        leftFloat = Screen.width - (boxWidth + 10);

        if (GUI.Button(new Rect(leftFloat, topFloat, boxWidth, boxHeight), selectedItem))
        {
            isEditing = true;
            
        }

        if (isEditing)
        {
            for (int element = 0; element < items.Count; element++)
            {
                if (GUI.Button(new Rect(leftFloat, boxHeight +  (boxHeight * element) + topFloat, boxWidth, boxHeight), items[element]))
                {
                    selectedItem = items[element];
                    isEditing = false;

                    Material selectedMaterial = this.GetComponent<MaterialInteraction>().materialList.Find(m=>m.name == selectedItem);

                    if (selectedMaterial != null)
                    { 
                        // set material on all preview objects
                        foreach (var previewObject in MaterialInteraction.previewObjects)
                        {
                            SceneSetup.normalMaterial.value = selectedMaterial.GetFloat("_BumpScale");
                            SceneSetup.ambientOcclusionMaterial.value = selectedMaterial.GetFloat("_OcclusionStrength");
                            SceneSetup.specularMaterial.value = selectedMaterial.GetFloat("_Glossiness");
                            SceneSetup.heightMaterial.value = selectedMaterial.GetFloat("_Parallax");

                            previewObject.GetComponent<Renderer>().material = selectedMaterial;

                            // set tiling
                            SceneSetup.tilingX.text = selectedMaterial.mainTextureScale.x.ToString();
                            SceneSetup.tilingY.text = selectedMaterial.mainTextureScale.y.ToString();

                        }
                    }

                }
            }
        }
    }
}
