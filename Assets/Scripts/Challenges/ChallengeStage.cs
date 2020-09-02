using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeStage : MonoBehaviour
{
	public const int DIRECTION_INVALID = -1;
	public const int DIRECTION_UP = 0;
	public const int DIRECTION_LEFT = 1;
	public const int DIRECTION_DOWN = 2;
	public const int DIRECTION_RIGHT = 3;

	public const int CLEAR_INVALID = -1;
	public const int CLEAR_DEATH = 0;
	public const int CLEAR_VICTORY = 1;
	public const int CLEAR_EXIT = 2;

	public const float HP_DRAIN_SPEED_FACTOR = 0.8f;
	public const float ROOM_DARKENER_SPEED_FACTOR = 1f;
	public const float ROOM_DARKENER_ALPHA_LIMIT = 0.7f;

	private GameController gameController;
	private GameObject cameraObject;
	private Camera cameraComponent;
	private CameraController cameraController;
	private GameObject playerObjectParent;
	private GameObject playerObject;
	private PlayerAgent playerComponent;
	private GameObject enemyObjectsParent;
	private EnemyDatabase enemyDatabase;
	private GameObject[] enemyObjects;
	private EnemyAgent[] enemyComponents;
	private int[] enemyNodeIDs;
	private GameObject npcObjectsParent;
	private GameObject[] npcObjects;
	private PlayerAgent[] npcComponents;
	private bool[] npcEnmity;
	private GameObject structObjectsParent;
	private GameObject previousStructObject;
	private GameObject currentStructObject;
	private GameObject[] nextStructObjects;
	private ChallengeStructure previousStructComponent;
	private ChallengeStructure currentStructComponent;
	private ChallengeStructure[] nextStructComponents;
	private GraphNode previousNode;
	private GraphNode currentNode;
	private GraphNode[] nextNodes;
	private Rect previousNodeMapSlots;
	private Rect currentNodeMapSlots;
	private Rect[] nextNodeMapSlots;
	private Rect[] nextDoors;
	private int[] nextDoorDirections;
	private StructurePersistentData[] structData;
	private TileFactory[] tileFactories;
	private InputManager inputManager;
	private ItemDatabase itemDatabase;
	private GameObject itemObjectsParent;
	private GameObject[] itemObjects;
	private ItemController[] itemControllers;
	private int[] itemNodeIDs;
	private int[] itemToItemClusters;
	private GameObject[] itemClusterObjects;
	private ItemCluster[] itemClusters;
	private GameObject[] eventItemObjects;
	private ItemController[] eventItemControllers;
	private int[] eventItemEventIDs;
	private ObstacleDatabase obstacleDatabase;
	private GameObject obstacleObjectsParent;
	private GameObject[] eventObstacleObjects;
	private ObstacleController[] eventObstacleControllers;
	private int[] eventObstacleEventIDs;
	private Graph graph;
	private Rect[] newLocalDoors;
	private Rect[] newWorldDoors;
	private int[] newWorldDoorDirections;
	private GraphNode[] newNodes;
	private ChallengeStructure[] newStructComponents;
	private GameObject[] newStructObjects;
	private Rect[] newNodeMapSlots;
	private Rect newWorldDoorGradual;
	private GraphNode newNodeGradual;
	private ChallengeStructure newStructComponentGradual;
	private GameObject newStructObjectGradual;
	private int newElements;
	private int newElementsDirection;
	private int newElementIndex;
	private Rect currentStructRect;
	private Vector2 currentStructCenter;
	private bool previousNodeFound;
	private bool finishNodeConstruction;
	private bool advanceToNextNextNode;
	private bool buildingNextNodes;
	private int clearPending;
	private float drainSpeed;
	private float drainDamage;
	private RoomSightBlocker roomBlocker;
	private float roomSlotWidth;
	private float roomSlotHeight;
	private bool paused;
	private bool pausedFromOutside;
	private BaseAgent[] pausingAgents;
	private SymbolDatabase fontDialogue;
	private SymbolDatabase fontDamage;
	private SymbolDatabase fontUI;
	private BoxAppearanceDatabase boxDatabase;
	private TextBoxUIManager textBoxUIManager;
	private TextBoxManager textBoxManager;
	private TransactionBoxManager transactionBoxManager;
	private EventManager eventManager;
	private GameObject roomDarkenerObject;
	private SpriteRenderer roomDarkenerRenderer;
	private bool roomDarkenerDark;
	private bool roomDarkenerClear;
	private bool roomUnlocked;
	private Waypoint[] waypoints;
	private int[] waypointNodeIDs;
	private int waypointsFirstIndex;
	private bool fadeOutRequested;
	private bool fadeInRequested;
	private Vector3 fadeOutRGB;
	//private Vector3 fadeOutRGBNormal;
	private Vector3 fadeOutRGBDeath;
	private bool savingDisabled;
	private float savingDisabledDuration;
	private float savingDisabledElapsed;
	private BackgroundController backgroundController;

	private GameObject debugItemObject;
	private ItemController debugItemController;
	private GameObject debugObstacleObject;
	private ObstacleController debugObstacleController;

	void Awake()
	{
		Vector3 correctDepthPosition = Vector3.zero;

		gameController = null;
		cameraObject = null;
		cameraComponent = null;
		cameraController = null;
		playerObjectParent = null;
		playerObject = null;
		playerComponent = null;
		enemyObjectsParent = null;
		enemyDatabase = null;
		enemyObjects = null;
		enemyComponents = null;
		enemyNodeIDs = null;
		npcObjectsParent = null;
		npcObjects = null;
		npcComponents = null;
		npcEnmity = null;
		structObjectsParent = null;
		previousStructObject = null;
		currentStructObject = null;
		nextStructObjects = null;
		previousStructComponent = null;
		currentStructComponent = null;
		nextStructComponents = null;
		previousNode = null;
		currentNode = null;
		nextNodes = null;
		previousNodeMapSlots = new Rect( -1f, -1f, 0f, 0f );
		currentNodeMapSlots = new Rect( -1f, -1f, 0f, 0f );
		nextNodeMapSlots = null;
		nextDoors = null;
		nextDoorDirections = null;
		structData = null;
		tileFactories = null;
		inputManager = null;
		itemDatabase = null;
		itemObjectsParent = null;
		itemObjects = null;
		itemControllers = null;
		itemNodeIDs = null;
		itemToItemClusters = null;
		itemClusterObjects = null;
		itemClusters = null;
		eventItemObjects = null;
		eventItemControllers = null;
		eventItemEventIDs = null;
		obstacleDatabase = null;
		obstacleObjectsParent = null;
		eventObstacleObjects = null;
		eventObstacleControllers = null;
		eventObstacleEventIDs = null;
		graph = null;
		newLocalDoors = null;
		newWorldDoors = null;
		newWorldDoorDirections = null;
		newNodes = null;
		newStructComponents = null;
		newStructObjects = null;
		newNodeMapSlots = null;
		newWorldDoorGradual = new Rect( 0f, 0f, 0f, 0f );
		newNodeGradual = null;
		newStructComponentGradual = null;
		newStructObjectGradual = null;
		newElements = 0;
		newElementsDirection = DIRECTION_INVALID;
		newElementIndex = -1;
		currentStructRect = new Rect( 0f, 0f, 0f, 0f );
		currentStructCenter = Vector2.zero;
		previousNodeFound = false;
		finishNodeConstruction = false;
		advanceToNextNextNode = false;
		buildingNextNodes = false;
		clearPending = CLEAR_INVALID;
		drainSpeed = 0f;
		drainDamage = 0f;
		roomBlocker = null;
		roomSlotWidth = 0f;
		roomSlotHeight = 0f;
		paused = false;
		pausedFromOutside = false;
		pausingAgents = null;
		fontDialogue = null;
		fontDamage = null;
		fontUI = null;
		boxDatabase = null;
		textBoxUIManager = null;
		textBoxManager = null;
		transactionBoxManager = null;
		eventManager = null;
		roomDarkenerObject = null;
		roomDarkenerRenderer = null;
		roomDarkenerDark = false;
		roomDarkenerClear = false;
		roomUnlocked = false;
		waypoints = null;
		waypointNodeIDs = null;
		waypointsFirstIndex = -1;
		fadeOutRequested = false;
		fadeInRequested = false;
		fadeOutRGB = Vector3.zero;
		//fadeOutRGBNormal = Vector3.zero;
		fadeOutRGBDeath = new Vector3( 0.4f, 0f, 0f );
		savingDisabled = false;
		savingDisabledDuration = SaveItemController.QUESTION_DELAY + 0.2f;
		savingDisabledElapsed = 0f;
		backgroundController = null;

		debugItemObject = null;
		debugItemController = null;
		debugObstacleObject = null;
		debugObstacleController = null;

		structObjectsParent = new GameObject( "StructureObjectsParent" );
		structObjectsParent.transform.SetParent( gameObject.transform );
		structObjectsParent.transform.rotation = Quaternion.identity;
		structObjectsParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_TILE ) );
		structObjectsParent.transform.position = correctDepthPosition;

		itemObjectsParent = new GameObject( "ItemObjectsParent" );
		itemObjectsParent.transform.SetParent( gameObject.transform );
		itemObjectsParent.transform.rotation = Quaternion.identity;
		itemObjectsParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_ITEM ) );
		itemObjectsParent.transform.position = correctDepthPosition;

		obstacleObjectsParent = new GameObject( "ObstacleObjectsParent" );
		obstacleObjectsParent.transform.SetParent( gameObject.transform );
		obstacleObjectsParent.transform.rotation = Quaternion.identity;
		obstacleObjectsParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_OBSTACLE ) );
		obstacleObjectsParent.transform.position = correctDepthPosition;

		npcObjectsParent = new GameObject( "NpcObjectsParent" );
		npcObjectsParent.transform.SetParent( gameObject.transform );
		npcObjectsParent.transform.rotation = Quaternion.identity;
		npcObjectsParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_NPC ) );
		npcObjectsParent.transform.position = correctDepthPosition;

		enemyObjectsParent = new GameObject( "EnemyObjectsParent" );
		enemyObjectsParent.transform.SetParent( gameObject.transform );
		enemyObjectsParent.transform.rotation = Quaternion.identity;
		enemyObjectsParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_ENEMY ) );
		enemyObjectsParent.transform.position = correctDepthPosition;

		playerObjectParent = new GameObject( "PlayerObjectParent" );
		playerObjectParent.transform.SetParent( gameObject.transform );
		playerObjectParent.transform.rotation = Quaternion.identity;
		playerObjectParent.transform.localScale = Vector3.one;
		correctDepthPosition = new Vector3( 0f, 0f, DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_PLAYER ) );
		playerObjectParent.transform.position = correctDepthPosition;

		backgroundController = new BackgroundController();
		CreateRoomDarkener();
	}

	private void CreateRoomDarkener()
	{
		Vector3 position = Vector3.zero;
		Color color = Color.black;

		if( roomDarkenerObject == null )
		{ 
			roomDarkenerObject = new GameObject( "RoomDarkenerObject" );
			position.z = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_ROOM_BLOCKER );
			roomDarkenerObject.transform.position = position;
			roomDarkenerRenderer = roomDarkenerObject.AddComponent<SpriteRenderer>();
			roomDarkenerRenderer.sprite = UsefulFunctions.CreateSolidSprite( Color.black );
			color = roomDarkenerRenderer.color;
			color.a = 0f;
			roomDarkenerRenderer.color = color;
			roomUnlocked = true;
			roomDarkenerDark = false;
			roomDarkenerClear = true;
		}
	}

	public void SetGameAndCamera( GameController newGameController, Camera newCameraComponent, Canvas newCanvasComponent )
	{
		float cameraMaxWorldWidth = 0f;
		float cameraMaxWorldHeight = 0f;
		Vector3 position = Vector3.zero;

		gameController = newGameController;
		if( newCameraComponent != null )
		{
			cameraComponent = newCameraComponent;
			cameraObject = cameraComponent.gameObject;
			cameraController = cameraObject.GetComponent<CameraController>();
			if( cameraController != null )
			{
				cameraController.SetInterfaceCanvas( newCanvasComponent );
				if( roomDarkenerObject != null )
				{
					roomDarkenerObject.transform.SetParent( cameraObject.transform, false );
					position = cameraObject.transform.position;
					position.z = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_ROOM_DARKENER );
					roomDarkenerObject.transform.position = position;
					cameraController.GetCameraMaxWorldDimensions( ref cameraMaxWorldWidth, ref cameraMaxWorldHeight );
					roomDarkenerObject.transform.localScale = new Vector3( 2f * cameraMaxWorldWidth, 2f * cameraMaxWorldHeight, 1f );
				}
				if( backgroundController != null )
				{
					backgroundController.SetCameraController( cameraController );
				}
				return;
			}
		}
		cameraController = null;
		cameraComponent = null;
		cameraObject = null;
	}

	public void SetBuildingResources( TileFactory[] newTileFactories, StructurePersistentData[] newStructureData, Graph newGraph,
		RoomSightBlocker newRoomBlocker )
	{
		TileFactory factory = null;
		Vector2 tileSize = Vector2.zero;
		Vector3 roomBlockerPosition = Vector3.zero;

		tileFactories = newTileFactories;
		if( tileFactories != null )
		{
			/*halmeida - tile factories come validated from the game controller. If the array isn't null,
			then it has at least one position and all of its positions are not null tile factories.*/
			factory = tileFactories[0];
			tileSize = factory.GetTileWorldDimensions();
			roomSlotWidth = tileSize.x * ChallengeStructure.TILES_TO_MAP_SQUARE_SIDE;
			roomSlotHeight = tileSize.y * ChallengeStructure.TILES_TO_MAP_SQUARE_SIDE;
		}
		structData = newStructureData;
		graph = newGraph;
		roomBlocker = newRoomBlocker;
		if( roomBlocker != null )
		{
			roomBlockerPosition.z = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_ROOM_BLOCKER );
			roomBlocker.gameObject.transform.position = roomBlockerPosition;
			roomBlocker.SetBlockerDimensions( roomSlotWidth, roomSlotHeight );
		}
	}

	public void SetInputManager( InputManager newInputManager )
	{
		inputManager = newInputManager;
		if( inputManager != null )
		{
			inputManager.SetChallengeStage( this );
			inputManager.SetBoxManagers( textBoxUIManager, textBoxManager, transactionBoxManager );
		}
	}

	public void SetEventManager( EventManager newEventManager )
	{
		eventManager = newEventManager;
		if( eventManager != null )
		{
			eventManager.SetStage( this );
		}
	}

	public void SetDatabases( ItemDatabase newItemDatabase, ObstacleDatabase newObstacleDatabase, EnemyDatabase newEnemyDatabase,
		SymbolDatabase newFontDialogue, SymbolDatabase newFontDamage, SymbolDatabase newFontUI, BoxAppearanceDatabase newBoxDatabase,
		BackgroundDatabase newBackgroundDatabase )
	{
		RectTransform canvasTransform = null;

		itemDatabase = newItemDatabase;
		obstacleDatabase = newObstacleDatabase;
		enemyDatabase = newEnemyDatabase;
		fontDialogue = newFontDialogue;
		fontDamage = newFontDamage;
		fontUI = newFontUI;
		boxDatabase = newBoxDatabase;
		textBoxUIManager = TextBoxUIManager.Instance;
		if( textBoxUIManager != null )
		{
			textBoxUIManager.SetMaterials( fontUI, boxDatabase );
			if( cameraController != null )
			{
				canvasTransform = cameraController.GetCanvasTransform();
				textBoxUIManager.SetUICanvasTransform( canvasTransform );
			}
		}
		textBoxManager = TextBoxManager.Instance;
		if( textBoxManager != null )
		{
			textBoxManager.SetMaterials( fontDialogue, boxDatabase );
			textBoxManager.SetReferences( this, cameraController );
		}
		transactionBoxManager = TransactionBoxManager.Instance;
		if( transactionBoxManager != null )
		{
			transactionBoxManager.SetMaterials( fontDialogue, boxDatabase );
			transactionBoxManager.SetCameraController( cameraController );
		}
		if( inputManager != null )
		{
			inputManager.SetBoxManagers( textBoxUIManager, textBoxManager, transactionBoxManager );
		}
		if( backgroundController != null )
		{
			backgroundController.SetBackgroundDatabase( newBackgroundDatabase );
		}
	}

	public void SetPlayerComponent( PlayerAgent newPlayerComponent )
	{
		playerComponent = newPlayerComponent;
		if( playerComponent != null )
		{
			playerObject = playerComponent.gameObject;
			playerObject.transform.SetParent( playerObjectParent.transform, false );
			playerComponent.SetStageAndCamera( this, cameraController );
			playerComponent.SetDamageDisplayFont( fontDamage );
			playerComponent.SetLevelUpDisplayFont( fontDamage );
			playerComponent.CreateGauges();
			UpdateItemTriggers();
			UpdateItemClusterTargets();
			UpdateEnemyAdversaries();
		}
	}

	public bool StartBuilding( int startingNodeID, int startingNodeSlotRow, int startingNodeSlotColumn )
	{
		if( (startingNodeID != GraphNode.NODE_ID_INVALID) && (currentStructComponent == null) )
		{
			return SetCurrentNode( startingNodeID, startingNodeSlotRow, startingNodeSlotColumn );
		}
		return false;
	}

	private bool RestartBuilding( int startingNodeID )
	{
		bool resetNextNodes = false;

		if( startingNodeID != GraphNode.NODE_ID_INVALID )
		{
			if( eventManager != null )
			{
				eventManager.CloseNodeEvents();
			}
			if( roomBlocker != null )
			{
				roomBlocker.ClearAllLoadedNodes();
			}
			if( currentNode != null )
			{
				if( currentNode.ID == startingNodeID )
				{
					/*halmeida - in this case, the SetCurrentNode won't do anything besides returning true.
					When we removed every node from the roomBlocker, we were expecting the node construction
					to load them back into the roomBlocker. Since they won't be rebuilt, they won't be
					loaded. In this case, we need to reset the next nodes cause they are loaded upon reset.*/
					resetNextNodes = true;
				}
			}
			if( SetCurrentNode( startingNodeID ) )
			{
				/*halmeida - the only nodes that may have had their original conditions changed, like having their
				items collected, are the current node and the previous node. All the others are being loaded brand
				new.*/
				ResetNodes( true, true, resetNextNodes );
				return true; 
			}
		}
		return false;
	}

	private void ExtractDrainSpeed()
	{
		if( currentNode != null )
		{
			drainSpeed = currentNode.HPDrainSpeed;
		}
		else
		{
			drainSpeed = 0f;
		}
	}

	public void Progress( float timeStep )
	{
		Rect nextDoor = new Rect( 0f, 0f, 0f, 0f );
		int nextDoorDirection = DIRECTION_INVALID;
		Vector2 playerLocalPos = Vector2.zero;
		GraphNode nextNode = null;
		BaseAgent pausingAgent = null;
		int applicableDrainDamage = 0;

		/*halmeida - unity events may trigger situations that require pausing the game. For example, when collisions
		happen, unity sends events to my program. This event may be an enemy being hit by the player. The enemy may
		die and give experience to the player. This experience may cause the player to level up, and the level up
		effects require the game to pause for a little while. Since unity events are sent before the call to Update(),
		I should check for their occurrence before doing all operations of the Update().*/ 
		EvaluatePauseConditions();
		/*halmeida - after evaluating if the game should be paused or unpaused with basis on the events received
		before the Update(), we can proceed to the actual operations of the Update().*/
		if( !paused )
		{
			if( buildingNextNodes )
			{
				//Debug.Log("Debug : ChallengeStage : trying to build next nodes.");
				CreateNextArrays();
			}
			else
			{
				if( graph != null )
				{
					/*halmeida - the graph may have had an area unlocked between the last progress and the current one.
					This is due to the fact that the unlocking transaction happens as a reaction to an event sent by
					unity to my program. Reseting the nodes will ensure their content is loaded considering the current
					state of the area.*/
					if( graph.RequestingUnlockTreatment() )
					{
						ResetNodes( false, false, true );
						graph.DismissUnlockTreatmentRequest();
					}
				}
				if( (nextDoors != null) && (playerObject != null) )
				{
					playerLocalPos = playerObject.transform.localPosition;
					for( int i=0; i<nextDoors.Length; i++ )
					{
						/*halmeida - relying on the coherence of all the "next" arrays.*/ 
						nextDoor = nextDoors[i];
						nextDoorDirection = nextDoorDirections[i];
						if( DoorCrossed( nextDoor, nextDoorDirection, playerLocalPos ) )
						{
							nextNode = nextNodes[i];
							if( nextNode != null )
							{
								if( eventManager != null )
								{
									eventManager.CloseNodeEvents();
								}
								SetCurrentNode( nextNode.ID );
								if( eventManager != null )
								{
									eventManager.PrepareNodeEvents( nextNode.ID );
								}
							}
							break;
						}
					}
				}
			}
			if( playerComponent != null )
			{
				drainDamage += drainSpeed * timeStep;
				applicableDrainDamage = (int)drainDamage;
				if( applicableDrainDamage > 0 )
				{
					drainDamage -= applicableDrainDamage;
					playerComponent.SufferHPDamage( applicableDrainDamage, null, null, false );
				}
				if( playerComponent.IsDead() )
				{
					if( inputManager != null )
					{
						inputManager.ToggleControls( false, false );
					}
					//playerController.StopMovement();
					/*halmeida - I do not stop the movement now, so that we can allow the player to
					have a shooting star death animation, using his inertia to look nicer.*/
					RequestFadeOut( fadeOutRGBDeath );
					clearPending = CLEAR_DEATH;
				}
				playerComponent.Progress( timeStep );
			}
			ProgressItems( timeStep );
			ProgressItemClusters( timeStep );
			ProgressObstacles( timeStep );
			ProgressEnemies( timeStep );
			ProgressNpcs( timeStep );
			if( cameraController != null )
			{
				cameraController.Progress( timeStep );
			}
			if( roomBlocker != null )
			{
				roomBlocker.Progress( timeStep );
			}
			if( textBoxUIManager != null )
			{
				textBoxUIManager.Progress( timeStep );
			}
			if( textBoxManager != null )
			{
				textBoxManager.Progress( timeStep );
			}
			if( transactionBoxManager != null )
			{
				transactionBoxManager.Progress( timeStep );
			}
			if( eventManager != null )
			{
				eventManager.Progress( timeStep );
			}
			ProgressRoomDarkener( timeStep );
			if( savingDisabled )
			{
				savingDisabledElapsed += timeStep;
				if( savingDisabledElapsed > savingDisabledDuration )
				{
					savingDisabled = false;
				}
			}
		}
		else
		{
			if( pausingAgents != null )
			{
				for( int i=0; i<pausingAgents.Length; i++ )
				{
					pausingAgent = pausingAgents[i];
					if( pausingAgent != null )
					{
						if( pausingAgent.IsPaused() )
						{
							pausingAgent.Progress( timeStep );
							if( pausingAgent.ShouldUnpause() )
							{
								UsefulFunctions.DecreaseArray<BaseAgent>( ref pausingAgents, i );
								if( pausingAgents == null )
								{
									break;
								}
								else
								{
									i--;
								}
							}
						}
					}
				}
			}
			if( cameraController != null )
			{
				cameraController.Progress( timeStep );
			}
			if( textBoxUIManager != null )
			{
				textBoxUIManager.Progress( timeStep );
			}
			if( textBoxManager != null )
			{
				textBoxManager.Progress( timeStep );
			}
		}
	}

	public void AddPausingAgent( BaseAgent newPausingAgent )
	{
		if( newPausingAgent != null )
		{
			if( newPausingAgent.IsPaused() )
			{
				UsefulFunctions.IncreaseArray<BaseAgent>( ref pausingAgents, newPausingAgent );
			}
		}
	}

	private void EvaluatePauseConditions()
	{
		if( paused )
		{
			if( !pausedFromOutside && (pausingAgents == null) )
			{
				TogglePause( false );
			}
		}
		else
		{
			if( pausedFromOutside || (pausingAgents != null) )
			{
				TogglePause( true );
			}
		}
	}

	private bool DoorCrossed( Rect doorLine, int crossingDirection, Vector2 agentPosition )
	{
		switch( crossingDirection )
		{
			case DIRECTION_UP:
				if( doorLine.width > 0f )
				{
					return ( (agentPosition.y > doorLine.y) && (agentPosition.x > doorLine.x) &&
						(agentPosition.x < doorLine.x + doorLine.width) );
				}
				break;
			case DIRECTION_LEFT:
				if( doorLine.height > 0f )
				{
					/*halmeida - just as a reminder : the y value of a door is its highest y.*/
					return ( (agentPosition.x < doorLine.x) && (agentPosition.y < doorLine.y) &&
						(agentPosition.y > doorLine.y - doorLine.height) );
				}
				break;
			case DIRECTION_DOWN:
				if( doorLine.width > 0f )
				{
					return ( (agentPosition.y < doorLine.y) && (agentPosition.x > doorLine.x) &&
						(agentPosition.x < doorLine.x + doorLine.width) );
				}
				break;
			case DIRECTION_RIGHT:
				if( doorLine.height > 0f )
				{
					/*halmeida - just as a reminder : the y value of a door is its highest y.*/
					return ( (agentPosition.x > doorLine.x) && (agentPosition.y < doorLine.y) &&
						(agentPosition.y > doorLine.y - doorLine.height) );
				}
				break;
		}
		return false;
	}

	public void FixedProgress( float timeStep )
	{
		BaseAgent agentComponent = null;

		if( !paused )
		{
			if( playerComponent != null )
			{
				playerComponent.FixedProgress( timeStep );
			}
			if( npcComponents != null )
			{
				for( int i=0; i<npcComponents.Length; i++ )
				{
					agentComponent = npcComponents[i];
					if( agentComponent != null )
					{
						agentComponent.FixedProgress( timeStep );
					}
				}
			}
			if( enemyComponents != null )
			{
				for( int i=0; i<enemyComponents.Length; i++ )
				{
					agentComponent = enemyComponents[i];
					if( agentComponent != null )
					{
						agentComponent.FixedProgress( timeStep );
					}
				}
			}
			if( cameraController != null )
			{
				cameraController.FixedProgress( timeStep );
			}
			if( backgroundController != null )
			{
				backgroundController.FixedProgress( timeStep );
			}
		}
	}

	public void Play( Vector2 playerOffsetWithinNode, bool playerFacingSide )
	{
		Vector3 playerLocalPosition = Vector3.zero;
		Vector3 cameraLocalPosition = Vector3.zero;

		if( currentStructComponent != null )
		{
			if( playerObject != null )
			{
				/*halmeida - since I will be placing the player into the stage, it may immediately collide
				with a save game trigger. However, we are placing the player at the exact state in which
				the game was saved, so we don't want the save operation to be triggered.*/
				/*halmeida - I have inserted a delay for the save question to pop up once a save game trigger
				is triggered. Once the player is placed into the stage, it will collide with the trigger, but
				the save question will only appear if the player remains inside the trigger for some time. We
				gotta be able to prevent the save question from popping up when that time completes and that
				is why we will disable saving for a certain amount of time. If the player leaves the trigger,
				the save point will be responsible for telling this class to allow future save requests.*/
				savingDisabled = true;
				savingDisabledElapsed = 0f;
				if( playerOffsetWithinNode.x > 0f )
				{
					playerLocalPosition = new Vector3( currentStructRect.x + playerOffsetWithinNode.x,
						currentStructRect.y + playerOffsetWithinNode.y, 0f );
				}
				else
				{
					playerLocalPosition = new Vector3( currentStructCenter.x, currentStructCenter.y, 0f );
				}
				playerObject.transform.localPosition = playerLocalPosition;
				if( playerComponent != null )
				{
					/*halmeida - since we changed the player's depth, we most likely messed up
					the displaying order of some higher display priority elements. This happens
					because the equipments have smaller depths than the player himself, so that
					they may be shown on top of him. Given enough equipments, they might invade
					the depth stripe of other smaller depth elements that show with higher
					priority than the player. To keep the equipments within the player's depth
					stripe, the PlayerAgent has a depth correcting function.*/
					playerComponent.CorrectDepthChain( true );
					playerComponent.PrepareForRetry( playerFacingSide );
				}
			}
			if( cameraObject != null )
			{
				cameraLocalPosition = cameraObject.transform.localPosition;
				cameraLocalPosition.x = currentStructCenter.x;
				cameraLocalPosition.y = currentStructCenter.y;
				cameraObject.transform.localPosition = cameraLocalPosition;
				cameraController.SetTargetPlayer( playerComponent, CameraController.SMOOTH_LERP_FACTOR );
			}
			if( inputManager != null )
			{
				inputManager.ToggleControls( true, true );
			}
			/*halmeida - let the game fade in. If any events still need the
			screen to be faded out, they can reassign the fade out request.*/
			RequestFadeIn();
			if( eventManager != null )
			{
				eventManager.PrepareNodelessEvents();
				/*halmeida - if currentStructComponent isn't null, currentNode also isn't.*/
				eventManager.PrepareNodeEvents( currentNode.ID );
			}
		}
	}

	public void AddDebugItem( GameObject debugItemModel )
	{
		Vector3 debugPosition = Vector3.zero;

		ClearDebugItem();
		if( debugItemModel != null )
		{
			debugItemObject = Instantiate( debugItemModel, Vector3.zero, Quaternion.identity ) as GameObject;
			if( debugItemObject != null )
			{
				debugItemController = debugItemObject.GetComponent<ItemController>();
				if( debugItemController != null )
				{
					if( cameraObject != null )
					{
						debugPosition = cameraObject.transform.localPosition;
						debugPosition.x -= 1f;
						debugPosition.z = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_ITEM );
						debugItemObject.transform.position = debugPosition;
					}
					debugItemController.CorrectDepthChain();
				}
				else
				{
					Destroy( debugItemObject );
					debugItemObject = null;
				}
			}
		}
	}

	public void AddDebugObstacle( GameObject debugObstacleModel )
	{
		Vector3 debugPosition = Vector3.zero;

		ClearDebugObstacle();
		if( debugObstacleModel != null )
		{
			debugObstacleObject = Instantiate( debugObstacleModel, Vector3.zero, Quaternion.identity ) as GameObject;
			if( debugObstacleObject != null )
			{
				debugObstacleController = debugObstacleObject.GetComponent<ObstacleController>();
				if( debugObstacleController != null )
				{
					if( cameraObject != null )
					{
						debugPosition = cameraObject.transform.localPosition;
						debugPosition.x -= 1f;
						debugPosition.z = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_OBSTACLE );
						debugObstacleObject.transform.position = debugPosition;
					}
				}
				else
				{
					Destroy( debugObstacleObject );
					debugObstacleObject = null;
				}
			}
		}
	}

	public void RequestFadeOut( Vector3 rgb )
	{
		fadeInRequested = false;
		fadeOutRequested = true;
		fadeOutRGB = rgb;
	}

	public void RequestFadeIn()
	{
		fadeOutRequested = false;
		fadeInRequested = true;
	}

	public bool RequestingFadeOut( ref Vector3 rgb )
	{
		if( fadeOutRequested )
		{
			rgb = fadeOutRGB;
			return true;
		}
		return false;
	}

	public bool RequestingFadeIn()
	{
		return fadeInRequested;
	}

	public void ConcludeFadeOut()
	{
		fadeOutRequested = false;	
	}

	public void ConcludeFadeIn()
	{
		fadeInRequested = false;
	}

	public bool RequestingClear( ref int clearType )
	{
		if( clearPending > CLEAR_INVALID )
		{
			clearType = clearPending;
			return true;
		}
		return false;
	}

	public bool DeathClear( int restartNodeID )
	{
		if( clearPending == CLEAR_DEATH )
		{
			if( cameraController != null )
			{
				cameraController.ClearCanvasTexts();
			}
			if( RestartBuilding( restartNodeID ) )
			{
				clearPending = CLEAR_INVALID;
				return true;
			}
		}
		return false;
	}

	public bool PauseGameForQuestion( IOptionsListener questionListener )
	{
		if( gameController != null )
		{
			return gameController.PauseForQuestion( questionListener );
		}
		return false;
	}

	public bool UnpauseGameFromQuestion( IOptionsListener questionListener )
	{
		if( gameController != null )
		{
			return gameController.UnpauseFromQuestion( questionListener );
		}
		return false;
	}

	public bool PauseGameForUIItemInfo()
	{
		if( gameController != null )
		{
			return gameController.PauseForUIItemInfo();
		}
		return false;
	}

	public bool SaveGameProgress()
	{
		Vector2 offsetWithinNode = Vector2.zero;
		Vector3 playerLocalPosition = Vector3.zero;

		if( (gameController != null) && (currentNode != null) )
		{
			if( gameController.SaveGameProgress( currentNode.ID ) )
			{
				if( (currentStructComponent != null) && (playerObject != null) )
				{
					playerLocalPosition = playerObject.transform.localPosition;
					offsetWithinNode.x = playerLocalPosition.x - currentStructRect.x;
					offsetWithinNode.y = playerLocalPosition.y - currentStructRect.y;
				}
				return gameController.SavePlayerProgress( offsetWithinNode );
			}
		}
		return false;
	}

	public bool SavedProgressExists()
	{
		if( gameController != null )
		{
			return gameController.ExistsRecordedGameProgress();
		}
		return false;
	}

	private void UpdateItemTriggers()
	{
		ItemController itemController = null;

		if( itemControllers != null )
		{
			for( int i=0; i<itemControllers.Length; i++ )
			{
				itemController = itemControllers[i];
				if( itemController != null )
				{
					itemController.SetTriggerSource( gameObject );
				}
			}
		}
		if( eventItemControllers != null )
		{
			for( int i=0; i<eventItemControllers.Length; i++ )
			{
				itemController = eventItemControllers[i];
				if( itemController != null )
				{
					itemController.SetTriggerSource( gameObject );
				}
			}
		}
	}

	private void UpdateItemClusterTargets()
	{
		ItemCluster itemCluster = null;

		if( itemClusters != null )
		{
			for( int i=0; i<itemClusters.Length; i++ )
			{
				itemCluster = itemClusters[i];
				if( itemCluster != null )
				{
					itemCluster.SetRewardTarget( playerComponent );
				}
			}
		}
	}

	private void UpdateEnemyAdversaries()
	{
		PlayerAgent[] currentPlayerComponents = null;
		PlayerAgent npcComponent = null;
		EnemyAgent enemyComponent = null;

		currentPlayerComponents = GetAllPlayerComponents();
		if( npcComponents != null )
		{
			for( int i=0; i<npcComponents.Length; i++ )
			{
				npcComponent = npcComponents[i];
				if( npcComponent != null )
				{
					/*halmeida - relying on the coherence of all the "npc" arrays.*/
					if( npcEnmity[i] )
					{
						npcComponent.SetAdversaries( currentPlayerComponents );
					}
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
					enemyComponent.SetAdversaries( currentPlayerComponents );
				}
			}
		}
	}

	private void UpdatePlayerAdversaries()
	{
		BaseAgent[] allAdversaries = null;
		PlayerAgent npcComponent = null;

		if( playerComponent != null )
		{
			if( npcComponents != null )
			{
				for( int i=0; i<npcComponents.Length; i++ )
				{
					npcComponent = npcComponents[i];
					if( npcComponent != null )
					{
						/*halmeida - relying on the coherence of all the "npc" arrays.*/
						if( npcEnmity[i] )
						{
							UsefulFunctions.IncreaseArray<BaseAgent>( ref allAdversaries, npcComponent );
						}
					}
				}
			}
			UsefulFunctions.IncreaseArrayWithArray<BaseAgent>( ref allAdversaries, enemyComponents );
			playerComponent.SetAdversaries( allAdversaries );
		}
	}

	private void CreateStructureItems( int nodeID, GameObject structureObject, ChallengeStructure structureComponent )
	{
		int[] itemIDs = null;
		int itemID = ItemData.INVALID_ITEM_ID;
		Vector2[] itemOffsets = null;
		int length = 0;
		GameObject itemModel = null;
		Vector2 itemOffset = Vector2.zero;
		Vector2 itemPosition = Vector2.zero;

		if( (itemDatabase != null) && (structureObject != null) && (structureComponent != null) )
		{
			itemIDs = structureComponent.GetItemIDs();
			itemOffsets = structureComponent.GetItemOffsets();
			if( (itemIDs != null) && (itemOffsets != null) )
			{
				length = itemIDs.Length;
				if( itemOffsets.Length == length )
				{
					for( int i=0; i<length; i++ )
					{
						itemID = itemIDs[i];
						//Debug.Log("Debug : ChallengeStage : will try to create item with code "+itemCode+".");
						itemModel = itemDatabase.GetItemModel( itemID );
						if( itemModel != null )
						{
							itemPosition.x = structureObject.transform.position.x;
							itemPosition.y = structureObject.transform.position.y;
							itemOffset = itemOffsets[i];
							itemPosition.x += itemOffset.x;
							itemPosition.y += itemOffset.y;
							CreateItem( itemModel, itemPosition, nodeID );
						}
						else
						{
							Debug.Log("Debug : ChallengeStage : item model for the ID "+itemID+" is null.");
						}
					}
				}
			}
		}
	}

	public ItemController CreateItem( GameObject itemModel, Vector2 itemPosition, int nodeID = GraphNode.NODE_ID_INVALID,
		int eventID = EventBase.INVALID_EVENT_ID )
	{
		Vector3 completePosition = Vector3.zero;
		GameObject newItem = null;
		ItemController itemController = null;

		if( itemModel != null )
		{
			completePosition.x = itemPosition.x;
			completePosition.y = itemPosition.y;
			newItem = Instantiate( itemModel, completePosition, Quaternion.identity ) as GameObject;
			itemController = newItem.GetComponent<ItemController>();
			if( itemController != null )
			{
				newItem.transform.SetParent( itemObjectsParent.transform, false );
				itemController.SetTriggerSource( gameObject );
				itemController.CorrectDepthChain();
				if( eventID == EventBase.INVALID_EVENT_ID )
				{
					UsefulFunctions.IncreaseArray<GameObject>( ref itemObjects, newItem );
					UsefulFunctions.IncreaseArray<ItemController>( ref itemControllers, itemController );
					UsefulFunctions.IncreaseArray<int>( ref itemNodeIDs, nodeID );
					UsefulFunctions.IncreaseArray<int>( ref itemToItemClusters, -1 );
				}
				else
				{
					UsefulFunctions.IncreaseArray<GameObject>( ref eventItemObjects, newItem );
					UsefulFunctions.IncreaseArray<ItemController>( ref eventItemControllers, itemController );
					UsefulFunctions.IncreaseArray<int>( ref eventItemEventIDs, eventID );
				}
			}
			else
			{
				Destroy( newItem );
				newItem = null;
			}
		}
		return itemController;
	}

	private void CreateStructureItemClusters( int nodeID, ChallengeStructure structureComponent )
	{
		int[] itemToItemClustersRead = null;
		int[] itemClusterRewardCodesRead = null;
		GameObject itemClusterObject = null;
		ItemCluster itemCluster = null;
		int clustersLength = 0;
		int itemsLength = 0;
		int rewardCode = ItemCluster.REWARD_CODE_INVALID;
		int clusterIndex = -1;
		ItemController itemController = null;
		int newItemsStart = -1;
		int newClustersStart = -1;

		if( structureComponent != null )
		{
			itemClusterRewardCodesRead = structureComponent.GetItemClusterRewardCodes();
			if( itemClusterRewardCodesRead != null )
			{
				clustersLength = itemClusterRewardCodesRead.Length;
				if( clustersLength > 0 )
				{
					for( int i=0; i<clustersLength; i++ )
					{
						rewardCode = itemClusterRewardCodesRead[i];
						CreateItemCluster( ref itemClusterObject, ref itemCluster, rewardCode, nodeID );
						UsefulFunctions.IncreaseArray<GameObject>( ref itemClusterObjects, itemClusterObject );
						UsefulFunctions.IncreaseArray<ItemCluster>( ref itemClusters, itemCluster );
						itemClusterObject = null;
						itemCluster = null;
					}
				}
				newClustersStart = itemClusters.Length - clustersLength;
			}
			if( itemControllers != null )
			{
				itemToItemClustersRead = structureComponent.GetItemToItemClusters();
				if( itemToItemClustersRead != null )
				{
					itemsLength = itemToItemClustersRead.Length;
					/*halmeida - relying on the coherence of all the "item" arrays.*/
					newItemsStart = itemToItemClusters.Length - itemsLength;
					for( int i=0; i<itemsLength; i++ )
					{
						/*halmeida - the cluster index we read is within the structure. However, our cluster
						array may have clusters of many other structures. Since we have just added all of
						this structure's clusters to the end of the array, we know where do the new clusters
						start, so we sum the index to this starting position within the array to get the
						final index of the cluster within the array.*/
						clusterIndex = itemToItemClustersRead[i];
						if( clusterIndex > -1 )
						{
							clusterIndex += newClustersStart;
							if( itemClusters != null )
							{
								if( clusterIndex < itemClusters.Length )
								{
									itemCluster = itemClusters[clusterIndex];
									if( itemCluster != null )
									{
										/*halmeida - i is just an index within the items read from the structure. However,
										the item array may have items from many other structures. For us to know where in
										this large array the items of this particular structure can be found, we need the
										items to have been added to the array just before calling this method, so that we
										can be sure the items for this structure are at the end of the large array.*/
										itemController = itemControllers[i+newItemsStart];
										if( itemController != null )
										{
											itemCluster.IncludeItem( itemController );
											itemToItemClusters[i+newItemsStart] = clusterIndex;
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

	private bool CreateItemCluster( ref GameObject itemClusterObject, ref ItemCluster itemCluster, int itemClusterRewardCode,
		int nodeID )
	{
		float scaleChange = 0f;

		if( (itemClusterObject == null) && (itemCluster == null) )
		{
			itemClusterObject = new GameObject("ItemClusterNode"+nodeID);
			itemClusterObject.transform.position = Vector3.zero;
			itemClusterObject.transform.rotation = Quaternion.identity;
			itemClusterObject.transform.localScale = Vector3.one;
			itemClusterObject.transform.SetParent( itemObjectsParent.transform, false );
			itemCluster = itemClusterObject.AddComponent<ItemCluster>();
			itemCluster.rewardCode = itemClusterRewardCode;
			scaleChange = ItemCluster.DEFAULT_SCALE_CHANGE_SECOND_ITEM;
			itemCluster.scaleChangeSecondItem = new Vector3( scaleChange, scaleChange, 1f );
			scaleChange = ItemCluster.DEFAULT_SCALE_CHANGE_LAST_ITEM;
			itemCluster.scaleChangeLastItem = new Vector3( scaleChange, scaleChange, 1f );
			itemCluster.SetRewardTarget( playerComponent );
			return true;
		}
		return false;
	}

	private void RemoveItem( int itemIndex, int eventID = EventBase.INVALID_EVENT_ID )
	{
		GameObject itemObject = null;
		ItemController itemController = null;
		int clusterIndex = -1;

		if( eventID == EventBase.INVALID_EVENT_ID )
		{
			if( (itemIndex > -1) && (itemObjects != null) )
			{
				if( itemIndex < itemObjects.Length )
				{
					/*halmeida - relying on the coherence of all the "item" arrays.*/
					clusterIndex = itemToItemClusters[itemIndex];
					if( clusterIndex > -1 )
					{
						/*halmeida - the check above aims to accelerate the particular case where the
						item is not related to any cluster.*/
						RemoveItemFromCluster( itemIndex, clusterIndex );
					}
					UsefulFunctions.DecreaseArray<int>( ref itemToItemClusters, itemIndex );
					UsefulFunctions.DecreaseArray<int>( ref itemNodeIDs, itemIndex );
					itemController = itemControllers[itemIndex];
					if( itemController != null )
					{
						itemController.Clear();
						itemControllers[itemIndex] = null;
					}
					UsefulFunctions.DecreaseArray<ItemController>( ref itemControllers, itemIndex );
					itemObject = itemObjects[itemIndex];
					if( itemObject != null )
					{
						Destroy( itemObject );
						itemObjects[itemIndex] = null;
					}
					UsefulFunctions.DecreaseArray<GameObject>( ref itemObjects, itemIndex );
				}
			}
		}
		else
		{
			if( eventItemObjects != null )
			{
				for( int i=0; i<eventItemObjects.Length; i++ )
				{
					/*halmeida - relying on the coherence of all the "eventItem" arrays.*/
					if( eventItemEventIDs[i] == eventID )
					{
						UsefulFunctions.DecreaseArray<int>( ref eventItemEventIDs, i );
						itemController = eventItemControllers[i];
						if( itemController != null )
						{
							itemController.Clear();
							eventItemControllers[i] = null;
						}
						UsefulFunctions.DecreaseArray<ItemController>( ref eventItemControllers, i );
						itemObject = eventItemObjects[i];
						if( itemObject != null )
						{
							Destroy( itemObject );
							eventItemObjects[i] = null;
						}
						UsefulFunctions.DecreaseArray<GameObject>( ref eventItemObjects, i );
						if( eventItemObjects == null )
						{
							return;
						}
						else
						{
							i--;
						}
					}
				}
			}
		}
	}

	private void RemoveItemFromCluster( int itemIndex, int itemClusterIndex )
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
						}
					}
					if( itemCluster.GetTotalItems() == 0 )
					{
						RemoveItemCluster( itemClusterIndex );
					}
				}
			}
		}
	}

	private void RemoveItemCluster( int itemClusterIndex )
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
		}
	}

	public void RemoveEventItem( int eventID )
	{
		if( eventID != EventBase.INVALID_EVENT_ID )
		{
			RemoveItem( -1, eventID );
		}
	}

	/*hamleida - ClearItems does not clear event created items, cause if it did, the events that create
	items would continue monitoring garbage. If an event creates an item, it must also clear the item.*/
	private void ClearItems( int targetNodeID = GraphNode.NODE_ID_INVALID )
	{
		int nodeID = GraphNode.NODE_ID_INVALID;

		if( itemObjects != null )
		{
			for( int i=0; i<itemObjects.Length; i++ )
			{
				/*halmeida - relying on the coherence of all the "item" arrays.*/
				nodeID = itemNodeIDs[i];
				if( (nodeID == targetNodeID) || (targetNodeID == GraphNode.NODE_ID_INVALID) )
				{
					RemoveItem( i );
					if( itemObjects == null )
					{
						break;
					}
					else
					{
						i--;
					}
				}
			}
		}
		ClearDebugItem();
	}

	private void ClearDebugItem()
	{
		if( debugItemController != null )
		{
			debugItemController.Clear();
			debugItemController = null;
		}
		if( debugItemObject != null )
		{
			Destroy( debugItemObject );
			debugItemObject = null;
		}
	}

	/*halmeida - for now, the only way to create an obstacle is through an event or through the debug option.*/
	public ObstacleController CreateEventObstacle( GameObject obstacleModel, Vector2 obstaclePosition, int eventID )
	{
		Vector3 completePosition = Vector3.zero;
		GameObject newObstacle = null;
		ObstacleController obstacleController = null;

		if( (obstacleModel != null) && (eventID != EventBase.INVALID_EVENT_ID) )
		{
			completePosition.x = obstaclePosition.x;
			completePosition.y = obstaclePosition.y;
			newObstacle = Instantiate( obstacleModel, completePosition, Quaternion.identity ) as GameObject;
			obstacleController = newObstacle.GetComponent<ObstacleController>();
			if( obstacleController != null )
			{
				newObstacle.transform.SetParent( obstacleObjectsParent.transform, false );
				UsefulFunctions.IncreaseArray<GameObject>( ref eventObstacleObjects, newObstacle );
				UsefulFunctions.IncreaseArray<ObstacleController>( ref eventObstacleControllers, obstacleController );
				UsefulFunctions.IncreaseArray<int>( ref eventObstacleEventIDs, eventID );
			}
			else
			{
				Destroy( newObstacle );
				newObstacle = null;
			}
		}
		return obstacleController;
	}

	public void RemoveEventObstacle( int eventID )
	{
		GameObject obstacleObject = null;
		ObstacleController obstacleController = null;

		if( eventObstacleObjects != null )
		{
			for( int i=0; i<eventObstacleObjects.Length; i++ )
			{
				/*halmeida - relying on the coherence of all the "eventObstacle" arrays.*/
				if( eventObstacleEventIDs[i] == eventID )
				{
					UsefulFunctions.DecreaseArray<int>( ref eventObstacleEventIDs, i );
					obstacleController = eventObstacleControllers[i];
					if( obstacleController != null )
					{
						obstacleController.Clear();
						eventObstacleControllers[i] = null;
					}
					UsefulFunctions.DecreaseArray<ObstacleController>( ref eventObstacleControllers, i );
					obstacleObject = eventObstacleObjects[i];
					if( obstacleObject != null )
					{
						Destroy( obstacleObject );
						eventObstacleObjects[i] = null;
					}
					UsefulFunctions.DecreaseArray<GameObject>( ref eventObstacleObjects, i );
					if( eventObstacleObjects == null )
					{
						return;
					}
					else
					{
						i--;
					}
				}
			}
		}
	}

	private void ClearDebugObstacle()
	{
		if( debugObstacleController != null )
		{
			debugObstacleController.Clear();
			debugObstacleController = null;
		}
		if( debugObstacleObject != null )
		{
			Destroy( debugObstacleObject );
			debugObstacleObject = null;
		}
	}

	private void CreateStructureWaypoints( int nodeID, GameObject structureObject, ChallengeStructure structureComponent )
	{
		Vector2[] waypointOffsets = null;
		Vector2 waypointOffset = Vector2.zero;
		Vector2 waypointPosition = Vector2.zero;
		int[] waypointIDs = null;
		int waypointID = Waypoint.INVALID_WAYPOINT_ID;
		int length = 0;

		if( (structureObject != null) && (structureComponent != null) )
		{
			waypointOffsets = structureComponent.GetWaypointOffsets();
			waypointIDs = structureComponent.GetWaypointIDs();
			if( (waypointOffsets != null) && (waypointIDs != null) )
			{
				length = waypointOffsets.Length;
				if( waypointIDs.Length == length )
				{
					/*halmeida - before creating the waypoints for this node, we save the index at which
					these new waypoints will start. We will need to know where in the array these new
					waypoints begin to be able to correctly associate enemies of a given node to the
					waypoints of the same node.*/
					if( waypoints != null )
					{
						waypointsFirstIndex = waypoints.Length;
					}
					else
					{
						waypointsFirstIndex = 0;
					}
					for( int i=0; i<length; i++ )
					{
						waypointOffset = waypointOffsets[i];
						waypointID = waypointIDs[i];
						waypointPosition.x = structureObject.transform.position.x;
						waypointPosition.y = structureObject.transform.position.y;
						waypointPosition.x += waypointOffset.x;
						waypointPosition.y += waypointOffset.y;
						CreateWaypoint( waypointPosition, waypointID, nodeID );
					}
				}
			}
		}
	}

	public Waypoint CreateWaypoint( Vector2 waypointPosition, int waypointID, int nodeID )
	{
		Waypoint waypoint = null;

		waypoint = new Waypoint( waypointPosition, waypointID );
		UsefulFunctions.IncreaseArray<Waypoint>( ref waypoints, waypoint );
		UsefulFunctions.IncreaseArray<int>( ref waypointNodeIDs, nodeID );
		return waypoint;
	}

	private void RemoveWaypoint( int waypointIndex, bool updateEnemyRoutes )
	{
		Waypoint waypoint = null;
		EnemyAgent enemyComponent = null;

		if( (waypoints != null) && (waypointIndex > -1) )
		{
			if( waypointIndex < waypoints.Length )
			{
				UsefulFunctions.DecreaseArray<int>( ref waypointNodeIDs, waypointIndex );
				waypoint = waypoints[waypointIndex];
				if( waypoint != null )
				{
					waypoint.Clear();
					waypoints[waypointIndex] = null;
				}
				UsefulFunctions.DecreaseArray<Waypoint>( ref waypoints, waypointIndex );
				if( updateEnemyRoutes && (enemyComponents != null) )
				{
					for( int i=0; i<enemyComponents.Length; i++ )
					{
						enemyComponent = enemyComponents[i];
						if( enemyComponent != null )
						{
							enemyComponent.RemoveWaypointIndexFromRoute( waypointIndex );
							enemyComponent.ExtractRoutePositions( waypoints );
						}
					}
				}
			}
		}
	}

	private void ClearWaypoints( int targetNodeID = GraphNode.NODE_ID_INVALID )
	{
		int nodeID = GraphNode.NODE_ID_INVALID;
		int enemyNodeID = GraphNode.NODE_ID_INVALID;
		EnemyAgent enemyComponent = null;

		if( waypoints != null )
		{
			for( int i=0; i<waypoints.Length; i++ )
			{
				/*halmeida - relying on the coherence of all the "waypoint" arrays.*/
				nodeID = waypointNodeIDs[i];
				if( (nodeID == targetNodeID) || (targetNodeID == GraphNode.NODE_ID_INVALID) )
				{
					RemoveWaypoint( i, false );
					if( waypoints == null )
					{
						break;
					}
					else
					{
						i--;
					}
				}
			}
			if( (enemyComponents != null) && (enemyNodeIDs != null) )
			{
				if( enemyComponents.Length == enemyNodeIDs.Length )
				{
					for( int i=0; i<enemyNodeIDs.Length; i++ )
					{
						enemyNodeID = enemyNodeIDs[i];
						if( (enemyNodeID == targetNodeID) || (targetNodeID == GraphNode.NODE_ID_INVALID) )
						{
							enemyComponent = enemyComponents[i];
							if( enemyComponent != null )
							{
								enemyComponent.ClearRoute();
							}
						}
					}
				}
			}
		}
	}

	private void CreateStructureEnemies( int nodeID, GameObject structureObject, ChallengeStructure structureComponent )
	{
		int[] enemyCodes = null;
		int enemyCode = EnemyDatabase.ENEMY_CODE_INVALID;
		Vector2[] enemyOffsets = null;
		RoutePersistentData[] enemyRoutes = null;
		int length = 0;
		GameObject enemyModel = null;
		Vector2 enemyOffset = Vector2.zero;
		Vector2 enemyPosition = Vector2.zero;
		EnemyAgent enemyComponent = null;
		RoutePersistentData enemyRoute = null;
		int[] enemyRouteIndexes = null;

		if( (enemyDatabase != null) && (structureObject != null) && (structureComponent != null) )
		{
			enemyCodes = structureComponent.GetEnemyCodes();
			enemyOffsets = structureComponent.GetEnemyOffsets();
			enemyRoutes = structureComponent.GetEnemyRoutes();
			if( (enemyCodes != null) && (enemyOffsets != null) && (enemyRoutes != null) )
			{
				length = enemyCodes.Length;
				if( (enemyOffsets.Length == length) && (enemyRoutes.Length == length) )
				{
					for( int i=0; i<length; i++ )
					{
						enemyCode = enemyCodes[i];
						//Debug.Log("Debug : ChallengeStage : will try to create enemy with code "+enemyCode+".");
						enemyModel = enemyDatabase.GetEnemyModel( enemyCode );
						if( enemyModel != null )
						{
							enemyPosition.x = structureObject.transform.position.x;
							enemyPosition.y = structureObject.transform.position.y;
							enemyOffset = enemyOffsets[i];
							enemyPosition.x += enemyOffset.x;
							enemyPosition.y += enemyOffset.y;
							enemyComponent = CreateEnemy( enemyModel, enemyPosition, nodeID, false );
							if( enemyComponent != null )
							{
								enemyRoute = enemyRoutes[i];
								if( enemyRoute != null )
								{
									enemyRouteIndexes = enemyRoute.GetWaypointIndexesCopy();
									/*halmeida - the waypoint indexes received are just counted within the structure.
									However, the waypoints of all structures are gathered in a single array in here.
									For the indexes within the structure to be of any use, we have to translate them
									into indexes within this bigger array. To do so, as we add the waypoints of a
									given structure to the big waypoint array, we record the index of its first
									waypoint. Adding the inner structure waypoint index to this recorded index, we
									get the index of the waypoints of the structure within the big array.*/
									if( enemyRouteIndexes != null )
									{
										for( int j=0; j<enemyRouteIndexes.Length; j++ )
										{
											enemyRouteIndexes[j] += waypointsFirstIndex;
										}
										enemyComponent.SetAllRouteWaypointIndexes( enemyRouteIndexes );
										enemyComponent.ExtractRoutePositions( waypoints );
									}
								}
							}
						}
						else
						{
							Debug.Log("Debug : ChallengeStage : enemy model for the code "+enemyCode+" is null.");
						}
					}
					UpdatePlayerAdversaries();
				}
			}
		}
	}

	public EnemyAgent CreateEnemy( GameObject enemyModel, Vector2 enemyPosition, int nodeID, bool updatePlayerAdversaries )
	{
		Vector3 completePosition = Vector3.zero;
		GameObject newEnemy = null;
		EnemyAgent enemyComponent = null;
		PlayerAgent[] currentPlayerComponents = null;

		if( enemyModel != null )
		{
			completePosition.x = enemyPosition.x;
			completePosition.y = enemyPosition.y;
			completePosition.z = 0f;
			newEnemy = Instantiate( enemyModel, completePosition, Quaternion.identity ) as GameObject;
			enemyComponent = newEnemy.GetComponent<EnemyAgent>();
			if( enemyComponent != null )
			{
				newEnemy.transform.SetParent( enemyObjectsParent.transform, false );
				currentPlayerComponents = GetAllPlayerComponents();
				enemyComponent.SetAdversaries( currentPlayerComponents );
				UsefulFunctions.IncreaseArray<GameObject>( ref enemyObjects, newEnemy );
				UsefulFunctions.IncreaseArray<EnemyAgent>( ref enemyComponents, enemyComponent );
				UsefulFunctions.IncreaseArray<int>( ref enemyNodeIDs, nodeID );
				if( updatePlayerAdversaries )
				{
					UpdatePlayerAdversaries();
				}
				enemyComponent.SetStageAndCamera( this, cameraController );
				enemyComponent.SetDamageDisplayFont( fontDamage );
			}
			else
			{
				Destroy( newEnemy );
			}
		}
		return enemyComponent;
	}

	private void RemoveEnemy( int enemyIndex, bool updatePlayerAdversaries )
	{
		GameObject enemyObject = null;
		EnemyAgent enemyComponent = null;

		if( (enemyIndex > -1) && (enemyObjects != null) )
		{
			if( enemyIndex < enemyObjects.Length )
			{
				/*halmeida - relying on the coherence of all the "enemy" arrays.*/
				UsefulFunctions.DecreaseArray<int>( ref enemyNodeIDs, enemyIndex );
				enemyComponent = enemyComponents[enemyIndex];
				if( enemyComponent != null )
				{
					enemyComponent.Clear();
					enemyComponents[enemyIndex] = null;
				}
				UsefulFunctions.DecreaseArray<EnemyAgent>( ref enemyComponents, enemyIndex );
				enemyObject = enemyObjects[enemyIndex];
				if( enemyObject != null )
				{
					Destroy( enemyObject );
					enemyObjects[enemyIndex] = null;
				}
				UsefulFunctions.DecreaseArray<GameObject>( ref enemyObjects, enemyIndex );
				if( updatePlayerAdversaries )
				{
					UpdatePlayerAdversaries();
				}
			}
		}
	}

	private void ClearEnemies( int targetNodeID = GraphNode.NODE_ID_INVALID )
	{
		int nodeID = GraphNode.NODE_ID_INVALID;

		if( enemyObjects != null )
		{
			for( int i=0; i<enemyObjects.Length; i++ )
			{
				/*halmeida - relying on the coherence of all the "enemy" arrays.*/
				nodeID = enemyNodeIDs[i];
				if( (nodeID == targetNodeID) || (targetNodeID == GraphNode.NODE_ID_INVALID) )
				{
					RemoveEnemy( i, false );
					if( enemyObjects == null )
					{
						break;
					}
					else
					{
						i--;
					}
				}
			}
			UpdatePlayerAdversaries();
		}
	}

	public bool GetNodeStructurePosition( int nodeID, ref Vector2 nodePosition )
	{
		GraphNode nextNode = null;
		GameObject nextStructObject = null;

		if( (currentNode != null) && (currentStructObject != null) )
		{
			if( currentNode.ID == nodeID )
			{
				nodePosition.x = currentStructObject.transform.localPosition.x;
				nodePosition.y = currentStructObject.transform.localPosition.y;
				return true;
			}
		}
		if( (nextNodes != null) && (nextStructObjects != null) )
		{
			for( int i=0; i<nextNodes.Length; i++ )
			{
				nextNode = nextNodes[i];
				if( nextNode != null )
				{
					if( nextNode.ID == nodeID )
					{
						/*halmeida - relying on the coherence of all the "next" arrays.*/
						nextStructObject = nextStructObjects[i];
						if( nextStructObject != null )
						{
							nodePosition.x = nextStructObject.transform.localPosition.x;
							nodePosition.y = nextStructObject.transform.localPosition.y;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public PlayerAgent CreateNonPlayableCharacter( GameObject npcModel, int nodeID, Vector2 offsetWithinNode, bool sideToFace, bool isAdversary )
	{
		Vector2 npcPosition = Vector3.zero;
		GameObject npcObject = null;
		PlayerAgent npcComponent = null;
		PlayerAgent[] currentPlayerComponents = null;

		if( (npcModel != null) && GetNodeStructurePosition( nodeID, ref npcPosition ) )
		{
			npcObject = Instantiate( npcModel, Vector3.zero, Quaternion.identity ) as GameObject;
			npcComponent = npcObject.GetComponent<PlayerAgent>();
			if( npcComponent != null )
			{
				/*halmeida - To get the npc insertion position, we add the offset to the position
				of the structure of the node.*/
				npcObject.transform.SetParent( npcObjectsParent.transform, false );
				npcPosition += offsetWithinNode;
				npcObject.transform.localPosition = new Vector3( npcPosition.x, npcPosition.y, 0f );
				/*halmeida - since we changed the npc's depth, we most likely messed up
				the displaying order of some higher display priority elements. This happens
				because the equipments have smaller depths than the npc himself, so that
				they may be shown on top of him. Given enough equipments, they might invade
				the depth stripe of other smaller depth elements that show with higher
				priority than the npc. To keep the equipments within the npc's depth
				stripe, the PlayerAgent has a depth correcting function.*/
				npcComponent.CorrectDepthChain( true );
				npcComponent.PrepareForRetry( sideToFace );
				UsefulFunctions.IncreaseArray<GameObject>( ref npcObjects, npcObject );
				UsefulFunctions.IncreaseArray<PlayerAgent>( ref npcComponents, npcComponent );
				UsefulFunctions.IncreaseArray<bool>( ref npcEnmity, isAdversary );
				if( isAdversary )
				{
					currentPlayerComponents = GetAllPlayerComponents();
					npcComponent.SetAdversaries( currentPlayerComponents );
					UpdatePlayerAdversaries();
				}
				return npcComponent;
			}
			Destroy( npcObject );
		}
		return null;
	}

	public bool RemoveNonPlayableCharacter( int agentID )
	{
		PlayerAgent npcComponent = null;
		GameObject npcObject = null;
		bool wasAdversary = false;

		if( npcComponents != null )
		{
			for( int i=0; i<npcComponents.Length; i++ )
			{
				npcComponent = npcComponents[i];
				if( npcComponent != null )
				{
					if( npcComponent.agentID == agentID )
					{
						/*halmeida - relying on the coherence of all the "npc" arrays.*/
						wasAdversary = npcEnmity[i];
						npcComponent.Clear();
						npcComponents[i] = null;
						npcObject = npcObjects[i];
						if( npcObject != null )
						{
							Destroy( npcObject );
							npcObjects[i] = null;
						}
						UsefulFunctions.DecreaseArray<bool>( ref npcEnmity, i );
						UsefulFunctions.DecreaseArray<PlayerAgent>( ref npcComponents, i );
						UsefulFunctions.DecreaseArray<GameObject>( ref npcObjects, i );
						if( wasAdversary )
						{
							UpdatePlayerAdversaries();
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	private void ClearNonPlayableCharacters()
	{
		PlayerAgent npcComponent = null;
		GameObject npcObject = null;
		bool oneAdversary = false;

		if( npcComponents != null )
		{
			for( int i=0; i<npcComponents.Length; i++ )
			{
				/*halmeida - relying on the coherence of all the "npc" arrays.*/
				if( !oneAdversary )
				{
					oneAdversary = npcEnmity[i];
				}
				npcComponent = npcComponents[i];
				if( npcComponent != null )
				{
					npcComponent.Clear();
					npcComponents[i] = null;
				}
				npcObject = npcObjects[i];
				if( npcObject != null )
				{
					Destroy( npcObject );
					npcObjects[i] = null;
				}
			}
			npcEnmity = null;
			npcComponents = null;
			npcObjects = null;
			if( oneAdversary )
			{
				UpdatePlayerAdversaries();
			}
		}
	}

	private void ProgressItems( float timeStep )
	{
		ItemController itemController = null;

		if( itemControllers != null )
		{
			for( int i=0; i<itemControllers.Length; i++ )
			{
				itemController = itemControllers[i];
				if( itemController != null )
				{
					itemController.Progress( timeStep );
					if( itemController.IsOver() )
					{
						RemoveItem( i );
						if( itemControllers == null )
						{
							break;
						}
						else
						{
							i--;
						}
					}
				}
			}
		}
		if( eventItemControllers != null )
		{
			for( int i=0; i<eventItemControllers.Length; i++ )
			{
				itemController = eventItemControllers[i];
				if( itemController != null )
				{
					itemController.Progress( timeStep );
					/*halmeida - event created items are not automatically removed. If an event
					creates an item, it has to remove it.*/
				}
			}
		}
		if( debugItemController != null )
		{
			debugItemController.Progress( timeStep );
			if( debugItemController.IsOver() )
			{
				ClearDebugItem();
			}
		}
	}

	private void ProgressItemClusters( float timeStep )
	{
		ItemCluster itemCluster = null;

		if( itemClusters != null )
		{
			for( int i=0; i<itemClusters.Length; i++ )
			{
				itemCluster = itemClusters[i];
				if( itemCluster != null )
				{
					itemCluster.Progress( timeStep );
				}
			}
		}
	}

	private void ProgressObstacles( float timeStep )
	{
		ObstacleController obstacleController = null;

		if( eventObstacleControllers != null )
		{
			for( int i=0; i<eventObstacleControllers.Length; i++ )
			{
				obstacleController = eventObstacleControllers[i];
				if( obstacleController != null )
				{
					obstacleController.Progress( timeStep );
					/*halmeida - event created obstacles are not automatically removed. If an event
					creates an obstacle, it has to remove it.*/
				}
			}
		}
		if( debugObstacleController != null )
		{
			debugObstacleController.Progress( timeStep );
			if( debugObstacleController.IsOver() )
			{
				ClearDebugObstacle();
			}
		}
	}

	private void ProgressEnemies( float timeStep )
	{
		EnemyAgent enemyComponent = null;

		if( enemyComponents != null )
		{
			for( int i=0; i<enemyComponents.Length; i++ )
			{
				enemyComponent = enemyComponents[i];
				if( enemyComponent != null )
				{
					enemyComponent.Progress( timeStep );
					if( enemyComponent.IsOver() )
					{
						RemoveEnemy( i, true );
						if( enemyComponents == null )
						{
							break;
						}
						else
						{
							i--;
						}
					}
				}
			}
		}
	}

	private void ProgressNpcs( float timeStep )
	{
		PlayerAgent npcComponent = null;

		if( npcComponents != null )
		{
			for( int i=0; i<npcComponents.Length; i++ )
			{
				npcComponent = npcComponents[i];
				if( npcComponent != null )
				{
					npcComponent.Progress( timeStep );
				}
			}
		}
	}

	private void ProgressRoomDarkener( float timeStep )
	{
		Color color = Color.black;
		float alpha = 0f;
		Vector3 cameraPosition = Vector3.zero;

		if( roomDarkenerObject != null )
		{
			if( roomUnlocked )
			{
				if( !roomDarkenerClear )
				{
					color = roomDarkenerRenderer.color;
					alpha = color.a;
					alpha -= timeStep * ROOM_DARKENER_SPEED_FACTOR;
					roomDarkenerDark = false;
					if( alpha < 0 )
					{
						alpha = 0f;
						roomDarkenerClear = true;
					}
					color.a = alpha;
					roomDarkenerRenderer.color = color;
				}
			}
			else
			{
				if( !roomDarkenerDark )
				{
					color = roomDarkenerRenderer.color;
					alpha = color.a;
					alpha += timeStep * ROOM_DARKENER_SPEED_FACTOR;
					roomDarkenerClear = false;
					if( alpha > ROOM_DARKENER_ALPHA_LIMIT )
					{
						alpha = ROOM_DARKENER_ALPHA_LIMIT;
						roomDarkenerDark = true;
					}
					color.a = alpha;
					roomDarkenerRenderer.color = color;
				}
			}
		}
	}

	private bool SetCurrentNode( int newNodeID, int mapFirstRow = -1, int mapFirstColumn = -1 )
	{
		GraphNode tempNode = null;
		GameObject tempStructObject = null;
		ChallengeStructure tempStructComponent = null;
		Rect tempNodeMapSlots = new Rect( -1f, -1f, 0f, 0f );
		bool structLoaded = false;
		Vector3 currentStructOffset = Vector3.zero;
		float currentStructWidth = 0f;
		float currentStructHeight = 0f;
		int firstSlotRow = -1;
		int firstSlotColumn = -1;

		if( buildingNextNodes )
		{
			/*halmeida - we are still building the next nodes for the node that was last designated as current.*/
			return false;
		}
		if( currentNode != null )
		{
			if( currentNode.ID == newNodeID )
			{
				return true;
			}
		}
		/*halmeida - this function should be used when the player transitions from one node to another.
		To be more efficient, it looks at the already loaded structures and tries to use any of them.*/
		if( (previousStructComponent != null) && (previousNode != null) )
		{
			if( previousNode.ID == newNodeID )
			{
				tempNode = currentNode;
				tempStructObject = currentStructObject;
				tempStructComponent = currentStructComponent;
				tempNodeMapSlots = currentNodeMapSlots;
				currentNode = previousNode;
				currentStructObject = previousStructObject;
				currentStructComponent = previousStructComponent;
				currentNodeMapSlots = previousNodeMapSlots;
				previousNode = tempNode;
				previousStructObject = tempStructObject;
				previousStructComponent = tempStructComponent;
				previousNodeMapSlots = tempNodeMapSlots;
				/*halmeida - no need to worry about the doors. All the next arrays will be created again.*/
				structLoaded = true;
			}
		}
		if( !structLoaded && (nextStructComponents != null) && (nextNodes != null) )
		{
			for( int i=0; i<nextStructComponents.Length; i++ )
			{
				tempStructComponent = nextStructComponents[i];
				if( tempStructComponent != null )
				{
					/*halmeida - relying on the coherence of all the "next" arrays.*/
					tempNode = nextNodes[i];
					if( tempNode != null )
					{
						if( tempNode.ID == newNodeID )
						{
							/*halmeida - one of the nodes loaded as next and not the previous is the target node.*/
							previousNode = currentNode;
							previousStructObject = currentStructObject;
							previousStructComponent = currentStructComponent;
							previousNodeMapSlots = currentNodeMapSlots;
							currentNode = tempNode;
							currentStructObject = nextStructObjects[i];
							currentStructComponent = tempStructComponent;
							currentNodeMapSlots = nextNodeMapSlots[i];
							structLoaded = true;
							break;
						}
					}
				}
			}
		}
		if( !structLoaded )
		{
			/*halmeida - the node we have to load wasn't the past one and wasn't already loaded as one of the next nodes.*/
			/*halmeida - since it also isn't the current node, we have to clear the current node, in case it exists.*/
			if( currentStructComponent != null )
			{
				currentNodeMapSlots = new Rect( -1f, -1f, 0f, 0f );
				if( roomBlocker != null )
				{
					roomBlocker.ClearLoadedNode( currentNode.ID );
				}
				ClearItems( currentNode.ID );
				ClearDebugObstacle();
				ClearEnemies( currentNode.ID );
				ClearWaypoints( currentNode.ID );
				currentStructComponent.Clear();
				currentStructComponent = null;
				Destroy( currentStructObject );
				currentStructObject = null;
				currentNode = null;
			}
			/*halmeida - since we are loading a node that is unrelated to all currently loaded nodes, the
			previous node becomes invalid and the next ones too. The next ones will be normally cleared so
			that the new next ones, appropriate to this new current node, can be loaded. The previous will
			be pointing to garbage when that happens, so we have to point it away.*/
			previousNode = null;
			previousStructComponent = null;
			previousStructObject = null;
			previousNodeMapSlots = new Rect( -1f, -1f, 0f, 0f );

			tempNode = null;
			tempStructObject = null;
			tempStructComponent = null;
			LoadStructureForNode( newNodeID, ref tempNode, ref tempStructObject, ref tempStructComponent, false );
			if( tempStructComponent != null )
			{
				currentNode = tempNode;
				currentStructObject = tempStructObject;
				currentStructComponent = tempStructComponent;
				currentStructObject.transform.SetParent( structObjectsParent.transform, false );

				/*halmeida - where in the world should this new structure be positioned? If the method
				received outside map coordinates, we use them. If it didn't, we try to see if the node
				already had map coordinates saved within itself. One of these two cases should be true.*/
				if( (mapFirstRow > -1) && (mapFirstColumn > -1) )
				{
					firstSlotRow = mapFirstRow;
					firstSlotColumn = mapFirstColumn;
				}
				else
				{
					firstSlotRow = currentNode.GetFirstSlotRow();
					firstSlotColumn = currentNode.GetFirstSlotColumn();
					if( (firstSlotRow < 0) || (firstSlotColumn < 0) )
					{
						firstSlotRow = 0;
						firstSlotColumn = 0;
					}
				}
				currentStructOffset.x = firstSlotColumn * roomSlotWidth;
				currentStructOffset.y = firstSlotRow * -roomSlotHeight;
				currentStructObject.transform.localPosition = currentStructOffset;
				currentStructComponent.GetStructureWorldDimensions( ref currentStructWidth, ref currentStructHeight );
				currentStructRect = new Rect( currentStructOffset.x, currentStructOffset.y, currentStructWidth, currentStructHeight );
				/*halmeida - we still gotta obtain the width and the height of the structure in map slots, for
				we already have the first map slot coordinates for the structure.*/
				currentNodeMapSlots.x = firstSlotColumn;
				currentNodeMapSlots.y = firstSlotRow;
				GetMapSlotCorrespondence( currentStructRect, ref currentNodeMapSlots );
				if( roomBlocker != null )
				{
					roomBlocker.LoadNodeIntoSlots( currentNode.ID, (int)currentNodeMapSlots.y, (int)currentNodeMapSlots.x,
						(int)currentNodeMapSlots.width, (int)currentNodeMapSlots.height );
				}

				CreateStructureWaypoints( currentNode.ID, currentStructObject, currentStructComponent );
				CreateStructureEnemies( currentNode.ID, currentStructObject, currentStructComponent );
				if( currentNode.loadItems && (graph != null) )
				{
					if( graph.IsAreaUnlocked( currentNode.areaLockIndex ) )
					{
						CreateStructureItems( currentNode.ID, currentStructObject, currentStructComponent );
						CreateStructureItemClusters( currentNode.ID, currentStructComponent );
					}
				}
				structLoaded = true;
			}
		}
		if( structLoaded )
		{
			currentStructOffset = currentStructObject.transform.localPosition;
			currentStructComponent.GetStructureWorldDimensions( ref currentStructWidth, ref currentStructHeight );
			currentStructRect = new Rect( currentStructOffset.x, currentStructOffset.y, currentStructWidth, currentStructHeight );
			currentStructCenter = new Vector2( currentStructRect.x + currentStructRect.width / 2f,
					currentStructRect.y - currentStructRect.height / 2f );
			/*halmeida - we have moved into a different node, so we have to visit it. Even if it
			was visited before it should be visited again because visiting it sets its map slot
			coordinates and setting them again may allow us to correct old outdated map coordinates
			saved into the node.*/
			if( graph != null )
			{
				graph.SetNodeVisitation( currentNode.ID, true, (int)currentNodeMapSlots.y, (int)currentNodeMapSlots.x,
					(int)currentNodeMapSlots.width, (int)currentNodeMapSlots.height );
				graph.SetCurrentNode( currentNode.ID );
				roomUnlocked = graph.IsAreaUnlocked( currentNode.areaLockIndex );
			}
			ClearNextArrays( true );
			ExtractDrainSpeed();
			if( backgroundController != null )
			{
				backgroundController.SetBackground( currentNode.backgroundID );
				if( currentNode.centersBackground )
				{
					backgroundController.SetBackgroundOriginPoint( currentStructCenter );
				}
			}
			/*halmeida - I gotta create the next arrays again, for the new current node. However, since the
			clearing of the next arrays may have already had a high time cost, I will leave the reconstruction
			of the next arrays to be started at the next frame.*/
			//Debug.Log("Debug : ChallengeStage : cleared next nodes and scheduled reconstruction.");
			buildingNextNodes = true;
			return true;
		}
		return false;
	}

	private void ResetNodes( bool current, bool previous, bool next )
	{
		GraphNode nextNode = null;
		GameObject nextStructObject = null;
		ChallengeStructure nextStructComponent = null;
		Rect nextStructMapSlots = new Rect( -1f, -1f, 0f, 0f );
		bool skipNode = false;

		Debug.Log("Debug : ChallengeStage : resetting nodes...");
		if( current && (currentNode != null) )
		{
			Debug.Log("Debug : ChallengeStage : resetting current node.");
			ClearItems( currentNode.ID );
			ClearDebugObstacle();
			ClearEnemies( currentNode.ID );
			/*halmeida - in theory, we could leave the waypoints untouched. However, the enemy association to waypoints
			only works if the waypoints of the node to which the enemies belong have just been created. So we have
			to clear and recreate the waypoints so that the enemy creation, afterwards, can make the correct associations.*/
			ClearWaypoints( currentNode.ID );
			CreateStructureWaypoints( currentNode.ID, currentStructObject, currentStructComponent );
			CreateStructureEnemies( currentNode.ID, currentStructObject, currentStructComponent );
			if( currentNode.loadItems && (graph != null) )
			{
				if( graph.IsAreaUnlocked( currentNode.areaLockIndex ) )
				{
					CreateStructureItems( currentNode.ID, currentStructObject, currentStructComponent );
					CreateStructureItemClusters( currentNode.ID, currentStructComponent );
				}
			}
			if( roomBlocker != null )
			{
				roomBlocker.LoadNodeIntoSlots( currentNode.ID, (int)currentNodeMapSlots.y, (int)currentNodeMapSlots.x,
					(int)currentNodeMapSlots.width, (int)currentNodeMapSlots.height );
			}
		}
		if( previous && (previousNode != null) )
		{
			Debug.Log("Debug : ChallengeStage : resetting previous node.");
			ClearItems( previousNode.ID );
			ClearDebugObstacle();
			ClearEnemies( previousNode.ID );
			ClearWaypoints( previousNode.ID );
			CreateStructureWaypoints( previousNode.ID, previousStructObject, previousStructComponent );
			CreateStructureEnemies( previousNode.ID, previousStructObject, previousStructComponent );
			if( previousNode.loadItems && (graph != null) )
			{
				if( graph.IsAreaUnlocked( previousNode.areaLockIndex ) )
				{
					CreateStructureItems( previousNode.ID, previousStructObject, previousStructComponent );
					CreateStructureItemClusters( previousNode.ID, previousStructComponent );
				}
			}
			if( roomBlocker != null )
			{
				roomBlocker.LoadNodeIntoSlots( previousNode.ID, (int)previousNodeMapSlots.y, (int)previousNodeMapSlots.x,
					(int)previousNodeMapSlots.width, (int)previousNodeMapSlots.height );
			}
		}
		if( next && (nextNodes != null) )
		{
			Debug.Log("Debug : ChallengeStage : resetting next nodes.");
			for( int i=0; i<nextNodes.Length; i++ )
			{
				nextNode = nextNodes[i];
				if( nextNode != null )
				{
					/*halmeida - the previous node will be one of the next nodes. If there's no intention to reset
					the previous node, the next node that corresponds to it has to be skipped. If there is the intention
					to reset the previous node, then it has already been reset above and should be skipped now. Either
					way, it should be skipped now.*/
					skipNode = false;
					if( previousNode != null )
					{
						if( previousNode.ID == nextNode.ID )
						{
							skipNode = true;
						}
					}
					if( !skipNode )
					{
						/*halmeida - relying on the coherence of all the "next" arrays.*/
						nextStructObject = nextStructObjects[i];
						nextStructComponent = nextStructComponents[i];
						nextStructMapSlots = nextNodeMapSlots[i];
						ClearItems( nextNode.ID );
						ClearDebugObstacle();
						ClearEnemies( nextNode.ID );
						ClearWaypoints( nextNode.ID );
						CreateStructureWaypoints( nextNode.ID, nextStructObject, nextStructComponent );
						CreateStructureEnemies( nextNode.ID, nextStructObject, nextStructComponent );
						if( nextNode.loadItems && (graph != null) )
						{
							if( graph.IsAreaUnlocked( nextNode.areaLockIndex ) )
							{
								CreateStructureItems( nextNode.ID, nextStructObject, nextStructComponent );
								CreateStructureItemClusters( nextNode.ID, nextStructComponent );
							}
						}
						if( roomBlocker != null )
						{
							roomBlocker.LoadNodeIntoSlots( nextNode.ID, (int)nextStructMapSlots.y, (int)nextStructMapSlots.x,
								(int)nextStructMapSlots.width, (int)nextStructMapSlots.height );
						}
					}
				}
			}
		}
	}

	private void LoadStructureForNode( int nodeID, ref GraphNode node, ref GameObject structObject, ref ChallengeStructure structComponent,
		bool buildGradually )
	{
		int tileFactoryIDFront = TileFactory.UNDEFINED_FACTORY_ID;
		int tileFactoryIDBack = TileFactory.UNDEFINED_FACTORY_ID;
		TileFactory tileFactoryFront = null;
		TileFactory tileFactoryBack = null;
		int structID = ChallengeStructure.STRUCTURE_ID_INVALID;
		StructurePersistentData singleStructData = null;
		bool loaded = false;
		bool loadFailed = false;
		TileFactory tempTileFactory = null;

		if( structComponent == null )
		{
			if( (tileFactories != null) && (structData != null) && (graph != null) && (structObject == null) )
			{
				if( (node == null) && (graph != null) )
				{
					node = graph.GetNodeWithID( nodeID );
				}
				if( node != null )
				{
					/*halmeida - correcting the ID in case the function received already a valid node and an invalid ID.*/
					nodeID = node.ID;
					tileFactoryIDFront = node.tileFactoryIDFront;
					tileFactoryIDBack = node.tileFactoryIDBack;
					if( tileFactoryIDFront != TileFactory.UNDEFINED_FACTORY_ID )
					{
						for( int i=0; i<tileFactories.Length; i++ )
						{
							tempTileFactory = tileFactories[i];
							if( tempTileFactory != null )
							{
								if( tempTileFactory.factoryID == tileFactoryIDFront )
								{
									tileFactoryFront = tempTileFactory;
									if( tileFactoryBack != null )
									{
										break;
									}
								}
								else if( tempTileFactory.factoryID == tileFactoryIDBack )
								{
									tileFactoryBack = tempTileFactory;
									if( tileFactoryFront != null )
									{
										break;
									}
								}
							}
						}
					}
				}
				if( tileFactoryFront != null )
				{
					structID = node.structureID;
					if( structID != ChallengeStructure.STRUCTURE_ID_INVALID )
					{
						for( int i=0; i<structData.Length; i++ )
						{
							singleStructData = structData[i];
							if( singleStructData != null )
							{
								if( singleStructData.structureID == structID )
								{
									break;
								}
								else
								{
									singleStructData = null;
								}
							}
						}
					}
				}
				if( singleStructData != null )
				{
					structObject = new GameObject( "Struct"+structID+"Node"+nodeID );
					structComponent = structObject.AddComponent<ChallengeStructure>();
					structComponent.SetTileFactories( tileFactoryFront, tileFactoryBack );
					loaded = structComponent.LoadFromPersistentData( singleStructData, false, buildGradually );
					if( !loaded )
					{
						if( buildGradually )
						{
							if( !structComponent.BuildingTiles() )
							{
								loadFailed = true;
							}
						}
						else
						{
							loadFailed = true;
						}
					}
				}
			}
		}
		else
		{
			/*halmeida - if this function already received a structure component, we can only be continuing the
			gradual build process of that structure. If we receive a structure component and are not building it
			gradually, than it is considered a load failure.*/  
			if( buildGradually )
			{
				loaded = structComponent.LoadFromPersistentData( null, false, true );
				if( !loaded && !structComponent.BuildingTiles() )
				{
					loadFailed = true;
				}
			}
			else
			{
				loadFailed = true;
			}
		}
		if( loadFailed )
		{
			structComponent.Clear();
			structComponent = null;
			Destroy( structObject );
			structObject = null;
		}
	}

	private bool GetMapSlotCorrespondence( Rect worldArea, ref Rect mapSlotArea )
	{
		float offset = 0f;

		if( (roomSlotWidth <= 0f) || (roomSlotHeight <= 0f) )
		{
			return false;
		}
		if( (mapSlotArea.x < 0f) || (mapSlotArea.y < 0f) )
		{
			/*halmeida - means we have to get the first slot coordinates of the area. This method is called
			this way when we are building the next nodes to a certain node, and we use the current node data
			as reference.*/
			offset = worldArea.x - currentStructRect.x;
			offset = Mathf.Round( offset / roomSlotWidth );
			mapSlotArea.x = currentNodeMapSlots.x + offset;
			offset = worldArea.y - currentStructRect.y;
			offset = Mathf.Round( offset / roomSlotHeight );
			mapSlotArea.y = currentNodeMapSlots.y - offset;
		}
		mapSlotArea.width = Mathf.Round( worldArea.width / roomSlotWidth );
		mapSlotArea.height = Mathf.Round( worldArea.height / roomSlotHeight );
		return true;
	}

	private void ClearNextArrays( bool spareCurrent )
	{
		ChallengeStructure structComponent = null;
		GameObject structObject = null;
		bool spareNode = false;
		GraphNode nextNode = null;
		GraphNode newNode = null;

		if( nextStructComponents != null )
		{
			for( int i=0; i<nextStructComponents.Length; i++ )
			{
				structComponent = nextStructComponents[i];
				if( structComponent != null )
				{
					/*halmeida - if this function is called when one of the next nodes has already become the current
					node, we have to spare that node when clearing them.*/
					/*halmeida - relying on the coherence of all the "next" arrays.*/
					nextNode = nextNodes[i];
					spareNode = false;
					if( spareCurrent && (currentNode != null) )
					{
						if( nextNode != null )
						{
							spareNode = (currentNode.ID == nextNode.ID);
						}
					}
					if( !spareNode )
					{
						if( nextNode != null )
						{
							if( roomBlocker != null )
							{
								roomBlocker.ClearLoadedNode( nextNode.ID );
							}
							ClearItems( nextNode.ID );
							ClearDebugObstacle();
							ClearEnemies( nextNode.ID );
							ClearWaypoints( nextNode.ID );
						}
						structComponent.Clear();
						structObject = nextStructObjects[i];
						if( structObject != null )
						{
							Destroy( structObject );
						}
					}
					nextStructComponents[i] = null;
					nextStructObjects[i] = null;
					nextNodes[i] = null;
				}
			}
			nextStructComponents = null;
			nextStructObjects = null;
			nextNodes = null;
			nextNodeMapSlots = null;
			nextDoors = null;
			nextDoorDirections = null;
		}
		/*halmeida - the new arrays are used to gradually build the next arrays. They should also be cleared.*/
		if( newStructComponentGradual != null )
		{
			newWorldDoorGradual = new Rect( 0f, 0f, 0f, 0f );
			/*halmeida - no need to clear the items related to the gradual node because when the node's items
			are created, the gradual node becomes a new node and we will clear the items of all new nodes.*/
			newNodeGradual = null;
			if( newStructComponentGradual != null )
			{
				newStructComponentGradual.Clear();
				newStructComponentGradual = null;
			}
			if( newStructObjectGradual != null )
			{
				Destroy( newStructObjectGradual );
				newStructObjectGradual = null;
			}
		}
		if( newStructComponents != null )
		{
			for( int i=0; i<newStructComponents.Length; i++ )
			{
				structComponent = newStructComponents[i];
				if( structComponent != null )
				{
					/*halmeida - relying on the coherence of all the "new" arrays.*/
					newNode = newNodes[i];
					if( newNode != null )
					{
						if( roomBlocker != null )
						{
							roomBlocker.ClearLoadedNode( newNode.ID );
						}
						ClearItems( newNode.ID );
						ClearDebugObstacle();
						ClearEnemies( newNode.ID );
						ClearWaypoints( newNode.ID );
					}
					structComponent.Clear();
					newStructComponents[i] = null;
					structObject = newStructObjects[i];
					if( structObject != null )
					{
						Destroy( structObject );
					}
					newStructObjects[i] = null;
					newNodes[i] = null;
				}
			}
			newStructComponents = null;
			newStructObjects = null;
			newNodes = null;
			newNodeMapSlots = null;
			newLocalDoors = null;
			newWorldDoors = null;
			newWorldDoorDirections = null;
			newElements = 0;
			newElementsDirection = DIRECTION_INVALID;
			newElementIndex = -1;
		}
		buildingNextNodes = false;
	}

	private void CreateNextArrays()
	{
		Rect doorToCurrentStruct = default( Rect );
		Vector3 newStructPosition = Vector3.zero;
		float newStructWidth = 0f;
		float newStructHeight = 0f;
		Rect newStructRect = new Rect( 0f, 0f, 0f, 0f );
		Rect newStructMapSlots = new Rect( -1f, -1f, 0f, 0f );

		/*halmeida - at each call of the function, we will build a piece of one of the next node structures.*/
		if( (nextStructComponents == null) && (newElementsDirection == DIRECTION_INVALID) && (currentStructComponent != null) )
		{
			/*halmeida - This is the first call of a cycle of calls.*/
			previousNodeFound = false;
			AdvanceNewElementsDirection();
			if( newElementsDirection == DIRECTION_INVALID )
			{
				/*halmeida - this should not happen because every node has to be connected to at least one
				other node, or it can never be reached.*/
				//Debug.Log("Debug : ChallengeStage : next arrays building failed.");
				buildingNextNodes = false;
				return;
			}
		}
		if( newElementIndex > -1 )
		{
			/*halmeida - if we still aren't building a node, we start to build one.*/
			if( newStructComponentGradual == null )
			{
				//Debug.Log("Debug : ChallengeStage : considering the construction of a next node.");
				finishNodeConstruction = false;
				advanceToNextNextNode = false;
				newWorldDoorGradual = newLocalDoors[newElementIndex];
				newWorldDoorGradual.x += currentStructRect.x;
				newWorldDoorGradual.y += currentStructRect.y;
				newNodeGradual = newNodes[newElementIndex];
				/*halmeida - when the previous node exists, it is necessarily one of the next nodes.
				So, when loading the next nodes, I should check to skip the previous node, which is
				already loaded.*/
				if( !previousNodeFound )
				{
					if( (newNodeGradual != null) && (previousNode != null) )
					{
						if( newNodeGradual.ID == previousNode.ID )
						{
							previousNodeFound = true;
							newStructObjectGradual = previousStructObject;
							newStructComponentGradual = previousStructComponent;
							newStructMapSlots = previousNodeMapSlots;
							if( newStructComponentGradual != null )
							{
								/*halmeida - this means the previous node's structure has been built previously and
								nothing should be done to it, leaving its state as it is, with items collected etc.*/
								advanceToNextNextNode = true;
							}
							//Debug.Log("Debug : ChallengeStage : was the previous node, no construction necessary.");
						}
					}
				}
				if( newStructComponentGradual == null )
				{
					/*halmeida - this means this node is not the previous node or the previous, for some reason, wasn't properly
					loaded.*/
					LoadStructureForNode( GraphNode.NODE_ID_INVALID, ref newNodeGradual, ref newStructObjectGradual,
						ref newStructComponentGradual, true );
					//Debug.Log("Debug : ChallengeStage : started building a next node.");
				}
			}
			else
			{
				/*halmeida - if we were already building a node, we continue building it.*/
				LoadStructureForNode( GraphNode.NODE_ID_INVALID, ref newNodeGradual, ref newStructObjectGradual,
					ref newStructComponentGradual, true );
				//Debug.Log("Debug : ChallengeStage : continued building a next node.");
			}

			if( !finishNodeConstruction )
			{
				if( !advanceToNextNextNode )
				{
					/*halmeida - means we did not use the previous node. So we either started or continued building one.*/ 
					if( newStructComponentGradual == null )
					{
						/*halmeida - the construction of the node failed.*/
						advanceToNextNextNode = true;
						//Debug.Log("Debug : ChallengeStage : node construction failed.");
					}
					else
					{
						/*halmeida - the construction has started. If the structure is still building tiles, the
						construction is going well. If it's no longer building tiles, the construction has ended
						well and, next time this function is called, we will finish the node by creating its
						other elements aside from tiles.*/
						if( !newStructComponentGradual.BuildingTiles() )
						{
							finishNodeConstruction = true;
							//Debug.Log("Debug : ChallengeStage : node construction ended correctly.");
						}
					}
				}
			}
			else
			{
				/*halmeida - all the tiles of the structure have been built, so we can create its items etc.*/
				newStructObjectGradual.transform.SetParent( structObjectsParent.transform, false );
				newStructComponentGradual.GetStructureWorldDimensions( ref newStructWidth, ref newStructHeight );
				/*halmeida - I have one of the next nodes ready, but where should I place it?
				I need to place the correct doors together.*/
				switch( newElementsDirection )
				{
					case DIRECTION_UP:
						doorToCurrentStruct = GetLocalDoorToID( newNodeGradual, newStructComponentGradual, DIRECTION_DOWN, currentNode.ID );
						if( doorToCurrentStruct.width != 0f )
						{
							newStructPosition.x = newWorldDoorGradual.x - doorToCurrentStruct.x;
							newStructPosition.y = newWorldDoorGradual.y + newStructHeight;
							newStructPosition.z = 0f;
							newStructObjectGradual.transform.localPosition = newStructPosition;
						}
						break;
					case DIRECTION_LEFT:
						doorToCurrentStruct = GetLocalDoorToID( newNodeGradual, newStructComponentGradual, DIRECTION_RIGHT, currentNode.ID );
						if( doorToCurrentStruct.height != 0f )
						{
							newStructPosition.x = newWorldDoorGradual.x - newStructWidth;
							/*halmeida - the y component of a door has the highest y value of the door, cause the
							gap rows within the structure are counted top to bottom, and the gap rows are used
							when calculating the doors.*/
							newStructPosition.y = newWorldDoorGradual.y - doorToCurrentStruct.y;
							newStructPosition.z = 0f;
							newStructObjectGradual.transform.localPosition = newStructPosition;
						}
						break;
					case DIRECTION_DOWN:
						doorToCurrentStruct = GetLocalDoorToID( newNodeGradual, newStructComponentGradual, DIRECTION_UP, currentNode.ID );
						if( doorToCurrentStruct.width != 0f )
						{
							newStructPosition.x = newWorldDoorGradual.x - doorToCurrentStruct.x;
							newStructPosition.y = newWorldDoorGradual.y;
							newStructPosition.z = 0f;
							newStructObjectGradual.transform.localPosition = newStructPosition;
						}
						break;
					case DIRECTION_RIGHT:
						doorToCurrentStruct = GetLocalDoorToID( newNodeGradual, newStructComponentGradual, DIRECTION_LEFT, currentNode.ID );
						if( doorToCurrentStruct.height != 0f )
						{
							newStructPosition.x = newWorldDoorGradual.x;
							newStructPosition.y = newWorldDoorGradual.y - doorToCurrentStruct.y;
							newStructPosition.z = 0f;
							newStructObjectGradual.transform.localPosition = newStructPosition;
						}
						break;
				}
				newStructRect = new Rect( newStructPosition.x, newStructPosition.y, newStructWidth, newStructHeight );
				if( GetMapSlotCorrespondence( newStructRect, ref newStructMapSlots ) )
				{
					SnapToMapGrid( newStructObjectGradual, newStructMapSlots );
				}
				CreateStructureWaypoints( newNodeGradual.ID, newStructObjectGradual, newStructComponentGradual );
				CreateStructureEnemies( newNodeGradual.ID, newStructObjectGradual, newStructComponentGradual );
				if( newNodeGradual.loadItems && (graph != null) )
				{
					if( graph.IsAreaUnlocked( newNodeGradual.areaLockIndex ) )
					{
						CreateStructureItems( newNodeGradual.ID, newStructObjectGradual, newStructComponentGradual );
						CreateStructureItemClusters( newNodeGradual.ID, newStructComponentGradual );
					}
				}
				if( roomBlocker != null )
				{
					roomBlocker.LoadNodeIntoSlots( newNodeGradual.ID, (int)newStructMapSlots.y, (int)newStructMapSlots.x,
						(int)newStructMapSlots.width, (int)newStructMapSlots.height );
				}
				advanceToNextNextNode = true;
			}

			if( advanceToNextNextNode )
			{
				newStructObjects[newElementIndex] = newStructObjectGradual;
				newStructComponents[newElementIndex] = newStructComponentGradual;
				newNodeMapSlots[newElementIndex] = newStructMapSlots;
				newWorldDoors[newElementIndex] = newWorldDoorGradual;
				newWorldDoorDirections[newElementIndex] = newElementsDirection;
				newWorldDoorGradual = new Rect( 0f, 0f, 0f, 0f );
				newNodeGradual = null;
				newStructComponentGradual = null;
				newStructObjectGradual = null;
				newElementIndex++;
				if( newElementIndex == newElements )
				{
					/*halmeida - all nodes to this direction have been loaded and are sitting quietly at
					the new arrays.*/
					UsefulFunctions.IncreaseArrayWithArray<GameObject>( ref nextStructObjects, newStructObjects );
					UsefulFunctions.IncreaseArrayWithArray<ChallengeStructure>( ref nextStructComponents, newStructComponents );
					UsefulFunctions.IncreaseArrayWithArray<Rect>( ref nextDoors, newWorldDoors );
					UsefulFunctions.IncreaseArrayWithArray<int>( ref nextDoorDirections, newWorldDoorDirections );
					UsefulFunctions.IncreaseArrayWithArray<GraphNode>( ref nextNodes, newNodes );
					UsefulFunctions.IncreaseArrayWithArray<Rect>( ref nextNodeMapSlots, newNodeMapSlots );
					AdvanceNewElementsDirection();
					if( newElementsDirection == DIRECTION_INVALID )
					{
						buildingNextNodes = false;
					}
				}
			}
		}
	}

	private void AdvanceNewElementsDirection()
	{
		if( (currentNode != null) && (currentStructComponent != null) )
		{
			newElements = 0;
			newElementIndex = -1;
			newStructObjects = null;
			newStructComponents = null;
			newWorldDoors = null;
			newWorldDoorDirections = null;
			newNodes = null;
			newNodeMapSlots = null;
			newLocalDoors = null;
			newElementsDirection++;
			//Debug.Log("Debug : ChallengeStage : advancing new elements direction to "+newElementsDirection+".");
			switch( newElementsDirection )
			{
				case DIRECTION_UP:
					newNodes = currentNode.GetNodesUp();
					newLocalDoors = currentStructComponent.GetLocalDoorsUp();
					break;
				case DIRECTION_LEFT:
					newNodes = currentNode.GetNodesLeft();
					newLocalDoors = currentStructComponent.GetLocalDoorsLeft();
					break;
				case DIRECTION_DOWN:
					newNodes = currentNode.GetNodesDown();
					newLocalDoors = currentStructComponent.GetLocalDoorsDown();
					break;
				case DIRECTION_RIGHT:
					newNodes = currentNode.GetNodesRight();
					newLocalDoors = currentStructComponent.GetLocalDoorsRight();
					break;
				default:
					newElementsDirection = DIRECTION_INVALID;
					break;
			}
			if( (newNodes != null) && (newLocalDoors != null) )
			{
				if( (newNodes.Length > 0) && (newNodes.Length == newLocalDoors.Length) )
				{
					newElements = newNodes.Length;
					newElementIndex = 0;
					newStructObjects = new GameObject[newElements];
					newStructComponents = new ChallengeStructure[newElements];
					newNodeMapSlots = new Rect[newElements];
					newWorldDoors = new Rect[newElements];
					newWorldDoorDirections = new int[newElements];
					return;
				}
			}
			if( (newElementIndex == -1) && (newElementsDirection != DIRECTION_INVALID) )
			{
				AdvanceNewElementsDirection();
			}
		}
	}

	private Rect GetLocalDoorToID( GraphNode originNode, ChallengeStructure originStructComponent, int directionFromOrigin, int targetID )
	{
		GraphNode[] nextNodeConnections = null;
		GraphNode nextNodeConnection = null;
		Rect[] nextStructDoors = null;
		Rect nextStructDoor = new Rect( 0f, 0f, 0f, 0f );

		if( (originNode != null) && (originStructComponent != null) )
		{
			switch( directionFromOrigin )
			{
				case DIRECTION_UP:
					nextNodeConnections = originNode.GetNodesUp();
					nextStructDoors = originStructComponent.GetLocalDoorsUp();
					break;
				case DIRECTION_LEFT:
					nextNodeConnections = originNode.GetNodesLeft();
					nextStructDoors = originStructComponent.GetLocalDoorsLeft();
					break;
				case DIRECTION_DOWN:
					nextNodeConnections = originNode.GetNodesDown();
					nextStructDoors = originStructComponent.GetLocalDoorsDown();
					break;
				case DIRECTION_RIGHT:
					nextNodeConnections = originNode.GetNodesRight();
					nextStructDoors = originStructComponent.GetLocalDoorsRight();
					break;
			}
			if( (nextNodeConnections != null) && (nextStructDoors != null) )
			{
				if( nextNodeConnections.Length == nextStructDoors.Length )
				{
					for( int i=0; i<nextNodeConnections.Length; i++ )
					{
						nextNodeConnection = nextNodeConnections[i];
						if( nextNodeConnection != null )
						{
							if( nextNodeConnection.ID == targetID )
							{
								nextStructDoor = nextStructDoors[i];
								break;
							}
						}
					}
				}
			}
		}
		return nextStructDoor;
	}

	private void SnapToMapGrid( GameObject structObject, Rect structMapSlots )
	{
		Vector3 structObjectPosition = Vector3.zero;

		if( (structObject != null) && (roomSlotWidth > 0f) && (roomSlotHeight > 0f) )
		{
			structObjectPosition = structObject.transform.localPosition;
			structObjectPosition.x = structMapSlots.x * roomSlotWidth;
			structObjectPosition.y = structMapSlots.y * -roomSlotHeight;
			structObject.transform.localPosition = structObjectPosition;
		}
	}

	public void ReactToInputDirection( int characterIndex, bool leftStick, Vector2 inputDirection )
	{
		PlayerAgent characterComponent = null;

		characterComponent = GetCharacter( characterIndex );
		if( characterComponent != null )
		{
			if( leftStick )
			{
				characterComponent.SetMovementDirection( inputDirection );
			}
			else
			{
				characterComponent.SetAimDirection( inputDirection );
			}
		}
	}

	public void ReactToUpKey( int characterIndex, bool leftStick, bool pressed )
	{
		PlayerAgent characterComponent = null;

		characterComponent = GetCharacter( characterIndex );
		if( characterComponent != null )
		{
			if( leftStick )
			{
				characterComponent.SetUpDirectionMove( pressed );
			}
			else
			{
				characterComponent.SetUpDirectionAim( pressed );
			}
		}
	}

	public void ReactToLeftKey( int characterIndex, bool leftStick, bool pressed )
	{
		PlayerAgent characterComponent = null;

		characterComponent = GetCharacter( characterIndex );
		if( characterComponent != null )
		{
			if( leftStick )
			{
				characterComponent.SetLeftDirectionMove( pressed );
			}
			else
			{
				characterComponent.SetLeftDirectionAim( pressed );
			}
		}
	}

	public void ReactToDownKey( int characterIndex, bool leftStick, bool pressed )
	{
		PlayerAgent characterComponent = null;

		characterComponent = GetCharacter( characterIndex );
		if( characterComponent != null )
		{
			if( leftStick )
			{
				characterComponent.SetDownDirectionMove( pressed );
			}
			else
			{
				characterComponent.SetDownDirectionAim( pressed );
			}
		}
	}

	public void ReactToRightKey( int characterIndex, bool leftStick, bool pressed )
	{
		PlayerAgent characterComponent = null;

		characterComponent = GetCharacter( characterIndex );
		if( characterComponent != null )
		{
			if( leftStick )
			{
				characterComponent.SetRightDirectionMove( pressed );
			}
			else
			{
				characterComponent.SetRightDirectionAim( pressed );
			}
		}
	}

	public void ReleaseAllKeys( int characterIndex )
	{
		PlayerAgent characterComponent = null;

		characterComponent = GetCharacter( characterIndex );
		if( characterComponent != null )
		{
			characterComponent.CancelCommands();
		}
	}

	public void ToggleGaugeVisibility( int characterIndex, bool enable )
	{
		PlayerAgent characterComponent = null;

		characterComponent = GetCharacter( characterIndex );
		if( characterComponent != null )
		{
			if( enable )
			{
				characterComponent.ShowGauges();
			}
			else
			{
				characterComponent.HideGauges();
			}
		}
	}

	/*halmeida - this is meant to be used by the GameController class only.*/
	public void RequestPause( GameController requester )
	{
		if( (requester != null) && (requester == gameController) )
		{
			pausedFromOutside = true;
		}
	}

	/*halmeida - this is meant to be used by the GameController class only.*/
	public void RequestUnpause( GameController requester )
	{
		if( (requester != null) && (requester == gameController) )
		{
			pausedFromOutside = false;
		}
	}

	private void TogglePause( bool pause )
	{
		if( pause )
		{
			if( !paused )
			{
				paused = true;
				ToggleAgentPause( true );
				if( cameraController != null )
				{
					cameraController.TogglePause( true );
				}
			}
		}
		else
		{
			if( paused )
			{
				paused = false;
				ToggleAgentPause( false );
				if( cameraController != null )
				{
					cameraController.TogglePause( false );
				}
			}
		}
	}

	private void ToggleAgentPause( bool pause )
	{
		BaseAgent agent = null;

		if( playerComponent != null )
		{
			playerComponent.TogglePause( pause );
		}
		if( npcComponents != null )
		{
			for( int i=0; i<npcComponents.Length; i++ )
			{
				agent = npcComponents[i];
				if( agent != null )
				{
					agent.TogglePause( pause );
				}
			}
		}
		if( enemyComponents != null )
		{
			for( int i=0; i<enemyComponents.Length; i++ )
			{
				agent = enemyComponents[i];
				if( agent != null )
				{
					agent.TogglePause( pause );
				}
			}
		}
	}

	public PlayerAgent GetCharacter( int characterIndex )
	{
		if( characterIndex == -1 )
		{
			return playerComponent;
		}
		else if( characterIndex > -1 )
		{
			if( npcComponents != null )
			{
				if( npcComponents.Length > characterIndex )
				{
					return npcComponents[characterIndex];
				}
			}
		}
		return null;
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

	public PlayerAgent[] GetAllNpcComponents()
	{
		return npcComponents;
	}

	public Waypoint[] GetAllWaypoints()
	{
		return waypoints;
	}

	public Waypoint GetWaypointFromCurrentStruct( int waypointID )
	{
		int[] waypointIDs = null;
		Waypoint waypoint = null;

		if( (waypoints != null) && (currentStructComponent != null) )
		{
			waypointIDs = currentStructComponent.GetWaypointIDs();
			if( waypointIDs != null )
			{
				for( int i=0; i<waypointIDs.Length; i++ )
				{
					if( waypointIDs[i] == waypointID )
					{
						/*halmeida - the waypoint requested belongs to the current struct, but
						to get the actual object we gotta find the waypoint within the great
						waypoint array that gathers waypoints from all loaded structures.*/
						for( int j=0; j<waypoints.Length; j++ )
						{
							waypoint = waypoints[j];
							if( waypoint != null )
							{
								if( waypoint.ID == waypointID )
								{
									return waypoint;
								}
							}
						}
					}
				}
			}
		}
		return null;
	}

	public int GetAgentID( string agentName, ref BaseAgent agentComponent )
	{
		BaseAgent agent = null;

		if( agentName != null )
		{
			if( playerComponent != null )
			{
				if( playerComponent.agentName == agentName )
				{
					agentComponent = playerComponent;
					return playerComponent.agentID;
				}
			}
			if( npcComponents != null )
			{
				for( int i=0; i<npcComponents.Length; i++ )
				{
					agent = npcComponents[i];
					if( agent != null )
					{
						if( agent.agentName == agentName )
						{
							agentComponent = agent;
							return agent.agentID;
						}
					}
				}
			}
			if( enemyComponents != null )
			{
				for( int i=0; i<enemyComponents.Length; i++ )
				{
					agent = enemyComponents[i];
					if( agent != null )
					{
						if( agent.agentName == agentName )
						{
							agentComponent = agent;
							return agent.agentID;
						}
					}
				}
			}
		}
		agentComponent = null;
		return BaseAgent.INVALID_AGENT_ID;
	}

	public RoutedAgent GetAgent( int agentID, int nodeID = GraphNode.NODE_ID_INVALID )
	{
		RoutedAgent agent = null;
		int agentNodeID = GraphNode.NODE_ID_INVALID;

		if( playerComponent != null )
		{
			if( playerComponent.agentID == agentID )
			{
				return playerComponent;
			}
		}
		if( npcComponents != null )
		{
			for( int i=0; i<npcComponents.Length; i++ )
			{
				agent = npcComponents[i];
				if( agent != null )
				{
					if( agent.agentID == agentID )
					{
						return agent;
					}
				}
			}
		}
		if( enemyComponents != null )
		{
			for( int i=0; i<enemyComponents.Length; i++ )
			{
				agent = enemyComponents[i];
				if( agent != null )
				{
					if( agent.agentID == agentID )
					{
						if( nodeID == GraphNode.NODE_ID_INVALID )
						{
							return agent;
						}
						else
						{
							/*halmeida - relying on the coherence of all the "enemy" arrays.*/
							agentNodeID = enemyNodeIDs[i];
							if( agentNodeID == nodeID )
							{
								return agent;
							}
						}
					}
				}
			}
		}
		return null;
	}

	public Graph GetGraph()
	{
		return graph;
	}

	public int GetCurrentNodeID()
	{
		if( currentNode != null )
		{
			return currentNode.ID;
		}
		return GraphNode.NODE_ID_INVALID;
	}

	public InputManager GetInputManager()
	{
		return inputManager;
	}

	public CameraController GetCameraController()
	{
		return cameraController;
	}

	public ItemDatabase GetItemDatabase()
	{
		return itemDatabase;
	}

	public ObstacleDatabase GetObstacleDatabase()
	{
		return obstacleDatabase;
	}

	public bool IsSavingEnabled()
	{
		return !savingDisabled;
	}

	public void EnableSaving()
	{
		savingDisabled = false;
	}
}
