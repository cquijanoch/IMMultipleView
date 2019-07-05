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

    public Subspace m_CurrentTakedSubspace = null;

    /** List of Subsoaces inner into MacroHand**/
    public List<Subspace> m_ContactInteractables = new List<Subspace>();
    public int m_currentIndexSelected = -1;

    public bool m_isPressedPrimaryPickup = false;
    public bool m_isPressedSecundaryPickup = false;
    public int m_TypeHand = Constants.HAND_NONE_USE;

    private float m_quantityTriggGrabDown = float.MaxValue;
    private bool m_FlagToTriggGrab = false;

    private float m_quantityTriggGripDown = float.MaxValue;
    private bool m_FlagToTriggGrip = false;

    public GameObject dialogCommon;
    private GameObject m_currentDialog;
    public Subspace dataToDelete;

    private Hand m_myHand;
    private MacroHand m_otherHand;

    public bool printEvents = false;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }
    void Start()
    {
        m_myHand = GetComponent<Hand>();
        m_otherHand = m_myHand.GetOtherMacroHand();
    }

    void Update()
    {
        if (m_FlagToTriggGrab && m_quantityTriggGrabDown < float.MaxValue)
            m_quantityTriggGrabDown += Time.deltaTime;

        if (m_FlagToTriggGrip && m_quantityTriggGripDown < float.MaxValue)
            m_quantityTriggGripDown += Time.deltaTime;

        if (m_GrabAction.GetStateDown(m_Pose.inputSource) && m_quantityTriggGrabDown > Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            if (printEvents) print(Time.deltaTime + " " + m_Pose.inputSource + " Trigger Grab Down");
            m_quantityTriggGrabDown = 0;
            m_FlagToTriggGrab = true;
            Pickup();
            return;
        }

        if (m_GrabAction.GetStateDown(m_Pose.inputSource) && m_quantityTriggGrabDown < Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            if (printEvents) print(Time.deltaTime + " " + m_Pose.inputSource + " Double Trigger Grab Down");
            m_FlagToTriggGrab = false;
            m_quantityTriggGrabDown = float.MaxValue;
            Clone();
            return;
        }

        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            if (printEvents) print(Time.deltaTime + " " + m_Pose.inputSource + "Trigger Grab Up");
            Drop();
            return;
        }

        if (m_ContactInteractables.Count < 2 && SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource) &&
            m_quantityTriggGripDown > Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            if (printEvents) print(Time.deltaTime + " " + m_Pose.inputSource + " Single Trigger Grip Down");
            m_quantityTriggGripDown = 0;
            m_FlagToTriggGrip = true;
            return;
        }

        if (m_ContactInteractables.Count > 1 && SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource) &&
            !m_isPressedPrimaryPickup && !m_isPressedSecundaryPickup)
        {
            if (printEvents) print(Time.deltaTime + " " + m_Pose.inputSource + " Inner Trigger Grip Down");
            ChangeCurrentSelectionSpace();
            return;
        }

        if (m_ContactInteractables.Count == 0 && m_currentDialog && SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource) &&
            m_quantityTriggGripDown < Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER && !m_isPressedPrimaryPickup)
        {
            if (printEvents) print(Time.deltaTime + " " + m_Pose.inputSource + " Double Trigger Grip Down No Subspaces");
            m_FlagToTriggGrip = false;
            m_quantityTriggGripDown = float.MaxValue;
            DisableToDelete();
            return;
        }

        if (m_ContactInteractables.Count == 1 && !m_currentDialog && SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource) &&
            m_quantityTriggGripDown < Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER && !m_isPressedPrimaryPickup)
        {
            if (printEvents) print(Time.deltaTime + " " + m_Pose.inputSource + " Double Trigger Grip Down");
            m_FlagToTriggGrip = false;
            m_quantityTriggGripDown = float.MaxValue;
            EnableToDelete();
            return;
        }

        if (m_CurrentTakedSubspace && SteamVR_Actions._default.TouchXbutton.GetStateDown(m_Pose.inputSource) && m_ContactInteractables.Count > 1)
        {
            if (printEvents) print(Time.deltaTime + " " + m_Pose.inputSource + "XButton Down");
            SetTransformForSimilar();
            return;
        }

    }

    private void SetTransformForSimilar()
    {
        Subspace target = m_currentIndexSelected + 1 == m_ContactInteractables.Count ? m_ContactInteractables[0] : m_ContactInteractables[m_currentIndexSelected + 1];
        if (m_CurrentTakedSubspace.DetectSimimilarTransform(target))
        {
            m_CurrentTakedSubspace.SetTransformToObject(target);
            Drop();
        }
    }


    private void ChangeCurrentSelectionSpace()
    {
        if (m_TypeHand == Constants.HAND_PRIMARY_USE)
        {
            m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner.Remove(this);
            if (m_ContactInteractables[m_currentIndexSelected].GetNumberUsedHandsInner() == 0 
                || m_otherHand.GetCurrentSubspace() != m_ContactInteractables[m_currentIndexSelected] )
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
        if (!other.gameObject.CompareTag("Subspace"))
            return;
        if (printEvents) print(Time.deltaTime + " " + "OnTriggerEnter : " + other.gameObject.name);
        Subspace subspace = other.gameObject.GetComponent<Subspace>();
        m_ContactInteractables.Add(subspace);
        if (m_currentIndexSelected < 0)
            m_currentIndexSelected = 0;
        subspace.m_numControllersInner++;
        if (enabled && !subspace.m_modePrepareToDelete)
            m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
        if (!m_CurrentTakedSubspace)
            subspace.m_HandsActivedInner.Add(this);
        //if (m_isPressedPrimaryPickup && m_isPressedSecundaryPickup)
        //    m_isPressedSecundaryPickup = false;
        DetectTypeHand();

    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Subspace"))
            return;
        if (printEvents) print(Time.deltaTime + " " + "OnTriggerExit : " + other.gameObject.name);
        Subspace subspace = other.gameObject.GetComponent<Subspace>();
        m_ContactInteractables.Remove(subspace);
        subspace.m_numControllersInner--;
        subspace.m_HandsActivedInner.Remove(this);
        if (enabled && !subspace.m_modePrepareToDelete && (subspace.GetNumberUsedHandsInner(true) == 0))
            other.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
        if (m_currentIndexSelected + 1 > m_ContactInteractables.Count)
            m_currentIndexSelected--;
        if (subspace.m_modeScale && subspace.m_PrimaryHand && subspace.m_SecondaryHand)
            StopScaleAndAutoDetectHand(subspace);
        if (m_ContactInteractables.Count > 0 && !m_CurrentTakedSubspace)
        {
            m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner.Add(this);
            if (enabled)
                m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
        }
        
        if (m_ContactInteractables.Count == 0)
            m_CurrentTakedSubspace = null;

        DetectTypeHand();
    }


    public void Pickup()
    {
        if (m_TypeHand == Constants.HAND_PRIMARY_USE)
            m_isPressedPrimaryPickup = true;
        if (m_TypeHand == Constants.HAND_SECONDARY_USE)
            m_isPressedSecundaryPickup = true;

        if (m_TypeHand == Constants.HAND_NONE_USE)
        {
            m_isPressedPrimaryPickup = false;
            m_isPressedSecundaryPickup = false;
        }
        

        if (m_CurrentTakedSubspace || m_currentIndexSelected < 0)
            return;

        m_CurrentTakedSubspace = m_ContactInteractables[m_currentIndexSelected];

        if (!m_CurrentTakedSubspace)
            return;
        DetectTypeHand();
        if (m_TypeHand == Constants.HAND_PRIMARY_USE)
        {
            Rigidbody targetBody = m_CurrentTakedSubspace.GetComponent<Rigidbody>();
            m_Joint.connectedBody = targetBody;
            m_CurrentTakedSubspace.m_PrimaryHand = this;
            m_CurrentTakedSubspace.m_SecondaryHand = null;
        }

        if (m_TypeHand == Constants.HAND_SECONDARY_USE)
        {
            m_CurrentTakedSubspace.m_SecondaryHand = this;
            StopJoiningSubspace();
            m_CurrentTakedSubspace.m_distanceInitialForScale = Vector3.Distance(m_CurrentTakedSubspace.m_PrimaryHand.transform.position, transform.position);
            m_CurrentTakedSubspace.m_modeScale = true;
        }
        DetectTypeHand();
    }

    public void Drop()
    {
        if (m_TypeHand == Constants.HAND_PRIMARY_USE)
            m_isPressedPrimaryPickup = false;
        if (m_TypeHand == Constants.HAND_SECONDARY_USE)
            m_isPressedSecundaryPickup = false;
        
        if (m_TypeHand == Constants.HAND_NONE_USE)
        {
            m_isPressedPrimaryPickup = false;
            m_isPressedSecundaryPickup = false;
        }
        
        if (m_currentIndexSelected < 0)
            return;

        if (!m_CurrentTakedSubspace)
            return;
        if (m_TypeHand == Constants.HAND_PRIMARY_USE)
        {
            if (m_CurrentTakedSubspace.m_PrimaryHand && !m_CurrentTakedSubspace.m_SecondaryHand)
            {
                Rigidbody targetBody = m_CurrentTakedSubspace.GetComponent<Rigidbody>();
                targetBody.velocity = Vector3.zero;
                targetBody.angularVelocity = Vector3.zero;
                m_Joint.connectedBody = null;
                m_CurrentTakedSubspace.m_PrimaryHand = null;
                m_CurrentTakedSubspace = null;
            }

            else if (m_CurrentTakedSubspace.m_PrimaryHand && m_CurrentTakedSubspace.m_SecondaryHand)
            {
                m_CurrentTakedSubspace.ResetDistanceInitialForScale();
                m_CurrentTakedSubspace.m_PrimaryHand = m_CurrentTakedSubspace.m_SecondaryHand;
                m_CurrentTakedSubspace.m_SecondaryHand = null;
                m_CurrentTakedSubspace = null;
                m_otherHand.m_isPressedPrimaryPickup = true;
                m_otherHand.m_isPressedSecundaryPickup = false;
                m_otherHand.DetectTypeHand();
                m_otherHand.JoiningSubspace();
            }
        }

        else if (m_TypeHand == Constants.HAND_SECONDARY_USE)
        {
            m_CurrentTakedSubspace.ResetDistanceInitialForScale();
            m_CurrentTakedSubspace.m_SecondaryHand = null;
            m_CurrentTakedSubspace = null;
            JoiningSubspace();
            
        }
        DetectTypeHand();
    }

    private void StopJoiningSubspace()
    {
        MacroHand hp = GetPrimaryHandPressedFromInner();
        if (!hp)
            return;
        if (!hp.m_ContactInteractables[m_currentIndexSelected])
            return;
        Rigidbody targetBody = hp.m_ContactInteractables[m_currentIndexSelected].GetComponent<Rigidbody>();
        targetBody.velocity = Vector3.zero;
        targetBody.angularVelocity = Vector3.zero;
        hp.m_Joint.connectedBody = null;
    }

    public void JoiningSubspace()
    {
        MacroHand hp = GetPrimaryHandPressedFromInner();
        if (!hp)
            return;
        if (!hp.m_ContactInteractables[m_currentIndexSelected])
            return;
        Rigidbody targetBody = hp.m_ContactInteractables[m_currentIndexSelected].GetComponent<Rigidbody>();
        hp.m_Joint.connectedBody = targetBody;
    }

    /**
    *  Get MacroHand that is primary and has pressed Pickup  from the contact subspace
    **/
    private MacroHand GetPrimaryHandPressedFromInner()
    {
        foreach(MacroHand h in m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner)
        {
            if (h.m_TypeHand == Constants.HAND_PRIMARY_USE && h.m_isPressedPrimaryPickup && h.m_CurrentTakedSubspace)
                return h;
        }
        return null;
    }

    /**
    * Detect Type Hand
    * 
    **/
    public void DetectTypeHand()
    {
        /* If not exists contact with subsapces*/
        if (m_ContactInteractables.Count == 0)
        {
            m_TypeHand = Constants.HAND_NONE_USE;
            return;
        }

        /* If in the current subspace is not asigned primaryHand*/
        if (!m_ContactInteractables[m_currentIndexSelected].m_PrimaryHand)
        {
            foreach (MacroHand h in m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner)
                h.m_TypeHand = Constants.HAND_PRIMARY_USE;
            return;
        }

        /*  RECALIBRATING HANDS INNER INTO SUBSPACE     */

        foreach (MacroHand h in m_ContactInteractables[m_currentIndexSelected].m_HandsActivedInner)
        {
            if (h.m_isPressedPrimaryPickup)
                h.m_TypeHand = Constants.HAND_PRIMARY_USE;
            else
                h.m_TypeHand = Constants.HAND_SECONDARY_USE;

        }

    }

    /**
    * Stop Scale mode and refactor type of Hands
    **/
    private void StopScaleAndAutoDetectHand(Subspace subspace)
    {
        subspace.ResetDistanceInitialForScale();
        // If the output MacroHand is the same that primaryHand
        if (this == subspace.m_PrimaryHand)
        {
            subspace.m_PrimaryHand = subspace.m_SecondaryHand;
            subspace.m_SecondaryHand = null;
            m_CurrentTakedSubspace = null;
            m_otherHand.m_isPressedPrimaryPickup = true;
            m_otherHand.m_isPressedSecundaryPickup = false;
            m_isPressedPrimaryPickup = false;
            m_isPressedSecundaryPickup = false;
            m_otherHand.DetectTypeHand();
            m_otherHand.JoiningSubspace();
        }
        else
        {
            subspace.m_SecondaryHand = null;
            m_CurrentTakedSubspace = null;
            m_isPressedPrimaryPickup = false;
            m_isPressedSecundaryPickup = false;
            DetectTypeHand();
            m_otherHand.DetectTypeHand();
            m_otherHand.JoiningSubspace();        
            
        }
            
        if (subspace.GetNumberUsedHandsInner() == 0)
            subspace.m_PrimaryHand = null;

    }

    /**
    * Clone Subspace that has contact 
    **/
    private void Clone()
    {
        if (m_TypeHand != Constants.HAND_PRIMARY_USE || m_currentIndexSelected < 0 || !m_ContactInteractables[m_currentIndexSelected] ||
            m_CurrentTakedSubspace)
            return;
        GameObject clone = Instantiate(m_ContactInteractables[m_currentIndexSelected].gameObject);
        clone.GetComponent<Subspace>().m_numControllersInner = 0;
        clone.GetComponent<Subspace>().m_modePrepareToDelete = false;
    }

   /**
    * Prepare to Subspace to Delete coloring and displying confirmation dialog
    **/
    private void EnableToDelete()
    {
        if (m_currentIndexSelected < 0 && !m_ContactInteractables[m_currentIndexSelected])
            return;
        dataToDelete = m_ContactInteractables[m_currentIndexSelected];
        dataToDelete.m_modePrepareToDelete = true;
        dataToDelete.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_PREPARE_TO_DELETE;
        m_myHand.HideHand();
        m_currentDialog = Instantiate(dialogCommon);
        m_currentDialog.transform.SetParent(transform);
    }

    /**
     * Delete the subspace or no , depends the parameter answer
     **/
    public void Delete(bool answer)
    {
        if (!answer && m_currentDialog)
        {
            DisableToDelete();
            return;
        }
        GetComponent<Valve.VR.InteractionSystem.Hand>().otherHand.GetComponent<MacroHand>().OnTriggerExit(dataToDelete.GetComponent<Collider>());
        m_ContactInteractables.Remove(dataToDelete);
        m_CurrentTakedSubspace = null;
        Destroy(dataToDelete.gameObject);
        m_currentIndexSelected--;
        DetectTypeHand();
        Destroy(m_currentDialog);
        m_myHand.ShowHand();

    }

    /**
     * Disable the Subspace to delete and close the confirmation dialog
     * 
     **/
    private void DisableToDelete()
    {
        Destroy(m_currentDialog);
        m_myHand.ShowHand();
        dataToDelete.m_modePrepareToDelete = false;
        if (dataToDelete.GetNumberUsedHandsInner() == 0)
            dataToDelete.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
        else
            dataToDelete.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
        dataToDelete = null;
    }

    /**
     * Set Empty COlor to subspaces that has contact with this Macrohand
     **/
    public void SetEmptyColorSubspaces()
    {
        foreach (Subspace sub in m_ContactInteractables)
        {
            sub.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
        }
    }

    /**
     * Set Empty COlor to Current Subspace
     **/
    public void SetEmptyColorCurrentSubspace()
    {
        if (m_currentIndexSelected < 0)
            return;
        m_ContactInteractables[m_currentIndexSelected].GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
    }

    /**
     * Set Color to Subspaces that has contact with this Macrohand
     **/
    public void SetAutoColorSubspaces()
    {
        foreach (Subspace sub in m_ContactInteractables)
        {
            sub.SetAutoColor();
        }
    }

    /**
     * Get Current Subspace to selected or selected already
     * This is colored in the visualization
     **/
    public Subspace GetCurrentSubspace()
    {
        if (m_currentIndexSelected < 0)
            return null;
        return m_ContactInteractables[m_currentIndexSelected];
    }

}
