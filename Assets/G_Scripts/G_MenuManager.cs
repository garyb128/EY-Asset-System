using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Manages project dropdown
public class G_MenuManager : MonoBehaviour
{
    public G_GoogleLoader googleLoader;
    public Image previewImage;
    public TMP_Text previewText;
    public TMP_Text previewPathText;

    void Start()
    {
        googleLoader = FindObjectOfType<G_GoogleLoader>();
    }

    //Sorts depending on project manager picked
    public void DropdownValueChanged(TMP_Dropdown change)
    {
        //Gets selected dropdown value
        string selectedDrop;
        selectedDrop = change.options[change.value].text;

        //Ensures dropdown box behaves properly
        change.RefreshShownValue();
        change.Hide();

        //Checks for buttons and puts them into a list
        var buttons = googleLoader.contentContainer.GetComponentsInChildren(typeof(Button), true);

        //Checks if project name of button is equal to dropdown value and if it is, it sets it to active
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].GetComponent<G_Object>().projectName == selectedDrop)
            {
                buttons[i].gameObject.SetActive(true);
            }
            
            else if (buttons[i].GetComponent<G_Object>().projectName != selectedDrop)
                buttons[i].gameObject.SetActive(false);
        }
    }

    //Handles opening and closing of asset menu
    public void OpenClose(Button current)
    {
        //Disables preview 
        previewImage.gameObject.SetActive(false);
        previewText.gameObject.GetComponent<TMP_Text>().enabled = false;
        previewPathText.gameObject.SetActive(false);
           
        if (current.gameObject.name == "CloseAssetButton")
        {
            googleLoader.assetmenu.SetActive(false);
            googleLoader.menuOpen = false;
        }

        else if (current.gameObject.name == "OpenAssetButton")
        {
            googleLoader.assetmenu.SetActive(true);
            googleLoader.menuOpen = true;
        }
    }

    //Exits program
    public void Exit()
    {
        Application.Quit();
    }

}

