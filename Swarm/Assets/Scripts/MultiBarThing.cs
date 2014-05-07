using UnityEngine;
using System.Collections;

public class MultiBarThing : MonoBehaviour 
{
	public static MultiBarThing i;

	public float BarMaxValue = 100f;
	public float[] BarValues = new float[8];
	public UITexture[] BarTextures = new UITexture[8];

	public float maxBarLength = 560;

	public UITable table;

	void Awake()
	{
		i = this;
	}

	void Update () 
	{
		float total = 0;

		for (int ctr = 0; ctr < BarValues.Length; ctr++)
		{
			total += BarValues[ctr];
			BarTextures[ctr].width = (int)((BarValues[ctr] / BarMaxValue) * maxBarLength);
		}
		table.Reposition();

		if (total != BarMaxValue)
			Debug.Log("error, sizes incorrect. " + BarMaxValue + " : " + total);
	}
}
