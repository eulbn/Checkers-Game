using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Arc
{

    [CreateAssetMenu(fileName = "Board", menuName = "Board Game/Board", order = 1)]
    public class Board : ScriptableObject
    {
        [SerializeField] private BoardSettings boardSettings;


        [System.Serializable]
        public class BoardInfo
        {
            [SerializeField] private List<Row> rows;
            public List<Row> Rows => rows;

            [System.Serializable]
            public class Row
            {
                [SerializeField] private List<Column> columns;

                public List<Column> Columns => columns;

                [System.Serializable]
                public class Column
                {
                    [SerializeField] private SlotInfo slot;

                    public SlotInfo Slot => slot;

                    [System.Serializable]
                    public class SlotInfo
                    {
                        public BoardPieceHolder boardPieceHolder;
                        public BoardPiece boardPiece;
                    }
                }
            }
        }

        [SerializeField] private BoardInfo boardInfo;
        public BoardInfo BoardInfor => boardInfo;

        [SerializeField] private List<string> teams;
        public List<string> Teams => teams;


        public BoardPiece GetBoardPiece(string pieceType, string pieceTeam)
        {
            return boardSettings.BoardPieces.Find((x) => x.PieceType == pieceType && x.PieceTeam == pieceTeam);
        }

        public List<BoardPiece> GetAllBoardPieces()
        {
            return boardSettings.BoardPieces;
        }

        public int GetNumberOfRows()
        {
            if(boardSettings != null)
            {
                return boardSettings.Rows;
            }
            return 0;
        }

        public int GetNumberOfColumns()
        {
            if (boardSettings != null)
            {
                return boardSettings.Columns;
            }
            return 0;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Board))]
    public class InventoryScriptableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (Board)target;
            base.OnInspectorGUI();
        }
    }
#endif
}
