using UnityEngine;
using System.Collections;

public class PoopScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PoopButton()
	{
		StartCoroutine(DoPoop());
	}

	IEnumerator DoPoop()
	{
		GameObject go = new GameObject("poop");
		go.transform.position = new Vector3(0, 0, -1);

		TextMesh mesh = go.AddComponent<TextMesh>();
		mesh.font = (Font)Resources.Load("Fonts/Lobster/Lobster");
		mesh.anchor = TextAnchor.MiddleCenter;
		mesh.text = "poop";
		mesh.fontSize = 100;

		go.renderer.material = mesh.font.material;

		yield return new WaitForSeconds(2);

		Destroy(go);
	}
}
