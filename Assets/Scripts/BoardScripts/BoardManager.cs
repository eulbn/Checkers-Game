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

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }


        public void Initialize()
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


        public string GetCurrentTeam()
        {
            return board.Teams[currentTeamIndex];
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
    }
}
