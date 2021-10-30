using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class StartButton : EventTrigger
{
    public Button StaButton;
    public Button OptButton;
    public Button QuitButton;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnPointerExit(PointerEventData OptButton)
    {
        base.OnPointerExit(OptButton);
        Debug.Log("離開" + this.gameObject.name);
    }
    public override void OnPointerEnter(PointerEventData OptButton)
    {
        base.OnPointerEnter(OptButton);
        Debug.Log("進入" + this.gameObject.name);
    }
    //public void OnPointerExit(PointerEventData OptButton)
    //{
    //    Debug.Log("離開");
    //}
    //public void OnPointerEnter(PointerEventData OptButton)
    //{
    //    Debug.Log("進入");
    //}
}
