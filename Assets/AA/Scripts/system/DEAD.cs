using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DEAD : MonoBehaviour
{
    public bool Dead;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Dead = HeroLife.Dead;
        if (Dead)
        {
            
            GetComponent<Text>().text = "You Dead";
        }
    }
}
