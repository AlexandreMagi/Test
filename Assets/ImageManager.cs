using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageManager : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> sprites;
    private GameManager gm;
    public static ImageManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gm = GameManager.instance;
        LoadImages();
    }

    private void LoadImages()
    {
        sprites = new List<Sprite>();

        for (int i = 1; i <= gm.arrayXSize * gm.arrayYSize; i++)
        {
            Sprite img = Resources.Load<Sprite>($"Android/{i}");
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