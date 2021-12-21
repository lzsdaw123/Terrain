using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitUI : MonoBehaviour
{
    public float HitUITime;
    public Color UIcolor;

    void Start()
    {
        gameObject.SetActive(false);
        HitUITime = 0;
    }

    void Update()
    {
        HitUITime += Time.deltaTime;
        UIcolor = GetComponent<Image>().color;
        transform.localScale += new Vector3(1f, 1f, 0f) * 10 * Time.deltaTime;

        if (UIcolor == Color.white)
        {
            if (transform.localScale.x >= 0.8)
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
            if (HitUITime >= 0.2f)
            {
                gameObject.SetActive(false);
                transform.localScale = new Vector3(0f, 0f, 1f);
                HitUITime = 0;
            }
        }
        if (UIcolor == Color.red)
        {
            if (transform.localScale.x >= 1.4)
            {
                transform.localScale = new Vector3(1.4f, 1.4f, 1f);
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
