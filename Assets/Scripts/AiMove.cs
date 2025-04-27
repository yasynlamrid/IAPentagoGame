using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiMove : MonoBehaviour
{
    public static AiMove instance;
    int numCols = 6;
    int numRows = 6;
    int maxSearch = 2;
    public class Move
    {
        public int column;
        public int row;
        public float score;

        public Move()
        {

        }
        public Move(float _score)
        {
            score = _score;
        }
        public Move(int _row, int _column)
        {
            row = _row;
            column = _column;
        }
        public Move(int _row, int _column, float _score)
        {
            row = _row;
            column = _column;
            score = _score;
        }

    }
    private void Awake()
    {

        instance = this;

    }

    // Liste qui va regrouper tous mouvements possibles 

    List<Move> GetValidMoves(int[,] currentBoard)
    {
        List<Move> movelist = new List<Move>();

        for (int row = 0; row < numRows; row++)
        {
            for (int column = 0; column < numCols; column++)
            {
                if (currentBoard[row, column] == 0) // 0 indique un espace vide
                {
                    Move m = new Move(row, column);
                    movelist.Add(m);
                }
            }
        }

        return movelist;
    }


    public IEnumerator BestMove()
    {
        Move bestMove = new Move(-1, -1, -Mathf.Infinity);
        int[,] currentPlayfield = Playfield.instance.CurrentPlayfield();

        List<Move> possibleMoves = new List<Move>();
        possibleMoves.AddRange(GetValidMoves(currentPlayfield));

        foreach (Move move in possibleMoves)
        {
            move.score = -Mathf.Infinity;
            int[,] tempBoard = PerformTempMove(move, currentPlayfield, 2);
            move.score = Minimax(tempBoard, maxSearch,-Mathf.Infinity, Mathf.Infinity , false);
            Debug.Log($"Evaluating move at ({move.row}, {move.column}) with score {move.score}");

            if (move.score > bestMove.score)
            {
                bestMove = move;
            }
        }
        GameManager.instance.ElementPressed(bestMove.row, bestMove.column);
        yield return new WaitForSeconds(1f);
    }


    int[,] PerformTempMove(Move move, int[,] currentBoard, int player)
    {
        int[,] tempBoard = new int[numRows, numCols];
        System.Array.Copy(currentBoard, tempBoard, currentBoard.Length);
        tempBoard[move.row, move.column] = player;
        return tempBoard;

    }
    float Minimax(int[,] currentBoard ,int searchDepth, float alpha, float beta, bool isMaximizer)
    {

        float bestScore = 0;

        if (searchDepth == 0)
        {
            //evaluate board score
            bestScore = EvaluateBoard(currentBoard, isMaximizer);
            Debug.Log($"Minimax at depth 0 with score {bestScore}"); 

            return bestScore;
        }
        if (isMaximizer)
        {
            bestScore = -Mathf.Infinity;
            List<Move> possibleMoves = new List<Move>();
            possibleMoves.AddRange(GetValidMoves(currentBoard));
            foreach (Move move in possibleMoves)
            {
                int[,] tempBoard = PerformTempMove(move, currentBoard, 2);
                bestScore = System.Math.Max(bestScore, Minimax(tempBoard, searchDepth - 1,alpha, beta, !isMaximizer));

                if (bestScore >= beta)
                {
                    break;
                }
                alpha = System.Math.Max(alpha, bestScore);
            }
            return bestScore;
        }
        else
        { //Minimizer 

            bestScore = Mathf.Infinity;
            List<Move> possibleMoves = new List<Move>();
            possibleMoves.AddRange(GetValidMoves(currentBoard));
            foreach (Move move in possibleMoves)
            {
                int[,] tempBoard = PerformTempMove(move, currentBoard, 1);
                bestScore = System.Math.Min(bestScore, Minimax(tempBoard, searchDepth - 1,alpha,beta, !isMaximizer));

                if (bestScore <= alpha)
                {
                    break;
                }
                beta = System.Math.Min(beta, bestScore);
            }
            return bestScore;

        }
    }

    public float EvaluateBoard(int[,] currentBoard, bool isMaximizer)
    {
        float boardScore = 0;

       boardScore += HorizontalCheck(currentBoard, isMaximizer);

        boardScore += VerticalCheck(currentBoard, isMaximizer);

        boardScore += DiagonalCheck(currentBoard, isMaximizer);

        boardScore += CenterPositionBonus(currentBoard);

        return boardScore;
    }



    float HorizontalCheck(int[,] currentBoard, bool isMaximizer)
    {
        float score = 0;
        int numCols = 6; // Dimension du plateau de Pentago
        int numRows = 6;

        for (int row = 0; row < numRows; row++)
        {
            for (int column = 0; column < numCols; column++)
            {
                if (currentBoard[row, column] == 0)
                {
                    continue; // Passe à l'itération suivante si la cellule est vide
                }
                // De gauche à droite
                if (column <= numCols - 5) // Ajustement pour aligner cinq pions
                {
                    int a = currentBoard[row, column];
                    int b = currentBoard[row, column + 1];
                    int c = currentBoard[row, column + 2];
                    int d = currentBoard[row, column + 3];
                    int e = currentBoard[row, column + 4];
                    if (a == b && b == c && c == d && d == e) // Vérification d'une ligne de 5 pions identiques
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -1000 : 1000;
                        }
                        else // Minimizer
                        {
                            score += (a == 2) ? 1000 : -1000;
                        }
                    }


                    if (a == b && b == c && c == d && e == 0)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -250 : 250;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 250 : -250;
                        }
                    }
                    if (a == b && b == c && d == 0 && e == 0)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -50 : 50;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 50 : -50;
                        }
                    }

                 
                }

                if (column >= 4) // Ajusté pour assurer qu'on ne dépasse pas les limites à gauche
                {
                    int a = currentBoard[row, column];
                    int b = currentBoard[row, column - 1];
                    int c = currentBoard[row, column - 2];
                    int d = currentBoard[row, column - 3];
                    int e = currentBoard[row, column - 4];
                    //wind check 
                    if (a == b && b == c && c == d && d == e)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -1000 : 1000;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 1000 : -1000;
                        }
                    }

                    if (a == b && b == c && c == d && e==0)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -250 : 250;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 250 : -250;
                        }
                    }
                    if (a == b && b == c && d==0 && e == 0)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -50 : 50;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 50 : -50;
                        }
                    }
                }

            }
        }
        return score;
    }


    float DiagonalCheck(int[,] currentBoard, bool isMaximizer)
    {
        float score = 0;

        for (int row = 0; row < numRows; row++)
        {
            for (int column = 0; column < numCols; column++)
            {
                if (currentBoard[row, column] == 0)
                {
                    continue; // Skip empty slots
                }

                // Horizontal Check (Left to Right)
                if (column <= numCols - 5)
                {
                    score += EvaluateLine(currentBoard, row, column, 0, 1, isMaximizer);
                }

                // Vertical Check (Top to Bottom)
                if (row <= numRows - 5)
                {
                    score += EvaluateLine(currentBoard, row, column, 1, 0, isMaximizer);
                }

                // Diagonal Check (Top-Left to Bottom-Right)
                if (row <= numRows - 5 && column <= numCols - 5)
                {
                    score += EvaluateLine(currentBoard, row, column, 1, 1, isMaximizer);
                }

                // Diagonal Check (Bottom-Left to Top-Right)
                if (row >= 4 && column <= numCols - 5)
                {
                    score += EvaluateLine(currentBoard, row, column, -1, 1, isMaximizer);
                }
            }
        }

        return score;
    }

    float EvaluateLine(int[,] board, int startRow, int startCol, int rowInc, int colInc, bool isMaximizer)
    {
        float lineScore = 0;
        int a = board[startRow, startCol];
        int b = board[startRow + rowInc, startCol + colInc];
        int c = board[startRow + 2 * rowInc, startCol + 2 * colInc];
        int d = board[startRow + 3 * rowInc, startCol + 3 * colInc];
        int e = board[startRow + 4 * rowInc, startCol + 4 * colInc];

      
        if (a == b && b == c && c == d && d == e)
        {
            lineScore += (isMaximizer ? (a == 1 ? -1000 : 1000) : (a == 2 ? 1000 : -1000));
        }

        if (a == b && b == c && c == d && e == 0)
        {
            lineScore += (isMaximizer ? (a == 1 ? -250 : 250) : (a == 2 ? 250 : -250));
        }
        if (a == b && b == c && d == 0 && e == a)
        {
            lineScore += (isMaximizer ? (a == 1 ? -250 : 250) : (a == 2 ? 250 : -250));
        }

  
        if (a == b && c == 0 && d == 0 && e == a)
        {
            lineScore += (isMaximizer ? (a == 1 ? -50 : 50) : (a == 2 ? 50 : -50));
        }
        if (a == b && b == c && d == 0 && e == 0)
        {
            lineScore += (isMaximizer ? (a == 1 ? -50 : 50) : (a == 2 ? 50 : -50));
        }

        

        return lineScore;
    }


    float VerticalCheck(int[,] currentBoard, bool isMaximizer)
    {
        float score = 0;
        int numCols = 6; // Dimension du plateau de Pentago
        int numRows = 6;

        for (int row = 0; row < numRows; row++)
        {
            for (int column = 0; column < numCols; column++)
            {
                if (currentBoard[row, column] == 0)
                {
                    continue; // Passe à l'itération suivante si la cellule est vide
                }
                // De gauche à droite
                if (row <= 1) // Ajustement pour aligner cinq pions
                {
                    int a = currentBoard[row, column];
                    int b = currentBoard[row+1, column];
                    int c = currentBoard[row+2, column];
                    int d = currentBoard[row+3, column];
                    int e = currentBoard[row+4, column];
                    if (a == b && b == c && c == d && d == e) // Vérification d'une ligne de 5 pions identiques
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -1000 : 1000;
                        }
                        else // Minimizer
                        {
                            score += (a == 2) ? 1000 : -1000;
                        }
                    }


                    if (a == b && b == c && c == d && d == e)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -1000 : 1000;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 1000 : -1000;
                        }
                    }

                    if (a == b && b == c && c == d && e == 0)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -250 : 250;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 250 : -250;
                        }
                    }
                    if (a == b && b == c && d == 0 && e == 0)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -50 : 50;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 50 : -50;
                        }
                    }
                }

                if (row >= 4) // Ajusté pour assurer qu'on ne dépasse pas les limites à gauche
                {
                    int a = currentBoard[row, column];
                    int b = currentBoard[row-1, column ];
                    int c = currentBoard[row-2, column ];
                    int d = currentBoard[row-3, column ];
                    int e = currentBoard[row-4, column ];
                    //wind check 
                    if (a == b && b == c && c == d && d == e)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -1000 : 1000;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 1000 : -1000;
                        }
                    }

                    if (a == b && b == c && c == d && e == 0)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -250 : 250;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 250 : -250;
                        }
                    }
                    if (a == b && b == c && d == 0 && e == 0)
                    {
                        if (isMaximizer)
                        {
                            score += (a == 1) ? -50 : 50;

                        }
                        else//Minimizer
                        {
                            score += (a == 2) ? 50 : -50;
                        }
                    }

                }

            }
        }
        return score;
    }

    private float CenterPositionBonus(int[,] board)
    {
        float score = 0;
        int[,] centerPositions = { { 1, 1 }, { 1, 4 }, { 4, 1 }, { 4, 4 } };

        foreach (int i in Enumerable.Range(0, centerPositions.GetLength(0)))
        {
            int row = centerPositions[i, 0];
            int col = centerPositions[i, 1];

           
            if (board[row, col] == 2)  
                score += 20;  
            if (board[row, col] == 1) 
                score -= 20; 
        }

        return score;
    }


    public IEnumerator BestRotation1()
    {
        int[,] currentBoard = Playfield.instance.CurrentPlayfield();
        float bestScore = -Mathf.Infinity;
        List<(int, int, bool)> bestRotations = new List<(int, int, bool)>();  // Liste pour stocker les meilleures rotations

        for (int rowStart = 0; rowStart < 6; rowStart += 3)
        {
            for (int colStart = 0; colStart < 6; colStart += 3)
            {
                foreach (bool clockwise in new bool[] { true, false })
                {
                    int[,] rotatedBoard = RotateBoard((int[,])currentBoard.Clone(), rowStart, colStart, clockwise);
                    float score = EvaluateBoard(rotatedBoard, true);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestRotations.Clear();
                        bestRotations.Add((rowStart, colStart, clockwise));
                    }
                    else if (score == bestScore)
                    {
                        bestRotations.Add((rowStart, colStart, clockwise));
                    }
                }
            }
        }

        // Choisir une rotation aléatoire parmi les meilleures
        var (bestRow, bestCol, bestClockwise) = bestRotations[UnityEngine.Random.Range(0, bestRotations.Count)];
        GameManager.instance.ButtonPressed(bestRow, bestCol, bestClockwise);
        yield return new WaitForSeconds(1f);
    }

    private int[,] RotateBoard(int[,] board, int rowStart, int colStart, bool clockwise)
    {
        int size = 3;
        int[,] newBoard = (int[,])board.Clone();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (clockwise)
                {
                    newBoard[rowStart + j, colStart + size - 1 - i] = board[rowStart + i, colStart + j];
                }
                else
                {
                    newBoard[rowStart + size - 1 - j, colStart + i] = board[rowStart + i, colStart + j];
                }
            }
        }
        return newBoard;
    }


}
