using UnityEngine;
using System.Collections;

public class TowerTargeting : MonoBehaviour {
	
	Transform _me;
	Collider2D _target = null;
	public float RotateSpeed = 5f;
	public int FireSpeed = 30;
	int FireCount = 0;
	bool HasTarget = false;
	bool FacingTarget = false;
	void Start ()
	{
		_me = this.transform;
		FireCount = 0;
	}
	
	void Update ()
	{
		if (_target != null)
		{
			if (FacingTarget)
			{
				this.transform.rotation = Quaternion.LookRotation(Vector3.forward, _target.transform.position - this.transform.position);
				if (FireCount <= 0)
				{
					GameObject P = (GameObject)GameObject.Instantiate (Resources.Load ("Prefabs/Bullet"), _me.position, _me.rotation);
					P.GetComponent<Bullet>().GoGoGo (_target.transform.position);
					FireCount = FireSpeed;
				}
				else { FireCount -= 1; }
			}
			else
			{
				
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D Col)
	{
		if (!HasTarget)
		{
			_target = Col;
			HasTarget = true;
			FacingTarget = true;
		}
	}
	void OnTriggerExit2D(Collider2D Col)
	{
		if (Col == _target)
		{
			HasTarget = false;
			_target = null;
			FacingTarget = false;
		}
	}
}
