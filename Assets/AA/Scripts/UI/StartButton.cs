using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Texture2D NoButton;
    public Texture2D Button;
    public GameObject UI;
    bool Down=false;
    [SerializeField] GameObject PP;
    GameObject GB;
    [SerializeField]public static bool SceneBool=false;

    void Awake()
    {
        if (SceneManager.sceneCount == 1 && !SceneBool )
        {
            SceneBool = true;
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }
    }
    void Start()
    {
        GB = this.gameObject;
        PP.SetActive(false);
    }
    void Update()
    {
        if (UI.GetComponent<RawImage>().texture == NoButton)
        {
            PP.SetActive(false);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Down)
        {
            UI.GetComponent<RawImage>().texture = NoButton;
        }        
    }    
    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.GetComponent<RawImage>().texture = Button;
        if (GB.name == "StartB")
        {
            PP.transform.position = new Vector3(-194.996f,-82.4f,242);
        }
        else if (GB.name == "OptionB")
        {
            PP.transform.position = new Vector3(-193f, -100f, 242);
        }
        else if (GB.name == "QuitB")
        {
            PP.transform.position = new Vector3(-191f, -114.2f, 242);
        }
        PP.SetActive(true);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        UI.GetComponent<RawImage>().texture = Button;
        Down = true;
    }
}
