using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NextNode : MonoBehaviour {

	public List<GameObject> NODES = new List<GameObject>();
	public int NextTarget = 0;


	void Start()
	{
		//Remove the black circles from the NODEs
		this.renderer.enabled = false;

		
		if (this.particleSystem != null)
			LookAt();
	}

	private void LookAt()
	{
		if (NODES.Count == 0)
		{
			this.particleSystem.enableEmission = false;
			return;
		}

		Vector3 dir = GetNextTargetObject().transform.position - this.transform.position;
		float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public void SwitchNextTarget()
	{
		NextTarget += 1;

		if (NextTarget >= NODES.Count)
			NextTarget = 0;

		if (this.particleSystem != null)
			LookAt();
	}
	public GameObject GetNextTargetObject()
	{
		if (NODES.Count > NextTarget)
			return NODES[NextTarget];
		else
			return null;
	}
	void OnMouseUpAsButton()
	{
		Debug.Log ("CLICKED");
		SwitchNextTarget ();
	}
}
