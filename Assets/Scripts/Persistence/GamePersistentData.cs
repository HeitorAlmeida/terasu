using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //to create classes with [Serializable]

[Serializable]
public class GamePersistentData
{
	public int[] nodeIDs;
	public int[] nodeFirstSlotRows;
	public int[] nodeFirstSlotColumns;
	public int[] nodeSlotWidths;
	public int[] nodeSlotHeights;
	public int[] nodeLastColumns;
	public bool[] visitedNodes;
	public int savedNodeID;
	public int[] areaUnlockPayments;
	public int[] eventIDs;
	public bool[] eventConclusions;

	public GamePersistentData()
	{
		Clear();
	}

	public void Clear()
	{
		nodeIDs = null;
		nodeFirstSlotRows = null;
		nodeFirstSlotColumns = null;
		nodeSlotWidths = null;
		nodeSlotHeights = null;
		visitedNodes = null;
		savedNodeID = GraphNode.NODE_ID_INVALID;
		areaUnlockPayments = null;
		eventIDs = null;
		eventConclusions = null;
	}
}
