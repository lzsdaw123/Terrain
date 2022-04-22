using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public GameObject play;
    public float distance;
    // Start is called before the first frame update
    void Start()
    {
        //play = GameObject.Find("POPP").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        distance = (play.transform.position - transform.position).magnitude / 3.5f;

        if (distance <= 2f)  //Ä²µo¶ZÂ÷ ­ì1.5f
        {
            ExitGame();
        }
    }
    public void ExitGame()
    {
        Settings.pause();
        Settings.ExitGame();
    }
}
