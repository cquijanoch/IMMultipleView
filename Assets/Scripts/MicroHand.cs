using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroHand : MonoBehaviour
{
    // Start is called before the first frame update

    public List<Data> selectedData;

    private float m_quantitySelectData = float.MaxValue;
    private bool m_FlagToSelectData = false;

    public bool printEvents = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_FlagToSelectData && m_quantitySelectData < float.MaxValue)
            m_quantitySelectData += Time.deltaTime;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("DataScatterplot"))
            return;
        if (printEvents)  print(Time.deltaTime + " " + "OnTriggerEnter : " + other.gameObject.name);

        if (m_quantitySelectData > Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            if (printEvents) print(Time.deltaTime + "  Single pick");
            m_quantitySelectData = 0;
            m_FlagToSelectData = true;
        }

        if (m_quantitySelectData < Constants.MINIMAL_TIME_PER_DOUBLE_TRIGGER)
        {
            if (printEvents) print(Time.deltaTime + "  Double pick");
            m_FlagToSelectData = false;
            m_quantitySelectData = float.MaxValue;
            
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("DataScatterplot"))
            return;
        print(Time.deltaTime + " " + "OnTriggerExit : " + other.gameObject.name);
        
    }
}
