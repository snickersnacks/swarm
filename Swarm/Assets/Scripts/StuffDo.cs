using UnityEngine;
using System.Collections;

public class StuffDo : MonoBehaviour {

	Transform T;
	Camera C;
	public float ZoomSpeed = 0.25f;
	public float MoveSpeed = 0.25f;
	int _releaseCount = 0;
	int _releaseTime = 5;
	public int ReleaseTotal = 60;
	void Start()
	{
		T = this.transform;
		C = this.gameObject.GetComponent<Camera>();
	}
	void Update ()
	{
		Vector3 MouPos = Camera.main.ScreenToViewportPoint (Input.mousePosition);
		Vector3 ThePos = this.transform.position;
		
		float NewSiz = C.orthographicSize;
		if (MouPos.x > 0f && MouPos.x < 1f && MouPos.y > 0f && MouPos.y < 1f)
		{
			if (MouPos.x >= 0.99f) { ThePos.x += MoveSpeed; }
			else if (MouPos.x <= 0.01f) { ThePos.x -= MoveSpeed; }
			else if (MouPos.x >= 0.975f) { ThePos.x += MoveSpeed * 0.5f; }
			else if (MouPos.x <= 0.025f) { ThePos.x -= MoveSpeed * 0.5f; }
			
			if (MouPos.y >= 0.99f) { ThePos.y += MoveSpeed; }
			else if (MouPos.y <= 0.01f) { ThePos.y -= MoveSpeed; }
			else if (MouPos.y >= 0.975f) { ThePos.y += MoveSpeed * 0.5f; }
			else if (MouPos.y <= 0.025f) { ThePos.y -= MoveSpeed * 0.5f; }
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
				GameObject G = GameObject.Find ("NODE_1");
				GameObject P = (GameObject)GameObject.Instantiate (Resources.Load ("Prefabs/Armor Walking"), G.transform.position, G.transform.rotation);
				Vector3 NS = new Vector3(0.25f, 0.25f, 0f);
				P.transform.localScale = NS;
				
				_releaseCount -= 1;
				_releaseTime = 15;
			}
		}
	}
}
