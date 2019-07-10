using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    private SteamVR_Behaviour_Pose m_Pose = null;

    public bool showMenu = false;
    private int modeTypeHand = Constants.INT_HAND_MODE_MACRO;

    public GameObject menuCanvas;

    private GameObject m_menuCanvas;
    // private MacroHand m_typeMacroHand;
    private MacroHand m_currentMacroHand;
    //private MicroHand m_typeMicroHand;
    private MicroHand m_currentMicroHand;

    private bool lastEnableMacroHand;

    void Start()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_currentMacroHand = GetComponent<MacroHand>();
        m_currentMicroHand = GetComponent<MicroHand>();
        ChangeModeTypeHand(modeTypeHand);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_currentMacroHand.m_CurrentTakedSubspace && !m_currentMacroHand.dataToDelete
            && SteamVR_Actions._default.TouchYbutton.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "YButton Down");
            ToogleModeHand();
        }

        //print(GetComponent<Valve.VR.InteractionSystem.Hand>().hoveringInteractable);
    }

    public void ChangeModeTypeHand(int intHandMode)
    {
        if (intHandMode == Constants.INT_HAND_MODE_MACRO)
        {
            m_currentMicroHand.enabled = false;
            m_currentMacroHand.enabled = true;
        }

        if (intHandMode == Constants.INT_HAND_MODE_MICRO)
        {
            m_currentMacroHand.enabled = false;
            m_currentMicroHand.enabled = true;
        }
    }

    private void ToogleMenuCanvas()
    {
        if (!showMenu && !m_menuCanvas)
        {
            m_menuCanvas = Instantiate(menuCanvas);
            m_menuCanvas.transform.SetParent(transform);
            showMenu = true;
        }
        else
        {
            Destroy(m_menuCanvas);
            showMenu = false;
        }
        
    }

    private void ToogleModeHand()
    {  
        // Change MACRO to MICRO
        if (modeTypeHand == Constants.INT_HAND_MODE_MACRO)
        {
            GetComponent<Valve.VR.InteractionSystem.Hand>().HideController(true);
            if (m_currentMacroHand.GetCurrentSubspace() && m_currentMacroHand.GetCurrentSubspace().GetNumberUsedHandsInner() == 1)//if isnt other macrohand inner
                m_currentMacroHand.SetEmptyColorCurrentSubspace();
            //if (m_currentMacroHand.GetCurrentSubspace())
            //    m_currentMacroHand.GetCurrentSubspace().m_HandsActivedInner.Remove(m_currentMacroHand);
            //GetComponent<Collider>().enabled = false;
            modeTypeHand = Constants.INT_HAND_MODE_MICRO;
            ChangeModeTypeHand(modeTypeHand);
            GetComponent<Valve.VR.InteractionSystem.Hand>().useFingerJointHover = true;
        }
        //Change MICRO TO MACRO
        else if (modeTypeHand == Constants.INT_HAND_MODE_MICRO)
        {
            GetComponent<Valve.VR.InteractionSystem.Hand>().ShowController(true);
            //if (m_currentMacroHand.GetCurrentSubspace())
            //    m_currentMacroHand.GetCurrentSubspace().m_HandsActivedInner.Add(m_currentMacroHand);
            //GetComponent<Collider>().enabled = true;
            modeTypeHand = Constants.INT_HAND_MODE_MACRO;
            ChangeModeTypeHand(modeTypeHand);
            m_currentMacroHand.SetAutoColorSubspaces();
            GetComponent<Valve.VR.InteractionSystem.Hand>().useFingerJointHover = false;
            m_currentMicroHand.CleanDescriptionDialog();
        }
        
    }

    public void HideFisicHand()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().HideSkeleton();
        GetComponent<Valve.VR.InteractionSystem.Hand>().HideGrabHint();
    }

    public void ShowFisicHand()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().ShowSkeleton();
        GetComponent<Valve.VR.InteractionSystem.Hand>().ShowGrabHint();
    }

    public void HideFisicController()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().HideController();
        GetComponent<Valve.VR.InteractionSystem.Hand>().HideGrabHint();
    }

    public void ShowFisicController()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().ShowController();
        GetComponent<Valve.VR.InteractionSystem.Hand>().ShowGrabHint();
    }

    public void HideHand()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().HideController();
        GetComponent<Valve.VR.InteractionSystem.Hand>().HideSkeleton();
        GetComponent<Valve.VR.InteractionSystem.Hand>().enabled = false;
        lastEnableMacroHand = m_currentMacroHand.enabled;
        m_currentMacroHand.enabled = false;
        //DisableBothMacroHand();
    }

    public void ShowHand()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().enabled = true;
        GetComponent<Valve.VR.InteractionSystem.Hand>().ShowController();
        GetComponent<Valve.VR.InteractionSystem.Hand>().ShowSkeleton();
        m_currentMacroHand.enabled = lastEnableMacroHand;
        //EnableBothMacroHand();
    }

    public void DisableBothMacroHand()
    {
        GetComponent<MacroHand>().enabled = false;
        DisableOtherMacroHand();
    }

    public void DisableOtherMacroHand()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().otherHand.GetComponent<MacroHand>().enabled = false;
    }

    public void EnableBothMacroHand()
    {
        GetComponent<MacroHand>().enabled = true;
        EnableOtherMacroHand();
    }

    public void EnableOtherMacroHand()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().otherHand.GetComponent<MacroHand>().enabled = true;
    }

    public MacroHand GetOtherMacroHand()
    {
        return GetComponent<Valve.VR.InteractionSystem.Hand>().otherHand.GetComponent<MacroHand>();
    }

    public Data getDataFromIndex()
    {
        if (GetComponent<Valve.VR.InteractionSystem.Hand>().hoveringInteractable)
            return GetComponent<Valve.VR.InteractionSystem.Hand>().hoveringInteractable.GetComponent<Data>();
        return null;
    }

    public bool getStatusFingerHand()
    {
        return GetComponent<Valve.VR.InteractionSystem.Hand>().useFingerJointHover;
    }
    public void ActivateFingerHand()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().useFingerJointHover = true;
    }

    public void DesactivateFingerHand()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().useFingerJointHover = false;
    }
}
