using System.Collections.Generic;
using UnityEngine;
using System;

public class DataPlotter : MonoBehaviour
{
    private List<Dictionary<string, object>> m_pointList;
    private string m_idName;
    private string m_xName;
    private string m_yName;
    private string m_zName;
    private string m_parentsName;
    private string m_colorRName;
    private string m_colorGName;
    private string m_colorBName;
    private string m_nameFirst;
    private string m_nameSecond;
    private string m_nameThird;
    private string m_nameFourth;
    private string m_dataSelected;
    private string m_brothersName;
    private Interaction m_interactionsCoordinated = null;

    public string inputfile;
    public int columnID = 0;
    public int columnX = 1;
    public int columnY = 2;
    public int columnZ = 3;
    public int colorR = 4;
    public int colorG = 5;
    public int colorB = 6;
    public int columnParents = 7;
    public int name_1 = 7;
    public int name_2 = 8;
    public int name_3 = 9;
    public int name_4 = 10;
    public int columnBrothers = 12;
    public int columnSelect = 13;
    public string subtitleName_1;
    public string subtitleName_2;
    public string subtitleName_3;
    public string subtitleName_4;
    public float plotScale = 1;
    public GameObject pointPrefab;
    public GameObject pointHolder;
    public GameObject interactions;
    public Material material_data;

    void Start()
    {
        if (interactions)
            m_interactionsCoordinated = interactions.GetComponent<Interaction>();
        m_pointList = CSVReader.Read(inputfile);
        List<string> columnList = new List<string>(m_pointList[1].Keys);

        m_idName = columnList[columnID];
        m_xName = columnList[columnX];
        m_yName = columnList[columnY];
        m_zName = columnList[columnZ];
        m_parentsName = columnList[columnParents];
        m_colorRName = columnList[colorR];
        m_colorGName = columnList[colorG];
        m_colorBName = columnList[colorB];

        if (name_1 > 0) m_nameFirst = columnList[name_1];
        if (name_2 > 0) m_nameSecond = columnList[name_2];
        if (name_3 > 0) m_nameThird = columnList[name_3];
        if (name_4 > 0) m_nameFourth = columnList[name_4];
        if (columnBrothers > 0) m_brothersName = columnList[columnBrothers];
        if (columnSelect > 0) m_dataSelected = columnList[columnSelect];

        float xMax = FindMaxValue(m_xName);
        float yMax = FindMaxValue(m_yName);
        float zMax = FindMaxValue(m_zName);
        
        float xMin = FindMinValue(m_xName);
        float yMin = FindMinValue(m_yName);
        float zMin = FindMinValue(m_zName);

        float midX = (xMax - xMin) / 2;
        float midY = (yMax - yMin) / 2;
        float midZ = (zMax - zMin) / 2;
       
        Transform subspace = transform.parent;
        Quaternion localrotation = subspace.localRotation;
        subspace.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        float scaleSubspace = subspace.localScale.x /2f;

        for (var i = 0; i < m_pointList.Count; i++)
        {
            /** Positions **/
            float x = (System.Convert.ToSingle(m_pointList[i][m_xName]) - xMin) * scaleSubspace * 2f/ (xMax - xMin);
            x += subspace.localPosition.x - scaleSubspace;
            float y = (System.Convert.ToSingle(m_pointList[i][m_yName]) - yMin) * scaleSubspace * 2f / (yMax - yMin);
            y += subspace.localPosition.y - scaleSubspace;
            float z = (System.Convert.ToSingle(m_pointList[i][m_zName]) - zMin)  * scaleSubspace * 2f / (zMax - zMin);
            z += subspace.localPosition.z - scaleSubspace; 

            GameObject dataPoint = Instantiate(pointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
            dataPoint.transform.SetParent(pointHolder.transform);
            dataPoint.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            string dataPointName = m_pointList[i][m_idName].ToString();
            dataPoint.transform.name = dataPointName;

            /** Color**/
            float color_R = System.Convert.ToSingle(m_pointList[i][m_colorRName]) / 255f;
            float color_G = System.Convert.ToSingle(m_pointList[i][m_colorGName]) / 255f;
            float color_B = System.Convert.ToSingle(m_pointList[i][m_colorBName]) / 255f;
            if (columnSelect > 0 && m_pointList[i][m_dataSelected].ToString().Equals(Constants.CSV_DATA_SELECTED))
            {
                material_data.color = new Color(color_R, color_G, color_B, Constants.COLOR_SELECT_A_COLOR);
                dataPoint.GetComponent<Data>().is_selected = true;
            }   
            else
                material_data.color = new Color(color_R, color_G, color_B, Constants.COLOR_UNSELECT_A_COLOR);
            dataPoint.GetComponent<Renderer>().material = new Material(material_data);
            dataPoint.GetComponent<Data>().Id = System.Convert.ToInt32(m_pointList[i][m_idName]);

            if (name_1 > 0) dataPoint.GetComponent<Data>().Name_1 = subtitleName_1 + " " + m_pointList[i][m_nameFirst].ToString();
            if (name_2 > 0) dataPoint.GetComponent<Data>().Name_2 = subtitleName_2 + " " + m_pointList[i][m_nameSecond].ToString();
            if (name_3 > 0) dataPoint.GetComponent<Data>().Name_3 = subtitleName_3 + " " + m_pointList[i][m_nameThird].ToString();
            if (name_4 > 0) dataPoint.GetComponent<Data>().Name_4 = subtitleName_4 + " " + m_pointList[i][m_nameFourth].ToString();

            dataPoint.GetComponent<Data>().CustomColor = new Color(color_R, color_G, color_B, material_data.color.a);
            dataPoint.GetComponent<Data>().m_currentSubpace = subspace.GetComponent<Subspace>();
            if (m_interactionsCoordinated)
            {
                string brother_list = "";
                string parent_list = "";

                if (columnBrothers > 0)
                    brother_list = m_pointList[i][m_brothersName].ToString();
                if (columnParents > 0)
                    parent_list = m_pointList[i][m_parentsName].ToString();
                
                m_interactionsCoordinated.InsertData(dataPointName,
                    parent_list.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries),
                    brother_list.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries),
                    dataPoint.GetComponent<Data>().is_selected,
                    subspace.GetInstanceID().ToString());
            }
        }
        subspace.localRotation = localrotation;
        this.enabled = false;
    }

    private float FindMaxValue(string columnName)
    {
        float maxValue = Convert.ToSingle(m_pointList[0][columnName]);
        for (var i = 0; i < m_pointList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(m_pointList[i][columnName]))
                maxValue = Convert.ToSingle(m_pointList[i][columnName]);
        }
        return maxValue;
    }

    private float FindMinValue(string columnName)
    {
        float minValue = Convert.ToSingle(m_pointList[0][columnName]);
        for (var i = 0; i < m_pointList.Count; i++)
        {
            if (Convert.ToSingle(m_pointList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(m_pointList[i][columnName]);
        }
        return minValue;
    }

}