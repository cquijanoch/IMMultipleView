
using System.Collections.Generic;

public class DataObj
{
    private string id;
    private bool state;
    private List<string> brothers;
    private string idContainer;
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

    public List<string> Brothers
    {
        get
        {
            return brothers;
        }
        set
        {
            brothers = new List<string>(value);
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

    public string IdContainer
    {
        get
        {
            return idContainer;
        }
        set
        {
            idContainer = value;
        }
    }

}
