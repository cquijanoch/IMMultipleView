﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BarPlotter : MonoBehaviour
{
    public string inputfile;

    private List<Dictionary<string, object>> barList;

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
    public string subtitleName_1;
    public string subtitleName_2;
    public string subtitleName_3;

    private string idName;
    private string xName;
    private string yName;
    private string parentsName;
    private string colorRName;
    private string colorGName;
    private string colorBName;
    private string nameFirst;
    private string nameSecond;
    private string nameThird;

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
    public int factorLineY = 1;

    public float factorWidthBar = 0.04f;
    public float factorPositionXBar = 0.05f;

    public GameObject Interactions;

    private Interaction m_interactionsCoordinated = null;

    public Material material_data;

    void Start()
    {
        if (Interactions)
            m_interactionsCoordinated = Interactions.GetComponent<Interaction>();
        barList = CSVReader.Read(inputfile);
        List<string> columnList = new List<string>(barList[1].Keys);
        idName = columnList[columID];
        xName = columnList[columnX];
        yName = columnList[columnY];
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

        float dataMax = FindMaxValue(yName);
        float dataMin = FindMinValue(yName) - 1;

        Transform subspace = transform.parent;
        Quaternion localrotation = subspace.localRotation;
        subspace.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        float characterSize = (1f / (float)characterAxisLabelSize/20f);
        if (characterSize > 0.8f)
            characterSize = 0.8f; 

        for (var i = 0; i < barList.Count; i++)
        {
            //Bar plot
            GameObject bar = Instantiate(BarPrefab) as GameObject;
            bar.transform.SetParent(PointHolder.transform);

            float height = (Convert.ToSingle(barList[i][yName]) - dataMin) / (dataMax - dataMin);
            float posX = 0.05f + subspace.localPosition.x - subspace.localScale.x /2f + i * factorPositionXBar;
            float relativeHeight = height * subspace.localScale.y;

            bar.transform.SetPositionAndRotation(new Vector3(posX, subspace.localPosition.y - (subspace.localScale.y - relativeHeight) /2f, subspace.localPosition.z), Quaternion.identity);

            Vector3 barScale = new Vector3(factorWidthBar, height, 0.04f);
            bar.transform.localScale = barScale;
            string dataPointName = barList[i][idName] + "";
            bar.transform.name = dataPointName;
            float color_R = System.Convert.ToSingle(barList[i][colorRName]) / 255f;
            float color_G = System.Convert.ToSingle(barList[i][colorGName]) / 255f;
            float color_B = System.Convert.ToSingle(barList[i][colorBName]) / 255f;
            material_data.color = new Color(color_R, color_G, color_B, Constants.TRANSPARENCY_DATA);
            bar.GetComponent<Renderer>().material = new Material(material_data);
            bar.GetComponent<Data>().Id = System.Convert.ToInt32(barList[i][idName]);
            if (name_1 > 0)
                bar.GetComponent<Data>().Name_1 = subtitleName_1 + " " + barList[i][nameFirst].ToString();
            if (name_2 > 0)
                bar.GetComponent<Data>().Name_2 = subtitleName_2 + " " + barList[i][nameSecond].ToString();
            if (name_3 > 0)
                bar.GetComponent<Data>().Name_3 = subtitleName_3 + " " + barList[i][nameThird].ToString();

            bar.GetComponent<Data>().CustomColor = new Color(color_R, color_G, color_B, Constants.TRANSPARENCY_DATA);
            bar.GetComponent<Data>().m_currentSubpace = subspace.GetComponent<Subspace>();
            if (m_interactionsCoordinated)
            {
                string parent_list = barList[i][parentsName].ToString();
                if (parent_list.Length > 0)
                {
                    foreach (string parent in parent_list.Split('-'))
                        m_interactionsCoordinated.InsertData(dataPointName, parent, bar.GetComponent<Data>().is_selected);
                }
            }

            if (createAxisXLabel)
            {
                GameObject newLabel = Instantiate(AxisXLabel, new Vector3(posX, subspace.localPosition.y - subspace.localScale.y / 2f, subspace.localPosition.z - 0.05f), Quaternion.Euler(0, 180, 0)) as GameObject;
                
                newLabel.GetComponent<TextMesh>().text = "  " + barList[i][xName].ToString();
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
                GameObject YLine = Instantiate(AxisYLine, new Vector3(subspace.localPosition.x - subspace.localScale.x / 2f, posInitY + YLineSeparation * line, subspace.localPosition.z), Quaternion.Euler(0, 90, 0)) as GameObject;
                
                GameObject YLineLabel = Instantiate(AxisYLabel, new Vector3(subspace.localPosition.x - subspace.localScale.x / 2f, posInitY +  YLineSeparation * line, subspace.localPosition.z), Quaternion.Euler(0, -90, 0)) as GameObject;
                
                var YLabelValue = line * factorLineY;
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

        float minValue = Convert.ToSingle(barList[0][columnName]);
        for (var i = 0; i < barList.Count; i++)
        {
            if (Convert.ToSingle(barList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(barList[i][columnName]);
        }

        return minValue;
    }

    private float FindMaxValue(string columnName)
    {
        float maxValue = Convert.ToSingle(barList[0][columnName]);
        for (var i = 0; i < barList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(barList[i][columnName]))
                maxValue = Convert.ToSingle(barList[i][columnName]);
        }

        return maxValue;
    }
}
