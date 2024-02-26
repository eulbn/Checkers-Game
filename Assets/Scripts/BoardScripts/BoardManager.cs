using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arc
{
    public class BoardManager : Singleton<BoardManager>
    {
        [SerializeField] Board board;
        private List<List<BoardPieceHolder>> grid;
        public Vector2 cellSize = Vector3.one;
        public Vector2 origin = Vector2.zero;

        private bool isPieceSelected = false;
        public bool IsPieceSelected { get { return isPieceSelected; } set { isPieceSelected = value; } }

        private int currentTeamIndex = 0;


        public void Intialize()
        {
            Setup();
            if(GameManager.Instance.IsPlayingWithAI)
            {
                AIManager.Instance.Intialize(board.GetNumberOfRows(), board.GetNumberOfColumns(), board.GetAllBoardPieces(), GetTheOtherTeam(), 7);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            /*else if(Input.GetKeyDown(KeyCode.K))
            {
                AIManager.Instance.Play(grid);
            }*/
        }


        public void Setup()
        {
            if (board == null)
            {
                Debug.LogError("Board is null");
                return;
            }

            Board.BoardInfo boardInfo = board.BoardInfor;
            grid = new List<List<BoardPieceHolder>>();

            for (int i = 0; i < boardInfo.Rows.Count; i++)
            {
                grid.Add(new List<BoardPieceHolder>());

                for (int j = 0; j < boardInfo.Rows[i].Columns.Count; j++)
                {

                    if (boardInfo.Rows[i].Columns[j].Slot.boardPieceHolder != null)
                    {
                        Vector3 position = origin + new Vector2(j * cellSize.y, i * cellSize.x);

                        GameObject holder = Instantiate(boardInfo.Rows[i].Columns[j].Slot.boardPieceHolder.gameObject,
                            position, Quaternion.identity);

                        holder.transform.position = position;
                        holder.transform.parent = transform;

                        BoardPieceHolder temBoardPieceHolder = holder.GetComponent<BoardPieceHolder>();

                        grid[i].Add(temBoardPieceHolder);

                        BoardPiece temBoardPiece = null;

                        if (boardInfo.Rows[i].Columns[j].Slot.boardPiece != null)
                        {
                            GameObject piece = Instantiate(boardInfo.Rows[i].Columns[j].Slot.boardPiece.gameObject,
                            position, Quaternion.identity);

                            holder.transform.position = position;
                            holder.transform.parent = holder.transform;

                            temBoardPiece = piece.GetComponent<BoardPiece>();
                        }
                        temBoardPieceHolder.Initialize(temBoardPiece, j, i);

                    }
                }
            }

        }


        public BoardPieceHolder GetBoardPieceHolderAtIndex(int x, int y)
        {
            if (y >= 0 && y < grid.Count
                && x >= 0 && x < grid[y].Count)
            {
                return grid[y][x];
            }
            else
            {
                return null;
            }
        }


        public void UnSelect()
        {
            if (isPieceSelected)
            {
                foreach (var row in grid)
                {
                    foreach (var item in row)
                    {
                        item.UnSelct();
                    }
                }
            }
        }

        public BoardPiece GetBoardPiece(string pieceType, string pieceTeam)
        {
            return board.GetBoardPiece(pieceType, pieceTeam);
        }

        public List<BoardPiece> GetAllBoardPieces()
        {
            return board.GetAllBoardPieces();
        }

        public string GetCurrentTeam()
        {
            return board.Teams[currentTeamIndex];
        }

        public string GetTheOtherTeam()
        {
            return board.Teams[board.Teams.Count - 1];
        }

        public void ChangeCurrentTeam()
        {
            currentTeamIndex++;
            if (currentTeamIndex >= board.Teams.Count)
            {
                currentTeamIndex = 0;
            }
        }


        public bool HasCurrentTeamLost()
        {
            string currentTeamName = GetCurrentTeam();
            foreach (var row in grid)
            {
                foreach (var col in row)
                {
                    if (!col.IsEmpty() && col.HoldingBoardPiece.PieceTeam == currentTeamName)
                    {
                        if (col.HasCurrentHoldingPieceMoves())
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }


        public void PlayAI()
        {
            GameManager.Instance.IsAIPlaying = true;
            StartCoroutine(PlayingAI());
        }

        IEnumerator PlayingAI()
        {
            yield return new WaitForSeconds(0.01f);
            if (AIManager.Instance.TeamNameAI == GetCurrentTeam())
            {
                AIManager.Instance.Play(grid);
            }
        }

        public void PlayMove(int xIndex, int yIndex, int moveIndex)
        {
            if (yIndex >= 0 && yIndex < grid.Count
               && xIndex >= 0 && xIndex < grid[yIndex].Count)
            {
                if (grid[yIndex][xIndex] != null)
                {
                    grid[yIndex][xIndex].PlayAIMove(moveIndex);
                }
            }
        }
    }
}
