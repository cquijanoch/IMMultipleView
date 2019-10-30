using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BarPlotter : MonoBehaviour
{
    private List<Dictionary<string, object>> m_barList;
    private string m_idName;
    private string m_xName;
    private string m_yName;
    private string m_parentsName;
    private string m_brotherssName;
    private string m_colorRName;
    private string m_colorGName;
    private string m_colorBName;
    private string m_nameFirst;
    private string m_nameSecond;
    private string m_nameThird;
    private Interaction m_interactionsCoordinated = null;

    public string inputfile;    
    public int columID = 0;
    public int columnX = 1;
    public int columnY = 2;
    public int colorR = 3;
    public int colorG = 4;
    public int colorB = 5;
    public int columnParents = 6;
    public int name_1 = 7;
    public int name_2 = 8;
    public int name_3 = 9;
    public int columnBrothers = 10;
    public string subtitleName_1;
    public string subtitleName_2;
    public string subtitleName_3;

    public float plotScale = 1;
    public GameObject BarPrefab;
    public GameObject PointHolder;
    public bool createAxisXLabel = false;
    public GameObject AxisXLabel;
    public bool createAxisYLabel = false;
    public GameObject AxisYLabel;
    public GameObject AxisYLine;
    public float characterAxisLabelSize = 4f;
    public float characterYLabelSize = 0.8f;
    public int numLinesAxisY = 10;
    //public int factorLineY = 1;
    public float factorWidthBar = 0.04f;
    public float factorPositionXBar = 0.05f;
    public GameObject Interactions;
    public Material material_data;

    private void Awake()
    {
        GameObject mainTask = GameObject.Find("MainMenu");
        if (mainTask && mainTask.GetComponent<InitMainMenu>().m_versionDataset == 1)
            inputfile = inputfile + "B";
    }

    void Start()
    {
        if (Interactions)
            m_interactionsCoordinated = Interactions.GetComponent<Interaction>();
        m_barList = CSVReader.Read(inputfile);
        List<string> columnList = new List<string>(m_barList[1].Keys);
        m_idName = columnList[columID];
        m_xName = columnList[columnX];
        m_yName = columnList[columnY];
        m_colorRName = columnList[colorR];
        m_colorGName = columnList[colorG];
        m_colorBName = columnList[colorB];

        if (name_1 > 0) m_nameFirst = columnList[name_1];
        if (name_2 > 0) m_nameSecond = columnList[name_2];
        if (name_3 > 0) m_nameThird = columnList[name_3];
        if (columnParents > 0) m_parentsName = columnList[columnParents];
        if (columnBrothers > 0) m_brotherssName = columnList[columnBrothers];

        float dataMax = FindMaxValue(m_yName);
        float dataMin = FindMinValue(m_yName) - 1;

        Transform subspace = transform.parent;
        Quaternion localrotation = subspace.localRotation;
        subspace.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        float characterSize = (1f / (float)characterAxisLabelSize/20f);
        if (characterSize > 0.8f)
            characterSize = 0.8f; 

        for (var i = 0; i < m_barList.Count; i++)
        {
            //Bar plot
            GameObject bar = Instantiate(BarPrefab) as GameObject;
            bar.transform.SetParent(PointHolder.transform);

            float height = (Convert.ToSingle(m_barList[i][m_yName]) - dataMin) / (dataMax - dataMin);
            float posX = 0.05f + subspace.localPosition.x - subspace.localScale.x /2f + i * factorPositionXBar;
            float relativeHeight = height * subspace.localScale.y;

            bar.transform.SetPositionAndRotation(new Vector3(posX,
                subspace.localPosition.y - (subspace.localScale.y - relativeHeight) /2f,
                subspace.localPosition.z),
                Quaternion.identity);

            Vector3 barScale = new Vector3(factorWidthBar, height, 0.04f);
            bar.transform.localScale = barScale;
            string dataPointName = m_barList[i][m_idName] + "";
            bar.transform.name = dataPointName;

            float color_R = System.Convert.ToSingle(m_barList[i][m_colorRName]) / 255f;
            float color_G = System.Convert.ToSingle(m_barList[i][m_colorGName]) / 255f;
            float color_B = System.Convert.ToSingle(m_barList[i][m_colorBName]) / 255f;

            material_data.color = new Color(color_R, color_G, color_B, Constants.COLOR_UNSELECT_A_COLOR);
            bar.GetComponent<Renderer>().material = new Material(material_data);
            bar.GetComponent<Data>().Id = System.Convert.ToInt32(m_barList[i][m_idName]);
            if (name_1 > 0) bar.GetComponent<Data>().Name_1 = subtitleName_1 + " " + m_barList[i][m_nameFirst].ToString();
            if (name_2 > 0) bar.GetComponent<Data>().Name_2 = subtitleName_2 + " " + m_barList[i][m_nameSecond].ToString();
            if (name_3 > 0) bar.GetComponent<Data>().Name_3 = subtitleName_3 + " " + m_barList[i][m_nameThird].ToString();

            bar.GetComponent<Data>().CustomColor = new Color(color_R, color_G, color_B, Constants.COLOR_UNSELECT_A_COLOR);
            bar.GetComponent<Data>().m_currentSubpace = subspace.GetComponent<Subspace>();
            if (m_interactionsCoordinated)
            {
                string parent_list = "";
                string brother_list = "";
                if (columnParents > 0)
                    parent_list = m_barList[i][m_parentsName].ToString();
                if (columnBrothers > 0)
                    brother_list = m_barList[i][m_brotherssName].ToString();
                m_interactionsCoordinated.InsertData(dataPointName,
                    parent_list.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries),
                    brother_list.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries),
                    bar.GetComponent<Data>().is_selected,
                    subspace.GetInstanceID().ToString());
            }

            if (createAxisXLabel)
            {
                GameObject newLabel = Instantiate(AxisXLabel,
                    new Vector3(posX, subspace.localPosition.y - subspace.localScale.y / 2f,
                    subspace.localPosition.z - 0.05f),
                    Quaternion.Euler(0, 180, 0)) as GameObject;
                
                newLabel.GetComponent<TextMesh>().text = "  " + m_barList[i][m_xName].ToString();
                newLabel.GetComponent<TextMesh>().characterSize = characterSize;
                newLabel.transform.Rotate(90f, 0, -90f);
                newLabel.transform.SetParent(PointHolder.transform);
            }
        }

        //AxiYlabel
        if (createAxisYLabel)
        {
            float posInitY = subspace.localPosition.y - subspace.localScale.y / 2f;
            float YLineSeparation = (float)(subspace.localScale.y) / numLinesAxisY;
            
            for ( int line = 0; line < numLinesAxisY; line++)
            {
                GameObject YLine = Instantiate(AxisYLine,
                    new Vector3(subspace.localPosition.x - subspace.localScale.x / 2f,
                    posInitY + YLineSeparation * line, subspace.localPosition.z),
                    Quaternion.Euler(0, 90, 0)) as GameObject;

                GameObject YLineLabel = Instantiate(AxisYLabel,
                    new Vector3(subspace.localPosition.x - subspace.localScale.x / 2f,
                    posInitY +  YLineSeparation * line,
                    subspace.localPosition.z), 
                    Quaternion.Euler(0, -90, 0)) as GameObject;

                var YLabelValue = dataMin + line * (dataMax - dataMin) / numLinesAxisY;//factorLineY;
                YLineLabel.GetComponent<TextMesh>().text = YLabelValue.ToString("0,0");
                YLineLabel.GetComponent<TextMesh>().characterSize = characterYLabelSize;
                YLine.transform.SetParent(PointHolder.transform);
                YLineLabel.transform.SetParent(PointHolder.transform);
            }
        }
        subspace.localRotation = localrotation;
        this.enabled = false;
    }

    private float FindMinValue(string columnName)
    {
        float minValue = Convert.ToSingle(m_barList[0][columnName]);
        for (var i = 0; i < m_barList.Count; i++)
        {
            if (Convert.ToSingle(m_barList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(m_barList[i][columnName]);
        }
        return minValue;
    }

    private float FindMaxValue(string columnName)
    {
        float maxValue = Convert.ToSingle(m_barList[0][columnName]);
        for (var i = 0; i < m_barList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(m_barList[i][columnName]))
                maxValue = Convert.ToSingle(m_barList[i][columnName]);
        }
        return maxValue;
    }
}
