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


    void Start()
    {
        barList = CSVReader.Read(inputfile);
        List<string> columnList = new List<string>(barList[1].Keys);

        xName = columnList[columnX];
        yName = columnList[columnY];

        float dataMax = FindMaxValue(yName);
        float dataMin = FindMinValue(yName);

        Transform subspace = transform.parent;

        for (var i = 0; i < barList.Count; i++)
        {
            float height = (Convert.ToSingle(barList[i][yName]) - dataMin) * subspace.localScale.y/(dataMax - dataMin);
            float posX = 0.1f + subspace.localPosition.x - subspace.localScale.x/2f + i * 0.1f;
            print(posX);
            GameObject bar = Instantiate(BarPrefab, new Vector3(posX, subspace.localPosition.y - subspace.localScale.y/2f + height/2f , subspace.localPosition.z), Quaternion.identity) as GameObject;
            Vector3 barScale = new Vector3(0.05f, height, 0.05f);
            bar.transform.localScale = barScale;
            bar.transform.SetParent(PointHolder.transform);
            string dataPointName = barList[i][xName] + "";
            bar.transform.name = dataPointName;

        }
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
