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
        if (SteamVR_Actions._default.TouchYbutton.GetStateDown(m_Pose.inputSource))
        {
            print(m_Pose.inputSource + "YButton Down");
            ToogleMenuCanvas();
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

}
