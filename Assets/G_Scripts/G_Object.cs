using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//Manages object buttons, stores and displays information and handles button clicks
public class G_Object : MonoBehaviour,ISelectHandler,IPointerExitHandler
{
    private G_GoogleLoader googleLoader;
    private EYSYSTEM eySys;
    private G_MenuManager menuManager;
    //private float posX, posY, posZ, rotX, rotY, rotZ;
    private Texture2D preview;
    public TMP_InputField inputField;
    public int id;
    public string objectNumber;
    public string projectName;
    public string objectName;
    public string objectDescription;
    public string objectPath;
    public GameObject _gameObject;

    public void Start()
    {
        googleLoader = FindObjectOfType<G_GoogleLoader>();
        eySys = FindObjectOfType<EYSYSTEM>();
        menuManager = FindObjectOfType<G_MenuManager>();
    }

    //Sets all correct information whenever the buttons are clicked
    public void OnClick()
    {
        //Use ID of clicked element to instantiate element from gameobjects
        int index = gameObject.GetComponent<G_Object>().id;

        //Hides current object before setting it to whatever the index is then instantiating it
        googleLoader.currentObject.SetActive(false);
        Destroy(googleLoader.currentObject);
        googleLoader.currentObject = googleLoader.Items[index];
        googleLoader.currentObject = Instantiate<GameObject>(googleLoader.Items[index], googleLoader.Rotator.transform);

        //Makes item grounded if needed
        googleLoader.MakeGrounded(googleLoader.currentObject);

        //Makes sure the item is active
        if (googleLoader.currentObject.activeSelf == false)
            googleLoader.currentObject.SetActive(true);

        GetBound(googleLoader.currentObject);
        ZoomFit(FindObjectOfType<Camera>(), googleLoader.currentObject, false);

        ////Rounds transforms to 2 decimal places
        //posX = Mathf.Round(FindObjectOfType<Camera>().transform.position.x * 100f) / 100f;
        //posY = Mathf.Round(FindObjectOfType<Camera>().transform.position.y * 100f) / 100f;
        //posZ = Mathf.Round(FindObjectOfType<Camera>().transform.position.z * 100f) / 100f;
        //rotX = Mathf.Round(FindObjectOfType<Camera>().transform.rotation.x * 100f) / 100f;
        //rotY = Mathf.Round(FindObjectOfType<Camera>().transform.rotation.y * 100f) / 100f;
        //rotZ = Mathf.Round(FindObjectOfType<Camera>().transform.rotation.z * 100f) / 100f;


        //Sets information to corresponding information in the list data from Google
        googleLoader.objNumTxt.GetComponent<TextMeshProUGUI>().text = "#" + googleLoader.objectNumbersList[index];
        googleLoader.objNameTxt.GetComponent<TextMeshProUGUI>().text = googleLoader.objNameList[index];
        googleLoader.projectNameTxt.GetComponent<TextMeshProUGUI>().text = googleLoader.projectNameList[index];
        googleLoader.objDescTxt.GetComponent<TextMeshProUGUI>().text = googleLoader.objDescList[index];
        googleLoader.objPathTxt.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<G_Object>().objectPath;

        
        //googleLoader.posXTxt.GetComponent<TextMeshProUGUI>().text = posX.ToString();
        //googleLoader.posYTxt.GetComponent<TextMeshProUGUI>().text = posY.ToString();
        //googleLoader.posZTxt.GetComponent<TextMeshProUGUI>().text = posZ.ToString();
        //googleLoader.rotXTxt.GetComponent<TextMeshProUGUI>().text = rotX.ToString();
        //googleLoader.rotYTxt.GetComponent<TextMeshProUGUI>().text = rotY.ToString();
        //googleLoader.rotZTxt.GetComponent<TextMeshProUGUI>().text = rotZ.ToString();

        //Hides asset menu
        googleLoader.assetmenu.SetActive(false);
        googleLoader.menuOpen = false;

        if(!googleLoader.menuOpen)
            menuManager.previewImage.gameObject.SetActive(false);

        //Clears current stored materials for wireframe
        eySys.currentMaterials.Clear();
        eySys.renderers.Clear();
    }


    internal static Bounds GetBound(GameObject go)
    {
        Bounds b = new Bounds(go.transform.position, Vector3.zero);
        var rList = go.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer r in rList)
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    /// <summary>
    /// Adjust the camera to zoom fit the game object
    /// There are multiple directions to get zoom-fit view of the game object,
    /// if ViewFromRandomDirecion is true, then random viewing direction is chosen
    /// else, the camera's forward direction will be sused
    /// </summary>
    /// <param name="c"> The camera, whose position and view direction will be 
    //                   adjusted to implement zoom-fit effect </param>
    /// <param name="go"> The GameObject which will be zoom-fit. This object may have
    ///                   children objects as well </param>
    /// <param name="ViewFromRandomDirecion"> if random viewing direction is chozen. </param>
    /// 
    internal static void ZoomFit(Camera c, GameObject go, bool ViewFromRandomDirecion = false)
    {
        Bounds b = GetBound(go);
        Vector3 max = b.size;
        float radius = Mathf.Max(max.x, Mathf.Max(max.y, max.z));
        float dist = radius / Mathf.Sin(c.fieldOfView * Mathf.Deg2Rad);
        Debug.Log("Radius = " + radius + " dist = " + dist);

        Vector3 view_direction = ViewFromRandomDirecion ? UnityEngine.Random.onUnitSphere : c.transform.InverseTransformDirection(Vector3.forward);

        Vector3 pos = view_direction * dist + b.center;
        c.transform.position = pos;
        c.transform.LookAt(b.center);
    }

    
    public void OnSelect(BaseEventData eventData)
    {
        if (googleLoader.menuOpen)
        {
            //Enables preview Image
            menuManager.previewImage.gameObject.SetActive(true);
            
            //Grab model assigned to button and create preview image
            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            preview = RuntimePreviewGenerator.GenerateModelPreview(gameObject.GetComponent<G_Object>()._gameObject.transform, 512, 512, false, true);
            menuManager.previewImage.sprite = Sprite.Create(preview, new Rect(0.0f, 0.0f, preview.width, preview.height), new Vector2(0.5f, 0.5f), 100.0f);
         
            //Displays object description            
            menuManager.previewText.gameObject.GetComponent<TMP_Text>().enabled = true;
            menuManager.previewText.GetComponent<TMP_Text>().text = gameObject.GetComponent<G_Object>().objectDescription;
            menuManager.previewPathText.text = gameObject.GetComponent<G_Object>().objectPath;
            menuManager.previewPathText.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (googleLoader.menuOpen)
        {
            menuManager.previewImage.gameObject.SetActive(false);
            menuManager.previewText.gameObject.GetComponent<TMP_Text>().enabled = false;
            menuManager.previewPathText.gameObject.SetActive(false);
        }
        
    }
}

