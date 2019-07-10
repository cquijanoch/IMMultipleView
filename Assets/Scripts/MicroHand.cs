using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MicroHand : MonoBehaviour
{
    // Start is called before the first frame update
    private SteamVR_Behaviour_Pose m_Pose = null;
    public List<Data> selectedData;
    private Data m_currentDataSelect = null;
    public bool printEvents = false;
    private Hand m_myHand;
    public GameObject descriptionDialog;
    private GameObject m_currentDialog;
    public float m_numberPushDataObject = float.MaxValue;
    private bool m_FlagToPushDataObject = false;
    public Data m_previousData = null;

    //private bool m_IsInnerDataObject = false;
    //private bool m_changeDataObject = false;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
    }
    void Start()
    {
        m_myHand = GetComponent<Hand>();
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
                return;
            }

            if (m_currentDataSelect != m_myHand.getDataFromIndex() && m_currentDialog)
            {
                Destroy(m_currentDialog);
                m_previousData = m_currentDataSelect;
                m_currentDataSelect = m_myHand.getDataFromIndex();
                m_currentDialog = Instantiate(descriptionDialog);
                m_currentDialog.transform.SetParent(m_currentDataSelect.transform);
                return;
            }

            if (m_numberPushDataObject > Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
            {
                if (printEvents) print(Time.deltaTime + " Single push Data Object");
                m_numberPushDataObject = 0;
                m_previousData = null;
                return;
            }

            if (m_numberPushDataObject < Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER && m_previousData == m_currentDataSelect)
            {
                if (printEvents) print(Time.deltaTime + " Double push Data Object");
                m_numberPushDataObject = float.MaxValue;
                if (m_currentDataSelect.ToogleSelectData())
                    selectedData.Add(m_currentDataSelect);
                else
                    selectedData.Remove(m_currentDataSelect);
                return;
            }
        }
        else
        {
            if (m_currentDataSelect)
            {
                m_previousData = m_currentDataSelect;
                Destroy(m_currentDialog);
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
