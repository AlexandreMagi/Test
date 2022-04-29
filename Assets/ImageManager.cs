using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageManager : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> sprites;
    private GameManager gm;
    private string folder;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        LoadImages();
    }

    private void LoadImages()
    {
        sprites = new List<Sprite>();

        if (Application.platform == RuntimePlatform.Android)
            folder = "Android";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            folder = "Apple";
        else
            folder = "Android"; //On met android par défaut, au cas où

        for (int i = 1; i <= gm.arrayXSize * gm.arrayYSize; i++)
        {
            Sprite img = Resources.Load<Sprite>($"{folder}/{i}");
            sprites.Add(img);
        }
    }

    public Sprite GetImage(int number)
    {
        //Les images sont pas toutes chargées, on renvoie rien tant que c'est pas fini
        if (sprites.Count != gm.arrayXSize * gm.arrayYSize) return null;

        return sprites[number];
    }
}