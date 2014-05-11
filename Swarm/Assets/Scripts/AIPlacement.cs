using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

public class AIPlacement : MonoBehaviour 
{
	public static AIPlacement i;

	public Transform Tower;

	GameObject node1;

	public Vector2 min;
	public Vector2 max;
	public float step;
	
	public Dictionary<Vector2, float> positions;

	public Shader MapGradient;

	void Awake()
	{
		i = this;
	}

	void Start() 
	{
		node1 = GameObject.Find("NODE_1");
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.B))
		{
			StartCoroutine(DoThing());
		}
	}

	public IEnumerator DoThing()
	{
		TowerTargeting towertargeting = Tower.GetComponent<TowerTargeting>();
		yield return StartCoroutine(GetBestPosition(towertargeting.GetRange(), towertargeting.GetTowerRadius()));

		Vector2 best = positions.OrderByDescending(p => p.Value).First().Key;
		
		Transform tower = (Transform)GameObject.Instantiate(Tower);
		//go.transform.localScale = radius * Vector3.one;
		tower.transform.position = new Vector3(best.x, best.y, 0);
	}

	public IEnumerator GetBestPosition(float rangeRadius, float towerRadius)
	{
		positions = new Dictionary<Vector2, float>();

		GameObject towerGO = new GameObject("towerChecker");
		BoxCollider2D towerCollider = towerGO.AddComponent<BoxCollider2D>();
		towerCollider.size = Vector2.one * towerRadius;
		towerCollider.isTrigger = true;
		towerGO.layer = LayerMask.NameToLayer("PositioningAITower");

		Debug.Log("size: " + towerRadius);

		for (float x = min.x; x < max.x; x+=step)
		{
			for (float y = min.y; y < max.y; y+=step)
			{
				GameObject rangeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
				DestroyImmediate(rangeGO.GetComponent<Collider>());
				CircleCollider2D rangeCollider = rangeGO.AddComponent<CircleCollider2D>();
				rangeCollider.radius = rangeRadius;
				rangeCollider.isTrigger = true;
				rangeGO.layer = LayerMask.NameToLayer("PositioningAIRange");

				Vector2 currentpos = new Vector2(x, y);
				rangeGO.transform.position = new Vector3(x, y, 0);
				towerGO.transform.position = rangeGO.transform.position;

				float totaldist = distsdists(node1, currentpos, rangeRadius, towerRadius);

				if (totaldist > 0)
				{
					positions.Add(currentpos, totaldist);
					rangeGO.name = totaldist.ToString();
					rangeGO.renderer.material.shader = MapGradient;
					rangeGO.renderer.material.color = new Color(totaldist / 20f, 0, 0, 0.7f);
				}
				else
					Destroy(rangeGO);
			}

			yield return null;
		}
		
		//Destroy(rangeGO);
		Destroy(towerGO);
	}

	private float distsdists(GameObject OnNode, Vector2 testpos, float rangeRadius, float towerRadius)
	{
		float totaldist = raydist(OnNode, testpos, rangeRadius, towerRadius);
		NextNode nextnode = OnNode.GetComponent<NextNode>();

		foreach (GameObject nextGO in nextnode.NODES)
		{
			float dist = distsdists(nextGO, testpos, rangeRadius, towerRadius);

			if (dist == -1)
				return -1;
			else
				totaldist += dist;
		}

		return totaldist;
	}

	private float raydist(GameObject nodeGO, Vector2 testpos, float rangeRadius, float towerRadius)
	{
		Vector2 originnodepos = new Vector2(nodeGO.transform.position.x, nodeGO.transform.position.y);
		float dist = 0;

		NextNode nextnode = nodeGO.GetComponent<NextNode>();

		foreach (GameObject nextGO in nextnode.NODES)
		{
			Vector2 startpos = Vector2.zero;
			Vector2 endpos = Vector2.zero;
			Vector2 nextpos = new Vector2(nextGO.transform.position.x, nextGO.transform.position.y);

			if (isPointInCircle(testpos, towerRadius, originnodepos) || isPointInCircle(testpos, towerRadius, nextpos))
			{
				//Debug.Log("node inside tower");
				return -1; //node is inside tower
			}

			RaycastHit2D towerhit = Physics2D.Raycast(originnodepos, (nextpos - originnodepos).normalized, 
			                                          Vector2.Distance(originnodepos, nextpos), 1 << LayerMask.NameToLayer("PositioningAITower"));
			if (towerhit.collider != null)
			{
				//Debug.Log("tower on path");
				return -1; //tower is on the path
			}
			
			if (isPointInCircle(testpos, rangeRadius, originnodepos))
			{
				//Debug.Log(originnodepos + " is inside " + testpos + " (rad " + radius + ")");
				startpos = originnodepos;
			}
			else
			{
				RaycastHit2D hit = Physics2D.Raycast(originnodepos, (nextpos - originnodepos).normalized, 
				                                     Vector2.Distance(originnodepos, nextpos), 1 << LayerMask.NameToLayer("PositioningAIRange"));
				if (hit.collider != null)
					startpos = hit.point;
				else
					continue;
			}

			if (isPointInCircle(testpos, rangeRadius, nextpos))
				endpos = nextpos;
			else
			{
				RaycastHit2D hit = Physics2D.Raycast(originnodepos, (originnodepos - nextpos).normalized, 
				                                     Vector2.Distance(originnodepos, nextpos), 1 << LayerMask.NameToLayer("PositioningAIRange"));
				if (hit.collider != null)
					endpos = hit.point;
				else
					continue;
			}

			dist += Vector2.Distance(startpos, endpos);

			//if (dist > 0)
			//	Debug.Log("(" + testpos + "): " + nodeGO.name + " to " + nextGO.name + ". DIST: " + dist);
		}

		return dist;
	}   

	private bool isPointInCircle(Vector2 center, float radius, Vector2 tocheck)
	{
		if (isInRectangle(center, radius, tocheck))
		{
			double dx = center.x - tocheck.x;
			double dy = center.y - tocheck.y;
			dx *= dx;
			dy *= dy;
			double distanceSquared = dx + dy;
			double radiusSquared = radius * radius;
			return distanceSquared <= radiusSquared;
		}
		return false;
	}
	
	private bool isInRectangle(Vector2 center, float radius, Vector2 tocheck)
	{
		return tocheck.x >= center.x - radius && tocheck.x <= center.x + radius && 
			tocheck.y >= center.y - radius && tocheck.y <= center.y + radius;
	} 
}
