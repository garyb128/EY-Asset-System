using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;

public class G_BackgroundChanger : MonoBehaviour
{
    public List<Sprite> backgrounds;
    public Image background;
    public TMP_Text backgroundName;
    public Light colouredLight;
    private int index;
    private EYSYSTEM eYSYSTEM;

    private void Start()
    {
        eYSYSTEM = FindObjectOfType<EYSYSTEM>();
    }

    public Color32 MainColorFromTexture(Texture2D tex)
    {
        Color32[] texColors = tex.GetPixels32();
        int total = texColors.Length;
        Dictionary<int, int> colors = new Dictionary<int, int>();

        int max = 1;
        Color32 mostCol = texColors[0]; //default to first texel
        mostCol.GetHashCode();
        for (int i = 0; i < total; i++)
        {

            Color32Array c = new Color32Array();
            c.color = texColors[i];

            if (colors.ContainsKey(c.key))
            {
                int count = ++colors[c.key];
                if (count > max)
                {
                    max = count;
                    mostCol = c.color;
                }
            }
            else colors.Add(c.key, 0);
        }

        mostCol.a = 255; // maybe you want this


        //Ensures that colour intensity isn't too high
        if (mostCol.r > 200 && mostCol.g < 200 && mostCol.b < 200)
            mostCol.r = 200;

        if (mostCol.g > 200 && mostCol.b < 200 && mostCol.r < 200)
            mostCol.g = 200;

        if (mostCol.b > 200 && mostCol.g < 200 && mostCol.r < 200)
            mostCol.b = 200;

        //Makes colours more intense
        if (mostCol.r < 50)
            mostCol.r *= 5;
        
        if (mostCol.g < 50)
            mostCol.g *= 5;

        if (mostCol.b < 50)
            mostCol.b *= 5;

        return mostCol;
        
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct Color32Array
    {
        [FieldOffset(0)]
        public int key;

        [FieldOffset(0)]
        public Color32 color;
    }


    //Checks button and scrolls through list
    public void ChangeBackground(Button button)
    {
        //Checks if right is pressed
        if (button.gameObject.name == "BackgroundRight" && index < backgrounds.Count-1)
        {
            index++;
            background.GetComponent<Image>().sprite = backgrounds[index];
            backgroundName.text = backgrounds[index].name;
            Color light = MainColorFromTexture(backgrounds[index].texture);       
            colouredLight.color = light;

        }

        //Checks if right is pressed and loops to beginning of list
        if (button.gameObject.name == "BackgroundRight" && index == backgrounds.Count-1)
        {
            index = 0;
            background.GetComponent<Image>().sprite = backgrounds[index];
            backgroundName.text = backgrounds[index].name;
            Color light = MainColorFromTexture(backgrounds[index].texture);        
            colouredLight.color = light;

        }

        //Checks if left is pressed
        if (button.gameObject.name == "BackgroundLeft" && index > 0)
        {
            index--;
            background.GetComponent<Image>().sprite = backgrounds[index];
            backgroundName.text = backgrounds[index].name;
            Color light = MainColorFromTexture(backgrounds[index].texture);       
            colouredLight.color = light;
            
        }

        //Ensures index loops back to the end of the list
        else if (button.gameObject.name == "BackgroundLeft" && index == 0)
        {
            index = backgrounds.Count - 2;
            background.GetComponent<Image>().sprite = backgrounds[index];
            backgroundName.text = backgrounds[index].name;
            Color light = MainColorFromTexture(backgrounds[index].texture);
            colouredLight.color = light;
        }
    }

}
