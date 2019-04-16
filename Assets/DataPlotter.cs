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

    void Start()
    {
        Debug.Log("Coordenada: " + PointHolder.transform.position.x + " " + PointHolder.transform.position.y + " " + PointHolder.transform.position.z);
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
        /**float xMax = 0.5f;
        float yMax = 0.5f;
        float zMax = 0.5f;

        float xMin = 0f;
        float yMin = 0f;
        float zMin = 0f;**/
        //PointHolder.transform.localPosition = PointHolder.transform.parent.position;


        for (var i = 0; i < pointList.Count; i++)
        {
            float x = (System.Convert.ToSingle(pointList[i][xName]) - xMin) / (xMax - xMin);
            x += PointHolder.transform.position.x;
            float y = (System.Convert.ToSingle(pointList[i][yName]) - yMin) / (yMax - yMin);
            y += PointHolder.transform.position.y;
            float z = (System.Convert.ToSingle(pointList[i][zName]) - zMin) / (zMax - zMin);
            z += PointHolder.transform.position.z;

            //Debug.Log("Coordenada: " + x + " " + y + " " + z);
            GameObject dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
            dataPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //dataPoint.transform.SetParent(PointHolder.transform);
            string dataPointName = pointList[i][xName] + " " + pointList[i][yName] + " " + pointList[i][zName];
            dataPoint.transform.name = dataPointName;
            //Debug.Log("GameObjetc: " + dataPoint.transform.position.x + " " + dataPoint.transform.position.y + " " + dataPoint.transform.position.z);
            // Gets material color and sets it to a new RGB color we define
            dataPoint.GetComponent<Renderer>().material.color =
                new Color(x, y, z, 1.0f);
        }

        Debug.Log("Coordenada: " + PointHolder.transform.position.x + " " + PointHolder.transform.position.y + " " + PointHolder.transform.position.z);
        

        
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