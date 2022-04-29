using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int number;
    private bool isImageOk = false;

    void Update()
    {
        //La tile met son image à jour
        if(isImageOk) return;

        Sprite img = ImageManager.instance.GetImage(number);

        if (img == null) return;

        
        GetComponent<SpriteRenderer>().sprite = img;
        isImageOk = true;
    }
}
