using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.XR;

public class MovementVR : MonoBehaviour
{
    private Vector2 trackpad;
    private float Direction;
    private Vector3 moveDirection;


    public SteamVR_Input_Sources Hand;//Set Hand To Get Input From
    public float speed;
    public GameObject Head;
    //public CapsuleCollider Collider;
    public float Deadzone;//the Deadzone of the trackpad. used to prevent unwanted walking.
    public bool walk = false;
    public bool fly = false;

    private void Awake()
    {
        XRSettings.enabled = true;
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        //m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        if (!Head)
            Head = GameObject.FindGameObjectWithTag("MainCamera");
    }

    void Update()
    {
        if (!walk)
            return;
        //Collider.height = Head.transform.localPosition.y;
        //Collider.center = new Vector3(Head.transform.localPosition.x, Head.transform.localPosition.y / 2, Head.transform.localPosition.z);
        
        moveDirection = Quaternion.AngleAxis(Angle(trackpad), Vector3.up) * Head.transform.forward;//get the angle of the touch and correct it for the rotation of the controller
        UpdateInput();
        moveDirection.y = 0;
        if (GetComponent<Rigidbody>().velocity.magnitude < speed && trackpad.magnitude > Deadzone)
        {//make sure the touch isn't in the deadzone and we aren't going to fast.
             //GetComponent<Rigidbody>().AddForce(moveDirection * 30);
             transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    public static float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }

    private void UpdateInput()
    {
        trackpad = SteamVR_Actions._default.JostickPad.GetAxis(Hand);
    }
}
