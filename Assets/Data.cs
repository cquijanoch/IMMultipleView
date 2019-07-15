
using UnityEngine;

public class Data : MonoBehaviour
{
    private int id;
    private string name_1;
    private string name_2;
    private string name_3;
    private string name_4;
    public bool is_selected = false;
    public Color customColor;
    public Subspace m_currentSubpace;
    private Material m_material;

    private void Start()
    {
        m_material = GetComponent<Renderer>().material;
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
            newMaterial.color = Constants.COLOR_DATA_OBJECT_SELECTED;
            gameObject.GetComponent<Renderer>().material = newMaterial;
            is_selected = true;
            m_currentSubpace.selectedData.Add(this);
        }
        else
        {
            newMaterial.color = customColor;
            gameObject.GetComponent<Renderer>().material = newMaterial;
            is_selected = false;
            m_currentSubpace.selectedData.Remove(this);
        }
        return is_selected;
    }

    public bool ChangeSelectData(bool state)
    {
        if (state == is_selected) return is_selected;
        Material newMaterial = new Material(m_material);
        if (state)
        {
            newMaterial.color = Constants.COLOR_DATA_OBJECT_SELECTED;
            gameObject.GetComponent<Renderer>().material = newMaterial;
            is_selected = true;
            m_currentSubpace.selectedData.Add(this);
        }
        else
        {
            newMaterial.color = customColor;
            gameObject.GetComponent<Renderer>().material = newMaterial;
            is_selected = false;
            m_currentSubpace.selectedData.Remove(this);
        }
        return is_selected;
    }

}
