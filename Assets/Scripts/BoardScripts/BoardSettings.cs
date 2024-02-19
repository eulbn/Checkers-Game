using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arc
{

    [CreateAssetMenu(fileName = "Board Settings", menuName = "Board Game/Board Settings", order = 1)]
    public class BoardSettings : ScriptableObject
    {
        [Header("---Board Parameters---")]
        [SerializeField] private int row;
        public int Row => row;


        [SerializeField] private int column;
        public int Column => column;


        [SerializeField] private List<string> teamNames;
        public List<string> TeamNames => teamNames;


        [SerializeField] private List<BoardPieceHolder> boardPiecesHolders;
        public List<BoardPieceHolder> BoardPieceHolders => boardPiecesHolders;


        [Header("---Board Pieces---")]

        [SerializeField] private List<BoardPiece> boardPieces;
        public List<BoardPiece> BoardPieces => boardPieces;


    }
}
