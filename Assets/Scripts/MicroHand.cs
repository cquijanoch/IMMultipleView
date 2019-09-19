using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Valve.VR;

public class MicroHand : MonoBehaviour
{
    private SteamVR_Behaviour_Pose m_Pose = null;
    private Data m_currentDataSelect = null;
    public bool printEvents = false;
    private Hand m_myHand;
    public GameObject descriptionDialog;
    private GameObject m_currentDialog;
    public Data m_previousData = null;
    public bool m_stateSelect = true;
    public GameObject interactions;
    private Interaction m_interactionsCoordinated = null;

    public AudioClip SingleSelectAudio;
    public AudioClip DoubleSelectAudio;
    private AudioSource m_audioSource;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        
        m_myHand = GetComponent<Hand>();
        if (interactions)
            m_interactionsCoordinated = interactions.GetComponent<Interaction>();
    }

    void Update()
    {

        if (isDataObject())
        {
            if (printEvents) print(Time.deltaTime + "  Single pick");
            if (!m_currentDataSelect)
            {
                m_currentDataSelect = m_myHand.getDataFromIndex();
                m_currentDialog = Instantiate(descriptionDialog, m_currentDataSelect.transform.position,
                    Quaternion.LookRotation(transform.position - GameObject.FindGameObjectWithTag("MainCamera").transform.position));
                m_currentDialog.GetComponent<InitDescriptionCanvas>().SetGameObjectToFollow(m_currentDataSelect.transform);
                //m_currentDialog.transform.SetParent(m_currentDataSelect.transform);
                m_currentDialog.GetComponentsInChildren<Text>()[0].text = m_currentDataSelect.Name_1;
                m_currentDialog.GetComponentsInChildren<Text>()[1].text = m_currentDataSelect.Name_2;
                m_currentDialog.GetComponentsInChildren<Text>()[2].text = m_currentDataSelect.Name_3;
                m_currentDialog.GetComponentsInChildren<Text>()[3].text = m_currentDataSelect.Name_4;
                return;
            }

            if (m_currentDataSelect != m_myHand.getDataFromIndex() && m_currentDialog)
            {
                Destroy(m_currentDialog);
                m_previousData = m_currentDataSelect;
                m_currentDataSelect = m_myHand.getDataFromIndex();
                m_currentDialog = Instantiate(descriptionDialog, m_currentDataSelect.transform.position,
                    Quaternion.LookRotation(transform.position - GameObject.FindGameObjectWithTag("MainCamera").transform.position));
                //m_currentDialog.transform.SetParent(m_currentDataSelect.transform);
                m_currentDialog.GetComponent<InitDescriptionCanvas>().SetGameObjectToFollow(m_currentDataSelect.transform);
                m_currentDialog.GetComponentsInChildren<Text>()[0].text = m_currentDataSelect.Name_1;
                m_currentDialog.GetComponentsInChildren<Text>()[1].text = m_currentDataSelect.Name_2;
                m_currentDialog.GetComponentsInChildren<Text>()[2].text = m_currentDataSelect.Name_3;
                m_currentDialog.GetComponentsInChildren<Text>()[3].text = m_currentDataSelect.Name_4;
                return;
            }

            if (SteamVR_Actions._default.GrabGrip.GetStateDown(m_Pose.inputSource)  && m_currentDataSelect)
            {
                if (printEvents) print(Time.deltaTime + " Double push Data Object");
                //m_interactionsCoordinated.ToogleDataParents(m_currentDataSelect);
                m_interactionsCoordinated.FilterData(m_currentDataSelect, m_currentDataSelect.m_currentSubpace.GetInstanceID().ToString());
                m_stateSelect = false;
                m_audioSource.PlayOneShot(DoubleSelectAudio, 1f);
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
        if (m_myHand.getDataFromIndex() && (m_myHand.getDataFromIndex().gameObject.CompareTag("DataScatterplot")
            || m_myHand.getDataFromIndex().gameObject.CompareTag("DataBarchart")))
            return true;
        return false;
    }

    public void CleanDescriptionDialog()
    {
        if (m_currentDialog)
            Destroy(m_currentDialog);
        m_currentDataSelect = null;
    }
   
}
