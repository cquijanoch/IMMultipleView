using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Record
{
    StreamWriter fUsersActions;
    StreamWriter fPiecesState;
    StreamWriter fResumed;
    StreamWriter fTaskResults;
    StreamWriter fUserPostion;
    string header;

    public Record()
    {

    }

    public void Log(string result, string filename)
    {
        //fTaskResults = File.CreateText(Application.persistentDataPath+"/Experiments/UserIDTaskIDDataSetIDVR.csv");
        fTaskResults = File.CreateText(Application.persistentDataPath + "/Experiments/" + filename + ".csv");
        header = "UserID, TaskID, Version, Scenario, VR, Time(sec), Clones, Delete accepted, Delete canceled, Select Subspaces(clicks)," +
            " Select Single, Brushing(clicks)," +
            " APickup(sec), ARotation(sec), ATraslation(sec), AScale(sec), ANavSlaving(sec)," +
            " BPickup(sec), BRotation(sec), BTraslation(sec), BScale(sec), BNavSlaving(sec)," +
            " UserAnswer, CorrectAnswer";
        Debug.Log(Application.persistentDataPath);
        Debug.Log(header);
        Debug.Log(result);

        fTaskResults.WriteLine(header);
        fTaskResults.WriteLine(result);
        flush();
        close();

    }

    public void LogPosition(List<string> results, string filename)
    {
        fUserPostion = File.CreateText(Application.persistentDataPath + "/Experiments/" + filename + ".csv");
        header = "x, z, facing";
        fUserPostion.WriteLine(header);
        foreach (string row in results)
            fUserPostion.WriteLine(row);
        fUserPostion.Flush();
        fUserPostion.Close();

    }

    public void close()
    {
        /*fUsersActions.Close();
        fPiecesState.Close();
        fResumed.Close();*/
        fTaskResults.Close();
    }

    public void flush()
    {
        fTaskResults.Flush();/*
        fUsersActions.Flush();
        fPiecesState.Flush();
        fResumed.Flush();*/
    }
}
