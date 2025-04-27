# IAPentagoGame
## Introduction
Pentago is a game for two players played on a board with 36 squares, arranged in a 6x6 grid. The board is divided into four smaller 3x3 grids. Each turn, players place a marble of their color on the board and then rotate a small square by a quarter turn to the right or left.  
The game ends when a player lines up 5 marbles. It can also end without a winner if all the squares are full without a line of five marbles, or if both players make a line of 5 marbles at the same time.  
The goal of this project is to implement the Minimax algorithm, optimized with alpha-beta pruning, for the game Pentago. To develop this game, I used Unity 2D and programmed in C#.
The game has a board that shows where the marbles are. There are also 8 buttons to turn each part of the board, either clockwise or counter-clockwise.
## Data Structure
The data structure is very important in my game. It is not just about where the marbles are, but also about turning them. I had two choices: first, I could make four small grids, one for each part. Second, I could make one big 6x6 grid. After trying both, I chose the second one. 
### Game Board
0: Empty space
1: Black marble
2: White marble (AI)
