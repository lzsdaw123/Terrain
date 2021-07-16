using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  //AI

public class AITest : MonoBehaviour
{
	public NavMeshAgent agent; //尋找代理人
	public Transform target;  //目標的座標

    void Start()
    {
		
    }

    void Update()
    {
		agent.destination = target.position; //設尋徑目標
	}
}
