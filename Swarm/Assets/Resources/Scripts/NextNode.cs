using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NextNode : MonoBehaviour {

	public List<GameObject> NODES = new List<GameObject>();
	int NextTarget = 0;
	void Start()
	{
		//Remove the black circles from the NODEs
		this.renderer.enabled = false;
	}
	public void SwitchNextTarget()
	{
		NextTarget += 1;
		if (NextTarget >= NODES.Count) { NextTarget = 0; }
	}
	public GameObject GetNextTargetObject()
	{
		return NODES[NextTarget];
	}
	void OnMouseUpAsButton()
	{
		Debug.Log ("CLICKED");
		SwitchNextTarget ();
	}
}
