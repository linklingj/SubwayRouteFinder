using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
                // 최대 줌
    public Settings settings;

    private Vector3 dragOrigin;
    private Camera mainCamera;
    private UIController uiController;

    private void Start()
    {
        mainCamera = Camera.main;
        uiController = FindAnyObjectByType<UIController>();
    }

    void Update()
    {
        if (!uiController.MouseOnUI())
        {
            HandleMouseDrag();
            HandleMouseScroll();
        }
    }

    
    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mainCamera.transform.position += difference;
        }
    }

    // 마우스 휠로 카메라 줌 인/아웃
    void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            mainCamera.orthographicSize -= scroll * settings.scrollSpeed;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, settings.minZoom, settings.maxZoom);
        }
    }
}
