using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MacroHand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Boolean m_GripAction = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();
    private int m_currentIndexSelected = -1;

    private bool m_isPressedPrimaryPickup = false;
    private bool m_isPressedSecundaryPickup = false;
    public int m_TypeHand = Constants.HAND_NONE_USE;

    private float m_quantityTriggGrabDown = float.MaxValue;
    private bool m_FlagToTriggGrab = false;

    private float m_quantityTriggGripDown = float.MaxValue;
    private bool m_FlagToTriggGrip = false;

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
        if (m_FlagToTriggGrab && m_quantityTriggGrabDown < float.MaxValue)
            m_quantityTriggGrabDown += Time.deltaTime;

        if (m_FlagToTriggGrip && m_quantityTriggGripDown < float.MaxValue)
            m_quantityTriggGripDown += Time.deltaTime;

        if (m_GrabAction.GetStateDown(m_Pose.inputSource) && m_quantityTriggGrabDown > Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            print(m_Pose.inputSource + " Trigger Grab Down");
            m_quantityTriggGrabDown = 0;
            m_FlagToTriggGrab = true;
            Pickup();
            return;
        }

        if (m_GrabAction.GetStateDown(m_Pose.inputSource) && m_quantityTriggGrabDown < Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            print(m_Pose.inputSource + " Double Trigger Grab Down");
            m_FlagToTriggGrab = false;
            m_quantityTriggGrabDown = float.MaxValue;
            Clone();
            return;
        }

        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "Trigger Grab Up");
            Drop();
            return;
        }

        if (m_ContactInteractables.Count == 1 && SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource) &&
            m_quantityTriggGripDown > Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            print(m_Pose.inputSource + "Single Trigger Grip Down");
            m_quantityTriggGripDown = 0;
            m_FlagToTriggGrip = true;
            return;
        }

        if (m_ContactInteractables.Count > 1 && SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "Inner Trigger Grip Down");
            ChangeCurrentSelectionSpace();
            return;
        }

        if (m_ContactInteractables.Count == 1 && SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource) &&
            m_quantityTriggGripDown < Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            print(m_Pose.inputSource + " Double Trigger Grip Down");
            m_FlagToTriggGrip = false;
            m_quantityTriggGripDown = float.MaxValue;
            Delete();
            return;
        }

        if (m_CurrentInteractable && SteamVR_Actions._default.TouchXbutton.GetStateDown(m_Pose.inputSource) && m_ContactInteractables.Count > 1)
        {
            print(m_Pose.inputSource + "XButton Down");
            SetTransformForSimilar();
            return;
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
            m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner.Remove(this);
            if (m_ContactInteractables[m_currentIndexSelected].m_numControllersInner == 1)
                m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
            Drop();
            if (++m_currentIndexSelected == m_ContactInteractables.Count)
                m_currentIndexSelected = 0;
            m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner.Add(this);
            m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
            DetectTypeHand();
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;
        print("OnTriggerEnter : " + other.gameObject.name);
        Interactable subspace = other.gameObject.GetComponent<Interactable>();
        m_ContactInteractables.Add(subspace);
        if (m_currentIndexSelected < 0)
            m_currentIndexSelected = 0;
        subspace.m_numControllersInner++;
        m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
        if (!m_isPressedPrimaryPickup)
            subspace.m_HandsActivedInner.Add(this);
        DetectTypeHand();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;
        print("OnTriggerExit : " + other.gameObject.name);
        Interactable subspace = other.gameObject.GetComponent<Interactable>();
        m_ContactInteractables.Remove(subspace);
        subspace.m_numControllersInner--;
        subspace.m_HandsActivedInner.Remove(this);
        if (subspace.m_numControllersInner == 0 || subspace.m_HandsActivedInner.Count == 0)
            other.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
        if (subspace.m_modeScale && subspace.m_PrimaryHand && subspace.m_SecondaryHand)
            StopScaleAndAutoDetectHand(subspace);
        if (m_currentIndexSelected + 1 > m_ContactInteractables.Count)
            m_currentIndexSelected--;
        if (m_ContactInteractables.Count > 0)
            m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
        DetectTypeHand();
    }

    public void Pickup()
    {
        if (m_currentIndexSelected < 0)
            return;

        m_CurrentInteractable = m_ContactInteractables[m_currentIndexSelected];
        if (!m_CurrentInteractable)
            return;
        DetectTypeHand();
        if (m_TypeHand == Constants.HAND_PRIMARY_USE)
        {
            Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
            m_Joint.connectedBody = targetBody;
            m_CurrentInteractable.m_PrimaryHand = this;
            m_isPressedPrimaryPickup = true;
        }

        if (m_TypeHand == Constants.HAND_SECONDARY_USE)
        {
            m_CurrentInteractable.m_SecondaryHand = this;
            m_isPressedSecundaryPickup = true;
            StopJoiningIteractable();
            m_CurrentInteractable.m_distanceInitialForScale = Vector3.Distance(m_CurrentInteractable.m_PrimaryHand.transform.position, transform.position);
            m_CurrentInteractable.m_modeScale = true;
        }
        DetectTypeHand();
    }

    public void Drop()
    {
        if (!m_CurrentInteractable)
            return;
        if (m_TypeHand == Constants.HAND_PRIMARY_USE)
        {
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
        }
        if (m_TypeHand == Constants.HAND_SECONDARY_USE)
        {
            m_CurrentInteractable.ResetDistanceInitialForScale();
            m_CurrentInteractable.m_SecondaryHand = null;
            m_isPressedSecundaryPickup = false;
            JoiningIteractable();
            
        }
        DetectTypeHand();
    }

    private void StopJoiningIteractable()
    {
        MacroHand hp = GetPrimaryHandPressedFromInner();
        if (!hp)
            return;
        if (!hp.m_CurrentInteractable)
            return;
        Rigidbody targetBody = hp.m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = Vector3.zero;
        targetBody.angularVelocity = Vector3.zero;
        hp.m_Joint.connectedBody = null;
    }

    private void JoiningIteractable()
    {
        MacroHand hp = GetPrimaryHandPressedFromInner();
        if (!hp)
            return;
        if (!hp.m_CurrentInteractable)
            return;
        Rigidbody targetBody = hp.m_CurrentInteractable.GetComponent<Rigidbody>();
        hp.m_Joint.connectedBody = targetBody;
    }

    private MacroHand GetPrimaryHandPressedFromInner()
    {
        foreach(MacroHand h in m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner)
        {
            if (h.m_TypeHand == Constants.HAND_PRIMARY_USE && h.m_isPressedPrimaryPickup)
                return h;
        }
        return null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach (Interactable interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }
        return nearest;
    }

    private void DetectTypeHand()
    {
        /* If not exists contact with interactables*/
        if (m_ContactInteractables.Count == 0)
        {
            m_TypeHand = Constants.HAND_NONE_USE;
            return;
        }

        /* If in the current interactable is not asigned primaryHand*/
        if (!m_ContactInteractables[m_currentIndexSelected].m_PrimaryHand)
        {
            foreach (MacroHand h in m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner)
                h.m_TypeHand = Constants.HAND_PRIMARY_USE;
            return;
        }

        /*  RECALIBRATING HANDS INNER INTO INTERACTABLE     */

        foreach (MacroHand h in m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner)
        {
            if (h.m_isPressedPrimaryPickup)
                h.m_TypeHand = Constants.HAND_PRIMARY_USE;
            else
                h.m_TypeHand = Constants.HAND_SECONDARY_USE;
        }

    }

    private void StopScaleAndAutoDetectHand(Interactable subspace)
    {
        subspace.m_HandsActivedInner.Remove(this);
        subspace.m_PrimaryHand.m_isPressedPrimaryPickup = false;
        subspace.m_SecondaryHand.m_isPressedPrimaryPickup = false;
        subspace.m_PrimaryHand = null;
        subspace.m_SecondaryHand = null;
    }

    private void Clone()
    {
        if (m_currentIndexSelected < 0 && !m_ContactInteractables[m_currentIndexSelected])
            return;
        Instantiate(m_ContactInteractables[m_currentIndexSelected].gameObject);
    }

    private void Delete()
    {
        if (m_currentIndexSelected < 0 && !m_ContactInteractables[m_currentIndexSelected])
            return;
        Interactable dataToDelete = m_ContactInteractables[m_currentIndexSelected];
        m_ContactInteractables.Remove(dataToDelete);
        m_CurrentInteractable = null;
        Destroy(dataToDelete.gameObject);
        m_currentIndexSelected--;
        DetectTypeHand();
    }

}
