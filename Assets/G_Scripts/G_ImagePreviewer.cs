using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class G_ImagePreviewer : MonoBehaviour,IPointerEnterHandler
{
    private Texture2D preview;
    public Image previewImage;


    //Create preview image whenever button is highlighted
    public void OnPointerEnter(PointerEventData BaeventData)
    {
        //Grab model assigned to button and create preview image
        preview = RuntimePreviewGenerator.GenerateModelPreview(gameObject.GetComponent<G_Object>()._gameObject.transform, 64, 64, false, true);
        previewImage.sprite = Sprite.Create(preview, new Rect(0.0f, 0.0f, preview.width, preview.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

}
