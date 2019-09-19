
using System.Collections.Generic;

public class DataObj
{
    private string id;
    private bool state;
    private string idParent;
    private List<string> parents;

    public DataObj() { }
    public string Id
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

    public bool State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
        }
    }

    public string IdParent
    {
        get
        {
            return idParent;
        }
        set
        {
            idParent = value;
        }
    }

    public List<string> Parents
    {
        get
        {
            return parents;
        }
        set
        {
            parents = new List<string>(value);
        }
    }

}
