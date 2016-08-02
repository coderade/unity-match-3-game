using UnityEngine;
using System.Collections;

public class MovablePiece : MonoBehaviour
{

    private GamePiece piece;

    void Awake()
    {
        piece = GetComponent<GamePiece>();
    }


    void Start()
    {

    }

    void Update()
    {

    }

    public void Move(int newX, int newY) {

        piece.X = newX;
        piece.Y = newY;

        piece.transform.localPosition = piece.GridRef.GetWorldPosition(newX, newY);

    }
}


