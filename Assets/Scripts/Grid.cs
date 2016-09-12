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

    //the piece that we press down on,
    private GamePiece pressedPiece;

    //the last piece that our mouse entered the bounding box of.
    private GamePiece enteredPiece;

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
        //Destroy(pieces[4, 4].gameObject);
        //SpawnNewPiece(4, 4, PieceType.OBSTACLE);


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

    //verifies if two pieces are adjacent - This returns true if piece one and piece two have the same x-coordinate
    //and the y-coordinates are within one space of each other and the same if  if the y-coordinates are equal, 
    //and the x-coordinates are within one space of each other 
    public bool isAdjacent(GamePiece piece1, GamePiece piece2) {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) 
            || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    //Method to swap the the pieces if they're adjacent
    public void SwapPieces(GamePiece piece1, GamePiece piece2) {
        //Swap the pieces if they're both movable
        if (piece1.isMovable() && piece2.isMovable()) {

            // If they are, then we assign them to each other's positions in our pieces array.
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            //just swap if the piece get a match
            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null)
            {
                //stores the x- and y-coordinates of piece1 in temporary variables to don't get overridden when we move the piece
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                //So I move the piece1 to piece2's position, and piece2 to piece1's position that we stored earlier.
                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);
            }
            //else  swap the pieces back to their original positions in the array
            else {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }

    //Method for when we press on a piece
    public void PressPiece(GamePiece piece) {
        pressedPiece = piece;
    }

    //Method for when we enter a piece.
    public void EnterPiece(GamePiece piece) {
        enteredPiece = piece;
    }

    public void ReleasePiece() {
        //Check if the pressed piece and the piece we were hovering over 
        //are adjacent to one another.
        if (isAdjacent(pressedPiece, enteredPiece)) {
            //if they are, we swap them
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY) {
        if (piece.hasCaracther()) {
            CharacterPiece.CharacterType characther = piece.CharacterComponent.Character;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //First check horizontal
            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++) {
                //xOffset =  xOffset is how far away the adjacent piece is from our central piece.
                for (int xOffset = 1; xOffset < xDim; xOffset++) {
                    int x;

                    if (dir == 0){//Left
                        x = newX - xOffset;
                    }
                    else {//Right
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= xDim) {
                        break;
                    }

                    //check if the adjacent piece is a match. 
                    if (pieces[x, newY].hasCaracther() && pieces[x, newY].CharacterComponent.Character == characther) {
                        horizontalPieces.Add(pieces[x, newY]);
                    } else {
                        break;
                    }

                }
            }

            if (horizontalPieces.Count >= 3) {
                for (int i = 0; i < horizontalPieces.Count; i++) {
                    matchingPieces.Add(horizontalPieces[i]);
                }                   
            }


            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++) {
                    //Traverse vertically if a match is found
                    for (int dir = 0; dir <= 1; dir++) {
                        for (int yOffset = 1; yOffset < yDim; yOffset++) {
                            int y;

                            if (dir == 0)
                            { //Up
                                y = newY - yOffset;
                            }
                            else
                            { //Down 
                                y = newY + yOffset;
                            }

                            //If the coordinate of the adjacent piece is outside of our grids dimensions,
                            //break out of the loop.
                            if (y < 0 || y >= yDim)
                            {
                                break;
                            }

                            //If the piece matches, we add it to the vertical pieces list. 
                            //If the piece doesn't match, we break out of the loop.
                            if (pieces[horizontalPieces[i].X, y].hasCaracther()
                                && pieces[horizontalPieces[i].X, y].CharacterComponent.Character == characther) {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else{
                                break;
                            }
                        }
                    }

                    //check if have enough vertical pieces to form a match
                    // we need two vertical pieces to form a match.
                    if (verticalPieces.Count < 2)  {
                        //If we don't have enough vertical pieces for a match, we clear the vertical pieces list 
                        //so that we can get ready to iterate along the next horizontal matching piece.
                        verticalPieces.Clear();
                    } else {
                        for (int j = 0; j < verticalPieces.Count; j++) {
                            matchingPieces.Add(verticalPieces[j]);
                        }

                        break;
                    }
                }
         }

        if (matchingPieces.Count >= 3) {
                return matchingPieces;
        }


        //Didn't find anything going horizontally first,
        //so now check vertically
        horizontalPieces.Clear();
        verticalPieces.Clear();
        verticalPieces.Add(piece);

        for (int dir = 0; dir <= 1; dir++) {
            for (int yOffset = 1; yOffset < yDim; yOffset++)
            {
                int y;

                if (dir == 0)
                {//Up
                    y = newY - yOffset;
                }
                else
                {//Down
                    y = newY + yOffset;
                }

                if (y < 0 || y >= yDim)
                {
                    break;
                }

                //check if the adjacent piece is a match. 
                if (pieces[newX, y].hasCaracther()
                        && pieces[newX, y].CharacterComponent.Character == characther) {
                    verticalPieces.Add(pieces[newX, y]);
                } else {
                    break;
                }

            }
        }

        if (verticalPieces.Count >= 3) {
            for (int i = 0; i < verticalPieces.Count; i++)
            {
                matchingPieces.Add(verticalPieces[i]);
            }
        }

        if (verticalPieces.Count >= 3) {
            for (int i = 0; i < verticalPieces.Count; i++) {
                //Traverse vertically if a match is found
                for (int dir = 0; dir <= 1; dir++) {
                    for (int xOffset = 1; xOffset < xDim; xOffset++) {
                        int x;

                        if (dir == 0)
                        { //Left
                            x = newX - xOffset;
                        }
                        else
                        { //Right 
                            x = newX + xOffset;
                        }

                        //If the coordinate of the adjacent piece is outside of our grids dimensions,
                        //break out of the loop.
                        if (x < 0 || x >= xDim)
                        {
                            break;
                        }

                        //If the piece matches, we add it to the vertical pieces list. 
                        //If the piece doesn't match, we break out of the loop.
                        if (pieces[x, verticalPieces[i].Y].hasCaracther()
                            && pieces[x, verticalPieces[i].Y].CharacterComponent.Character == characther) {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                        } else {
                            break;
                        }
                    }
                }

                //check if have enough horizontal pieces to form a match
                //need just two horizontal pieces to form a match.
                if (horizontalPieces.Count < 2) {
                    //If we don't have enough horizontal pieces for a match, we need to clear the horizontal pieces list 
                    //so that we can get ready to iterate along the next horizontal matching piece.
                    horizontalPieces.Clear();
                } else {
                    for (int j = 0; j < horizontalPieces.Count; j++) {
                        matchingPieces.Add(horizontalPieces[j]);
                    }

                    break;
                }
            }
        }

        if (matchingPieces.Count >= 3) {
            return matchingPieces;

        }
    }

        //if no match is found
        return null;
    }

    void Update () {
	
	}
}
