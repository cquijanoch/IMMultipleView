
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Interaction : MonoBehaviour
{
    private Dictionary<string, DataObj> m_parents = new Dictionary<string, DataObj>();
    public Dictionary<string, List<string>> m_filter = new Dictionary<string, List<string>>();
    public bool setEmptyColorWhenSelectData = true;

    // Start is called before the first frame update

    public bool InsertData(string data, string parent, bool state, string idParent)
    {
        if (!m_parents.ContainsKey(data))
        {
            DataObj dataObj = new DataObj();
            dataObj.Id = data;
            dataObj.State = state;
            dataObj.Parents = new List<string>();
            dataObj.IdParent = idParent;
            m_parents.Add(data, dataObj);
        }
        m_parents[data].Parents.Add(parent);
        return true;
    }

    public bool InsertData(string data, string[] parents, bool state, string idParent)
    {
        if (!m_parents.ContainsKey(data))
        {
            DataObj dataObj = new DataObj();
            dataObj.Id = data;
            dataObj.State = state;
            dataObj.Parents = new List<string>(parents);
            dataObj.IdParent = idParent;
            m_parents.Add(data, dataObj);
        }
        return true;
    }

    public List<string> GetParents(string key)
    {
        if (m_parents.ContainsKey(key))
            return m_parents[key].Parents;
        return null;
    }

    public bool ToogleDataParents(Data data)
    {
        bool state = data.ToogleSelectData();
        if (!enabled) return state; 
        if (m_parents.ContainsKey(data.Id.ToString()))
        { 
            foreach (string d in m_parents[data.Id.ToString()].Parents)
            {
                GameObject.Find(d).GetComponent<Data>().ChangeSelectData(state, data.customColor);
            }
        }
        return state;
    }

    public bool FilterData(Data data, string filterType)
    {
        bool state = data.ToogleSelectData();
        if (!enabled) return state;
        string id = data.Id.ToString();
        if (!m_parents.ContainsKey(id))
            return false;

        if (!state)
        {
            if (!m_filter.ContainsKey(filterType))
                return false;
            foreach (string element in m_parents[id].Parents)
                if (m_filter[filterType].Contains(element))
                        m_filter[filterType].Remove(element);
            if (m_filter[filterType].Count == 0)
                m_filter.Remove(filterType);
        } 
        else
        {
            if (m_filter.ContainsKey(filterType))
                m_filter[filterType].AddRange(new List<string>(m_parents[id].Parents));
            else
                m_filter.Add(filterType, new List<string>(m_parents[id].Parents));
        }
            

        List<string> filterComparer = new List<string>();
        int i = 0;
        foreach (List<string> filterer in new List<List<string>>(m_filter.Values))
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
           m_parents[d].State = state;
            GameObject.Find(d).GetComponent<Data>().ChangeSelectData(true, Constants.COLOR_DATA_OBJECT_SELECTED);
        }

        return state;
    }

    private void ClearSelectData()
    {
        foreach (string d in new List<string>(m_parents.Keys))
        {
            m_parents[d].State = false;
            GameObject.Find(d).GetComponent<Data>().ChangeSelectData(false, Constants.COLOR_DATA_OBJECT_SELECTED);
        }
    }

    private void RemoveListfromList(ref List<string> list, List<string> list2)
    {
        foreach(string element in list2)
        {
            if (list.Contains(element))
                list.Remove(element);
        }
    }

}
