
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Interaction : MonoBehaviour
{
    private Dictionary<string, DataObj> m_parents = new Dictionary<string, DataObj>();//<name of Data, DataObj>
    private Dictionary<string, List<string>> m_filter = new Dictionary<string, List<string>>();//<Id of subspace, parents>
    private List<string> m_filterHistory = new List<string>();

    public Dictionary<string, int> versionSubspace = new Dictionary<string, int>();
    public bool setEmptyColorWhenSelectData = true;

    /** -Task- **/
    public int m_selectSingle = 0;
    public int m_brushing = 0;
    /** -Task- **/

    public bool InsertData(string data, string parent, string brother, bool state, string idContainer)
    {
        if (!m_parents.ContainsKey(data))
        {
            DataObj dataObj = new DataObj();
            dataObj.Id = data;
            dataObj.State = state;
            dataObj.Parents = new List<string>();
            dataObj.Brothers = new List<string>();
            dataObj.IdContainer = idContainer;
            m_parents.Add(data, dataObj);
        }
        m_parents[data].Parents.Add(parent);
        m_parents[data].Brothers.Add(brother);
        return true;
    }

    public bool InsertData(string data, string[] parents, string[] brothers, bool state, string idContainer)
    {
        if (!m_parents.ContainsKey(data))
        {
            DataObj dataObj = new DataObj();
            dataObj.Id = data;
            dataObj.State = state;
            dataObj.Parents = new List<string>(parents);
            dataObj.Brothers = new List<string>(brothers);
            dataObj.IdContainer = idContainer;
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
        if (m_parents.ContainsKey(data.Id.ToString()) || data.m_currentSubpace.m_letFilter)
            foreach (string d in m_parents[data.Id.ToString()].Parents)
                GameObject.Find(d).GetComponent<Data>().ChangeSelectData(state, data.customColor);
        return state;
    }

    public bool FilterData(Data data)
    {      
        bool state = data.ToogleSelectData();
        if (!enabled)
            return state;

        string id = data.Id.ToString();
        if (!m_parents.ContainsKey(id) || !data.m_currentSubpace.m_letFilter)
        {
            m_selectSingle++;
            return false;
        }

        if (m_parents[id].Parents.Count == 0)
        {
            m_selectSingle++;
            return state;
        }

        m_brushing++;

        UpdateFilterHistory(id, state);
        foreach (string idBrother in m_parents[id].Brothers)
        {
            GameObject dataGameObj = GameObject.Find(idBrother);
            if (dataGameObj)
            {
                bool stateBrother = dataGameObj.GetComponent<Data>().ToogleSelectData();
                UpdateFilterHistory(idBrother, stateBrother);
                SetFilterParameters(idBrother, stateBrother, m_parents[idBrother].IdContainer);
            }
                
        }

        SetFilterParameters(id, state, m_parents[id].IdContainer);

        ClearSelectData();
        if (m_filter.Count == 0)
        {
            SelectAll();
            return false;
        }
            
        foreach (string d in GetIntersectFilter())
        {
            if (m_parents.ContainsKey(d))
            {
                m_parents[d].State = state;
                GameObject dataGameObj = GameObject.Find(d);
                Data dataObj = null;
                if (dataGameObj)
                    dataObj = dataGameObj.GetComponent<Data>();
                if (dataObj)
                {
                    //if (dataObj.m_currentSubpace.m_letFilter)
                        dataObj.ChangeSelectData(true, dataObj.customColor);
                    //else
                    //    dataObj.ChangeSelectData(true, Constants.COLOR_DATA_OBJECT_SELECTED);
                }
            } 
        }
        return state;
    }

    private void UpdateFilterHistory(string idData, bool state)
    {
        if (m_parents[idData].Parents.Count > 0)
        {
            if (state)
                m_filterHistory.Add(idData);
            else
                m_filterHistory.Remove(idData);
        }
    }

    private bool SetFilterParameters(string idData, bool state, string filterContainer)
    {
        if (!state)
        {
            if (!m_filter.ContainsKey(filterContainer))
                return false;
            foreach (string element in m_parents[idData].Parents)
                if (m_filter[filterContainer].Contains(element))
                    m_filter[filterContainer].Remove(element);
            if (m_filter[filterContainer].Count == 0)
                m_filter.Remove(filterContainer);
        }
        else
        {
            if (m_filter.ContainsKey(filterContainer))
                m_filter[filterContainer].AddRange(new List<string>(m_parents[idData].Parents));
            else
                m_filter.Add(filterContainer, new List<string>(m_parents[idData].Parents));
        }
        return true;
    }

    private List<string> GetIntersectFilter()
    {
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
        return filterComparer;
    }

    private List<string> GetParentsWithParents(string id)
    {
        List<string> dataWithParents = new List<string>();
        foreach (string parent in m_parents[id].Parents)
            if (m_parents[parent].Parents.Count > 0)
                dataWithParents.Add(parent);
        return dataWithParents;
    }

    private void ClearSelectData()
    {
        foreach (string d in new List<string>(m_parents.Keys))
        {
            if (!m_filterHistory.Contains(d))
            {
                m_parents[d].State = false;
                Data dataObj = GameObject.Find(d).GetComponent<Data>();
                if (dataObj.m_currentSubpace.m_letFilter)
                    dataObj.ChangeSelectData(false, dataObj.customColor);
                else
                    dataObj.ChangeSelectData(false, dataObj.customColor, true);
            }    
        }
    }

    private void RemoveListfromList(ref List<string> list, List<string> list2)
    {
        foreach(string element in list2)
            if (list.Contains(element))
                list.Remove(element);
    }

    private void SelectAll()
    {
        foreach (string d in new List<string>(m_parents.Keys))
        {
            m_parents[d].State = false;
            Data dataObj = GameObject.Find(d).GetComponent<Data>();
            dataObj.ChangeSelectData(false, dataObj.customColor, false);
        }
    }
}
