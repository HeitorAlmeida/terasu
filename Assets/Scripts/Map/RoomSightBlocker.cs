using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSightBlocker : MonoBehaviour
{
	public const int SURROUND_STRIPE_SIZE = 2;

	public Sprite blockerSprite;

	private int mapMatrixSize;
	private int[][] slotNodeIDs;
	private int[] loadedNodeIDs;
	private int[] nodeFirstRows;
	private int[] nodeFirstColumns;
	private int[] nodeLastRows;
	private int[] nodeLastColumns;
	private int[] nodeBlockerFirstRows;
	private int[] nodeBlockerFirstColumns;
	private int[] nodeBlockerLastRows;
	private int[] nodeBlockerLastColumns;
	private GameObject[] blockerObjects;
	private SpriteRenderer[] blockerRenderers;
	private int[] blockerRows;
	private int[] blockerColumns;
	private bool[] blockerFadings;
	private float blockerWidth;
	private float blockerHeight;
	private int surroundStripeSize;
	private float spriteWidthCorrection;
	private float spriteHeightCorrection;

	void Awake()
	{
		slotNodeIDs = null;
		loadedNodeIDs = null;
		nodeFirstRows = null;
		nodeFirstColumns = null;
		nodeLastRows = null;
		nodeLastColumns = null;
		nodeBlockerFirstRows = null;
		nodeBlockerFirstColumns = null;
		nodeBlockerLastRows = null;
		nodeBlockerLastColumns = null;
		blockerObjects = null;
		blockerRenderers = null;
		blockerRows = null;
		blockerColumns = null;
		blockerFadings = null;
		blockerWidth = 0f;
		blockerHeight = 0f;
		surroundStripeSize = SURROUND_STRIPE_SIZE;
		spriteWidthCorrection = 0f;
		spriteHeightCorrection = 0f;

		//transform.position = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_ROOM_BLOCKER ) );
	}

	public void SetMapSize( int newMapSize )
	{
		int[] IDRow = null;

		if( slotNodeIDs == null )
		{
			mapMatrixSize = newMapSize;
			slotNodeIDs = new int[mapMatrixSize][];
			for( int i=0; i<mapMatrixSize; i++ )
			{
				IDRow = new int[mapMatrixSize];
				for( int j=0; j<mapMatrixSize; j++ )
				{
					IDRow[j] = GraphNode.NODE_ID_INVALID;
				}
				slotNodeIDs[i] = IDRow;
			}
		}
	}

	public bool TileDimensionSet()
	{
		return !( (blockerWidth == 0f) || (blockerHeight == 0f) );
	}

	public bool SetBlockerDimensions( float blockerWorldWidth, float blockerWorldHeight )
	{
		if( (blockerObjects == null) && (blockerWorldWidth > 0f) && (blockerWorldHeight > 0f) )
		{
			blockerWidth = blockerWorldWidth;
			blockerHeight = blockerWorldHeight;
			if( blockerSprite != null )
			{
				spriteWidthCorrection = blockerWidth / blockerSprite.bounds.size.x;
				spriteHeightCorrection = blockerHeight / blockerSprite.bounds.size.y;
			}
			return true;
		}
		return false;
	}

	public bool LoadNodeIntoSlots( int nodeID, int firstSlotRow, int firstSlotColumn, int slotsWidth, int slotsHeight )
	{
		int lastSlotRow = -1;
		int lastSlotColumn = -1;
		int otherFirstRow = -1;
		int otherFirstColumn = -1;
		int otherLastRow = -1;
		int otherLastColumn = -1;
		int[] IDRow = null;
		int blockFirstRow = -1;
		int blockFirstColumn = -1;
		int blockLastRow = -1;
		int blockLastColumn = -1;
		int blockerIndex = -1;
		SpriteRenderer blockerRenderer = null;
		Color blockerColor = Color.black;

		if( (nodeID == GraphNode.NODE_ID_INVALID) || (slotsWidth < 1) || (slotsHeight < 1) )
		{
			return false;
		}
		lastSlotRow = firstSlotRow + slotsHeight - 1;
		lastSlotColumn = firstSlotColumn + slotsWidth - 1;
		if( (firstSlotRow < 0) || (firstSlotColumn < 0) || (lastSlotRow >= mapMatrixSize) || (lastSlotColumn >= mapMatrixSize) )
		{
			return false;
		}
		/*halmeida - check if the new node collides with the area of another.*/
		/*halmeida - following the same logic as the tiles within a structure, the row index of the slots grows as the
		position gets lower. For that reason, the top of the area of a node is its last and lowest row, with the biggest
		slot row index within the node.*/
		if( loadedNodeIDs != null )
		{
			for( int i=0; i<loadedNodeIDs.Length; i++ )
			{
				otherFirstRow = nodeFirstRows[i];
				otherFirstColumn = nodeFirstColumns[i];
				otherLastRow = nodeLastRows[i];
				otherLastColumn = nodeLastColumns[i];
				if( UsefulFunctions.AreasCollideLimits( firstSlotColumn, lastSlotRow, lastSlotColumn, firstSlotRow, otherFirstColumn,
					otherLastRow, otherLastColumn, otherFirstRow ) )
				{
					Debug.Log("Debug : RoomSightBlocker : impossible to add node "+nodeID+", collision detected with "+loadedNodeIDs[i]+".");
					return false;
				}
			}
		}
		/*halmeida - add the node into the designated slots.*/
		UsefulFunctions.IncreaseArray<int>( ref loadedNodeIDs, nodeID );
		UsefulFunctions.IncreaseArray<int>( ref nodeFirstRows, firstSlotRow );
		UsefulFunctions.IncreaseArray<int>( ref nodeFirstColumns, firstSlotColumn );
		UsefulFunctions.IncreaseArray<int>( ref nodeLastRows, lastSlotRow );
		UsefulFunctions.IncreaseArray<int>( ref nodeLastColumns, lastSlotColumn );
		if( slotNodeIDs != null )
		{
			for( int i=firstSlotRow; i<=lastSlotRow; i++ )
			{
				IDRow = slotNodeIDs[i];
				for( int j=firstSlotColumn; j<=lastSlotColumn; j++ )
				{
					IDRow[j] = nodeID;
					if( BlockerExists( i, j, ref blockerIndex ) )
					{
						blockerFadings[blockerIndex] = true;
					}
				}
			}
		}
		/*halmeida - add sight blockers to the surrounding slots. If a blocker is created where another
		node's slot is already loaded, that blocker is made immediately invisible.*/
		blockFirstRow = firstSlotRow - surroundStripeSize;
		if( blockFirstRow < 0 )
		{
			blockFirstRow = 0;
		}
		blockFirstColumn = firstSlotColumn - surroundStripeSize;
		if( blockFirstColumn < 0 )
		{
			blockFirstColumn = 0;
		}
		blockLastRow = lastSlotRow + surroundStripeSize;
		if( blockLastRow > (mapMatrixSize-1) )
		{
			blockLastRow = mapMatrixSize-1;
		}
		blockLastColumn = lastSlotColumn + surroundStripeSize;
		if( blockLastColumn > (mapMatrixSize-1) )
		{
			blockLastColumn = mapMatrixSize-1;
		}
		UsefulFunctions.IncreaseArray<int>( ref nodeBlockerFirstRows, blockFirstRow );
		UsefulFunctions.IncreaseArray<int>( ref nodeBlockerFirstColumns, blockFirstColumn );
		UsefulFunctions.IncreaseArray<int>( ref nodeBlockerLastRows, blockLastRow );
		UsefulFunctions.IncreaseArray<int>( ref nodeBlockerLastColumns, blockLastColumn );
		if( slotNodeIDs != null )
		{
			for( int i=blockFirstRow; i<=blockLastRow; i++ )
			{
				IDRow = slotNodeIDs[i];
				for( int j=blockFirstColumn; j<=blockLastColumn; j++ )
				{
					if( IDRow[j] != nodeID )
					{
						if( !BlockerExists( i, j, ref blockerIndex ) )
						{
							CreateBlocker( i, j );
							if( blockerObjects != null )
							{
								blockerIndex = blockerObjects.Length-1;
							}
						}
						if( IDRow[j] != GraphNode.NODE_ID_INVALID )
						{
							if( !blockerFadings[blockerIndex] )
							{
								blockerFadings[blockerIndex] = true;
								blockerRenderer = blockerRenderers[blockerIndex];
								blockerColor = blockerRenderer.color;
								blockerColor.a = 0f;
								blockerRenderer.color = blockerColor;
							}
						}
					}
				}
			}
		}
		return true;
	}

	public void ClearLoadedNode( int nodeID )
	{
		int nodeIndex = -1;
		int blockFirstRow = -1;
		int blockFirstColumn = -1;
		int blockLastRow = -1;
		int blockLastColumn = -1;
		int otherBlockFirstRow = -1;
		int otherBlockFirstColumn = -1;
		int otherBlockLastRow = -1;
		int otherBlockLastColumn = -1;
		int[] IDRow = null;
		bool intersectionFound = false;
		int blockerIndex = -1;

		if( loadedNodeIDs != null )
		{
			for( int i=0; i<loadedNodeIDs.Length; i++ )
			{
				if( loadedNodeIDs[i] == nodeID )
				{
					nodeIndex = i;
					break;
				}
			}
		}
		if( nodeIndex > -1 )
		{
			if( slotNodeIDs != null )
			{
				blockFirstRow = nodeBlockerFirstRows[nodeIndex];
				blockFirstColumn = nodeBlockerFirstColumns[nodeIndex];
				blockLastRow = nodeBlockerLastRows[nodeIndex];
				blockLastColumn = nodeBlockerLastColumns[nodeIndex];
				/*halmeida - I remove every blocker in this node's blocked area that is not within other
				node's blocked area, i.e., I remove every blocker that belongs exclusively to this node.*/
				for( int i=blockFirstRow; i<=blockLastRow; i++ )
				{
					IDRow = slotNodeIDs[i];
					for( int j=blockFirstColumn; j<=blockLastColumn; j++ )
					{
						if( IDRow[j] == GraphNode.NODE_ID_INVALID )
						{
							if( BlockerExists( i, j, ref blockerIndex ) )
							{
								/*halmeida - when this node was loaded, a blocker was created in this position or
								a blocker already existed in this position. I can only remove it if it does not
								belong to the blocked area of another node.*/
								intersectionFound = false;
								for( int k=0; k<loadedNodeIDs.Length; k++ )
								{
									if( k != nodeIndex )
									{
										otherBlockFirstRow = nodeBlockerFirstRows[k];
										otherBlockFirstColumn = nodeBlockerFirstColumns[k];
										otherBlockLastRow = nodeBlockerLastRows[k];
										otherBlockLastColumn = nodeBlockerLastColumns[k];
										if( UsefulFunctions.AreaContainsSlot( otherBlockFirstColumn, otherBlockLastRow, otherBlockLastColumn,
											otherBlockFirstRow, j, i ) )
										{
											intersectionFound = true;
											break;
										}
									}
								}
								if( !intersectionFound )
								{
									RemoveBlocker( blockerIndex );
								}
							}
						}
						else
						{
							/*halmeida - if the actual slots of this node were loaded on top of any previously
							loaded blockers, these blockers were faded to show the node. Now, removing this node,
							we remove the fading from these blockers. But that isn't all : the node that created
							that blocker that is within this node's actual area may have already been removed. At
							the ocasion, it did not remove this particular blocker because it also belonged to
							the area of this node. So, if we don't remove it now, it will never be removed. To
							sum it up, if there are no other nodes responsible for a blocker that is within the
							actual area of a node, we gotta remove that blocker when removing the node.*/
							if( IDRow[j] == nodeID )
							{
								if( BlockerExists( i, j, ref blockerIndex ) )
								{
									intersectionFound = false;
									for( int k=0; k<loadedNodeIDs.Length; k++ )
									{
										if( k != nodeIndex )
										{
											otherBlockFirstRow = nodeBlockerFirstRows[k];
											otherBlockFirstColumn = nodeBlockerFirstColumns[k];
											otherBlockLastRow = nodeBlockerLastRows[k];
											otherBlockLastColumn = nodeBlockerLastColumns[k];
											if( UsefulFunctions.AreaContainsSlot( otherBlockFirstColumn, otherBlockLastRow, otherBlockLastColumn,
												otherBlockFirstRow, j, i ) )
											{
												intersectionFound = true;
												break;
											}
										}
									}
									if( !intersectionFound )
									{
										RemoveBlocker( blockerIndex );
									}
									else
									{
										blockerFadings[blockerIndex] = false;
									}
								}
							}
						}
					}
				}
				/*halmeida - remove the records of this node.*/
				blockFirstRow = nodeFirstRows[nodeIndex];
				blockFirstColumn = nodeFirstColumns[nodeIndex];
				blockLastRow = nodeLastRows[nodeIndex];
				blockLastColumn = nodeLastColumns[nodeIndex];
				for( int i=blockFirstRow; i<=blockLastRow; i++ )
				{
					IDRow = slotNodeIDs[i];
					for( int j=blockFirstColumn; j<=blockLastColumn; j++ )
					{
						IDRow[j] = GraphNode.NODE_ID_INVALID;
					}
				}
			}
			UsefulFunctions.DecreaseArray<int>( ref loadedNodeIDs, nodeIndex );
			UsefulFunctions.DecreaseArray<int>( ref nodeFirstRows, nodeIndex );
			UsefulFunctions.DecreaseArray<int>( ref nodeFirstColumns, nodeIndex );
			UsefulFunctions.DecreaseArray<int>( ref nodeLastRows, nodeIndex );
			UsefulFunctions.DecreaseArray<int>( ref nodeLastColumns, nodeIndex );
			UsefulFunctions.DecreaseArray<int>( ref nodeBlockerFirstRows, nodeIndex );
			UsefulFunctions.DecreaseArray<int>( ref nodeBlockerFirstColumns, nodeIndex );
			UsefulFunctions.DecreaseArray<int>( ref nodeBlockerLastRows, nodeIndex );
			UsefulFunctions.DecreaseArray<int>( ref nodeBlockerLastColumns, nodeIndex );
		}
	}

	private bool BlockerExists( int row, int column, ref int blockerIndex )
	{
		if( blockerRows != null )
		{
			for( int i=0; i<blockerRows.Length; i++ )
			{
				if( blockerRows[i] == row )
				{
					if( blockerColumns[i] == column )
					{
						blockerIndex = i;
						return true;
					}
				}
			}
		}
		blockerIndex = -1;
		return false;
	}

	private void CreateBlocker( int row, int column )
	{
		GameObject newBlockerObject = null;
		SpriteRenderer newBlockerRenderer = null;

		newBlockerObject = new GameObject("RoomBlocker("+row+", "+column+")");
		newBlockerObject.transform.SetParent( transform, false );
		newBlockerRenderer = newBlockerObject.AddComponent<SpriteRenderer>();
		newBlockerRenderer.sprite = blockerSprite;
		newBlockerObject.transform.localScale = new Vector3( spriteWidthCorrection, spriteHeightCorrection, 1f );
		newBlockerObject.transform.localPosition = new Vector3( (column+0.5f) * blockerWidth, (row+0.5f) * -blockerHeight, 0f );
		UsefulFunctions.IncreaseArray<GameObject>( ref blockerObjects, newBlockerObject );
		UsefulFunctions.IncreaseArray<SpriteRenderer>( ref blockerRenderers, newBlockerRenderer );
		UsefulFunctions.IncreaseArray<int>( ref blockerRows, row );
		UsefulFunctions.IncreaseArray<int>( ref blockerColumns, column );
		UsefulFunctions.IncreaseArray<bool>( ref blockerFadings, false );
	}

	private void RemoveBlocker( int row, int column )
	{
		if( blockerRows != null )
		{
			for( int i=0; i<blockerRows.Length; i++ )
			{
				if( blockerRows[i] == row )
				{
					if( blockerColumns[i] == column )
					{
						blockerRenderers[i] = null;
						Destroy( blockerObjects[i] );
						blockerObjects[i] = null;
						UsefulFunctions.DecreaseArray<GameObject>( ref blockerObjects, i );
						UsefulFunctions.DecreaseArray<SpriteRenderer>( ref blockerRenderers, i );
						UsefulFunctions.DecreaseArray<int>( ref blockerRows, i );
						UsefulFunctions.DecreaseArray<int>( ref blockerColumns, i );
						UsefulFunctions.DecreaseArray<bool>( ref blockerFadings, i );
						return;
					}
				}
			}
		}
	}

	private void RemoveBlocker( int blockerIndex )
	{
		if( (blockerRows != null) && (blockerIndex > -1) )
		{
			if( blockerRows.Length > blockerIndex )
			{
				blockerRenderers[blockerIndex] = null;
				Destroy( blockerObjects[blockerIndex] );
				blockerObjects[blockerIndex] = null;
				UsefulFunctions.DecreaseArray<GameObject>( ref blockerObjects, blockerIndex );
				UsefulFunctions.DecreaseArray<SpriteRenderer>( ref blockerRenderers, blockerIndex );
				UsefulFunctions.DecreaseArray<int>( ref blockerRows, blockerIndex );
				UsefulFunctions.DecreaseArray<int>( ref blockerColumns, blockerIndex );
				UsefulFunctions.DecreaseArray<bool>( ref blockerFadings, blockerIndex );
			}
		}
	}

	public void Progress( float timeStep )
	{
		Color newColor = Color.black;
		float newAlpha = 0f;

		if( blockerFadings != null )
		{
			for( int i=0; i<blockerFadings.Length; i++ )
			{
				newColor = blockerRenderers[i].color;
				if( blockerFadings[i] )
				{
					newAlpha = newColor.a;
					newAlpha -= timeStep;
					if( newAlpha < 0f )
					{
						newAlpha = 0f;
					}
					newColor.a = newAlpha;
				}
				else
				{
					newColor.a = 1f;
				}
				blockerRenderers[i].color = newColor;
			}
		}
	}

	public void ClearAllLoadedNodes()
	{
		int[] IDRow = null;

		if( blockerObjects != null )
		{
			for( int i=0; i<blockerObjects.Length; i++ )
			{
				blockerRenderers[i] = null;
				Destroy( blockerObjects[i] );
				blockerObjects[i] = null;
			}
			blockerObjects = null;
			blockerRenderers = null;
			blockerRows = null;
			blockerColumns = null;
			blockerFadings = null;
		}
		if( loadedNodeIDs != null )
		{
			loadedNodeIDs = null;
			nodeFirstRows = null;
			nodeFirstColumns = null;
			nodeLastRows = null;
			nodeLastColumns = null;
			nodeBlockerFirstRows = null;
			nodeBlockerFirstColumns = null;
			nodeBlockerLastRows = null;
			nodeBlockerLastColumns = null;
			if( slotNodeIDs != null )
			{
				for( int i=0; i<slotNodeIDs.Length; i++ )
				{
					IDRow = slotNodeIDs[i];
					for( int j=0; j<IDRow.Length; j++ )
					{
						IDRow[j] = GraphNode.NODE_ID_INVALID;
					}
				}
			}
		}
	}

	public void Clear()
	{
		ClearAllLoadedNodes();
		if( slotNodeIDs != null )
		{
			for( int i=0; i<slotNodeIDs.Length; i++ )
			{
				slotNodeIDs[i] = null;
			}
			slotNodeIDs = null;
		}
	}

	private void PrintSlotNodeIDs()
	{
		int[] IDRow = null;

		Debug.Log("Debug : RoomSightBlocker : printing slots...");
		if( slotNodeIDs != null )
		{
			for( int i=0; i<slotNodeIDs.Length; i++ )
			{
				IDRow = slotNodeIDs[i];
				if( IDRow != null )
				{
					Debug.Log(UsefulFunctions.StringFromArray<int>( ", ", IDRow ));
				}
			}
		}
	}
}
