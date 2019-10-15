using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMouseLook : MonoBehaviour
{
    Vector2 mouseLook;
    Vector2 smoothly;
    public float sensivity = 4f;
    public float smoothing = 2f;

    GameObject character;

    public bool mouselooked = false;
    void Start()
    {
        character = transform.parent.gameObject;
    }

    void Update()
    {
        if (mouselooked) return;
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensivity * smoothing, sensivity * smoothing));
        smoothly.x = Mathf.Lerp(smoothly.x, md.x, 1f / smoothing);
        smoothly.y = Mathf.Lerp(smoothly.y, md.y, 1f / smoothing);
        mouseLook += smoothly;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
