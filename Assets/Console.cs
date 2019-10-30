using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    private Queue<string> m_Inputs;
    public int m_MaxLines = 15;
    private Text m_ConsoleText;

    void Start()
    {
        m_Inputs = new Queue<string>();
        m_ConsoleText = GetComponentInChildren<Text>();
    }

    public void AddText(string newInput)
    {
        if (m_Inputs.Count >= m_MaxLines)
            m_Inputs.Dequeue();

        m_Inputs.Enqueue(newInput);
        UpdateText();
    }

    public void UpdateText()
    {
        m_ConsoleText.text = "";

        foreach (string obj in m_Inputs)
            m_ConsoleText.text += obj + "\n";
    }

    public void Clear()
    {
        m_Inputs.Clear();
        UpdateText();
    }
}
