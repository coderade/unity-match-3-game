using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {


    public enum PieceType
    {
        EMPTY,
        NORMAL,
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
        //use StartCoroutine method to animate the fill
       StartCoroutine(Fill());
    }

    public IEnumerator Fill() {
        //The while loop we'll call the FillStep method and if it's true 
        //it will just keep calling the method until it returns false
        while (FillStep()) {
            yield return new WaitForSeconds(fillTime);
        }
    }

    public bool FillStep() {

        bool movedPiece = false;

        //check if the piece under it was empty, and then move the piece down
        for (int y = yDim - 2; y >= 0; y--) {
            for (int x = 0; x < xDim; x++) {
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
