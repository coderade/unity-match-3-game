using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {


    public enum PieceType
    {
        EMPTY,
        NORMAL,
        OBSTACLE,
        COUNT
    };

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    };

    public int xDim;
    public int yDim;
    public float fillTime;

    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;

    private Dictionary<PieceType, GameObject> piecePrefabDict;

    private GamePiece[,] pieces;

    private bool inverse = false;


    void Start()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            //cheks if the dictionary contains my key - PieceType
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                //if dont contais add a new key value pair to my dictionary
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        //instantiate the backgorundPrefab for each cell in the grid
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            { //Quaternion.identity => corresponds to "no rotation" 
                GameObject background = (GameObject)Instantiate(backgroundPrefab,
                    GetWorldPosition(x, y), Quaternion.identity);

                //make this new background, a child of the grid object
                background.transform.parent = transform;
            }
        }

        pieces = new GamePiece[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                //GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL],
                //    Vector3.zero, Quaternion.identity);

                //newPiece.name = "Piece(" + x + " , " + y + ")";
                //newPiece.transform.parent = transform;

                //pieces[x, y] = newPiece.GetComponent<GamePiece>();
                //pieces[x, y].Init(x, y, this, PieceType.NORMAL);

                //if (pieces[x, y].isMovable()) {
                //    pieces[x, y].MovableComponent.Move(x, y);
                //}

                //if (pieces[x, y].hasCaracther())
                //{
                //    pieces[x, y].CharacterComponent.SetCharacter((CharacterPiece.CharacterType)Random.Range(0, pieces[x, y].CharacterComponent.NumCharacters));
                //}

                SpawnNewPiece(x, y, PieceType.EMPTY);

            }
        }
        Destroy(pieces[4, 4].gameObject);
        SpawnNewPiece(4, 4, PieceType.OBSTACLE);


        //TEST diagonal fill where has obstacles
        //Destroy(pieces[1, 4].gameObject);
        //SpawnNewPiece(1, 4, PieceType.OBSTACLE);

        //Destroy(pieces[2, 4].gameObject);
        //SpawnNewPiece(2, 4, PieceType.OBSTACLE);

        //Destroy(pieces[3, 4].gameObject);
        //SpawnNewPiece(3, 4, PieceType.OBSTACLE);

        //Destroy(pieces[5, 4].gameObject);
        //SpawnNewPiece(5, 4, PieceType.OBSTACLE);

        //Destroy(pieces[6, 4].gameObject);
        //SpawnNewPiece(6, 4, PieceType.OBSTACLE);

        //Destroy(pieces[7, 4].gameObject);
        //SpawnNewPiece(7, 4, PieceType.OBSTACLE);

        //Destroy(pieces[4, 0].gameObject);
        //SpawnNewPiece(4, 0, PieceType.OBSTACLE);


        //use StartCoroutine method to animate the fill
        StartCoroutine(Fill());
    }

    public IEnumerator Fill() {
        //The while loop we'll call the FillStep method and if it's true 
        //it will just keep calling the method until it returns false
        while (FillStep()) {
            inverse = !inverse;
            yield return new WaitForSeconds(fillTime);
        }
    }

    public bool FillStep() {

        bool movedPiece = false;

        //check if the piece under it was empty, and then move the piece down
        for (int y = yDim - 2; y >= 0; y--) {
            for (int loopX = 0; loopX < xDim; loopX++) {

                int x = loopX;
                if (inverse) {
                    x = xDim - 1 - loopX;
                }

                GamePiece piece = pieces[x, y];

                if (piece.isMovable()) {
                    GamePiece pieceBelow = pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.EMPTY) {
                        //The Destoy method we'll simply destroy the empty piece 
                        //below before moving a new piece into it
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;

                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    } else {
                        for (int diag = -1; diag <= 1; diag++) {
                            if (diag != 0) {
                                int diagX = x + diag;

                                if (inverse) {
                                    diagX = x - diag;
                                }

                                if (diagX >=0 && diagX < xDim){
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];

                                    if (diagonalPiece.Type == PieceType.EMPTY) {
                                        bool hasPieceAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--) {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];

                                            if (pieceAbove.isMovable())
                                            {
                                                break;
                                            }
                                            else if (!pieceAbove.isMovable() && pieceAbove.Type != PieceType.EMPTY) {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }

                                        //if the piece doesn't have piece above I need to fill it diagonally
                                        if (!hasPieceAbove) {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < xDim; x++) {
            GamePiece pieceBelow = pieces[x, 0];

            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x,-1), Quaternion.identity);
                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].CharacterComponent.SetCharacter((CharacterPiece.CharacterType)Random.Range(0, pieces[x, 0].CharacterComponent.NumCharacters));
                movedPiece = true;
            }
        }

        return movedPiece;

    }

    public Vector2 GetWorldPosition(int x, int y) {
        return new Vector2(transform.position.x - xDim / 2.0f + x,
            transform.position.y + yDim / 2.0f - y);
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this, type);

        return pieces[x, y];
    }


    void Update () {
	
	}
}
