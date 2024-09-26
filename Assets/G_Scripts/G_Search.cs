using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//Manages searching
public class G_Search : MonoBehaviour
{
    private G_GoogleLoader googleLoader;
    public GameObject contentHolder;
    public TMP_InputField searchBar;
    private string searchText;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize the google loader
        googleLoader = FindObjectOfType<G_GoogleLoader>();
    }

    public void Search()
    {
        //Grabs the text from the search bar
        searchText = searchBar.text.ToLower();
        
        Debug.Log("Searching for ... " + searchText);
        
        //Grabs all children of content container
        var allChildren = googleLoader.contentContainer.Cast<Transform>().Select(t => t.gameObject).ToArray();


        //Loops through all of the objects
        foreach (var child in allChildren)
        {
            //Checks if the object is active and if the search text is in the title then enables it otherwise disables it
            if (child.activeInHierarchy)
            {
                if (child.GetComponent<G_Object>().objectName.ToLower().Contains(searchText))
                {
                    child.SetActive(true);
                    Debug.Log("Has found " + searchText);
                }
                else if (!child.GetComponent<G_Object>().objectName.ToLower().Contains(searchText))
                {
                    child.SetActive(false);
                    Debug.Log("Can't find " + searchText);
                }
            }  
            //If the input field is empty enable the buttons that match the project name
            if (!child.activeInHierarchy && String.IsNullOrEmpty(searchText) && child.GetComponent<G_Object>().projectName.ToLower() == googleLoader.projectNameDrop.options[googleLoader.projectNameDrop.value].text.ToLower())
            {
                child.SetActive(true);
                Debug.Log("Resetting...");
            }
        }
    }
}






