using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using Input = UnityEngine.Input;

public class CameraTilemapController : MonoBehaviour
{
    private Camera _mainCamera;

    private Vector3 _currentMouseOff;

    private float _velocity = 0f;
    public float smoothTime = 0.3F;

    public float currentZoomLevel = 5;
    
    public float minZoomLevel = 3;
    public float maxZoomLevel = 6;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = GetComponent<Camera>();
        _mainCamera.orthographicSize = currentZoomLevel;
    }

    // Update is called once per frame
    void Update()
    {
        var mouseDelta = Input.mouseScrollDelta;
        if (mouseDelta != Vector2.zero)
            Zoom(mouseDelta.y);

        var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(1))
        {
            _currentMouseOff = mousePos;
        }

        if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
        {
            var offset = _currentMouseOff - mousePos;

            MoveCamera(offset);
        }

        _mainCamera.orthographicSize = Mathf.SmoothDamp(_mainCamera.orthographicSize, currentZoomLevel, ref _velocity, smoothTime);
    }

    void Zoom(float direction)
    {
        currentZoomLevel += -direction;

        currentZoomLevel = Mathf.Clamp(currentZoomLevel, minZoomLevel, maxZoomLevel);
    }

    void MoveCamera(Vector3 direction)
    {
        direction.z = -10;

        _mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, direction, (maxZoomLevel - currentZoomLevel) * Time.deltaTime);
    }
}
