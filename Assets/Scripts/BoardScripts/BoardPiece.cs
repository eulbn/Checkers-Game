using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arc
{
    public class BoardPiece : MonoBehaviour
    {
        [SerializeField] private string pieceType;
        public string PieceType => pieceType;


        [SerializeField] private string pieceTeam;
        public string PieceTeam => pieceTeam;


        [System.Serializable]
        public class MovementRelativeCoordinate
        {
            public Vector2Int coordinate;
        }
        [SerializeField] List<MovementRelativeCoordinate> movementRelativeCoordinates;
        public List<MovementRelativeCoordinate> MovementRelativeCoordinates => movementRelativeCoordinates;

    }
}





