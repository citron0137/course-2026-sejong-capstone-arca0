using UnityEngine;
using UnityEngine.InputSystem;

public class StartMonoBehavior : MonoBehaviour
{

    void Awake()
    {

    }

    void Update()
    {
        if (Mouse.current == null) return;
        // 마우스 커서 위치 파악
        var worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Debug.Log("World Position: " + worldPosition);

        // btn_exit 영역 파악
        var btnExit = GameObject.Find("btn_exit");
        var btnExitCollider = btnExit.GetComponent<BoxCollider2D>();
        if (btnExitCollider.OverlapPoint(worldPosition))
        {
            Debug.Log("btn_exit 영역 파악");
        }else
        {
            Debug.Log("btn_exit 영역 아님");
        }

    }
}
