using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Animator Animator;
    public bool DownUp=true;
    public LayerMask LayerMask;
    GameObject play;

    void Start()
    {
        
    }

    void Update()
    {

    }
    void Up()  //動畫
    {
        DownUp = true;
    }
    void Down()
    {
        DownUp = false;
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            play = collider.gameObject;
            EnterEV();
            if (DownUp)
            {
                Animator.SetTrigger("Down");
            }
            else
            {
                Animator.SetTrigger("Up");
            }
        }
    }void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Actor"))
        {
            play = collider.gameObject;
            ExitEV();
        }
    }
    void EnterEV()
    {
        //XX物件變成子物件
        play.transform.parent = gameObject.transform;
        play.GetComponent<PlayerMove>().enabled = false;
    }
    void ExitEV()
    {
        //子物件脫離父物件
        play.transform.parent = null;
        play.GetComponent<PlayerMove>().enabled = true;
    }
}
