using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    private Dictionary<string, List<string>> m_parents = new Dictionary<string, List<string>>();
    public Dictionary<string, bool> m_totalData = new Dictionary<string, bool>();
    public Dictionary<string, List<string>> m_filter = new Dictionary<string, List<string>>();
    public bool setEmptyColorWhenSelectData = true;

    private void Start()
    {
        print("init");
    }

    // Start is called before the first frame update

    public bool InsertData(string data, string parent, bool state)
    {
        if (!m_parents.ContainsKey(data))
        {
            m_parents.Add(data, new List<string>());
            m_totalData.Add(data, state);
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

    public bool FilterData(Data data)
    {
        bool state = data.ToogleSelectData();
        if (!enabled) return state;
        string id = data.Id.ToString();
        if (!m_parents.ContainsKey(id))
            return false;

        if (!state)
            m_filter.Remove(id);
        else
            m_filter.Add(id, m_parents[id]);

        List<string> filterComparer = new List<string>();
        int i = 0;
        foreach (List<string> filterer in m_filter.Values)
        {
            if (i == 0)
                filterComparer = new List<string>(filterer);
            else
                filterComparer = new List<string>(filterComparer.Intersect(filterer));
            i++;
        }
        ClearSelectData();
        foreach (string d in filterComparer)
        {
            m_totalData[d] = state;
            GameObject.Find(d).GetComponent<Data>().ChangeSelectData(state, Constants.COLOR_DATA_OBJECT_SELECTED);
        }

        return state;
    }

    private void ClearSelectData()
    {
        foreach (string d in m_totalData.Keys)
        {
            m_totalData[d] = false;
            GameObject.Find(d).GetComponent<Data>().ChangeSelectData(false, Constants.COLOR_DATA_OBJECT_SELECTED);
        }
    }

}
