using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Boolean m_GripAction = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();
    private int m_currentIndexSelected = -1;

    public float m_moveSped = 10f;

    private Color m_colorSpaceSelected = new Color(0.6666527f, 0.6581524f, 0.1f, 0.2941177f);
    private Color m_colorSpaceWithoutSelected = new Color(0.6666527f, 0.6581524f, 0.9622641f, 0.2941177f);

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }
    void Start()
    {
        
    }

    void Update()
    {

        if(m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + " Trigger Grab Down");
            Pickup();
        }

        if (m_ContactInteractables.Count > 1 && SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + " Trigger Grip Down");
            ChangeCurrentSelectionSpace();
        }

        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "Trigger Grab Up");
            Drop();
        }

        if (m_CurrentInteractable && SteamVR_Actions._default.TouchXbutton.GetStateDown(m_Pose.inputSource) && m_ContactInteractables.Count > 1)
        {
            print(m_Pose.inputSource + "XButton Down");
            SetTransformForSimilar();
        }

    }

    private void SetTransformForSimilar()
    {
        Interactable target = m_currentIndexSelected + 1 == m_ContactInteractables.Count ? m_ContactInteractables[0] : m_ContactInteractables[m_currentIndexSelected + 1];
        if (m_CurrentInteractable.DetectSimimilarTransform(target))
        {
            m_CurrentInteractable.SetTransformToObject(target);
        }
    }


    private void ChangeCurrentSelectionSpace()
    {
        m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = m_colorSpaceWithoutSelected;
        if ( ++m_currentIndexSelected == m_ContactInteractables.Count)
        {
            m_currentIndexSelected = 0;
        }
        m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = m_colorSpaceSelected;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;
        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
        if (m_currentIndexSelected < 0) m_currentIndexSelected = 0;
        m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = m_colorSpaceSelected;


    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;
        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
        other.GetComponent<Renderer>().material.color = m_colorSpaceWithoutSelected;
        if ( m_currentIndexSelected + 1 > m_ContactInteractables.Count )
        {
            m_currentIndexSelected--;
        }
        if (m_ContactInteractables.Count > 0)
            m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = m_colorSpaceSelected;
    }

    public void Pickup()
    {
        if (m_currentIndexSelected < 0 )
            return;
        m_CurrentInteractable = m_ContactInteractables[m_currentIndexSelected];
        if (!m_CurrentInteractable)
            return;
        if (m_CurrentInteractable.m_ActiveHand)
            m_CurrentInteractable.m_ActiveHand.Drop();

        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetBody;

        m_CurrentInteractable.m_ActiveHand = this;
    }

    public void Drop()
    {
        if (!m_CurrentInteractable)
            return;
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = Vector3.zero;
        targetBody.angularVelocity = Vector3.zero;

        m_Joint.connectedBody = null;

        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach(Interactable interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;
            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }
        return nearest;
    }


}
