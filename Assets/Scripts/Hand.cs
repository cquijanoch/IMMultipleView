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

    private bool m_isPressedPrimaryPickup = false;
    private int m_TypeHand = Constants.HAND_NONE_USE;

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

        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + " Trigger Grab Down");
            Pickup();
        }

        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "Trigger Grab Up");
            Drop();
        }

        if (m_ContactInteractables.Count > 1 && SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + " Trigger Grip Down");
            ChangeCurrentSelectionSpace();
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
            Drop();
        }
    }


    private void ChangeCurrentSelectionSpace()
    {
        if (m_TypeHand == Constants.HAND_PRIMARY_USE)
        {
            if (m_ContactInteractables[m_currentIndexSelected].m_numControllersInner == 1)
            {
                //m_ContactInteractables[m_currentIndexSelected].m_isTarget = false;
                m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
            }
            Drop();
            if (++m_currentIndexSelected == m_ContactInteractables.Count)
            {
                m_currentIndexSelected = 0;
            }
            //m_ContactInteractables[m_currentIndexSelected].m_isTarget = true;
            m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
            if (m_isPressedPrimaryPickup)
                Pickup();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;
        Interactable subspace = other.gameObject.GetComponent<Interactable>();
        m_ContactInteractables.Add(subspace);
        if (m_currentIndexSelected < 0)
            m_currentIndexSelected = 0;
        subspace.m_numControllersInner++;
        m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
        //subspace.m_isTarget = true;
        DetectTypeHand();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;
        Interactable subspace = other.gameObject.GetComponent<Interactable>();
        m_ContactInteractables.Remove(subspace);
        subspace.m_numControllersInner--;

        if (subspace.m_numControllersInner == 0)
        {
            other.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
            //subspace.m_isTarget = false;
        }
        if (m_currentIndexSelected + 1 > m_ContactInteractables.Count)
            m_currentIndexSelected--;
        if (m_ContactInteractables.Count > 0)
            m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
    }

    public void Pickup()
    {
        if (m_currentIndexSelected < 0)
            return;
        m_CurrentInteractable = m_ContactInteractables[m_currentIndexSelected];
        if (!m_CurrentInteractable)
            return;
        if (m_TypeHand == Constants.HAND_PRIMARY_USE)
        {
            Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
            m_Joint.connectedBody = targetBody;
            m_CurrentInteractable.m_PrimaryHand = this;
            m_isPressedPrimaryPickup = true;
        }

        if (m_TypeHand == Constants.HAND_SECONDARY_USE)
        {
        
        }
    }


    public void Drop()
    {
        if (!m_CurrentInteractable) 
            return;
        if (m_CurrentInteractable.m_PrimaryHand)
        {
            Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
            targetBody.velocity = Vector3.zero;
            targetBody.angularVelocity = Vector3.zero;

            m_Joint.connectedBody = null;
            m_CurrentInteractable.m_PrimaryHand = null;
            m_CurrentInteractable = null;
            m_isPressedPrimaryPickup = false;
        }
        DetectTypeHand();
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

    private void DetectTypeHand()
    {
        if (m_ContactInteractables.Count == 0)
        {
            m_TypeHand = Constants.HAND_NONE_USE;
            return;
        }
           
        if (m_TypeHand == Constants.HAND_NONE_USE && m_ContactInteractables[m_currentIndexSelected].m_PrimaryHand)
        {
            m_TypeHand = Constants.HAND_SECONDARY_USE;
            return;
        }
            
        if (m_TypeHand == Constants.HAND_NONE_USE && !m_ContactInteractables[m_currentIndexSelected].m_PrimaryHand)
        {
            m_TypeHand = Constants.HAND_PRIMARY_USE;
            return;
        }
            
    }

}
