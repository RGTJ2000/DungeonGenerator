using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float defaultViewSize;
    private Vector3 defaultCameraPosition;
    private Quaternion defaultCameraRotation;

    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        defaultViewSize = _camera.orthographicSize;
        defaultCameraPosition = _camera.transform.position;
        defaultCameraRotation = _camera.transform.rotation;
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
