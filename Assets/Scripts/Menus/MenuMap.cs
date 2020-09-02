using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //to use Image component.

public class MenuMap : MenuSimple
{
	public int slotsPerMapSide;
	public float slotToCanvasRate;
	public float wallToSlotRate;
	public GameObject markerButtonModel;

	private int mapSize;
	private int[][] slotNodeIDs;
	private int[] loadedNodeIDs;
	private int[] nodeFirstRows;
	private int[] nodeFirstColumns;
	private int[] nodeLastRows;
	private int[] nodeLastColumns;
	private bool[][] nodePassages;
	private GameObject[] nodeSlotObjects;
	private Image[] nodeSlotImages;
	private GameObject[][] nodeWallObjects;
	private Image[][] nodeWallImages;
	private float canvasSlotSize;
	private float canvasWallSize;
	private bool mapVisible;
	private int currentNodeID;
	private GameObject markerButtonObject;
	private RectTransform markerButtonTrans;
	private ButtonAnimator markerButton;
	private Vector2 markerAnchoredPos;
	private int minVisibleRow;
	private int minVisibleColumn;
	private int maxVisibleRow;
	private int maxVisibleColumn;

	protected override void Awake()
	{
		int[] IDRow = null;

		base.Awake();
		mapSize = slotsPerMapSide;
		mapSize = (mapSize > 0) ? mapSize : 1; 
		slotToCanvasRate = (slotToCanvasRate > 0f) ? slotToCanvasRate : 0.1f;
		wallToSlotRate = (wallToSlotRate > 0f) ? wallToSlotRate : 0.2f;
		/*halmeida - why should the slotNodeIDs matrix be static instead of dynamic?
		If it was dynamic, the allocation of a slot with a very high value of row and column would be much more
		expensive than the allocation of a slot with small values, because high row and column values demand the
		allocation of a much longer array, which still has to be entirely initialized with invalid ID values
		before receiving the proper ID value at specific indexes.
		We could imagine that if the matrix was sparse instead of filled, dynamic allocation would not be a problem,
		but that isn't true because the filled matrix of integers is equivalent to a sparse matrix of pointers. When
		pointing unused pointers to null, integers are being set with a specific value, as the integers that receive
		the invalid ID values in the filled static matrix.*/
		slotNodeIDs = new int[mapSize][];
		for( int i=0; i<mapSize; i++ )
		{
			IDRow = new int[mapSize];
			for( int j=0; j<mapSize; j++ )
			{
				IDRow[j] = GraphNode.NODE_ID_INVALID;
			}
			slotNodeIDs[i] = IDRow;
		}
		loadedNodeIDs = null;
		nodeFirstRows = null;
		nodeFirstColumns = null;
		nodeLastRows = null;
		nodeLastColumns = null;
		nodePassages = null;
		nodeSlotObjects = null;
		nodeSlotImages = null;
		nodeWallObjects = null;
		nodeWallImages = null;
		canvasSlotSize = 0f;
		canvasWallSize = 0f;
		mapVisible = false;
		currentNodeID = GraphNode.NODE_ID_INVALID;
		markerButtonObject = null;
		markerButtonTrans = null;
		markerButton = null;
		markerAnchoredPos = Vector2.zero;
		minVisibleRow = -1;
		minVisibleColumn = -1;
		maxVisibleRow = -1;
		maxVisibleColumn = -1;
	}

	public override void SetGameController( GameController newGameController, Canvas newInterfaceCanvas, GameObject newImageOverlayModel,
		EventManager newEventManager )
	{
		float referenceSize = 0f;

		base.SetGameController( newGameController, newInterfaceCanvas, newImageOverlayModel, newEventManager );
		if( interfaceCanvasComponent != null )
		{
			referenceSize = (interfaceCanvasRect.width > interfaceCanvasRect.height) ? interfaceCanvasRect.height : interfaceCanvasRect.width;
			canvasSlotSize = slotToCanvasRate * referenceSize;
			canvasWallSize = wallToSlotRate * canvasSlotSize;
		}
	}

	public void SetCurrentNode( int nodeID )
	{
		currentNodeID = nodeID;
	}

	public bool LoadNodeIntoSlots( GraphNode graphNode )
	{
		int nodeIndex = -1;
		int nodeID = GraphNode.NODE_ID_INVALID;
		int firstSlotRow = -1;
		int firstSlotColumn = -1;
		int slotsWidth = 0;
		int slotsHeight = 0;
		int lastSlotRow = -1;
		int lastSlotColumn = -1;
		bool[] passages = null;
		int otherFirstRow = -1;
		int otherFirstColumn = -1;
		int otherLastRow = -1;
		int otherLastColumn = -1;
		int[] IDRow = null;

		if( graphNode == null )
		{
			return false;
		}
		nodeID = graphNode.ID;
		firstSlotRow = graphNode.GetFirstSlotRow();
		firstSlotColumn = graphNode.GetFirstSlotColumn();
		slotsWidth = graphNode.GetWidthInSlots();
		slotsHeight = graphNode.GetHeightInSlots();
		if( (nodeID == GraphNode.NODE_ID_INVALID) || (slotsWidth < 1) || (slotsHeight < 1) )
		{
			return false;
		}
		lastSlotRow = firstSlotRow + slotsHeight - 1;
		lastSlotColumn = firstSlotColumn + slotsWidth - 1;
		if( (firstSlotRow < 0) || (firstSlotColumn < 0) || (lastSlotRow >= mapSize) || (lastSlotColumn >= mapSize) )
		{
			return false;
		}
		/*halmeida - if the node has already been loaded into the map before, with different data, we remove it before adding it
		again.*/
		nodeIndex = GetLoadedNodeIndex( nodeID );
		if( nodeIndex > -1 )
		{
			if( (nodeFirstRows[nodeIndex] != firstSlotRow) || (nodeFirstColumns[nodeIndex] != firstSlotColumn) ||
				(nodeLastRows[nodeIndex] != lastSlotRow) || (nodeLastColumns[nodeIndex] != lastSlotColumn) )
			{
				ClearLoadedNodeWithIndex( nodeIndex );
			}
			else
			{
				return true;
			}
		}
		/*halmeida - check if the new node collides with the area of another.*/
		/*halmeida - following the same logic as the tiles within a structure, the row index of the slots grows as the
		position gets lower. For that reason, the bottom of the area of a node is its last and lowest row, with the biggest
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
					Debug.Log("Debug : MenuMap : impossible to add node "+nodeID+", collision detected with "+loadedNodeIDs[i]+".");
					return false;
				}
			}
		}
		/*halmeida - get the passage presence data from the node.*/
		passages = new bool[4];
		passages[0] = (graphNode.GetNodesUp() != null);
		passages[1] = (graphNode.GetNodesLeft() != null);
		passages[2] = (graphNode.GetNodesDown() != null);
		passages[3] = (graphNode.GetNodesRight() != null);
		/*halmeida - add the node into the designated slots.*/
		if( slotNodeIDs != null )
		{
			for( int i=firstSlotRow; i<=lastSlotRow; i++ )
			{
				IDRow = slotNodeIDs[i];
				for( int j=firstSlotColumn; j<=lastSlotColumn; j++ )
				{
					IDRow[j] = nodeID;
				}
			}
		}
		UsefulFunctions.IncreaseArray<int>( ref loadedNodeIDs, nodeID );
		UsefulFunctions.IncreaseArray<int>( ref nodeFirstRows, firstSlotRow );
		UsefulFunctions.IncreaseArray<int>( ref nodeFirstColumns, firstSlotColumn );
		UsefulFunctions.IncreaseArray<int>( ref nodeLastRows, lastSlotRow );
		UsefulFunctions.IncreaseArray<int>( ref nodeLastColumns, lastSlotColumn );
		UsefulFunctions.IncreaseArray<bool[]>( ref nodePassages, passages );
		UsefulFunctions.IncreaseArray<GameObject>( ref nodeSlotObjects, null );
		UsefulFunctions.IncreaseArray<Image>( ref nodeSlotImages, null );
		UsefulFunctions.IncreaseArray<GameObject[]>( ref nodeWallObjects, null );
		UsefulFunctions.IncreaseArray<Image[]>( ref nodeWallImages, null );
		if( mapVisible )
		{
			CreateNodeVisual( loadedNodeIDs.Length-1 );
		}
		return true;
	}

	public int GetLoadedNodeIndex( int nodeID )
	{
		if( loadedNodeIDs != null )
		{
			for( int i=0; i<loadedNodeIDs.Length; i++ )
			{
				if( loadedNodeIDs[i] == nodeID )
				{
					return i;
				}
			}
		}
		return -1;
	}

	public void ClearLoadedNode( int nodeID )
	{
		ClearLoadedNodeWithIndex( GetLoadedNodeIndex( nodeID ) );
	}

	public void ClearLoadedNodeWithIndex( int nodeIndex )
	{
		int firstRow = -1;
		int firstColumn = -1;
		int lastRow = -1;
		int lastColumn = -1;
		int[] IDRow = null;

		if( loadedNodeIDs != null )
		{
			if( (nodeIndex > -1) && (nodeIndex < loadedNodeIDs.Length) )
			{
				/*halmeida - if the node is currently visually loaded, we have to clear its visual objects.*/
				if( mapVisible )
				{
					ClearNodeVisual( nodeIndex );
				}
				/*halmeida - with the visual objects cleared, we clear the node's registered data.*/
				nodePassages[nodeIndex] = null;
				if( slotNodeIDs != null )
				{
					firstRow = nodeFirstRows[nodeIndex];
					firstColumn = nodeFirstColumns[nodeIndex];
					lastRow = nodeLastRows[nodeIndex];
					lastColumn = nodeLastColumns[nodeIndex];
					for( int i=firstRow; i<=lastRow; i++ )
					{
						IDRow = slotNodeIDs[i];
						for( int j=firstColumn; j<=lastColumn; j++ )
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
				UsefulFunctions.DecreaseArray<bool[]>( ref nodePassages, nodeIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref nodeSlotObjects, nodeIndex );
				UsefulFunctions.DecreaseArray<Image>( ref nodeSlotImages, nodeIndex );
				UsefulFunctions.DecreaseArray<GameObject[]>( ref nodeWallObjects, nodeIndex );
				UsefulFunctions.DecreaseArray<Image[]>( ref nodeWallImages, nodeIndex );
			}
		}
	}

	private void CreateCurrentNodeMarker()
	{
		if( (optionsParentTransform != null) && (markerButtonObject == null) && (markerButtonModel != null) )
		{
			markerButtonObject = Instantiate( markerButtonModel ) as GameObject;
			markerButtonTrans = markerButtonObject.GetComponent<RectTransform>();
			markerButton = markerButtonObject.GetComponent<ButtonAnimator>();
			if( (markerButtonTrans == null) || (markerButton == null) )
			{
				if( markerButton != null )
				{
					markerButton.Clear();
					markerButton = null;
				}
				markerButtonTrans = null;
				Destroy( markerButtonObject );
				markerButtonObject = null;
			}
			else
			{
				markerButtonTrans.SetParent( optionsParentTransform, false );
				markerButtonTrans.anchoredPosition = markerAnchoredPos;
			}
		}
	}

	private void CreateNodeVisual( int nodeIndex )
	{
		int nodeID = GraphNode.NODE_ID_INVALID;
		int firstRow = -1;
		int firstColumn = -1;
		int lastRow = -1;
		int lastColumn = -1;
		GameObject newSlotObject = null;
		Image newSlotImage = null;
		RectTransform rectTrans = null;
		float anchorX = 0f;
		float anchorY = 0f;
		GameObject newWallObject = null;
		Image newWallImage = null;
		GameObject[] newWallObjects = null;
		Image[] newWallImages = null;
		float nodeLeftX = 0f;
		float nodeTopY = 0f;
		float nodeWidth = 0f;
		float nodeHeight = 0f;
		string nodeObjectName = null;
		string wallName = null;
		float wallWidth = 0f;
		float wallHeight = 0f;
		float wallOffsetX = 0f;
		float wallOffsetY = 0f;
		bool[] passages = null;
		Color wallColor = Color.white;
		int passageIndex = -1;

		if( (optionsParentTransform != null) && (nodeSlotObjects != null) && (nodeIndex > -1) )
		{
			if( nodeSlotObjects.Length > nodeIndex )
			{
				nodeID = loadedNodeIDs[nodeIndex];
				nodeObjectName = "MapNode"+nodeID;
				Debug.Log("Debug : MenuMap : creating visual for node "+nodeID+".");
				firstRow = nodeFirstRows[nodeIndex];
				firstColumn = nodeFirstColumns[nodeIndex];
				lastRow = nodeLastRows[nodeIndex];
				lastColumn = nodeLastColumns[nodeIndex];
				passages = nodePassages[nodeIndex];
				newSlotObject = new GameObject(nodeObjectName, typeof(RectTransform));
				newSlotImage = newSlotObject.AddComponent<Image>();
				newSlotImage.color = Color.blue;
				rectTrans = newSlotObject.GetComponent<RectTransform>();
				rectTrans.SetParent( optionsParentTransform, false );
				nodeWidth = (lastColumn - firstColumn + 1) * canvasSlotSize;
				nodeHeight = (lastRow - firstRow + 1) * canvasSlotSize;
				rectTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, nodeWidth );
				rectTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, nodeHeight );
				nodeLeftX = firstColumn * canvasSlotSize;
				nodeTopY = -firstRow * canvasSlotSize;
				anchorX = nodeLeftX + (nodeWidth / 2f);
				anchorY = nodeTopY - (nodeHeight / 2f);
				rectTrans.anchoredPosition = new Vector2( anchorX, anchorY );
				if( currentNodeID == nodeID )
				{
					markerAnchoredPos = rectTrans.anchoredPosition;
				}
				nodeSlotObjects[nodeIndex] = newSlotObject;
				nodeSlotImages[nodeIndex] = newSlotImage;
				/*halmeida - update displayed map limits to use in map offset limit calculations.*/
				if( (minVisibleRow == -1) || (firstRow < minVisibleRow) )
				{
					minVisibleRow = firstRow;
				}
				if( (minVisibleColumn == -1) || (firstColumn < minVisibleColumn) )
				{
					minVisibleColumn = firstColumn;
				}
				if( (maxVisibleRow == -1) || (lastRow > maxVisibleRow) )
				{
					maxVisibleRow = lastRow;
				}
				if( (maxVisibleColumn == -1) || (lastColumn > maxVisibleColumn) )
				{
					maxVisibleColumn = lastColumn;
				}
				/*halmeida - create pointers for walls.*/
				newWallObjects = new GameObject[4];
				newWallImages = new Image[4];
				for( int i=0; i<4; i++ )
				{
					switch( i )
					{
						case 0:
							wallName = "WallU";
							wallWidth = nodeWidth;
							wallHeight = canvasWallSize;
							wallOffsetX = nodeWidth / 2f;
							wallOffsetY = -canvasWallSize / 2f;
							passageIndex = 0;
							break;
						case 1:
							wallName = "WallL";
							wallWidth = canvasWallSize;
							wallHeight = nodeHeight;
							wallOffsetX = canvasWallSize / 2f;
							wallOffsetY = -nodeHeight / 2f;
							passageIndex = 1;
							break;
						case 2:
							wallName = "WallD";
							wallWidth = nodeWidth;
							wallHeight = canvasWallSize;
							wallOffsetX = nodeWidth / 2f;
							wallOffsetY = -nodeHeight + (canvasWallSize / 2f);
							passageIndex = 2;
							break;
						case 3:
							wallName = "WallR";
							wallWidth = canvasWallSize;
							wallHeight = nodeHeight;
							wallOffsetX = nodeWidth - (canvasWallSize / 2f);
							wallOffsetY = -nodeHeight / 2f;
							passageIndex = 3;
							break;
					}
					newWallObject = new GameObject(nodeObjectName+wallName, typeof(RectTransform));
					newWallImage = newWallObject.AddComponent<Image>();
					if( passages != null )
					{
						if( passages.Length > passageIndex )
						{
							wallColor = (passages[passageIndex] ? Color.yellow : Color.white);
						}
					}
					newWallImage.color = wallColor;
					rectTrans = newWallObject.GetComponent<RectTransform>();
					rectTrans.SetParent( optionsParentTransform, false );
					rectTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, wallWidth );
					rectTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, wallHeight );
					anchorX = nodeLeftX + wallOffsetX;
					anchorY = nodeTopY + wallOffsetY;
					rectTrans.anchoredPosition = new Vector2( anchorX, anchorY );
					newWallObjects[i] = newWallObject;
					newWallImages[i] = newWallImage;
				}
				nodeWallObjects[nodeIndex] = newWallObjects;
				nodeWallImages[nodeIndex] = newWallImages;
			}
		}
	}

	private void ClearNodeVisual( int nodeIndex )
	{
		GameObject slotObject = null;
		GameObject wallObject = null;
		GameObject[] wallObjects = null;

		if( (nodeSlotObjects != null) && (nodeIndex > -1) )
		{
			if( nodeSlotObjects.Length > nodeIndex )
			{
				nodeSlotImages[nodeIndex] = null;
				slotObject = nodeSlotObjects[nodeIndex];
				if( slotObject != null )
				{
					Destroy( slotObject );
					nodeSlotObjects[nodeIndex] = null;
				}
				nodeWallImages[nodeIndex] = null;
				wallObjects = nodeWallObjects[nodeIndex];
				if( wallObjects != null )
				{
					for( int i=0; i<wallObjects.Length; i++ )
					{
						wallObject = wallObjects[i];
						if( wallObject != null )
						{
							Destroy( wallObject );
							wallObjects[i] = null;
						}
					}
					wallObjects = null;
					nodeWallObjects[nodeIndex] = null;
				}
			}
		}
	}

	protected override void BuildOptions()
	{
		if( !mapVisible && (loadedNodeIDs != null) )
		{
			minVisibleRow = -1;
			minVisibleColumn = -1;
			maxVisibleRow = -1;
			maxVisibleColumn = -1;
			for( int i=0; i<loadedNodeIDs.Length; i++ )
			{
				CreateNodeVisual( i );
			}
			CreateCurrentNodeMarker();
			mapVisible = true;
		}
	}

	protected override void ClearOptions()
	{
		if( markerButtonObject != null )
		{
			if( markerButton != null )
			{
				markerButton.Clear();
				markerButton = null;
			}
			markerButtonTrans = null;
			Destroy( markerButtonObject );
			markerButtonObject = null;
		}
		if( nodeSlotObjects != null )
		{
			for( int i=0; i<nodeSlotObjects.Length; i++ )
			{
				ClearNodeVisual( i );
			}
		}
		mapVisible = false;
	}

	protected override void SetMaxScreenOffsets()
	{
		/*halmeida - the top left of the slot at row 0 and column 0 sits at canvas coordinates (0, 0). This slot
		may never be occupied by a node, so it is wrong to assume that the map offset of (0, 0) is acceptable. I
		want to accept offsets that add at most half of the canvas dimensions outside of the displayed map limits.
		If we consider this first slot as the only occupied slot in our entire map, with a map offset of (0, 0)
		this slot already has half the canvas width empty to its left, so we cannot accept any offset of the map
		towards the right. Then, if we consider that only the slot to the right of this slot is occupied in the
		entire map, with a map offset of (0, 0) this slot will have more than half the canvas width empty to its
		left, and that cannot be accepted. The maximum accepted offset to the right, in this case, has to be
		negative.*/
		maxScreenOffsetRight = -minVisibleColumn * canvasSlotSize;
		maxScreenOffsetLeft = maxVisibleColumn * canvasSlotSize;
		maxScreenOffsetDown = -minVisibleRow * canvasSlotSize;
		maxScreenOffsetUp = maxVisibleRow * canvasSlotSize;
	}

	protected override void ScrollForUserConvenience()
	{
		if( optionsParentTransform != null )
		{
			optionsParentTransform.anchoredPosition = -markerAnchoredPos;
		}
	}

	protected override void ProgressOptionComponents( float timeStep )
	{
		if( markerButton != null )
		{
			markerButton.Progress( timeStep );
		}
	}
}
