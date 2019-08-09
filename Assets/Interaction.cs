using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    private Dictionary<string, List<string>> m_parents = new Dictionary<string, List<string>>();
    public bool setEmptyColorWhenSelectData = true;
    // Start is called before the first frame update

    public bool InsertData(string data, string parent)
    {
        if (!m_parents.ContainsKey(data))
        {
            m_parents.Add(data, new List<string>());
        }
        m_parents[data].Add(parent);
        return true;
    }

    public List<string> GetParents(string key)
    {
        if (m_parents.ContainsKey(key))
            return m_parents[key];
        return null;
    }

    public bool ToogleDataParents(Data data)
    {
        bool state = data.ToogleSelectData();
        if (!enabled) return state; 
        if (m_parents.ContainsKey(data.Id.ToString()))
        { 
            foreach (string d in m_parents[data.Id.ToString()])
            {
                GameObject.Find(d).GetComponent<Data>().ChangeSelectData(state, data.customColor);
            }
        }
        return state;
    }

}
