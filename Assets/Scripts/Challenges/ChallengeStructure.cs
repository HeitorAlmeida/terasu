using UnityEngine;
using System.Collections;
using System; //to use Exception types and String methods
using System.Runtime.Serialization.Formatters.Binary; //to use BinaryFormatter
using System.Runtime.Serialization; //to use SerializationException
using System.IO; //to use FileStream or MemoryStream

public class ChallengeStructure : MonoBehaviour
{
	public const int STRUCTURE_ID_INVALID = -1;

	public const int BLUEPRINT_CODE_INVALID = -1;
	public const int BLUEPRINT_CODE_NO_TILE = 0;
	public const int BLUEPRINT_CODE_TILE_NORMAL = 1;
	public const int BLUEPRINT_CODE_TILE_TRIANGLE = 2;
	public const int BLUEPRINT_CODE_TILE_N_SECOND = 3;
	public const int BLUEPRINT_CODE_TILE_N_LAST = 33;
	public const int BLUEPRINT_CODE_TILE_T_SECOND = 34;
	public const int BLUEPRINT_CODE_TILE_T_LAST = 63;

	public const int UNDEFINED_DIFFICULTY = -1;
	public const int DIFFICULTY_MIN = 0;
	public const int DIFFICULTY_MAX = 999;

	public const int TRIANGLE_CORNER_INVALID = -1;
	public const int TRIANGLE_CORNER_UL = 0;
	public const int TRIANGLE_CORNER_LD = 1;
	public const int TRIANGLE_CORNER_DR = 2;
	public const int TRIANGLE_CORNER_UR = 3;

	public const int TILES_TO_MAP_SQUARE_SIDE = 10;
	public const int TILE_GRADUAL_BUILD_AMOUNT = 30;

	private TileFactory tileFactoryFront;
	private TileFactory tileFactoryBack;
	private int structureID;
	private bool sufixedID;
	private float tileWidthFront;
	private float tileHeightFront;
	private float tileWidthBack;
	private float tileHeightBack;
	private int tileRows;
	private int tileColumns;
	private float structureWidth;
	private float structureHeight;
	private int[][] tileBlueprintFront;
	private int[][] tileBlueprintBack;
	private int[][] tileToOuterBox;
	private int[][] tileToOuterTri;
	private GameObject[][] tileObjectsFront;
	private GameObject[][] tileObjectsBack;
	private Tile[][] tileComponentsFront;
	private Tile[][] tileComponentsBack;
	private bool loadSuccess;
	private bool saveSuccess;
	private int[] gapUpColumns;
	private int[] gapLeftRows;
	private int[] gapDownColumns;
	private int[] gapRightRows;
	private int difficulty;
	private int[] itemIDs;
	private Vector2[] itemOffsets;
	private int[] itemToItemClusters;
	private BoxCollider2D[] outerColliderBox;
	private int[] outerBoxFirstRows;
	private int[] outerBoxFirstColumns;
	private int[] outerBoxLastRows;
	private int[] outerBoxLastColumns;
	private PolygonCollider2D[] outerColliderTri;
	private int[] outerTriFirstRows;
	private int[] outerTriFirstColumns;
	private int[] outerTriLastRows;
	private int[] outerTriLastColumns;
	private int[] outerTriCorners;
	private GameObject tileLayerFront;
	private GameObject tileLayerBack;
	private GameObject colliderMarkerLayer;
	private GameObject outerColliderLayer;
	private GameObject colliderMarkerModel;
	private GameObject[][] tileColliderMarkers;
	private bool markColliders;
	private bool showingColliderMarkers;
	private float mergeTolerance;
	private int[] mergingRows;
	private int[] mergingColumns;
	private Tile[] mergingComponents;
	private BoxCollider2D[] mergingCollidersBox;
	private PolygonCollider2D[] mergingCollidersTri;
	private int[] mergingOuterBoxIndexes;
	private int[] mergingOuterTriIndexes;
	private int totalMergingBoxes;
	private int totalMergingTris;
	private Rect[] doorsUp;
	private Rect[] doorsLeft;
	private Rect[] doorsDown;
	private Rect[] doorsRight;
	private int[] itemClusterRewardCodes;
	private int buildingTileRowFront;
	private int buildingTileRowBack;
	private int buildingTileColumnFront;
	private int buildingTileColumnBack;
	private bool buildingTilesFront;
	private bool buildingTilesBack;
	private bool persistentDataExtracted;
	private bool tilesBuilt;
	private Vector2[] waypointOffsets;
	private int[] waypointIDs;
	private int[] enemyCodes;
	private Vector2[] enemyOffsets;
	private RoutePersistentData[] enemyRoutes;

	void Awake()
	{
		Vector3 correctDepthPosition = Vector3.zero;
		float distanceToBack = 0f;

		tileFactoryFront = null;
		tileFactoryBack = null;
		structureID = STRUCTURE_ID_INVALID;
		sufixedID = false;
		tileWidthFront = 0f;
		tileWidthBack = 0f;
		tileHeightFront = 0f;
		tileHeightBack = 0f;
		tileRows = 0;
		tileColumns = 0;
		structureWidth = 0f;
		structureHeight = 0f;
		tileBlueprintFront = null;
		tileBlueprintBack = null;
		tileToOuterBox = null;
		tileToOuterTri = null;
		tileObjectsFront = null;
		tileObjectsBack = null;
		tileComponentsFront = null;
		tileComponentsBack = null;
		loadSuccess = false;
		saveSuccess = false;
		gapUpColumns = null;
		gapLeftRows = null;
		gapDownColumns = null;
		gapRightRows = null;
		difficulty = UNDEFINED_DIFFICULTY;
		itemIDs = null;
		itemOffsets = null;
		itemToItemClusters = null;
		outerColliderBox = null;
		outerBoxFirstRows = null;
		outerBoxFirstColumns = null;
		outerBoxLastRows = null;
		outerBoxLastColumns = null;
		outerColliderTri = null;
		outerTriFirstRows = null;
		outerTriFirstColumns = null;
		outerTriLastRows = null;
		outerTriLastColumns = null;
		outerTriCorners = null;
		tileLayerFront = null;
		tileLayerBack = null;
		colliderMarkerLayer = null;
		outerColliderLayer = null;
		colliderMarkerModel = null;
		tileColliderMarkers = null;
		markColliders = false;
		showingColliderMarkers = false;
		mergeTolerance = 0f;
		mergingRows = null;
		mergingColumns = null;
		mergingComponents = null;
		mergingCollidersBox = null;
		mergingCollidersTri = null;
		mergingOuterBoxIndexes = null;
		mergingOuterTriIndexes = null;
		totalMergingBoxes = 0;
		totalMergingTris = 0;
		doorsUp = null;
		doorsLeft = null;
		doorsDown = null;
		doorsRight = null;
		itemClusterRewardCodes = null;
		buildingTileRowFront = -1;
		buildingTileRowBack = -1;
		buildingTileColumnFront = -1;
		buildingTileColumnBack = -1;
		buildingTilesFront = false;
		buildingTilesBack = false;
		persistentDataExtracted = false;
		tilesBuilt = false;
		waypointOffsets = null;
		waypointIDs = null;
		enemyCodes = null;
		enemyOffsets = null;
		enemyRoutes = null;

		tileLayerFront = new GameObject( "TileLayerFront" );
		tileLayerFront.transform.SetParent( gameObject.transform );
		tileLayerFront.transform.rotation = Quaternion.identity;
		tileLayerFront.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, 0f );
		tileLayerFront.transform.localPosition = correctDepthPosition;

		tileLayerBack = new GameObject( "TileLayerBack" );
		tileLayerBack.transform.SetParent( gameObject.transform );
		tileLayerBack.transform.rotation = Quaternion.identity;
		tileLayerBack.transform.localScale = Vector3.one;
		distanceToBack = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_BACK_TILE );
		distanceToBack -= DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_TILE );
		correctDepthPosition.z = distanceToBack;
		tileLayerBack.transform.localPosition = correctDepthPosition;

		colliderMarkerLayer = new GameObject( "ColliderMarkerLayer" );
		colliderMarkerLayer.transform.SetParent( gameObject.transform );
		colliderMarkerLayer.transform.rotation = Quaternion.identity;
		colliderMarkerLayer.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, -1f * DisplayDepthManager.ELEMENT_TO_ELEMENT_OFFSET / 100f );
		colliderMarkerLayer.transform.localPosition = correctDepthPosition;

		outerColliderLayer = new GameObject( "OuterColliderLayer" );
		outerColliderLayer.transform.SetParent( gameObject.transform );
		outerColliderLayer.transform.rotation = Quaternion.identity;
		outerColliderLayer.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, -2f * DisplayDepthManager.ELEMENT_TO_ELEMENT_OFFSET / 100f );
		outerColliderLayer.transform.localPosition = correctDepthPosition;
	}

	public void SetColliderMarkerModel( GameObject newColliderMarkerModel )
	{
		if( tileObjectsFront == null )
		{
			colliderMarkerModel = newColliderMarkerModel;
			markColliders = ( colliderMarkerModel != null );
		}
	}

	public void SetTileFactories( TileFactory newTileFactoryFront, TileFactory newTileFactoryBack )
	{
		Vector2 tileWorldDimensions = Vector3.zero;

		tileFactoryFront = newTileFactoryFront;
		if( tileFactoryFront != null )
		{
			tileWorldDimensions = tileFactoryFront.GetTileWorldDimensions();
			tileWidthFront = tileWorldDimensions.x;
			tileHeightFront = tileWorldDimensions.y;
			structureWidth = tileColumns * tileWidthFront;
			structureHeight = tileRows * tileHeightFront;
			mergeTolerance = tileHeightFront / 100f;
		}
		else
		{
			tileWidthFront = 0f;
			tileHeightFront = 0f;
			structureWidth = 0f;
			structureHeight = 0f;
			mergeTolerance = 0f;
		}
		tileFactoryBack = newTileFactoryBack;
		if( tileFactoryBack != null )
		{
			tileWorldDimensions = tileFactoryBack.GetTileWorldDimensions();
			tileWidthBack = tileWorldDimensions.x;
			tileHeightBack = tileWorldDimensions.y;
		}
		else
		{
			tileWidthBack = 0f;
			tileHeightBack = 0f;
		}
		/*halmeida - recreate all tiles using the new tileFactory.*/
		ClearTiles();
		CreateAllTilesFromBlueprint();
		CreateOuterCollidersFromLimits();
		UpdateColliderMarkers();
	}

	public TileFactory GetTileFactory( bool front )
	{
		if( front )
		{
			return tileFactoryFront;
		}
		return tileFactoryBack;
	}

	public void GetStructureDimensions( ref int rows, ref int columns )
	{
		rows = tileRows;
		columns = tileColumns;
	}

	public void GetStructureWorldDimensions( ref float width, ref float height )
	{
		width = structureWidth;
		height = structureHeight;
	}

	public void SetStructureDimensions( int newRows, int newColumns )
	{
		int[] blueprintFrontRow = null;
		int[] blueprintBackRow = null;
		int[] newBlueprintFrontRow = null;
		int[] newBlueprintBackRow = null;
		int[][] newBlueprintFront = null;
		int[][] newBlueprintBack = null;
		GameObject[][] newTileObjectsFront = null;
		GameObject[][] newTileObjectsBack = null;
		GameObject[] newTileObjectRowFront = null;
		GameObject[] newTileObjectRowBack = null;
		Tile[][] newTileComponentsFront = null;
		Tile[][] newTileComponentsBack = null;
		Tile[] newTileComponentRowFront = null;
		Tile[] newTileComponentRowBack = null;
		int[][] newTileToOuterBox = null;
		int[] newTileToOuterBoxRow = null;
		int[][] newTileToOuterTri = null;
		int[] newTileToOuterTriRow = null;
		GameObject[][] newTileColliderMarkers = null;
		GameObject[] newTileColliderMarkerRow = null;
		bool createClearRow = false;
		GameObject[] tileObjectRowFront = null;
		GameObject[] tileObjectRowBack = null;
		Tile[] tileComponentRowFront = null;
		Tile[] tileComponentRowBack = null;
		int[] tileToOuterBoxRow = null;
		int[] tileToOuterTriRow = null;
		GameObject[] tileColliderMarkerRow = null;
		int changedRow = 0;
		int changedColumn = 0;
		Tile tileComponent = null;
		int currentTileShapeIndex = -1;
		int correctTileShapeIndex = -1;
		int oldTileRows = 0;
		int oldTileColumns = 0;
		GameObject tileObject = null;
		bool usesOuterCollider = false;
		int outerColliderIndex = -1;

		if( (newRows == tileRows) && (newColumns == tileColumns) )
		{
			return;
		}
		if( (newRows <= 0) || (newColumns <= 0) )
		{
			ClearBlueprint( true );
		}
		else
		{
			newBlueprintFront = new int[newRows][];
			newBlueprintBack = new int[newRows][];
			newTileToOuterBox = new int[newRows][];
			newTileToOuterTri = new int[newRows][];
			if( tileBlueprintFront != null )
			{
				/*halmeida - remove the outer colliders that will lose any pieces with the change.*/
				for( int i=0; i<tileRows; i++ )
				{
					tileToOuterBoxRow = tileToOuterBox[i];
					tileToOuterTriRow = tileToOuterTri[i];
					for( int j=0; j<tileColumns; j++ )
					{
						if( (j >= newColumns) || (i >= newRows) )
						{
							outerColliderIndex = tileToOuterBoxRow[j];
							RemoveOuterCollider( outerColliderIndex, true, true ); 
							outerColliderIndex = tileToOuterTriRow[j];
							RemoveOuterCollider( outerColliderIndex, false, true );
						}
					}
				}
				/*halmeida - copy the old blueprint as far as possible.*/
				for( int i=0; i<newRows; i++ )
				{
					createClearRow = false;
					newBlueprintFrontRow = new int[newColumns];
					newBlueprintBackRow = new int[newColumns];
					newTileToOuterBoxRow = new int[newColumns];
					newTileToOuterTriRow = new int[newColumns];
					if( i < tileBlueprintFront.Length )
					{
						blueprintFrontRow = tileBlueprintFront[i];
						blueprintBackRow = tileBlueprintBack[i];
						tileToOuterBoxRow = tileToOuterBox[i];
						tileToOuterTriRow = tileToOuterTri[i];
						if( blueprintFrontRow != null )
						{
							for( int j=0; j<newColumns; j++ )
							{
								if( j < blueprintFrontRow.Length )
								{
									newBlueprintFrontRow[j] = blueprintFrontRow[j];
									newBlueprintBackRow[j] = blueprintBackRow[j];
									newTileToOuterBoxRow[j] = tileToOuterBoxRow[j];
									newTileToOuterTriRow[j] = tileToOuterTriRow[j];
								}
								else
								{
									newBlueprintFrontRow[j] = BLUEPRINT_CODE_NO_TILE;
									newBlueprintBackRow[j] = BLUEPRINT_CODE_NO_TILE;
									newTileToOuterBoxRow[j] = -1;
									newTileToOuterTriRow[j] = -1;
								}
							}
						}
						else
						{
							createClearRow = true;
						}
					}
					else
					{
						createClearRow = true;
					}
					if( createClearRow )
					{
						for( int j=0; j<newColumns; j++ )
						{
							newBlueprintFrontRow[j] = BLUEPRINT_CODE_NO_TILE;
							newBlueprintBackRow[j] = BLUEPRINT_CODE_NO_TILE;
							newTileToOuterBoxRow[j] = -1;
							newTileToOuterTriRow[j] = -1;
						}
					}
					newBlueprintFront[i] = newBlueprintFrontRow;
					newBlueprintBack[i] = newBlueprintBackRow;
					newTileToOuterBox[i] = newTileToOuterBoxRow;
					newTileToOuterTri[i] = newTileToOuterTriRow;
					newBlueprintFrontRow = null;
					newBlueprintBackRow = null;
					newTileToOuterBoxRow = null;
					newTileToOuterTriRow = null;
				}
			}
			else
			{
				for( int i=0; i<newRows; i++ )
				{
					newBlueprintFrontRow = new int[newColumns];
					newBlueprintBackRow = new int[newColumns];
					newTileToOuterBoxRow = new int[newColumns];
					newTileToOuterTriRow = new int[newColumns];
					for( int j=0; j<newColumns; j++ )
					{
						newBlueprintFrontRow[j] = BLUEPRINT_CODE_NO_TILE;
						newBlueprintBackRow[j] = BLUEPRINT_CODE_NO_TILE;
						newTileToOuterBoxRow[j] = -1;
						newTileToOuterTriRow[j] = -1;
					}
					newBlueprintFront[i] = newBlueprintFrontRow;
					newBlueprintBack[i] = newBlueprintBackRow;
					newTileToOuterBox[i] = newTileToOuterBoxRow;
					newTileToOuterTri[i] = newTileToOuterTriRow;
					newBlueprintFrontRow = null;
					newBlueprintBackRow = null;
					newTileToOuterBoxRow = null;
					newTileToOuterTriRow = null;
				}
			}
			ClearBlueprint( false );
			tileBlueprintFront = newBlueprintFront;
			tileBlueprintBack = newBlueprintBack;
			tileToOuterBox = newTileToOuterBox;
			tileToOuterTri = newTileToOuterTri;
			newBlueprintFront = null;
			newBlueprintBack = null;
			newTileToOuterBox = null;
			newTileToOuterTri = null;
			/*halmeida - first I will create an entire empty array of pointers, with
			all the adequate room. Then I will sweep the old array of pointers and,
			as I find no room in the new array for an old object, that object is
			removed.*/
			newTileObjectsFront = new GameObject[newRows][];
			newTileObjectsBack = new GameObject[newRows][];
			newTileComponentsFront = new Tile[newRows][];
			newTileComponentsBack = new Tile[newRows][];
			newTileColliderMarkers = new GameObject[newRows][];
			for( int i=0; i<newRows; i++ )
			{
				newTileObjectRowFront = new GameObject[newColumns];
				newTileObjectRowBack = new GameObject[newColumns];
				newTileComponentRowFront = new Tile[newColumns];
				newTileComponentRowBack = new Tile[newColumns];
				newTileColliderMarkerRow = new GameObject[newColumns];
				for( int j=0; j<newColumns; j++ )
				{
					newTileObjectRowFront[j] = null;
					newTileObjectRowBack[j] = null;
					newTileComponentRowFront[j] = null;
					newTileComponentRowBack[j] = null;
					newTileColliderMarkerRow[j] = null;
				}
				newTileObjectsFront[i] = newTileObjectRowFront;
				newTileObjectsBack[i] = newTileObjectRowBack;
				newTileComponentsFront[i] = newTileComponentRowFront;
				newTileComponentsBack[i] = newTileComponentRowBack;
				newTileColliderMarkers[i] = newTileColliderMarkerRow;
				newTileObjectRowFront = null;
				newTileObjectRowBack = null;
				newTileComponentRowFront = null;
				newTileComponentRowBack = null;
				newTileColliderMarkerRow = null;
			}
			if( tileObjectsFront != null )
			{
				for( int i=0; i<tileObjectsFront.Length; i++ )
				{
					tileObjectRowFront = tileObjectsFront[i];
					tileObjectRowBack = tileObjectsBack[i];
					tileComponentRowFront = tileComponentsFront[i];
					tileComponentRowBack = tileComponentsBack[i];
					tileColliderMarkerRow = tileColliderMarkers[i];
					if( tileObjectRowFront != null )
					{
						if( i < newTileObjectsFront.Length )
						{
							newTileObjectRowFront = newTileObjectsFront[i];
							newTileObjectRowBack = newTileObjectsBack[i];
							newTileComponentRowFront = newTileComponentsFront[i];
							newTileComponentRowBack = newTileComponentsBack[i];
							newTileColliderMarkerRow = newTileColliderMarkers[i];
							for( int j=0; j<tileObjectRowFront.Length; j++ )
							{
								if( j < newTileObjectRowFront.Length )
								{
									newTileObjectRowFront[j] = tileObjectRowFront[j];
									newTileObjectRowBack[j] = tileObjectRowBack[j];
									newTileComponentRowFront[j] = tileComponentRowFront[j];
									newTileComponentRowBack[j] = tileComponentRowBack[j];
									newTileColliderMarkerRow[j] = tileColliderMarkerRow[j];
								}
								else
								{
									tileObject = tileColliderMarkerRow[j];
									if( tileObject != null )
									{
										Destroy( tileObject );
									}
									tileComponent = tileComponentRowFront[j];
									if( tileComponent != null )
									{
										/*halmeida - when clearing a tile, I have to destroy the colliders that the
										TileFactory added to it. This can't be done within the Tile's Clear function
										because the Tile is not the one that creates its colliders, so it can't really
										tell if it is ok to destroy them or not.*/
										TileFactory.RemoveColliders( tileComponent );
										tileComponent.Clear();
									}
									tileObject = tileObjectRowFront[j];
									if( tileObject != null )
									{
										Destroy( tileObject );
									}
									tileComponent = tileComponentRowBack[j];
									if( tileComponent != null )
									{
										tileComponent.Clear();
									}
									tileObject = tileObjectRowBack[j];
									if( tileObject != null )
									{
										Destroy( tileObject );
									}
								}
								tileColliderMarkerRow[j] = null;
								tileComponentRowFront[j] = null;
								tileComponentRowBack[j] = null;
								tileObjectRowFront[j] = null;
								tileObjectRowBack[j] = null;
							}
						}
						else
						{
							for( int j=0; j<tileObjectRowFront.Length; j++ )
							{
								tileObject = tileColliderMarkerRow[j];
								if( tileObject != null )
								{
									Destroy( tileObject );
									tileColliderMarkerRow[j] = null;
								}
								tileComponent = tileComponentRowFront[j];
								if( tileComponent != null )
								{
									/*halmeida - when clearing a tile, I have to destroy the colliders that the
									TileFactory added to it. This can't be done within the Tile's Clear function
									because the Tile is not the one that creates its colliders, so it can't really
									tell if it is ok to destroy them or not.*/
									TileFactory.RemoveColliders( tileComponent );
									tileComponent.Clear();
									tileComponentRowFront[j] = null;
								}
								tileObject = tileObjectRowFront[j];
								if( tileObject != null )
								{
									Destroy( tileObject );
									tileObjectRowFront[j] = null;
								}
								tileComponent = tileComponentRowBack[j];
								if( tileComponent != null )
								{
									tileComponent.Clear();
									tileComponentRowBack[j] = null;
								}
								tileObject = tileObjectRowBack[j];
								if( tileObject != null )
								{
									Destroy( tileObject );
									tileObjectRowBack[j] = null;
								}
							}
						}
						tileColliderMarkers[i] = null;
						tileComponentsFront[i] = null;
						tileComponentsBack[i] = null;
						tileObjectsFront[i] = null;
						tileObjectsBack[i] = null;
					}
				}
				tileColliderMarkers = null;
				tileComponentsFront = null;
				tileComponentsBack = null;
				tileObjectsFront = null;
				tileObjectsBack = null;
			}
			/*halmeida - we saved what could be saved from the old array of tiles into the new one.*/
			tileObjectsFront = newTileObjectsFront;
			tileObjectsBack = newTileObjectsBack;
			tileComponentsFront = newTileComponentsFront;
			tileComponentsBack = newTileComponentsBack;
			tileColliderMarkers = newTileColliderMarkers;
			newTileObjectsFront = null;
			newTileObjectsBack = null;
			newTileComponentsFront = null;
			newTileComponentsBack = null;
			newTileColliderMarkers = null;
			/*halmeida - we still need to review the shape of the tiles, for at least the tiles
			at the border of the new or the old structure may have changed their shapes with basis
			on connecting neighbors, since the border tiles are always considered connected to
			the inexistent tiles outside.*/
			oldTileRows = tileRows;
			oldTileColumns = tileColumns;
			tileRows = newRows;
			tileColumns = newColumns;
			structureWidth = tileColumns * tileWidthFront;
			structureHeight = tileRows * tileHeightFront;
			if( (oldTileRows > 0) && (oldTileColumns > 0) )
			{
				changedRow = ( oldTileRows > tileRows ) ? tileRows : oldTileRows;
				changedColumn = ( oldTileColumns > tileColumns ) ? tileColumns : oldTileColumns;
				changedRow--;
				changedColumn--;
				for( int i=0; i<tileObjectsFront.Length; i++ )
				{
					tileComponentRowFront = tileComponentsFront[i];
					tileComponentRowBack = tileComponentsBack[i];
					tileToOuterBoxRow = tileToOuterBox[i];
					tileToOuterTriRow = tileToOuterTri[i];
					if( i != changedRow )
					{
						/*halmeida - in this case we check only the changed column.*/
						tileComponent = tileComponentRowFront[changedColumn];
						if( tileComponent != null )
						{
							currentTileShapeIndex = tileComponent.GetShapeIndex();
							correctTileShapeIndex = GetTileShapeIndexFromBlueprint( i, changedColumn, true );
							if( currentTileShapeIndex != correctTileShapeIndex )
							{
								tileComponent = null;
								usesOuterCollider = ( (tileToOuterBoxRow[changedColumn] != -1) || (tileToOuterTriRow[changedColumn] != -1) );
								/*halmeida - the creation will already destroy the old tile at the position.*/
								CreateTile( i, changedColumn, true, correctTileShapeIndex, 0, !usesOuterCollider );
							}
						}
						else
						{
							/*halmeida - the creation will already destroy the old tile at the position.*/
							CreateTile( i, changedColumn, true, -1, 0, false );
						}
						tileComponent = tileComponentRowBack[changedColumn];
						if( tileComponent != null )
						{
							currentTileShapeIndex = tileComponent.GetShapeIndex();
							correctTileShapeIndex = GetTileShapeIndexFromBlueprint( i, changedColumn, false );
							if( currentTileShapeIndex != correctTileShapeIndex )
							{
								tileComponent = null;
								CreateTile( i, changedColumn, false, correctTileShapeIndex, 0, false );
							}
						}
						else
						{
							CreateTile( i, changedColumn, false, -1, 0, false );
						}
					}
					else
					{
						/*halmeida - in this case we check the entire row.*/
						for( int j=0; j<tileComponentRowFront.Length; j++ )
						{
							tileComponent = tileComponentRowFront[j];
							if( tileComponent != null )
							{
								currentTileShapeIndex = tileComponent.GetShapeIndex();
								correctTileShapeIndex = GetTileShapeIndexFromBlueprint( i, j, true );
								//Debug.Log("Debug : ChallengeStructure : tile ("+i+","+j+") tile code "+currentTileCode+" => "+correctTileCode+".");
								if( currentTileShapeIndex != correctTileShapeIndex )
								{
									tileComponent = null;
									usesOuterCollider = ( (tileToOuterBoxRow[j] != -1) || (tileToOuterTriRow[j] != -1) );
									/*halmeida - the creation will already destroy the old tile at the position.*/
									CreateTile( i, j, true, correctTileShapeIndex, 0, !usesOuterCollider );
								}
							}
							else
							{
								/*halmeida - the creation will already destroy the old tile at the position.*/
								CreateTile( i, j, true, -1, 0, false );
							}
							tileComponent = tileComponentRowBack[j];
							if( tileComponent != null )
							{
								currentTileShapeIndex = tileComponent.GetShapeIndex();
								correctTileShapeIndex = GetTileShapeIndexFromBlueprint( i, j, false );
								if( currentTileShapeIndex != correctTileShapeIndex )
								{
									tileComponent = null;
									CreateTile( i, j, false, correctTileShapeIndex, 0, false );
								}
							}
							else
							{
								CreateTile( i, j, false, -1, 0, false );
							}
						}
					}
				}
			}
			//Debug.Log("Debug : ChallengeStructure : new structure dimensions ("+tileRows+","+tileColumns+").");
		}
		UpdateColliderMarkers();
		ClearDoors();
	}

	private void ClearBlueprint( bool clearDependencies )
	{
		if( tileBlueprintFront != null )
		{
			for( int i=0; i<tileBlueprintFront.Length; i++ )
			{
				tileBlueprintFront[i] = null;
				tileBlueprintBack[i] = null;
				tileToOuterBox[i] = null;
				tileToOuterTri[i] = null;
			}
			tileBlueprintFront = null;
			tileBlueprintBack = null;
			tileToOuterBox = null;
			tileToOuterTri = null;
		}
		if( clearDependencies )
		{
			ClearTiles();
			tileRows = 0;
			tileColumns = 0;
			structureWidth = 0f;
			structureHeight = 0f;
			ClearDoors();
		}
	}

	private void ClearTiles()
	{
		GameObject[] tileObjectRowFront = null;
		GameObject[] tileObjectRowBack = null;
		Tile[] tileComponentRowFront = null;
		Tile[] tileComponentRowBack = null;
		GameObject[] tileColliderMarkerRow = null;
		GameObject tileObject = null;
		Tile tileComponent = null;

		if( tileObjectsFront != null )
		{
			buildingTileRowFront = -1;
			buildingTileRowBack = -1;
			buildingTileColumnFront = -1;
			buildingTileColumnBack = -1;
			buildingTilesFront = false;
			buildingTilesBack = false;
			tilesBuilt = false;
			ClearColliderMerging();
			ClearOuterColliders( false );
			for( int i=0; i<tileObjectsFront.Length; i++ )
			{
				tileObjectRowFront = tileObjectsFront[i];
				tileObjectRowBack = tileObjectsBack[i];
				tileComponentRowFront = tileComponentsFront[i];
				tileComponentRowBack = tileComponentsBack[i];
				tileColliderMarkerRow = tileColliderMarkers[i];
				if( tileObjectRowFront != null )
				{
					for( int j=0; j<tileObjectRowFront.Length; j++ )
					{
						/*halmeida - relying on the coheerence of all the "tile" arrays.*/
						tileObject = tileColliderMarkerRow[j];
						if( tileObject != null )
						{
							Destroy( tileObject );
							tileColliderMarkerRow[j] = null;
						}
						tileComponent = tileComponentRowFront[j];
						if( tileComponent != null )
						{
							/*halmeida - when clearing a tile, I have to destroy the colliders that the
							TileFactory added to it. This can't be done within the Tile's Clear function
							because the Tile is not the one that creates its colliders, so it can't really
							tell if it is ok to destroy them or not.*/
							TileFactory.RemoveColliders( tileComponent );
							tileComponent.Clear();
							tileComponentRowFront[j] = null;
						}
						tileObject = tileObjectRowFront[j];
						if( tileObject != null )
						{
							Destroy( tileObject );
							tileObjectRowFront[j] = null;
						}
						tileComponent = tileComponentRowBack[j];
						if( tileComponent != null )
						{
							/*halmeida - no need to remove colliders from a back tile because it should never
							receive one.*/
							tileComponent.Clear();
							tileComponentRowBack[j] = null;
						}
						tileObject = tileObjectRowBack[j];
						if( tileObject != null )
						{
							Destroy( tileObject );
							tileObjectRowBack[j] = null;
						}
					}
					tileColliderMarkers[i] = null;
					tileComponentsFront[i] = null;
					tileComponentsBack[i] = null;
					tileObjectsFront[i] = null;
					tileObjectsBack[i] = null;
				}
			}
			tileColliderMarkers = null;
			tileComponentsFront = null;
			tileComponentsBack = null;
			tileObjectsFront = null;
			tileObjectsBack = null;
		}
	}

	private void ClearDoors()
	{
		gapUpColumns = null;
		gapLeftRows = null;
		gapDownColumns = null;
		gapRightRows = null;
		doorsUp = null;
		doorsLeft = null;
		doorsDown = null;
		doorsRight = null;
	}

	private bool CreateTile( int row, int column, bool front, int shapeIndex, int textureVariation, bool withCollider )
	{
		GameObject oldTileObject = null;
		GameObject newTileObject = null;
		GameObject[] tileObjectRow = null;
		Tile[] tileComponentRow = null;
		GameObject[] tileColliderMarkerRow = null;
		Vector3 tileObjectPosition = Vector3.zero;
		Tile tileComponent = null;
		GameObject newColliderMarker = null;
		GameObject[][] tileObjects = null;
		TileFactory tileFactory = null;
		Tile[][] tileComponents = null;
		Transform layerTransform = null;
		float tileWidth = 0f;
		float tileHeight = 0f;

		if( (row < 0) || (column < 0) )
		{
			return false;
		}
		tileObjects = (front ? tileObjectsFront : tileObjectsBack);
		tileComponents = (front ? tileComponentsFront : tileComponentsBack);
		tileFactory = (front ? tileFactoryFront : tileFactoryBack);
		layerTransform = (front ? tileLayerFront.transform : tileLayerBack.transform);
		tileWidth = (front ? tileWidthFront : tileWidthBack);
		tileHeight = (front ? tileHeightFront : tileHeightBack);
		if( (tileObjects != null) && (tileFactory != null) )
		{
			if( row < tileObjects.Length )
			{
				tileObjectRow = tileObjects[row];
				tileComponentRow = tileComponents[row];
				tileColliderMarkerRow = tileColliderMarkers[row];
				if( tileObjectRow != null )
				{
					if( column < tileObjectRow.Length )
					{
						oldTileObject = tileObjectRow[column];
						if( oldTileObject != null )
						{
							if( front )
							{
								oldTileObject = tileColliderMarkerRow[column];
								if( oldTileObject != null )
								{
									Destroy( oldTileObject );
									tileColliderMarkerRow[column] = null;
								}
							}
							tileComponent = tileComponentRow[column];
							if( tileComponent != null )
							{
								/*halmeida - when clearing a tile, I have to destroy the colliders that the
								TileFactory added to it. This can't be done within the Tile's Clear function
								because the Tile is not the one that creates its colliders, so it can't really
								tell if it is ok to destroy them or not.*/
								if( front )
								{
									TileFactory.RemoveColliders( tileComponent );
								}
								tileComponent.Clear();
								tileComponentRow[column] = null;
							}
							oldTileObject = tileObjectRow[column];
							Destroy( oldTileObject );
							tileObjectRow[column] = null;
						}
						newTileObject = tileFactory.GetTileByShapeIndex( shapeIndex, textureVariation, withCollider, ref tileComponent );
						if( newTileObject != null )
						{
							if( front )
							{
								if( markColliders && (colliderMarkerModel != null) )
								{
									newColliderMarker = Instantiate( colliderMarkerModel, Vector3.zero, Quaternion.identity ) as GameObject;
								}
								else
								{
									newColliderMarker = null;
								}
							}
							tileObjectRow[column] = newTileObject;
							tileComponentRow[column] = tileComponent;
							if( front )
							{
								tileColliderMarkerRow[column] = newColliderMarker;
							}
							newTileObject.transform.SetParent( layerTransform );
							if( newColliderMarker != null )
							{
								newColliderMarker.transform.SetParent( colliderMarkerLayer.transform );
							}
							tileObjectPosition.x = (column + 0.5f) * tileWidth;
							tileObjectPosition.y = -1f * (row + 0.5f) * tileHeight;
							tileObjectPosition.z = 0f;
							newTileObject.transform.localPosition = tileObjectPosition;
							if( newColliderMarker != null )
							{
								newColliderMarker.transform.localPosition = tileObjectPosition;
								newColliderMarker.SetActive( showingColliderMarkers );
							}
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private bool CreateAllTilesFromBlueprint()
	{
		GameObject[] tileObjectRowFront = null;
		GameObject[] tileObjectRowBack = null;
		Tile[] tileComponentRowFront = null;
		Tile[] tileComponentRowBack = null;
		GameObject[] tileColliderMarkerRow = null;
		int[] blueprintRowFront = null;
		int[] blueprintRowBack = null;
		int[] tileToOuterBoxRow = null;
		int[] tileToOuterTriRow = null;
		int tileShapeIndex = -1;
		int blueprintCode = BLUEPRINT_CODE_INVALID;
		int textureVariation = 0;
		bool tileFailed = false;
		bool usesOuterCollider = false;

		if( tileBlueprintFront == null )
		{
			return false;
		}
		if( tileObjectsFront == null )
		{
			/*halmeida - create all the room for tiles. (all the tile pointers)*/
			tileObjectsFront = new GameObject[tileRows][];
			tileObjectsBack = new GameObject[tileRows][];
			tileComponentsFront = new Tile[tileRows][];
			tileComponentsBack = new Tile[tileRows][];
			tileColliderMarkers = new GameObject[tileRows][];
			for( int i=0; i<tileRows; i++ )
			{
				tileObjectRowFront = new GameObject[tileColumns];
				tileObjectRowBack = new GameObject[tileColumns];
				tileComponentRowFront = new Tile[tileColumns];
				tileComponentRowBack = new Tile[tileColumns];
				tileColliderMarkerRow = new GameObject[tileColumns];
				for( int j=0; j<tileColumns; j++ )
				{
					tileObjectRowFront[j] = null;
					tileObjectRowBack[j] = null;
					tileComponentRowFront[j] = null;
					tileComponentRowBack[j] = null;
					tileColliderMarkerRow[j] = null;
				}
				tileObjectsFront[i] = tileObjectRowFront;
				tileObjectsBack[i] = tileObjectRowBack;
				tileComponentsFront[i] = tileComponentRowFront;
				tileComponentsBack[i] = tileComponentRowBack;
				tileColliderMarkers[i] = tileColliderMarkerRow;
			}
		}
		/*halmeida - look at each position in the blueprint.*/
		for( int i=0; i<tileRows; i++ )
		{
			blueprintRowFront = tileBlueprintFront[i];
			blueprintRowBack = tileBlueprintBack[i];
			tileToOuterBoxRow = tileToOuterBox[i];
			tileToOuterTriRow = tileToOuterTri[i];
			for( int j=0; j<tileColumns; j++ )
			{
				blueprintCode = blueprintRowFront[j];
				if( IsTilePresenceCode( blueprintCode ) )
				{
					tileShapeIndex = GetTileShapeIndexFromBlueprint( i, j, true );
					if( tileShapeIndex > -1 )
					{
						usesOuterCollider = ( (tileToOuterBoxRow[j] != -1) || (tileToOuterTriRow[j] != -1) );
						textureVariation = GetTileTextureVariation( blueprintCode );
						if( !CreateTile( i, j, true, tileShapeIndex, textureVariation, !usesOuterCollider ) )
						{
							tileFailed = true;
						}
					}
				}
				else
				{
					CreateTile( i, j, true, -1, 0, false );
				}
				blueprintCode = blueprintRowBack[j];
				if( IsTilePresenceCode( blueprintCode ) )
				{
					tileShapeIndex = GetTileShapeIndexFromBlueprint( i, j, false );
					if( tileShapeIndex > -1 )
					{
						textureVariation = GetTileTextureVariation( blueprintCode );
						if( !CreateTile( i, j, false, tileShapeIndex, textureVariation, false ) )
						{
							tileFailed = true;
						}
					}
				}
			}
		}
		tilesBuilt = true;
		return !tileFailed;
	}

	private bool CreateSomeTilesFromBlueprint( int amountToCreate )
	{
		GameObject[] tileObjectRowFront = null;
		GameObject[] tileObjectRowBack = null;
		Tile[] tileComponentRowFront = null;
		Tile[] tileComponentRowBack = null;
		GameObject[] tileColliderMarkerRow = null;
		int[] blueprintRow = null;
		int[] tileToOuterBoxRow = null;
		int[] tileToOuterTriRow = null;
		int tileShapeIndex = -1;
		bool tileFailed = false;
		bool usesOuterCollider = false;
		int startingColumn = 0;
		int amountCreated = 0;
		int[][] tileBlueprint = null;
		int buildingTileRow = -1;
		int buildingTileColumn = -1;
		bool front = false;
		int blueprintCode = BLUEPRINT_CODE_INVALID;
		int textureVariation = 0;

		if( (tileBlueprintFront == null) || (amountToCreate <= 0) )
		{
			return false;
		}
		if( tileObjectsFront == null )
		{
			/*halmeida - create all the room for tiles. (all the tile pointers)*/
			tileObjectsFront = new GameObject[tileRows][];
			tileObjectsBack = new GameObject[tileRows][];
			tileComponentsFront = new Tile[tileRows][];
			tileComponentsBack = new Tile[tileRows][];
			tileColliderMarkers = new GameObject[tileRows][];
			for( int i=0; i<tileRows; i++ )
			{
				tileObjectRowFront = new GameObject[tileColumns];
				tileObjectRowBack = new GameObject[tileColumns];
				tileComponentRowFront = new Tile[tileColumns];
				tileComponentRowBack = new Tile[tileColumns];
				tileColliderMarkerRow = new GameObject[tileColumns];
				for( int j=0; j<tileColumns; j++ )
				{
					tileObjectRowFront[j] = null;
					tileObjectRowBack[j] = null;
					tileComponentRowFront[j] = null;
					tileComponentRowBack[j] = null;
					tileColliderMarkerRow[j] = null;
				}
				tileObjectsFront[i] = tileObjectRowFront;
				tileObjectsBack[i] = tileObjectRowBack;
				tileComponentsFront[i] = tileComponentRowFront;
				tileComponentsBack[i] = tileComponentRowBack;
				tileColliderMarkers[i] = tileColliderMarkerRow;
			}
		}
		if( !buildingTilesFront && !buildingTilesBack )
		{
			buildingTilesFront = true;
			buildingTileRowFront = 0;
			buildingTileColumnFront = 0;
			tilesBuilt = false;
		}
		if( buildingTilesFront )
		{
			tileBlueprint = tileBlueprintFront;
			buildingTileRow = buildingTileRowFront;
			buildingTileColumn = buildingTileColumnFront;
			front = true;
		}
		else if( buildingTilesBack )
		{
			tileBlueprint = tileBlueprintBack;
			buildingTileRow = buildingTileRowBack;
			buildingTileColumn = buildingTileColumnBack;
		}
		if( buildingTilesFront || buildingTilesBack )
		{
			/*halmeida - look at each position in the blueprint.*/
			for( int i=buildingTileRow; i<tileRows; i++ )
			{
				blueprintRow = tileBlueprint[i];
				tileToOuterBoxRow = tileToOuterBox[i];
				tileToOuterTriRow = tileToOuterTri[i];
				if( i == buildingTileRow )
				{
					startingColumn = buildingTileColumn;
				}
				else
				{
					startingColumn = 0;
				}
				for( int j=startingColumn; j<tileColumns; j++ )
				{
					blueprintCode = blueprintRow[j];
					if( IsTilePresenceCode( blueprintCode ) )
					{
						tileShapeIndex = GetTileShapeIndexFromBlueprint( i, j, front );
						if( tileShapeIndex > -1 )
						{
							textureVariation = GetTileTextureVariation( blueprintCode );
							if( front )
							{
								usesOuterCollider = ( (tileToOuterBoxRow[j] != -1) || (tileToOuterTriRow[j] != -1) );
								if( !CreateTile( i, j, true, tileShapeIndex, textureVariation, !usesOuterCollider ) )
								{
									tileFailed = true;
								}
							}
							else
							{
								if( !CreateTile( i, j, false, tileShapeIndex, textureVariation, false ) )
								{
									tileFailed = true;
								}
							}
							/*halmeida - since the function CreateTile may do a lot of work even when the tile
							creation fails, we count the flawed attempt as a creation.*/
							amountCreated++;
						}
					}
					else
					{
						CreateTile( i, j, front, -1, 0, false );
					}
					/*halmeida - the creation has to stop in one of two conditions : the amount to create is reached
					or the creation is completed before reaching that amount.*/
					if( amountCreated == amountToCreate )
					{
						/*halmeida - we save the building state corresponding to the next tile.*/
						if( front )
						{
							buildingTileRowFront = i;
							buildingTileColumnFront = j+1;
							if( buildingTileColumnFront == tileColumns )
							{
								buildingTileColumnFront = 0;
								buildingTileRowFront++;
								if( buildingTileRowFront == tileRows )
								{
									buildingTileColumnFront = -1;
									buildingTileRowFront = -1;
									buildingTilesFront = false;
									buildingTilesBack = true;
									buildingTileRowBack = 0;
									buildingTileColumnBack = 0;
								}
							}
						}
						else
						{
							buildingTileRowBack = i;
							buildingTileColumnBack = j+1;
							if( buildingTileColumnBack == tileColumns )
							{
								buildingTileColumnBack = 0;
								buildingTileRowBack++;
								if( buildingTileRowBack == tileRows )
								{
									buildingTileColumnBack = -1;
									buildingTileRowBack = -1;
									buildingTilesBack = false;
									tilesBuilt = true;
								}
							}
						}
						break;
					}
				}
				if( amountCreated == amountToCreate )
				{
					break;
				}
				else if( i == (tileRows-1) )
				{
					if( buildingTilesFront )
					{
						buildingTileColumnFront = -1;
						buildingTileRowFront = -1;
						buildingTilesFront = false;
						buildingTilesBack = true;
						buildingTileRowBack = 0;
						buildingTileColumnBack = 0;
						//Debug.Log("Debug : ChallengeStructure : completed front tile construction.");
					}
					else
					{
						buildingTileColumnBack = -1;
						buildingTileRowBack = -1;
						buildingTilesBack = false;
						tilesBuilt = true;
						//Debug.Log("Debug : ChallengeStructure : completed back tile construction.");
					}
				}
			}
		}
		//Debug.Log("Debug : ChallengeStructure : created "+amountCreated+" tiles.");
		return !tileFailed;
	}

	public void ToggleTileColliders( bool enable )
	{
		Tile[] tileComponentRow = null;
		Tile tileComponent = null;
		BoxCollider2D colliderBox = null;
		PolygonCollider2D colliderPolygon = null;

		if( (tileComponentsFront != null) && (tileRows > 0) && (tileColumns > 0) )
		{
			for( int i=0; i<tileRows; i++ )
			{
				tileComponentRow = tileComponentsFront[i];
				for( int j=0; j<tileColumns; j++ )
				{
					tileComponent = tileComponentRow[j];
					if( tileComponent != null )
					{
						colliderBox = tileComponent.GetColliderBox();
						if( colliderBox != null )
						{
							colliderBox.enabled = enable;
						}
						colliderPolygon = tileComponent.GetColliderPolygon();
						if( colliderPolygon != null )
						{
							colliderPolygon.enabled = enable;
						}
					}
				}
			}
		}
	}

	public void SetBlueprintValue( int row, int column, bool front, int value, bool updateTile, bool reviewNeighbors, bool updateColliderMarkers )
	{
		int[] blueprintRow = null;
		int[] tileToOuterBoxRow = null;
		int[] tileToOuterTriRow = null;
		int blueprintCode = BLUEPRINT_CODE_INVALID;
		int reviewRow = -1;
		int reviewColumn = -1;
		int shapeIndex = -1;
		int[][] tileBlueprint = null;
		int textureVariation = 0;

		tileBlueprint = (front ? tileBlueprintFront : tileBlueprintBack);
		if( tileBlueprint != null )
		{
			if( (row > -1) && (row < tileRows) )
			{
				blueprintRow = tileBlueprint[row];
				if( front )
				{
					tileToOuterBoxRow = tileToOuterBox[row];
					tileToOuterTriRow = tileToOuterTri[row];
				}
				if( (column > -1) && (column < tileColumns) )
				{
					blueprintRow[column] = value;
					/*halmeida - gotta check if this position was linked to an outer collider. If it was, I gotta
					remove that outer collider entirely, updating other positions that also linked to it.*/
					if( front )
					{
						RemoveOuterCollider( tileToOuterBoxRow[column], true, true );
						RemoveOuterCollider( tileToOuterTriRow[column], false, true );
					}
					if( updateTile )
					{
						if( reviewNeighbors )
						{
							/*halmeida - recreate the 9 tiles possibly affected by the change.*/
							reviewRow = row - 1;
							while( reviewRow < row + 2 )
							{
								if( (reviewRow > -1) && (reviewRow < tileRows) )
								{
									blueprintRow = tileBlueprint[reviewRow];
									if( front )
									{
										tileToOuterBoxRow = tileToOuterBox[reviewRow];
										tileToOuterTriRow = tileToOuterTri[reviewRow];
									}
									reviewColumn = column - 1;
									while( reviewColumn < column + 2 )
									{
										if( (reviewColumn > -1) && (reviewColumn < tileColumns) )
										{
											blueprintCode = blueprintRow[reviewColumn];
											if( front )
											{
												RemoveOuterCollider( tileToOuterBoxRow[reviewColumn], true, true );
												RemoveOuterCollider( tileToOuterTriRow[reviewColumn], false, true );
											}
											if( IsTilePresenceCode( blueprintCode ) )
											{
												shapeIndex = GetTileShapeIndexFromBlueprint( reviewRow, reviewColumn, front );
												/*halmeida - at this point I should do some correction at the blueprint if
												it had a triangular tile code and the tile can no longer be triangular.*/
												if( IsTriangleTileCode( blueprintCode ) )
												{
													if( !Tile.ShapeIndexShouldBeTriangle( shapeIndex ) )
													{
														blueprintCode = BLUEPRINT_CODE_TILE_NORMAL;
														blueprintRow[reviewColumn] = blueprintCode;
													}
												}
												textureVariation = GetTileTextureVariation( blueprintCode );
												CreateTile( reviewRow, reviewColumn, front, shapeIndex, textureVariation, front );
											}
											else
											{
												CreateTile( reviewRow, reviewColumn, front, -1, 0, front );
											}
										}
										reviewColumn++;
									}
								}
								reviewRow++;
							}
						}
						else
						{
							if( IsTilePresenceCode( value ) )
							{
								shapeIndex = GetTileShapeIndexFromBlueprint( row, column, front );
								textureVariation = GetTileTextureVariation( value );
								CreateTile( row, column, front, shapeIndex, textureVariation, front );
							}
							else
							{
								CreateTile( row, column, front, -1, 0, false );
							}
						}
					}
					if( updateColliderMarkers )
					{
						UpdateColliderMarkers();
					}
				}
			}
		}
	}

	private void ClearOuterColliders( bool updateTiles )
	{
		while( outerColliderBox != null )
		{
			RemoveOuterCollider( 0, true, updateTiles );
		}
		while( outerColliderTri != null )
		{
			RemoveOuterCollider( 0, false, updateTiles );
		}
	}

	private void RemoveOuterCollider( int outerColliderIndex, bool boxType, bool restoreColliders )
	{
		int[] tileToOuterBoxRow = null;
		int[] tileToOuterTriRow = null;
		BoxCollider2D colliderBox = null;
		PolygonCollider2D colliderTri = null;

		if( outerColliderIndex > -1 )
		{
			if( boxType && (outerColliderBox != null) )
			{
				if( outerColliderBox.Length > outerColliderIndex )
				{
					colliderBox = outerColliderBox[outerColliderIndex];
					UsefulFunctions.DecreaseArray<BoxCollider2D>( ref outerColliderBox, outerColliderIndex );
					UsefulFunctions.DecreaseArray<int>( ref outerBoxFirstRows, outerColliderIndex );
					UsefulFunctions.DecreaseArray<int>( ref outerBoxFirstColumns, outerColliderIndex );
					UsefulFunctions.DecreaseArray<int>( ref outerBoxLastRows, outerColliderIndex );
					UsefulFunctions.DecreaseArray<int>( ref outerBoxLastColumns, outerColliderIndex );
					/*halmeida - the collider was removed from the array, but since it is a script attached
					to an object, it still needs to be destroyed so that the component is removed from the
					object.*/
					if( colliderBox != null )
					{
						Destroy( colliderBox );
						colliderBox = null;
					}
					if( (tileToOuterBox != null) && restoreColliders )
					{
						/*halmeida - every tile linked to this outer collider should now link to collider of index -1.
						Every tile linked to outer colliders of higher indexes should link to colliders of indexes one
						unit lower.*/
						for( int i=0; i<tileToOuterBox.Length; i++ )
						{
							tileToOuterBoxRow = tileToOuterBox[i];
							if( tileToOuterBoxRow != null )
							{
								for( int j=0; j<tileToOuterBoxRow.Length; j++ )
								{
									if( tileToOuterBoxRow[j] == outerColliderIndex )
									{
										tileToOuterBoxRow[j] = -1;
										RestoreOriginalCollider( i, j );
									}
									else if( tileToOuterBoxRow[j] > outerColliderIndex )
									{
										tileToOuterBoxRow[j]--;
									}
								}
							}
						}
					}
				}
			}
			if( !boxType && (outerColliderTri != null) )
			{
				if( outerColliderTri.Length > outerColliderIndex )
				{
					colliderTri = outerColliderTri[outerColliderIndex];
					UsefulFunctions.DecreaseArray<PolygonCollider2D>( ref outerColliderTri, outerColliderIndex );
					UsefulFunctions.DecreaseArray<int>( ref outerTriFirstRows, outerColliderIndex );
					UsefulFunctions.DecreaseArray<int>( ref outerTriFirstColumns, outerColliderIndex );
					UsefulFunctions.DecreaseArray<int>( ref outerTriLastRows, outerColliderIndex );
					UsefulFunctions.DecreaseArray<int>( ref outerTriLastColumns, outerColliderIndex );
					UsefulFunctions.DecreaseArray<int>( ref outerTriCorners, outerColliderIndex );
					/*halmeida - the collider was removed from the array, but since it is a script attached
					to an object, it still needs to be destroyed so that the component is removed from the
					object.*/
					if( colliderTri != null )
					{
						Destroy( colliderTri );
						colliderTri = null;
					}
					if( (tileToOuterTri != null) && restoreColliders )
					{
						/*halmeida - every tile linked to this outer collider should now link to collider of index -1.
						Every tile linked to outer colliders of higher indexes should link to colliders of indexes one
						unit lower.*/
						for( int i=0; i<tileToOuterTri.Length; i++ )
						{
							tileToOuterTriRow = tileToOuterTri[i];
							if( tileToOuterTriRow != null )
							{
								for( int j=0; j<tileToOuterTriRow.Length; j++ )
								{
									if( tileToOuterTriRow[j] == outerColliderIndex )
									{
										tileToOuterTriRow[j] = -1;
										RestoreOriginalCollider( i, j );
									}
									else if( tileToOuterTriRow[j] > outerColliderIndex )
									{
										tileToOuterTriRow[j]--;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private void RestoreOriginalCollider( int row, int column )
	{
		Tile[] tileComponentRow = null;
		Tile tileComponent = null;

		if( (tileComponentsFront != null) && (row > -1) && (column > -1) && (tileFactoryFront != null) )
		{
			if( tileComponentsFront.Length > row )
			{
				tileComponentRow = tileComponentsFront[row];
				if( tileComponentRow != null )
				{
					if( tileComponentRow.Length > column )
					{
						tileComponent = tileComponentRow[column];
						if( tileComponent != null )
						{
							tileFactoryFront.AddPropperCollider( tileComponent );
						}
					}
				}
			}
		}
	}

	public int GetBlueprintValue( int row, int column, bool front )
	{
		int[] blueprintRow = null;
		int[][] tileBlueprint = null;

		tileBlueprint = (front ? tileBlueprintFront : tileBlueprintBack);
		if( tileBlueprint != null )
		{
			if( (row > -1) && (row < tileRows) && (column > -1) && (column < tileColumns) )
			{
				blueprintRow = tileBlueprint[row];
				return blueprintRow[column];
			}
		}
		return BLUEPRINT_CODE_INVALID;
	}

	public int[][] GetBlueprintMirror( bool front )
	{
		int[][] tileBlueprintMirror = null;
		int[] blueprintRow = null;
		int[] blueprintRowMirror = null;
		int[][] tileBlueprint = null;

		tileBlueprint = (front ? tileBlueprintFront : tileBlueprintBack);
		if( tileBlueprint != null )
		{
			tileBlueprintMirror = new int[tileRows][];
			for( int i=0; i<tileRows; i++ )
			{
				blueprintRow = tileBlueprint[i];
				blueprintRowMirror = new int[tileColumns];
				for( int j=0; j<tileColumns; j++ )
				{
					blueprintRowMirror[j] = blueprintRow[tileColumns-1-j];
				}
				tileBlueprintMirror[i] = blueprintRowMirror;
			}
		}
		return tileBlueprintMirror;
	}

	public int[][] GetBlueprintToOuterColliderMirror( bool boxType )
	{
		int[][] tileToOuter = null;
		int[][] tileToOuterMirror = null;
		int[] tileToOuterRow = null;
		int[] tileToOuterMirrorRow = null;

		tileToOuter = (boxType ? tileToOuterBox : tileToOuterTri);
		if( tileToOuter != null )
		{
			tileToOuterMirror = new int[tileRows][];
			for( int i=0; i<tileRows; i++ )
			{
				tileToOuterRow = tileToOuter[i];
				tileToOuterMirrorRow = new int[tileColumns];
				for( int j=0; j<tileColumns; j++ )
				{
					tileToOuterMirrorRow[j] = tileToOuterRow[tileColumns-1-j];
				}
				tileToOuterMirror[i] = tileToOuterMirrorRow;
			}
		}
		return tileToOuterMirror;
	}

	public Vector2[] GetItemOffsetsMirror()
	{
		Vector2[] mirroredOffsets = null;
		int length = 0;

		if( (itemOffsets != null) && (structureWidth > 0f) )
		{
			length = itemOffsets.Length;
			if( length > 0 )
			{
				mirroredOffsets = new Vector2[length];
				for( int i=0; i<length; i++ )
				{
					mirroredOffsets[i] = new Vector2(structureWidth - itemOffsets[i].x, itemOffsets[i].y);
				}
			}
		}
		return mirroredOffsets;
	}

	private void GetOuterBoxLimitsMirror( ref int[] firstRows, ref int[] firstColumns, ref int[] lastRows, ref int[] lastColumns )
	{
		int totalOuterBoxes = 0;

		if( (tileBlueprintFront != null) && (outerBoxFirstRows != null) )
		{
			totalOuterBoxes = outerBoxFirstRows.Length;
			if( totalOuterBoxes > 0 )
			{
				firstRows = new int[totalOuterBoxes];
				firstColumns = new int[totalOuterBoxes];
				lastRows = new int[totalOuterBoxes];
				lastColumns = new int[totalOuterBoxes];
				for( int i=0; i<totalOuterBoxes; i++ )
				{
					firstRows[i] = outerBoxFirstRows[i];
					firstColumns[i] = tileColumns-1-outerBoxLastColumns[i];
					lastRows[i] = outerBoxLastRows[i];
					lastColumns[i] = tileColumns-1-outerBoxFirstColumns[i];
				}
			}
		}
	}

	private void GetOuterTriLimitsMirror( ref int[] firstRows, ref int[] firstColumns, ref int[] lastRows, ref int[] lastColumns,
		ref int[] corners )
	{
		int totalOuterTris = 0;

		if( (tileBlueprintFront != null) && (outerTriFirstRows != null) )
		{
			totalOuterTris = outerTriFirstRows.Length;
			if( totalOuterTris > 0 )
			{
				firstRows = new int[totalOuterTris];
				firstColumns = new int[totalOuterTris];
				lastRows = new int[totalOuterTris];
				lastColumns = new int[totalOuterTris];
				corners = new int[totalOuterTris];
				for( int i=0; i<totalOuterTris; i++ )
				{
					firstRows[i] = outerTriFirstRows[i];
					firstColumns[i] = tileColumns-1-outerTriLastColumns[i];
					lastRows[i] = outerTriLastRows[i];
					lastColumns[i] = tileColumns-1-outerTriFirstColumns[i];
					corners[i] = TRIANGLE_CORNER_INVALID;
					switch( outerTriCorners[i] )
					{
						case TRIANGLE_CORNER_UL:
							corners[i] = TRIANGLE_CORNER_UR;
							break;
						case TRIANGLE_CORNER_LD:
							corners[i] = TRIANGLE_CORNER_DR;
							break;
						case TRIANGLE_CORNER_DR:
							corners[i] = TRIANGLE_CORNER_LD;
							break;
						case TRIANGLE_CORNER_UR:
							corners[i] = TRIANGLE_CORNER_UL;
							break;
					}
				}
			}
		}
	}

	public Vector2[] GetWaypointOffsetsMirror()
	{
		Vector2[] mirroredOffsets = null;
		int length = 0;

		if( (waypointOffsets != null) && (structureWidth > 0f) )
		{
			length = waypointOffsets.Length;
			if( length > 0 )
			{
				mirroredOffsets = new Vector2[length];
				for( int i=0; i<length; i++ )
				{
					mirroredOffsets[i] = new Vector2(structureWidth - waypointOffsets[i].x, waypointOffsets[i].y);
				}
			}
		}
		return mirroredOffsets;
	}

	public Vector2[] GetEnemyOffsetsMirror()
	{
		Vector2[] mirroredOffsets = null;
		int length = 0;

		if( (enemyOffsets != null) && (structureWidth > 0f) )
		{
			length = enemyOffsets.Length;
			if( length > 0 )
			{
				mirroredOffsets = new Vector2[length];
				for( int i=0; i<length; i++ )
				{
					mirroredOffsets[i] = new Vector2(structureWidth - enemyOffsets[i].x, enemyOffsets[i].y);
				}
			}
		}
		return mirroredOffsets;
	}

	public static bool IsTilePresenceCode( int blueprintCode )
	{
		return ( IsNormalTileCode( blueprintCode ) || IsTriangleTileCode( blueprintCode ) );
	}

	public static bool IsNormalTileCode( int blueprintCode )
	{
		return ( (blueprintCode == BLUEPRINT_CODE_TILE_NORMAL) ||
			((blueprintCode >= BLUEPRINT_CODE_TILE_N_SECOND) && (blueprintCode <= BLUEPRINT_CODE_TILE_N_LAST)) );
	}

	public static bool IsTriangleTileCode( int blueprintCode )
	{
		return ( (blueprintCode == BLUEPRINT_CODE_TILE_TRIANGLE) ||
			((blueprintCode >= BLUEPRINT_CODE_TILE_T_SECOND) && (blueprintCode <= BLUEPRINT_CODE_TILE_T_LAST)) );
	}

	public bool GetRowAndColumnAtPosition( ref int row, ref int column, bool front, Vector2 worldPosition )
	{
		Vector2 positionWithin = Vector2.zero;
		float tileWidth = 0f;
		float tileHeight = 0f;
		float rowFloat = 0f;
		float columnFloat = 0f;
		float backToFloat = 0f;

		tileWidth = (front ? tileWidthFront : tileWidthBack);
		tileHeight = (front ? tileHeightFront : tileHeightBack);
		if( (tileWidth == 0f) || (tileHeight == 0f) )
		{
			return false;
		}
		positionWithin = new Vector2( worldPosition.x - gameObject.transform.position.x, worldPosition.y - gameObject.transform.position.y );
		rowFloat = -positionWithin.y / tileHeight;
		row = (int) rowFloat;
		if( positionWithin.y > 0f )
		{
			backToFloat = row;
			if( backToFloat != rowFloat )
			{
				row--;
			}
		}
		columnFloat = positionWithin.x / tileWidth;
		column = (int) columnFloat;
		if( positionWithin.x < 0f )
		{
			backToFloat = column;
			if( backToFloat != columnFloat )
			{
				column--;
			}
		}
		return true;
	}

	public bool GetPositionForRowAndColumn( int row, int column, bool front, ref Vector2 worldPosition )
	{
		float offset = 0f;
		float tileWidth = 0f;
		float tileHeight = 0f;

		tileWidth = (front ? tileWidthFront : tileWidthBack);
		tileHeight = (front ? tileHeightFront : tileHeightBack);
		if( (tileWidth == 0f) || (tileHeight == 0f) )
		{
			return false;
		}
		offset = gameObject.transform.position.x + (column + 0.5f) * tileWidth;
		worldPosition.x = offset;
		offset = gameObject.transform.position.y - (row + 0.5f) * tileHeight;
		worldPosition.y = offset;
		return true;
	}

	public bool BreakTile( int row, int column, bool front )
	{
		if( IsTilePresenceCode( GetBlueprintValue( row, column, front ) ) )
		{
			SetBlueprintValue( row, column, front, BLUEPRINT_CODE_NO_TILE, true, false, true );
			return true;
		}
		return false;
	}

	public void AddItemData( int itemID, Vector2 itemWorldOffset )
	{
		UsefulFunctions.IncreaseArray<int>( ref itemIDs, itemID );
		UsefulFunctions.IncreaseArray<Vector2>( ref itemOffsets, itemWorldOffset );
		UsefulFunctions.IncreaseArray<int>( ref itemToItemClusters, -1 );
	}

	public void AddItemClusterData( int clusterRewardCode )
	{
		UsefulFunctions.IncreaseArray<int>( ref itemClusterRewardCodes, clusterRewardCode );
	}

	public void AlterItemDataOffset( int itemIndex, Vector2 newItemOffset )
	{
		if( (itemIndex > -1) && (itemOffsets != null) )
		{
			if( itemIndex < itemOffsets.Length )
			{
				itemOffsets[itemIndex] = newItemOffset;
			}
		}
	}

	public void AlterItemDataClusterPertinence( int itemIndex, int newItemClusterIndex )
	{
		if( (itemIndex > -1) && (itemToItemClusters != null) && (newItemClusterIndex > -1) && (itemClusterRewardCodes != null) )
		{
			if( (itemIndex < itemToItemClusters.Length) && (newItemClusterIndex < itemClusterRewardCodes.Length) )
			{
				itemToItemClusters[itemIndex] = newItemClusterIndex;
			}
		}
	}

	public void RemoveItemData( int itemIndex )
	{
		UsefulFunctions.DecreaseArray<int>( ref itemIDs, itemIndex );
		UsefulFunctions.DecreaseArray<Vector2>( ref itemOffsets, itemIndex );
		UsefulFunctions.DecreaseArray<int>( ref itemToItemClusters, itemIndex );
	}

	public void RemoveItemClusterData( int itemClusterIndex )
	{
		int tempClusterIndex = -1;

		UsefulFunctions.DecreaseArray<int>( ref itemClusterRewardCodes, itemClusterIndex );
		/*halmeida - since each item was referencing an item cluster through an item cluster index,
		we have to update these indexes so that the items keep on referencing the same clusters.*/
		if( itemToItemClusters != null )
		{
			for( int  i=0; i<itemToItemClusters.Length; i++ )
			{
				tempClusterIndex = itemToItemClusters[i];
				if( tempClusterIndex == itemClusterIndex )
				{
					itemToItemClusters[i] = -1;
				}
				else if( tempClusterIndex > itemClusterIndex )
				{
					itemToItemClusters[i] = tempClusterIndex-1;
				}
			}
		}
	}

	public void ClearItemData()
	{
		itemIDs = null;
		itemOffsets = null;
		itemToItemClusters = null;
	}

	public void ClearItemClusterData()
	{
		itemClusterRewardCodes = null;
		/*halmeida - if the items were still referencing item clusters through indexes, they must all
		be invalidated.*/
		if( itemToItemClusters != null )
		{
			for( int i=0; i<itemToItemClusters.Length; i++ )
			{
				itemToItemClusters[i] = -1;
			}
		}
	}

	public void AddWaypointData( Vector2 waypointOffset, int waypointID = Waypoint.INVALID_WAYPOINT_ID )
	{
		UsefulFunctions.IncreaseArray<Vector2>( ref waypointOffsets, waypointOffset );
		UsefulFunctions.IncreaseArray<int>( ref waypointIDs, waypointID ); 
	}

	public void AlterWaypointDataOffset( int waypointIndex, Vector2 newWaypointOffset )
	{
		if( waypointOffsets != null )
		{
			if( (waypointIndex > -1) && (waypointIndex < waypointOffsets.Length) )
			{
				waypointOffsets[waypointIndex] = newWaypointOffset;
			}
		}
	}

	public void AlterWaypointDataID( int waypointIndex, int newWaypointID )
	{
		if( waypointIDs != null )
		{
			if( (waypointIndex > -1) && (waypointIndex < waypointIDs.Length) )
			{
				waypointIDs[waypointIndex] = newWaypointID;
			}
		}
	}

	public void RemoveWaypointData( int waypointIndex )
	{
		UsefulFunctions.DecreaseArray<Vector2>( ref waypointOffsets, waypointIndex );
		UsefulFunctions.DecreaseArray<int>( ref waypointIDs, waypointIndex );
		RemoveWaypointFromRoutes( waypointIndex );
	}

	public void ClearWaypointData()
	{
		waypointOffsets = null;
		waypointIDs = null;
		ClearEnemyRoutes();
	}

	public void AddEnemyData( int enemyCode, Vector2 enemyWorldOffset )
	{
		UsefulFunctions.IncreaseArray<int>( ref enemyCodes, enemyCode );
		UsefulFunctions.IncreaseArray<Vector2>( ref enemyOffsets, enemyWorldOffset );
		UsefulFunctions.IncreaseArray<RoutePersistentData>( ref enemyRoutes, null );
	}

	public void AlterEnemyDataOffset( int enemyIndex, Vector2 newEnemyOffset )
	{
		if( enemyOffsets != null )
		{
			if( (enemyIndex > -1) && (enemyIndex < enemyOffsets.Length) )
			{
				enemyOffsets[enemyIndex] = newEnemyOffset;
			}
		}
	}

	public void AlterEnemyDataRoute( int enemyIndex, RoutePersistentData newEnemyRoute )
	{
		int length = 0;

		if( enemyRoutes == null )
		{
			if( enemyCodes != null )
			{
				length = enemyCodes.Length;
				enemyRoutes = new RoutePersistentData[length];
				for( int i=0; i<length; i++ )
				{
					if( i == enemyIndex )
					{
						enemyRoutes[i] = newEnemyRoute;
					}
					else
					{
						enemyRoutes[i] = null;
					}
				}
			}
		}
		else
		{
			if( (enemyIndex > -1) && (enemyIndex < enemyRoutes.Length) )
			{
				enemyRoutes[enemyIndex] = newEnemyRoute;
			}
		}
	}

	public void RemoveEnemyData( int enemyIndex )
	{
		RoutePersistentData routeData = null;

		UsefulFunctions.DecreaseArray<int>( ref enemyCodes, enemyIndex );
		UsefulFunctions.DecreaseArray<Vector2>( ref enemyOffsets, enemyIndex );
		if( enemyRoutes != null )
		{
			if( (enemyIndex > -1) && (enemyIndex < enemyRoutes.Length) )
			{
				routeData = enemyRoutes[enemyIndex];
				if( routeData != null )
				{
					routeData.Clear();
					enemyRoutes[enemyIndex] = null;
				}
			}
		}
		UsefulFunctions.DecreaseArray<RoutePersistentData>( ref enemyRoutes, enemyIndex );
	}

	public void ClearEnemyData()
	{
		ClearEnemyRoutes();
		enemyOffsets = null;
		enemyCodes = null;
	}

	public void AddWaypointIndexToEnemyRoute( int enemyIndex, int newWaypointIndex )
	{
		RoutePersistentData routeData = null;

		if( enemyRoutes != null )
		{
			if( (enemyIndex > -1) && (enemyIndex < enemyRoutes.Length) )
			{
				routeData = enemyRoutes[enemyIndex];
				if( routeData == null )
				{
					routeData = new RoutePersistentData();
					enemyRoutes[enemyIndex] = routeData;
				}
				routeData.AddWaypointIndex( newWaypointIndex );
			}
		}
	}

	public void ReduceEnemyRoute( int enemyIndex )
	{
		RoutePersistentData routeData = null;

		if( enemyRoutes != null )
		{
			if( (enemyIndex > -1) && (enemyIndex < enemyRoutes.Length) )
			{
				routeData = enemyRoutes[enemyIndex];
				if( routeData != null )
				{
					routeData.RemoveLastWaypointIndex();
					if( routeData.IsEmpty() )
					{
						routeData.Clear();
						enemyRoutes[enemyIndex] = null;
					}
				}
			}
		}
	}

	public void ClearEnemyRoutes()
	{
		RoutePersistentData routeData = null;

		if( enemyRoutes != null )
		{
			for( int i=0; i<enemyRoutes.Length; i++ )
			{
				routeData = enemyRoutes[i];
				if( routeData != null )
				{
					routeData.Clear();
					enemyRoutes[i] = null;
				}
			}
			enemyRoutes = null;
		}
	}

	private void RemoveWaypointFromRoutes( int waypointIndex )
	{
		RoutePersistentData routeData = null;

		if( enemyRoutes != null )
		{
			for( int i=0; i<enemyRoutes.Length; i++ )
			{
				routeData = enemyRoutes[i];
				if( routeData != null )
				{
					routeData.RemoveWaypointIndexAndUpdate( waypointIndex );
					if( routeData.IsEmpty() )
					{
						routeData.Clear();
						enemyRoutes[i] = null;
					}
				}
			}
		}
	}

	public bool SaveToFile( string path, int newStructureID, int newDifficulty, bool createMirror )
	{
		StructurePersistentData data = null;
		StructurePersistentData dataMirror = null;
		string pathWithName = null;
		string pathWithNameMirror = null;
		int newStructureIDMirror = STRUCTURE_ID_INVALID;
		int[] mirrorBoxFirstRows = null;
		int[] mirrorBoxFirstColumns = null;
		int[] mirrorBoxLastRows = null;
		int[] mirrorBoxLastColumns = null;
		int[] mirrorTriFirstRows = null;
		int[] mirrorTriFirstColumns = null;
		int[] mirrorTriLastRows = null;
		int[] mirrorTriLastColumns = null;
		int[] mirrorTriCorners = null;
		RoutePersistentData routeData = null;

		saveSuccess = false;
		if( !String.IsNullOrEmpty(path) && (tileBlueprintFront != null) )
		{
			if( newDifficulty != UNDEFINED_DIFFICULTY )
			{
				if( newDifficulty < DIFFICULTY_MIN )
				{
					newDifficulty = DIFFICULTY_MIN;
				}
				if( newDifficulty > DIFFICULTY_MAX )
				{
					newDifficulty = DIFFICULTY_MAX;
				}
			}
			data = new StructurePersistentData();
			/*halmeida - the challenge ID assignment strategy is ending with 0 for normal challenge,
			ending with 1 for mirrored challenge, ending with 2 for rotated 90 degrees challenge,
			ending with 3 for rotated 180 degrees challenge and ending with 4 for rotated 270 degrees.*/
			newStructureID *= 10;
			pathWithName = path + "/" + newStructureID + ".txt";
			data.structureID = newStructureID;
			data.difficulty = newDifficulty;
			if( data.SetBlueprint( tileRows, tileColumns, tileBlueprintFront, tileToOuterBox, tileToOuterTri, tileBlueprintBack ) )
			{
				data.SetItemData( itemIDs, itemOffsets );
				data.SetItemClusterData( itemClusterRewardCodes, itemToItemClusters );
				data.SetWaypointData( waypointOffsets, waypointIDs );
				data.SetEnemyData( enemyCodes, enemyOffsets );
				if( enemyRoutes != null )
				{
					for( int i=0; i<enemyRoutes.Length; i++ )
					{
						routeData = enemyRoutes[i];
						data.SetEnemyRoute( i, routeData );
					}
				}
				/*halmeida - before saving the outer collider limits, we update them just to correct structures
				that were originally saved with wrong outer collider limits, were loaded and are now being
				overwritten.*/
				ScanOuterColliderLimits();
				data.SetOuterBoxLimits( outerBoxFirstRows, outerBoxFirstColumns, outerBoxLastRows, outerBoxLastColumns );
				data.SetOuterTriLimits( outerTriFirstRows, outerTriFirstColumns, outerTriLastRows, outerTriLastColumns, outerTriCorners );
				gapUpColumns = data.gapUpColumns;
				gapLeftRows = data.gapLeftRows;
				gapDownColumns = data.gapDownColumns;
				gapRightRows = data.gapRightRows;
				if( SavePersistentDataToFile( pathWithName, data ) )
				{
					data.Clear();
					structureID = newStructureID;
					sufixedID = true;
					difficulty = newDifficulty;
					if( createMirror )
					{
						newStructureIDMirror = newStructureID + 1;
						pathWithNameMirror = path + "/" + newStructureIDMirror + ".txt";
						dataMirror = new StructurePersistentData();
						dataMirror.structureID = newStructureIDMirror;
						dataMirror.difficulty = newDifficulty;
						if( dataMirror.SetBlueprint( tileRows, tileColumns, GetBlueprintMirror(true), GetBlueprintToOuterColliderMirror(true),
							GetBlueprintToOuterColliderMirror(false), GetBlueprintMirror(false) ) )
						{
							dataMirror.SetItemData( itemIDs, GetItemOffsetsMirror() );
							dataMirror.SetItemClusterData( itemClusterRewardCodes, itemToItemClusters );
							dataMirror.SetWaypointData( GetWaypointOffsetsMirror(), waypointIDs );
							dataMirror.SetEnemyData( enemyCodes, GetEnemyOffsetsMirror() );
							if( enemyRoutes != null )
							{
								for( int i=0; i<enemyRoutes.Length; i++ )
								{
									routeData = enemyRoutes[i];
									dataMirror.SetEnemyRoute( i, routeData );
								}
							}
							GetOuterBoxLimitsMirror( ref mirrorBoxFirstRows, ref mirrorBoxFirstColumns, ref mirrorBoxLastRows,
								ref mirrorBoxLastColumns );
							dataMirror.SetOuterBoxLimits( mirrorBoxFirstRows, mirrorBoxFirstColumns, mirrorBoxLastRows, mirrorBoxLastColumns );
							GetOuterTriLimitsMirror( ref mirrorTriFirstRows, ref mirrorTriFirstColumns, ref mirrorTriLastRows,
								ref mirrorTriLastColumns, ref mirrorTriCorners );
							dataMirror.SetOuterTriLimits( mirrorTriFirstRows, mirrorTriFirstColumns, mirrorTriLastRows, mirrorTriLastColumns,
								mirrorTriCorners );
							if( SavePersistentDataToFile( pathWithNameMirror, dataMirror ) )
							{
								dataMirror.Clear();
								saveSuccess = true;
								return true;
							}
						}
					}
					else
					{
						saveSuccess = true;
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool SavePersistentDataToFile( string pathWithName, StructurePersistentData persistentData )
	{
		BinaryFormatter bf = null;
		MemoryStream memoryStream = null;
		byte[] bytesFromStream = null;
		string stringFromBytes = null;

		if( !String.IsNullOrEmpty(pathWithName) && (persistentData != null) )
		{
			bf = new BinaryFormatter();
			memoryStream = new MemoryStream();
			try
			{
				bf.Serialize( memoryStream, persistentData );
			}
			catch( Exception e )
			{
				Debug.Log("Debug : ChallengeStructure : save serialization failed. Exception = "+e.Message+".");
				memoryStream = null;
			}
			if( memoryStream != null )
			{
				/*halmeida - read all bytes from the stream.*/
				bytesFromStream = memoryStream.ToArray();
				stringFromBytes = Convert.ToBase64String( bytesFromStream );
				try
				{
					File.WriteAllText( pathWithName, stringFromBytes );
				}
				catch( MethodAccessException e )
				{
					Debug.Log ("Debug : ChallengeStructure : save file writing failed. Exception = "+e.Message+".");
					Debug.Log("Debug : ChallengeStructure : methods from class File cannot be accessed.");
					/*halmeida - means it is web player version. Should save another way.*/
					return false;
				}
				catch( Exception e )
				{
					Debug.Log("Debug : CallengeStructure : save file writing failed. Exception = "+e.Message+".");
					return false;
				}
				Debug.Log ("Debug : ChallengeStructure : persistent data successfully saved to \""+pathWithName+"\".");
				return true;
			}
		}
		Debug.Log("Debug : ChallengeStructure : persistent data or path to save it is invalid.");
		return false;
	}

	public bool LoadFromResourcesFile( string pathWithName = null )
	{
		bool stepFailed = false;
		TextAsset loadedTextAsset = null;
		StructurePersistentData loadedPersistentData = null;

		if( pathWithName != null )
		{
			try
			{
				loadedTextAsset = Resources.Load( pathWithName ) as TextAsset;
			}
			catch( Exception e )
			{
				Debug.Log ("Debug : ChallengeStructure : resource could not be loaded. Load caused exception.");
				Debug.Log ("Debug : ChallengeStructure : exception message = "+e.Message+".");
				stepFailed = true;
			}
			if( loadedTextAsset == null )
			{
				Debug.Log("Debug : ChallengeStructure : resource could not be loaded. Wasn't found.");
				stepFailed = true;
			}
		}
		else
		{
			Debug.Log(" Debug : ChallengeStructure : resource could not be loaded. Path is null.");
			stepFailed = true;
		}
		if( !stepFailed )
		{
			loadedPersistentData = GetPersistentDataFromTextAsset( loadedTextAsset );
			if( loadedTextAsset != null )
			{
				Resources.UnloadAsset( loadedTextAsset );
			}
			return LoadFromPersistentData( loadedPersistentData, true, false ); 
		}
		if( loadedTextAsset != null )
		{
			Resources.UnloadAsset( loadedTextAsset );
		}
		return false;
	}

	public static StructurePersistentData GetPersistentDataFromTextAsset( TextAsset textAsset )
	{
		string recoveredString = null;
		byte[] bytesFromString = null;
		MemoryStream memoryStream = null;
		BinaryFormatter bf = null;
		StructurePersistentData persistentData = null;

		if( textAsset != null )
		{
			recoveredString = textAsset.text;
			if( recoveredString != null )
			{
				bytesFromString = Convert.FromBase64String( recoveredString );
				memoryStream = new MemoryStream( bytesFromString );
				bf = new BinaryFormatter();
				try
				{
					persistentData = (StructurePersistentData)bf.Deserialize( memoryStream );
				}
				catch( Exception e )
				{
					Debug.Log ("Debug : ChallengeStructure : data extraction failed. Exception = "+e.Message+".");
					persistentData = null;
				}
			}
		}
		return persistentData;
	}

	public bool LoadFromPersistentData( StructurePersistentData persistentData, bool reload = false, bool gradualTiles = false )
	{
		if( reload )
		{
			Clear();
		}
		if( !loadSuccess )
		{
			if( !persistentDataExtracted )
			{
				persistentDataExtracted = ExtractFromPersistentData( persistentData );
			}
			if( persistentDataExtracted )
			{
				if( !tilesBuilt )
				{
					if( gradualTiles )
					{
						CreateSomeTilesFromBlueprint( TILE_GRADUAL_BUILD_AMOUNT );
					}
					else
					{
						CreateAllTilesFromBlueprint();
					}
				}
				if( tilesBuilt )
				{
					CreateOuterCollidersFromLimits();
					UpdateColliderMarkers();
					loadSuccess = true;
				}
			}
		}
		return loadSuccess;
	}

	public void Clear()
	{
		ClearEnemyData();
		ClearWaypointData();
		ClearItemData();
		ClearItemClusterData();
		ClearBlueprint( true );
		loadSuccess = false;
		persistentDataExtracted = false;
	}

	private bool ExtractFromPersistentData( StructurePersistentData persistentData )
	{
		int[] linearBlueprintFront = null;
		int[] linearBlueprintBack = null;
		int[] linearBlueprintToOuterBox = null;
		int[] linearBlueprintToOuterTri = null;
		int[] tileToOuterBoxRow = null;
		int[] tileToOuterTriRow = null;
		float[] offsetsX = null;
		float[] offsetsY = null;
		int length = 0;

		if( persistentData != null )
		{
			SetStructureDimensions( persistentData.rows, persistentData.columns );
			linearBlueprintFront = persistentData.blueprintFront;
			linearBlueprintBack = persistentData.blueprintBack;
			linearBlueprintToOuterBox = persistentData.blueprintToOuterBox;
			linearBlueprintToOuterTri = persistentData.blueprintToOuterTri;
			if( linearBlueprintFront != null )
			{
				int linearIndex = 0;
				for( int i=0; i<tileRows; i++ )
				{
					for( int j=0; j<tileColumns; j++ )
					{
						if( linearIndex < linearBlueprintFront.Length )
						{
							SetBlueprintValue( i, j, true, linearBlueprintFront[linearIndex], false, false, false );
						}
						if( linearBlueprintBack != null )
						{
							if( linearIndex < linearBlueprintBack.Length )
							{
								SetBlueprintValue( i, j, false, linearBlueprintBack[linearIndex], false, false, false );
							}
						}
						if( linearBlueprintToOuterBox != null )
						{
							if( linearIndex < linearBlueprintToOuterBox.Length )
							{
								tileToOuterBoxRow = tileToOuterBox[i];
								tileToOuterBoxRow[j] = linearBlueprintToOuterBox[linearIndex];
							}
						}
						if( linearBlueprintToOuterTri != null )
						{
							if( linearIndex < linearBlueprintToOuterTri.Length )
							{
								tileToOuterTriRow = tileToOuterTri[i];
								tileToOuterTriRow[j] = linearBlueprintToOuterTri[linearIndex];
							}
						}
						linearIndex++;
					}
				}
				gapUpColumns = persistentData.gapUpColumns;
				gapLeftRows = persistentData.gapLeftRows;
				gapDownColumns = persistentData.gapDownColumns;
				gapRightRows = persistentData.gapRightRows;
				structureID = persistentData.structureID;
				sufixedID = true;
				difficulty = persistentData.difficulty;
				itemIDs = persistentData.itemCodes;
				itemOffsets = null;
				offsetsX = persistentData.itemOffsetsX;
				offsetsY = persistentData.itemOffsetsY;
				if( (offsetsX != null) && (offsetsY != null) )
				{
					length = offsetsX.Length;
					if( offsetsY.Length == length )
					{
						itemOffsets = new Vector2[length];
						for( int i=0; i<length; i++ )
						{
							itemOffsets[i] = new Vector2( offsetsX[i], offsetsY[i] );
						}
					}
				}
				itemClusterRewardCodes = persistentData.itemClusterRewardCodes;
				itemToItemClusters = persistentData.itemToItemClusters;
				outerBoxFirstRows = persistentData.outerBoxFirstRows;
				outerBoxFirstColumns = persistentData.outerBoxFirstColumns;
				outerBoxLastRows = persistentData.outerBoxLastRows;
				outerBoxLastColumns = persistentData.outerBoxLastColumns;
				outerTriFirstRows = persistentData.outerTriFirstRows;
				outerTriFirstColumns = persistentData.outerTriFirstColumns;
				outerTriLastRows = persistentData.outerTriLastRows;
				outerTriLastColumns = persistentData.outerTriLastColumns;
				outerTriCorners = persistentData.outerTriCorners;
				waypointOffsets = null;
				offsetsX = persistentData.waypointOffsetsX;
				offsetsY = persistentData.waypointOffsetsY;
				if( (offsetsX != null) && (offsetsY != null) )
				{
					length = offsetsX.Length;
					if( offsetsY.Length == length )
					{
						waypointOffsets = new Vector2[length];
						for( int i=0; i<length; i++ )
						{
							waypointOffsets[i] = new Vector2( offsetsX[i], offsetsY[i] );
						}
					}
				}
				waypointIDs = persistentData.waypointIDs;
				enemyCodes = persistentData.enemyCodes;
				enemyOffsets = null;
				offsetsX = persistentData.enemyOffsetsX;
				offsetsY = persistentData.enemyOffsetsY;
				if( (offsetsX != null) && (offsetsY != null) )
				{
					length = offsetsX.Length;
					if( offsetsY.Length == length )
					{
						enemyOffsets = new Vector2[length];
						for( int i=0; i<length; i++ )
						{
							enemyOffsets[i] = new Vector2( offsetsX[i], offsetsY[i] );
						}
					}
				}
				enemyRoutes = persistentData.GetEnemyRoutesCopy();
				return true;
			}
		}
		return false;
	}

	public bool SavedSuccessfully()
	{
		return saveSuccess;
	}

	public bool LoadedSuccessfully()
	{
		return loadSuccess;
	}

	public bool BuildingTiles()
	{
		return (buildingTilesFront || buildingTilesBack);
	}

	public int GetStructureID( bool withIDSufix )
	{
		if( withIDSufix )
		{
			if( sufixedID )
			{
				return structureID;
			}
			else
			{
				return STRUCTURE_ID_INVALID;
			}
		}
		else
		{
			if( sufixedID )
			{
				return structureID / 10;
			}
			else
			{
				return structureID;
			}
		}
	}

	public int GetStructureIDSufix()
	{
		if( sufixedID )
		{
			return (structureID % 10);
		}
		return -1;
	}

	public int[] GetGapUpColumns()
	{
		return gapUpColumns;
	}

	public int[] GetGapLeftRows()
	{
		return gapLeftRows;
	}

	public int[] GetGapDownColumns()
	{
		return gapDownColumns;
	}

	public int[] GetGapRightRows()
	{
		return gapRightRows;
	}

	public Rect[] GetLocalDoorsUp()
	{
		int[] gapIndexes = null;
		int firstIndex = -1;
		int checkIndex = -1;
		int lastIndex = -1;
		float tileDimension = 0f;
		Rect doorRect = default( Rect );
		bool doorReady = false;

		/*halmeida - if the doors haven't yet been calculated, calculate them.*/
		if( (doorsUp == null) && (gapUpColumns != null) && (tileFactoryFront != null) )
		{
			gapIndexes = gapUpColumns;
			tileDimension = tileWidthFront;
			for( int i=0; i<gapIndexes.Length; i++ )
			{
				checkIndex = gapIndexes[i];
				if( firstIndex == -1 )
				{
					firstIndex = checkIndex;
					lastIndex = checkIndex;
				}
				else
				{
					if( checkIndex == (lastIndex + 1) )
					{
						lastIndex = checkIndex;
					}
					else
					{
						doorReady = true;
						i--;
					}
				}
				if( !doorReady && (i == (gapIndexes.Length - 1)) )
				{
					doorReady = true;
				}
				if( doorReady )
				{
					/*halmeida - we got a door from first index to last index.*/
					doorRect.x = firstIndex * tileDimension;
					doorRect.y = 0f;
					doorRect.width = (lastIndex - firstIndex + 1) * tileDimension;
					doorRect.height = 0f;
					UsefulFunctions.IncreaseArray<Rect>( ref doorsUp, doorRect );
					firstIndex = -1;
					doorReady = false;
				}
			}
		}
		return doorsUp;
	}

	public Rect[] GetLocalDoorsLeft()
	{
		int[] gapIndexes = null;
		int firstIndex = -1;
		int checkIndex = -1;
		int lastIndex = -1;
		float tileDimension = 0f;
		Rect doorRect = default( Rect );
		bool doorReady = false;

		/*halmeida - if the doors haven't yet been calculated, calculate them.*/
		if( (doorsLeft == null) && (gapLeftRows != null) && (tileFactoryFront != null) )
		{
			gapIndexes = gapLeftRows;
			tileDimension = tileHeightFront;
			for( int i=0; i<gapIndexes.Length; i++ )
			{
				checkIndex = gapIndexes[i];
				if( firstIndex == -1 )
				{
					firstIndex = checkIndex;
					lastIndex = checkIndex;
				}
				else
				{
					if( checkIndex == (lastIndex + 1) )
					{
						lastIndex = checkIndex;
					}
					else
					{
						doorReady = true;
						i--;
					}
				}
				if( !doorReady && (i == (gapIndexes.Length - 1)) )
				{
					doorReady = true;
				}
				if( doorReady )
				{
					/*halmeida - we got a door from first index to last index.*/
					doorRect.x = 0f;
					doorRect.y = -firstIndex * tileDimension;
					doorRect.width = 0f;
					doorRect.height = (lastIndex - firstIndex + 1) * tileDimension;
					UsefulFunctions.IncreaseArray<Rect>( ref doorsLeft, doorRect );
					firstIndex = -1;
					doorReady = false;
				}
			}
		}
		return doorsLeft;
	}

	public Rect[] GetLocalDoorsDown()
	{
		int[] gapIndexes = null;
		int firstIndex = -1;
		int checkIndex = -1;
		int lastIndex = -1;
		float tileDimension = 0f;
		Rect doorRect = default( Rect );
		bool doorReady = false;

		/*halmeida - if the doors haven't yet been calculated, calculate them.*/
		if( (doorsDown == null) && (gapDownColumns != null) && (tileFactoryFront != null) )
		{
			gapIndexes = gapDownColumns;
			tileDimension = tileWidthFront;
			for( int i=0; i<gapIndexes.Length; i++ )
			{
				checkIndex = gapIndexes[i];
				if( firstIndex == -1 )
				{
					firstIndex = checkIndex;
					lastIndex = checkIndex;
				}
				else
				{
					if( checkIndex == (lastIndex + 1) )
					{
						lastIndex = checkIndex;
					}
					else
					{
						doorReady = true;
						i--;
					}
				}
				if( !doorReady && (i == (gapIndexes.Length - 1)) )
				{
					doorReady = true;
				}
				if( doorReady )
				{
					/*halmeida - we got a door from first index to last index.*/
					doorRect.x = firstIndex * tileDimension;
					doorRect.y = -structureHeight;
					doorRect.width = (lastIndex - firstIndex + 1) * tileDimension;
					doorRect.height = 0f;
					UsefulFunctions.IncreaseArray<Rect>( ref doorsDown, doorRect );
					firstIndex = -1;
					doorReady = false;
				}
			}
		}
		return doorsDown;
	}

	public Rect[] GetLocalDoorsRight()
	{
		int[] gapIndexes = null;
		int firstIndex = -1;
		int checkIndex = -1;
		int lastIndex = -1;
		float tileDimension = 0f;
		Rect doorRect = default( Rect );
		bool doorReady = false;

		/*halmeida - if the doors haven't yet been calculated, calculate them.*/
		if( (doorsRight == null) && (gapRightRows != null) && (tileFactoryFront != null) )
		{
			gapIndexes = gapRightRows;
			tileDimension = tileHeightFront;
			for( int i=0; i<gapIndexes.Length; i++ )
			{
				checkIndex = gapIndexes[i];
				if( firstIndex == -1 )
				{
					firstIndex = checkIndex;
					lastIndex = checkIndex;
				}
				else
				{
					if( checkIndex == (lastIndex + 1) )
					{
						lastIndex = checkIndex;
					}
					else
					{
						doorReady = true;
						i--;
					}
				}
				if( !doorReady && (i == (gapIndexes.Length - 1)) )
				{
					doorReady = true;
				}
				if( doorReady )
				{
					/*halmeida - we got a door from first index to last index.*/
					doorRect.x = structureWidth;
					doorRect.y = -firstIndex * tileDimension;
					doorRect.width = 0f;
					doorRect.height = (lastIndex - firstIndex + 1) * tileDimension;
					UsefulFunctions.IncreaseArray<Rect>( ref doorsRight, doorRect );
					firstIndex = -1;
					doorReady = false;
				}
			}
		}
		return doorsRight;
	}

	public int GetDifficulty()
	{
		return difficulty;
	}

	public int[] GetItemIDs()
	{
		return itemIDs;
	}

	public Vector2[] GetItemOffsets()
	{
		return itemOffsets;
	}

	public int[] GetItemToItemClusters()
	{
		return itemToItemClusters;
	}

	public int[] GetItemClusterRewardCodes()
	{
		return itemClusterRewardCodes;
	}

	public int[] GetWaypointIDs()
	{
		return waypointIDs;
	}

	public Vector2[] GetWaypointOffsets()
	{
		return waypointOffsets;
	}

	public int[] GetEnemyCodes()
	{
		return enemyCodes;
	}

	public Vector2[] GetEnemyOffsets()
	{
		return enemyOffsets;
	}

	public RoutePersistentData[] GetEnemyRoutes()
	{
		return enemyRoutes;
	}

	public void ShowColliderMarkers( bool show )
	{
		GameObject[] tileColliderMarkerRow = null;
		GameObject tileColliderMarker = null;

		if( markColliders )
		{
			showingColliderMarkers = show;
			if( tileColliderMarkers != null )
			{
				for( int i=0; i<tileColliderMarkers.Length; i++ )
				{
					tileColliderMarkerRow = tileColliderMarkers[i];
					if( tileColliderMarkerRow != null )
					{
						for( int j=0; j<tileColliderMarkerRow.Length; j++ )
						{
							tileColliderMarker = tileColliderMarkerRow[j];
							if( tileColliderMarker != null )
							{
								tileColliderMarker.SetActive( showingColliderMarkers );
							}
						}
					}
				}
			}
		}
	}

	private void UpdateColliderMarkers()
	{
		/*halmeida - the objective is to slightly change position and color of collider markers to reflect wether
		they are linked to outer colliders.*/
		GameObject markerObject = null;
		GameObject[] markerRow = null;
		GameObject tileObject = null;
		GameObject[] tileRow = null;
		int[] tileToOuterBoxRow = null;
		int[] tileToOuterTriRow = null;
		bool connectedUp = false;
		bool connectedLeft = false;
		bool connectedDown = false;
		bool connectedRight = false;
		SpriteRenderer markerRenderer = null;
		Color newColor = Color.white;
		Vector3 tilePosition = Vector3.zero;
		Vector3 markerPosition = Vector3.zero;
		int tileToOuterBoxLink = -1;
		int tileToOuterTriLink = -1;
		int[][] linkSource = null;
		int[] tempLinkRow = null;
		int linkIndex = -1;

		if( markColliders )
		{
			if( (tileToOuterBox != null) && (tileToOuterTri != null) && (tileObjectsFront != null) && (tileColliderMarkers != null) )
			{
				for( int i=0; i<tileRows; i++ )
				{
					tileToOuterBoxRow = tileToOuterBox[i];
					tileToOuterTriRow = tileToOuterTri[i];
					tileRow = tileObjectsFront[i];
					markerRow = tileColliderMarkers[i];
					for( int j=0; j<tileColumns; j++ )
					{
						markerObject = markerRow[j];
						if( markerObject != null )
						{
							tileToOuterBoxLink = tileToOuterBoxRow[j];
							tileToOuterTriLink = tileToOuterTriRow[j];
							markerRenderer = markerObject.GetComponent<SpriteRenderer>();
							if( markerRenderer != null )
							{
								newColor = ( (tileToOuterBoxLink > -1) || (tileToOuterTriLink > -1) ) ? Color.blue : Color.white;
								markerRenderer.color = newColor;
							}
							tileObject = tileRow[j];
							if( tileObject != null )
							{
								linkSource = null;
								linkIndex = -1;
								connectedUp = false;
								connectedLeft = false;
								connectedDown = false;
								connectedRight = false;
								/*halmeida - use position of tile as the base of the new position of the marker.*/
								if( tileToOuterBoxLink > -1 )
								{
									linkSource = tileToOuterBox;
									linkIndex = tileToOuterBoxLink;
								}
								else if( tileToOuterTriLink > -1 )
								{
									linkSource = tileToOuterTri;
									linkIndex = tileToOuterTriLink;
								}
								if( linkSource != null )
								{
									if( i > 0 )
									{
										tempLinkRow = linkSource[i-1];
										connectedUp = (tempLinkRow[j] == linkIndex);
									}
									tempLinkRow = linkSource[i];
									if( j > 0 )
									{
										connectedLeft = (tempLinkRow[j-1] == linkIndex);
									}
									if( j < (tileColumns-1) )
									{
										connectedRight = (tempLinkRow[j+1] == linkIndex);
									}
									if( i < (tileRows-1) )
									{
										tempLinkRow = linkSource[i+1];
										connectedDown = (tempLinkRow[j] == linkIndex);
									}
									tilePosition = tileObject.transform.localPosition;
									markerPosition = markerObject.transform.localPosition;
									markerPosition.x = tilePosition.x;
									markerPosition.y = tilePosition.y;
									if( connectedUp && !connectedDown )
									{
										markerPosition.y += tileHeightFront / 6f;
									}
									if( connectedLeft && !connectedRight )
									{
										markerPosition.x -= tileWidthFront / 6f;
									}
									if( connectedDown && !connectedUp )
									{
										markerPosition.y -= tileHeightFront / 6f;
									}
									if( connectedRight && !connectedLeft )
									{
										markerPosition.x += tileWidthFront / 6f;
									}
									markerObject.transform.localPosition = markerPosition;
								}
							}
						}
					}
				}
			}
		}
	}

	public void ClearColliderMerging()
	{
		mergingRows = null;
		mergingColumns = null;
		mergingComponents = null;
		mergingCollidersBox = null;
		mergingCollidersTri = null;
		mergingOuterBoxIndexes = null;
		mergingOuterTriIndexes = null;
		totalMergingBoxes = 0;
		totalMergingTris = 0;
	}

	public void AddToColliderMerging( int row, int column )
	{
		Tile tileComponent = null;
		BoxCollider2D tileColliderBox = null;
		PolygonCollider2D tileColliderTri = null;
		int tileOuterBoxIndex = -1;
		int tileOuterTriIndex = -1;
		Tile previousComponent = null;
		BoxCollider2D previousColliderBox = null;
		PolygonCollider2D previousColliderTri = null;

		GetAssociatedCollider( row, column, ref tileComponent, ref tileColliderBox, ref tileOuterBoxIndex, ref tileColliderTri,
			ref tileOuterTriIndex );
		/*halmeida - only react if a collider was obtained.*/
		if( (tileColliderBox != null) || (tileColliderTri != null) )
		{
			if( mergingRows == null )
			{
				/*halmeida - no complex reaction should be taken, just add collider as first.*/
				UsefulFunctions.IncreaseArray<int>( ref mergingRows, row );
				UsefulFunctions.IncreaseArray<int>( ref mergingColumns, column );
				UsefulFunctions.IncreaseArray<Tile>( ref mergingComponents, tileComponent );
				UsefulFunctions.IncreaseArray<BoxCollider2D>( ref mergingCollidersBox, tileColliderBox );
				UsefulFunctions.IncreaseArray<PolygonCollider2D>( ref mergingCollidersTri, tileColliderTri );
				UsefulFunctions.IncreaseArray<int>( ref mergingOuterBoxIndexes, tileOuterBoxIndex );
				UsefulFunctions.IncreaseArray<int>( ref mergingOuterTriIndexes, tileOuterTriIndex );
				if( tileColliderBox != null )
				{
					totalMergingBoxes++;
				}
				else
				{
					totalMergingTris++;
				}
				Debug.Log("Debug : ChallengeStructure : collider merging started. OBI, OTI = "+tileOuterBoxIndex+", "+tileOuterTriIndex+".");
			}
			else
			{
				/*halmeida - check if the newly selected collider is different from the others.*/
				if( tileComponent != null )
				{
					for( int i=0; i<mergingComponents.Length; i++ )
					{
						previousComponent = mergingComponents[i];
						if( previousComponent != null )
						{
							if( previousComponent.gameObject == tileComponent.gameObject )
							{
								Debug.Log("Debug : ChallengeStructure : last merging selection ignored, equal objects.");
								return;
							}
						}
					}
				}
				if( tileColliderBox != null )
				{
					for( int i=0; i<mergingCollidersBox.Length; i++ )
					{
						previousColliderBox = mergingCollidersBox[i];
						if( previousColliderBox != null )
						{
							if( previousColliderBox == tileColliderBox )
							{
								Debug.Log("Debug : ChallengeStructure : last merging selection ignored, equal colliders.");
								return;
							}
						}
					}
				}
				if( tileColliderTri != null )
				{
					for( int i=0; i<mergingCollidersTri.Length; i++ )
					{
						previousColliderTri = mergingCollidersTri[i];
						if( previousColliderTri != null )
						{
							if( previousColliderTri == tileColliderTri )
							{
								Debug.Log("Debug : ChallengeStructure : last merging selection ignored, equal colliders.");
								return;
							}
						}
					}
				}
				/*halmeida - since it is not repeated, we add it as the second or third element.*/
				UsefulFunctions.IncreaseArray<int>( ref mergingRows, row );
				UsefulFunctions.IncreaseArray<int>( ref mergingColumns, column );
				UsefulFunctions.IncreaseArray<Tile>( ref mergingComponents, tileComponent );
				UsefulFunctions.IncreaseArray<BoxCollider2D>( ref mergingCollidersBox, tileColliderBox );
				UsefulFunctions.IncreaseArray<PolygonCollider2D>( ref mergingCollidersTri, tileColliderTri );
				UsefulFunctions.IncreaseArray<int>( ref mergingOuterBoxIndexes, tileOuterBoxIndex );
				UsefulFunctions.IncreaseArray<int>( ref mergingOuterTriIndexes, tileOuterTriIndex );
				Debug.Log("Debug : ChallengeStructure : collider merging continued. OBI, OTI = "+tileOuterBoxIndex+", "+tileOuterTriIndex+".");
				if( tileColliderBox != null )
				{
					totalMergingBoxes++;
				}
				else
				{
					totalMergingTris++;
				}
				/*halmeida - the only possible merge combinations are (two boxes) or
				(two triangles and one box).*/
				if( (totalMergingBoxes > 2) || (totalMergingTris > 2) )
				{
					ClearColliderMerging();
					Debug.Log("Debug : ChallengeStructure : collider merging cancelled. Too many colliders.");
					return;
				}
				if( totalMergingBoxes == 1 )
				{
					if( totalMergingTris == 2 )
					{
						Debug.Log("Debug : ChallengeStructure : attempting a 3->1 collider merge...");
						MergeColliders();
						ClearColliderMerging();
					}
				}
				if( totalMergingBoxes == 2 )
				{
					if( totalMergingTris > 0 )
					{
						Debug.Log("Debug : ChallengeStructure : collider merging cancelled. Incompatible selection.");
						ClearColliderMerging();
						return;
					}
					Debug.Log("Debug : ChallengeStructure : attempting a 2->1 collider merge...");
					MergeColliders();
					ClearColliderMerging();
				}
			}
		}
		else
		{
			Debug.Log("Debug : ChallengeStructure : clicked position has no associated collider.");
		}
	}

	public void GetAssociatedCollider( int row, int column, ref Tile tileComponent, ref BoxCollider2D box, ref int boxIndex,
		ref PolygonCollider2D tri, ref int triIndex )
	{
		Tile[] tileComponentRow = null;
		int[] tileToOuterBoxRow = null;
		int[] tileToOuterTriRow = null;
		int outerBoxIndex = -1;
		int outerTriIndex = -1;

		if( tileComponentsFront != null )
		{
			if( (row > -1) && (row < tileRows) && (column > -1) && (column < tileColumns) )
			{
				tileComponentRow = tileComponentsFront[row];
				tileComponent = tileComponentRow[column];
				if( tileComponent != null )
				{
					box = tileComponent.GetColliderBox();
					if( box != null )
					{
						boxIndex = -1;
						tri = null;
						triIndex = -1;
						return;
					}
					else
					{
						tri = tileComponent.GetColliderPolygon();
						if( tri != null )
						{
							boxIndex = -1;
							triIndex = -1;
							return;
						}
					}
					/*halmeida - the tileComponent should be used by the caller to get the transform to determine where
					exactly the collider will be positioned. When we provide outer colliders, their offset component
					already states their exact position.*/
					tileComponent = null;
					if( (tileToOuterBox != null) && (tileToOuterTri != null) )
					{
						tileToOuterBoxRow = tileToOuterBox[row];
						tileToOuterTriRow = tileToOuterTri[row];
						outerBoxIndex = tileToOuterBoxRow[column];
						outerTriIndex = tileToOuterTriRow[column];
						if( (outerBoxIndex > -1) && (outerColliderBox != null) )
						{
							if( outerBoxIndex < outerColliderBox.Length )
							{
								box = outerColliderBox[outerBoxIndex];
								if( box != null )
								{
									boxIndex = outerBoxIndex;
									triIndex = -1;
									tri = null;
									return;
								}
							}
						}
						if( (outerTriIndex > -1) && (outerColliderTri != null) )
						{
							if( outerTriIndex < outerColliderTri.Length )
							{
								tri = outerColliderTri[outerTriIndex];
								if( tri != null )
								{
									box = null;
									boxIndex = -1;
									triIndex = outerTriIndex;
									return;
								}
							}
						}
					}
				}
			}
		}
		tileComponent = null;
		box = null;
		boxIndex = -1;
		tri = null;
		triIndex = -1;
		return;
	}

	private bool MergeColliders()
	{
		Rect[] worldRects = null;
		int totalColliders = 0;
		BoxCollider2D colliderBox = null;
		PolygonCollider2D colliderTri = null;
		Tile tileComponent = null;
		Transform tileTransform = null;
		Rect colliderRectOne = default( Rect );
		float lowestX = 0f;
		float highestX = 0f;
		float lowestY = 0f;
		float highestY = 0f;
		Vector2[] colliderPoints = null;
		Vector2 colliderPoint = Vector2.zero;
		bool touchingUp =  false;
		bool touchingDown = false;
		bool touchingLeft = false;
		bool touchingRight = false;
		Rect colliderRectTwo = default( Rect );
		Rect jointColliderRect = default( Rect );
		int outerBoxIndex = -1;
		int smallestOuterBoxIndex = -1;
		int outerTriIndex = -1;
		int smallestOuterTriIndex = -1;
		int[] tileToOuterBoxRow = null;
		int[] tileToOuterTriRow = null;
		int jointOuterBoxIndex = -1;
		int jointOuterTriIndex = -1;
		int lastRemovedBoxIndex = -1;
		int lastRemovedTriIndex = -1;
		int outerTriCorner = TRIANGLE_CORNER_INVALID;
		int[] tempOuterTriCorners = null;
		bool triFacingUp = false;
		bool triFacingLeft = false;
		bool triFacingDown = false;
		bool triFacingRight = false;
		int firstMergingTriIndex = -1;
		int secondMergingTriIndex = -1;
		int mergingBoxIndex = -1;
		Rect colliderRectThree = default( Rect );

		if( (mergingCollidersBox != null) && (mergingOuterBoxIndexes != null) && (mergingCollidersTri != null) && (mergingOuterTriIndexes != null)
			&& (outerColliderLayer != null) )
		{
			totalColliders = mergingCollidersBox.Length;
			if( (totalColliders > 0) && (mergingOuterBoxIndexes.Length == totalColliders) && (mergingCollidersTri.Length == totalColliders) &&
				(mergingOuterTriIndexes.Length == totalColliders) )
			{
				worldRects = new Rect[totalColliders];
				tempOuterTriCorners = new int[totalColliders];
				for( int i=0; i<totalColliders; i++ )
				{
					colliderRectOne.width = 0f;
					outerBoxIndex = -1;
					outerTriIndex = -1;
					colliderBox = mergingCollidersBox[i];
					colliderTri = mergingCollidersTri[i];
					outerTriCorner = TRIANGLE_CORNER_INVALID;
					if( colliderBox != null )
					{
						colliderRectOne.x = colliderBox.offset.x - (colliderBox.size.x / 2f);
						colliderRectOne.y = colliderBox.offset.y - (colliderBox.size.y / 2f);
						colliderRectOne.width = colliderBox.size.x;
						colliderRectOne.height = colliderBox.size.y;
						outerBoxIndex = mergingOuterBoxIndexes[i];
					}
					else if( colliderTri != null )
					{
						lowestX = 0f;
						highestX = 0f;
						lowestY = 0f;
						highestY = 0f;
						triFacingUp = false;
						triFacingLeft = false;
						triFacingDown = false;
						triFacingRight = false;
						colliderPoints = colliderTri.points;
						if( colliderPoints != null )
						{
							for( int j=0; j<colliderPoints.Length; j++ )
							{
								colliderPoint = colliderPoints[j];
								if( lowestX == 0f )
								{
									if( colliderPoint.x < lowestX )
									{
										lowestX = colliderPoint.x;
									}
								}
								else
								{
									if( AreCloseEnoughToMerge( colliderPoint.x, lowestX ) )
									{
										triFacingLeft = true;
									}
								}
								if( highestX == 0f )
								{
									if( colliderPoint.x > highestX )
									{
										highestX = colliderPoint.x;
									}
								}
								else
								{
									if( AreCloseEnoughToMerge( colliderPoint.x, highestX ) )
									{
										triFacingRight = true;
									}
								}
								if( lowestY == 0f )
								{
									if( colliderPoint.y < lowestY )
									{
										lowestY = colliderPoint.y;
									}
								}
								else
								{
									if( AreCloseEnoughToMerge( colliderPoint.y, lowestY ) )
									{
										triFacingDown = true;
									}
								}
								if( highestY == 0f )
								{
									if( colliderPoint.y > highestY )
									{
										highestY = colliderPoint.y;
									}
								}
								else
								{
									if( AreCloseEnoughToMerge( colliderPoint.y, highestY ) )
									{
										triFacingUp = true;
									}
								}
							}
						}
						colliderRectOne.width = highestX - lowestX;
						colliderRectOne.height = highestY - lowestY;
						colliderRectOne.x = colliderTri.offset.x - colliderRectOne.width / 2f;
						colliderRectOne.y = colliderTri.offset.y - colliderRectOne.height / 2f;
						outerTriIndex = mergingOuterTriIndexes[i];
						if( triFacingUp && triFacingLeft )
						{
							outerTriCorner = TRIANGLE_CORNER_UL;
						}
						if( triFacingLeft && triFacingDown )
						{
							outerTriCorner = TRIANGLE_CORNER_LD;
						}
						if( triFacingDown && triFacingRight )
						{
							outerTriCorner = TRIANGLE_CORNER_DR;
						}
						if( triFacingUp && triFacingRight )
						{
							outerTriCorner = TRIANGLE_CORNER_UR;
						}
					}
					if( colliderRectOne.width == 0f )
					{
						/*halmeida - null collider received.*/
						return false; 
					}
					/*halmeida - I save the smallest outer collider index cause I will use that collider
					and that index to assume the merged form.*/
					if( outerBoxIndex > -1 )
					{
						/*halmeida - the collider we just looked at has a valid outer box index.*/
						if( smallestOuterBoxIndex == -1 )
						{
							smallestOuterBoxIndex = outerBoxIndex;
						}
						else if( outerBoxIndex < smallestOuterBoxIndex )
						{
							smallestOuterBoxIndex = outerBoxIndex;
						}
					}
					if( outerTriIndex > -1 )
					{
						/*halmeida - the collider we just looked at has a valid outer tri index.*/
						if( smallestOuterTriIndex == -1 )
						{
							smallestOuterTriIndex = outerTriIndex;
						}
						else if( outerTriIndex < smallestOuterTriIndex )
						{
							smallestOuterTriIndex = outerTriIndex;
						}
					}
					if( mergingComponents != null )
					{
						if( mergingComponents.Length > i )
						{
							tileComponent = mergingComponents[i];
							if( tileComponent != null )
							{
								tileTransform = tileComponent.gameObject.transform;
								if( tileTransform != null )
								{
									colliderRectOne.x += tileTransform.position.x;
									colliderRectOne.y += tileTransform.position.y;
								}
							}
						}
					}
					worldRects[i] = colliderRectOne;
					tempOuterTriCorners[i] = outerTriCorner;
				}
				colliderBox = null;
				colliderTri = null;
				jointColliderRect.width = 0f;
				/*halmeida - to merge two boxes, they must have a touching face. In each collider, the
				touching face must have the same size.*/
				if( totalColliders == 2 )
				{
					/*halmeida - if merge conditions are met, discover joint collider shape.*/
					colliderRectOne = worldRects[0];
					colliderRectTwo = worldRects[1];
					touchingUp = AreCloseEnoughToMerge( colliderRectOne.y + colliderRectOne.height, colliderRectTwo.y );
					touchingDown = AreCloseEnoughToMerge( colliderRectOne.y, colliderRectTwo.y + colliderRectTwo.height );
					if( (touchingUp || touchingDown) && AreCloseEnoughToMerge( colliderRectOne.width, colliderRectTwo.width ) &&
						AreCloseEnoughToMerge( colliderRectOne.x, colliderRectTwo.x ) )
					{
						jointColliderRect.x = colliderRectOne.x;
						jointColliderRect.y = (colliderRectOne.y < colliderRectTwo.y) ? colliderRectOne.y : colliderRectTwo.y;
						jointColliderRect.width = colliderRectOne.width;
						jointColliderRect.height = colliderRectOne.height + colliderRectTwo.height;
					}
					touchingLeft = AreCloseEnoughToMerge( colliderRectOne.x, colliderRectTwo.x + colliderRectTwo.width );
					touchingRight = AreCloseEnoughToMerge( colliderRectOne.x + colliderRectOne.width, colliderRectTwo.x );
					if( (touchingLeft || touchingRight) && AreCloseEnoughToMerge( colliderRectOne.height, colliderRectTwo.height ) &&
						AreCloseEnoughToMerge( colliderRectOne.y, colliderRectTwo.y ) )
					{
						jointColliderRect.x = (colliderRectOne.x < colliderRectTwo.x) ? colliderRectOne.x : colliderRectTwo.x;
						jointColliderRect.y = colliderRectOne.y;
						jointColliderRect.width = colliderRectOne.width + colliderRectTwo.width;
						jointColliderRect.height = colliderRectOne.height;
					}
					if( jointColliderRect.width > 0f )
					{
						/*halmeida - joint shape ready, let's pick the collider that shall take that shape.*/
						if( smallestOuterBoxIndex != -1 )
						{
							colliderBox = outerColliderBox[smallestOuterBoxIndex];
							jointOuterBoxIndex = smallestOuterBoxIndex;
						}
						else
						{
							/*halmeida - no previously existing outer collider was selected for merging.
							We create a new outer collider and add it to the structure.*/
							colliderBox = outerColliderLayer.AddComponent<BoxCollider2D>();
							UsefulFunctions.IncreaseArray<BoxCollider2D>( ref outerColliderBox, colliderBox );
							jointOuterBoxIndex = outerColliderBox.Length - 1;
						}
						/*halmeida - finally, we assign collider shape.*/
						if( colliderBox != null )
						{
							colliderBox.offset = new Vector2( jointColliderRect.x + jointColliderRect.width / 2f,
								jointColliderRect.y + jointColliderRect.height / 2f );
							colliderBox.size = new Vector2( jointColliderRect.width, jointColliderRect.height );
						}
					}
					else
					{
						Debug.Log("Debug : ChallengeStructure : selected colliders do not meet merging requirements.");
					}
				}
				/*halmeida - To merge two triangles and a box, the two triangles must have a touching face
				with the box, these touching faces must be consecutive at the box and the triangles must have
				the same shape, even if they have different sizes.*/
				if( totalColliders == 3 )
				{
					/*halmeida - if merge conditions are met, discover joint collider shape.*/
					for( int i=0; i<totalColliders; i++ )
					{
						if( tempOuterTriCorners[i] == TRIANGLE_CORNER_INVALID )
						{
							if( mergingBoxIndex == -1 )
							{
								mergingBoxIndex = i;
							}
							else
							{
								/*halmeida - besides the box, we found another shape with invalid triangle code.
								The merging cannot continue, for it can only happen between two triangles and
								one box.*/
								break;
							}
						}
						else
						{
							if( firstMergingTriIndex == -1 )
							{
								firstMergingTriIndex = i;
							}
							else
							{
								if( secondMergingTriIndex == -1 )
								{
									secondMergingTriIndex = i;
								}
								else
								{
									/*halmeida - the merging cannot continue because all three colliders were recognized
									as triangles.*/
									break;
								}
							}
						}
					}
					if( (mergingBoxIndex > -1) && (firstMergingTriIndex > -1) && (secondMergingTriIndex > -1) )
					{
						if( tempOuterTriCorners[firstMergingTriIndex] == tempOuterTriCorners[secondMergingTriIndex] )
						{
							/*halmeida - the triangles have the same shape. We can now look at their relations to
							the box to see if the merge is possible.*/
							outerTriCorner = TRIANGLE_CORNER_INVALID;
							colliderRectOne = worldRects[firstMergingTriIndex];
							colliderRectTwo = worldRects[secondMergingTriIndex];
							colliderRectThree = worldRects[mergingBoxIndex];
							switch( tempOuterTriCorners[firstMergingTriIndex] )
							{
								case TRIANGLE_CORNER_UL:
									touchingUp = AreCloseEnoughToMerge( colliderRectOne.y + colliderRectOne.height, colliderRectThree.y );
									if( touchingUp && AreCloseEnoughToMerge( colliderRectOne.width, colliderRectThree.width ) &&
										AreCloseEnoughToMerge( colliderRectOne.x, colliderRectThree.x ) )
									{
										touchingLeft = AreCloseEnoughToMerge( colliderRectTwo.x, colliderRectThree.x + colliderRectThree.width );
										if( touchingLeft && AreCloseEnoughToMerge( colliderRectTwo.height, colliderRectThree.height ) &&
											AreCloseEnoughToMerge( colliderRectTwo.y, colliderRectThree.y ) )
										{
											jointColliderRect.x = colliderRectOne.x;
											jointColliderRect.y = colliderRectOne.y;
											outerTriCorner = TRIANGLE_CORNER_UL;
											break;
										}
									}
									touchingLeft = AreCloseEnoughToMerge( colliderRectOne.x, colliderRectThree.x + colliderRectThree.width );
									if( touchingLeft && AreCloseEnoughToMerge( colliderRectOne.height, colliderRectThree.height ) &&
										AreCloseEnoughToMerge( colliderRectOne.y, colliderRectThree.y ) )
									{
										touchingUp = AreCloseEnoughToMerge( colliderRectTwo.y + colliderRectTwo.height, colliderRectThree.y );
										if( touchingUp && AreCloseEnoughToMerge( colliderRectTwo.width, colliderRectThree.width ) &&
											AreCloseEnoughToMerge( colliderRectTwo.x, colliderRectThree.x ) )
										{
											jointColliderRect.x = colliderRectTwo.x;
											jointColliderRect.y = colliderRectTwo.y;
											outerTriCorner = TRIANGLE_CORNER_UL;
											break;
										}
									}
									break;
								case TRIANGLE_CORNER_LD:
									touchingLeft = AreCloseEnoughToMerge( colliderRectOne.x, colliderRectThree.x + colliderRectThree.width );
									if( touchingLeft && AreCloseEnoughToMerge( colliderRectOne.height, colliderRectThree.height ) &&
										AreCloseEnoughToMerge( colliderRectOne.y, colliderRectThree.y ) )
									{
										touchingDown = AreCloseEnoughToMerge( colliderRectTwo.y, colliderRectThree.y + colliderRectThree.height );
										if( touchingDown && AreCloseEnoughToMerge( colliderRectTwo.width, colliderRectThree.width ) &&
											AreCloseEnoughToMerge( colliderRectTwo.x, colliderRectThree.x ) )
										{
											jointColliderRect.x = colliderRectTwo.x;
											jointColliderRect.y = colliderRectOne.y;
											outerTriCorner = TRIANGLE_CORNER_LD;
											break;
										}
									}
									touchingDown = AreCloseEnoughToMerge( colliderRectOne.y, colliderRectThree.y + colliderRectThree.height );
									if( touchingDown && AreCloseEnoughToMerge( colliderRectOne.width, colliderRectThree.width ) &&
										AreCloseEnoughToMerge( colliderRectOne.x, colliderRectThree.x ) )
									{
										touchingLeft = AreCloseEnoughToMerge( colliderRectTwo.x, colliderRectThree.x + colliderRectThree.width );
										if( touchingLeft && AreCloseEnoughToMerge( colliderRectTwo.height, colliderRectThree.height ) &&
											AreCloseEnoughToMerge( colliderRectTwo.y, colliderRectThree.y ) )
										{
											jointColliderRect.x = colliderRectOne.x;
											jointColliderRect.y = colliderRectTwo.y;
											outerTriCorner = TRIANGLE_CORNER_LD;
											break;
										}
									}
									break;
								case TRIANGLE_CORNER_DR:
									touchingDown = AreCloseEnoughToMerge( colliderRectOne.y, colliderRectThree.y + colliderRectThree.height );
									if( touchingDown && AreCloseEnoughToMerge( colliderRectOne.width, colliderRectThree.width ) &&
										AreCloseEnoughToMerge( colliderRectOne.x, colliderRectThree.x ) )
									{
										touchingRight = AreCloseEnoughToMerge( colliderRectTwo.x + colliderRectTwo.width, colliderRectThree.x );
										if( touchingRight && AreCloseEnoughToMerge( colliderRectTwo.height, colliderRectThree.height ) &&
											AreCloseEnoughToMerge( colliderRectTwo.y, colliderRectThree.y ) )
										{
											jointColliderRect.x = colliderRectTwo.x;
											jointColliderRect.y = colliderRectTwo.y;
											outerTriCorner = TRIANGLE_CORNER_DR;
											break;
										}
									}
									touchingRight = AreCloseEnoughToMerge( colliderRectOne.x + colliderRectOne.width, colliderRectThree.x );
									if( touchingRight && AreCloseEnoughToMerge( colliderRectOne.height, colliderRectThree.height ) &&
										AreCloseEnoughToMerge( colliderRectOne.y, colliderRectThree.y ) )
									{
										touchingDown = AreCloseEnoughToMerge( colliderRectTwo.y, colliderRectThree.y + colliderRectThree.height );
										if( touchingDown && AreCloseEnoughToMerge( colliderRectTwo.width, colliderRectThree.width ) &&
											AreCloseEnoughToMerge( colliderRectTwo.x, colliderRectThree.x ) )
										{
											jointColliderRect.x = colliderRectOne.x;
											jointColliderRect.y = colliderRectOne.y;
											outerTriCorner = TRIANGLE_CORNER_DR;
											break;
										}
									}
									break;
								case TRIANGLE_CORNER_UR:
									touchingUp = AreCloseEnoughToMerge( colliderRectOne.y + colliderRectOne.height, colliderRectThree.y );
									if( touchingUp && AreCloseEnoughToMerge( colliderRectOne.width, colliderRectThree.width ) &&
										AreCloseEnoughToMerge( colliderRectOne.x, colliderRectThree.x ) )
									{
										touchingRight = AreCloseEnoughToMerge( colliderRectTwo.x + colliderRectTwo.width, colliderRectThree.x );
										if( touchingRight && AreCloseEnoughToMerge( colliderRectTwo.height, colliderRectThree.height ) &&
											AreCloseEnoughToMerge( colliderRectTwo.y, colliderRectThree.y ) )
										{
											jointColliderRect.x = colliderRectTwo.x;
											jointColliderRect.y = colliderRectOne.y;
											outerTriCorner = TRIANGLE_CORNER_UR;
											break;
										}
									}
									touchingRight = AreCloseEnoughToMerge( colliderRectOne.x + colliderRectOne.width, colliderRectThree.x );
									if( touchingRight && AreCloseEnoughToMerge( colliderRectOne.height, colliderRectThree.height ) &&
										AreCloseEnoughToMerge( colliderRectOne.y, colliderRectThree.y ) )
									{
										touchingUp = AreCloseEnoughToMerge( colliderRectTwo.y + colliderRectTwo.height, colliderRectThree.y );
										if( touchingUp && AreCloseEnoughToMerge( colliderRectTwo.width, colliderRectThree.width ) &&
											AreCloseEnoughToMerge( colliderRectTwo.x, colliderRectThree.x ) )
										{
											jointColliderRect.x = colliderRectOne.x;
											jointColliderRect.y = colliderRectTwo.y;
											outerTriCorner = TRIANGLE_CORNER_UR;
											break;
										}
									}
									break;
							}
							if( outerTriCorner != TRIANGLE_CORNER_INVALID )
							{
								jointColliderRect.width = colliderRectOne.width + colliderRectTwo.width;
								jointColliderRect.height = colliderRectOne.height + colliderRectTwo.height;
							}
						}
					}
					if( jointColliderRect.width > 0f )
					{
						/*halmeida - joint shape ready, let's pick the collider that shall take that shape.*/
						if( smallestOuterTriIndex != -1 )
						{
							colliderTri = outerColliderTri[smallestOuterTriIndex];
							jointOuterTriIndex = smallestOuterTriIndex;
						}
						else
						{
							colliderTri = outerColliderLayer.AddComponent<PolygonCollider2D>();
							UsefulFunctions.IncreaseArray<PolygonCollider2D>( ref outerColliderTri, colliderTri );
							jointOuterTriIndex = outerColliderTri.Length - 1;
						}
						/*halmeida - finally, we assign collider shape.*/
						if( colliderTri != null )
						{
							colliderPoints = new Vector2[3];
							switch( outerTriCorner )
							{
								case TRIANGLE_CORNER_UL:
									colliderPoints[0] = new Vector2( -jointColliderRect.width / 2f, jointColliderRect.height / 2f );
									colliderPoints[1] = new Vector2( jointColliderRect.width / 2f, jointColliderRect.height / 2f );
									colliderPoints[2] = new Vector2( -jointColliderRect.width / 2f, -jointColliderRect.height / 2f );
									break;
								case TRIANGLE_CORNER_LD:
									colliderPoints[0] = new Vector2( -jointColliderRect.width / 2f, jointColliderRect.height / 2f );
									colliderPoints[1] = new Vector2( -jointColliderRect.width / 2f, -jointColliderRect.height / 2f );
									colliderPoints[2] = new Vector2( jointColliderRect.width / 2f, -jointColliderRect.height / 2f );
									break;
								case TRIANGLE_CORNER_DR:
									colliderPoints[0] = new Vector2( jointColliderRect.width / 2f, jointColliderRect.height / 2f );
									colliderPoints[1] = new Vector2( -jointColliderRect.width / 2f, -jointColliderRect.height / 2f );
									colliderPoints[2] = new Vector2( jointColliderRect.width / 2f, -jointColliderRect.height / 2f );
									break;
								case TRIANGLE_CORNER_UR:
									colliderPoints[0] = new Vector2( -jointColliderRect.width / 2f, jointColliderRect.height / 2f );
									colliderPoints[1] = new Vector2( jointColliderRect.width / 2f, jointColliderRect.height / 2f );
									colliderPoints[2] = new Vector2( jointColliderRect.width / 2f, -jointColliderRect.height / 2f );
									break;
							}
							colliderTri.points = colliderPoints;
							colliderTri.offset = new Vector2( jointColliderRect.x + jointColliderRect.width / 2f, jointColliderRect.y +
								jointColliderRect.height / 2f );
						}
					}
					else
					{
						Debug.Log("Debug : ChallengeStructure : selected colliders do not meet merging requirements.");
					}
				}
				worldRects = null;
				tempOuterTriCorners = null;
				if( (jointOuterBoxIndex > -1) || (jointOuterTriIndex > -1) )
				{
					/*halmeida - the outer collider has been created or reshaped. We must now remove the original colliders
					of each tile and link the tiles to this collider, and remove the unused outer colliders too. When removing
					the outer colliders, the removal of one could make the indexes of the others invalid, since the removal
					decreases the array. The solution would be to reorder all the merging data arrays to bring the highest
					outer collider index to the first position and then remove them normally from first to last position. That
					would be heavy, and not really needed. Since we have at most three elements (colliders) and one of them
					will be kept safe, we only remove at most two. So, we just check if the second one we remove is at an
					index higher than the first one. If it is, we adjust the index to remove from the outer collider array.*/
					for( int i=0; i<mergingComponents.Length; i++ )
					{
						tileComponent = mergingComponents[i];
						if( tileComponent != null )
						{
							TileFactory.RemoveColliders( tileComponent );
							if( jointOuterBoxIndex > -1 )
							{
								tileToOuterBoxRow = tileToOuterBox[mergingRows[i]];
								tileToOuterBoxRow[mergingColumns[i]] = jointOuterBoxIndex;
								tileToOuterTriRow = tileToOuterTri[mergingRows[i]];
								tileToOuterTriRow[mergingColumns[i]] = -1;
							}
							else
							{
								tileToOuterBoxRow = tileToOuterBox[mergingRows[i]];
								tileToOuterBoxRow[mergingColumns[i]] = -1;
								tileToOuterTriRow = tileToOuterTri[mergingRows[i]];
								tileToOuterTriRow[mergingColumns[i]] = jointOuterTriIndex;
							}
						}
						else
						{
							outerBoxIndex = mergingOuterBoxIndexes[i];
							if( outerBoxIndex > -1 )
							{
								if( outerBoxIndex != jointOuterBoxIndex )
								{
									if( lastRemovedBoxIndex <= -1 )
									{
										RemoveOuterCollider( outerBoxIndex, true, false );
										lastRemovedBoxIndex = outerBoxIndex;
									}
									else
									{
										if( outerBoxIndex > lastRemovedBoxIndex )
										{
											outerBoxIndex--;
										}
										RemoveOuterCollider( outerBoxIndex, true, false );
									}
									/*halmeida - at each removed outer collider, I have to look at every tile to see
									if they were linking to that removed collider and update them to link to the newly
									shaped collider. Also, if they were linking to a collider of index higher than
									the removed index, I have to lower it one unit.*/
									for( int j=0; j<tileRows; j++ )
									{
										tileToOuterBoxRow = tileToOuterBox[j];
										tileToOuterTriRow = tileToOuterTri[j];
										for( int k=0; k<tileColumns; k++ )
										{
											if( tileToOuterBoxRow[k] == outerBoxIndex )
											{
												tileToOuterBoxRow[k] = jointOuterBoxIndex;
												tileToOuterTriRow[k] = jointOuterTriIndex;
											}
											else if( tileToOuterBoxRow[k] > outerBoxIndex )
											{
												tileToOuterBoxRow[k]--;
												tileToOuterTriRow[k] = -1;
											}
										}
									}
								}
							}
							else
							{
								outerTriIndex = mergingOuterTriIndexes[i];
								if( (outerTriIndex > -1) && (outerTriIndex != jointOuterTriIndex) )
								{
									if( lastRemovedTriIndex <= -1 )
									{
										RemoveOuterCollider( outerTriIndex, false, false );
										lastRemovedTriIndex = outerTriIndex;
									}
									else
									{
										if( outerTriIndex > lastRemovedTriIndex )
										{
											outerTriIndex--;
										}
										RemoveOuterCollider( outerTriIndex, false, false );
									}
									/*halmeida - at each removed outer collider, I have to look at every tile to see
									if they were linking to that removed collider and update them to link to the newly
									shaped collider. Also, if they were linking to a collider of index higher than
									the removed index, I have to lower it one unit.*/
									for( int j=0; j<tileRows; j++ )
									{
										tileToOuterBoxRow = tileToOuterBox[j];
										tileToOuterTriRow = tileToOuterTri[j];
										for( int k=0; k<tileColumns; k++ )
										{
											if( tileToOuterTriRow[k] == outerTriIndex )
											{
												tileToOuterBoxRow[k] = -1;
												tileToOuterTriRow[k] = jointOuterTriIndex;
											}
											else if( tileToOuterTriRow[k] > outerTriIndex )
											{
												tileToOuterBoxRow[k] = -1;
												tileToOuterTriRow[k]--;
											}
										}
									}
								}
							}
						}
					}
					ScanOuterColliderLimits();
					UpdateColliderMarkers();
					PrintTilesToOuterColliders( true );
					PrintTilesToOuterColliders( false );
					Debug.Log("Debug : ChallengeStructure : collider merge SUCCESSFUL!");
					return true;
				}
			}
		}
		Debug.Log("Debug : ChallengeStructure : collider merge FAILED.");
		return false;
	}

	private bool AreCloseEnoughToMerge( float f1, float f2 )
	{
		float difference = 0f;

		difference = f2 - f1;
		difference *= (difference < 0f) ? -1 : 1;
		return (difference <= mergeTolerance);
	}

	private void ScanOuterColliderLimits()
	{
		int currentOuterIndex = -1;
		int[] outerBoxIndexRow = null;
		int[] outerTriIndexRow = null;
		int boxFirstRow = -1;
		int boxLastRow = -1;
		int boxFirstColumn = -1;
		int boxLastColumn = -1;
		int triFirstRow = -1;
		int triLastRow = -1;
		int triFirstColumn = -1;
		int triLastColumn = -1;
		int triCorner = TRIANGLE_CORNER_INVALID;
		bool currentIndexFoundBox = true;
		bool currentIndexFoundTri = true;

		if( tileBlueprintFront != null )
		{
			/*halmeida - clear previously scanned box limits.*/
			if( outerBoxFirstRows != null )
			{
				outerBoxFirstRows = null;
				outerBoxFirstColumns = null;
				outerBoxLastRows = null;
				outerBoxLastColumns = null;
			}
			/*halmeida - clear previously scanned tri limits.*/
			if( outerTriFirstRows != null )
			{
				outerTriFirstRows = null;
				outerTriFirstColumns = null;
				outerTriLastRows = null;
				outerTriLastColumns = null;
				outerTriCorners = null;
			}
			/*halmeida - scan again.*/
			currentOuterIndex = 0;
			while( currentIndexFoundBox || currentIndexFoundTri )
			{
				currentIndexFoundBox = false;
				currentIndexFoundTri = false;
				boxFirstRow = -1;
				boxLastRow = -1;
				boxFirstColumn = -1;
				boxLastColumn = -1;
				triFirstRow = -1;
				triLastRow = -1;
				triFirstColumn = -1;
				triLastColumn = -1;
				triCorner = TRIANGLE_CORNER_INVALID;
				for( int i=0; i<tileRows; i++ )
				{
					outerBoxIndexRow = tileToOuterBox[i];
					outerTriIndexRow = tileToOuterTri[i];
					for( int j=0; j<tileColumns; j++ )
					{
						if( !currentIndexFoundBox && (outerBoxIndexRow[j] == currentOuterIndex) )
						{
							currentIndexFoundBox = true;
							boxFirstRow = i;
							boxFirstColumn = j;
							boxLastRow = i;
							boxLastColumn = j;
							if( currentIndexFoundTri )
							{
								break;
							}
						}
						if( !currentIndexFoundTri && (outerTriIndexRow[j] == currentOuterIndex) )
						{
							currentIndexFoundTri = true;
							triFirstRow = i;
							triFirstColumn = j;
							triLastRow = i;
							triLastColumn = j;
							if( currentIndexFoundBox )
							{
								break;
							}
						}
					}
					if( currentIndexFoundBox && currentIndexFoundTri )
					{
						break;
					}
				}
				if( currentIndexFoundBox )
				{
					/*halmeida - we mount the box, finding the last row and column. This is done by using
					the first row and column as starting point.*/
					for( int i=(boxFirstRow+1); i<tileRows; i++ )
					{
						outerBoxIndexRow = tileToOuterBox[i];
						if( outerBoxIndexRow[boxFirstColumn] == currentOuterIndex )
						{
							boxLastRow = i;
						}
						else
						{
							break;
						}
					}
					outerBoxIndexRow = tileToOuterBox[boxLastRow];
					for( int j=(boxFirstColumn+1); j<tileColumns; j++ )
					{
						if( outerBoxIndexRow[j] == currentOuterIndex )
						{
							boxLastColumn = j;
						}
						else
						{
							break;
						}
					}
					/*halmeida - at this point we have the full area of the box collider, from its
					first row and column to its last row and column.*/
					UsefulFunctions.IncreaseArray<int>( ref outerBoxFirstRows, boxFirstRow );
					UsefulFunctions.IncreaseArray<int>( ref outerBoxFirstColumns, boxFirstColumn );
					UsefulFunctions.IncreaseArray<int>( ref outerBoxLastRows, boxLastRow );
					UsefulFunctions.IncreaseArray<int>( ref outerBoxLastColumns, boxLastColumn );
				}
				if( currentIndexFoundTri )
				{
					/*halmeida - we mount the triangle, finding the last row and column. This is done by using
					the first row and column as starting point, but it is not as simple as with the box.*/
					/*halmeida - the first thing to do is find the actual first row and first column of the tri.
					It is possible that the actual first row and column don't even belong to the triangle, in
					the case of a triangle of corner DR (Down-Right). That is why the tri is described by the
					box-like limits and by a corner code, to tell where exactly within that box the tri is.*/
					if( triFirstRow < (tileRows-1) )
					{
						outerTriIndexRow = tileToOuterTri[triFirstRow+1];
						if( outerTriIndexRow[triFirstColumn] == currentOuterIndex )
						{
							/*halmeida - the triangle corner may be UL or LD or DR.*/
							if( triFirstColumn > 0 )
							{
								if( outerTriIndexRow[triFirstColumn-1] == currentOuterIndex )
								{
									/*halmeida - the triangle corner can only be DR.*/
									triCorner = TRIANGLE_CORNER_DR;
									for( int i=(triFirstRow+1); i<tileRows; i++ )
									{
										outerTriIndexRow = tileToOuterTri[i];
										if( outerTriIndexRow[triLastColumn] == currentOuterIndex )
										{
											triLastRow = i;
										}
										else
										{
											break;
										}
									}
									outerTriIndexRow = tileToOuterTri[triLastRow];
									for( int i=(triLastColumn-1); i>-1; i-- )
									{
										if( outerTriIndexRow[i] == currentOuterIndex )
										{
											triFirstColumn = i;
										}
										else
										{
											break;
										}
									}
								}
							}
							if( triCorner == TRIANGLE_CORNER_INVALID )
							{
								/*halmeida - the triangle corner may be UL or LD.*/
								if( triFirstColumn < (tileColumns-1) )
								{
									outerTriIndexRow = tileToOuterTri[triFirstRow];
									if( outerTriIndexRow[triFirstColumn+1] == currentOuterIndex )
									{
										/*halmeida - the triangle corner can only be UL.*/
										triCorner = TRIANGLE_CORNER_UL;
										for( int i=(triFirstColumn+1); i<tileColumns; i++ )
										{
											if( outerTriIndexRow[i] == currentOuterIndex )
											{
												triLastColumn = i;
											}
											else
											{
												break;
											}
										}
										for( int i=(triFirstRow+1); i<tileRows; i++ )
										{
											outerTriIndexRow = tileToOuterTri[i];
											if( outerTriIndexRow[triFirstColumn] == currentOuterIndex )
											{
												triLastRow = i;
											}
											else
											{
												break;
											}
										}
									}
									else
									{
										/*halmeida - the triangle corner can only be LD.*/
										triCorner = TRIANGLE_CORNER_LD;
										for( int i=(triFirstRow+1); i<tileRows; i++ )
										{
											outerTriIndexRow = tileToOuterTri[i];
											if( outerTriIndexRow[triFirstColumn] == currentOuterIndex )
											{
												triLastRow = i;
											}
											else
											{
												break;
											}
										}
										outerTriIndexRow = tileToOuterTri[triLastRow];
										for( int i=(triFirstColumn+1); i<tileColumns; i++ )
										{
											if( outerTriIndexRow[i] == currentOuterIndex )
											{
												triLastColumn = i;
											}
											else
											{
												break;
											}
										}
									}
								}
							}
						}
						else
						{
							/*halmeida - the triangle corner can only be UR.*/
							triCorner = TRIANGLE_CORNER_UR;
							for( int i=(triFirstColumn+1); i<tileColumns; i++ )
							{
								if( outerTriIndexRow[i] == currentOuterIndex )
								{
									triLastColumn = i;
								}
								else
								{
									break;
								}
							}
							for( int i=(triFirstRow+1); i<tileRows; i++ )
							{
								outerTriIndexRow = tileToOuterTri[i];
								if( outerTriIndexRow[triLastColumn] == currentOuterIndex )
								{
									triLastRow = i;
								}
								else
								{
									break;
								}
							}
						}
					}
					if( triCorner != TRIANGLE_CORNER_INVALID )
					{
						UsefulFunctions.IncreaseArray<int>( ref outerTriFirstRows, triFirstRow );
						UsefulFunctions.IncreaseArray<int>( ref outerTriFirstColumns, triFirstColumn );
						UsefulFunctions.IncreaseArray<int>( ref outerTriLastRows, triLastRow );
						UsefulFunctions.IncreaseArray<int>( ref outerTriLastColumns, triLastColumn );
						UsefulFunctions.IncreaseArray<int>( ref outerTriCorners, triCorner );
					}
				}
				currentOuterIndex++;
			}
		}
	}

	private void CreateOuterCollidersFromLimits()
	{
		int firstRow = -1;
		int firstColumn = -1;
		int lastRow = -1;
		int lastColumn = -1;
		Rect colliderRect = default( Rect );
		BoxCollider2D newColliderBox = null;
		PolygonCollider2D newColliderTri = null;
		int triangleCorner = TRIANGLE_CORNER_INVALID;
		Vector2[] colliderPoints = null;

		if( (tileFactoryFront != null) && (outerColliderLayer != null) )
		{
			/*halmeida - create the boxes using scanned limits.*/
			if( (outerColliderBox == null) && (outerBoxFirstRows != null) )
			{
				for( int i=0; i<outerBoxFirstRows.Length; i++ )
				{
					firstRow = outerBoxFirstRows[i];
					firstColumn = outerBoxFirstColumns[i];
					lastRow = outerBoxLastRows[i];
					lastColumn = outerBoxLastColumns[i];
					colliderRect.width = (lastColumn - firstColumn + 1) * tileWidthFront;
					colliderRect.height = (lastRow - firstRow + 1) * tileHeightFront;
					colliderRect.x = firstColumn * tileWidthFront;
					colliderRect.y = (-firstRow * tileHeightFront) - colliderRect.height;
					newColliderBox = outerColliderLayer.AddComponent<BoxCollider2D>();
					newColliderBox.offset = new Vector2( colliderRect.x + colliderRect.width / 2f,
						colliderRect.y + colliderRect.height / 2f );
					newColliderBox.size = new Vector2( colliderRect.width, colliderRect.height );
					UsefulFunctions.IncreaseArray<BoxCollider2D>( ref outerColliderBox, newColliderBox );
				}
			}
			/*halmeida - create the triangles using scanned limits.*/
			if( (outerColliderTri == null) && (outerTriFirstRows != null) )
			{
				for( int i=0; i<outerTriFirstRows.Length; i++ )
				{
					firstRow = outerTriFirstRows[i];
					firstColumn = outerTriFirstColumns[i];
					lastRow = outerTriLastRows[i];
					lastColumn = outerTriLastColumns[i];
					triangleCorner = outerTriCorners[i];
					colliderPoints = null;
					colliderRect.width = (lastColumn - firstColumn + 1) * tileWidthFront;
					colliderRect.height = (lastRow - firstRow + 1) * tileHeightFront;
					colliderRect.x = firstColumn * tileWidthFront;
					colliderRect.y = (-firstRow * tileHeightFront) - colliderRect.height;
					switch( triangleCorner )
					{
						case TRIANGLE_CORNER_UL:
							colliderPoints = new Vector2[3];
							colliderPoints[0] = new Vector2( -colliderRect.width / 2f, colliderRect.height / 2f );
							colliderPoints[1] = new Vector2( colliderRect.width / 2f, colliderRect.height / 2f );
							colliderPoints[2] = new Vector2( -colliderRect.width / 2f, -colliderRect.height / 2f );
							break;
						case TRIANGLE_CORNER_LD:
							colliderPoints = new Vector2[3];
							colliderPoints[0] = new Vector2( -colliderRect.width / 2f, colliderRect.height / 2f );
							colliderPoints[1] = new Vector2( -colliderRect.width / 2f, -colliderRect.height / 2f );
							colliderPoints[2] = new Vector2( colliderRect.width / 2f, -colliderRect.height / 2f );
							break;
						case TRIANGLE_CORNER_DR:
							colliderPoints = new Vector2[3];
							colliderPoints[0] = new Vector2( colliderRect.width / 2f, colliderRect.height / 2f );
							colliderPoints[1] = new Vector2( -colliderRect.width / 2f, -colliderRect.height / 2f );
							colliderPoints[2] = new Vector2( colliderRect.width / 2f, -colliderRect.height / 2f );
							break;
						case TRIANGLE_CORNER_UR:
							colliderPoints = new Vector2[3];
							colliderPoints[0] = new Vector2( -colliderRect.width / 2f, colliderRect.height / 2f );
							colliderPoints[1] = new Vector2( colliderRect.width / 2f, colliderRect.height / 2f );
							colliderPoints[2] = new Vector2( colliderRect.width / 2f, -colliderRect.height / 2f );
							break;
					}
					if( colliderPoints != null )
					{
						newColliderTri = outerColliderLayer.AddComponent<PolygonCollider2D>();
						newColliderTri.offset = new Vector2( colliderRect.x + colliderRect.width / 2f,
							colliderRect.y + colliderRect.height / 2f );
						newColliderTri.points = colliderPoints;
						UsefulFunctions.IncreaseArray<PolygonCollider2D>( ref outerColliderTri, newColliderTri );
					}
				}
			}
		}
	}

	private void PrintTilesToOuterColliders( bool boxType )
	{
		int[][] valueMatrix = null;
		int[] valueRow = null;
		string rowString = null;

		if( tileBlueprintFront != null )
		{
			if( boxType )
			{
				Debug.Log("Debug : ChallengeStructure : printing outer box linkage...");
			}
			else
			{
				Debug.Log("Debug : ChallengeStructure : printing outer tri linkage...");
			}
			valueMatrix = (boxType ? tileToOuterBox : tileToOuterTri);
			for( int i=0; i<tileRows; i++ )
			{
				valueRow = valueMatrix[i];
				rowString = UsefulFunctions.StringFromArray<int>( ", ", valueRow );
				Debug.Log("Debug : ChallengeStructure : row "+i+" :        "+rowString+".");
			}
		}
	}

	public GameObject[] GetLinearTileArray( bool front )
	{
		GameObject[] linearTileObjects = null;
		GameObject[] tileObjectRow = null;
		GameObject[][] tileObjects = null;

		tileObjects = (front ? tileObjectsFront : tileObjectsBack);
		if( (tileObjects != null) && (tileRows > 0) && (tileColumns > 0) )
		{
			linearTileObjects = new GameObject[tileRows * tileColumns];
			for( int i=0; i<tileRows; i++ )
			{
				tileObjectRow = tileObjects[i];
				for( int j=0; j<tileColumns; j++ )
				{
					linearTileObjects[i * tileColumns + j] = tileObjectRow[j];
				}
			}
		}
		return linearTileObjects;
	}

	public GameObject GetTileAt( int row, int column, bool front )
	{
		GameObject[] tileObjectRow = null;
		GameObject[][] tileObjects = null;

		tileObjects = (front ? tileObjectsFront : tileObjectsBack);
		if( (tileObjects != null) && (row > -1) && (column > -1) )
		{
			if( tileObjects.Length > row )
			{
				tileObjectRow = tileObjects[row];
				if( tileObjectRow != null )
				{
					if( tileObjectRow.Length > column )
					{
						return tileObjectRow[column];
					}
				}
			}
		}
		return null;
	}

	public void FocusOnTileLayer( bool frontLayer, bool putFocus )
	{
		Tile tileComponent = null;
		Tile[] tileComponentRow = null;
		Tile[][] tileComponents = null;
		float newAlpha = 0f;

		tileComponents = (frontLayer ? tileComponentsBack : tileComponentsFront);
		newAlpha = (putFocus ? 0.3f : 1f);
		if( tileComponents != null )
		{
			for( int i=0; i<tileRows; i++ )
			{
				tileComponentRow = tileComponents[i];
				if( tileComponentRow != null )
				{
					for( int j=0; j<tileColumns; j++ )
					{
						tileComponent = tileComponentRow[j];
						if( tileComponent != null )
						{
							tileComponent.SetRenderingAlpha( newAlpha );
						}
					}
				}
			}
		}
	}

	public static bool IsCodeWithTextureVariation( int blueprintCode )
	{
		return (blueprintCode > BLUEPRINT_CODE_TILE_TRIANGLE);
	}

	public static int GetTileTextureVariation( int blueprintCode )
	{
		int variation = 0;

		/*halmeida - if a variation from the standards is requested...*/
		if( IsCodeWithTextureVariation( blueprintCode ) )
		{
			/*halmeida - it can be either a variation of the normal squared tiles or of the triangle tiles.*/
			if( blueprintCode <= BLUEPRINT_CODE_TILE_N_LAST )
			{
				variation = blueprintCode - BLUEPRINT_CODE_TILE_N_SECOND + 1;
			}
			else if( blueprintCode <= BLUEPRINT_CODE_TILE_T_LAST )
			{
				variation = blueprintCode - BLUEPRINT_CODE_TILE_T_SECOND + 1;
			}
		}
		return variation;
	}

	public static int GetAlternativeBlueprintCode( int blueprintCode, int textureVariation )
	{
		int newBlueprintCode = BLUEPRINT_CODE_INVALID;

		/*halmeida - we reset the blueprint code received to get the lowest value for the
		tile type. Then we add the variation and keep it within valid range.*/
		newBlueprintCode = GetOriginalBlueprintCode( blueprintCode );
		if( newBlueprintCode == BLUEPRINT_CODE_TILE_NORMAL )
		{
			if( textureVariation > 0 )
			{
				textureVariation--;
				newBlueprintCode = BLUEPRINT_CODE_TILE_N_SECOND + textureVariation;
				if( newBlueprintCode > BLUEPRINT_CODE_TILE_N_LAST )
				{
					newBlueprintCode = BLUEPRINT_CODE_TILE_N_LAST;
				}
			}
		}
		else if( newBlueprintCode == BLUEPRINT_CODE_TILE_TRIANGLE )
		{
			if( textureVariation > 0 )
			{
				textureVariation--;
				newBlueprintCode = BLUEPRINT_CODE_TILE_T_SECOND + textureVariation;
				if( newBlueprintCode > BLUEPRINT_CODE_TILE_T_LAST )
				{
					newBlueprintCode = BLUEPRINT_CODE_TILE_T_LAST;
				}
			}
		}
		return newBlueprintCode;
	}

	public static int GetOriginalBlueprintCode( int blueprintCode )
	{
		int originalBlueprintCode = blueprintCode;

		if( (blueprintCode >= BLUEPRINT_CODE_TILE_N_SECOND) && (blueprintCode <= BLUEPRINT_CODE_TILE_N_LAST) )
		{
			originalBlueprintCode = BLUEPRINT_CODE_TILE_NORMAL;
		}
		else if( (blueprintCode >= BLUEPRINT_CODE_TILE_T_SECOND) && (blueprintCode <= BLUEPRINT_CODE_TILE_T_LAST) )
		{
			originalBlueprintCode = BLUEPRINT_CODE_TILE_TRIANGLE;
		}
		return originalBlueprintCode;
	}

	public int GetTileShapeIndexFromBlueprint( int row, int column, bool front )
	{
		bool U = false;
		bool UL = false;
		bool L = false;
		bool LD = false;
		bool D = false;
		bool DR = false;
		bool R = false;
		bool UR = false;
		bool T = false;
		int shapeIndex = -1;

		if( GetOccupiedDirections( row, column, front, ref U, ref UL, ref L, ref LD, ref D, ref DR, ref R, ref UR, ref T ) )
		{
			shapeIndex = Tile.GetShapeIndexByNeighborhood( U, UL, L, LD, D, DR, R, UR, T );
		}
		return shapeIndex;
	}

	/*halmeida - the function below is used for tiles. Outside the limits of the structure, every position is considered
	occupied, to give the illusion of continuity between adjacent structures.*/
	private bool GetOccupiedDirections( int row, int column, bool front, ref bool up, ref bool upLeft, ref bool left, ref bool leftDown, ref bool down,
		ref bool downRight, ref bool right, ref bool upRight, ref bool triangle )
	{
		int[] blueprintRow = null;
		int checkingRow = 0;
		int checkingColumn = 0;
		int[][] tileBlueprint = null;

		if( (tileRows <= 0) || (tileColumns <= 0) )
		{
			return false;
		}
		if( (row < 0) || (row >= tileRows) || (column < 0) || (column >= tileColumns) )
		{
			return false;
		}
		tileBlueprint = (front ? tileBlueprintFront : tileBlueprintBack );
		if( tileBlueprint == null )
		{
			return false;
		}
		if( tileBlueprint.Length != tileRows )
		{
			return false;
		}
		checkingRow = row - 1;
		/*halmeida - outside the limits determined by the tileRows and tileColumns variables,
		every position should be considered occupied, to give the illusion of continuity of
		the stage, allowing different challenge structures to properly connect to each other.*/
		if( checkingRow < 0 )
		{
			upLeft = true;
			up = true;
			upRight = true;
		}
		else
		{
			blueprintRow = tileBlueprint[checkingRow];
			if( blueprintRow == null )
			{
				checkingColumn = column - 1;
				if( checkingColumn < 0 )
				{
					upLeft = true;
				}
				else
				{
					upLeft = false;
				}
				up = false;
				checkingColumn = column + 1;
				if( checkingColumn < tileColumns )
				{
					upRight = false;
				}
				else
				{
					upRight = true;
				}
			}
			else
			{
				/*halmeida - each blueprint row may have a different length. The actual number of
				tile columns has already been recorded in the tileColumns variable, so the length
				of the blueprint rows should not be used with this purpose. We can actually assume
				that if a blueprint row has a shorter length than the tileColumns, the tiles it
				does not describe are just not there.*/
				checkingColumn = column - 1;
				if( checkingColumn < 0 )
				{
					upLeft = true;
				}
				else
				{
					if( checkingColumn < blueprintRow.Length )
					{
						upLeft = IsTilePresenceCode( blueprintRow[checkingColumn] );
					}
					else
					{
						upLeft = false;
					}
				}
				checkingColumn = column;
				if( checkingColumn < blueprintRow.Length )
				{
					up = IsTilePresenceCode( blueprintRow[checkingColumn] );
				}
				else
				{
					up = false;
				}
				checkingColumn = column + 1;
				if( checkingColumn < blueprintRow.Length )
				{
					upRight = IsTilePresenceCode( blueprintRow[checkingColumn] );
				}
				else
				{
					if( checkingColumn < tileColumns )
					{
						upRight = false;
					}
					else
					{
						upRight = true;
					}
				}
			}
		}
		checkingRow = row;
		blueprintRow = tileBlueprint[checkingRow];
		if( blueprintRow == null )
		{
			checkingColumn = column - 1;
			if( checkingColumn < 0 )
			{
				left = true;
			}
			else
			{
				left = false;
			}
			/*halmeida - since the tile at (row, column) doesn't exist in the blueprint, it isn't triangular.*/
			triangle = false;
			checkingColumn = column + 1;
			if( checkingColumn < tileColumns )
			{
				right = false;
			}
			else
			{
				right = true;
			}
		}
		else
		{
			checkingColumn = column - 1;
			if( checkingColumn < 0 )
			{
				left = true;
			}
			else
			{
				if( checkingColumn < blueprintRow.Length )
				{
					left = IsTilePresenceCode( blueprintRow[checkingColumn] );
				}
				else
				{
					left = false;
				}
			}
			checkingColumn = column;
			if( checkingColumn < blueprintRow.Length )
			{
				triangle = IsTriangleTileCode( blueprintRow[checkingColumn] );
			}
			else
			{
				triangle = false;
			}
			checkingColumn = column + 1;
			if( checkingColumn < blueprintRow.Length )
			{
				right = IsTilePresenceCode( blueprintRow[checkingColumn] );
			}
			else
			{
				if( checkingColumn < tileColumns )
				{
					right = false;
				}
				else
				{
					right = true;
				}
			}
		}
		checkingRow = row + 1;
		if( checkingRow >= tileRows )
		{
			leftDown = true;
			down = true;
			downRight = true;
		}
		else
		{
			blueprintRow = tileBlueprint[checkingRow];
			if( blueprintRow == null )
			{
				checkingColumn = column - 1;
				if( checkingColumn < 0 )
				{
					leftDown = true;
				}
				else
				{
					leftDown = false;
				}
				down = false;
				checkingColumn = column + 1;
				if( checkingColumn < tileColumns )
				{
					downRight = false;
				}
				else
				{
					downRight = true;
				}
			}
			else
			{
				checkingColumn = column - 1;
				if( checkingColumn < 0 )
				{
					leftDown = true;
				}
				else
				{
					if( checkingColumn < blueprintRow.Length )
					{
						leftDown = IsTilePresenceCode( blueprintRow[checkingColumn] );
					}
					else
					{
						leftDown = false;
					}
				}
				checkingColumn = column;
				if( checkingColumn < blueprintRow.Length )
				{
					down = IsTilePresenceCode( blueprintRow[checkingColumn] );
				}
				else
				{
					down = false;
				}
				checkingColumn = column + 1;
				if( checkingColumn < blueprintRow.Length )
				{
					downRight = IsTilePresenceCode( blueprintRow[checkingColumn] );
				}
				else
				{
					if( checkingColumn < tileColumns )
					{
						downRight = false;
					}
					else
					{
						downRight = true;
					}
				}
			}
		}
		return true;
	}

	/*halmeida - the function below is used for object placement. Outside the limits of the structure, every position is considered
	free (not occupied), to allow an object to be placed beyond the limits of the structure. This will prevent an object from keeping
	on attempting to find a position and never getting one.*/
	public bool AnyPositionOccupied( bool front, int topRow, int leftColumn, int bottomRow, int rightColumn, ref int occupiedRow,
		ref int occupiedColumn )
	{
		int[] blueprintRow = null;
		int[][] tileBlueprint = null;

		occupiedRow = -1;
		occupiedColumn = -1;
		if( (tileRows <= 0) || (tileColumns <= 0) )
		{
			return false;
		}
		if( (bottomRow < topRow) || (rightColumn < leftColumn) )
		{
			return false;
		}
		tileBlueprint = (front ? tileBlueprintFront : tileBlueprintBack );
		if( tileBlueprint == null )
		{
			return false;
		}
		if( tileBlueprint.Length != tileRows )
		{
			return false;
		}
		for( int i=topRow; i<=bottomRow; i++ )
		{
			if( i > -1 )
			{
				if( i >= tileRows )
				{
					/*halmeida - beyond the limits, nothing is occupied.*/
					return false;
				}
				else
				{
					blueprintRow = tileBlueprint[i];
					if( blueprintRow != null )
					{
						for( int j=leftColumn; j<=rightColumn; j++ )
						{
							if( (j > -1) && (j < tileColumns) )
							{
								if( IsTilePresenceCode( blueprintRow[j] ) )
								{
									occupiedRow = i;
									occupiedColumn = j;
									return true;
								}
							}
						}
					}
				}
			}
		}
		return false;
	}
}
