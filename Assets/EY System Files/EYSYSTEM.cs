using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EYSYSTEM : MonoBehaviour
{
    G_GoogleLoader googleLoader;
    public float velocidade = 40;
    public float speed = 100;
    public GameObject Rotator;
    public bool hasRotated,onButton;

    public Camera cam;
    public float zoomAmount = 0;
    public float maxToClamp = 10;
    public float rotSpeed = 10;

    public Material wireframe;
    public List<Material> currentMaterials;
    public List<MeshRenderer> renderers;
    public Canvas allMenu;
    private bool isWireframe,menuHidden;

    // Start is called before the first frame update
    void Start()
    {
        googleLoader = FindObjectOfType<G_GoogleLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if it has been rotated and if it has stop it from spinning then only the user can rotate it
        if (!hasRotated)
        {
            Rotator.transform.Rotate(Vector3.up * velocidade * Time.deltaTime);

            //Rotate left
            if (Input.GetKey(KeyCode.A) && !googleLoader.menuOpen)
            {
                Rotator.transform.Rotate(Vector3.up * velocidade * Time.deltaTime / 1f);
                hasRotated = true;
                Debug.Log("Stop rotation");
            }

            //Rotate right
            if (Input.GetKey(KeyCode.D) && !googleLoader.menuOpen)
            {
                Rotator.transform.Rotate(-Vector3.up * velocidade * Time.deltaTime * 2.5f);
                hasRotated = true;
                Debug.Log("Stop rotation");
            }

        }

        //User can rotate the object freely if it has been rotated
        if (hasRotated)
        {
            if (Input.GetKey(KeyCode.A) && !googleLoader.menuOpen && hasRotated)
                Rotator.transform.Rotate(Vector3.up * velocidade * Time.deltaTime * 2.5f);
            if (Input.GetKey(KeyCode.D) && !googleLoader.menuOpen && hasRotated)
                Rotator.transform.Rotate(-Vector3.up * velocidade * Time.deltaTime * 2.5f);
        }


        //If menu isn't open allow user to freely rotate object
        if (Input.GetMouseButton(0) && !googleLoader.menuOpen && !onButton)
        {
            hasRotated = true;
            googleLoader.currentObject.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speed, Space.World);
        }

        //Allows object to rotate whenever menu is open
        if (googleLoader.menuOpen)
            hasRotated = false;

        //Resets everything whenever R is pushed
        if (Input.GetKey(KeyCode.R))
        {
            hasRotated = false;
            ResetMaterials();
            isWireframe = false;
            if (menuHidden)
                allMenu.gameObject.SetActive(true);
            zoomAmount = 0;
        }

        //Allows user to zoom into object using the scroll wheel
        zoomAmount += Input.GetAxis("Mouse ScrollWheel");
        zoomAmount = Mathf.Clamp(zoomAmount, -maxToClamp, maxToClamp);
        var translate = Mathf.Min(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")), maxToClamp - MathF.Abs(zoomAmount));
        cam.transform.Translate(0, 0, translate * rotSpeed * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")));

        //Pan camera with middle mouse button
        if (Input.GetMouseButton(2))
            cam.transform.Translate(new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0) * Time.deltaTime * 2f);

        //Hides UI if H is pressed
        if (Input.GetKeyDown(KeyCode.H))
        {
            menuHidden = !menuHidden;
            
            if (menuHidden) 
                allMenu.gameObject.SetActive(false);
                
            else if (!menuHidden)
                allMenu.gameObject.SetActive(true);
                
        }


        //Enables wireframe mode whenever W is pressed
        if (Input.GetKeyDown(KeyCode.W))
        {
            isWireframe = !isWireframe;
            Wireframe();
        }
    }

    //Changes current object to wireframe when W is pushed
    void Wireframe()
    {
        //Grabs MeshRenderers of current objects
        renderers = googleLoader.currentObject.GetComponentsInChildren<MeshRenderer>().ToList();

        //Checks if current materials is empty and puts them into a list
        if(currentMaterials.Count == 0)
        {
            foreach(MeshRenderer mr in renderers)
            {
                currentMaterials.Add(mr.GetComponent<MeshRenderer>().material);
            }
        }

        //Sets all materials to wireframe
        if (isWireframe)
        {
            foreach(MeshRenderer mr in renderers)
            {
                mr.GetComponent<MeshRenderer>().material = wireframe;
            }
        }

        else if(!isWireframe)
           ResetMaterials();
    }

    //Resets materials on current object
    void ResetMaterials()
    {
        for (int i = 0; i < currentMaterials.Count; i++)
        {
            renderers[i].material = currentMaterials[i];
        }
        currentMaterials.Clear();
        renderers.Clear();
    }

}
