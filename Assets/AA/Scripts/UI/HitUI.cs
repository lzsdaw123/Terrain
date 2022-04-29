using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitUI : MonoBehaviour
{
    public float HitUITime;
    public Color UIcolor;
    public float speed=10; 

    void Start()
    {
        gameObject.SetActive(false);
        HitUITime = 0;
    }

    void Update()
    {
        HitUITime += Time.deltaTime;
        UIcolor = GetComponent<Image>().color;
        transform.localScale += new Vector3(1f, 1f, 0f) * speed * Time.deltaTime;

        if (UIcolor == Color.white)  //命中
        {
            if (transform.localScale.x >= 0.85)
            {
                transform.localScale = new Vector3(0.85f, 0.85f, 1f);
            }
            if (HitUITime >= 0.2f)
            {
                gameObject.SetActive(false);
                transform.localScale = new Vector3(0f, 0f, 1f);
                HitUITime = 0;
            }
        }
        if (UIcolor == Color.red)  //擊殺
        {
            if (transform.localScale.x >= 1.5)
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            }
            if (HitUITime >= 0.3f)
            {
                gameObject.SetActive(false);
                transform.localScale = new Vector3(0f, 0f, 1f);
                HitUITime = 0;
            }
        }
    }
}
