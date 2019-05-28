using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitMenuCanvas : MonoBehaviour
{
    public GameObject Head;

    public bool modeHand = true; // true : macro - false : micro

    public GameObject buttonMode;

    void Start()
    {
        Head = GameObject.FindGameObjectWithTag("MainCamera");
        if (transform.parent)
            transform.position = transform.parent.position;

        //ChangeModeMenu(true);
;

    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Head.transform.position);
    }

    public void ChangeModeMenu(bool isModeMacro)
    {
        modeHand = isModeMacro;
        /**if (modeHand)
        {
            buttonMode.GetComponent<TextMesh>().text = Constants.HAND_MODE_MACRO;
            buttonMode.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            buttonMode.GetComponent<TextMesh>().text = Constants.HAND_MODE_MICRO;
            buttonMode.GetComponent<Renderer>().material.color = Color.blue;
        }**/
    }
}
