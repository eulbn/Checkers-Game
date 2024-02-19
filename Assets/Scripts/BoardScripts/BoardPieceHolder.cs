using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Arc
{
    public class BoardPieceHolder : MonoBehaviour
    {

        [SerializeField] SpriteRenderer heighlight;

        [SerializeField] int xIndex, yIndex;


        [System.Serializable]
        public class UpgradeConversion
        {
            public string pieceTeam;
            public string fromPieceType;
            public string toPieceType;

        }
        [SerializeField] private List<UpgradeConversion> upgradeConversions;
        public List<UpgradeConversion> UpgradeConversions => upgradeConversions;



        private BoardPiece holdingBoardPiece = null;
        public BoardPiece HoldingBoardPiece { get { return holdingBoardPiece; } set { holdingBoardPiece = value; } }


        private bool isSelected = false;
        public bool IsSelected => isSelected;


        private static BoardPieceHolder CurrentPieceHolder = null;



        List<List<BoardPieceHolder>> killBoardPieceHolderLists = null;


        public void Initialize(BoardPiece boardPiece, int xIndex, int yIndex)
        {
            holdingBoardPiece = boardPiece;
            this.xIndex = xIndex;
            this.yIndex = yIndex;

            killBoardPieceHolderLists = new List<List<BoardPieceHolder>>();
        }

        public void OnClick()
        {
            if (CurrentPieceHolder != null && isSelected == true)
            {
                BoardPiece temBoardPiece = CurrentPieceHolder.holdingBoardPiece;
                CurrentPieceHolder.holdingBoardPiece = null;

                temBoardPiece.transform.parent = transform;
                temBoardPiece.transform.position = transform.position;
                holdingBoardPiece = temBoardPiece;

                CurrentPieceHolder = null;

                if (killBoardPieceHolderLists.Count > 0)
                {
                    List<BoardPieceHolder> largestKillList = killBoardPieceHolderLists[killBoardPieceHolderLists.Count - 1];

                    int max = 0;
                    foreach (var item in killBoardPieceHolderLists)
                    {
                        if (item.Count > max)
                        {
                            max = item.Count;
                            largestKillList = item;
                        }
                    }
                    foreach (var killBoardPieceHolder in largestKillList)
                    {
                        if (killBoardPieceHolder.holdingBoardPiece != null)
                        {
                            Destroy(killBoardPieceHolder.holdingBoardPiece.gameObject);
                            killBoardPieceHolder.holdingBoardPiece = null;
                        }
                    }
                }

                BoardManager.Instance.UnSelect();
                OnUpgrade();
                BoardManager.Instance.ChangeCurrentTeam();

                if (BoardManager.Instance.HasCurrentTeamLost())
                {
                    UIManager.Instance.OpenFinalPopup(BoardManager.Instance.GetCurrentTeam());
                }
            }
            else if (!IsEmpty() && BoardManager.Instance.GetCurrentTeam() == holdingBoardPiece.PieceTeam)
            {
                BoardManager.Instance.UnSelect();

                CurrentPieceHolder = this;

                CheckCoords(null);
            }
            else
            {
                BoardManager.Instance.UnSelect();
            }
        }

        private void CheckCoords(List<BoardPieceHolder> killBoardPieceHolderList)
        {
            if (killBoardPieceHolderList != null)
            {
                killBoardPieceHolderLists.Add(killBoardPieceHolderList);
            }

            foreach (var cords in CurrentPieceHolder.holdingBoardPiece.MovementRelativeCoordinates)
            {
                int relativeXIndex = xIndex + cords.coordinate.x;
                int relativeYIndex = yIndex + cords.coordinate.y;

                BoardPieceHolder relativeBoardPieceHolder = BoardManager.Instance.GetBoardPieceHolderAtIndex(relativeXIndex, relativeYIndex);

                if (relativeBoardPieceHolder != null)
                {
                    if (relativeBoardPieceHolder.IsEmpty() && killBoardPieceHolderList == null)
                    {
                        relativeBoardPieceHolder.Select();
                    }
                    else if (!relativeBoardPieceHolder.IsEmpty()
                        && relativeBoardPieceHolder.holdingBoardPiece.PieceTeam != CurrentPieceHolder.holdingBoardPiece.PieceTeam)
                    {
                        relativeXIndex += cords.coordinate.x;
                        relativeYIndex += cords.coordinate.y;

                        BoardPieceHolder killBoardPieceHolder = BoardManager.Instance.GetBoardPieceHolderAtIndex(relativeXIndex, relativeYIndex);

                        if ((killBoardPieceHolder != null && killBoardPieceHolder.IsEmpty())
                            || (killBoardPieceHolder != null && killBoardPieceHolder == CurrentPieceHolder))
                        {
                            List<BoardPieceHolder> temKillBoardPieceHolderList = new List<BoardPieceHolder>();

                            if (killBoardPieceHolderList != null)
                            {
                                foreach (var item in killBoardPieceHolderList)
                                {
                                    temKillBoardPieceHolderList.Add(item);
                                }
                            }

                            bool dontLookFurther = false;
                            if (temKillBoardPieceHolderList.Contains(relativeBoardPieceHolder) == true)
                            {
                                dontLookFurther = true;
                            }


                            if (dontLookFurther == false)
                            {
                                temKillBoardPieceHolderList.Add(relativeBoardPieceHolder);
                                killBoardPieceHolder.CheckCoords(temKillBoardPieceHolderList);
                                killBoardPieceHolder.Select();
                            }
                        }
                    }
                }
            }
        }

        public void Select()
        {
            BoardManager.Instance.IsPieceSelected = true;
            isSelected = true;
            heighlight.gameObject.SetActive(true);
        }



        public void UnSelct()
        {
            if (isSelected == true)
            {
                isSelected = false;
                heighlight.gameObject.SetActive(false);
                CurrentPieceHolder = null;
                killBoardPieceHolderLists.Clear();
            }
        }

        public bool IsEmpty()
        {
            if (holdingBoardPiece != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void OnUpgrade()
        {
            if (upgradeConversions != null && holdingBoardPiece != null)
            {
                UpgradeConversion temUpgradeConversion = upgradeConversions.Find((x) => x.fromPieceType == holdingBoardPiece.PieceType && x.pieceTeam == holdingBoardPiece.PieceTeam);

                if (temUpgradeConversion != null)
                {
                    BoardPiece temBoardPiece = BoardManager.Instance.GetBoardPiece(temUpgradeConversion.toPieceType, temUpgradeConversion.pieceTeam);

                    if (temBoardPiece != null)
                    {
                        Destroy(holdingBoardPiece.gameObject);

                        GameObject piece = Instantiate(temBoardPiece.gameObject, transform.position, Quaternion.identity);
                        piece.transform.parent = transform;
                        holdingBoardPiece = piece.GetComponent<BoardPiece>();
                    }

                }
            }
        }

        public bool HasCurrentHoldingPieceMoves()
        {
            if (!IsEmpty())
            {
                foreach (var cords in holdingBoardPiece.MovementRelativeCoordinates)
                {
                    int relativeXIndex = xIndex + cords.coordinate.x;
                    int relativeYIndex = yIndex + cords.coordinate.y;

                    BoardPieceHolder relativeBoardPieceHolder = BoardManager.Instance.GetBoardPieceHolderAtIndex(relativeXIndex, relativeYIndex);

                    if (relativeBoardPieceHolder != null)
                    {
                        if (relativeBoardPieceHolder.IsEmpty())
                        {
                            return true;
                        }
                        else if (!relativeBoardPieceHolder.IsEmpty()
                        && relativeBoardPieceHolder.holdingBoardPiece.PieceTeam != holdingBoardPiece.PieceTeam)
                        {
                            relativeXIndex += cords.coordinate.x;
                            relativeYIndex += cords.coordinate.y;

                            BoardPieceHolder killBoardPieceHolder = BoardManager.Instance.GetBoardPieceHolderAtIndex(relativeXIndex, relativeYIndex);

                            if (killBoardPieceHolder != null && killBoardPieceHolder.IsEmpty())
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}



