using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitMainMenu : MonoBehaviour
{
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
    //private int m_currentIndexScene = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
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
        cleanColorButton();
        m_buttonTrain1.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        SceneManager.LoadScene(1);
        m_trainNumber = 1;
    }

    public void Train_2()
    {
        m_taskNumber = - 1;
        cleanColorButton();
        m_buttonTrain2.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        SceneManager.LoadScene(2);
        m_trainNumber = 2;
    }

    public void StartTask()
    {
        Task_1();
    }

    public void Task_1()
    {
        m_trainNumber = -1;
        cleanColorButton();
        m_buttonTask1.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        SceneManager.LoadScene(3);
        m_taskNumber = 1;
    }

    private void cleanColorButton()
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
        m_trainNumber = -1;
        m_taskNumber = -1;
        m_totalTime = 0f;
        m_totalTrainTime = 0f;
        m_totalTaskTime = 0f;
        m_textUserId.GetComponent<InputField>().text = "";
        cleanColorButton();
        Destroy(GameObject.FindGameObjectWithTag("Player"));
    }
}
