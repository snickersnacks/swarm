using UnityEngine;
using System.Collections;

public class AIPlacement : MonoBehaviour 
{
	public static AIPlacement i;

	public Transform Tower;

	GameObject node1;

	public Vector2 min;
	public Vector2 max;
	public float step;

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
		yield return StartCoroutine(GetBestPosition(Tower.GetComponent<TowerTargeting>().GetRadius()));
		Vector2 best = imabest;
		
		Transform tower = (Transform)GameObject.Instantiate(Tower);
		//go.transform.localScale = radius * Vector3.one;
		tower.transform.position = new Vector3(best.x, best.y, 0);
	}

	private Vector2 imabest;
	public IEnumerator GetBestPosition(float radius)
	{
		Vector2 best = Vector2.zero;
		float topdist = -1;
		
		GameObject go = new GameObject("yourmom");
		CircleCollider2D collider = go.AddComponent<CircleCollider2D>();
		collider.radius = radius;
		collider.isTrigger = true;
		go.layer = LayerMask.NameToLayer("PositioningAI");

		for (float x = min.x; x < max.x; x+=step)
		{
			for (float y = min.y; y < max.y; y+=step)
			{
				go.transform.position = new Vector3(x, y, 0);

				float totaldist = distsdists(node1, new Vector2(x, y), radius);

				if (totaldist > topdist)
				{
					topdist = totaldist;
					best = new Vector2(x, y);
				}

				go.name = totaldist.ToString();

				//if (totaldist > 0)
				//	Debug.Log("(" + x + ", " + y + "): " + totaldist);
				//Debug.Break();
			}

			yield return null;
		}

		Destroy(go);

		Debug.Log("best pos: " + best + ". totaldist: " + topdist);
		imabest = best;
	}

	float distsdists(GameObject OnNode, Vector2 testpos, float radius)
	{
		float dist = raydist(OnNode, testpos, radius);
		NextNode nextnode = OnNode.GetComponent<NextNode>();

		foreach (GameObject nextGO in nextnode.NODES)
		{

			dist += distsdists(nextGO, testpos, radius);
			//Debug.Log("(" + testpos + "): " + OnNode.name + " to " + nextGO.name + ". DIST: " + dist);

		}

		return dist;
	}

	private float raydist(GameObject nodeGO, Vector2 testpos, float radius)
	{
		Vector2 originnodepos = new Vector2(nodeGO.transform.position.x, nodeGO.transform.position.y);
		float dist = 0;

		NextNode nextnode = nodeGO.GetComponent<NextNode>();

		foreach (GameObject nextGO in nextnode.NODES)
		{
			Vector2 startpos = Vector2.zero;
			Vector2 endpos = Vector2.zero;
			Vector2 nextpos = new Vector2(nextGO.transform.position.x, nextGO.transform.position.y);
			
			if (isPointInCircle(testpos, radius, originnodepos))
			{
				//Debug.Log(originnodepos + " is inside " + testpos + " (rad " + radius + ")");
				startpos = originnodepos;
			}
			else
			{
				RaycastHit2D hit = Physics2D.Raycast(originnodepos, (nextpos - originnodepos).normalized, 
				                                     Vector2.Distance(originnodepos, nextpos), 1 << LayerMask.NameToLayer("PositioningAI"));
				if (hit.collider != null)
					startpos = hit.point;
				else
					continue;
			}

			if (isPointInCircle(testpos, radius, nextpos))
				endpos = nextpos;
			else
			{
				RaycastHit2D hit = Physics2D.Raycast(originnodepos, (originnodepos - nextpos).normalized, 
				                                     Vector2.Distance(originnodepos, nextpos), 1 << LayerMask.NameToLayer("PositioningAI"));
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
