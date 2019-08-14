
using UnityEngine;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
    [SerializeField]
    private int id;
    [SerializeField]
    private string name_1;
    [SerializeField]
    private string name_2;
    [SerializeField]
    private string name_3;
    [SerializeField]
    private string name_4;
    public bool is_selected = false;
    public Color customColor;
    private Color m_currentColor;
    public Subspace m_currentSubpace;
    private Material m_material;
    private List<Color> m_colorList = new List<Color>();

    private void Start()
    {
        m_material = GetComponent<Renderer>().material;
        m_currentColor = m_material.color;
    }

    public int Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }

    public string Name_1
    {
        get
        {
            return name_1;
        }
        set
        {
            name_1 = value;
        }
    }
    
    public string Name_2
    {
        get
        {
            return name_2;
        }
        set
        {
            name_2 = value;
        }
    }
    
    public string Name_3
    {
        get
        {
            return name_3;
        }
        set
        {
            name_3 = value;
        }
    }
    
    public string Name_4
    {
        get
        {
            return name_4;
        }
        set
        {
            name_4 = value;
        }
    }

    public Color CustomColor
    {
        get
        {
            return customColor;
        }
        set
        {
            customColor = value;
        }
    }

    public bool ToogleSelectData()
    {
        Material newMaterial = new Material(m_material);
        if (!is_selected)
        {
            m_currentColor = new Color(newMaterial.color.r, newMaterial.color.g, newMaterial.color.b, 1f);
            m_colorList.Add(m_currentColor);
            newMaterial.color = m_currentColor;//Constants.COLOR_DATA_OBJECT_SELECTED;
            gameObject.GetComponent<Renderer>().material = newMaterial;
            is_selected = true;
            m_currentSubpace.selectedData.Add(this);
        }
        else
        {
            m_colorList.Remove(m_currentColor);
            if (m_colorList.Count > 0)
            {
                m_currentColor = m_colorList[m_colorList.Count - 1];
            }
            else
            {
                m_currentColor = customColor;
                is_selected = false;
            }
            newMaterial.color = m_currentColor;
            gameObject.GetComponent<Renderer>().material = newMaterial;
            m_currentSubpace.selectedData.Remove(this);
        }
        return is_selected;
    }

    public bool ChangeSelectData(bool state, Color color)
    {
        if (state == is_selected && m_currentColor.Equals(color)) return is_selected;
        Material newMaterial = new Material(m_material);
        if (state)
        {
            m_currentColor = new Color(color.r, color.g, color.b, 1f);
            m_colorList.Add(m_currentColor);
            newMaterial.color = m_currentColor;
            gameObject.GetComponent<Renderer>().material = newMaterial;
            is_selected = true;
            m_currentSubpace.selectedData.Add(this);
        }
        else
        {
            m_colorList.Remove(m_currentColor);
            if (m_colorList.Count > 0)
            {
                m_currentColor = m_colorList[m_colorList.Count - 1];
            }
            else
            {
                m_currentColor = customColor;
                is_selected = false;
            }
            newMaterial.color = m_currentColor;
            gameObject.GetComponent<Renderer>().material = newMaterial;
            m_currentSubpace.selectedData.Remove(this);
        }
        return is_selected;
    }

}
