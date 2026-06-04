using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera _cam;

    void Start() => _cam = Camera.main;
    void LateUpdate() => transform.rotation = _cam.transform.rotation;
}