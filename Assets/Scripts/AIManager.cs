using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

namespace Arc
{
    public class AIManager : Singleton<AIManager>
    {


        [System.Serializable]
        private class BoardSlot
        {
            public bool isEmpty;
            public string pieceType;
            public string pieceTeam;

            public int score;

            public class MovementRelativeCoordinate
            {
                public Vector2Int coordinate;
            }
            public List<MovementRelativeCoordinate> movementRelativeCoordinates;

            public class UpgradeConversion
            {
                public string pieceTeam;
                public string fromPieceType;
                public string toPieceType;

            }
            public List<UpgradeConversion> upgradeConversions;
        }
        private int row, column;
        private BoardSlot[,] gameState;

        [SerializeField] private string teamNameAI;
        public string TeamNameAI => teamNameAI;

        private List<BoardPiece> boardPieces;

        private int depthSerachLimit = 10;



        public void Intialize(int row, int column, List<BoardPiece> boardPieces, string teamNameAI, int depthSerachLimit = 5)
        {
            this.boardPieces = boardPieces;
            this.teamNameAI = teamNameAI;
            this.depthSerachLimit = depthSerachLimit;

            gameState = new BoardSlot[row, column];
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < column; c++)
                {
                    gameState[r, c] = new BoardSlot();
                }
            }

            this.row = row; this.column = column;
        }


        public void Play(List<List<BoardPieceHolder>> grid)
        {
            if (grid != null) 
            {
                for (int i = 0; i < row; i++) // Save the current game state
                {
                    for (int j = 0; j < column; j++)
                    {
                        gameState[i, j].isEmpty = true;
                        gameState[i, j].pieceType = "";
                        gameState[i, j].pieceTeam = "";
                        gameState[i, j].score = 0;
                        gameState[i, j].movementRelativeCoordinates = null;
                        gameState[i, j].upgradeConversions = null;

                        if (grid[i][j] != null && grid[i][j].IsEmpty() == false)
                        {
                            gameState[i, j].isEmpty = false;
                            gameState[i, j].pieceType = grid[i][j].HoldingBoardPiece.PieceType;
                            gameState[i, j].pieceTeam = grid[i][j].HoldingBoardPiece.PieceTeam;
                            gameState[i, j].score = GetPieceScore(gameState[i, j].pieceType, gameState[i, j].pieceTeam);

                            gameState[i, j].movementRelativeCoordinates = new List<BoardSlot.MovementRelativeCoordinate>();

                            foreach (var item in grid[i][j].HoldingBoardPiece.MovementRelativeCoordinates)
                            {
                                BoardSlot.MovementRelativeCoordinate temMovementRelativeCoordinate = new BoardSlot.MovementRelativeCoordinate();
                                temMovementRelativeCoordinate.coordinate = item.coordinate;
                                gameState[i, j].movementRelativeCoordinates.Add(temMovementRelativeCoordinate);
                            }

                            if (grid[i][j].UpgradeConversions != null)
                            {
                                gameState[i, j].upgradeConversions = new List<BoardSlot.UpgradeConversion>();
                                foreach (var upgrade in grid[i][j].UpgradeConversions)
                                {
                                    BoardSlot.UpgradeConversion temUpgradeConversion = new BoardSlot.UpgradeConversion();

                                    temUpgradeConversion.pieceTeam = upgrade.pieceTeam;
                                    temUpgradeConversion.toPieceType = upgrade.toPieceType;
                                    temUpgradeConversion.fromPieceType = upgrade.fromPieceType;
                                }
                            }
                        }
                    }
                }

                isWinningMove = false;
                endSerach = false;
                currentR = 0;
                currentC = 0;
                currentMoveIndex = 0;
                Serach(0, true);


                BoardManager.Instance.PlayMove(currentC, currentR, currentMoveIndex);


            }
            else
            {
                Debug.LogError("Board is Null");
            }
        }


        int currentR = 0;
        int currentC = 0;
        int currentMoveIndex = 0;
        bool endSerach = false;
        bool isWinningMove = false;

        public int Serach(int depth, bool isMax)
        {
            if (depth >= depthSerachLimit || endSerach)
            {
                return EvaluateScore();
            }
            int currentMaxScore = int.MinValue;
            for (int rr = 0; rr < row; rr++)
            {
                for (int cc = 0; cc < column; cc++)
                {
                    if (gameState[rr, cc].isEmpty == false
                        && ((isMax && gameState[rr, cc].pieceTeam == teamNameAI) || 
                        (isMax == false && gameState[rr, cc].pieceTeam != teamNameAI))) // Explore Possible States
                    {
                        int movementRelativeCoordinatesCount = gameState[rr, cc].movementRelativeCoordinates.Count;
                        for (int i = 0; i < movementRelativeCoordinatesCount; i++)
                        {
                            int temXCoord = gameState[rr, cc].movementRelativeCoordinates[i].coordinate.x;
                            int temYCoord = gameState[rr, cc].movementRelativeCoordinates[i].coordinate.y;
                            

                            int newCurrentScore = Move(depth, isMax, rr, cc, i, rr + temYCoord, cc + temXCoord);

                            float prob = Random.Range(0f, 1f);
                            if ((prob > 0.5f? (newCurrentScore >= currentMaxScore): false ) || (newCurrentScore > currentMaxScore) || isWinningMove)
                            {
                                currentMaxScore = newCurrentScore;
                                if(depth == 0)
                                {
                                    currentR = rr;
                                    currentC = cc;
                                    currentMoveIndex = i;
                                }

                                if (isWinningMove)
                                {
                                    endSerach = true;
                                    return currentMaxScore;
                                }
                            }
                        }
                    }
                }
            }
            return currentMaxScore;

        }

        public int Move(int depth, bool isMax, int rr, int cc, int moveIndex, int temYCoord, int temXCoord, bool isKillMove = false)
        {
            if (IsInBounds(temXCoord, temYCoord) && gameState[temYCoord, temXCoord].isEmpty == true)
            {

                gameState[temYCoord, temXCoord].isEmpty = false;
                gameState[temYCoord, temXCoord].pieceType = gameState[rr, cc].pieceType;
                gameState[temYCoord, temXCoord].pieceTeam = gameState[rr, cc].pieceTeam;
                gameState[temYCoord, temXCoord].score = gameState[rr, cc].score;
                gameState[temYCoord, temXCoord].movementRelativeCoordinates = gameState[rr, cc].movementRelativeCoordinates;

                int temScore = gameState[rr, cc].score;
                string temPieceType = gameState[rr, cc].pieceType;
                string temPieceTeam = gameState[rr, cc].pieceTeam;

                UpgradeConversion(temYCoord, temXCoord, temPieceType, temPieceTeam);

                gameState[rr, cc].isEmpty = true;
                gameState[rr, cc].pieceType = "";
                gameState[rr, cc].pieceTeam = "";
                gameState[rr, cc].score = 0;
                gameState[rr, cc].movementRelativeCoordinates = null;

                int currentScore = Serach(depth + 1,!isMax);

                gameState[rr, cc].isEmpty = false;
                gameState[rr, cc].pieceType = temPieceType;
                gameState[rr, cc].pieceTeam = temPieceTeam;
                gameState[rr, cc].score = temScore;
                gameState[rr, cc].movementRelativeCoordinates = gameState[temYCoord, temXCoord].movementRelativeCoordinates;

                gameState[temYCoord, temXCoord].isEmpty = true;
                gameState[temYCoord, temXCoord].pieceType = "";
                gameState[temYCoord, temXCoord].pieceTeam = "";
                gameState[temYCoord, temXCoord].score = 0;
                gameState[temYCoord, temXCoord].movementRelativeCoordinates = null;

                return currentScore;
            }
            else if (IsInBounds(temXCoord, temYCoord) &&
                gameState[temYCoord, temXCoord].isEmpty == false &&
                gameState[temYCoord, temXCoord].pieceTeam != gameState[rr, cc].pieceTeam && 
                isKillMove == false)
            {
                int killPieceScore = gameState[temYCoord, temXCoord].score;
                string temPieceType = gameState[temYCoord, temXCoord].pieceType;
                string temPieceTeam = gameState[temYCoord, temXCoord].pieceTeam;
                List<BoardSlot.MovementRelativeCoordinate> temMovementRelativeCoordinates = gameState[temYCoord, temXCoord].movementRelativeCoordinates;

                gameState[temYCoord, temXCoord].isEmpty = true;
                gameState[temYCoord, temXCoord].score = 0;
                gameState[temYCoord, temXCoord].pieceTeam = "";
                gameState[temYCoord, temXCoord].pieceType = "";
                gameState[temYCoord, temXCoord].movementRelativeCoordinates = null;

                int newTemYCoord = temYCoord - rr;
                int newTemXCoord = temXCoord - cc;

                int currentScore = Move(depth, isMax, rr, cc, moveIndex, rr + newTemYCoord + newTemYCoord, cc + newTemXCoord + newTemXCoord, true);

                gameState[temYCoord, temXCoord].isEmpty = false;
                gameState[temYCoord, temXCoord].score = killPieceScore;
                gameState[temYCoord, temXCoord].pieceType = temPieceType;
                gameState[temYCoord, temXCoord].pieceTeam = temPieceTeam;
                gameState[temYCoord, temXCoord].movementRelativeCoordinates = temMovementRelativeCoordinates;

                return currentScore;
            }

            return int.MinValue;
        }

        private void PrintScore()
        {
            for (int rr = row - 1; rr >= 0; rr--)
            {
                string someStr = "";
                for (int cc = 0; cc < column; cc++)
                {
                    someStr += " " + gameState[rr, cc].score;
                }
                Debug.Log(someStr);
            }
        }

        private int EvaluateScore()
        {
            int totalScore = 0;
            int negativeScore = 0;
            for (int rr = 0; rr < row; rr++)
            {
                for (int cc = 0; cc < column; cc++)
                {
                    totalScore += gameState[rr, cc].score;
                    if (gameState[rr, cc].score < 0)
                    {
                        negativeScore += gameState[rr, cc].score;
                    }
                }
            }
            if(negativeScore == 0)
            {
                isWinningMove = true;
            }

            return totalScore;
        }

        

        public void UpgradeConversion(int yCoord, int xCoord, string pieceType, string pieceTeam)
        {
            if (gameState[yCoord, xCoord].upgradeConversions != null)
            {
                foreach (var upgrade in gameState[yCoord, xCoord].upgradeConversions)
                {
                    if(upgrade.fromPieceType == pieceType && upgrade.pieceTeam == pieceTeam)
                    {
                        gameState[yCoord, xCoord].score = GetPieceScore(upgrade.toPieceType, upgrade.pieceTeam);
                        return;
                    }
                }
            }
        }

        private int GetPieceScore(string pieceType, string pieceTeam)
        {
            foreach (var boardPiece in boardPieces)
            {
                if(boardPiece.PieceType == pieceType && boardPiece.PieceTeam == pieceTeam)
                {
                    if (pieceTeam == teamNameAI)
                    {
                        return boardPiece.Score;
                    }
                    else
                    {
                        return -boardPiece.Score;
                    }
                }
            }

            return 0;
        }

        private bool IsInBounds(int xIndex, int yIndex)
        {
            if (yIndex < row && yIndex >= 0 && xIndex < column && xIndex >= 0)
            {
                return true;
            }
            return false;
        }
    }


}
