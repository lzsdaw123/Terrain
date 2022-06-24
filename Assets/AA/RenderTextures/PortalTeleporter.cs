using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour {

	public Transform player;
	public Transform reciever;
	public GameObject[] Portal;

	private bool playerIsOverlapping = false;
	public float dotProduct;
	public bool coolDown;
	public float cTime;
	
	public Light LightA;
	public Light LightB;
	public int Type;

	void Start()
    {
		cTime = -1;
		if (player == null)
		{
			player = Save_Across_Scene.Play.transform;
		}
	}
    // Update is called once per frame
    void Update () {
		if (playerIsOverlapping)
		{
			Vector3 portalToPlayer = player.position - transform.position;
			dotProduct = Vector3.Dot(transform.up, portalToPlayer);
			// 如果這是真的：玩家已經穿過傳送門
			if (dotProduct < 0f)
			{
                // 傳送他！
                float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
                rotationDiff += 180;
                player.Rotate(Vector3.up, rotationDiff);
				if(LightA !=null && LightB != null)
				{
					switch (Type)
					{
						case 0:  //降B燈
							LightB.intensity = 4320000;
							LightA.intensity = 3.072e+07f;
							break;
						case 1:  //降A燈
							LightA.intensity = 4320000;
							LightB.intensity = 3.072e+07f;
							break;
					}
				}

				Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
				player.gameObject.GetComponent<PlayerMove>().enabled = false;
				player.gameObject.GetComponent<Shooting>().enabled = false;
                player.gameObject.SetActive(false);
                player.position = reciever.position + positionOffset;
				//reciever.gameObject.GetComponent<BoxCollider>().enabled = true;
				player.gameObject.GetComponent<PlayerMove>().enabled = true;
				player.gameObject.GetComponent<Shooting>().enabled = true;
                player.gameObject.SetActive(true);
				playerIsOverlapping = false;
            }			
		}
		if (cTime >= 0)
		{
			cTime += Time.deltaTime;
		}
		if (cTime >= 20)
		{
			cTime = -1;
			gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player")
		{
			playerIsOverlapping = true;
            if (coolDown)
            {
				coolDown = false;
				cTime = 0;
			}
			//reciever.gameObject.GetComponent<BoxCollider>().enabled = false;
			//Portal[0].GetComponent<BoxCollider>().enabled = false;
			//Portal[1].GetComponent<BoxCollider>().enabled = true;
        }
	}

	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Player")
		{
			playerIsOverlapping = false;
			//reciever.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
	}
}
