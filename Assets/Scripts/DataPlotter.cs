using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataPlotter : MonoBehaviour
{

    public string inputfile;

    private List<Dictionary<string, object>> pointList;

    public int columnX = 0;
    public int columnY = 1;
    public int columnZ = 2;

    public string xName;
    public string yName;
    public string zName;

    public float plotScale = 1;

    public GameObject PointPrefab;
    public GameObject PointHolder;

    public Color color;

    void Start()
    {
        pointList = CSVReader.Read(inputfile);
        List<string> columnList = new List<string>(pointList[1].Keys);

        xName = columnList[columnX];
        yName = columnList[columnY];
        zName = columnList[columnZ];
        
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
            float x = (System.Convert.ToSingle(pointList[i][xName]) - xMin) * scaleSubspace * 2f/ (xMax - xMin);
            x += subspace.localPosition.x - scaleSubspace; //- midX / (xMax - xMin);
            float y = (System.Convert.ToSingle(pointList[i][yName]) - yMin) * scaleSubspace * 2f / (yMax - yMin);
            y += subspace.localPosition.y - scaleSubspace; //- midY / (yMax - yMin);
            float z = (System.Convert.ToSingle(pointList[i][zName]) - zMin)  * scaleSubspace * 2f / (zMax - zMin);
            z += subspace.localPosition.z - scaleSubspace; //- midZ / (zMax - zMin);

            GameObject dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
            dataPoint.transform.SetParent(PointHolder.transform);
            dataPoint.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            string dataPointName = pointList[i][xName] + " " + pointList[i][yName] + " " + pointList[i][zName];
            dataPoint.transform.name = dataPointName;
            dataPoint.GetComponent<Renderer>().material.color = color;
            dataPoint.GetComponent<Data>().Name_1 = x.ToString();
            dataPoint.GetComponent<Data>().Name_2 = y.ToString();
            dataPoint.GetComponent<Data>().Name_3 = z.ToString();
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