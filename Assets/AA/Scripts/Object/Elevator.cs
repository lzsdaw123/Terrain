using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Animator Animator;
    public bool DownUp=true;
    public LayerMask LayerMask;
    GameObject play;
    public AudioSource AudioSource;

    void Start()
    {
        AudioSource.volume = 1.7f;
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
                AudioSource.Play();
            }
            else
            {
                Animator.SetTrigger("Up");
                AudioSource.Play();
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
