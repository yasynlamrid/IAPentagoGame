using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PlacingPiece,
        RotatingQuadrant
    }
    public GameState currentState = GameState.PlacingPiece;

    bool gameIsOver = false;
    public float spacing = 1.0f;
    public float cellSize = 1.0f;
    public float a = -3.3f;
    public float b = 4.5f;
    public static GameManager instance;
    int currentPlayer = 1;
    public GameObject blackCoin;
    public GameObject whiteCoin;
    public GameObject quadrant1Object;
    public GameObject quadrant2Object;
    public GameObject quadrant3Object;
    public GameObject quadrant4Object;




    void Awake()
    {
        instance = this;
    }


    public void ButtonPressed(int startRow, int startCol, bool clockwise)
    {
        if (currentState != GameState.RotatingQuadrant || gameIsOver)
            return;
        Playfield.instance.RotateQuadrant(startRow, startCol, clockwise);
        GameObject quadrantObject = DetermineQuadrantObjectByStartCoords(startRow, startCol);
        StartCoroutine(RotateQuadrantAnimation(quadrantObject, clockwise ? -90 : 90, 1.0f, () => {
            UpdateElementPositionsAfterRotation(startRow, startCol, clockwise, quadrantObject);
            SwitchPlayer();
        }));

       
    }
    

  

    public void ElementPressed(int row, int column)
    {
        if (currentState != GameState.PlacingPiece || gameIsOver)
            return;

        bool isValidMove = Playfield.instance.ValidateMove(row, column);
        
        if (isValidMove == true)
        {

            Playfield.instance.PlaceCoin(row, column, currentPlayer);
            StartCoroutine(PlayCoin(row, column));

        }
        currentState = GameState.RotatingQuadrant;

    }

    IEnumerator PlayCoin(int row, int column)
    {


        
        GameObject coin = Instantiate((currentPlayer == 1 ? blackCoin : whiteCoin)) as GameObject;
        Vector3 startPosition = new Vector3((column * (cellSize + spacing)) + -a * cellSize, 10, 0);
        coin.transform.position = startPosition;
        Vector3 finalPosition = new Vector3((column * (cellSize + spacing)) + a * cellSize, (6 - row) * (cellSize + spacing) - b * cellSize, 0);
      // les deux lignes suivantes permet de placer les pions dans chaque quadrant 
        GameObject quadrantObject = DetermineQuadrantObject(row, column);
        coin.transform.parent = quadrantObject.transform;
        while (coin.transform.position != finalPosition)
        {
            coin.transform.position = Vector3.MoveTowards(coin.transform.position, finalPosition, 2f);
            yield return null;
        }

      
      //  SwitchPlayer();
    }




    IEnumerator RotateQuadrantAnimation(GameObject quadrant, float angle, float duration, Action onComplete)
    {
        Quaternion initialRotation = quadrant.transform.rotation;
        Quaternion finalRotation = quadrant.transform.rotation * Quaternion.Euler(0, 0, angle);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            quadrant.transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        quadrant.transform.rotation = finalRotation;
        onComplete?.Invoke(); // Exécute le callback après la fin de l'animation
    }




    void SwitchPlayer()
    {
        // Changez de joueur.
        currentPlayer = (currentPlayer == 1 ? 2 : 1);

        // Réinitialisez l'état pour le nouveau tour du joueur.
        currentState = GameState.PlacingPiece;

        // Si le joueur actuel est l'IA, déclenchez ses actions.
        if (currentPlayer == 2)
        {
            // Assurez-vous que ces appels respectent l'état du jeu et qu'ils sont séquentiels.
            StartCoroutine(ExecuteAiMoveAndRotation()); // L'IA place une pièce et fait une rotation.
        }
    }
    public void WinCondition(bool winner)
    {

        if(winner)
        {
            Debug.Log("Le jeu est terminé. Un joueur a gagné.");
            gameIsOver = true;

        }
        else
        {
           

        }
    }

    GameObject DetermineQuadrantObject(int row, int column)
    {
        if (row < 3)
        {
            if (column < 3)
            {
                return quadrant1Object;
            }
            else
            {
                return quadrant2Object;
            }
        }
        else
        {
            if (column < 3)
            {
                return quadrant3Object;
            }
            else
            {
                return quadrant4Object;
            }
        }
    }


    GameObject DetermineQuadrantObjectByStartCoords(int startRow, int startCol)
    {
        if (startRow < 3)
        {
            return startCol < 3 ? quadrant1Object : quadrant2Object;
        }
        else
        {
            return startCol < 3 ? quadrant3Object : quadrant4Object;
        }
    }



    public void UpdateElementPositionsAfterRotation(int startRow, int startCol, bool clockwise, GameObject quadrantObject)
    {
        ElementInput[] elements = quadrantObject.GetComponentsInChildren<ElementInput>();
        foreach (var element in elements)
        {
            int newRow, newCol;
            if (clockwise)
            {
                newRow = startRow + (element.column - startCol);
                newCol = startCol + (2 - (element.row - startRow));
            }
            else
            {
                newRow = startRow + (2 - (element.column - startCol));
                newCol = startCol + (element.row - startRow);
            }
           
            element.row = newRow;
            element.column = newCol;
        }
    }



    IEnumerator ExecuteAiMoveAndRotation()
    {
        // Démarrer la coroutine de déplacement de l'IA et attendre qu'elle se termine
        yield return StartCoroutine(AiMove.instance.BestMove());

        // Une fois la coroutine de déplacement terminée, démarrer la rotation
        yield return StartCoroutine(AiMove.instance.BestRotation1());
    }
}
