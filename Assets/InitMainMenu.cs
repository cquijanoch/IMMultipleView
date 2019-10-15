using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class InitMainMenu : MonoBehaviour
{
    private int m_currentScene = 0;
    public int m_trainNumber = -1;
    public int m_taskNumber = -1;
    public float m_totalTime = 0f;
    public float m_totalTrainTime = 0f;
    public float m_totalTaskTime = 0f;

    public GameObject m_textUserId;

    public GameObject m_buttonTrain1;
    public GameObject m_buttonTrain2;

    public GameObject m_buttonTask1;
    public GameObject m_buttonTask2;
    public GameObject m_buttonTask3;

    public GameObject m_textTotalTime;
    public GameObject m_textTrainTime;
    public GameObject m_textTaskTime;

    public GameObject m_toogle;

    public GameObject m_StartPosition;
    public GameObject m_PlayerVR;
    public GameObject m_PlayerFPS;
    private GameObject m_Player;


    public bool VR;

    private void Awake()
    {  
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(m_StartPosition);
        
    }
    void Start()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
    }

    void Update()
    {
        if (m_trainNumber > 0 || m_taskNumber > 0)
            m_totalTime += Time.deltaTime;

        if (m_trainNumber > 0)
            m_totalTrainTime += Time.deltaTime;

        if (m_taskNumber > 0)
            m_totalTaskTime += Time.deltaTime;

        m_textTotalTime.GetComponent<Text>().text = m_totalTime.ToString();
        m_textTrainTime.GetComponent<Text>().text = m_totalTrainTime.ToString();
        m_textTaskTime.GetComponent<Text>().text = m_totalTaskTime.ToString();
    }

    public void StartTraining()
    {
        Train_1();
    }

    public void Train_1()
    {
        m_taskNumber = -1;
        CleanColorButton();
        m_buttonTrain1.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 1;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_trainNumber = 1;
    }

    public void Train_2()
    {
        m_taskNumber = - 1;
        CleanColorButton();
        m_buttonTrain2.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 2;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_trainNumber = 2;
    }

    public void StartTask()
    {
        Task_1();
    }

    public void Task_1()
    {
        m_trainNumber = -1;
        CleanColorButton();
        m_buttonTask1.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 3;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_taskNumber = 1;
    }

    private void CleanColorButton()
    {
        m_buttonTrain1.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonTrain2.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonTask1.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonTask2.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonTask3.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
    }

    public void StopTrain()
    {
        m_trainNumber = -1;
    }

    public void StopTask()
    {
        m_taskNumber = -1;
    }

    public void ResetValues()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 0;
        SceneManager.LoadScene(0);
        m_trainNumber = -1;
        m_taskNumber = -1;
        m_totalTime = 0f;
        m_totalTrainTime = 0f;
        m_totalTaskTime = 0f;
        m_textUserId.GetComponent<InputField>().text = "";
        CleanColorButton();

    }

    public void CreatePlayer()
    {
        VR = m_toogle.GetComponent<Toggle>().isOn;
        if (VR)
            m_Player = Instantiate(m_PlayerVR, m_StartPosition.transform.position, m_StartPosition.transform.rotation);
        else
            m_Player =Instantiate(m_PlayerFPS, m_StartPosition.transform.position, m_StartPosition.transform.rotation);
    }
}
