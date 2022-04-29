using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Tile> tiles;
    private List<Tile> ghostTiles;

    private int[] tilePositions;
    private int voidPosition;

    private Tile lockedTile;
    private Vector2 initialLockPosition;

    private bool isGameGoing = false;
    private bool isMoveOnCooldown = false;

    private int moves = 0;

    private int timeLeft;

    [SerializeField]
    private GameObject tileObject;

    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private TextMeshProUGUI movesText;
    [SerializeField]
    private GameObject victory;
    [SerializeField]
    private GameObject defeat;
    [SerializeField]
    private GameObject button;

    [SerializeField]
    private int initialTime = 180;

    [SerializeField]
    private float movementThreshhold = 50f;

    [SerializeField]
    private float smoothnessOnSwipe = 70f;

    [SerializeField]
    private int numberOfMixes = 50;

    //On pourra sérialiser ça plus tard, et faire un découpage dynamique de la sprite originale pour éventuellement faire des taquins à taille variable.
    [HideInInspector]
    public int arrayXSize = 3;
    [HideInInspector]
    public int arrayYSize = 3;

    private float tileSize; //On pars du principe qu'une tile est carrée, pour avoir moins de calculs à faire

    void Start()
    {
        tileSize = tileObject.GetComponent<SpriteRenderer>().bounds.size.x;

        //On crée le tableau logique des positions
        tilePositions = new int[arrayXSize * arrayYSize];

        tiles = new List<Tile>();
        ghostTiles = new List<Tile>();
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

        //On crée les ghost tiles (l'image complète)
        for (int i = 0; i < arrayYSize; i++)
        {
            for (int j = 0; j < arrayXSize; j++)
            {
                GameObject tile = Instantiate(tileObject);
                Tile tileComp = tile.GetComponent<Tile>();
                tileComp.number = j + i * arrayXSize;

                ghostTiles.Add(tileComp);
            }
        }

        //On fait le mélange
        for (int i = 0; i < numberOfMixes; i++) MoveTile((SwipeDirection)Random.Range(0, 4), true);

        UpdateTilesPositions();
        UpdateGhostTilesPositions();

        //On lance le timer
        timeLeft = initialTime;
        UpdateTimer();
        isGameGoing = true;
        StartCoroutine(TimerTick());
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
        for(int i = 0; i<tilePositions.Length; i++)
        {
            if (i == voidPosition) continue;

            if (i != tilePositions[i]) return false;
        }

        return true;
    }
    #endregion

#region Logic
    private void MoveTile(SwipeDirection direction, bool isShuffle)
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

        
        if (!isShuffle)
        {
            UpdateTilesPositions();
            UpdateGhostTilesPositions();
            moves++;
            UpdateMoves();

            if (CheckVictory())
            {
                ShowVictoryScreen();
            }
        }
    }

    public Tile GetTileMovedFromDirection(SwipeDirection direction)
    {
        switch (direction)
        {
            case SwipeDirection.Up:
                return tiles.Find(t => t.number == tilePositions[voidPosition + arrayXSize]);
            case SwipeDirection.Down:
                return tiles.Find(t => t.number == tilePositions[voidPosition - arrayXSize]);
            case SwipeDirection.Left:
                return tiles.Find(t => t.number == tilePositions[voidPosition + 1]);
            case SwipeDirection.Right:
                return tiles.Find(t => t.number == tilePositions[voidPosition - 1]);
        }

        return null;
    }

    //Ces fonctions pourront être utiles plus tard si on veux faire des formes autres que des carrés. Le +.1f est pour faire un léger écart
    public float GetVerticalDistance()
    {
        return tileSize + .1f;
    }

    public float GetHorizontalDistance()
    {
        return tileSize + .1f;
    }

    private float HorizontalMovementDistance(SwipeData data)
    {
        return data.startPos.x - data.endPos.x;
    }

    private float VerticalMovementDistance(SwipeData data)
    {
        return data.startPos.y - data.endPos.y;
    }

    private void ShowLoseScreen()
    {
        isGameGoing = false;

        defeat.SetActive(true);
        button.SetActive(true);
    }

    private void ShowVictoryScreen()
    {
        isGameGoing = false;
        victory.SetActive(true);
        button.SetActive(true);

        //On enregistre le score
        if((!PlayerPrefs.HasKey("BestScore")) || PlayerPrefs.GetInt("BestScore") > moves)
            PlayerPrefs.SetInt("BestScore", moves);
        if ((!PlayerPrefs.HasKey("BestTime")) || PlayerPrefs.GetInt("BestTime") > initialTime - timeLeft)
            PlayerPrefs.SetInt("BestTime", initialTime - timeLeft);

        StopAllCoroutines();
    }

    #endregion

#region Updates

    private void UpdateTilesPositions()
    {
        //Position initiale à gauche et en bas
        var initialX = -(Mathf.Floor(arrayXSize / 2) * GetHorizontalDistance() + arrayXSize % 2 == 0 ? GetHorizontalDistance() / 2 : 0);
        var initialY = -(Mathf.Floor(arrayYSize / 2) * GetVerticalDistance() + arrayYSize % 2 == 0 ? GetVerticalDistance() / 2 : 0);

        //Positionnement des tiles
        for (int i=0;i<tilePositions.Length;i++)
        {
            //On ignore la case vide
            if (tilePositions[i] == -1) continue;

            Tile tile = tiles.Find(t => t.number == tilePositions[i]);

            tile.transform.position = new Vector2(
                initialX + GetHorizontalDistance() * (i % arrayXSize - 1),
                initialY + GetVerticalDistance() * (arrayYSize - Mathf.Ceil(i / arrayYSize))
            );
        }
    }

    //Les ghost tile sont juste la formation de l'image normale et complète, on les décale en X exprès pour l'affichage
    private void UpdateGhostTilesPositions()
    {
        //Position initiale à gauche et en bas
        var initialX = -(Mathf.Floor(arrayXSize / 2) * tileSize + arrayXSize % 2 == 0 ? tileSize / 2 : 0) + 5f;
        var initialY = -(Mathf.Floor(arrayYSize / 2) * tileSize + arrayYSize % 2 == 0 ? tileSize / 2 : 0);

        //Positionnement des tiles
        for (int i = 0; i < tilePositions.Length; i++)
        {
            Tile tile = ghostTiles.Find(t => t.number == tilePositions[i]);
            if (tile == null) tile = ghostTiles.Find(t => t.number == arrayXSize * arrayYSize / 2); //Le void doit forcément être rempli ainsi

            tile.transform.position = new Vector2(
                initialX + tileSize * (i % arrayXSize - 1),
                initialY + tileSize * (arrayYSize - Mathf.Ceil(i / arrayYSize))
            );
        }
    }

    private void UpdateTimer()
    {
        string secText = timeLeft % 60 > 9 ? "" + timeLeft % 60 : "0" + timeLeft % 60;
        timerText.text = $"{Mathf.Floor(timeLeft / 60)}:{secText}";
    }

    private void UpdateMoves()
    {
        movesText.text = $"Moves : {moves}";
    }

    public void OnMovementDetected(SwipeData data)
    {
        if (!CheckMoveIsPossible(data.direction)) return;

        if (lockedTile == null)
        {
            lockedTile = GetTileMovedFromDirection(data.direction);
            initialLockPosition = lockedTile.transform.position;
        }

        //On bouge la tile d'un pourcentage du threshhold, pour donner une impression de déplacement smooth
        float thresholdPercentage =
            (data.direction == SwipeDirection.Left || data.direction == SwipeDirection.Right ? HorizontalMovementDistance(data) : VerticalMovementDistance(data)) / movementThreshhold;

        if (thresholdPercentage < 0)
        {
            if (data.direction == SwipeDirection.Left || data.direction == SwipeDirection.Down) thresholdPercentage = -thresholdPercentage;
            else thresholdPercentage = 0;
        }
        if (thresholdPercentage > 1) thresholdPercentage = 1;


        //On calcule la position finale pour mettre un smooth
        Vector2 endPos = Vector2.zero;
        switch (data.direction)
        {
            case SwipeDirection.Up:
                endPos = initialLockPosition + new Vector2(0, GetVerticalDistance() * thresholdPercentage);
                break;
            case SwipeDirection.Down:
                endPos = initialLockPosition - new Vector2(0, GetVerticalDistance() * thresholdPercentage);
                break;
            case SwipeDirection.Left:
                endPos = initialLockPosition - new Vector2(GetHorizontalDistance() * thresholdPercentage, 0);
                break;
            case SwipeDirection.Right:
                endPos = initialLockPosition + new Vector2(GetHorizontalDistance() * thresholdPercentage, 0);
                break;
        }

        lockedTile.transform.position = new Vector2(
            Mathf.Lerp(lockedTile.transform.position.x, endPos.x, 1 / smoothnessOnSwipe),
            Mathf.Lerp(lockedTile.transform.position.y, endPos.y, 1 / smoothnessOnSwipe)
        );

    }

    public void OnSwipeDetected(SwipeData data)
    {
        MoveTile(data.direction, false);
        StartCoroutine(MicroCDForMoves());

        lockedTile = null;
    }

    #endregion


#region Coroutines
    private IEnumerator TimerTick()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);

            timeLeft--;
            UpdateTimer();

            if(timeLeft <= 0)
            {
                ShowLoseScreen();

                yield break; //fin coroutine
            }
        }
        
    }

    //Pour éviter des double-move sur mobile
    private IEnumerator MicroCDForMoves()
    {
        isMoveOnCooldown = true;

        yield return new WaitForSecondsRealtime(.2f);

        isMoveOnCooldown = false;

        yield return null;
    }

#endregion


}


