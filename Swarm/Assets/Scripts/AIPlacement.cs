using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

public class AIPlacement : MonoBehaviour 
{
	public static AIPlacement i;

	public Transform Tower;

	public GameObject[] startingNodes;

	public Vector2 min;
	public Vector2 max;
	public float step;

	public Shader HeatMapShader;

	private int towerlayer;
	private int rangelayer;

    private List<LineSegment> linesegments = new List<LineSegment>();
    private List<PotentialTower> potentialTowers = new List<PotentialTower>();

    private Transform potentialTowerParent;

	void Awake()
	{
		i = this;
		towerlayer = LayerMask.NameToLayer("PositioningAITower");
		rangelayer = LayerMask.NameToLayer("PositioningAIRange");
        potentialTowerParent = new GameObject("[DEBUG] Potential Tower Parent").transform;
	}


	void Start() 
	{
		foreach (GameObject startingnode in startingNodes)
		{
			GetSegments(startingnode);
		}
	}

	private void GetSegments(GameObject fromnode)
	{
		NextNode nodeScript = fromnode.GetComponent<NextNode>();

		foreach (GameObject tonode in nodeScript.NODES)
		{
            LineSegment segment = new LineSegment(fromnode, tonode);
            if (linesegments.Contains(segment) == false)
                linesegments.Add(segment);

            GetSegments(tonode);
		}
	}

    bool showdebug = false;
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.B))
		{
			StartCoroutine(DoThing());
		}

        if (Input.GetKeyUp(KeyCode.N))
        {
            showdebug = !showdebug;

            foreach (LineSegment segment in linesegments)
                segment.DebugLine.enabled = showdebug;

            foreach (PotentialTower pTower in potentialTowers)
            {
                pTower.gameObject.renderer.enabled = false;
            }
        }
	}

	public IEnumerator DoThing()
	{
		TowerTargeting towertargeting = Tower.GetComponent<TowerTargeting>();
		yield return StartCoroutine(GetBestPosition(towertargeting.GetRange(), towertargeting.GetTowerRadius()));

        Vector2 best = potentialTowers.OrderByDescending(pTower => pTower.TotalDistance).First().Location;
		
		Transform tower = (Transform)GameObject.Instantiate(Tower);
		//go.transform.localScale = radius * Vector3.one;
		tower.transform.position = new Vector3(best.x, best.y, 0);
	}

	public IEnumerator GetBestPosition(float rangeRadius, float towerRadius)
	{
		GameObject towerGO = new GameObject("towerChecker");
		BoxCollider2D towerCollider = towerGO.AddComponent<BoxCollider2D>();
		towerCollider.size = Vector2.one * towerRadius;
		towerCollider.isTrigger = true;
		towerGO.layer = towerlayer;

		for (float x = min.x; x < max.x; x+=step)
		{
			for (float y = min.y; y < max.y; y+=step)
			{
                GameObject rangeGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DestroyImmediate(rangeGO.GetComponent<Collider>());
				CircleCollider2D rangeCollider = rangeGO.AddComponent<CircleCollider2D>();
				rangeCollider.radius = rangeRadius;
				rangeCollider.isTrigger = true;
                rangeGO.layer = rangelayer;
                rangeGO.transform.parent = potentialTowerParent;
                rangeGO.renderer.material.shader = HeatMapShader;

				Vector2 currentpos = new Vector2(x, y);
				rangeGO.transform.position = new Vector3(x, y, 0);
				towerGO.transform.position = rangeGO.transform.position;

				PotentialTower potentialTower = rangeGO.AddComponent<PotentialTower>();
                
                distsdists(potentialTower, currentpos, rangeRadius, towerRadius);

                potentialTowers.Add(potentialTower);

                if (potentialTower.TotalDistance > 0)
				{
                    rangeGO.name = potentialTower.TotalDistance.ToString();
                    rangeGO.renderer.material.color = new Color(potentialTower.TotalDistance / 26f, 0, 0, 0.85f);
				}
				else
                {
                    if (potentialTower.TotalDistance == (int)AIPLACEMENTERRORS.NodeTouchingTower)
                    {
                        rangeGO.name = "Node is touching tower";
                        rangeGO.renderer.material.color = Color.yellow; 
                    }
                    else if (potentialTower.TotalDistance == (int)AIPLACEMENTERRORS.PathOutsideRange)
                    {
                        rangeGO.name = "Path is outside the range of the tower";
                        rangeGO.renderer.material.color = Color.blue; 
                    }
                    else if (potentialTower.TotalDistance == (int)AIPLACEMENTERRORS.TowerInPath)
                    {
                        rangeGO.name = "Tower is in the path";
                        rangeGO.renderer.material.color = Color.green;
                    }
                    else if (potentialTower.TotalDistance == 0)
                    {
                        rangeGO.name = "_____ERROR TOWN_____";
                        Debug.Log("This really shouldn't happen. The potential tower is touching the path, but... no... amount of distance on the path.");
                        rangeGO.renderer.material.color = Color.cyan; // no... distance?
                    }

                }

                Destroy(rangeCollider);
			}

			yield return null;
		}
		
		Destroy(towerGO);
	}

    private void distsdists(PotentialTower potentialtower, Vector2 testpos, float rangeRadius, float towerRadius)
	{
        potentialtower.Location = testpos;

		foreach (LineSegment segment in linesegments)
		{
            Vector2 startIntersection = Vector2.one;
            Vector2 endIntersection = Vector2.one;

			if (isPointInCircle(testpos, towerRadius, segment.StartPos) || isPointInCircle(testpos, towerRadius, segment.EndPos))
			{
                potentialtower.TotalDistance = (int)AIPLACEMENTERRORS.NodeTouchingTower; // node inside tower
                return;
			}

			RaycastHit2D towerhit = Physics2D.Raycast(segment.StartPos, (segment.EndPos - segment.StartPos).normalized, 
			                                          Vector2.Distance(segment.StartPos, segment.EndPos), 1 << towerlayer);
			if (towerhit.collider != null)
            {
                potentialtower.TotalDistance = (int)AIPLACEMENTERRORS.TowerInPath; // tower on path
                return;
			}
			
			if (isPointInCircle(testpos, rangeRadius, segment.StartPos))
			{
				//Debug.Log(originnodepos + " is inside " + testpos + " (rad " + radius + ")");
                startIntersection = segment.StartPos;
			}
			else
			{
                RaycastHit2D hit = Physics2D.Raycast(segment.StartPos, (segment.EndPos - segment.StartPos).normalized,
                                                     Vector2.Distance(segment.StartPos, segment.EndPos), 1 << rangelayer);
				if (hit.collider != null)
                    startIntersection = hit.point;
				else
					continue;
			}

            if (isPointInCircle(testpos, rangeRadius, segment.EndPos))
                endIntersection = segment.EndPos;
			else
			{
                RaycastHit2D hit = Physics2D.Raycast(segment.EndPos, (segment.StartPos - segment.EndPos).normalized,
                                                     Vector2.Distance(segment.StartPos, segment.EndPos), 1 << rangelayer);
				if (hit.collider != null)
                    endIntersection = hit.point;
				else
					continue;
			}

            potentialtower.SegmentDistances.Add(segment, Vector2.Distance(startIntersection, endIntersection));
		}

        potentialtower.ComputeTotal();
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

public enum AIPLACEMENTERRORS
{
    PathOutsideRange = -1,
    NodeTouchingTower = -2,
    TowerInPath = -3,
}

public class LineSegment
{
    public static Transform parent;

	public GameObject StartNodeGO;
    public GameObject EndNodeGO;
    public NextNode StartNodeScript;
    public NextNode EndNodeScript;
    public Vector2 StartPos;
    public Vector2 EndPos;

    public LineRenderer DebugLine;

    public LineSegment(GameObject start, GameObject end)
    {
        StartNodeGO = start;
        EndNodeGO = end;

        StartNodeScript = start.GetComponent<NextNode>();
        EndNodeScript = end.GetComponent<NextNode>();

        StartPos = new Vector2(start.transform.position.x, start.transform.position.y);
        EndPos = new Vector2(end.transform.position.x, end.transform.position.y);

        if (parent == null)
        {
            parent = new GameObject("[DEBUG] Line segment parent").transform;
        }

        GameObject line = new GameObject(this.StartNodeGO.name + " to " + this.EndNodeGO.name);
        line.transform.parent = parent;
        DebugLine = line.AddComponent<LineRenderer>();
        DebugLine.SetWidth(0.05f, 0.05f);
        DebugLine.SetPosition(0, new Vector3(this.StartPos.x, this.StartPos.y, -1));
        DebugLine.SetPosition(1, new Vector3(this.EndPos.x, this.EndPos.y, -1));
    }

	public override bool Equals(System.Object obj)
	{
		if (obj == null)
			return false;

		LineSegment comparing = obj as LineSegment;
		if (comparing == null)
			return false;

		return (comparing.StartNodeGO == this.StartNodeGO && comparing.EndNodeGO == this.EndNodeGO);
	}

    public override int GetHashCode()
    {
        return StartNodeGO.GetHashCode() ^ EndNodeGO.GetHashCode();
    }
}
