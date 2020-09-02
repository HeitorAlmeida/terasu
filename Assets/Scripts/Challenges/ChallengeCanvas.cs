using UnityEngine;
using System.Collections;
using System; //to use static String methods

public class ChallengeCanvas : MonoBehaviour
{
	public const int PURPOSE_INVALID = -1;
	public const int SMALLEST_PURPOSE = 0;
	public const int PURPOSE_EDIT_TILES = 0;
	public const int PURPOSE_EDIT_COLLIDERS = 1;
	public const int PURPOSE_EDIT_ITEMS = 2;
	public const int PURPOSE_EDIT_ITEM_CLUSTERS = 3;
	public const int PURPOSE_EDIT_ENEMIES = 4;
	public const int PURPOSE_EDIT_WAYPOINTS = 5;
	public const int PURPOSE_EDIT_BACK_TILES = 6;
	public const int BIGGEST_PURPOSE = 6;

	public const int DIVIDER_SCREENS_VERTICAL = 5;
	public const int DIVIDER_SCREENS_HORIZONTAL = 4;

	public const float WAYPOINT_FADE_ALPHA = 0.3f;

	private GameObject cameraObject;
	private Camera cameraComponent;
	private CameraController cameraController;
	private float cameraWorldWidth;
	private float cameraWorldHeight;
	private TileFactory[] tileFactories;
	private TileFactory factoryFront;
	private TileFactory factoryBack;
	private int factoryIndexFront;
	private int factoryIndexBack;
	private GameObject tileDividerModel;
	private GameObject[] tileDividers;
	private GameObject tileDividersParent;
	private bool showingTileDividers;
	private float tileWorldWidthFront;
	private float tileWorldWidthBack;
	private float tileWorldHeightFront;
	private float tileWorldHeightBack;
	private int tileRowsPerScreen;
	private int tileColumnsPerScreen;
	private GameObject challengeStructureObject;
	private ChallengeStructure challengeStructure;
	private GameObject sizeMarkerModel;
	private GameObject sizeMarker;
	private GameObject sizeMarkerParent;
	private SpriteRenderer sizeMarkerRenderer;
	private GameObject spawnMarkerModel;
	private GameObject spawnMarker;
	private GameObject spawnMarkerParent;
	private SpriteRenderer spawnMarkerRenderer;
	private GameObject playerModel;
	private GameObject playerObject;
	private GameObject playerObjectParent;
	private PlayerAgent playerComponent;
	private bool draggingSizeMarker;
	private bool draggingSpawnMarker;
	private bool draggingCamera;
	private Vector2 draggingCameraStart;
	private Vector2 draggingMouseStart;
	private float draggingUnitsPerPixel;
	private int blueprintInsertionCode;
	private bool brushingTiles;
	private ItemDatabase itemDatabase;
	private int currentPurpose;
	private int currentItemID;
	private ItemData currentItemData;
	private int lastValidItemID;
	private ItemData lastValidItemData;
	private GameObject[] itemObjects;
	private Collider2D[] itemColliders;
	private ItemController[] itemControllers;
	private GameObject colliderMarkerModel;
	private bool inputActive;
	private int constructionUnitSide;
	private int currentItemClusterIndex;
	private GameObject[] itemClusterObjects;
	private ItemCluster[] itemClusters;
	private int[] itemToItemClusters;
	private EnemyDatabase enemyDatabase;
	private int currentEnemyCode;
	private int lastValidEnemyCode;
	private GameObject[] enemyObjects;
	private Collider2D[] enemyColliders;
	private EnemyAgent[] enemyComponents;
	private GameObject waypointModel;
	private GameObject[] waypointObjects;
	private Rect[] waypointAreas;
	private SpriteRenderer[] waypointRenderers;
	private Waypoint[] waypoints;
	private int currentWaypointIndex;
	private ChallengeEditor challengeEditor;
	private int currentEnemyIndex;
	private EnemyAgent currentEnemyComponent;
	private int currentTileVariation;

	void Awake()
	{
		Vector3 correctDepthPosition = Vector3.zero;

		cameraObject = null;
		cameraComponent = null;
		cameraController = null;
		cameraWorldWidth = 0f;
		cameraWorldHeight = 0f;
		tileFactories = null;
		factoryFront = null;
		factoryBack = null;
		factoryIndexFront = -1;
		factoryIndexBack = -1;
		tileDividerModel = null;
		tileDividers = null;
		tileDividersParent = null;
		showingTileDividers = true;
		tileWorldWidthFront = 0f;
		tileWorldWidthBack = 0f;
		tileWorldHeightFront = 0f;
		tileWorldHeightBack = 0f;
		tileRowsPerScreen = 0;
		tileColumnsPerScreen = 0;
		challengeStructureObject = null;
		challengeStructure = null;
		sizeMarkerModel = null;
		sizeMarker = null;
		sizeMarkerParent = null;
		sizeMarkerRenderer = null;
		spawnMarkerModel = null;
		spawnMarker = null;
		spawnMarkerParent = null;
		spawnMarkerRenderer = null;
		playerModel = null;
		playerObject = null;
		playerObjectParent = null;
		playerComponent = null;
		draggingSizeMarker = false;
		draggingSpawnMarker = false;
		draggingCamera = false;
		draggingCameraStart = Vector2.zero;
		draggingMouseStart = Vector2.zero;
		draggingUnitsPerPixel = 0f;
		blueprintInsertionCode = ChallengeStructure.BLUEPRINT_CODE_INVALID;
		brushingTiles = false;
		itemDatabase = null;
		currentPurpose = PURPOSE_INVALID;
		currentItemID = ItemData.INVALID_ITEM_ID;
		currentItemData = null;
		lastValidItemID = ItemData.INVALID_ITEM_ID;
		lastValidItemData = null;
		itemObjects = null;
		itemColliders = null;
		itemControllers = null;
		itemToItemClusters = null;
		colliderMarkerModel = null;
		inputActive = false;
		constructionUnitSide = ChallengeStructure.TILES_TO_MAP_SQUARE_SIDE;
		currentItemClusterIndex = -1;
		itemClusterObjects = null;
		itemClusters = null;
		enemyDatabase = null;
		currentEnemyCode = EnemyDatabase.ENEMY_CODE_INVALID;
		lastValidEnemyCode = EnemyDatabase.ENEMY_CODE_INVALID;
		enemyObjects = null;
		enemyColliders = null;
		enemyComponents = null;
		waypointModel = null;
		waypointObjects = null;
		waypointAreas = null;
		waypointRenderers = null;
		waypoints = null;
		currentWaypointIndex = -1;
		challengeEditor = null;
		currentEnemyIndex = -1;
		currentEnemyComponent = null;
		currentTileVariation = 0;

		challengeStructureObject = new GameObject( "ChallengeStructure" );
		challengeStructure = challengeStructureObject.AddComponent<ChallengeStructure>();
		challengeStructureObject.transform.SetParent( gameObject.transform );
		challengeStructureObject.transform.rotation = Quaternion.identity;
		challengeStructureObject.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_TILE ) );
		challengeStructureObject.transform.position = correctDepthPosition;
		tileDividersParent = new GameObject( "TileDividersParent" );
		tileDividersParent.transform.SetParent( gameObject.transform );
		tileDividersParent.transform.rotation = Quaternion.identity;
		tileDividersParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_TILE_DIVIDER ) );
		tileDividersParent.transform.position = correctDepthPosition;
		sizeMarkerParent = new GameObject( "SizeMarkerParent" );
		sizeMarkerParent.transform.SetParent( gameObject.transform );
		sizeMarkerParent.transform.rotation = Quaternion.identity;
		sizeMarkerParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_SIZE_MARKER ) );
		sizeMarkerParent.transform.position = correctDepthPosition;
		spawnMarkerParent = new GameObject( "SpawnMarkerParent" );
		spawnMarkerParent.transform.SetParent( gameObject.transform );
		spawnMarkerParent.transform.rotation = Quaternion.identity;
		spawnMarkerParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_SPAWN_MARKER ) );
		spawnMarkerParent.transform.position = correctDepthPosition;
		playerObjectParent = new GameObject( "PlayerObjectParent" );
		playerObjectParent.transform.SetParent( gameObject.transform );
		playerObjectParent.transform.rotation = Quaternion.identity;
		playerObjectParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_PLAYER ) );
		playerObjectParent.transform.position = correctDepthPosition;
	}

	public void SetCameraObject( GameObject newCameraObject, GameObject newInterfaceCanvasObject )
	{
		Vector3 cameraPosition = Vector3.zero;

		cameraObject = newCameraObject;
		if( cameraObject != null )
		{
			cameraComponent = cameraObject.GetComponent<Camera>();
			cameraController = cameraObject.GetComponent<CameraController>();
			if( cameraController != null )
			{
				if( cameraController.GetCameraOriginalWorldDimensions( ref cameraWorldWidth, ref cameraWorldHeight ) )
				{
					Debug.Log("Debug : ChallengeCanvas : screen in units ("+cameraWorldWidth+","+cameraWorldHeight+").");
					cameraPosition = cameraObject.transform.position;
					cameraPosition.x = cameraWorldWidth / 2.0f;
					cameraPosition.y = -cameraWorldHeight / 2.0f;
					cameraObject.transform.position = cameraPosition;
					cameraController.SetTargetEditor();
					if( cameraController.GetCameraOriginalUnitsPerPixel( ref draggingUnitsPerPixel ) )
					{
						return;
					}
				}
			}
		}
		Debug.Log("Debug : ChallengeCanvas : dimensions not ready for copy.");
		draggingUnitsPerPixel = 0f;
		cameraWorldWidth = 0f;
		cameraWorldHeight = 0f;
		cameraController = null;
		cameraComponent = null;
		cameraObject = null;
	}

	public void SetTileFactories( TileFactory[] newTileFactories, int newFactoryIndexFront, int newFactoryIndexBack )
	{
		if( newTileFactories != null )
		{
			tileFactories = newTileFactories;
			SetCurrentTileFactoryIndex( true, newFactoryIndexFront, false );
			SetCurrentTileFactoryIndex( false, newFactoryIndexBack, true );
			if( factoryFront != null )
			{
				SetPurpose( PURPOSE_EDIT_TILES );
				blueprintInsertionCode = ChallengeStructure.BLUEPRINT_CODE_TILE_NORMAL;
				inputActive = true;
			}
		}
	}

	private void SetCurrentTileFactoryIndex( bool front, int newIndex, bool updateTiles )
	{
		Vector2 tileWorldDimensions = Vector2.one;

		if( tileFactories == null )
		{
			return;
		}
		if( (newIndex < 0) || (newIndex >= tileFactories.Length) )
		{
			return;
		}
		if( tileFactories[newIndex] == null )
		{
			return;
		}
		if( front )
		{
			factoryIndexFront = newIndex;
			factoryFront = tileFactories[factoryIndexFront];
			tileWorldDimensions = factoryFront.GetTileWorldDimensions();
			tileWorldWidthFront = tileWorldDimensions.x;
			tileWorldHeightFront = tileWorldDimensions.y;
			tileRowsPerScreen = (int)(cameraWorldHeight / tileWorldHeightFront);
			tileColumnsPerScreen = (int)(cameraWorldWidth / tileWorldWidthFront);
			/*halmeida - we add an extra column because the integer number of columns
			might not cover the entire screen. There may be a little space left that is
			not enough to fit a tile. To cover that space, we add this column.*/
			if( ((cameraWorldHeight / tileWorldHeightFront) - tileRowsPerScreen) > 0f )
			{
				tileRowsPerScreen++;
			}
			if( ((cameraWorldWidth / tileWorldWidthFront) - tileColumnsPerScreen) > 0f )
			{
				tileColumnsPerScreen++;
			}
			CreateTileDividers();
			if( !showingTileDividers )
			{
				ShowTileDividers( false );
			}
			CreateSizeMarker();
		}
		else
		{
			factoryIndexBack = newIndex;
			factoryBack = tileFactories[factoryIndexBack];
			tileWorldDimensions = factoryBack.GetTileWorldDimensions();
			tileWorldWidthBack = tileWorldDimensions.x;
			tileWorldHeightBack = tileWorldDimensions.y;
		}
		if( (challengeStructure != null) && updateTiles )
		{
			challengeStructure.SetTileFactories( factoryFront, factoryBack );
		}
	}

	private void ClearTileDividers()
	{
		if( tileDividers != null )
		{
			for( int i=0; i<tileDividers.Length; i++ )
			{
				if( tileDividers[i] != null )
				{
					Destroy( tileDividers[i] );
					tileDividers[i] = null;
				}
			}
			tileDividers = null;
		}
	}

	private void ShowTileDividers( bool show )
	{
		GameObject tileDivider = null;

		if( tileDividers != null )
		{
			for( int i=0; i<tileDividers.Length; i++ )
			{
				tileDivider = tileDividers[i];
				if( tileDivider != null )
				{
					tileDivider.SetActive( show );
				}
			}
		}
		showingTileDividers = show;
	}

	public void SetTileDividerModel( GameObject newTileDividerModel )
	{
		if( newTileDividerModel != null )
		{
			tileDividerModel = newTileDividerModel;
		}
	}

	public void SetSizeMarkerModel( GameObject newSizeMarkerModel )
	{
		if( newSizeMarkerModel != null )
		{
			sizeMarkerModel = newSizeMarkerModel;
		}
	}

	public void SetSpawnMarkerModel( GameObject newSpawnMarkerModel )
	{
		if( newSpawnMarkerModel != null )
		{
			spawnMarkerModel = newSpawnMarkerModel;
			CreateSpawnMarker();
		}
	}

	public void SetPlayerModel( GameObject newPlayerModel )
	{
		if( newPlayerModel != null )
		{
			playerModel = newPlayerModel;
		}
	}

	public void SetColliderMarkerModel( GameObject newColliderMarkerModel )
	{
		if( newColliderMarkerModel != null )
		{
			colliderMarkerModel =  newColliderMarkerModel;
			if( challengeStructure != null )
			{
				challengeStructure.SetColliderMarkerModel( colliderMarkerModel );
			}
		}
	}

	public void SetWaypointMarkerModel( GameObject newWaypointMarkerModel )
	{
		if( newWaypointMarkerModel != null )
		{
			waypointModel = newWaypointMarkerModel;
		}
	}

	public void SetModels( GameObject newTileDividerModel, GameObject newSizeMarkerModel, GameObject newSpawnMarkerModel,
		GameObject newPlayerModel, GameObject newColliderMarkerModel, GameObject newWaypointMarkerModel )
	{
		SetTileDividerModel( newTileDividerModel );
		SetSizeMarkerModel( newSizeMarkerModel );
		SetSpawnMarkerModel( newSpawnMarkerModel );
		SetPlayerModel( newPlayerModel );
		SetColliderMarkerModel( newColliderMarkerModel );
		SetWaypointMarkerModel( newWaypointMarkerModel );
	}

	public void SetItemDatabase( ItemDatabase newItemDatabase )
	{
		itemDatabase = newItemDatabase;
	}

	public void SetEnemyDatabase( EnemyDatabase newEnemyDatabase )
	{
		enemyDatabase = newEnemyDatabase;
	}

	public void SetChallengeEditor( ChallengeEditor newChallengeEditor )
	{
		challengeEditor = newChallengeEditor;
	}

	private void CreateTileDividers()
	{
		float currentWorldX = 0f;
		float currentWorldY = 0f;
		GameObject newDivider = null;
		int dividerRows = 0;
		int dividerColumns = 0;
		int totalTileDividers = 0;
		Vector3 dividerPosition = Vector3.zero;
		bool markRow = false;

		ClearTileDividers();
		if( (tileDividersParent != null) && (tileDividerModel != null) && (factoryFront != null) )
		{
			/*halmeida - Initially, I will create room for some times the height and width of
			the screen, in tile dividers. Note that, since there are dividers at the boundaries
			of the space, we need one more divider at every row and one more row of dividers.*/
			dividerRows = tileRowsPerScreen * DIVIDER_SCREENS_VERTICAL + 1;
			dividerColumns = tileColumnsPerScreen * DIVIDER_SCREENS_HORIZONTAL + 1;
			totalTileDividers = dividerRows * dividerColumns;
			tileDividers = new GameObject[totalTileDividers];
			for( int i=0; i<dividerRows; i++ )
			{
				markRow = false;
				if( constructionUnitSide > 0 )
				{
					if( (i % constructionUnitSide) == 0 )
					{
						markRow = true;
					}
				}
				for( int j=0; j<dividerColumns; j++ )
				{
					newDivider = Instantiate( tileDividerModel ) as GameObject;
					newDivider.transform.SetParent( tileDividersParent.transform, false );
					dividerPosition.x = currentWorldX;
					dividerPosition.y = currentWorldY;
					dividerPosition.z = 0f;
					newDivider.transform.localPosition = dividerPosition;
					newDivider.transform.localRotation = Quaternion.identity;
					if( markRow && ((j % constructionUnitSide) == 0) )
					{
						newDivider.transform.localScale = Vector3.one;
					}
					else
					{
						newDivider.transform.localScale = new Vector3( 0.5f, 0.5f, 1f );
					}
					tileDividers[ (i * dividerColumns) + j ] = newDivider;
					currentWorldX += tileWorldWidthFront;
				}
				currentWorldX = 0f;
				currentWorldY -= tileWorldHeightFront;
			}
		}
	}

	private void CreateSizeMarker()
	{
		int structureRows = -1;
		int structureColumns = -1;
		float worldX = 0f;
		float worldY = 0f;
		Vector2 worldPosition = Vector2.zero;

		if( sizeMarker != null )
		{
			sizeMarkerRenderer = null;
			Destroy( sizeMarker );
			sizeMarker = null;
		}
		if( (sizeMarkerParent != null) && (sizeMarkerModel != null) && (factoryFront != null) )
		{
			sizeMarker = Instantiate( sizeMarkerModel ) as GameObject;
			sizeMarker.transform.SetParent( sizeMarkerParent.transform );
			sizeMarker.transform.localRotation = Quaternion.identity;
			sizeMarker.transform.localScale = Vector3.one;
			if( challengeStructure != null )
			{
				challengeStructure.GetStructureDimensions( ref structureRows, ref structureColumns );
				worldX = structureColumns * tileWorldWidthFront;
				worldY = structureRows * tileWorldHeightFront * -1;
			}
			worldPosition = sizeMarker.transform.position;
			worldPosition.x = worldX;
			worldPosition.y = worldY;
			sizeMarker.transform.localPosition = worldPosition;
			sizeMarkerRenderer = sizeMarker.GetComponent<SpriteRenderer>();
		}
	}

	public void CreateSpawnMarker()
	{
		if( spawnMarker != null )
		{
			spawnMarkerRenderer = null;
			Destroy( spawnMarker );
			spawnMarker = null;
		}
		if( (spawnMarkerParent != null) && (spawnMarkerModel != null) )
		{
			spawnMarker = Instantiate( spawnMarkerModel ) as GameObject;
			spawnMarker.transform.SetParent( spawnMarkerParent.transform );
			spawnMarker.transform.localRotation = Quaternion.identity;
			spawnMarker.transform.localScale = Vector2.one;
			spawnMarker.transform.localPosition = new Vector2( 0.6f, 0f );
			spawnMarkerRenderer = spawnMarker.GetComponent<SpriteRenderer>();
		}
	}

	private void TogglePlayer()
	{
		Vector2 spawnPosition = Vector2.zero;
		EnemyAgent enemyComponent = null;
		PlayerAgent[] currentPlayerComponents = null;

		if( playerObject != null )
		{
			if( playerComponent != null )
			{
				playerComponent.Clear();
				playerComponent = null;
			}
			Destroy( playerObject );
			playerObject = null;
			if( cameraController != null )
			{
				cameraController.SetTargetPlayer( null );
				cameraController.SetTargetEditor();
			}
		}
		else
		{
			if( (playerObjectParent != null) && (spawnMarker != null) && (playerModel != null) )
			{
				playerObject = Instantiate( playerModel ) as GameObject;
				if( playerObject != null )
				{
					playerComponent = playerObject.GetComponent<PlayerAgent>();
					if( playerComponent != null )
					{
						playerComponent.SetStageAndCamera( null, cameraController );
						playerComponent.CreateGauges();
						if( cameraController != null )
						{
							cameraController.SetTargetPlayer( playerComponent, CameraController.SMOOTH_LERP_FACTOR );
						}
					}
				}
				spawnPosition = spawnMarker.transform.position;
				playerObject.transform.position = spawnPosition;
				playerObject.transform.SetParent( playerObjectParent.transform );
				playerObject.transform.localRotation = Quaternion.identity;
				playerObject.transform.localScale = Vector2.one;
			}
		}
		if( enemyComponents != null )
		{
			currentPlayerComponents = GetAllPlayerComponents();
			for( int i=0; i<enemyComponents.Length; i++ )
			{
				enemyComponent = enemyComponents[i];
				if( enemyComponent != null )
				{
					enemyComponent.SetAdversaries( currentPlayerComponents );
				}
			}
		}
	}

	public PlayerAgent[] GetAllPlayerComponents()
	{
		PlayerAgent[] players = null;

		if( playerComponent != null )
		{
			UsefulFunctions.IncreaseArray<PlayerAgent>( ref players, playerComponent );
		}
		return players;
	}

	public void ToggleTileDividers()
	{
		ShowTileDividers( !showingTileDividers );
	}

	public void AdvancePurpose()
	{
		SetPurpose( currentPurpose+1 );
	}

	private void SetPurpose( int newPurpose )
	{
		int lastPurpose = PURPOSE_INVALID;

		if( (newPurpose != currentPurpose) && (newPurpose >= SMALLEST_PURPOSE) )
		{
			lastPurpose = currentPurpose;
			currentPurpose = newPurpose;
			/*halmeida - first we make sure the purpose value is valid.*/
			if( (currentPurpose == PURPOSE_EDIT_ITEMS) && (itemDatabase == null) )
			{
				currentPurpose++;
			}
			if( currentPurpose > BIGGEST_PURPOSE )
			{
				currentPurpose = SMALLEST_PURPOSE;
			}
			/*halmeida - then we make propper preparations for and from each purpose.*/
			if( currentPurpose != PURPOSE_EDIT_ITEMS )
			{
				if( lastPurpose == PURPOSE_EDIT_ITEMS )
				{
					lastValidItemID = currentItemID;
					lastValidItemData = currentItemData;
					currentItemID = ItemData.INVALID_ITEM_ID;
					currentItemData = null;
				}
			}
			else
			{
				if( itemDatabase != null )
				{
					if( lastValidItemID == ItemData.INVALID_ITEM_ID )
					{
						lastValidItemID = itemDatabase.GetNextItemID( ItemData.INVALID_ITEM_ID, ref lastValidItemData );
					}
					currentItemID = lastValidItemID;
					currentItemData = lastValidItemData;
				}
			}
			if( currentPurpose != PURPOSE_EDIT_ITEM_CLUSTERS )
			{
				if( lastPurpose == PURPOSE_EDIT_ITEM_CLUSTERS )
				{
					currentItemClusterIndex = -1;
					UpdateItemClusterDisplay();
				}
			}
			else
			{
				currentItemClusterIndex = 0;
				UpdateItemClusterDisplay();
			}
			if( currentPurpose != PURPOSE_EDIT_COLLIDERS )
			{
				if( lastPurpose == PURPOSE_EDIT_COLLIDERS )
				{
					if( challengeStructure != null )
					{
						challengeStructure.ShowColliderMarkers( false );
						challengeStructure.ClearColliderMerging();
					}
				}
			}
			else
			{
				if( challengeStructure != null )
				{
					challengeStructure.ShowColliderMarkers( true );
				}
			}
			if( currentPurpose != PURPOSE_EDIT_BACK_TILES )
			{
				if( lastPurpose == PURPOSE_EDIT_BACK_TILES )
				{
					if( challengeStructure != null )
					{
						challengeStructure.FocusOnTileLayer( false, false );
					}
				}
			}
			else
			{
				if( challengeStructure != null )
				{
					challengeStructure.FocusOnTileLayer( false, true );
				}
				currentTileVariation = 0;
			}
			if( currentPurpose != PURPOSE_EDIT_TILES )
			{
				if( lastPurpose == PURPOSE_EDIT_TILES )
				{
					if( challengeStructure != null )
					{
						challengeStructure.FocusOnTileLayer( true, false );
					}
				}
			}
			else
			{
				if( challengeStructure != null )
				{
					challengeStructure.FocusOnTileLayer( true, true );
				}
				currentTileVariation = 0;
			}
			if( currentPurpose != PURPOSE_EDIT_ENEMIES )
			{
				if( lastPurpose == PURPOSE_EDIT_ENEMIES )
				{
					lastValidEnemyCode = currentEnemyCode;
					currentEnemyCode = EnemyDatabase.ENEMY_CODE_INVALID;
					currentEnemyIndex = -1;
				}
			}
			else
			{
				currentEnemyIndex = -1;
				if( enemyDatabase != null )
				{
					if( lastValidEnemyCode == EnemyDatabase.ENEMY_CODE_INVALID )
					{
						lastValidEnemyCode = EnemyDatabase.SMALLEST_ENEMY_CODE;
					}
					currentEnemyCode = lastValidEnemyCode;
				}
			}
			if( currentPurpose != PURPOSE_EDIT_WAYPOINTS )
			{
				if( lastPurpose == PURPOSE_EDIT_WAYPOINTS )
				{
					currentWaypointIndex = -1;
					FocusOnWaypoints( false );
				}
			}
			else
			{
				currentWaypointIndex = -1;
				FocusOnWaypoints( true );
			}
		}
	}

	public int GetCurrentPurpose()
	{
		return currentPurpose;
	}

	public static string GetPurposeInitial( int purposeCode )
	{
		switch( purposeCode )
		{
			case PURPOSE_EDIT_TILES:
				return "T";
			case PURPOSE_EDIT_ITEMS:
				return "I";
			case PURPOSE_EDIT_ITEM_CLUSTERS:
				return "IC";
			case PURPOSE_EDIT_COLLIDERS:
				return "C";
			case PURPOSE_EDIT_ENEMIES:
				return "E";
			case PURPOSE_EDIT_WAYPOINTS:
				return "W";
			case PURPOSE_EDIT_BACK_TILES:
				return "BT";
		}
		return "-";
	}

	public void ChangeCurrentCode( bool increase )
	{
		switch( currentPurpose )
		{
			case PURPOSE_EDIT_TILES:
				currentTileVariation += (increase ? 1 : -1);
				if( currentTileVariation < 0 )
				{
					currentTileVariation = 0;
				}
				break;
			case PURPOSE_EDIT_BACK_TILES:
				currentTileVariation += (increase ? 1 : -1);
				if( currentTileVariation < 0 )
				{
					currentTileVariation = 0;
				}
				break;
			case PURPOSE_EDIT_ITEMS:
				if( itemDatabase != null )
				{
					if( increase )
					{
						currentItemID = itemDatabase.GetNextItemID( currentItemID, ref currentItemData );
					}
					else
					{
						currentItemID = itemDatabase.GetPreviousItemID( currentItemID, ref currentItemData );
					}
				}
				break;
			case PURPOSE_EDIT_ITEM_CLUSTERS:
				currentItemClusterIndex += (increase ? 1 : -1);
				if( itemClusters == null )
				{
					currentItemClusterIndex = 0;
				}
				else
				{
					if( currentItemClusterIndex < 0 )
					{
						currentItemClusterIndex = itemClusters.Length;
					}
					else if( currentItemClusterIndex > itemClusters.Length )
					{
						currentItemClusterIndex = 0;
					}
				}
				UpdateItemClusterDisplay();
				break;
			case PURPOSE_EDIT_ENEMIES:
				currentEnemyCode += (increase ? 1 : -1);
				if( currentEnemyCode > EnemyDatabase.BIGGEST_ENEMY_CODE )
				{
					currentEnemyCode = EnemyDatabase.SMALLEST_ENEMY_CODE;
				}
				if( currentEnemyCode < EnemyDatabase.SMALLEST_ENEMY_CODE )
				{
					currentEnemyCode = EnemyDatabase.BIGGEST_ENEMY_CODE;
				}
				break;
		}
	}

	private void CreateItem( int itemID, Vector2 worldPosition, bool addToStructure )
	{
		GameObject itemModel = null;
		GameObject itemObject = null;
		Collider2D itemCollider = null;
		ItemController itemController = null;
		Vector3 itemPosition = Vector3.zero;

		if( itemDatabase != null )
		{
			itemModel = itemDatabase.GetItemModel( itemID );
			if( itemModel != null )
			{
				itemPosition.x = worldPosition.x;
				itemPosition.y = worldPosition.y;
				itemPosition.z = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_ITEM );
				itemObject = Instantiate( itemModel, itemPosition, Quaternion.identity ) as GameObject;
				if( itemObject != null )
				{
					itemCollider = itemObject.GetComponent<Collider2D>();
					itemController = itemObject.GetComponent<ItemController>();
					UsefulFunctions.IncreaseArray<GameObject>( ref itemObjects, itemObject );
					UsefulFunctions.IncreaseArray<Collider2D>( ref itemColliders, itemCollider );
					UsefulFunctions.IncreaseArray<ItemController>( ref itemControllers, itemController );
					UsefulFunctions.IncreaseArray<int>( ref itemToItemClusters, -1 );
					if( (challengeStructure != null) && addToStructure )
					{
						challengeStructure.AddItemData( itemID, worldPosition );
					}
				}
			}
		}
	}

	private void CreateItemsFromStructure()
	{
		int[] itemIDsRead = null;
		Vector2[] itemPositionsRead = null;

		ClearItems( false );
		if( challengeStructure != null )
		{
			itemIDsRead = challengeStructure.GetItemIDs();
			itemPositionsRead = challengeStructure.GetItemOffsets();
			if( (itemIDsRead != null) && (itemPositionsRead != null) )
			{
				if( itemIDsRead.Length == itemPositionsRead.Length )
				{
					for( int i=0; i<itemIDsRead.Length; i++ )
					{
						CreateItem( itemIDsRead[i], itemPositionsRead[i], false );
					}
				}
			}
		}
	}

	private bool CreateItemCluster( ref GameObject itemClusterObject, ref ItemCluster itemCluster, int itemClusterRewardCode,
		int itemClusterIndex, bool addToStructure )
	{
		if( (itemClusterObject == null) && (itemCluster == null) )
		{
			itemClusterObject = new GameObject("ItemCluster"+itemClusterIndex);
			itemClusterObject.transform.position = Vector3.zero;
			itemClusterObject.transform.rotation = Quaternion.identity;
			itemClusterObject.transform.localScale = Vector3.one;
			itemCluster = itemClusterObject.AddComponent<ItemCluster>();
			itemCluster.rewardCode = itemClusterRewardCode;
			itemCluster.scaleChangeSecondItem = new Vector3( 1.5f, 1.5f, 1f );
			itemCluster.scaleChangeLastItem = new Vector3( 2f, 2f, 1f );
			if( (challengeStructure != null) && addToStructure )
			{
				challengeStructure.AddItemClusterData( itemCluster.rewardCode );
			}
			return true;
		}
		return false;
	}

	private void CreateItemClustersFromStructure()
	{
		int[] itemToItemClustersRead = null;
		int[] itemClusterRewardCodesRead = null;
		GameObject itemClusterObject = null;
		ItemCluster itemCluster = null;
		int length = 0;
		int rewardCode = ItemCluster.REWARD_CODE_INVALID;
		int clusterIndex = -1;
		ItemController itemController = null;

		ClearItemClusters( false );
		if( challengeStructure != null )
		{
			itemClusterRewardCodesRead = challengeStructure.GetItemClusterRewardCodes();
			if( itemClusterRewardCodesRead != null )
			{
				length = itemClusterRewardCodesRead.Length;
				if( length > 0 )
				{
					itemClusterObjects = new GameObject[length];
					itemClusters = new ItemCluster[length];
					for( int i=0; i<length; i++ )
					{
						rewardCode = itemClusterRewardCodesRead[i];
						if( CreateItemCluster( ref itemClusterObject, ref itemCluster, rewardCode, i, false ) )
						{
							itemClusterObjects[i] = itemClusterObject;
							itemClusters[i] = itemCluster;
						}
						else
						{
							itemClusterObjects[i] = null;
							itemClusters[i] = null;
						}
						itemClusterObject = null;
						itemCluster = null;
					}
				}
			}
			itemToItemClusters = null;
			if( itemControllers != null )
			{
				itemToItemClusters = new int[itemControllers.Length];
				for( int i=0; i<itemToItemClusters.Length; i++ )
				{
					itemToItemClusters[i] = -1;
				}
				itemToItemClustersRead = challengeStructure.GetItemToItemClusters();
				if( itemToItemClustersRead != null )
				{
					for( int i=0; i<itemToItemClustersRead.Length; i++ )
					{
						if( i < itemToItemClusters.Length )
						{
							/*halmeida - the array that we read from the structure has a position that the
							array we just created for cluster indexes also has. We could just copy the value into
							the new array, but we will only do it if the value is valid.*/
							clusterIndex = itemToItemClustersRead[i];
							if( (clusterIndex > -1) && (itemClusters != null) )
							{
								if( clusterIndex < itemClusters.Length )
								{
									itemCluster = itemClusters[clusterIndex];
									if( itemCluster != null )
									{
										itemController = itemControllers[i];
										if( itemController != null )
										{
											itemCluster.IncludeItem( itemController );
											itemToItemClusters[i] = clusterIndex;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private void AddItemToCluster( int itemIndex, int clusterIndex, bool alterAtStructure )
	{
		ItemController itemController = null;
		ItemCluster itemCluster = null;
		GameObject itemClusterObject = null;

		if( (itemIndex > -1) && (itemControllers != null) )
		{
			if( itemControllers.Length > itemIndex )
			{
				itemController = itemControllers[itemIndex];
			}
		}
		if( (itemController != null) && (clusterIndex > -1) )
		{
			if( itemClusters == null )
			{
				if( clusterIndex == 0 )
				{
					if( CreateItemCluster( ref itemClusterObject, ref itemCluster, ItemCluster.REWARD_CODE_HP_RECOVERY,
						0, alterAtStructure ) )
					{
						itemClusterObjects = new GameObject[1];
						itemClusters = new ItemCluster[1];
						itemClusterObjects[0] = itemClusterObject;
						itemClusters[0] = itemCluster;
					}
				}
			}
			else if( clusterIndex < itemClusters.Length )
			{
				itemCluster = itemClusters[clusterIndex];
			}
			else if( clusterIndex == itemClusters.Length )
			{
				if( CreateItemCluster( ref itemClusterObject, ref itemCluster, ItemCluster.REWARD_CODE_HP_RECOVERY,
					clusterIndex, alterAtStructure ) )
				{
					UsefulFunctions.IncreaseArray<GameObject>( ref itemClusterObjects, itemClusterObject );
					UsefulFunctions.IncreaseArray<ItemCluster>( ref itemClusters, itemCluster );
				}
			}
		}
		if( itemCluster != null )
		{
			itemCluster.IncludeItem( itemController );
			if( itemToItemClusters != null )
			{
				if( itemToItemClusters.Length > itemIndex )
				{
					itemToItemClusters[itemIndex] = clusterIndex;
					if( (challengeStructure != null) && alterAtStructure )
					{
						challengeStructure.AlterItemDataClusterPertinence( itemIndex, clusterIndex );
					}
				}
			}
		}
	}

	private void RemoveItem( int itemIndex, bool removeFromStructure )
	{
		ItemController itemController = null;
		GameObject itemObject = null;
		int itemClusterIndex = -1;

		if( itemToItemClusters != null )
		{
			if( itemToItemClusters.Length > itemIndex )
			{
				itemClusterIndex = itemToItemClusters[itemIndex];
				RemoveItemFromCluster( itemIndex, itemClusterIndex, removeFromStructure );
				UsefulFunctions.DecreaseArray<int>( ref itemToItemClusters, itemIndex );
			}
		}
		if( itemControllers != null )
		{
			if( itemControllers.Length > itemIndex )
			{
				itemController = itemControllers[itemIndex];
				if( itemController != null )
				{
					itemController.Clear();
					itemControllers[itemIndex] = null;
				}
				UsefulFunctions.DecreaseArray<ItemController>( ref itemControllers, itemIndex );
			}
		}
		UsefulFunctions.DecreaseArray<Collider2D>( ref itemColliders, itemIndex );
		if( itemObjects != null )
		{
			if( itemObjects.Length > itemIndex )
			{
				itemObject = itemObjects[itemIndex];
				if( itemObject != null )
				{
					Destroy( itemObject );
					itemObjects[itemIndex] = null;
				}
				UsefulFunctions.DecreaseArray<GameObject>( ref itemObjects, itemIndex );
			}
		}
		if( removeFromStructure && (challengeStructure != null) )
		{
			challengeStructure.RemoveItemData( itemIndex );
		}
	}

	private void RemoveItemFromCluster( int itemIndex, int itemClusterIndex, bool removeFromStructure )
	{
		ItemCluster itemCluster = null;
		ItemController itemController = null;

		if( (itemIndex > -1) && (itemControllers != null) )
		{
			if( itemIndex < itemControllers.Length )
			{
				itemController = itemControllers[itemIndex];
			}
		}
		if( (itemController != null) && (itemClusters != null) )
		{
			if( (itemClusterIndex > -1) && (itemClusterIndex < itemClusters.Length) )
			{
				itemCluster = itemClusters[itemClusterIndex];
				if( itemCluster != null )
				{
					itemCluster.ExcludeItem( itemController );
					if( itemToItemClusters != null )
					{
						if( itemIndex < itemToItemClusters.Length )
						{
							itemToItemClusters[itemIndex] = -1;
							if( (challengeStructure != null) && removeFromStructure )
							{
								challengeStructure.AlterItemDataClusterPertinence( itemIndex, -1 );
							}
						}
					}
					if( itemCluster.GetTotalItems() == 0 )
					{
						RemoveItemCluster( itemClusterIndex, removeFromStructure );
					}
				}
			}
		}
	}

	private void RemoveItemCluster( int itemClusterIndex, bool removeFromStructure )
	{
		ItemCluster itemCluster = null;
		GameObject itemClusterObject = null;
		int tempClusterIndex = -1;

		if( (itemClusterIndex > -1) && (itemClusters != null) && (itemClusterObjects != null) )
		{
			if( itemClusterIndex < itemClusters.Length )
			{
				itemCluster = itemClusters[itemClusterIndex];
				if( itemCluster != null )
				{
					itemCluster.Clear();
					itemClusters[itemClusterIndex] = null;
				}
				UsefulFunctions.DecreaseArray<ItemCluster>( ref itemClusters, itemClusterIndex );
			}
			if( itemClusterIndex < itemClusterObjects.Length )
			{
				itemClusterObject = itemClusterObjects[itemClusterIndex];
				if( itemClusterObject != null )
				{
					Destroy( itemClusterObject );
					itemClusterObjects[itemClusterIndex] = null;
				}
				UsefulFunctions.DecreaseArray<GameObject>( ref itemClusterObjects, itemClusterIndex );
			}
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
			/*halmeida - reset the item cluster index to make sure we are going to a valid state.*/
			currentItemClusterIndex = 0;
			if( (challengeStructure != null) && removeFromStructure )
			{
				challengeStructure.RemoveItemClusterData( itemClusterIndex );
			}
		}
	}

	private void ClearItems( bool clearFromStructure )
	{
		ItemController itemController = null;
		GameObject itemObject = null;

		itemToItemClusters = null;
		if( itemControllers != null )
		{
			for( int i=0; i<itemControllers.Length; i++ )
			{
				itemController = itemControllers[i];
				if( itemController != null )
				{
					itemController.Clear();
					itemControllers[i] = null;
				}
			}
			itemControllers = null;
		}
		itemColliders = null;
		if( itemObjects != null )
		{
			for( int i=0; i<itemObjects.Length; i++ )
			{
				itemObject = itemObjects[i];
				if( itemObject != null )
				{
					Destroy( itemObject );
					itemObjects[i] = null;
				}
			}
			itemObjects = null;
		}
		if( clearFromStructure && (challengeStructure != null) )
		{
			challengeStructure.ClearItemData();
		}
	}

	private void ClearItemClusters( bool clearFromStructure )
	{
		ItemCluster itemCluster = null;
		GameObject itemClusterObject = null;

		if( itemClusters != null )
		{
			for( int i=0; i<itemClusters.Length; i++ )
			{
				itemCluster = itemClusters[i];
				if( itemCluster != null )
				{
					itemCluster.Clear();
					itemClusters[i] = null;
				}
			}
			itemClusters = null;
		}
		if( itemClusterObjects != null )
		{
			for( int i=0; i<itemClusterObjects.Length; i++ )
			{
				itemClusterObject = itemClusterObjects[i];
				if( itemClusterObject != null )
				{
					Destroy( itemClusterObject );
					itemClusterObjects[i] = null;
				}
			}
			itemClusterObjects = null;
		}
		currentItemClusterIndex = (currentPurpose == PURPOSE_EDIT_ITEM_CLUSTERS) ? 0 : -1;
		/*halmeida - if the items were still referencing item clusters through indexes, they must all
		be invalidated.*/
		if( itemToItemClusters != null )
		{
			for( int i=0; i<itemToItemClusters.Length; i++ )
			{
				itemToItemClusters[i] = -1;
			}
		}
		if( (challengeStructure != null) && clearFromStructure )
		{
			challengeStructure.ClearItemClusterData();
		}
	}

	private void UpdateItemClusterDisplay()
	{
		ItemController itemController = null;

		/*halmeida - go through all items setting their appearances appropriately.
		If an item is owned by the current item cluster, it should look different.*/
		if( (itemControllers != null) && (itemToItemClusters != null) )
		{
			for( int i=0; i<itemControllers.Length; i++ )
			{
				itemController = itemControllers[i];
				if( itemController != null )
				{
					if( i < itemToItemClusters.Length )
					{
						if( (itemToItemClusters[i] == currentItemClusterIndex) && (currentItemClusterIndex > -1) )
						{
							itemController.RequestScaleChange( new Vector3( 1.5f, 1.5f, 1f ), true );
						}
						else
						{
							itemController.RequestScaleChange( Vector3.one, true );
						}
					}
				}
			}
		}
	}

	private EnemyAgent CreateEnemy( int enemyCode, Vector2 worldPosition, bool addToStructure )
	{
		GameObject enemyModel = null;
		GameObject enemyObject = null;
		Collider2D enemyCollider = null;
		EnemyAgent enemyComponent = null;
		Vector3 enemyPosition = Vector3.zero;

		if( enemyDatabase != null )
		{
			enemyModel = enemyDatabase.GetEnemyModel( enemyCode );
			if( enemyModel != null )
			{
				enemyPosition.x = worldPosition.x;
				enemyPosition.y = worldPosition.y;
				enemyPosition.z = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_ENEMY );
				enemyObject = Instantiate( enemyModel, enemyPosition, Quaternion.identity ) as GameObject;
				if( enemyObject != null )
				{
					enemyCollider = enemyObject.GetComponent<Collider2D>();
					enemyComponent = enemyObject.GetComponent<EnemyAgent>();
					UsefulFunctions.IncreaseArray<GameObject>( ref enemyObjects, enemyObject );
					UsefulFunctions.IncreaseArray<Collider2D>( ref enemyColliders, enemyCollider );
					UsefulFunctions.IncreaseArray<EnemyAgent>( ref enemyComponents, enemyComponent );
					if( (challengeStructure != null) && addToStructure )
					{
						challengeStructure.AddEnemyData( enemyCode, worldPosition );
					}
				}
			}
		}
		return enemyComponent;
	}

	private void CreateEnemiesFromStructure()
	{
		int[] enemyCodesRead = null;
		Vector2[] enemyPositionsRead = null;
		RoutePersistentData[] enemyRoutesRead = null;
		RoutePersistentData enemyRoute = null;
		EnemyAgent enemyComponent = null;

		ClearEnemies( false );
		if( challengeStructure != null )
		{
			enemyCodesRead = challengeStructure.GetEnemyCodes();
			enemyPositionsRead = challengeStructure.GetEnemyOffsets();
			enemyRoutesRead = challengeStructure.GetEnemyRoutes();
			if( (enemyCodesRead != null) && (enemyPositionsRead != null) )
			{
				if( enemyCodesRead.Length == enemyPositionsRead.Length )
				{
					if( enemyRoutesRead != null )
					{
						if( enemyRoutesRead.Length == enemyCodesRead.Length )
						{
							Debug.Log("Debug : ChallengeCanvas : enemyRoutesRead is adequate.");
						}
						else
						{
							Debug.Log("Debug : ChallengeCanvas : enemyRoutesRead is inadequate.");
						}
					}
					else
					{
						Debug.Log("Debug : ChallengeCanvas : enemyRoutesRead is null.");
					}
					for( int i=0; i<enemyCodesRead.Length; i++ )
					{
						enemyComponent = CreateEnemy( enemyCodesRead[i], enemyPositionsRead[i], false );
						if( enemyComponent != null )
						{
							if( enemyRoutesRead != null )
							{
								if( enemyRoutesRead.Length > i )
								{
									enemyRoute = enemyRoutesRead[i];
									if( enemyRoute != null )
									{
										enemyComponent.SetAllRouteWaypointIndexes( enemyRoute.GetWaypointIndexesCopy() );
									}
								}
								else
								{
									Debug.Log("Debug : ChallengeCanvas : Error, enemyRoutesRead is not null but is too short.");
								}
							}
							else
							{
								challengeStructure.AlterEnemyDataRoute( i, null );
							}
						}
					}
				}
			}
		}
	}

	private void RemoveEnemy( int enemyIndex, bool removeFromStructure )
	{
		EnemyAgent enemyComponent = null;
		GameObject enemyObject = null;

		if( enemyComponents != null )
		{
			if( enemyComponents.Length > enemyIndex )
			{
				enemyComponent = enemyComponents[enemyIndex];
				if( enemyComponent != null )
				{
					enemyComponent.Clear();
					enemyComponents[enemyIndex] = null;
				}
				UsefulFunctions.DecreaseArray<EnemyAgent>( ref enemyComponents, enemyIndex );
			}
		}
		UsefulFunctions.DecreaseArray<Collider2D>( ref enemyColliders, enemyIndex );
		if( enemyObjects != null )
		{
			if( enemyObjects.Length > enemyIndex )
			{
				enemyObject = enemyObjects[enemyIndex];
				if( enemyObject != null )
				{
					Destroy( enemyObject );
					enemyObjects[enemyIndex] = null;
				}
				UsefulFunctions.DecreaseArray<GameObject>( ref enemyObjects, enemyIndex );
			}
		}
		if( removeFromStructure && (challengeStructure != null) )
		{
			challengeStructure.RemoveEnemyData( enemyIndex );
		}
	}

	private void ClearEnemies( bool clearFromStructure )
	{
		EnemyAgent enemyComponent = null;
		GameObject enemyObject = null;

		if( enemyComponents != null )
		{
			for( int i=0; i<enemyComponents.Length; i++ )
			{
				enemyComponent = enemyComponents[i];
				if( enemyComponent != null )
				{
					enemyComponent.Clear();
					enemyComponents[i] = null;
				}
			}
			enemyComponents = null;
		}
		enemyColliders = null;
		if( enemyObjects != null )
		{
			for( int i=0; i<enemyObjects.Length; i++ )
			{
				enemyObject = enemyObjects[i];
				if( enemyObject != null )
				{
					Destroy( enemyObject );
					enemyObjects[i] = null;
				}
			}
			enemyObjects = null;
		}
		if( clearFromStructure && (challengeStructure != null) )
		{
			challengeStructure.ClearEnemyData();
		}
	}

	private void CreateWaypoint( Vector2 worldPosition, int waypointID, bool addToStructure )
	{
		GameObject waypointObject = null;
		Rect waypointArea = default( Rect );
		SpriteRenderer waypointRenderer = null;
		Sprite waypointSprite = null;
		Vector3 waypointExtents = Vector3.zero;
		Vector3 waypointSize = Vector3.zero;
		Vector3 waypointPosition = Vector3.zero;
		Waypoint waypoint = null;

		waypoint = new Waypoint( worldPosition, waypointID );
		if( waypointModel != null )
		{
			waypointPosition.x = worldPosition.x;
			waypointPosition.y = worldPosition.y;
			waypointPosition.z = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_WAYPOINT );
			waypointObject = Instantiate( waypointModel, waypointPosition, Quaternion.identity ) as GameObject;
			waypointRenderer = waypointObject.GetComponent<SpriteRenderer>();
			if( waypointRenderer != null )
			{
				waypointSprite = waypointRenderer.sprite;
				if( waypointSprite != null )
				{
					waypointExtents = waypointSprite.bounds.extents;
					waypointSize = waypointSprite.bounds.size;
					/*halmeida - top-left anchored area.*/
					waypointArea = new Rect( worldPosition.x - waypointExtents.x, worldPosition.y + waypointExtents.y,
						waypointSize.x, waypointSize.y );
				}
			}
		}
		UsefulFunctions.IncreaseArray<GameObject>( ref waypointObjects, waypointObject );
		UsefulFunctions.IncreaseArray<Rect>( ref waypointAreas, waypointArea );
		UsefulFunctions.IncreaseArray<SpriteRenderer>( ref waypointRenderers, waypointRenderer );
		UsefulFunctions.IncreaseArray<Waypoint>( ref waypoints, waypoint );
		if( (challengeStructure != null) && addToStructure )
		{
			challengeStructure.AddWaypointData( worldPosition, waypointID );
		}
	}

	private void CreateWaypointsFromStructure()
	{
		Vector2[] waypointPositionsRead = null;
		int[] waypointIDsRead = null;

		ClearWaypoints( false );
		if( challengeStructure != null )
		{
			waypointPositionsRead = challengeStructure.GetWaypointOffsets();
			waypointIDsRead = challengeStructure.GetWaypointIDs();
			if( (waypointPositionsRead != null) && (waypointIDsRead != null) )
			{
				if( waypointPositionsRead.Length == waypointIDsRead.Length )
				{
					for( int i=0; i<waypointPositionsRead.Length; i++ )
					{
						CreateWaypoint( waypointPositionsRead[i], waypointIDsRead[i], false );
					}
				}
			}
		}
	}

	private void StartWaypointAlteration( int waypointIndex )
	{
		Waypoint waypoint = null;
		int waypointID = Waypoint.INVALID_WAYPOINT_ID;

		if( (challengeEditor != null) && (waypoints != null) && (waypointIndex > -1) )
		{
			if( waypointIndex < waypoints.Length )
			{
				waypoint = waypoints[waypointIndex];
				if( waypoint != null )
				{
					currentWaypointIndex = waypointIndex;
					waypointID = waypoint.ID;
					challengeEditor.OnClickWaypoint( waypointID );
				}
			}
		}
	}

	public void EndWaypointAlteration( bool applyChange, bool delete, int newWaypointID )
	{
		Waypoint waypoint = null;

		if( applyChange )
		{
			if( delete )
			{
				RemoveWaypoint( currentWaypointIndex, true );
			}
			else
			{
				if( (waypoints != null) && (currentWaypointIndex > -1) )
				{
					if( currentWaypointIndex < waypoints.Length )
					{
						waypoint = waypoints[currentWaypointIndex];
						if( waypoint != null )
						{
							waypoint.ID = newWaypointID;
						}
						if( challengeStructure != null )
						{
							challengeStructure.AlterWaypointDataID( currentWaypointIndex, newWaypointID );
						}
					}
				}
			}
		}
		currentWaypointIndex = -1;
	}

	private void FocusOnWaypoints( bool putFocus )
	{
		Color color = Color.black;
		float alpha = 0f;
		SpriteRenderer waypointRenderer = null;

		if( waypointRenderers != null )
		{
			alpha = putFocus ? 1f : WAYPOINT_FADE_ALPHA;
			for( int i=0; i<waypointRenderers.Length; i++ )
			{
				waypointRenderer = waypointRenderers[i];
				if( waypointRenderer != null )
				{
					color = waypointRenderer.color;
					color.a = alpha;
					waypointRenderer.color = color;
				}
			}
		}
	}

	private void RemoveWaypoint( int waypointIndex, bool removeFromStructure )
	{
		Waypoint waypoint = null;
		GameObject waypointObject = null;
		EnemyAgent enemyComponent = null;

		if( waypointIndex > -1 )
		{
			if( waypoints != null )
			{
				if( waypointIndex < waypoints.Length )
				{
					waypoint = waypoints[waypointIndex];
					if( waypoint != null )
					{
						waypoint.Clear();
						waypoints[waypointIndex] = null;
					}
					UsefulFunctions.DecreaseArray<Waypoint>( ref waypoints, waypointIndex );
					if( enemyComponents != null )
					{
						for( int i=0; i<enemyComponents.Length; i++ )
						{
							enemyComponent = enemyComponents[i];
							if( enemyComponent != null )
							{
								enemyComponent.RemoveWaypointIndexFromRoute( waypointIndex );
							}
						}
					}
				}
			}
			UsefulFunctions.DecreaseArray<SpriteRenderer>( ref waypointRenderers, waypointIndex );
			UsefulFunctions.DecreaseArray<Rect>( ref waypointAreas, waypointIndex );
			if( waypointObjects != null )
			{
				if( waypointIndex < waypointObjects.Length )
				{
					waypointObject = waypointObjects[waypointIndex];
					if( waypointObject != null )
					{
						Destroy( waypointObject );
						waypointObjects[waypointIndex] = null;
					}
					UsefulFunctions.DecreaseArray<GameObject>( ref waypointObjects, waypointIndex );
				}
			}
			if( removeFromStructure && (challengeStructure != null) )
			{
				challengeStructure.RemoveWaypointData( waypointIndex );
			}
		}
	}

	private void ClearWaypoints( bool clearFromStructure )
	{
		Waypoint waypoint = null;
		GameObject waypointObject = null;
		EnemyAgent enemyComponent = null;

		if( waypoints != null )
		{
			for( int i=0; i<waypoints.Length; i++ )
			{
				waypoint = waypoints[i];
				if( waypoint != null )
				{
					waypoint.Clear();
					waypoints[i] = null;
				}
			}
			waypoints = null;
			if( enemyComponents != null )
			{
				for( int i=0; i<enemyComponents.Length; i++ )
				{
					enemyComponent = enemyComponents[i];
					if( enemyComponent != null )
					{
						enemyComponent.ClearRoute();
					}
				}
			}
		}
		waypointRenderers = null;
		waypointAreas = null;
		if( waypointObjects != null )
		{
			for( int i=0; i<waypointObjects.Length; i++ )
			{
				waypointObject = waypointObjects[i];
				if( waypointObject != null )
				{
					Destroy( waypointObject );
					waypointObjects[i] = null;
				}
			}
			waypointObjects = null;
		}
		if( clearFromStructure && (challengeStructure != null) )
		{
			challengeStructure.ClearWaypointData();
		}
	}

	private void StartRouteEdition()
	{
		RoutePersistentData enemyRoute = null;
		int[] routeWaypointIndexes = null;

		if( enemyComponents != null )
		{
			if( (currentEnemyIndex > -1) && (enemyComponents.Length > currentEnemyIndex) )
			{
				currentEnemyComponent = enemyComponents[currentEnemyIndex];
				if( (currentEnemyComponent != null) && (challengeEditor != null) )
				{
					enemyRoute = currentEnemyComponent.GetWaypointIndexesRoute();
					routeWaypointIndexes = null;
					if( enemyRoute != null )
					{
						routeWaypointIndexes = enemyRoute.GetWaypointIndexesCopy();
					}
					challengeEditor.OnEditRoute( routeWaypointIndexes, false );
				}
			}
		}
	}

	private void EndRouteEdition()
	{
		currentEnemyComponent = null;
		if( challengeEditor != null )
		{
			challengeEditor.OnEditRoute( null, true );
		}
	}

	public bool SaveChallengeStructure( string path, int structureID, int difficulty, bool createMirror )
	{
		if( challengeStructure != null )
		{
			CorrectObjectPositions();
			return challengeStructure.SaveToFile( path, structureID, difficulty, createMirror );
		}
		return false;
	}

	public bool GetDataFromSavedStructure( ref int structureID, bool withIDSufix, ref int structureIDSufix, ref int difficulty )
	{
		if( challengeStructure != null )
		{
			if( challengeStructure.SavedSuccessfully() )
			{
				structureID = challengeStructure.GetStructureID( withIDSufix );
				structureIDSufix = challengeStructure.GetStructureIDSufix();
				difficulty = challengeStructure.GetDifficulty();
				return true;
			}
		}
		return false;
	}

	public bool LoadChallengeStructure( string pathWithName )
	{
		int validCharacterIndex = -1;
		string pathFromResources = null;
		Vector3 cameraPosition = Vector3.zero;

		if( !String.IsNullOrEmpty( pathWithName ) )
		{
			validCharacterIndex = pathWithName.IndexOf("/Resources/");
			if( validCharacterIndex > -1 )
			{
				pathFromResources = pathWithName.Substring( validCharacterIndex + 11 );
			}
			else
			{
				pathFromResources = pathWithName;
			}
			Debug.Log("Debug : ChallengeCanvas : pathFromResources = "+pathFromResources+".");
			if( challengeStructure != null )
			{
				if( challengeStructure.LoadFromResourcesFile( pathFromResources ) )
				{
					/*halmeida - Recreate the size marker so that it goes to the correct position
					considering the dimensions of the structure.*/
					CreateSizeMarker();
					if( (cameraObject != null) && (factoryFront != null) )
					{
						cameraPosition = cameraObject.transform.position;
						cameraPosition.x = tileColumnsPerScreen * tileWorldWidthFront / 2f;
						cameraObject.transform.position = cameraPosition;
					}
					CreateItemsFromStructure();
					CreateItemClustersFromStructure();
					UpdateItemClusterDisplay();
					CreateWaypointsFromStructure();
					CreateEnemiesFromStructure();
					if( currentPurpose != PURPOSE_EDIT_WAYPOINTS )
					{
						FocusOnWaypoints( false );
					}
					return true;
				}
				else
				{
					CreateSizeMarker();
				}
			}
		}
		return false;
	}

	public bool GetDataFromLoadedStructure( ref int structureID, bool withIDSufix, ref int structureIDSufix, ref int difficulty )
	{
		if( challengeStructure != null )
		{
			if( challengeStructure.LoadedSuccessfully() )
			{
				structureID = challengeStructure.GetStructureID( withIDSufix );
				structureIDSufix = challengeStructure.GetStructureIDSufix();
				difficulty = challengeStructure.GetDifficulty();
				return true;
			}
		}
		return false;
	}

	public int GetCurrentTileVariation()
	{
		return currentTileVariation;
	}

	public int GetCurrentItemID()
	{
		return currentItemID;
	}

	public int GetCurrentItemClusterIndex()
	{
		return currentItemClusterIndex;
	}

	public int GetCurrentEnemyCode()
	{
		return currentEnemyCode;
	}

	public bool IsEditingEnemyRoute()
	{
		return (currentEnemyIndex > -1);
	}

	public void ReactToPointerDown( int screenPointX, int screenPointY )
	{
		float worldPointX = 0f;
		float worldPointY = 0f;
		Vector2 worldPosition = Vector2.zero;
		Vector2 markerPosition = Vector2.zero;
		Sprite sprite = null;
		Bounds bounds = default( Bounds );
		float left = 0f;
		float top = 0f;
		bool intercepted = false;
		int clickedRow = -1;
		int clickedColumn = -1;
		int blueprintCode = ChallengeStructure.BLUEPRINT_CODE_INVALID;
		GameObject tempObject = null;
		Collider2D tempCollider = null;
		Vector2 tempPosition = Vector2.zero;
		int currentShapeIndex = -1;
		int itemClusterIndex = -1;
		Rect areaRect = default( Rect );
		int nextBlueprintCode = ChallengeStructure.BLUEPRINT_CODE_INVALID;
		bool isTriangleTile = false;

		if( inputActive && (currentPurpose != PURPOSE_INVALID) && (cameraComponent != null) )
		{
			worldPosition = cameraComponent.ScreenToWorldPoint( new Vector2( screenPointX, screenPointY ) );
			worldPointX = worldPosition.x;
			worldPointY = worldPosition.y;
			if( (spawnMarker != null) && (spawnMarkerRenderer != null) )
			{
				markerPosition = spawnMarker.transform.position;
				sprite = spawnMarkerRenderer.sprite;
				if( sprite != null )
				{
					bounds = sprite.bounds;
					left = markerPosition.x - bounds.extents.x;
					top = markerPosition.y + bounds.extents.y;
					if( UsefulFunctions.AreaContainsPoint( left, top, bounds.size.x, bounds.size.y, worldPointX, worldPointY ) )
					{
						intercepted = true;
						draggingSpawnMarker = true;
					}
				}
			}
			if( !intercepted )
			{
				if( (sizeMarker != null) && (sizeMarkerRenderer != null) )
				{
					markerPosition = sizeMarker.transform.position;
					sprite = sizeMarkerRenderer.sprite;
					if( sprite != null )
					{
						bounds = sprite.bounds;
						left = markerPosition.x - bounds.extents.x;
						top = markerPosition.y + bounds.extents.y;
						if( UsefulFunctions.AreaContainsPoint( left, top, bounds.size.x, bounds.size.y, worldPointX, worldPointY ) )
						{
							intercepted = true;
							draggingSizeMarker = true;
						}
					}
				}
			}
			if( !intercepted )
			{
				switch( currentPurpose )
				{
					case PURPOSE_EDIT_TILES:
						clickedRow = (int) (-1.0f * worldPointY / tileWorldHeightFront);
						clickedColumn = (int) (worldPointX / tileWorldWidthFront);
						if( challengeStructure != null )
						{
							blueprintCode = challengeStructure.GetBlueprintValue( clickedRow, clickedColumn, true );
							if( blueprintCode != ChallengeStructure.BLUEPRINT_CODE_INVALID )
							{
								if( currentTileVariation == 0 )
								{
									/*halmeida - if there was a tile at the position, we either change its shape or remove it.
									If there was no tile, we insert one according to the current blueprint insertion code.*/
									if( ChallengeStructure.IsTilePresenceCode( blueprintCode ) )
									{
										isTriangleTile = ChallengeStructure.IsTriangleTileCode( blueprintCode );
										if( !isTriangleTile )
										{
											/*halmeida - if it was normal and it can be turned into a triangle, it becomes a triangle.
											If it can't be turned, it is removed.*/
											Debug.Log("Debug : ChallengeCanvas : clicked at a normal tile.");
											currentShapeIndex = challengeStructure.GetTileShapeIndexFromBlueprint( clickedRow, clickedColumn, true );
											if( Tile.ShapeIndexCanBecomeTriangle( currentShapeIndex ) )
											{
												challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, true,
													ChallengeStructure.BLUEPRINT_CODE_TILE_TRIANGLE, true, false, true );
												Debug.Log("Debug : ChallengeCanvas : tried to create a triangular tile.");
											}
											else
											{
												challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, true,
													ChallengeStructure.BLUEPRINT_CODE_NO_TILE, true, true, true );
												Debug.Log("Debug : ChallengeCanvas : tile could not become triangle and was removed.");
											}
										}
										else
										{
											/*halmeida - if it was a triangle it is removed.*/
											challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, true,
												ChallengeStructure.BLUEPRINT_CODE_NO_TILE, true, true, true );
										}
									}
									else
									{
										challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, true, blueprintInsertionCode,
											true, true, true );
										brushingTiles = true;
									}
								}
								else
								{
									/*halmeida - we only try to change the look of an existing tile. In this mode, we do not change
									the shape of a tile, create a new one or remove an existing one.*/
									if( (factoryFront != null) && ChallengeStructure.IsTilePresenceCode( blueprintCode ) )
									{
										nextBlueprintCode = ChallengeStructure.GetAlternativeBlueprintCode( blueprintCode, currentTileVariation );
										currentShapeIndex = challengeStructure.GetTileShapeIndexFromBlueprint( clickedRow, clickedColumn, true );
										if( !factoryFront.HasTextureVariation( currentShapeIndex, currentTileVariation, ref sprite ) )
										{
											nextBlueprintCode = ChallengeStructure.GetOriginalBlueprintCode( blueprintCode );
										}
										challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, true, nextBlueprintCode,
											true, false, false );
									}
								}
							}
							else
							{
								Debug.Log("Debug : ChallengeCanvas : clicked outside structure area.");
							}
						}
						break;
					case PURPOSE_EDIT_ITEMS:
						if( (itemObjects != null) && (itemColliders != null) )
						{
							for( int i=0; i<itemObjects.Length; i++ )
							{
								tempObject = itemObjects[i];
								if( (tempObject != null) && (itemColliders.Length > i) )
								{
									tempPosition = tempObject.transform.position;
									tempCollider = itemColliders[i];
									if( tempCollider != null )
									{
										bounds = tempCollider.bounds;
										left = tempPosition.x - bounds.extents.x;
										top = tempPosition.y + bounds.extents.y;
										if( UsefulFunctions.AreaContainsPoint( left, top, bounds.size.x, bounds.size.y, worldPointX, worldPointY ) )
										{
											RemoveItem( i, true );
											intercepted = true;
											break;
										}
									}
								}
							}
						}
						if( !intercepted )
						{
							CreateItem( currentItemID, worldPosition, true );
						}
						break;
					case PURPOSE_EDIT_ITEM_CLUSTERS:
						if( (itemObjects != null) && (itemColliders != null) && (itemToItemClusters != null) )
						{
							for( int i=0; i<itemObjects.Length; i++ )
							{
								tempObject = itemObjects[i];
								if( (tempObject != null) && (itemColliders.Length > i) && (itemToItemClusters.Length > i) )
								{
									tempPosition = tempObject.transform.position;
									tempCollider = itemColliders[i];
									itemClusterIndex = itemToItemClusters[i];
									if( tempCollider != null )
									{
										bounds = tempCollider.bounds;
										left = tempPosition.x - bounds.extents.x;
										top = tempPosition.y + bounds.extents.y;
										if( UsefulFunctions.AreaContainsPoint( left, top, bounds.size.x, bounds.size.y, worldPointX, worldPointY ) )
										{
											/*halmeida - If the item is already associated to any cluster, we remove it from that cluster.
											If it belongs to no cluster we add it to the current.*/
											if( itemClusterIndex > -1 )
											{
												RemoveItemFromCluster( i, itemClusterIndex, true );
											}
											else
											{
												AddItemToCluster( i, currentItemClusterIndex, true );
											}
											UpdateItemClusterDisplay();
											intercepted = true;
											break;
										}
									}
								}
							}
						}
						break;
					case PURPOSE_EDIT_COLLIDERS:
						clickedRow = (int) (-1.0f * worldPointY / tileWorldHeightFront);
						clickedColumn = (int) (worldPointX / tileWorldWidthFront);
						if( challengeStructure != null )
						{
							challengeStructure.AddToColliderMerging( clickedRow, clickedColumn );
						}
						break;
					case PURPOSE_EDIT_ENEMIES:
						if( currentEnemyIndex != -1 )
						{
							/*halmeida - means we had already selected an enemy, either to edit its route or to remove it.*/
							if( waypointAreas != null )
							{
								for( int i=0; i<waypointAreas.Length; i++ )
								{
									areaRect = waypointAreas[i];
									left = areaRect.x;
									top = areaRect.y;
									if( UsefulFunctions.AreaContainsPoint( left, top, areaRect.width, areaRect.height, worldPointX, worldPointY ) )
									{
										if( currentEnemyComponent != null )
										{
											currentEnemyComponent.AddRouteWaypointIndex( i );
											StartRouteEdition();
											if( challengeStructure != null )
											{
												challengeStructure.AddWaypointIndexToEnemyRoute( currentEnemyIndex, i );
											}
										}
										intercepted = true;
										break;
									}
								}
							}
						}
						/*halmeida - If we didn't click at any waypoints, so we might be clicking at the previously selected enemy
						to remove it, or we might be clicking at a different enemy to select it.*/
						if( !intercepted && (enemyObjects != null) && (enemyColliders != null) )
						{
							for( int i=0; i<enemyObjects.Length; i++ )
							{
								tempObject = enemyObjects[i];
								if( (tempObject != null) && (enemyColliders.Length > i) )
								{
									tempPosition = tempObject.transform.position;
									tempCollider = enemyColliders[i];
									if( tempCollider != null )
									{
										bounds = tempCollider.bounds;
										left = tempPosition.x - bounds.extents.x;
										top = tempPosition.y + bounds.extents.y;
										if( UsefulFunctions.AreaContainsPoint( left, top, bounds.size.x, bounds.size.y, worldPointX, worldPointY ) )
										{
											if( currentEnemyIndex == -1 )
											{
												currentEnemyIndex = i;
												StartRouteEdition();
											}
											else
											{
												if( i == currentEnemyIndex )
												{
													currentEnemyIndex = -1;
													EndRouteEdition();
													RemoveEnemy( i, true );
												}
												else
												{
													currentEnemyIndex = i;
													StartRouteEdition();
												}
											}
											intercepted = true;
											break;
										}
									}
								}
							}
						}
						/*halmeida - If I didn't click a waypoint or an enemy, I can create an enemy at the position, but only
						if I'm not editing an enemy's route, cause I don't want to create an enemy if I click very close to a
						waypoint and miss it.*/
						if( !intercepted && (currentEnemyIndex == -1) )
						{
							CreateEnemy( currentEnemyCode, worldPosition, true );
						}
						break;
					case PURPOSE_EDIT_WAYPOINTS:
						if( waypointAreas != null )
						{
							for( int i=0; i<waypointAreas.Length; i++ )
							{
								areaRect = waypointAreas[i];
								left = areaRect.x;
								top = areaRect.y;
								if( UsefulFunctions.AreaContainsPoint( left, top, areaRect.width, areaRect.height, worldPointX, worldPointY ) )
								{
									StartWaypointAlteration( i );
									intercepted = true;
									break;
								}
							}
						}
						if( !intercepted )
						{
							CreateWaypoint( worldPosition, Waypoint.INVALID_WAYPOINT_ID, true );
						}
						break;
					case PURPOSE_EDIT_BACK_TILES:
						clickedRow = (int) (-1.0f * worldPointY / tileWorldHeightBack);
						clickedColumn = (int) (worldPointX / tileWorldWidthBack);
						if( challengeStructure != null )
						{
							blueprintCode = challengeStructure.GetBlueprintValue( clickedRow, clickedColumn, false );
							if( blueprintCode != ChallengeStructure.BLUEPRINT_CODE_INVALID )
							{
								if( currentTileVariation == 0 )
								{
									/*halmeida - if there was a tile at the position, we either change its shape or remove it.
									If there was no tile, we insert one according to the current blueprint insertion code.*/
									if( ChallengeStructure.IsTilePresenceCode( blueprintCode ) )
									{
										isTriangleTile = ChallengeStructure.IsTriangleTileCode( blueprintCode );
										if( !isTriangleTile )
										{
											/*halmeida - if it was normal and it can be turned into a triangle, it becomes a triangle.
											If it can't be turned, it is removed.*/
											Debug.Log("Debug : ChallengeCanvas : clicked at a normal tile.");
											currentShapeIndex = challengeStructure.GetTileShapeIndexFromBlueprint( clickedRow, clickedColumn, false );
											if( Tile.ShapeIndexCanBecomeTriangle( currentShapeIndex ) )
											{
												challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, false,
													ChallengeStructure.BLUEPRINT_CODE_TILE_TRIANGLE, true, false, true );
												Debug.Log("Debug : ChallengeCanvas : tried to create a triangular tile.");
											}
											else
											{
												challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, false,
													ChallengeStructure.BLUEPRINT_CODE_NO_TILE, true, true, true );
												Debug.Log("Debug : ChallengeCanvas : tile could not become triangle and was removed.");
											}
										}
										else
										{
											/*halmeida - if it was a triangle it is removed.*/
											challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, false,
												ChallengeStructure.BLUEPRINT_CODE_NO_TILE, true, true, true );
										}
									}
									else
									{
										challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, false, blueprintInsertionCode,
											true, true, true );
										brushingTiles = true;
									}
								}
								else
								{
									/*halmeida - we only try to change the look of an existing tile. In this mode, we do not change
									the shape of a tile, create a new one or remove an existing one.*/
									if( (factoryBack != null) && ChallengeStructure.IsTilePresenceCode( blueprintCode ) )
									{
										nextBlueprintCode = ChallengeStructure.GetAlternativeBlueprintCode( blueprintCode, currentTileVariation );
										currentShapeIndex = challengeStructure.GetTileShapeIndexFromBlueprint( clickedRow, clickedColumn, false );
										if( !factoryBack.HasTextureVariation( currentShapeIndex, currentTileVariation, ref sprite ) )
										{
											nextBlueprintCode = ChallengeStructure.GetOriginalBlueprintCode( blueprintCode );
										}
										challengeStructure.SetBlueprintValue( clickedRow, clickedColumn, false, nextBlueprintCode,
											true, false, false );
									}
								}
							}
							else
							{
								Debug.Log("Debug : ChallengeCanvas : clicked outside structure area.");
							}
						}
						break;
				}
			}
		}
	}

	public void ReactToPointerUp()
	{
		draggingSpawnMarker = false;
		draggingSizeMarker = false;
		brushingTiles = false;
	}

	public void ReactToReturnKeyDown()
	{
		if( currentEnemyIndex != -1 )
		{
			currentEnemyIndex = -1;
			EndRouteEdition();
		}
		else
		{
			TogglePlayer();
		}
	}

	public void ReactToEscapeKeyDown()
	{
		if( currentEnemyIndex != -1 )
		{
			if( currentEnemyComponent != null )
			{
				currentEnemyComponent.ReduceWaypointIndexesRoute();
				StartRouteEdition();
				if( challengeStructure != null )
				{
					challengeStructure.ReduceEnemyRoute( currentEnemyIndex );
				}
			}
		}
	}

	public void ReactToSpaceKey( bool down )
	{
		if( down && (cameraObject != null) )
		{
			draggingCamera = true;
			draggingCameraStart = cameraObject.transform.position;
			draggingMouseStart = Input.mousePosition;
		}
		else
		{
			draggingCamera = false;
		}
	}

	public void ReactToLeftKey( bool down )
	{
		if( playerComponent != null )
		{
			playerComponent.SetLeftDirectionMove( down );
		}
	}

	public void ReactToRightKey( bool down )
	{
		if( playerComponent != null )
		{
			playerComponent.SetRightDirectionMove( down );
		}
	}

	public void ReactToUpKey( bool down )
	{
		if( playerComponent != null )
		{
			playerComponent.SetUpDirectionMove( down );
		}
	}

	public void ReactToDownKey( bool down )
	{
		if( playerComponent != null )
		{
			playerComponent.SetDownDirectionMove( down );
		}
	}

	public void Progress( float timeStep )
	{
		Vector2 mouseWorldPosition = Vector2.zero;
		float mouseScreenPointX = 0f;
		float mouseScreenPointY = 0f;
		float mouseWorldPointX = 0f;
		float mouseWorldPointY = 0f;
		Vector2 markerPosition = Vector2.zero;
		float mouseRowFloat = 0f;
		float mouseColumnFloat = 0f;
		int mouseRowInt = -1;
		int mouseColumnInt = -1;
		int blueprintCode = ChallengeStructure.BLUEPRINT_CODE_INVALID;
		Vector3 cameraPosition = Vector3.zero;
		float cameraOffsetX = 0f;
		float cameraOffsetY = 0f;
		ItemController itemController = null;
		EnemyAgent enemyComponent = null;

		if( inputActive && (currentPurpose != PURPOSE_INVALID) && (cameraObject != null) && (cameraComponent != null) )
		{
			mouseScreenPointX = Input.mousePosition.x;
			mouseScreenPointY = Input.mousePosition.y;
			mouseWorldPosition = cameraComponent.ScreenToWorldPoint( new Vector3( mouseScreenPointX, mouseScreenPointY ) );
			mouseWorldPointX = mouseWorldPosition.x;
			mouseWorldPointY = mouseWorldPosition.y;

			if( draggingCamera )
			{
				cameraPosition = cameraObject.transform.position;
				cameraOffsetX = (mouseScreenPointX - draggingMouseStart.x) * draggingUnitsPerPixel;
				cameraOffsetY = (mouseScreenPointY - draggingMouseStart.y) * draggingUnitsPerPixel;
				cameraPosition.x = draggingCameraStart.x - cameraOffsetX;
				cameraPosition.y = draggingCameraStart.y - cameraOffsetY;
				cameraObject.transform.position = cameraPosition; 
			}
			//else
			//{
			//	if( (cameraController != null) && (playerController != null) )
			//	{
			//		cameraController.Progress( timeStep );
			//	}
				/*halmeida - can't call the progress of the camera here cause the camera's progress
				depends on the player position and, since the player has a rigidbody, the player
				position is only updated between FixedUpdates. The correct positioning of the camera
				depends on an propperly updated position of the player.*/
			//}

			if( draggingSpawnMarker && (spawnMarker != null) )
			{
				markerPosition = spawnMarker.transform.position;
				markerPosition.x = mouseWorldPointX;
				markerPosition.y = mouseWorldPointY;
				spawnMarker.transform.position = markerPosition;
			}
			else if( challengeStructure != null )
			{
				if( draggingSizeMarker && (sizeMarker != null) )
				{
					mouseRowFloat = ( -1.0f * mouseWorldPointY / tileWorldHeightFront );
					mouseColumnFloat = mouseWorldPointX / tileWorldWidthFront;
					mouseRowInt = (int)mouseRowFloat;
					mouseColumnInt = (int)mouseColumnFloat;
					markerPosition = sizeMarker.transform.position;
					markerPosition.x = mouseWorldPointX;
					markerPosition.y = mouseWorldPointY;
					sizeMarker.transform.position = markerPosition;
					if( mouseRowFloat - mouseRowInt > 0.5f )
					{
						mouseRowInt++;
					}
					if( mouseColumnFloat - mouseColumnInt > 0.5f )
					{
						mouseColumnInt++;
					}
					challengeStructure.SetStructureDimensions( mouseRowInt, mouseColumnInt );
				}
				else if( brushingTiles )
				{
					if( currentPurpose == PURPOSE_EDIT_TILES )
					{
						mouseRowFloat = ( -1f * mouseWorldPointY / tileWorldHeightFront );
						mouseColumnFloat = mouseWorldPointX / tileWorldWidthFront;
						mouseRowInt = (int)mouseRowFloat;
						mouseColumnInt = (int)mouseColumnFloat;
						blueprintCode = challengeStructure.GetBlueprintValue( mouseRowInt, mouseColumnInt, true );
						/*halmeida - the brushing function only creates tiles, it does not remove. If there was
						no tile, we insert one according to the currently selected code.*/
						if( blueprintCode == ChallengeStructure.BLUEPRINT_CODE_NO_TILE )
						{
							challengeStructure.SetBlueprintValue( mouseRowInt, mouseColumnInt, true, blueprintInsertionCode, true, true, true );
						}
					}
					else if( currentPurpose == PURPOSE_EDIT_BACK_TILES )
					{
						mouseRowFloat = ( -1f * mouseWorldPointY / tileWorldHeightBack );
						mouseColumnFloat = mouseWorldPointX / tileWorldWidthBack;
						mouseRowInt = (int)mouseRowFloat;
						mouseColumnInt = (int)mouseColumnFloat;
						blueprintCode = challengeStructure.GetBlueprintValue( mouseRowInt, mouseColumnInt, false );
						if( blueprintCode == ChallengeStructure.BLUEPRINT_CODE_NO_TILE )
						{
							challengeStructure.SetBlueprintValue( mouseRowInt, mouseColumnInt, false, blueprintInsertionCode, true, true, false );
						}
					}
				}
			}
		}
		if( playerComponent != null )
		{
			playerComponent.Progress( timeStep );
		}
		if( itemControllers != null )
		{
			for( int i=0; i<itemControllers.Length; i++ )
			{
				itemController = itemControllers[i];
				if( itemController != null )
				{
					itemController.Progress( timeStep );
				}
			}
		}
		if( enemyComponents != null )
		{
			for( int i=0; i<enemyComponents.Length; i++ )
			{
				enemyComponent = enemyComponents[i];
				if( enemyComponent != null )
				{
					enemyComponent.Progress( timeStep );
				}
			}
		}
	}

	public void FixedProgress( float timeStep )
	{
		if( playerComponent != null )
		{
			playerComponent.FixedProgress( timeStep );
		}
		if( !draggingCamera && (cameraController != null) )
		{
			cameraController.FixedProgress( timeStep );
		}
	}

	public void SetInputActive( bool enable )
	{
		inputActive = enable;
	}

	public void SetCameraFurther( bool enable, float distanceFactor = 1f )
	{
		if( cameraController != null )
		{
			if( enable )
			{
				cameraController.AlterOrthoSize( distanceFactor );
			}
			else
			{
				cameraController.ResetOrthoSize();
			}
		}
	}

	private void CorrectObjectPositions()
	{
		ItemController itemController = null;
		float newOffsetY = 0f;
		Vector2 newOffset = Vector2.zero;
		Vector3 newPosition = Vector3.zero;
		GameObject genericObject = null;

		if( itemControllers != null )
		{
			for( int i=0; i<itemControllers.Length; i++ )
			{
				itemController = itemControllers[i];
				if( itemController != null )
				{
					if( itemController.grounded )
					{
						if( PlaceItemOnFloor( i, itemController, ref newOffsetY ) )
						{
							newPosition = itemController.gameObject.transform.position;
							newPosition.y = newOffsetY;
							itemController.gameObject.transform.position = newPosition;
							newOffset.x = newPosition.x;
							newOffset.y = newPosition.y;
							challengeStructure.AlterItemDataOffset( i, newOffset );
						}
					}
					else
					{
						if( SnapObjectToGrid( itemController.gameObject, ref newOffset ) )
						{
							newPosition = itemController.gameObject.transform.position;
							newPosition.x = newOffset.x;
							newPosition.y = newOffset.y;
							itemController.gameObject.transform.position = newPosition;
							challengeStructure.AlterItemDataOffset( i, newOffset );
						}
					}
				}
			}
		}
		if( enemyObjects != null )
		{
			for( int i=0; i<enemyObjects.Length; i++ )
			{
				genericObject = enemyObjects[i];
				if( genericObject != null )
				{
					if( SnapObjectToGrid( genericObject, ref newOffset ) )
					{
						newPosition = genericObject.transform.position;
						newPosition.x = newOffset.x;
						newPosition.y = newOffset.y;
						genericObject.transform.position = newPosition;
						challengeStructure.AlterEnemyDataOffset( i, newOffset );
					}
				}
			}
		}
		if( waypointObjects != null )
		{
			for( int i=0; i<waypointObjects.Length; i++ )
			{
				genericObject = waypointObjects[i];
				if( genericObject != null )
				{
					if( SnapObjectToGrid( genericObject, ref newOffset ) )
					{
						newPosition = genericObject.transform.position;
						newPosition.x = newOffset.x;
						newPosition.y = newOffset.y;
						genericObject.transform.position = newPosition;
						challengeStructure.AlterWaypointDataOffset( i, newOffset );
					}
				}
			}
		}
	}
	
	private bool PlaceItemOnFloor( int itemIndex, ItemController itemController, ref float itemFinalOffsetY )
	{
		GameObject itemObject = null;
		Transform itemTrans = null;
		Rect itemRectLocal = default( Rect );
		Rect itemRectWorld = default( Rect );
		int totalRows = 0;
		int totalColumns = 0;
		int startRow = -1;
		int startColumn = -1;
		int endRow = -1;
		int endColumn = -1;
		bool collidedDown = false;
		int collisionRow = -1;
		int collisionColumn = -1;
		bool correctedUp = false;
		Vector2 collisionCenter = Vector2.zero;
		Vector3 newPosition = Vector3.zero;
		int attempts = 0;
		int maxAttempts = 40;

		if( (itemObjects != null) && (challengeStructure != null) && (itemController != null) && (factoryFront != null) )
		{
			if( itemObjects.Length > itemIndex )
			{
				itemObject = itemObjects[itemIndex];
				if( itemObject != null )
				{
					itemTrans = itemObject.transform;
					challengeStructure.GetStructureDimensions( ref totalRows, ref totalColumns );
					/*halmeida - if the item says it should be grounded, I gotta bring it to the floor. However, I do
					not use the item's collider, but actually the collision rect that it provides. The item's collider
					is used as its reactive area rather than its physical area, and to bring it to the ground I need
					its physical area.*/
					itemRectLocal = itemController.GetLocalCollisionRect();
					itemRectWorld = itemRectLocal;
					itemRectWorld.x += itemTrans.position.x;
					itemRectWorld.y += itemTrans.position.y;
					if( GetRowsAndColumnsWithinRect( itemRectWorld, ref startRow, ref startColumn, ref endRow, ref endColumn ) )
					{
						collidedDown = false;
						correctedUp = false;
						while( !collidedDown )
						{
							if( challengeStructure.AnyPositionOccupied( true, startRow, startColumn, endRow, endColumn,
								ref collisionRow, ref collisionColumn ) )
							{
								collidedDown = true;
							}
							else
							{
								startRow++;
								endRow++;
								if( startRow > totalRows )
								{
									return false;
								}
							}
						}
						while( !correctedUp )
						{
							if( challengeStructure.GetPositionForRowAndColumn( collisionRow, collisionColumn, true, ref collisionCenter ) )
							{
								itemRectWorld.y = collisionCenter.y + tileWorldHeightFront/2f + 0.001f;
								if( GetRowsAndColumnsWithinRect( itemRectWorld, ref startRow, ref startColumn, ref endRow, ref endColumn ) )
								{
									if( !challengeStructure.AnyPositionOccupied( true, startRow, startColumn, endRow, endColumn,
										ref collisionRow, ref collisionColumn ) )
									{
										correctedUp = true;
										/*halmeida - we corrected the position of the collision rectangle of the object so that it lies
										exactly above ground. From this new position of the rectangle we gotta obtain the new position
										of the object itself.*/
										itemFinalOffsetY = itemRectWorld.y - itemRectLocal.y;
										/*halmeida - if the item says that it should penetrate the ground a certain amount, we respect
										that and reduce the object's y.*/
										itemFinalOffsetY -= itemController.groundPenetration;
										return true;
									}
									else
									{
										attempts++;
										if( attempts > maxAttempts )
										{
											break;
										}
									}
								}
								else
								{
									return false;
								}
							}
							else
							{
								return false;
							}
						}
						if( !correctedUp )
						{
							Debug.Log("Debug : ChallengeCanvas : gave up trying to correct up an item's position.");
							Debug.Log("Debug : ChallengeCanvas : gave up at ("+collisionRow+", "+collisionColumn+").");
							return false;
						}
					}
					else
					{
						return false;
					}
				}
			}
		}
		return false;
	}

	private bool GetRowsAndColumnsWithinRect( Rect bottomLeftAnchoredRect, ref int topRow, ref int leftColumn, ref int bottomRow, ref int rightColumn )
	{
		Vector2 areaTopLeft = Vector2.zero;
		Vector2 areaBottomRight = Vector2.zero;

		if( challengeStructure != null )
		{
			areaTopLeft = new Vector2( bottomLeftAnchoredRect.x, bottomLeftAnchoredRect.y + bottomLeftAnchoredRect.height );
			areaBottomRight = new Vector2( bottomLeftAnchoredRect.x + bottomLeftAnchoredRect.width, bottomLeftAnchoredRect.y );
			if( challengeStructure.GetRowAndColumnAtPosition( ref topRow, ref leftColumn, true, areaTopLeft ) )
			{
				if( challengeStructure.GetRowAndColumnAtPosition( ref bottomRow, ref rightColumn, true, areaBottomRight ) )
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool SnapObjectToGrid( GameObject genericObject, ref Vector2 newObjectOffset )
	{
		Vector3 currentOffset = Vector3.zero;
		float coveredWidths = 0f;
		float coveredHeights = 0f;
		float floor = 0f;
		float floorAndHalf = 0f;
		float floorAndOne = 0f;

		if( (genericObject != null) && (factoryFront != null) )
		{
			currentOffset = genericObject.transform.position;
			coveredWidths = currentOffset.x / tileWorldWidthFront;
			floor = Mathf.Floor( coveredWidths );
			floorAndHalf = floor + 0.5f;
			floorAndOne = floor + 1f;
			if( coveredWidths >= floorAndHalf )
			{
				if( (coveredWidths - floorAndHalf) >= (floorAndOne - coveredWidths) )
				{
					newObjectOffset.x = floorAndOne * tileWorldWidthFront;
				}
				else
				{
					newObjectOffset.x = floorAndHalf * tileWorldWidthFront;
				}
			}
			else
			{
				if( (coveredWidths - floor) >= (floorAndHalf - coveredWidths) )
				{
					newObjectOffset.x = floorAndHalf * tileWorldWidthFront;
				}
				else
				{
					newObjectOffset.x = floor * tileWorldWidthFront;
				}
			}
			coveredHeights = (-1f * currentOffset.y) / tileWorldHeightFront;
			floor = Mathf.Floor( coveredHeights );
			floorAndHalf = floor + 0.5f;
			floorAndOne = floor + 1f;
			if( coveredHeights >= floorAndHalf )
			{
				if( (coveredHeights - floorAndHalf) >= (floorAndOne - coveredHeights) )
				{
					newObjectOffset.y = floorAndOne * tileWorldHeightFront;
				}
				else
				{
					newObjectOffset.y = floorAndHalf * tileWorldHeightFront;
				}
			}
			else
			{
				if( (coveredHeights - floor) >= (floorAndHalf - coveredHeights) )
				{
					newObjectOffset.y = floorAndHalf * tileWorldHeightFront;
				}
				else
				{
					newObjectOffset.y = floor * tileWorldHeightFront;
				}
			}
			newObjectOffset.y *= -1f;
			return true;
		}
		return false;
	}
}
