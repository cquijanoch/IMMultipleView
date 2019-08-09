using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitTrainingCanvas : MonoBehaviour
{
    public bool Answer = false;
    public GameObject m_title;
    public string tittle = "Training";
    public int m_currentIndexScene = 0;
    public GameObject current_player;

    void Start()
    {
        Answer = false;
        m_title.GetComponent<Text>().text = tittle;
    }

    void Update()
    {
        
    }

    public void onClickYesButton()
    {
        print("Yes Button Clicked");
        Answer = true;
        Destroy(current_player);
        SceneManager.LoadScene(++m_currentIndexScene);
    }

    public void onClickNoButton()
    {
        print("No Button Clicked");
        Answer = false;
        Destroy(current_player);
        if (m_currentIndexScene == 0)
            SceneManager.LoadScene(m_currentIndexScene);
        else
            SceneManager.LoadScene(--m_currentIndexScene);
    }
}
