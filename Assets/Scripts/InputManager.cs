using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arc
{
    public class InputManager : MonoBehaviour
    {
        private Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && GameManager.Instance.IsAIPlaying == false)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                if (hit.collider != null)
                {
                    BoardPieceHolder holder = hit.collider.GetComponent<BoardPieceHolder>();
                    if (holder != null)
                    {
                        holder.OnClick();
                    }
                    else
                    {
                        BoardManager.Instance.UnSelect();
                    }
                }
                else
                {
                    BoardManager.Instance.UnSelect();
                }
            }
        }
    }
}
