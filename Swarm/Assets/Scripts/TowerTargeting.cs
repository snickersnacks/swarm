using UnityEngine;
using System.Collections;

public class TowerTargeting : MonoBehaviour 
{
	public Transform ActualTower;
	
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
			//Debug.Log ("T Health: " + _target.GetComponent <AnimTest>().HealthPoints);
			if (FacingTarget)
			{
				this.transform.rotation = Quaternion.LookRotation(Vector3.forward, _target.transform.position + Vector3.forward - this.transform.position);
				if (FireCount <= 0)
				{
					Vector3 OffSetTargeting = Vector3.zero;
					OffSetTargeting.y = Random.Range (-0.3f, 0.3f);
					OffSetTargeting.x = Random.Range (-0.3f, 0.3f);
					GameObject P = (GameObject)GameObject.Instantiate (Resources.Load ("Prefabs/Bullet"), _me.position, _me.rotation);
					P.GetComponent<Bullet>().GoGoGo (_target.transform.position + OffSetTargeting);
					FireCount = FireSpeed;
				}
				else { FireCount -= 1; }
			}
			else
			{
				
			}
		}
		else
		{
			HasTarget = false;
		}
	}
	
	void OnTriggerStay2D(Collider2D Col)
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

	public float GetRange()
	{
		return this.GetComponent<CircleCollider2D>().radius;
	}

	public float GetTowerRadius()
	{
		Collider2D col = ActualTower.GetComponent<Collider2D>();

		if (col is CircleCollider2D)
			return ((CircleCollider2D)col).radius * ActualTower.localScale.x;
		else if (col is BoxCollider2D)
			return ((BoxCollider2D)col).size.x * ActualTower.localScale.x;

		Debug.Log("Error: ActualTower collider of odd type.");
		return 1;
	}
}
