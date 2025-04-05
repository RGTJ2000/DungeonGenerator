using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float defaultViewSize;
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        defaultViewSize = _camera.orthographicSize;
    }

    public void SetCameraViewSize(float size)
    {
        _camera.orthographicSize = size;
    }

    public void ResetCameraViewSize()
    {
        _camera.orthographicSize = defaultViewSize;
    }
}
