using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Helpers;

public class SceneSetup : MonoBehaviour {

    private Slider lightDirection;
    public static Slider normalMaterial;
    public static Slider ambientOcclusionMaterial;
    public static Slider specularMaterial;
    public static Slider heightMaterial;
    public static InputField tilingX;
    public static InputField tilingY;

    public Light sceneLight;
    

    public Color errorCode { get { return new Color(1, 0.5f, 0.5f);}} 
    // Reference to preview object to get material etc.
    public GameObject[] previewObjects;


	// Use this for initialization
	void Start () {
        previewObjects = GameObject.FindGameObjectsWithTag("PreviewObject");

        lightDirection = GameObject.Find("Slider Light Direction").GetComponent<Slider>();
        normalMaterial = GameObject.Find("Slider Normal").GetComponent<Slider>();
        ambientOcclusionMaterial = GameObject.Find("Slider AO").GetComponent<Slider>();
        specularMaterial = GameObject.Find("Slider Spec").GetComponent<Slider>();
        heightMaterial = GameObject.Find("Slider Height").GetComponent<Slider>();
        tilingX = GameObject.Find("TilingX").GetComponent<InputField>();
        tilingY = GameObject.Find("TilingY").GetComponent<InputField>();

        SetupSliderValues();
        SetupInputFieldValues();
	}

    public void SetupSliderValues()
    {
        // Set default values
        lightDirection.value = gameObject.GetComponent<Camera>().transform.eulerAngles.y;
        normalMaterial.value = previewObjects[0].GetComponent<MeshRenderer>().material.GetFloat("_BumpScale");
        ambientOcclusionMaterial.value = previewObjects[0].GetComponent<MeshRenderer>().material.GetFloat("_OcclusionStrength");
        specularMaterial.value = previewObjects[0].GetComponent<MeshRenderer>().material.GetFloat("_Glossiness");
        heightMaterial.value = previewObjects[0].GetComponent<MeshRenderer>().material.GetFloat("_Parallax");
    }

    public void SetupInputFieldValues()
    {
        tilingX.text = previewObjects[0].GetComponent<MeshRenderer>().material.mainTextureScale.x.ToString();
        tilingY.text = previewObjects[0].GetComponent<MeshRenderer>().material.mainTextureScale.y.ToString();
    }
	
	// Update is called once per frame
	void Update () {
		sceneLight.transform.Rotate(Vector3.up * lightDirection.value);

        foreach (var previewObject in previewObjects)
        {

            SetMaterialPropertyValue(previewObject, "_BumpScale", normalMaterial.value);
            SetMaterialPropertyValue(previewObject, "_OcclusionStrength", ambientOcclusionMaterial.value);
            SetMaterialPropertyValue(previewObject, "_Glossiness", specularMaterial.value);
            SetMaterialPropertyValue(previewObject, "_Parallax", heightMaterial.value);

            TilingManagement(previewObject);
        }
   
    }

    private static void TilingManagement(GameObject previewObject)
    {
        // Reset error color

        tilingX.GetComponent<Image>().color = ColorsDefinition.defaultBackground;
        tilingY.GetComponent<Image>().color = ColorsDefinition.defaultBackground;

        float newTilingX;
        float newTilingY;

        bool tilingXParseResults = float.TryParse(tilingX.text, out newTilingX);
        bool tilingYParseResults = float.TryParse(tilingY.text, out newTilingY);

        if (tilingXParseResults && tilingYParseResults)
        {
            previewObject.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(newTilingX, newTilingY);
        }
        else
        {
            // Highlight error input field 
            if (!tilingXParseResults)
            {
                tilingX.GetComponent<Image>().color = ColorsDefinition.errorCode;
            }
            if (!tilingYParseResults)
            {
                tilingY.GetComponent<Image>().color = ColorsDefinition.errorCode;
            }

        }
    }

    private static void SetMaterialPropertyValue(GameObject previewObject, string propertyName, float propertyValue)
    {
        previewObject.GetComponent<MeshRenderer>().material.SetFloat(propertyName, propertyValue);
    }

}
