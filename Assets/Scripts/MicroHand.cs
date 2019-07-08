using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroHand : MonoBehaviour
{
    // Start is called before the first frame update

    public List<Data> selectedData;

    private float m_quantitySelectData = float.MaxValue;
    private Data m_currentDataSelect = null;


    public bool printEvents = false;
    private Hand m_myHand;

    void Start()
    {
        m_myHand = GetComponent<Hand>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_quantitySelectData < float.MaxValue)
            m_quantitySelectData += Time.deltaTime;

        //if (printEvents) print(Time.deltaTime + " " + "OnTriggerEnter : " + m_myHand.getDataFromIndex().gameObject.name);

        if (isDataObject() && m_quantitySelectData > Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            if (printEvents) print(Time.deltaTime + "  Single pick");
            m_quantitySelectData = 0;
            m_currentDataSelect = m_myHand.getDataFromIndex();
            return;
        }

        if (isDataObject() && m_myHand.getDataFromIndex() == m_currentDataSelect && m_quantitySelectData < Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            if (printEvents) print(Time.deltaTime + "  Double pick");
            m_quantitySelectData = float.MaxValue;
            m_myHand.getDataFromIndex().GetComponent<Renderer>().material.color = Color.green;
        }

    }

    private bool isDataObject()
    {
        if (!m_myHand.getDataFromIndex() || !m_myHand.getDataFromIndex().gameObject.CompareTag("DataScatterplot"))
            return false;
        return true;
    }

   
}
