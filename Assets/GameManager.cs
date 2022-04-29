using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private List<Tile> tiles;

    [SerializeField]
    private GameObject tileObject;

    //On pourra sérialiser ça plus tard, et faire un découpage dynamique de la sprite originale pour éventuellement faire des taquins à taille variable.
    private int arrayXSize = 3;
    private int arrayYSize = 3;

    private float tileSize = 1f; //On pars du principe qu'une tile est carrée, pour avoir moins de calculs à faire


    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Start()
    {
        tiles = new List<Tile>();
        var center = arrayXSize * arrayYSize / 2;

        //On crée la pool de tiles
        for (int i=0; i<arrayYSize; i++) {
            for(int j=0; j<arrayXSize; j++) {

                //On skip la case centrale
                if (j + i * arrayXSize == center) continue;

                GameObject tile = Instantiate(tileObject);
                Tile tileComp = tile.GetComponent<Tile>();
                tileComp.number = j + i * arrayXSize;

                //On leur affecte leur image

                tiles.Add(tileComp);
            }
        }

        //Position initiale à gauche et en haut
        var initialX = -(Mathf.Floor(arrayXSize / 2) * tileSize + arrayXSize % 2 == 0 ? tileSize / 2 : 0);
        var initialY = -(Mathf.Floor(arrayYSize / 2) * tileSize + arrayYSize % 2 == 0 ? tileSize / 2 : 0);

        //Positionnement des tiles
        foreach(Tile tile in tiles)
        {
            tile.transform.position = new Vector2(
                initialX + tileSize * (tile.number % arrayXSize - 1),
                initialY + tileSize * (arrayYSize - Mathf.Ceil(tile.number / arrayYSize))
            );
        }

    }
}
