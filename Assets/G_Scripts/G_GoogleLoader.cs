using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class G_GoogleLoader : MonoBehaviour
{
    private SheetReader sheetReader;
    public List<GameObject> objects;
    public GameObject objectPrefab, currentObject, assetmenu;
    public GameObject sysVersionTxt, objNumTxt, projectNameTxt, objNameTxt, objDescTxt, objPathTxt;
    //public GameObject posXTxt, posYTxt, posZTxt, rotXTxt, rotYTxt, rotZTxt;

    public List<string> versionNumbersList, objectNumbersList, projectNameList, objNameList, objDescList;
    public List<GameObject> Items;
    public Transform Rotator, contentContainer, groundPlane;
    public TMP_Dropdown projectNameDrop;
    public bool menuOpen;

    private void Start()
    {
        //Grabs and sets up the sheet reader
        objects = new List<GameObject>();
        sheetReader = new SheetReader();

        //Downloads the data from the google sheet
        GoogleDisplay();
        FillDrop();

        //Ensures the asset menu is hidden as it starts after being filled
        assetmenu.SetActive(false);

    }

    //Grabs information from Google Sheets
    public void GoogleDisplay()
    {
        //Looks through each column and stores in a generic variable
        var sysVersion = sheetReader.getSheetRange("Sheet1!A2:A1000");
        var objNum = sheetReader.getSheetRange("Sheet1!B2:B1000");
        var projectName = sheetReader.getSheetRange("Sheet1!C2:C1000");
        var objName = sheetReader.getSheetRange("Sheet1!D2:D1000");
        var objDesc = sheetReader.getSheetRange("Sheet1!E2:E1000");


        //Stores info from google sheet into a list
        foreach (var cell in sysVersion.SelectMany(row => row))
        {
            versionNumbersList.Add(cell.ToString());
        }

        foreach (var cell in objNum.SelectMany(row => row))
        {
            objectNumbersList.Add(cell.ToString());
        }

        foreach (var cell in projectName.SelectMany(row => row))
        {
            projectNameList.Add(cell.ToString());
        }

        foreach (var cell in objName.SelectMany(row => row))
        {
            objNameList.Add(cell.ToString());
        }

        foreach (var cell in objDesc.SelectMany(row => row))
        {
            objDescList.Add(cell.ToString());
        }
    }

    //Fills in object menu with objects based on data taken from the google sheet
    public void FillDrop()
    {
        //Ensures duplicates of project name is removed before being added to dropdown
        var sansDupe = projectNameList.Distinct().ToList();

        //Fills the dropdowns with the data from the Google Sheet
        projectNameDrop.ClearOptions();
        projectNameDrop.AddOptions(sansDupe);
        projectNameDrop.RefreshShownValue();

        for (int i = 0; i < Items.Count; i++)
        {
            var obj_go = Instantiate(objectPrefab, contentContainer);
            objects.Add(obj_go);
            obj_go.GetComponent<G_Object>().id = i;
            obj_go.GetComponent<G_Object>().objectNumber = objectNumbersList[i];
            obj_go.GetComponent<G_Object>().projectName = projectNameList[i];
            obj_go.GetComponent<G_Object>().objectName = objNameList[i];
            obj_go.GetComponent<G_Object>().objectDescription = objDescList[i];
            obj_go.GetComponent<G_Object>().objectPath = AssetDatabase.GetAssetPath(Items[i]);
            obj_go.GetComponent<G_Object>()._gameObject = Items[i];
            obj_go.GetComponentInChildren<TextMeshProUGUI>().text = obj_go.GetComponent<G_Object>().objectName;

            //Enables first selected gamepbject and disables the others
            if (i > 0)
            {
                obj_go.SetActive(false);
            }
        }
    }

    //Makes Object grounded by using raycasts
    public void MakeGrounded(GameObject gameObject)
    {
        //Ensures spawned object is always grounded. Sends a raycast to check the ground
        RaycastHit hit;
        float distance = 100f;

        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, distance))
        {
            /*
             * Set the target location to the location of the hit.
             */
            Vector3 targetLocation = hit.point;
            /*
             * Modify the target location so that the object is being perfectly aligned with the ground (if it's flat).
             */
            //targetLocation += new Vector3(0, transform.localScale.y / 2, 0);
            /*
             * Move the object to the target location.
             */
            gameObject.transform.position = targetLocation;
        }
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

    private void Update()
    {
        //If user pushes R the resets camera
        if (Input.GetKeyDown(KeyCode.R) && !menuOpen)
        {
            FindObjectOfType<Camera>().transform.localEulerAngles = Vector3.zero;
            currentObject.transform.localEulerAngles = Vector3.zero;
            GetBound(currentObject);
            ZoomFit(FindObjectOfType<Camera>(), currentObject, false);
            MakeGrounded(currentObject);
        }
    }


}

