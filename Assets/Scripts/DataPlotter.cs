using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataPlotter : MonoBehaviour
{

    public string inputfile;

    private List<Dictionary<string, object>> pointList;

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

    public int columnSelect = 13;

    public string subtitleName_1;
    public string subtitleName_2;
    public string subtitleName_3;
    public string subtitleName_4;

    private string idName;
    private string xName;
    private string yName;
    private string zName;
    private string parentsName;
    private string colorRName;
    private string colorGName;
    private string colorBName;
    private string nameFirst;
    private string nameSecond;
    private string nameThird;
    private string nameFourth;
    private string dataSelected;

    public float plotScale = 1;

    public GameObject PointPrefab;
    public GameObject PointHolder;
    public GameObject interactions;

    public Material material_data;

    private Interaction m_interactionsCoordinated = null;

    void Start()
    {
        if (interactions)
            m_interactionsCoordinated = interactions.GetComponent<Interaction>();
        pointList = CSVReader.Read(inputfile);
        List<string> columnList = new List<string>(pointList[1].Keys);

        idName = columnList[columnID];
        xName = columnList[columnX];
        yName = columnList[columnY];
        zName = columnList[columnZ];
        parentsName = columnList[columnParents];
        colorRName = columnList[colorR];
        colorGName = columnList[colorG];
        colorBName = columnList[colorB];
        if (name_1 > 0)
            nameFirst = columnList[name_1];
        if (name_2 > 0)
            nameSecond = columnList[name_2];
        if (name_3 > 0)
            nameThird = columnList[name_3];
        if (name_4 > 0)
            nameFourth = columnList[name_4];
        if (columnSelect > 0)
        dataSelected = columnList[columnSelect];

        float xMax = FindMaxValue(xName);
        float yMax = FindMaxValue(yName);
        float zMax = FindMaxValue(zName);
        
        float xMin = FindMinValue(xName);
        float yMin = FindMinValue(yName);
        float zMin = FindMinValue(zName);

        float midX = (xMax - xMin) / 2;
        float midY = (yMax - yMin) / 2;
        float midZ = (zMax - zMin) / 2;
       
        Transform subspace = transform.parent;
        Quaternion localrotation = subspace.localRotation;
        subspace.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        float scaleSubspace = subspace.localScale.x /2f;


        for (var i = 0; i < pointList.Count; i++)
        {
            /** Positions **/
            float x = (System.Convert.ToSingle(pointList[i][xName]) - xMin) * scaleSubspace * 2f/ (xMax - xMin);
            x += subspace.localPosition.x - scaleSubspace;
            float y = (System.Convert.ToSingle(pointList[i][yName]) - yMin) * scaleSubspace * 2f / (yMax - yMin);
            y += subspace.localPosition.y - scaleSubspace;
            float z = (System.Convert.ToSingle(pointList[i][zName]) - zMin)  * scaleSubspace * 2f / (zMax - zMin);
            z += subspace.localPosition.z - scaleSubspace; 

            GameObject dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
            dataPoint.transform.SetParent(PointHolder.transform);
            dataPoint.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            string dataPointName = pointList[i][idName].ToString();
            dataPoint.transform.name = dataPointName;

            /** Color**/
            float color_R = System.Convert.ToSingle(pointList[i][colorRName]) / 255f;
            float color_G = System.Convert.ToSingle(pointList[i][colorGName]) / 255f;
            float color_B = System.Convert.ToSingle(pointList[i][colorBName]) / 255f;
            if (columnSelect > 0 && pointList[i][dataSelected].ToString().Equals("1"))
            {
                material_data.color = new Color(color_R, color_G, color_B, 1f);
                dataPoint.GetComponent<Data>().is_selected = true;
            }
                
            else
                material_data.color = new Color(color_R, color_G, color_B, Constants.TRANSPARENCY_DATA);
            dataPoint.GetComponent<Renderer>().material = new Material(material_data);
            dataPoint.GetComponent<Data>().Id = System.Convert.ToInt32(pointList[i][idName]);
            if (name_1 > 0)
                dataPoint.GetComponent<Data>().Name_1 = subtitleName_1 + " " + pointList[i][nameFirst].ToString();
            if (name_2 > 0)
                dataPoint.GetComponent<Data>().Name_2 = subtitleName_2 + " " + pointList[i][nameSecond].ToString();
            if (name_3 > 0)
                dataPoint.GetComponent<Data>().Name_3 = subtitleName_3 + " " + pointList[i][nameThird].ToString();
            if (name_4 > 0)
                dataPoint.GetComponent<Data>().Name_4 = subtitleName_4 + " " + pointList[i][nameFourth].ToString();
            dataPoint.GetComponent<Data>().CustomColor = new Color(color_R, color_G, color_B, material_data.color.a);
            dataPoint.GetComponent<Data>().m_currentSubpace = subspace.GetComponent<Subspace>();
            if (m_interactionsCoordinated)
            {
                string parent_list = pointList[i][parentsName].ToString();
                if (parent_list.Length > 0)
                {
                    foreach(string parent in parent_list.Split('-'))
                    {
                        m_interactionsCoordinated.InsertData(dataPointName, parent);
                    }
                }
                
            }
            
        }
        subspace.localRotation = localrotation;

        this.enabled = false;
    }

    private float FindMaxValue(string columnName)
    {
        float maxValue = Convert.ToSingle(pointList[0][columnName]);
        for (var i = 0; i < pointList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(pointList[i][columnName]))
                maxValue = Convert.ToSingle(pointList[i][columnName]);
        }

        return maxValue;
    }

    private float FindMinValue(string columnName)
    {

        float minValue = Convert.ToSingle(pointList[0][columnName]);
        for (var i = 0; i < pointList.Count; i++)
        {
            if (Convert.ToSingle(pointList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(pointList[i][columnName]);
        }

        return minValue;
    }

}