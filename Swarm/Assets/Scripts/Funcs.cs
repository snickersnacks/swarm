using System;
using System.Collections;
using UnityEngine;

namespace AssemblyCSharp
{
	public class Funcs
	{
		public static Vector3 GetLineDifference(Vector3 PointOne, Vector3 PointTwo)
		{
			Vector3 A = Vector3.zero;
			A.x = PointTwo.x - PointOne.x;
			A.y = PointTwo.y - PointOne.y;
			A.z = 0f;
			//Debug.Log ("P1:" + PointOne + "\r\nP2:" + PointTwo + "\r\nPR:" + A);
			return A;
		}
	}
}

