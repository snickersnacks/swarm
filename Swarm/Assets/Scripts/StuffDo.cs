using UnityEngine;
using System.Collections;

public class StuffDo : MonoBehaviour 
{
	public Transform SwarmParent;

	Transform T;
	Camera C;
	public float ZoomSpeed = 0.25f;
	public float MoveSpeed = 0.25f;
	int _releaseCount = 60;
	int _releaseTime = 5;
	public int ReleaseTotal = 60;

	private GameObject walker;
	private GameObject node1;

	void Awake()
	{
		walker = (GameObject)Resources.Load ("Prefabs/Armor Walking");
	}

	void Start()
	{
		T = this.transform;
		C = this.gameObject.GetComponent<Camera>();
		
		node1 = GameObject.Find ("NODE_1");
	}

	void Update ()
	{
		Vector3 MouPos = Camera.main.ScreenToViewportPoint (Input.mousePosition);
		Vector3 ThePos = this.transform.position;
		//Debug.Log (Input.mousePresent);
		float NewSiz = C.orthographicSize;
		if (MouPos.x > 0f && MouPos.x < 1f && MouPos.y > 0f && MouPos.y < 1f)
		{
			if (MouPos.x >= 0.9f) { ThePos.x += MoveSpeed; }
			else if (MouPos.x <= 0.1f) { ThePos.x -= MoveSpeed; }
			else if (MouPos.x >= 0.8f) { ThePos.x += MoveSpeed * 0.5f; }
			else if (MouPos.x <= 0.2f) { ThePos.x -= MoveSpeed * 0.5f; }
			
			if (MouPos.y >= 0.9f) { ThePos.y += MoveSpeed; }
			else if (MouPos.y <= 0.1f) { ThePos.y -= MoveSpeed; }
			else if (MouPos.y >= 0.8f) { ThePos.y += MoveSpeed * 0.5f; }
			else if (MouPos.y <= 0.2f) { ThePos.y -= MoveSpeed * 0.5f; }
		}
		
		if (Input.GetKey (KeyCode.UpArrow)) { ThePos.y += MoveSpeed; }
		if (Input.GetKey (KeyCode.DownArrow)) { ThePos.y -= MoveSpeed; }
		if (Input.GetKey (KeyCode.RightArrow)) { ThePos.x += MoveSpeed; }
		if (Input.GetKey (KeyCode.LeftArrow)) { ThePos.x -= MoveSpeed; }
		if (Input.GetKey (KeyCode.Z)) { NewSiz += ZoomSpeed; }
		if (Input.GetKey (KeyCode.X)) { NewSiz -= ZoomSpeed; }

		NewSiz = Mathf.Clamp (NewSiz, 5f, 12f);
		T.position = ThePos;
		C.orthographicSize = NewSiz;

		if (Input.GetKeyUp (KeyCode.G)) { _releaseCount = 50; }

		if (_releaseCount > 0)
		{
			if (_releaseTime > 0)
			{
				_releaseTime -= 1;
			}
			else
			{
				GameObject P = (GameObject)GameObject.Instantiate (walker, node1.transform.position, node1.transform.rotation);
				//P.transform.parent = SwarmParent;

				Vector3 NS = new Vector3(0.25f, 0.25f, T.position.z);
				P.transform.localScale = NS;
				_releaseCount -= 1;
				_releaseTime = 15;
			}
		}
	}
}
