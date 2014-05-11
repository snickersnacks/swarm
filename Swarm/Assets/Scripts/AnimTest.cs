using UnityEngine;
using System.Collections;

public class AnimTest : MonoBehaviour {

	public float _moveSpeed = 0.025f;
	public float _rotateSpeed = 6f;
	public int HealthPoints = 5;
	float _myLayer = 0f;
	Transform _me;
	GameObject NextTarget;
	bool Rotatin = true;
	Vector3 OffSet = Vector3.zero;
	void Start()
	{
		OffSet.y = Random.Range (-0.30f, 0.30f);
		OffSet.x = Random.Range (-0.30f, 0.30f);
		_me = this.transform;
		_myLayer = _me.position.z;
		NextTarget = GameObject.Find ("NODE_1");
	}
	void Update ()
	{
		if (HealthPoints <= 0)
		{
			Destroy(this.gameObject);
		}
		Vector3 NV = _me.position;
		Vector3 NP = NextTarget.transform.position + OffSet;
		NP.z = _myLayer;
		Quaternion RV = _me.transform.rotation;
		if (NV.x != NP.x|| NV.y != NP.y)
		{
			if (Rotatin)
			{
				Quaternion Q = Quaternion.LookRotation (Vector3.forward, NP - NV);
				RV = Quaternion.RotateTowards (RV, Q, _rotateSpeed);
				if (_me.rotation == Q)
				{
					Rotatin = false;
				}
			}
			NV = Vector3.MoveTowards (NV, NP, _moveSpeed);
		}
		else
		{
			NextTarget = NextTarget.GetComponent<NextNode>().GetNextTargetObject ();
			if (NextTarget == null)
			{
				Destroy(this.gameObject);
			}
			Rotatin = true;
		}
		_me.position = NV;
		_me.transform.rotation = RV;
	}
}
