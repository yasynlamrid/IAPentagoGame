using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playfield : MonoBehaviour
{
    public static Playfield instance;




    public const int numRows = 6;
    public const int numCols = 6;
    int[,] board = new int[numRows, numCols]
    {
        { 0, 0, 0, 0, 0, 0},
        { 0, 0, 0, 0, 0, 0},
        { 0, 0, 0, 0, 0, 0},
        { 0, 0, 0, 0, 0, 0},
        { 0, 0, 0, 0, 0, 0},
        { 0, 0, 0, 0, 0, 0}
    }; 

    void Awake()
    {
      instance = this;


    }


    public bool ValidateMove(int row, int column)
    {
        // Vérifie si la position est dans les limites et vide.
        if (row >= 0 && row < numRows && column >= 0 && column < numCols && board[row, column] == 0)
        {
            return true;
        }
        else
        {
            Debug.Log($"Mouvement invalide : hors limites ou position déjà occupée.{ row},{column}");
            return false;
        }
    }

    public void PlaceCoin(int x, int y, int player)
    {
            board[x, y] = player;
            print(DebugBoard()); // Affiche le plateau après placement.
            GameManager.instance.WinCondition(WinCheck());
    }



    // Construit une chaîne de caractères qui représente le plateau pour le débogage.
    public string DebugBoard()
    {
        string s = "";
        string separator = ",";
        string border = "|";

        for (int x = 0; x < numRows; x++)
        {
            s += border;
            for (int y = 0; y < numCols; y++)
            {
                s += board[x, y].ToString();
                if (y < numCols - 1) s += separator;
            }
            s += border + "\n";
        }
        return s;
    }


    bool WinCheck()
    {
        if(HorizontalCheck() || VerticalCheck() || DiagonalCheck())
        {
            return true;
        }
        return false;
       
    }

    bool HorizontalCheck()
    {
        for (int row = 0; row < numRows; row++)
        {
            for (int column = 0; column < numCols - 4; column++) 
            {
                if (board[row, column] > 0 &&
                    board[row, column] == board[row, column + 1] &&
                    board[row, column + 1] == board[row, column + 2] &&
                    board[row, column + 2] == board[row, column + 3] &&
                    board[row, column + 3] == board[row, column + 4])
                {
                    Debug.Log($"Le joueur {board[row, column]} a gagné horizontalement sur la ligne {row}.");
                    return true; 
                }
            }
        }
        Debug.Log("Pas encore de gagnant horizontal.");
        return false; 
    }

    bool VerticalCheck()
    {
        for (int column = 0; column < numCols; column++)
        {
            for (int row = 0; row < numRows - 4; row++)
            {
                if (board[row, column] > 0 &&
                    board[row, column] == board[row + 1, column] &&
                    board[row + 1, column] == board[row + 2, column] &&
                    board[row + 2, column] == board[row + 3, column] &&
                    board[row + 3, column] == board[row + 4, column])
                {
                    Debug.Log($"Le joueur {board[row, column]} a gagné verticalement sur la colonne {column}.");
                    return true; 
                }
            }
        }
        Debug.Log("Pas encore de gagnant vertical.");
        return false; 
    }
    bool DiagonalCheck()
    {
        // Vérifie les diagonales principales
        for (int row = 0; row < numRows - 4; row++)
        {
            for (int column = 0; column < numCols - 4; column++)
            {
                if (board[row, column] > 0 &&
                    board[row, column] == board[row + 1, column + 1] &&
                    board[row + 1, column + 1] == board[row + 2, column + 2] &&
                    board[row + 2, column + 2] == board[row + 3, column + 3] &&
                    board[row + 3, column + 3] == board[row + 4, column + 4])
                {
                    Debug.Log($"Le joueur {board[row, column]} a gagné en diagonale principale commençant à la ligne {row}, colonne {column}.");
                    return true;
                }
            }
        }

        // Vérifie les diagonales secondaires
        for (int row = 4; row < numRows; row++)
        {
            for (int column = 0; column < numCols - 4; column++)
            {
                if (board[row, column] > 0 &&
                    board[row, column] == board[row - 1, column + 1] &&
                    board[row - 1, column + 1] == board[row - 2, column + 2] &&
                    board[row - 2, column + 2] == board[row - 3, column + 3] &&
                    board[row - 3, column + 3] == board[row - 4, column + 4])
                {
                    Debug.Log($"Le joueur {board[row, column]} a gagné en diagonale secondaire commençant à la ligne {row}, colonne {column}.");
                    return true;
                }
            }
        }

        Debug.Log("Pas encore de gagnant diagonal.");
        return false;
    }



    public void RotateQuadrant(int startRow, int startCol, bool clockwise)
    {
        int size = 3;
        int[,] tempQuadrant = new int[size, size];
        GameManager.instance.WinCondition(WinCheck());

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                tempQuadrant[i, j] = board[startRow + i, startCol + j];
            }
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (clockwise)
                {
                    board[startRow + j, startCol + size - 1 - i] = tempQuadrant[i, j];
                }
                else
                {
                    board[startRow + size - 1 - j, startCol + i] = tempQuadrant[i, j];
                }
            }
        }
        GameManager.instance.WinCondition(WinCheck());
        print(DebugBoard()); // Affiche le plateau après la rotation.
    }


    // cette méthode permet de retourner la matrice à chaque mouvement et rotation.
    public int[,] CurrentPlayfield()
    {
        int[,] current = new int[numRows, numCols];
        System.Array.Copy(board,current,board.Length);
        return current;
    }


}


