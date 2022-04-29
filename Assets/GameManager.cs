using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private List<Tile> tiles;

    private int[] tilePositions;
    private int voidPosition;

    [SerializeField]
    private GameObject tileObject;

    //On pourra sérialiser ça plus tard, et faire un découpage dynamique de la sprite originale pour éventuellement faire des taquins à taille variable.
    [HideInInspector]
    public int arrayXSize = 3;
    [HideInInspector]
    public int arrayYSize = 3;

    private float tileSize = 1f; //On pars du principe qu'une tile est carrée, pour avoir moins de calculs à faire


    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Start()
    {
        //On crée le tableau logique des positions
        tilePositions = new int[arrayXSize * arrayYSize];

        tiles = new List<Tile>();
        var center = arrayXSize * arrayYSize / 2;

        tilePositions[center] = -1; //On met la position centrale logique à -1
        voidPosition = center;

        //On crée la pool de tiles
        for (int i=0; i<arrayYSize; i++) {
            for(int j=0; j<arrayXSize; j++) {

                //On skip la case centrale
                if (j + i * arrayXSize == center) continue;

                GameObject tile = Instantiate(tileObject);
                Tile tileComp = tile.GetComponent<Tile>();
                tileComp.number = j + i * arrayXSize;

                tiles.Add(tileComp);

                tilePositions[i * arrayXSize + j] = tileComp.number;
            }
        }

        UpdateTilesPositions();

        

    }

    private void MoveTile(SwipeDirection direction, bool checkVictory)
    {
        //On se fiche de quelle tile veut être bougée, il n'y a qu'une possibilité par direction de toute manière.

        if (!CheckMoveIsPossible(direction)) return;

        int tileNumber;
        switch (direction)
        {
            //On déplace la case logique vers le haut
            case SwipeDirection.Up:
                tileNumber = tilePositions[voidPosition + arrayXSize];
                tilePositions[voidPosition] = tileNumber;
                tilePositions[voidPosition + arrayXSize] = -1;

                voidPosition += arrayXSize;
                break;

            //On déplace la case logique vers le bas
            case SwipeDirection.Down:
                tileNumber = tilePositions[voidPosition - arrayXSize];
                tilePositions[voidPosition] = tileNumber;
                tilePositions[voidPosition - arrayXSize] = -1;

                voidPosition -= arrayXSize;
                break;

            case SwipeDirection.Left:
                tileNumber = tilePositions[voidPosition + 1];
                tilePositions[voidPosition] = tileNumber;
                tilePositions[voidPosition + 1] = -1;

                voidPosition++;
                break;

            case SwipeDirection.Right:
                tileNumber = tilePositions[voidPosition - 1];
                tilePositions[voidPosition] = tileNumber;
                tilePositions[voidPosition - 1] = -1;

                voidPosition--;
                break;
        }

        UpdateTilesPositions();
    }

#region Checks
    private bool CheckMoveIsPossible(SwipeDirection direction)
    {
        switch (direction)
        {
            //Que si la case vide est pas en bas
            case SwipeDirection.Up:
                if (voidPosition >= arrayXSize * (arrayYSize - 1)) return false;
                else return true;

            //Que si la case vide est pas en haut
            case SwipeDirection.Down:
                if (voidPosition < arrayXSize) return false;
                else return true;

            //Que si la case vide est pas à droite
            case SwipeDirection.Left:
                if (voidPosition % arrayXSize == arrayXSize - 1) return false;
                else return true;

            //Que si la case vide est pas à gauche
            case SwipeDirection.Right:
                if (voidPosition % arrayXSize == 0) return false;
                else return true;

            default:
                return false;
        }
    }

    private bool CheckVictory()
    {
        return false;
    }
#endregion

    private void UpdateTilesPositions()
    {
        //Position initiale à gauche et en bas
        var initialX = -(Mathf.Floor(arrayXSize / 2) * tileSize + arrayXSize % 2 == 0 ? tileSize / 2 : 0);
        var initialY = -(Mathf.Floor(arrayYSize / 2) * tileSize + arrayYSize % 2 == 0 ? tileSize / 2 : 0);

        //Positionnement des tiles
        for(int i=0;i<tilePositions.Length;i++)
        {
            //On ignore la case vide
            if (tilePositions[i] == -1) continue;

            Tile tile = tiles.Find(t => t.number == tilePositions[i]);

            tile.transform.position = new Vector2(
                initialX + tileSize * (i % arrayXSize - 1),
                initialY + tileSize * (arrayYSize - Mathf.Ceil(i / arrayYSize))
            );
        }
    }

    public void OnMovementDetected(SwipeData data)
    {
        
    }
}
