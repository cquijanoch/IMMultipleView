using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BarPlotter : MonoBehaviour
{
    public string inputfile;

    private List<Dictionary<string, object>> barList;

    public int columnX = 0;
    public int columnY = 1;

    public string xName;
    public string yName;
    public float plotScale = 1;

    public GameObject BarPrefab;
    public GameObject PointHolder;

    public bool createAxisXLabel = false;
    public GameObject AxisXLabel;

    public bool createAxisYLabel = false;
    public GameObject AxisYLabel;
    public GameObject AxisYLine;
    public float characterAxisLabelSize = 6f;
    public int numLinesAxisY = 10;

    void Start()
    {
        barList = CSVReader.Read(inputfile);
        List<string> columnList = new List<string>(barList[1].Keys);

        xName = columnList[columnX];
        yName = columnList[columnY];

        float dataMax = FindMaxValue(yName);
        float dataMin = FindMinValue(yName);

        Transform subspace = transform.parent;
        Quaternion localrotation = subspace.localRotation;
        subspace.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        float characterSize = (1f / (float)characterAxisLabelSize/20f);
        if (characterSize > 0.1f)
            characterSize = 0.1f; 

        for (var i = 0; i < barList.Count; i++)
        {
            //Bar plot
            GameObject bar = Instantiate(BarPrefab) as GameObject;
            bar.transform.SetParent(PointHolder.transform);

            float height = (Convert.ToSingle(barList[i][yName]) - dataMin) / (dataMax - dataMin);
            float posX = 0.1f + subspace.localPosition.x - subspace.localScale.x /2f + i * 0.1f;
            float relativeHeight = height * subspace.localScale.y;

            bar.transform.SetPositionAndRotation(new Vector3(posX, subspace.localPosition.y - (subspace.localScale.y - relativeHeight) /2f, subspace.localPosition.z), Quaternion.identity);

            Vector3 barScale = new Vector3(0.05f, height, 0.05f);
            bar.transform.localScale = barScale;
            string dataPointName = barList[i][xName] + "";
            bar.transform.name = dataPointName;
            
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
                
                var YLabelValue = line;
                YLineLabel.GetComponent<TextMesh>().text = YLabelValue.ToString("0,0");
                YLineLabel.GetComponent<TextMesh>().characterSize = characterSize;
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
