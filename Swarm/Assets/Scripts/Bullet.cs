using UnityEngine;
using System.Collections;
using AssemblyCSharp;
public class Bullet : MonoBehaviour {

	Vector3 currentTarget;
	float speed = 0.25f;
	bool GoGo = false;
	int LivesFor = 1500;
	Vector3 Line;
	void Start () {
	
	}
	
	void Update ()
	{
		if (GoGo)
		{
			if (this.transform.position != currentTarget)
			{
				this.transform.position = Vector3.MoveTowards (this.transform.position, currentTarget, speed);
			}
			else
			{
				currentTarget += Line;
			}
		}
		if (LivesFor > 0)
		{
			LivesFor -= 1;
		}
		else
		{
			Destroy (this.gameObject);
		}
	}
	void OnTriggerEnter2D(Collider2D Col)
	{
		if (Col.name != "Tower")
		{
			Destroy (this.gameObject);
		}
	}
	public void GoGoGo(Vector3 GoHere)
	{
		GoHere.z = this.transform.position.z;
		Line = Funcs.GetLineDifference (this.transform.position, GoHere);
		currentTarget = GoHere;
		this.transform.rotation = Quaternion.LookRotation(Vector3.forward, currentTarget - this.transform.position);
		GoGo = true;
	}
}
