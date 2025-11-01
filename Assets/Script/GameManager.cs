using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform tileParent;
    public TMP_Text result;

    public int rows = 3;
    public int cols = 3;
    public float tileSpacing = 1.0f;

    private int[,] board;
    private Tile[,] tiles;

    public bool isPlayerTurn = true;
    public bool isEnemyTurn = true;

    private void Start()
    {
        board = new int[rows, cols];
        tiles = new Tile[rows, cols];
        CreateBoard();
        result.text = "Tic Tac Toe!";
    }

    private void CreateBoard()
    {
        Vector2 startPos = new Vector2(-(cols - 1) * tileSpacing / 2, (rows - 1) * tileSpacing / 2);

        Transform parentTransform;
        if (tileParent != null)
        {
            parentTransform = tileParent;
        }
        else
        {
            parentTransform = transform;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject tileObj = Instantiate(tilePrefab, parentTransform);

                tileObj.transform.localPosition = startPos + new Vector2(c * tileSpacing, -r * tileSpacing);

                Tile tile = tileObj.GetComponent<Tile>();

                if(tile == null)
                {
                    tile = tileObj.AddComponent<Tile>();
                }

                tile.Initialize(new Vector2Int(r, c), this);
                tile.SetSymbol(0);

                tiles[r, c] = tile;
                board[r, c] = 0;
            }
        }
    }

    public void PlayerMove(Vector2Int gridPosition)
    {
        int r = gridPosition.x;
        int c = gridPosition.y;

        if (board[r, c] != 0) return;

        int playerVal;
        if (isPlayerTurn)
        {
            playerVal = 1;
        }
        else
        {
            playerVal = -1;
        }

        board[r, c] = playerVal;
        tiles[r, c].SetSymbol(playerVal);

        int winner = CheckWinner(board);
        if (winner != 0)
        {
            OnGameOver(winner);
            return;
        }

        if (IsBoardFull(board))
        {
            OnGameOver(0);
            return;
        }

        if(isEnemyTurn)
        {
            EnemyMove();
        }
    }

    private void EnemyMove()
    {
        int aiVal;

        if (isPlayerTurn)
        {
            aiVal = -1;
        }
        else
        {
            aiVal = 1;
        }

        Vector2Int move = GetBestMove(board, aiVal);

        board[move.x, move.y] = aiVal;
        tiles[move.x, move.y].SetSymbol(aiVal);

        int winner = CheckWinner(board);

        if (winner != 0)
        {
            OnGameOver(winner);
            return;
        }

        if (IsBoardFull(board))
        {
            OnGameOver(0);
            return;
        }
    }

    private void OnGameOver(int winner)
    {
        if (winner == 0)
        {
            result.text = "Game Over: Draw!";
        }
        else
        {
            int playerVal;
            if (isPlayerTurn)
            {
                playerVal = 1;
            }
            else
            {
                playerVal = -1;
            }

            if (winner == playerVal)
            {
                result.text = "Game Over: Player Wins!";
            }
            else
            {
                result.text = "Game Over: AI Wins!";
            }
        }
        Invoke(nameof(InitializeBoard), 1.5f);
    }

    private void InitializeBoard()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                board[r, c] = 0;
                tiles[r, c].SetSymbol(0);
            }
        }
        result.text = "Tic Tac Toe!";
    }

    private Vector2Int GetBestMove(int[,] state, int aiVal)
    {
        int bestScore = int.MinValue;
        Vector2Int bestMove = new Vector2Int(-1, -1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (state[r, c] == 0)
                {
                    state[r, c] = aiVal;
                    int score = Minimax(state, 0, false, aiVal);
                    state[r, c] = 0;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = new Vector2Int(r, c);
                    }
                }
            }
        }
        return bestMove;
    }

    private int Minimax(int[,] state, int depth, bool isMaximizing, int aiVal)
    {
        int winner = CheckWinner(state);

        if (winner != 0)
        {
            if (winner == aiVal)
                return 10 - depth;
            else
                return depth - 10;
        }

        if (IsBoardFull(state))
        {
            return 0;
        }

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (state[r, c] == 0)
                    {
                        state[r, c] = aiVal;
                        int score = Minimax(state, depth + 1, false, aiVal);

                        if (score > bestScore)
                            bestScore = score;

                        state[r, c] = 0;
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            int opponentVal = -aiVal;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (state[r, c] == 0)
                    {
                        state[r, c] = opponentVal;
                        int score = Minimax(state, depth + 1, true, aiVal);

                        if (score < bestScore)
                            bestScore = score;

                        state[r, c] = 0;
                    }
                }
            }
            return bestScore;
        }
    }

    private int CheckWinner(int[,] state)
    {
        for (int i = 0; i < 3; i++)
        {
            // Row
            if (state[i, 0] != 0 && state[i, 0] == state[i, 1] && state[i, 1] == state[i, 2])
                return state[i, 0];

            // Column
            if (state[0, i] != 0 && state[0, i] == state[1, i] && state[1, i] == state[2, i])
                return state[0, i];
        }

        // Check diagonals
        if (state[0, 0] != 0 && state[0, 0] == state[1, 1] && state[1, 1] == state[2, 2])
            return state[0, 0];

        if (state[0, 2] != 0 && state[0, 2] == state[1, 1] && state[1, 1] == state[2, 0])
            return state[0, 2];

        return 0;
    }

    private bool IsBoardFull(int[,] state)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (state[r, c] == 0)
                    return false;
            }
        }
        return true;
    }
}
