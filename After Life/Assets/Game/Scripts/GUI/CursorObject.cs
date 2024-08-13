using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorObject : MonoBehaviour
{

    [SerializeField] private CursorManager.CursorType cursorType;

    private void OnMouseEnter() {
        CursorManager.Instance.SetActiveCursorType(cursorType);

        if(Input.GetButtonDown("Interact"))
            CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Idle);
    }

    private void OnMouseExit() {
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Idle);
    }
}
