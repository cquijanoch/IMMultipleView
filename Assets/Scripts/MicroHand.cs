using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Valve.VR;

public class MicroHand : MonoBehaviour
{
    // Start is called before the first frame update
    private SteamVR_Behaviour_Pose m_Pose = null;
    private Data m_currentDataSelect = null;
    public bool printEvents = false;
    private Hand m_myHand;
    public GameObject descriptionDialog;
    private GameObject m_currentDialog;
    public float m_numberPushDataObject = float.MaxValue;
    private bool m_FlagToPushDataObject = false;
    public Data m_previousData = null;
    public bool m_stateSelect = true;
    public GameObject interactions;
    private Interaction m_interactionsCoordinated = null;

    //private bool m_IsInnerDataObject = false;
    //private bool m_changeDataObject = false;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
    }
    void Start()
    {
        m_myHand = GetComponent<Hand>();
        if (interactions)
            m_interactionsCoordinated = interactions.GetComponent<Interaction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_numberPushDataObject < float.MaxValue)
            m_numberPushDataObject += Time.deltaTime;

        if (isDataObject())
        {
            if (printEvents) print(Time.deltaTime + "  Single pick");
            
            if (!m_currentDataSelect)
            {
                m_currentDataSelect = m_myHand.getDataFromIndex();
                m_currentDialog = Instantiate(descriptionDialog);
                m_currentDialog.transform.SetParent(m_currentDataSelect.transform);
                m_currentDialog.GetComponentsInChildren<Text>()[0].text = m_currentDataSelect.m_currentSubpace.name;
                m_currentDialog.GetComponentsInChildren<Text>()[1].text = m_currentDataSelect.GetInstanceID().ToString();
                m_currentDialog.GetComponentsInChildren<Text>()[2].text = m_currentDataSelect.GetComponent<Renderer>().GetInstanceID().ToString();
                return;
            }

            if (m_currentDataSelect != m_myHand.getDataFromIndex() && m_currentDialog)
            {
                Destroy(m_currentDialog);
                m_previousData = m_currentDataSelect;
                m_currentDataSelect = m_myHand.getDataFromIndex();
                m_currentDialog = Instantiate(descriptionDialog);
                m_currentDialog.transform.SetParent(m_currentDataSelect.transform);
                m_currentDialog.GetComponentsInChildren<Text>()[0].text = m_currentDataSelect.m_currentSubpace.name;
                m_currentDialog.GetComponentsInChildren<Text>()[1].text = m_currentDataSelect.GetInstanceID().ToString() ;
                m_currentDialog.GetComponentsInChildren<Text>()[2].text = m_currentDataSelect.GetComponent<Renderer>().GetInstanceID().ToString();
                return;
            }

            if (m_numberPushDataObject > Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER && m_stateSelect)
            {
                if (printEvents) print(Time.deltaTime + " Single push Data Object");
                m_numberPushDataObject = 0;
                m_previousData = null;
                m_stateSelect = false;
                return;
            }

            if (m_numberPushDataObject < Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER && m_previousData == m_currentDataSelect)
            {
                if (printEvents) print(Time.deltaTime + " Double push Data Object");
                m_numberPushDataObject = float.MaxValue;
                m_interactionsCoordinated.ToogleDataParents(m_currentDataSelect);
                m_stateSelect = false;
                return;
            }
        }
        else
        {
            if (m_currentDataSelect)
            {
                m_previousData = m_currentDataSelect;
                Destroy(m_currentDialog);
                if (!m_stateSelect) m_stateSelect = true;
            }
            m_currentDataSelect = null;
            
        }

    }

    private bool isDataObject()
    {
        if (!m_myHand.getDataFromIndex() || !m_myHand.getDataFromIndex().gameObject.CompareTag("DataScatterplot"))
            return false;
        return true;
    }

    public void CleanDescriptionDialog()
    {
        if (m_currentDialog)
            Destroy(m_currentDialog);
        m_currentDataSelect = null;
    }



   
}
