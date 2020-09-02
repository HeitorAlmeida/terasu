using UnityEngine;
using System.Collections;

public class GraphNode : MonoBehaviour
{
	public const int NODE_ID_INVALID = -1;

	public int ID;
	public int structureID;
	public int tileFactoryIDFront;
	public int tileFactoryIDBack;
	public int backgroundID;
	public bool centersBackground;
	public bool isSavePoint;
	public float HPDrainSpeed;
	public bool loadItems;
	public int areaLockIndex;
	public int[] nodeUpIDs;  /*adjacent and connected nodes above, from left to right.*/ 
	public int[] nodeLeftIDs;  /*adjacent and connected nodes to the left, from top to bottom.*/
	public int[] nodeDownIDs;  /*adjacent and connected nodes below, from left to right.*/
	public int[] nodeRightIDs;  /*adjacent and connected nodes to the right, from top to bottom.*/

	private GraphNode[] nodesUp;
	private GraphNode[] nodesLeft;
	private GraphNode[] nodesDown;
	private GraphNode[] nodesRight;
	private bool visited;
	private int firstSlotRow;
	private int firstSlotColumn;
	private int slotsWidth;
	private int slotsHeight;

	void Awake()
	{
		nodesUp = null;
		nodesLeft = null;
		nodesDown = null;
		nodesRight = null;
		visited = false;
		firstSlotRow = -1;
		firstSlotColumn = -1;
		slotsWidth = 0;
		slotsHeight = 0;
	}

	public void SetNodesUp( GraphNode[] newNodesUp )
	{
		nodesUp = newNodesUp;
	}

	public void SetNodesLeft( GraphNode[] newNodesLeft )
	{
		nodesLeft = newNodesLeft;
	}

	public void SetNodesDown( GraphNode[] newNodesDown )
	{
		nodesDown = newNodesDown;
	}

	public void SetNodesRight( GraphNode[] newNodesRight )
	{
		nodesRight = newNodesRight;
	}

	public GraphNode[] GetNodesUp()
	{
		return nodesUp;
	}

	public GraphNode[] GetNodesLeft()
	{
		return nodesLeft;
	}

	public GraphNode[] GetNodesDown()
	{
		return nodesDown;
	}

	public GraphNode[] GetNodesRight()
	{
		return nodesRight;
	}

	public void Visit( int newFirstSlotRow, int newFirstSlotColumn, int newSlotsWidth, int newSlotsHeight )
	{
		visited = true;
		firstSlotRow = newFirstSlotRow;
		firstSlotColumn = newFirstSlotColumn;
		slotsWidth = newSlotsWidth;
		slotsHeight = newSlotsHeight;
	}

	public void Unvisit()
	{
		visited = false;
		firstSlotRow = -1;
		firstSlotColumn = -1;
		slotsWidth = 0;
		slotsHeight = 0;
	}

	public bool IsVisited()
	{
		return visited;
	}

	public int GetFirstSlotRow()
	{
		return firstSlotRow;
	}

	public int GetFirstSlotColumn()
	{
		return firstSlotColumn;
	}

	public int GetWidthInSlots()
	{
		return slotsWidth;
	}

	public int GetHeightInSlots()
	{
		return slotsHeight;
	}
}
