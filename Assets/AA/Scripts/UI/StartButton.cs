using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Texture2D NoButton;
    public Texture2D Button;
    public GameObject UI;

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //UI.GetComponent<RawImage>().texture = NoButton;
    }    
    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.GetComponent<RawImage>().texture = Button;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        UI.GetComponent<RawImage>().texture = Button;
    }
}
