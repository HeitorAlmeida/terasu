using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
	public GameObject[] nodeModels;
	public int suggestedStartNodeID;
	public int startNodeMapRow;
	public int startNodeMapColumn;
	public int[] costsToUnlockAreas;

	private GameObject[] nodeObjects;
	private GraphNode[] nodes;
	private int validStartNodeID;
	private MenuMap menuMap;
	private int unlockableAreas;
	private int[] areaUnlockCost;
	private int[] areaUnlockPaid;
	private bool[] areaUnlocked;
	private int currentNodeID;
	private bool untreatedUnlock;

	void Awake()
	{
		int cost = 0;

		nodeObjects = null;
		nodes = null;
		validStartNodeID = GraphNode.NODE_ID_INVALID;
		menuMap = null;
		ValidateNodes();
		ConnectNodes();
		if( costsToUnlockAreas != null )
		{
			unlockableAreas = costsToUnlockAreas.Length;
			if( unlockableAreas > 0 )
			{
				areaUnlockCost = new int[unlockableAreas];
				areaUnlockPaid = new int[unlockableAreas];
				areaUnlocked = new bool[unlockableAreas];
				for( int i=0; i<unlockableAreas; i++ )
				{
					cost = costsToUnlockAreas[i];
					cost *= (cost < 0 ? -1 : 1);
					areaUnlockCost[i] = cost;
					areaUnlockPaid[i] = 0;
					areaUnlocked[i] = (cost == 0);
				}
			}
		}
		currentNodeID = GraphNode.NODE_ID_INVALID;
		untreatedUnlock = false;
	}

	private void ValidateNodes()
	{
		int validNodes = 0;
		GameObject nodeModel = null;
		GameObject nodeObject = null;
		GraphNode node = null;
		int insertionPosition = 0;

		if( (nodeModels != null) && (nodeObjects == null) )
		{
			for( int i=0; i<nodeModels.Length; i++ )
			{
				nodeModel = nodeModels[i];
				if( nodeModel != null )
				{
					if( nodeModel.GetComponent<GraphNode>() != null )
					{
						validNodes++;
					}
				}
			}
		}
		if( validNodes > 0 )
		{
			nodeObjects = new GameObject[validNodes];
			nodes = new GraphNode[validNodes];
			for( int i=0; i<nodeModels.Length; i++ )
			{
				nodeModel = nodeModels[i];
				if( nodeModel != null )
				{
					if( nodeModel.GetComponent<GraphNode>() != null )
					{
						nodeObject = Instantiate( nodeModel ) as GameObject;
						nodeObject.transform.SetParent( this.gameObject.transform, false );
						node = nodeObject.GetComponent<GraphNode>();
						nodeObjects[insertionPosition] = nodeObject;
						nodes[insertionPosition] = node;
						if( (node.ID == suggestedStartNodeID) && (validStartNodeID == GraphNode.NODE_ID_INVALID) )
						{
							validStartNodeID = suggestedStartNodeID;
						}
						insertionPosition++;
					}
				}
			}
		}
	}

	private void ConnectNodes()
	{
		GraphNode node = null;
		int[] nextNodeIDs = null;
		GraphNode[] nextNodes = null;

		if( nodes != null )
		{
			/*halmeida - connect all the nodes according to their own instructions.*/
			for( int i=0; i<nodes.Length; i++ )
			{
				node = nodes[i];
				nextNodeIDs = node.nodeUpIDs;
				nextNodes = GetNodesWithIDs( nextNodeIDs );
				node.SetNodesUp( nextNodes );
				nextNodeIDs = node.nodeLeftIDs;
				nextNodes = GetNodesWithIDs( nextNodeIDs );
				node.SetNodesLeft( nextNodes );
				nextNodeIDs = node.nodeDownIDs;
				nextNodes = GetNodesWithIDs( nextNodeIDs );
				node.SetNodesDown( nextNodes );
				nextNodeIDs = node.nodeRightIDs;
				nextNodes = GetNodesWithIDs( nextNodeIDs );
				node.SetNodesRight( nextNodes );
			}
		}
	}

	public GraphNode GetNodeWithID( int nodeID )
	{
		GraphNode node = null;

		if( (nodes != null) && (nodeID != GraphNode.NODE_ID_INVALID) )
		{
			for( int i=0; i<nodes.Length; i++ )
			{
				node = nodes[i];
				if( node.ID == nodeID )
				{
					return node;
				}
			}
		}
		return null;
	}

	public GraphNode[] GetNodesWithIDs( int[] nodeIDs )
	{
		GraphNode[] correspondingNodes = null;
		int totalIDs = 0;
		int targetID = GraphNode.NODE_ID_INVALID;
		GraphNode node = null;

		if( (nodes != null) && (nodeIDs != null) )
		{
			totalIDs = nodeIDs.Length;
			if( totalIDs > 0 )
			{
				correspondingNodes = new GraphNode[totalIDs];
				for( int i=0; i<totalIDs; i++ )
				{
					targetID = nodeIDs[i];
					correspondingNodes[i] = null;
					for( int j=0; j<nodes.Length; j++ )
					{
						node = nodes[j];
						if( node.ID == targetID )
						{
							correspondingNodes[i] = node;
							break;
						}
					}
				}
			}
		}
		return correspondingNodes;
	}

	public int GetStartNodeID()
	{
		return validStartNodeID;
	}

	public void SetAssociatedMap( MenuMap newMenuMap )
	{
		menuMap = newMenuMap;
	}

	public void SetCurrentNode( int nodeID )
	{
		currentNodeID = nodeID;
		if( menuMap != null )
		{
			menuMap.SetCurrentNode( currentNodeID );
		}
	}

	public void SetNodeVisitation( int nodeID, bool nodeVisited, int nodeFirstRow, int nodeFirstColumn, int nodeSlotsWidth, int nodeSlotsHeight )
	{
		GraphNode node = null;

		if( (nodes != null) && (nodeID != GraphNode.NODE_ID_INVALID) )
		{
			for( int i=0; i<nodes.Length; i++ )
			{
				node = nodes[i];
				if( node.ID == nodeID )
				{
					if( nodeVisited )
					{
						node.Visit( nodeFirstRow, nodeFirstColumn, nodeSlotsWidth, nodeSlotsHeight );
						if( menuMap != null )
						{
							menuMap.LoadNodeIntoSlots( node );
						}
					}
					else
					{
						node.Unvisit();
						if( menuMap != null )
						{
							menuMap.ClearLoadedNode( nodeID );
						}
					}
					break;
				}
			}
		}
	}

	public int[] GetAllNodeIDs( ref bool[] nodeVisitations, ref int[] nodeFirstRows, ref int[] nodeFirstColumns, ref int[] nodeSlotWidths,
		ref int[] nodeSlotHeights )
	{
		int[] nodeIDs = null;
		GraphNode node = null;

		nodeVisitations = null;
		nodeFirstRows = null;
		nodeFirstColumns = null;
		nodeSlotWidths = null;
		nodeSlotHeights = null;
		if( nodes != null )
		{
			nodeIDs = new int[nodes.Length];
			nodeVisitations = new bool[nodes.Length];
			nodeFirstRows = new int[nodes.Length];
			nodeFirstColumns = new int[nodes.Length];
			nodeSlotWidths = new int[nodes.Length];
			nodeSlotHeights = new int[nodes.Length];
			for( int i=0; i<nodes.Length; i++ )
			{
				node = nodes[i];
				nodeIDs[i] = node.ID;
				nodeVisitations[i] = node.IsVisited();
				nodeFirstRows[i] = node.GetFirstSlotRow();
				nodeFirstColumns[i] = node.GetFirstSlotColumn();
				nodeSlotWidths[i] = node.GetWidthInSlots();
				nodeSlotHeights[i] = node.GetHeightInSlots();
			}
		}
		return nodeIDs;
	}

	public void SetAreaUnlockPayments( int[] paidValues )
	{
		if( (paidValues != null) && (unlockableAreas > 0) )
		{
			for( int i=0; i<unlockableAreas; i++ )
			{
				if( paidValues.Length > i )
				{
					areaUnlockPaid[i] = paidValues[i];
				}
				else
				{
					break;
				}
			}
			UpdateAllAreaLocks();
		}
	}

	private void UpdateAllAreaLocks()
	{
		int cost = 0;
		int paid = 0;
		bool previouslyLocked = false;

		if( unlockableAreas > 0 )
		{
			for( int i=0; i<unlockableAreas; i++ )
			{
				cost = areaUnlockCost[i];
				paid = areaUnlockPaid[i];
				previouslyLocked = !areaUnlocked[i];
				areaUnlocked[i] = ( paid >= cost );
				if( previouslyLocked && areaUnlocked[i] )
				{
					untreatedUnlock = true;
				}
			}
		}
	}

	public bool PayToUnlockArea( int areaIndex, int paidValue, ref int change )
	{
		int cost = 0;
		int alreadyPaid = 0;

		change = 0;
		if( (unlockableAreas > 0) && (areaIndex > -1) && (areaIndex < unlockableAreas) && (paidValue > 0) )
		{
			cost = areaUnlockCost[areaIndex];
			alreadyPaid = areaUnlockPaid[areaIndex];
			if( alreadyPaid < cost )
			{
				alreadyPaid += paidValue;
				if( alreadyPaid > cost )
				{
					change = alreadyPaid - cost;
					alreadyPaid = cost;
				}
				areaUnlockPaid[areaIndex] = alreadyPaid;
				areaUnlocked[areaIndex] = (alreadyPaid == cost);
				untreatedUnlock = areaUnlocked[areaIndex];
				return true;
			}
		}
		return false;
	}

	public bool IsAreaUnlocked( int areaIndex )
	{
		if( (unlockableAreas > 0) && (areaIndex > -1) && (areaIndex < unlockableAreas) )
		{
			return areaUnlocked[areaIndex];
		}
		return true;
	}

	public int[] GetAreaUnlockCostsCopy()
	{
		int[] costsCopy = null;

		if( unlockableAreas > 0 )
		{
			costsCopy = new int[unlockableAreas];
			for( int i=0; i<unlockableAreas; i++ )
			{
				costsCopy[i] = areaUnlockCost[i];
			}
		}
		return costsCopy;
	}

	public int[] GetAreaUnlockPaymentsCopy()
	{
		int[] paymentsCopy = null;

		if( unlockableAreas > 0 )
		{
			paymentsCopy = new int[unlockableAreas];
			for( int i=0; i<unlockableAreas; i++ )
			{
				paymentsCopy[i] = areaUnlockPaid[i];
			}
		}
		return paymentsCopy;
	}

	public int GetAreaUnlockCost( int areaIndex )
	{
		int cost = -1;

		if( (unlockableAreas > 0) && (areaIndex > -1) && (areaIndex < unlockableAreas) )
		{
			cost = areaUnlockCost[areaIndex];
		}
		return cost;
	}

	public int GetAreaUnlockPaid( int areaIndex )
	{
		int paid = -1;

		if( (unlockableAreas > 0) && (areaIndex > -1) && (areaIndex < unlockableAreas) )
		{
			paid = areaUnlockPaid[areaIndex];
		}
		return paid;
	}

	public bool RequestingUnlockTreatment()
	{
		return untreatedUnlock;
	}

	public void DismissUnlockTreatmentRequest()
	{
		untreatedUnlock = false;
	}
}
