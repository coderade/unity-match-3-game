using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {


    public enum PieceType
    {
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
               GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL],
                   GetWorldPosition(x, y), Quaternion.identity);

                newPiece.name = "Piece(" + x + " , " + y + ")";
                newPiece.transform.parent = transform;

                pieces[x, y] = newPiece.GetComponent<GamePiece>();
                pieces[x, y].Init(x, y, this, PieceType.NORMAL);
            }
        }
    }

    Vector2 GetWorldPosition(int x, int y) {
        return new Vector2(transform.position.x - xDim / 2.0f + x,
            transform.position.y + yDim / 2.0f - y);
    }

	void Update () {
	
	}
}
