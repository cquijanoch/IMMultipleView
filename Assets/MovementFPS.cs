using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MovementFPS : MonoBehaviour
{
    public float speed = 0.1f;
    public bool stopMovement = false;

    private void Awake()
    {
        XRSettings.enabled = false;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
    }

    private void Update()
    {
        if (stopMovement)
            return;
        float translation = Input.GetAxis("Vertical") * speed;
        float straffe = Input.GetAxis("Horizontal") * speed;
        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;
        transform.Translate(straffe, 0, translation);
        if (Input.GetKeyDown("escape"))
            Cursor.lockState = CursorLockMode.None;
    }
}
