﻿using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    private SteamVR_Behaviour_Pose m_Pose = null;
    private int modeTypeHand = Constants.INT_HAND_MODE_MICRO;
    private GameObject m_menuCanvas;
    private MacroHand m_currentMacroHand;
    private MicroHand m_currentMicroHand;
    private bool m_lastEnableMacroHand;

    public bool showMenu = false;
    public bool modeAnswer = false;
    public GameObject menuCanvas;

    [HideInInspector]
    public GameObject m_FinishTaskDialog;
    public GameObject finishTaskCommon;
    private bool m_custonStatusFingerOtherHand = false;

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
        if (modeAnswer)
            return;
        //if (m_ContactInteractables.Count > 1 && SteamVR_Actions._default.GrabPinch.GetStateDown(m_Pose.inputSource) &&
        //     !m_isPressedPrimaryPickup && !m_isPressedSecundaryPickup)
        if (!m_currentMacroHand.m_currentDialog && !m_FinishTaskDialog
             && ((m_currentMacroHand.enabled && !m_currentMacroHand.m_isPressedPrimaryPickup)
             || (!m_currentMacroHand.enabled && m_currentMicroHand.enabled))
             && SteamVR_Actions._default.TouchJostick.GetStateDown(m_Pose.inputSource))
        {
            FinishTask();
            return;
        }
        if (!m_currentMacroHand.m_CurrentTakedSubspace && !m_currentMacroHand.dataToDelete
            && (SteamVR_Actions._default.TouchNoPressGrabPinch.GetStateUp(m_Pose.inputSource)
            || !SteamVR_Actions._default.TouchNoPressGrabPinch.GetState(m_Pose.inputSource))
            && (SteamVR_Actions._default.GrabGrip.GetStateUp(m_Pose.inputSource) 
            || !SteamVR_Actions._default.GrabGrip.GetState(m_Pose.inputSource))
            )
        {
            MacroToMicro();
            return;
        }

        if (!m_currentMacroHand.m_CurrentTakedSubspace && !m_currentMacroHand.dataToDelete
            && SteamVR_Actions._default.TouchNoPressGrabPinch.GetStateDown(m_Pose.inputSource))
        {
            MicroToMacro();
            return;
        }
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
        if (modeTypeHand == Constants.INT_HAND_MODE_MACRO) // Change MACRO to MICRO
            MacroToMicro();
        else if (modeTypeHand == Constants.INT_HAND_MODE_MICRO)  //Change MICRO TO MACRO
            MicroToMacro();
    }

    private void MicroToMacro()
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

    private void MacroToMicro()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().HideController(true);
        if (m_currentMacroHand.GetCurrentSubspace() && m_currentMacroHand.GetCurrentSubspace().GetNumberUsedHandsInner() == 0)//if isnt other macrohand inner
            m_currentMacroHand.SetEmptyColorCurrentSubspace();
        //if (m_currentMacroHand.GetCurrentSubspace())
        //    m_currentMacroHand.GetCurrentSubspace().m_HandsActivedInner.Remove(m_currentMacroHand);
        //GetComponent<Collider>().enabled = false;
        modeTypeHand = Constants.INT_HAND_MODE_MICRO;
        ChangeModeTypeHand(modeTypeHand);
        GetComponent<Valve.VR.InteractionSystem.Hand>().useFingerJointHover = true;
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
        m_lastEnableMacroHand = m_currentMacroHand.enabled;
        m_currentMacroHand.enabled = false;
    }

    public void ShowHand()
    {
        GetComponent<Valve.VR.InteractionSystem.Hand>().enabled = true;
        GetComponent<Valve.VR.InteractionSystem.Hand>().ShowController();
        GetComponent<Valve.VR.InteractionSystem.Hand>().ShowSkeleton();
        m_currentMacroHand.enabled = m_lastEnableMacroHand;
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

    private void FinishTask()
    {
        HideHand();
        m_FinishTaskDialog = Instantiate(finishTaskCommon);
        m_FinishTaskDialog.transform.SetParent(transform);
        m_custonStatusFingerOtherHand = GetOtherMacroHand().GetComponent<Hand>().getStatusFingerHand();
        GetOtherMacroHand().GetComponent<Hand>().ActivateFingerHand();
        GetOtherMacroHand().GetComponent<Hand>().modeAnswer = true;
    }

    public void DisableFinishTask(bool answer)
    {
        Destroy(m_FinishTaskDialog);
        ShowHand();
        if (!m_custonStatusFingerOtherHand)
            GetOtherMacroHand().GetComponent<Hand>().DesactivateFingerHand();
        GetOtherMacroHand().GetComponent<Hand>().modeAnswer = false;
    }
}