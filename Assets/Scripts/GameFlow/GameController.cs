using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary; //to use BinaryFormatter
using System.Runtime.Serialization; //to use SerializationException
using System.IO; //to use FileStream or MemoryStream
using System; //to use Exception

public class GameController : MonoBehaviour
{
	public const int AVERAGE_FRAMES = 30;/*halmeida - how many frame times we use to calculate and display an average FPS*/

	public const int TITLE_TYPE_STUDIO = 0;
	public const int TITLE_TYPE_GAME = 1;

	public const float TITLE_STUDIO_FADE_SPEED = 0.8f;
	public const float TITLE_STUDIO_PLAIN_DURATION = 1.6f;
	public const float TITLE_GAME_FADE_SPEED = 1f;
	public const float TITLE_GAME_PLAIN_DURATION = 1.6f;
	public const float OVERLAY_FADE_SPEED_GAME = 0.9f;
	public const float OVERLAY_FADE_SPEED_STAGES = 1f;//4f;

	public const int STATUS_INVALID = -1;
	public const int STATUS_SPLASH = 0;
	public const int STATUS_EDITOR = 1;
	public const int STATUS_TITLE = 2;
	public const int STATUS_TITLE_AND_GAME = 3;
	public const int STATUS_GAME = 4;
	public const int STATUS_PAUSE = 5;

	public const string FILE_NAME_GAME_PROGRESS = "GameProgress.dat";
	public const string FILE_NAME_PLAYER_PROGRESS = "PlayerProgress.dat";
	public const string DIRECTORY_PATH_GAME_TEXT = "Assets/Resources/Text/";
	public const string FILE_NAME_GAME_TEXT_EN = "GameTextXMLEnglish"; 
	public const string FILE_NAME_GAME_TEXT_PT = "GameTextXMLPortuguese";

	public const int INVALID_LANGUAGE_CODE = -1;
	public const int LANGUAGE_CODE_EN = 0;
	public const int LANGUAGE_CODE_PT = 1;

	public GameObject cameraObject;
	public GameObject interfaceCanvasObject;
	public bool showFPS;
	public bool forceDestination;
	public int forceDestinationNodeID;
	public GameObject inputManagerModel;
	public bool challengeEditorMode;
	public GameObject challengeEditorModel;
	public GameObject[] tileFactoryModels;
	public GameObject playerEditorModel;
	public GameObject playerGameModel;
	public GameObject fontDialogueModel;
	public GameObject fontDamageModel;
	public GameObject fontUIModel;
	public GameObject fontFPSModel;
	public GameObject itemDatabaseModel;
	public GameObject obstacleDatabaseModel;
	public GameObject enemyDatabaseModel;
	public GameObject boxDatabaseModel;
	public GameObject backgroundDatabaseModel;
	public GameObject graphModel;
	public GameObject imageOverlayModel;
	public GameObject imageStudioTitleModel;
	public GameObject imageGameTitleModel;
	public GameObject roomBlockerModel;
	public GameObject eventManagerModel;
	public GameObject menuPauseModel;
	public GameObject menuHelpModel;
	public GameObject menuMapModel;
	public GameObject menuInventoryModel;
	public bool debug;
	public GameObject debugItem;
	public GameObject debugObstacle;
	public int debugExtraCoins;
	public bool debugSkills;
	public bool debugSkillShoot;
	public int debugSkillBreak;
	public int[] debugExtraItemIDs;

	private Camera cameraComponent;
	private Canvas interfaceCanvasComponent;
	private RectTransform interfaceCanvasTrans;
	private Rect interfaceCanvasRect;
	private Rect interfaceCanvasScreenRect;
	private GameObject databasesParent;
	private GameObject fontDialogueObject;
	private GameObject fontDamageObject;
	private GameObject fontUIObject;
	private GameObject fontFPSObject;
	private SymbolDatabase fontDialogue;
	private SymbolDatabase fontDamage;
	private SymbolDatabase fontUI;
	private SymbolDatabase fontFPS;
	private GameObject itemDatabaseObject;
	private ItemDatabase itemDatabase;
	private GameObject obstacleDatabaseObject;
	private ObstacleDatabase obstacleDatabase;
	private GameObject enemyDatabaseObject;
	private EnemyDatabase enemyDatabase;
	private GameObject boxDatabaseObject;
	private BoxAppearanceDatabase boxDatabase;
	private GameObject backgroundDatabaseObject;
	private BackgroundDatabase backgroundDatabase;
	private GameTextDatabase gameTextDatabase;
	private int gameTextLanguage;
	private int fpsFrames;
	private float fpsTime;
	private float fps;
	private GameObject fpsObject;
	private SpritedStringUI fpsSpritedString;
	private int currentStatus;
	private GameObject inputManagerObject;
	private InputManager inputManager;
	private bool ignoreBriefly;
	private bool editorMode;
	private GameObject challengeEditorObject;
	private ChallengeEditor challengeEditor;
	private GameObject tileFactoriesParent;
	private GameObject[] tileFactoryObjects;
	private TileFactory[] tileFactories;
	private GameObject graphObject;
	private Graph graph;
	private GameObject imageOverlayObject;
	private Image imageOverlayComponent;
	private GameObject imageTitleObject;
	private Image imageTitleComponent;
	private float titleDuration;
	private float titleElapsed;
	private float titleFadeSpeed;
	private bool titleFadeOver;
	private float overlayFadeSpeed;
	private bool overlayDark;
	private bool overlayClear;
	private Vector3 overlayRGB;
	private TextAsset[] challengeTextAssets;
	private StructurePersistentData[] challengeData;
	private int startingNodeID;
	private int startingNodeSlotRow;
	private int startingNodeSlotColumn;
	private bool selectNode;
	private bool nodeSelectionPending;
	private int selectedNodeID;
	private int selectedNodeSlotRow;
	private int selectedNodeSlotColumn;
	private GameObject stageObject;
	private ChallengeStage stageComponent;
	private bool stageBuilt;
	private bool stageStarted;
	private GameObject playerObject;
	private PlayerAgent playerComponent;
	private Vector2 playerOffsetWithinNode;
	private bool playerFacingSide;
	private GameObject roomBlockerObject;
	private RoomSightBlocker roomBlockerComponent;
	private GameObject eventManagerObject;
	private EventManager eventManager;
	private GameObject menuPauseObject;
	private GameObject menuHelpObject;
	private GameObject menuMapObject;
	private GameObject menuInventoryObject;
	private MenuPause menuPauseComponent;
	private MenuSimple menuHelpComponent;
	private MenuMap menuMapComponent;
	private MenuInventory menuInventoryComponent;
	private MenuSimple frontMenuComponent;
	private TextBoxManager textBoxManager;
	private TextBoxUIManager textBoxUIManager;
	private IOptionsListener pausedQuestionListener;
	private bool enableKeypadAtResume;

	private GamePersistentData gameProgressData;
	private PlayerPersistentData playerProgressData;
	private string gameProgressFilePath;
	private string playerProgressFilePath;

	void Awake()
	{
		cameraComponent = null;
		if( cameraObject != null )
		{
			cameraComponent = cameraObject.GetComponent<Camera>();
			if( cameraComponent == null )
			{
				cameraObject = null;
			}
		}
		interfaceCanvasComponent = null;
		interfaceCanvasTrans = null;
		interfaceCanvasRect = default( Rect );
		interfaceCanvasScreenRect = default( Rect );
		if( interfaceCanvasObject != null )
		{
			interfaceCanvasComponent = interfaceCanvasObject.GetComponent<Canvas>();
			/*halmeida - can't extract canvas' properties like its size just yet, cause the canvas will
			still change its dimensions if we are in the editor.*/
		}

		fpsFrames = 0;
		fpsTime = 0f;
		fps = 0f;
		fpsObject = null;
		fpsSpritedString = null;

		databasesParent = null;
		fontDialogueObject = null;
		fontDialogue = null;
		fontDamageObject = null;
		fontDamage = null;
		fontUIObject = null;
		fontUI = null;
		fontFPSObject = null;
		fontFPS = null;
		itemDatabaseObject = null;
		itemDatabase = null;
		obstacleDatabaseObject = null;
		obstacleDatabase = null;
		enemyDatabaseObject = null;
		enemyDatabase = null;
		boxDatabaseObject = null;
		boxDatabase = null;
		backgroundDatabaseObject = null;
		backgroundDatabase = null;
		gameTextDatabase = null;
		gameTextLanguage = INVALID_LANGUAGE_CODE;
		CreateDatabases();

		currentStatus = STATUS_INVALID;
		inputManagerObject = null;
		inputManager = null;
		ignoreBriefly = false;
		editorMode = challengeEditorMode;
		challengeEditorObject = null;
		challengeEditor = null;

		tileFactoriesParent = null;
		tileFactoryObjects = null;
		tileFactories = null;
		ValidateTileFactories();

		graphObject = null;
		graph = null;
		CreateGraph();

		imageOverlayObject = null;
		imageOverlayComponent = null;
		imageTitleObject = null;
		imageTitleComponent = null;
		titleDuration = 0f;
		titleElapsed = 0f;
		titleFadeSpeed = 0f;
		titleFadeOver = false;
		overlayFadeSpeed = 0f;
		overlayDark = false;
		overlayClear = false;
		overlayRGB = Vector3.zero;

		challengeTextAssets = null;
		challengeData = null;
		startingNodeID = GraphNode.NODE_ID_INVALID;
		startingNodeSlotRow = -1;
		startingNodeSlotColumn = -1;
		selectNode = false;
		nodeSelectionPending = false;
		selectedNodeID = GraphNode.NODE_ID_INVALID;
		selectedNodeSlotRow = -1;
		selectedNodeSlotColumn = -1;
		stageObject = null;
		stageComponent = null;
		stageBuilt = false;
		stageStarted = false;
		playerObject = null;
		playerComponent = null;
		playerOffsetWithinNode = Vector2.zero;
		playerFacingSide = BaseAnimator.SIDE_RIGHT;
		roomBlockerObject = null;
		roomBlockerComponent = null;
		eventManagerObject = null;
		eventManager = null;
		if( eventManagerModel != null )
		{
			eventManagerObject = Instantiate( eventManagerModel, Vector3.zero, Quaternion.identity ) as GameObject;
			eventManager = eventManagerObject.GetComponent<EventManager>();
			if( eventManager == null )
			{
				Destroy( eventManagerObject );
				eventManagerObject = null;
			}
		}

		menuPauseObject = null;
		menuHelpObject = null;
		menuMapObject = null;
		menuInventoryObject = null;
		menuPauseComponent = null;
		menuHelpComponent = null;
		menuMapComponent = null;
		menuInventoryComponent = null;
		frontMenuComponent = null;
		textBoxManager = null;
		textBoxUIManager = null;
		pausedQuestionListener = null;
		enableKeypadAtResume = true;
		/*halmeida - can't create the menus yet cause they will need the AudioCenter ready when they try to
		load sounds in their respective Awake()'s, but the Awake of the AudioCenter itself hasn't yet happened.*/

		gameProgressData = null;
		playerProgressData = null;
		gameProgressFilePath = Application.persistentDataPath+"/"+FILE_NAME_GAME_PROGRESS;
		playerProgressFilePath = Application.persistentDataPath+"/"+FILE_NAME_PLAYER_PROGRESS;
	}

	private void ValidateTileFactories()
	{
		int validFactoryModels = 0;
		int insertionIndex = 0;
		GameObject tileFactoryObject = null;

		if( (tileFactoryModels != null) && (tileFactoryObjects == null) )
		{
			if( tileFactoriesParent == null )
			{
				tileFactoriesParent = new GameObject("TileFactoriesParent");
				tileFactoriesParent.transform.SetParent( transform, false );
				tileFactoriesParent.transform.localPosition = Vector3.zero;
			}
			for( int i=0; i<tileFactoryModels.Length; i++ )
			{
				tileFactoryObject = tileFactoryModels[i];
				if( tileFactoryObject != null )
				{
					if( tileFactoryObject.GetComponent<TileFactory>() != null )
					{
						validFactoryModels++;
					}
				}
			}
			if( validFactoryModels > 0 )
			{
				tileFactoryObjects = new GameObject[validFactoryModels];
				tileFactories = new TileFactory[validFactoryModels];
				for( int i=0; i<tileFactoryModels.Length; i++ )
				{
					tileFactoryObject = tileFactoryModels[i];
					if( tileFactoryObject != null )
					{
						if( tileFactoryObject.GetComponent<TileFactory>() != null )
						{
							tileFactoryObject = Instantiate( tileFactoryModels[i], Vector3.zero, Quaternion.identity ) as GameObject;
							tileFactoryObjects[insertionIndex] = tileFactoryObject;
							tileFactories[insertionIndex] = tileFactoryObject.GetComponent<TileFactory>();
							tileFactoryObject.transform.SetParent( tileFactoriesParent.transform, false );
							tileFactoryObject.transform.localPosition = Vector3.zero;
							insertionIndex++;
						}
					}
				}
			}
		}
	}

	private void CreateDatabases()
	{
		if( databasesParent == null )
		{
			databasesParent = new GameObject("DatabasesParent");
			databasesParent.transform.SetParent( transform, false );
			databasesParent.transform.localPosition = Vector3.zero;
		}
		if( (fontDialogueObject == null) && (fontDialogueModel != null) )
		{
			fontDialogueObject = Instantiate( fontDialogueModel, Vector3.zero, Quaternion.identity ) as GameObject;
			fontDialogue = fontDialogueObject.GetComponent<SymbolDatabase>();
			if( fontDialogue == null )
			{
				Destroy( fontDialogueObject );
				fontDialogueObject = null;
			}
			else
			{
				fontDialogueObject.transform.SetParent( databasesParent.transform, false );
				fontDialogueObject.transform.localPosition = Vector3.zero;
			}
		}
		if( (fontDamageObject == null) && (fontDamageModel != null) )
		{
			fontDamageObject = Instantiate( fontDamageModel, Vector3.zero, Quaternion.identity ) as GameObject;
			fontDamage = fontDamageObject.GetComponent<SymbolDatabase>();
			if( fontDamage == null )
			{
				Destroy( fontDamageObject );
				fontDamageObject = null;
			}
			else
			{
				fontDamageObject.transform.SetParent( databasesParent.transform, false );
				fontDamageObject.transform.localPosition = Vector3.zero;
			}
		}
		if( (fontUIObject == null) && (fontUIModel != null) )
		{
			fontUIObject = Instantiate( fontUIModel, Vector3.zero, Quaternion.identity ) as GameObject;
			fontUI = fontUIObject.GetComponent<SymbolDatabase>();
			if( fontUI == null )
			{
				Destroy( fontUIObject );
				fontUIObject = null;
			}
			else
			{
				fontUIObject.transform.SetParent( databasesParent.transform, false );
				fontUIObject.transform.localPosition = Vector3.zero;
			}
		}
		if( showFPS && (fontFPSObject == null) && (fontFPSModel != null) )
		{
			fontFPSObject = Instantiate( fontFPSModel, Vector3.zero, Quaternion.identity ) as GameObject;
			fontFPS = fontFPSObject.GetComponent<SymbolDatabase>();
			if( fontFPS == null )
			{
				Destroy( fontFPSObject );
				fontFPSObject = null;
			}
			else
			{
				fontFPSObject.transform.SetParent( databasesParent.transform, false );
				fontFPSObject.transform.localPosition = Vector3.zero;
			}
		}
		if( (itemDatabaseObject == null) && (itemDatabaseModel != null) )
		{
			itemDatabaseObject = Instantiate( itemDatabaseModel, Vector3.zero, Quaternion.identity ) as GameObject;
			itemDatabase = itemDatabaseObject.GetComponent<ItemDatabase>();
			if( itemDatabase == null )
			{
				Destroy( itemDatabaseObject );
				itemDatabaseObject = null;
			}
			else
			{
				itemDatabaseObject.transform.SetParent( databasesParent.transform, false );
				itemDatabaseObject.transform.localPosition = Vector3.zero;
			}
		}
		if( (obstacleDatabaseObject == null) && (obstacleDatabaseModel != null) )
		{
			obstacleDatabaseObject = Instantiate( obstacleDatabaseModel, Vector3.zero, Quaternion.identity ) as GameObject;
			obstacleDatabase = obstacleDatabaseObject.GetComponent<ObstacleDatabase>();
			if( obstacleDatabase == null )
			{
				Destroy( obstacleDatabaseObject );
				obstacleDatabaseObject = null;
			}
			else
			{
				obstacleDatabaseObject.transform.SetParent( databasesParent.transform, false );
				obstacleDatabaseObject.transform.localPosition = Vector3.zero;
			}
		}
		if( (enemyDatabaseObject == null) && (enemyDatabaseModel != null) )
		{
			enemyDatabaseObject = Instantiate( enemyDatabaseModel, Vector3.zero, Quaternion.identity ) as GameObject;
			enemyDatabase = enemyDatabaseObject.GetComponent<EnemyDatabase>();
			if( enemyDatabase == null )
			{
				Destroy( enemyDatabaseObject );
				enemyDatabaseObject = null;
			}
			else
			{
				enemyDatabaseObject.transform.SetParent( databasesParent.transform, false );
				enemyDatabaseObject.transform.localPosition = Vector3.zero;
			}
		}
		if( (boxDatabaseObject == null) && (boxDatabaseModel != null) )
		{
			boxDatabaseObject = Instantiate( boxDatabaseModel, Vector3.zero, Quaternion.identity ) as GameObject;
			boxDatabase = boxDatabaseObject.GetComponent<BoxAppearanceDatabase>();
			if( boxDatabase == null )
			{
				Destroy( boxDatabaseObject );
				boxDatabaseObject = null;
			}
			else
			{
				boxDatabaseObject.transform.SetParent( databasesParent.transform, false );
				boxDatabaseObject.transform.localPosition = Vector3.zero;
			}
		}
		if( (backgroundDatabaseObject == null) && (backgroundDatabaseModel != null) )
		{
			backgroundDatabaseObject = Instantiate( backgroundDatabaseModel, Vector3.zero, Quaternion.identity ) as GameObject;
			backgroundDatabase = backgroundDatabaseObject.GetComponent<BackgroundDatabase>();
			if( backgroundDatabase == null )
			{
				Destroy( backgroundDatabaseObject );
				backgroundDatabaseObject = null;
			}
			else
			{
				backgroundDatabaseObject.transform.SetParent( databasesParent.transform, false );
				backgroundDatabase.transform.localPosition = Vector3.zero;
			}
		}
		LoadTextDatabase();
	}

	private void CreateGraph()
	{
		if( (graphObject == null) && (graphModel != null) )
		{
			graphObject = Instantiate( graphModel, Vector3.zero, Quaternion.identity ) as GameObject;
			graph = graphObject.GetComponent<Graph>();
			if( graph == null )
			{
				Destroy( graphObject );
				graphObject = null;
			}
		}
	}

	private void CreateMenus()
	{
		if( (menuPauseObject == null) && (menuPauseModel != null) )
		{
			menuPauseObject = Instantiate( menuPauseModel, Vector3.zero, Quaternion.identity ) as GameObject;
			menuPauseComponent = menuPauseObject.GetComponent<MenuPause>();
			if( menuPauseComponent == null )
			{
				Destroy( menuPauseObject );
				menuPauseObject = null;
			}
		}
		if( (menuHelpObject == null) && (menuHelpModel != null) )
		{
			menuHelpObject = Instantiate( menuHelpModel, Vector3.zero, Quaternion.identity ) as GameObject;
			menuHelpComponent = menuHelpObject.GetComponent<MenuSimple>();
			if( menuHelpComponent == null )
			{
				Destroy( menuHelpObject );
				menuHelpObject = null;
			}
		}
		if( (menuMapObject == null) && (menuMapModel != null) )
		{
			menuMapObject = Instantiate( menuMapModel, Vector3.zero, Quaternion.identity ) as GameObject;
			menuMapComponent = menuMapObject.GetComponent<MenuMap>();
			if( menuMapComponent == null )
			{
				Destroy( menuMapObject );
				menuMapObject = null;
			}
			else
			{
				if( graph != null )
				{
					graph.SetAssociatedMap( menuMapComponent );
				}
			}
		}
		if( (menuInventoryObject == null) && (menuInventoryModel != null) )
		{
			menuInventoryObject = Instantiate( menuInventoryModel, Vector3.zero, Quaternion.identity ) as GameObject;
			menuInventoryComponent = menuInventoryObject.GetComponent<MenuInventory>();
			if( menuInventoryComponent == null )
			{
				Destroy( menuInventoryObject );
				menuInventoryObject = null;
			}
		}
		textBoxManager = TextBoxManager.Instance;
		textBoxUIManager = TextBoxUIManager.Instance;
	}

	private void LinkMenusToGame()
	{
		if( menuPauseComponent != null )
		{
			menuPauseComponent.SetGameController( this, interfaceCanvasComponent, imageOverlayModel, eventManager );
			menuPauseComponent.SetItemDatabase( itemDatabase );
		}
		if( menuHelpComponent != null )
		{
			menuHelpComponent.SetGameController( this, interfaceCanvasComponent, imageOverlayModel, eventManager );
		}
		if( menuMapComponent != null )
		{
			menuMapComponent.SetGameController( this, interfaceCanvasComponent, imageOverlayModel, eventManager );
		}
		if( menuInventoryComponent != null )
		{
			menuInventoryComponent.SetGameController( this, interfaceCanvasComponent, imageOverlayModel, eventManager );
			menuInventoryComponent.SetItemDatabase( itemDatabase );
		}
		/*if( menuCreditsComponent != null )
		{
			menuCreditsComponent.SetChallengeGame( this, interfaceCanvasComponent, imageOverlayModel );
		}*/
	}

	void Update()
	{
		int clearingType = ChallengeStage.CLEAR_INVALID;
		bool backToGame = false;
		MusicController musicController = null;

		if( showFPS )
		{
			CalculateFPS( Time.deltaTime );
		}
		//Debug.Log("Debug : GameController : delta time = "+Time.deltaTime+".");
		UpdateRunningFades( Time.deltaTime );
		if( inputManager != null )
		{
			inputManager.Progress( Time.deltaTime );
		}
		switch( currentStatus )
		{
			case STATUS_INVALID:
				ExtractCanvasProperties();
				currentStatus = STATUS_SPLASH;
				break;
			case STATUS_SPLASH:
				if( (inputManagerObject == null) && (inputManagerModel != null) )
				{
					inputManagerObject = Instantiate( inputManagerModel, Vector3.zero, Quaternion.identity ) as GameObject;
					inputManager = inputManagerObject.GetComponent<InputManager>();
					if( inputManager != null )
					{
						inputManager.SetCameraObject( cameraObject );
						inputManager.SetGameController( this, interfaceCanvasComponent );
					}
				}
				if( editorMode )
				{
					if( LoadChallengeEditor() )
					{
						currentStatus = STATUS_EDITOR;
					}
					else
					{
						editorMode = false;
					}
				}
				if( !editorMode )
				{
					if( imageTitleComponent == null )
					{
						//if( cameraComponent != null )
						//{
						//	cameraComponent.backgroundColor = Color.black;
						//}
						LoadFadeOverlay( true, overlayRGB );
						if( LoadTitle( TITLE_TYPE_STUDIO ) )
						{
							titleFadeSpeed = TITLE_STUDIO_FADE_SPEED;
							titleFadeOver = false;
						}
						else
						{
							CreateMenus();
							if( challengeData == null )
							{
								LoadAllChallengeData();
							}
							currentStatus = STATUS_TITLE;
						}
					}
					else
					{
						if( titleFadeOver )
						{
							if( titleDuration == 0f )
							{
								titleDuration = TITLE_STUDIO_PLAIN_DURATION;
								titleElapsed = 0f;
								CreateMenus();
								if( challengeData == null )
								{
									LoadAllChallengeData();
								}
							}
							else
							{
								if( titleDuration <= titleElapsed )
								{
									if( titleFadeSpeed > 0f )
									{
										titleFadeSpeed *= -1f;
										titleFadeOver = false;
									}
									else
									{
										imageTitleComponent = null;
										Destroy( imageTitleObject );
										imageTitleObject = null;
										titleDuration = 0f;
										titleElapsed = 0f;
										titleFadeSpeed = 0f;
										titleFadeOver = false;
										currentStatus = STATUS_TITLE;
									}
								}
							}
						}
					}
				}
				break;
			case STATUS_EDITOR:
				if( challengeEditor != null )
				{
					challengeEditor.Progress( Time.deltaTime );
				}
				break;
			case STATUS_TITLE:
				if( imageTitleComponent == null )
				{
					if( LoadTitle( TITLE_TYPE_GAME ) )
					{
						titleFadeSpeed = TITLE_GAME_FADE_SPEED;
						titleFadeOver = false;
					}
					else
					{
						titleFadeOver = true;
					}
				}
				if( titleFadeOver )
				{
					if( titleDuration == 0f )
					{
						titleDuration = TITLE_GAME_PLAIN_DURATION;
						if( imageTitleComponent != null )
						{
							titleElapsed = 0f;
						}
						else
						{
							titleElapsed = titleDuration;
						}
						LoadGameProgress();
						selectNode = false;
						LinkMenusToGame();
						//currentStageID = ChallengeStage.INVALID_STAGE_ID;
						//currentStageExit = ChallengeStage.INVALID_EXIT;
						nodeSelectionPending = false;
					}
					else
					{
						if( titleElapsed >= titleDuration )
						{
							/*halmeida - offer stage selection here*/
							if( selectNode )
							{
								selectNode = false;
								nodeSelectionPending = true;
								//if( menuGraphComponent != null )
								//{
								//	if( inputManager != null )
								//	{
								//		inputManager.SetMenu( menuGraphComponent );
								//	}
								//	menuGraphComponent.BuildVisualRepresentation( false );
								//}
							}
							else
							{
								if( !nodeSelectionPending )
								{
									titleFadeSpeed = 0f;
									titleFadeOver = false;
									currentStatus = STATUS_TITLE_AND_GAME;
								}
							}
						}
					}
				}
				//if( menuGraphComponent != null )
				//{
				//	menuGraphComponent.Progress( Time.fixedDeltaTime );
				//}
				break;
			case STATUS_TITLE_AND_GAME:
				if( stageComponent == null )
				{
					LoadStage();
				}
				if( stageComponent != null )
				{
					if( !stageBuilt )
					{
						stageBuilt = stageComponent.StartBuilding( startingNodeID, startingNodeSlotRow, startingNodeSlotColumn );
					}
				}
				if( stageBuilt && !stageStarted )
				{
					if( LoadPlayer() )
					{
						//if( purchaser != null )
						//{
							/*halmeida - since the player component is now available to receive items,
							we can allow the purchaser to apply all pending purchases.*/ 
						//	purchaser.SetGameController( this );
						//}
						if( inputManager != null )
						{
							inputManager.CreateControls();
						}
						/*halmeida - add a player to complete the elements awaited by the stage.*/
						stageComponent.SetPlayerComponent( playerComponent );
						/*halmeida - give the stage a chance to place things correctly.*/
						stageComponent.Play( playerOffsetWithinNode, playerFacingSide );
						stageStarted = true;
						if( debug )
						{
							if( debugItem != null )
							{
								stageComponent.AddDebugItem( debugItem );
							}
							if( debugObstacle != null )
							{
								stageComponent.AddDebugObstacle( debugObstacle );
							}
						}
					}
				}
				if( stageStarted )
				{
					/*halmeida - during the fadein, the stage may continue to build itself.*/
					stageComponent.Progress( Time.deltaTime );
					UpdateFadeRequests();
					if( overlayClear )
					{
						currentStatus = STATUS_GAME;
					}
				}
				break;
			case STATUS_GAME:
				ignoreBriefly = false;
				/*halmeida - the line above is suspending the denial of requests from the InputManager. It is
				necessary because, when elements of the virtual keypad are disabled and then, later on, enabled,
				the input manager undestands as if the pointer has just entered the area of an element, even
				if the pointer had just been resting there all along. The consequence of such a fact is that as
				soon as I unpaused the game, enabling back the virtual keypad, before this Update could happen,
				the virtual keypad was interpreting the invasion, by the pointer, of the recently allowed pause
				button and sending the game once again into the paused state. I unpaused and the game immediately
				paused again. To prevent that, once I unpause the game, enabling back the virtual keypad, I ignore
				the virtual keypad's claims until the line above has been executed. Since the abusive claim happens
				before the Update runs, I can stop ignoring claims from the InputManager as soon as Update is called.*/
				if( imageTitleComponent != null )
				{
					if( !titleFadeOver )
					{
						titleFadeSpeed = -TITLE_GAME_FADE_SPEED;
					}
					else
					{
						imageTitleComponent = null;
						Destroy( imageTitleObject );
						imageTitleObject = null;
						titleDuration = 0f;
						titleElapsed = 0f;
						titleFadeSpeed = 0f;
						titleFadeOver = false;
					}
				}
				if( stageComponent != null )
				{
					stageComponent.Progress( Time.deltaTime );
					UpdateFadeRequests();
					if( stageComponent.RequestingClear( ref clearingType ) )
					{
						if( overlayDark )
						{
							/*halmeida - when a stage is cleared, its items are necessarily cleared. If
							an event has created an item and is waiting for the player to obtain it, this
							event has to be canceled or it will be forever monitoring garbage, since the
							item component will be deleted.*/
							//if( eventManager != null )
							//{
							//	eventManager.CloseStageEvents();
							//}
							if( clearingType == ChallengeStage.CLEAR_DEATH )
							{
								LoadGameProgress();
								LoadPlayerProgress();
								if( stageComponent.DeathClear( startingNodeID ) )
								{
									stageComponent.Play( playerOffsetWithinNode, playerFacingSide );
								}
							}
						}
					}
				}
				break;
			case STATUS_PAUSE:
				if( stageComponent != null )
				{
					stageComponent.Progress( Time.deltaTime );
				}
				backToGame = true;
				if( menuPauseComponent != null )
				{
					if( menuPauseComponent.RequiresProgress() )
					{
						menuPauseComponent.Progress( Time.deltaTime );
						backToGame = false;
					}
				}
				if( menuHelpComponent != null )
				{
					if( menuHelpComponent.RequiresProgress() )
					{
						menuHelpComponent.Progress( Time.deltaTime );
						backToGame = false;
					}
				}
				if( menuMapComponent != null )
				{
					if( menuMapComponent.RequiresProgress() )
					{
						menuMapComponent.Progress( Time.deltaTime );
						backToGame = false;
					}
				}
				if( menuInventoryComponent != null )
				{
					if( menuInventoryComponent.RequiresProgress() )
					{
						menuInventoryComponent.Progress( Time.deltaTime );
						backToGame = false;
					}
				}
				if( pausedQuestionListener != null )
				{
					backToGame = false;
				}
				if( textBoxUIManager != null )
				{
					if( textBoxUIManager.IsDisplayingItem() )
					{
						backToGame = false;
					}
				}
				/*if( menuCreditsComponent != null )
				{
					if( menuCreditsComponent.RequiresProgress() )
					{
						menuCreditsComponent.Progress( Time.fixedDeltaTime );
						backToGame = false;
					}
				}*/
				/*if( (frontMenuComponent != null) && (frontMenuComponent == menuCreditsComponent) )
				{
					/*halmeida - during the credits we will keep running events cause they are
					the only way out of the end-game credits.*/
					/*if( menuCreditsComponent.AutoScrolling() && menuCreditsComponent.ScrollingOver() )
					{
						if( eventManager != null )
						{
							eventManager.Progress( Time.fixedDeltaTime );
						}
					}
				}*/
				if( backToGame )
				{
					/*halmeida - if a new stage was selected during PAUSE, there will be a fade out after
					the menus are closed, so we gotta wait until it ends and the fade speed becomes zero.
					If the game was just unpaused, without a new stage being selected, there will be no
					fade happening after the menus are closed and the speed will also be zero.*/
					backToGame = (overlayFadeSpeed == 0f);
				}
				if( backToGame )
				{
					if( selectedNodeID != GraphNode.NODE_ID_INVALID )
					{
						/*if( eventManager != null )
						{
							eventManager.CloseStageEvents();
						}
						if( currentStageObject != null )
						{
							if( currentStageComponent != null )
							{
								currentStageComponent.Clear();
								if( inputManager != null )
								{
									inputManager.SetChallengeStage( null );
								}
								currentStageComponent = null;
							}
							Destroy( currentStageObject );
							currentStageObject = null;
						}
						currentStageID = selectedStageID;
						selectedStageID = ChallengeStage.INVALID_STAGE_ID;*/
					}
					else
					{
						if( stageComponent != null )
						{
							stageComponent.RequestUnpause( this );
						}
					}
					if( textBoxManager != null )
					{
						textBoxManager.TogglePause( false );
					}
					if( (inputManager != null) && enableKeypadAtResume )
					{
						inputManager.ToggleControls( true, true );
						ignoreBriefly = true;
						/*halmeida - the line above fixes a bug. A very thorough explanation can be found
						within this Update, on the STATUS_GAME part. ;) */
					}
					enableKeypadAtResume = true;
					musicController = MusicController.Instance;
					if( musicController != null )
					{
						musicController.SetTargetVolume( 1f );
					}
					currentStatus = STATUS_GAME;
				}
				break;
		}
	}

	void FixedUpdate()
	{
		switch( currentStatus )
		{
			case STATUS_EDITOR:
				if( challengeEditor != null )
				{
					challengeEditor.FixedProgress( Time.fixedDeltaTime );
				}
				break;
			case STATUS_TITLE_AND_GAME:
				if( stageComponent != null )
				{
					stageComponent.FixedProgress( Time.fixedDeltaTime );
				}
				break;
			case STATUS_GAME:
				if( stageComponent != null )
				{
					stageComponent.FixedProgress( Time.fixedDeltaTime );
				}
				break;
		}
	}

	public void SetSelectedNode( int newSelectedNodeID )
	{
		GraphNode selectedNode = null;

		selectedNodeID = GraphNode.NODE_ID_INVALID;
		selectedNodeSlotRow = -1;
		selectedNodeSlotColumn = -1;
		if( currentStatus == STATUS_TITLE )
		{
			if( nodeSelectionPending )
			{
				nodeSelectionPending = false;
				selectedNodeID = newSelectedNodeID;
			}
		}
		else if( currentStatus == STATUS_PAUSE )
		{
			while( frontMenuComponent != null )
			{
				CloseMenu();
			}
			if( newSelectedNodeID != GraphNode.NODE_ID_INVALID )
			{
				selectedNodeID = newSelectedNodeID;
				if( imageOverlayComponent != null )
				{
					SetOverlayColor( overlayRGB );
					overlayDark = false;
					overlayFadeSpeed = OVERLAY_FADE_SPEED_STAGES;
				}
				else
				{
					overlayClear = false;
					overlayDark = true;
				}
			}
		}
		if( (selectedNodeID != GraphNode.NODE_ID_INVALID) && (graph != null) )
		{
			selectedNode = graph.GetNodeWithID( selectedNodeID );
			if( selectedNode != null )
			{
				if( selectedNode.IsVisited() )
				{
					selectedNodeSlotRow = selectedNode.GetFirstSlotRow();
					selectedNodeSlotColumn = selectedNode.GetFirstSlotColumn();
				}
			}
		}
		if( selectedNodeSlotRow < 0 )
		{
			selectedNodeID = GraphNode.NODE_ID_INVALID;
			selectedNodeSlotRow = -1;
			selectedNodeSlotColumn = -1;
		}
	}

	private void Pause()
	{
		MusicController musicController = null;

		if( !ignoreBriefly && (currentStatus == STATUS_GAME) )
		{
			if( inputManager != null )
			{
				inputManager.ToggleControls( false, false );
			}
			if( stageComponent != null )
			{
				stageComponent.RequestPause( this );
				/*halmeida - the stage will only actually pause when it progresses after the request.*/
			}
			musicController = MusicController.Instance;
			if( musicController != null )
			{
				musicController.SetTargetVolume( MusicController.LOWERED_VOLUME );
			}
			currentStatus = STATUS_PAUSE;
		}
	}

	public bool IsPaused()
	{
		return (currentStatus == STATUS_PAUSE);
	}

	public bool OpenPauseMenu( bool withQuitOption = true )
	{
		if( (menuPauseComponent != null) && (frontMenuComponent == null) )
		{
			/*halmeida - The pause menu can only be the first menu, under all others. The other menus may open
			on top of each other, but the pause menu cannot. This is to avoid cycles, since the pause menu may
			take the player to the other menus and there would be some flux problems if any of these menus was
			already opened under the pause menu.*/
			Pause();
			if( currentStatus == STATUS_PAUSE )
			{
				if( inputManager != null )
				{
					inputManager.SetMenu( menuPauseComponent );
				}
				menuPauseComponent.SetClosureBeforeEffect( false );
				menuPauseComponent.SetPreviousMenu( null );
				menuPauseComponent.BuildVisualRepresentation( withQuitOption );
				frontMenuComponent = menuPauseComponent;
				return menuPauseComponent.RequiresProgress();
			}
		}
		return false;
	}

	public bool OpenMap( bool withQuitOption = true )
	{
		if( (menuMapComponent != null) && (frontMenuComponent != menuMapComponent) )
		{
			Pause();
			if( currentStatus == STATUS_PAUSE )
			{
				if( inputManager != null )
				{
					inputManager.SetMenu( menuMapComponent );
				}
				menuMapComponent.SetPreviousMenu( frontMenuComponent );
				menuMapComponent.BuildVisualRepresentation( withQuitOption );
				frontMenuComponent = menuMapComponent;
				return menuMapComponent.RequiresProgress();
			}
		}
		return false;
	}

	public bool OpenInventory( bool withQuitOption = true )
	{
		if( (menuInventoryComponent != null) && (frontMenuComponent != menuInventoryComponent) )
		{
			Pause();
			if( currentStatus == STATUS_PAUSE )
			{
				if( inputManager != null )
				{
					inputManager.SetMenu( menuInventoryComponent );
				}
				menuInventoryComponent.SetPreviousMenu( frontMenuComponent );
				menuInventoryComponent.BuildVisualRepresentation( withQuitOption );
				frontMenuComponent = menuInventoryComponent;
				return menuInventoryComponent.RequiresProgress();
			}
		}
		return false;
	}

	public void CloseMenu()
	{
		if( (currentStatus == STATUS_PAUSE) && (frontMenuComponent != null) )
		{
			if( frontMenuComponent.RequiresProgress() )
			{
				/*halmeida - this means the front menu passed the choice of closure to this class before
				closing itself and has to be manually cleared.*/
				frontMenuComponent.ClearVisualRepresentation( false );
				/*halmeida - as long as I keep processing this menu it will finish closing itself. I can
				focus on the menu behind it as the new front menu.*/
			}
			frontMenuComponent = frontMenuComponent.GetPreviousMenu();
			if( frontMenuComponent != null )
			{
				/*halmeida - if there was a menu behind, the option that leads the flow to the recently
				closed menu will be in the selected state, so we gotta deselect it to make the menu usefull
				once again.*/
				if( frontMenuComponent.RequiresProgress() )
				{
					frontMenuComponent.DeselectOptionComponent();
				}
			}
			if( inputManager != null )
			{
				inputManager.SetMenu( frontMenuComponent );
			}
		}
	}

	public bool PauseForQuestion( IOptionsListener newQuestionListener )
	{
		if( (pausedQuestionListener == null) && (newQuestionListener != null) )
		{
			Pause();
			if( currentStatus == STATUS_PAUSE )
			{
				pausedQuestionListener = newQuestionListener;
				if( pausedQuestionListener.ListensFromUI() )
				{
					textBoxManager.TogglePause( true );
				}
				/*halmeida - if it's not a UI question, then it is a world space question. In that case, I do not
				pause the TextBoxManager because it has to keep working during the pause to allow the question to
				be answered.*/
				/*halmeida - the TextBoxUIManager is, by design, never paused.*/
				return true;
			}
		}
		return false;
	}

	public bool UnpauseFromQuestion( IOptionsListener currentQuestionListener )
	{
		if( (pausedQuestionListener != null) && (pausedQuestionListener == currentQuestionListener) )
		{
			pausedQuestionListener = null;
			return true;
		}
		return false;
	}

	public bool PauseForUIItemInfo()
	{
		if( textBoxUIManager != null )
		{
			if( textBoxUIManager.IsDisplayingItem() )
			{
				Pause();
				if( currentStatus == STATUS_PAUSE )
				{
					if( textBoxManager != null )
					{
						textBoxManager.TogglePause( true );
					}
					return true;
				}
			}
		}
		return false;
	}

	public PlayerAgent GetAgentCharacter( int agentCharacterIndex )
	{
		PlayerAgent[] npcComponents = null;

		if( agentCharacterIndex == -1 )
		{
			return playerComponent;
		}
		if( (agentCharacterIndex > -1) && (stageComponent != null) )
		{
			npcComponents = stageComponent.GetAllNpcComponents();
			if( npcComponents != null )
			{
				if( npcComponents.Length > agentCharacterIndex )
				{
					return npcComponents[agentCharacterIndex];
				}
			}
		}
		return null;
	}

	public SymbolDatabase GetSymbolDatabaseDialogue()
	{
		return fontDialogue;
	}

	public SymbolDatabase GetSymbolDatabaseDamage()
	{
		return fontDamage;
	}

	public SymbolDatabase GetSymbolDatabaseUI()
	{
		return fontUI;
	}

	public InputManager GetInputManager()
	{
		return inputManager;
	}

	public bool SaveGameProgress( int currentNodeID )
	{
		BinaryFormatter bf = null;
		FileStream file = null;
		bool[] visitedNodes = null;
		int[] nodeFirstSlotRows = null;
		int[] nodeFirstSlotColumns = null;
		int[] nodeSlotWidths = null;
		int[] nodeSlotHeights = null;
		bool[] eventConclusions = null;

		bf = new BinaryFormatter();
		try
		{
			file = File.Create( gameProgressFilePath );
			if( gameProgressData != null )
			{
				gameProgressData.Clear();
				gameProgressData = null;
			}
			gameProgressData = new GamePersistentData();
			if( graph != null )
			{
				gameProgressData.nodeIDs = graph.GetAllNodeIDs( ref visitedNodes, ref nodeFirstSlotRows, ref nodeFirstSlotColumns,
					ref nodeSlotWidths, ref nodeSlotHeights );
				gameProgressData.visitedNodes = visitedNodes;
				gameProgressData.nodeFirstSlotRows = nodeFirstSlotRows;
				gameProgressData.nodeFirstSlotColumns = nodeFirstSlotColumns;
				gameProgressData.nodeSlotWidths = nodeSlotWidths;
				gameProgressData.nodeSlotHeights = nodeSlotHeights;
				gameProgressData.savedNodeID = currentNodeID;
				gameProgressData.areaUnlockPayments = graph.GetAreaUnlockPaymentsCopy();
			}
			if( eventManager != null )
			{
				gameProgressData.eventIDs = eventManager.GetAllEventIDs( ref eventConclusions );
				gameProgressData.eventConclusions = eventConclusions;
			}
			bf.Serialize( file, gameProgressData );
			file.Close();
			Debug.Log("Debug : GameController : game progress saved to \""+gameProgressFilePath+"\".");
		}
		catch( MethodAccessException e )
		{
			Debug.Log("Debug : GameController : game progress save failed. Exception message \""+e.Message+"\".");
			/*halmeida - means it is web player version. Should save another way.*/
			return false;
		}
		catch( Exception e )
		{
			Debug.Log("Debug : GameController : game progress save failed. Exception message \""+e.Message+"\".");
			return false;
		}
		return true;
	}

	/*halmeida - sometimes we can't save the whole state of the game. That's because, for example, some
	events need to happen in a chain within the stage. An event may rely on another event leaving the
	stage at a given state to be able to work properly. Leaving a character at a given position, for
	example. Therefore, we cannot save the conclusion of events at any arbitrary moment and that is
	why it is done only in save points. However, we need some immediate save method for another purposes,
	like recording a purchased item.*/
	public bool SavePlayerProgress( Vector2 offsetWithinNode )
	{
		BinaryFormatter bf = null;
		FileStream file = null;
		bool shootingSkill = false;
		int breakingSkill = -1;

		if( playerComponent == null )
		{
			return false;
		}
		bf = new BinaryFormatter();
		try
		{
			file = File.Create( playerProgressFilePath );
			if( playerProgressData != null )
			{
				playerProgressData.Clear();
				playerProgressData = null;
			}
			playerProgressData = new PlayerPersistentData();
			playerProgressData.playerCoins = playerComponent.GetCurrentCoins();
			playerComponent.GetCurrentSkills( ref shootingSkill, ref breakingSkill );
			playerProgressData.playerSkillShoot = shootingSkill;
			playerProgressData.playerSkillBreak = breakingSkill;
			playerProgressData.playerExperiencePoints = playerComponent.GetTotalExperiencePoints();
			playerProgressData.playerInventoryItemIDs = playerComponent.GetInventoryItemIDsCopy();
			playerProgressData.playerInventoryItemUnits = playerComponent.GetInventoryItemUnitsCopy();
			playerProgressData.playerEquippedItemIDs = playerComponent.GetEquippedItemIDsCopy();
			playerProgressData.playerCurrentName = playerComponent.GetCurrentName();
			playerProgressData.playerOffsetXInNode = offsetWithinNode.x;
			playerProgressData.playerOffsetYInNode = offsetWithinNode.y;
			playerProgressData.playerFacingSide = playerComponent.GetFacingSide();
			bf.Serialize( file, playerProgressData );
			file.Close();
			//Debug.Log("Debug : GameController : player progress saved to \""+playerProgressFilePath+"\".");
		}
		catch( MethodAccessException e )
		{
			Debug.Log("Debug : GameController : player progress save failed. Exception message \""+e.Message+"\".");
			/*halmeida - means it is web player version. Should save another way.*/
			return false;
		}
		catch( Exception e )
		{
			Debug.Log("Debug : GameController : player progress save failed. Exception message \""+e.Message+"\".");
			return false;
		}
		return true;
	}

	private bool LoadGameProgress()
	{
		int[] recoveredNodeIDs =  null;
		bool[] recoveredNodeVisitations = null;
		int[] recoveredNodeFirstRows = null;
		int[] recoveredNodeFirstColumns = null;
		int[] recoveredNodeSlotWidths = null;
		int[] recoveredNodeSlotHeights = null;
		int nodeID = GraphNode.NODE_ID_INVALID;
		bool nodeVisited = false;
		int nodeFirstRow = -1;
		int nodeFirstColumn = -1;
		int nodeSlotWidth = 0;
		int nodeSlotHeight = 0;
		int savedNodeID = GraphNode.NODE_ID_INVALID;
		int savedNodeFirstRow = -1;
		int savedNodeFirstColumn = -1;
		int[] recoveredAreaUnlockPayments = null;
		int[] recoveredEventIDs = null;
		bool[] recoveredEventConclusions = null;

		if( eventManager != null )
		{
			eventManager.Reset();
		}
		LoadGamePersistentData();
		if( gameProgressData != null )
		{
			if( graph != null )
			{
				savedNodeID = gameProgressData.savedNodeID;
				recoveredNodeIDs = gameProgressData.nodeIDs;
				recoveredNodeVisitations = gameProgressData.visitedNodes;
				recoveredNodeFirstRows = gameProgressData.nodeFirstSlotRows;
				recoveredNodeFirstColumns = gameProgressData.nodeFirstSlotColumns;
				recoveredNodeSlotWidths = gameProgressData.nodeSlotWidths;
				recoveredNodeSlotHeights = gameProgressData.nodeSlotHeights;
				if( (recoveredNodeIDs != null) && (recoveredNodeVisitations != null) && (recoveredNodeFirstRows != null) &&
					(recoveredNodeFirstColumns != null) && (recoveredNodeSlotWidths != null) && (recoveredNodeSlotHeights != null) )
				{
					for( int i=0; i<recoveredNodeIDs.Length; i++ )
					{
						nodeID = recoveredNodeIDs[i];
						nodeVisited = false;
						if( recoveredNodeVisitations.Length > i )
						{
							nodeVisited = recoveredNodeVisitations[i];
						}
						nodeFirstRow = -1;
						if( recoveredNodeFirstRows.Length > i )
						{
							nodeFirstRow = recoveredNodeFirstRows[i];
						}
						nodeFirstColumn = -1;
						if( recoveredNodeFirstColumns.Length > i )
						{
							nodeFirstColumn = recoveredNodeFirstColumns[i];
						}
						nodeSlotWidth = 0;
						if( recoveredNodeSlotWidths.Length > i )
						{
							nodeSlotWidth = recoveredNodeSlotWidths[i];
						}
						nodeSlotHeight = 0;
						if( recoveredNodeSlotHeights.Length > i )
						{
							nodeSlotHeight = recoveredNodeSlotHeights[i];
						}
						graph.SetNodeVisitation( nodeID, nodeVisited, nodeFirstRow, nodeFirstColumn, nodeSlotWidth, nodeSlotHeight );
						if( nodeID == savedNodeID )
						{
							savedNodeFirstRow = nodeFirstRow;
							savedNodeFirstColumn = nodeFirstColumn;
						}
					}
				}
				recoveredAreaUnlockPayments = gameProgressData.areaUnlockPayments;
				graph.SetAreaUnlockPayments( recoveredAreaUnlockPayments );
				/*halmeida - the graph raises a flag when a payment unlocks a previously locked area. This is so that
				the stage can change things that are already loaded. In this case, the stage isn't loaded yet, so we
				can dismiss the flag.*/
				graph.DismissUnlockTreatmentRequest();
			}
			if( eventManager != null )
			{
				recoveredEventIDs = gameProgressData.eventIDs;
				recoveredEventConclusions = gameProgressData.eventConclusions;
				eventManager.LoadEventConclusions( recoveredEventIDs, recoveredEventConclusions );
			}
		}
		if( savedNodeID != GraphNode.NODE_ID_INVALID )
		{
			startingNodeID = savedNodeID;
			startingNodeSlotRow = savedNodeFirstRow;
			startingNodeSlotColumn = savedNodeFirstColumn;
			if( forceDestination && (forceDestinationNodeID != GraphNode.NODE_ID_INVALID) )
			{
				startingNodeID = forceDestinationNodeID;
			}
			return true;
		}
		if( graph != null )
		{
			startingNodeID = graph.GetStartNodeID();
			startingNodeSlotRow = graph.startNodeMapRow;
			startingNodeSlotColumn = graph.startNodeMapColumn;
			if( forceDestination && (forceDestinationNodeID != GraphNode.NODE_ID_INVALID) )
			{
				startingNodeID = forceDestinationNodeID;
			}
		}
		return false;
	}

	private void LoadGamePersistentData()
	{
		BinaryFormatter bf = null;
		FileStream file = null;

		if( gameProgressData != null )
		{
			gameProgressData.Clear();
			gameProgressData = null;
		}
		try
		{
			if( File.Exists( gameProgressFilePath ) )
			{
				bf = new BinaryFormatter();
				file = File.Open( gameProgressFilePath, FileMode.Open );
				try
				{
					gameProgressData = (GamePersistentData)bf.Deserialize(file);
				}
				catch( SerializationException e )
				{
					Debug.Log("Debug : GameController : game persistent data load failed. Exception message \""+e.Message+"\".");
					if( gameProgressData != null )
					{
						gameProgressData.Clear();
						gameProgressData = null;
					}
				}
				file.Close();
				if( gameProgressData != null )
				{
					Debug.Log ("Debug : GameController : game persistent data loaded from \""+gameProgressFilePath+"\".");
				}
			}
			else
			{
				Debug.Log ("Debug : GameController : game persistent data load failed. File \""+gameProgressFilePath+"\" does not exist.");
			}
		}
		catch( MethodAccessException e )
		{
			Debug.Log("Debug : GameController : game persistent data load failed. Exception message \""+e.Message+"\".");
			/*halmeida - means it is web player version. Should load another way.*/
		}
		catch( Exception e )
		{
			Debug.Log("Debug : GameController : game persistent data load failed. Exception message \""+e.Message+"\".");
		}
	}

	private void LoadStage()
	{
		if( stageObject == null )
		{
			stageObject = new GameObject( "ChallengeStage" );
			stageComponent = stageObject.AddComponent<ChallengeStage>();
			stageComponent.SetGameAndCamera( this, cameraComponent, interfaceCanvasComponent );
			if( (roomBlockerModel != null) && (roomBlockerObject == null) )
			{
				roomBlockerObject = Instantiate( roomBlockerModel, Vector3.zero, Quaternion.identity ) as GameObject;
				roomBlockerComponent = roomBlockerObject.GetComponent<RoomSightBlocker>();
				if( roomBlockerComponent == null )
				{
					Destroy( roomBlockerObject );
					roomBlockerObject = null;
				}
				else if( menuMapComponent != null )
				{
					roomBlockerComponent.SetMapSize( menuMapComponent.slotsPerMapSide );
				}
			}
			stageComponent.SetBuildingResources( tileFactories, challengeData, graph, roomBlockerComponent );
			stageComponent.SetInputManager( inputManager );
			stageComponent.SetDatabases( itemDatabase, obstacleDatabase, enemyDatabase, fontDialogue, fontDamage, fontUI,
				boxDatabase, backgroundDatabase );
			stageComponent.SetEventManager( eventManager );
		}
	}

	private bool LoadPlayer()
	{
		if( (playerObject == null) && (playerGameModel != null) )
		{
			playerObject = Instantiate( playerGameModel, Vector3.zero, Quaternion.identity ) as GameObject;
			playerComponent = playerObject.GetComponent<PlayerAgent>();
			if( playerComponent == null )
			{
				Destroy( playerObject );
				playerObject = null;
			}
			else
			{
				playerComponent.SetItemDatabase( itemDatabase );
				LoadPlayerProgress();
				if( debug )
				{
					DebugPlayerProgress();
				}
				return true;
			}
		}
		return false;
	}

	private bool LoadPlayerProgress()
	{
		int[] itemIDs = null;
		int[] itemUnits = null;
		int itemID = ItemData.INVALID_ITEM_ID;
		int itemAmount = 0;
		bool shootingSkill = false;
		int breakingSkill = -1;

		LoadPlayerPersistentData();
		if( playerProgressData != null )
		{
			if( playerComponent != null )
			{
				playerComponent.ClearInventory();
				playerComponent.ResetExperience( true );
				playerComponent.SetCurrentCoins( playerProgressData.playerCoins );
				shootingSkill = playerProgressData.playerSkillShoot;
				breakingSkill = playerProgressData.playerSkillBreak;
				playerComponent.SetCurrentSkills( shootingSkill, breakingSkill );
				playerComponent.ReceiveExperience( playerProgressData.playerExperiencePoints, false );
				itemIDs = playerProgressData.playerInventoryItemIDs;
				itemUnits = playerProgressData.playerInventoryItemUnits;
				if( (itemIDs != null) && (itemUnits != null) )
				{
					if( itemIDs.Length == itemUnits.Length )
					{
						for( int i=0; i<itemIDs.Length; i++ )
						{
							itemID = itemIDs[i];
							itemAmount = itemUnits[i];
							for( int j=0; j<itemAmount; j++ )
							{
								playerComponent.ObtainItem( itemID, false );
							}
						}
					}
				}
				itemIDs = playerProgressData.playerEquippedItemIDs;
				if( itemIDs != null )
				{
					for( int i=0; i<itemIDs.Length; i++ )
					{
						playerComponent.EquipItem( itemIDs[i] );
					}
				}
				playerComponent.FillHP( true );
				playerComponent.SetCurrentName( playerProgressData.playerCurrentName );
				if( !forceDestination )
				{
					playerOffsetWithinNode = new Vector2( playerProgressData.playerOffsetXInNode, playerProgressData.playerOffsetYInNode );
					playerFacingSide = playerProgressData.playerFacingSide;
				}
				return true;
			}
		}
		return false;
	}

	private void LoadPlayerPersistentData()
	{
		BinaryFormatter bf = null;
		FileStream file = null;

		if( playerProgressData != null )
		{
			playerProgressData.Clear();
			playerProgressData = null;
		}
		try
		{
			if( File.Exists( playerProgressFilePath ) )
			{
				bf = new BinaryFormatter();
				file = File.Open( playerProgressFilePath, FileMode.Open );
				try
				{
					playerProgressData = (PlayerPersistentData)bf.Deserialize(file);
				}
				catch( SerializationException e )
				{
					Debug.Log("Debug : GameController : player persistent data load failed. Exception message \""+e.Message+"\".");
					if( playerProgressData != null )
					{
						playerProgressData.Clear();
						playerProgressData = null;
					}
				}
				file.Close();
				if( playerProgressData != null )
				{
					Debug.Log ("Debug : GameController : player persistent data loaded from \""+playerProgressFilePath+"\".");
				}
			}
			else
			{
				Debug.Log ("Debug : GameController : player persistent data load failed. File \""+playerProgressFilePath+"\" does not exist.");
			}
		}
		catch( MethodAccessException e )
		{
			Debug.Log("Debug : GameController : player persistent data load failed. Exception message \""+e.Message+"\".");
			/*halmeida - means it is web player version. Should load another way.*/
		}
		catch( Exception e )
		{
			Debug.Log("Debug : GameController : player persistent data load failed. Exception message \""+e.Message+"\".");
		}
	}

	private void DebugPlayerProgress()
	{
		if( playerComponent != null )
		{
			if( debugExtraCoins > 0 )
			{
				playerComponent.SetCurrentCoins( playerComponent.GetCurrentCoins() + debugExtraCoins );
			}
			if( debugSkills )
			{
				playerComponent.SetCurrentSkills( debugSkillShoot, debugSkillBreak );
			}
			if( debugExtraItemIDs != null )
			{
				for( int i=0; i<debugExtraItemIDs.Length; i++ )
				{
					playerComponent.ObtainItem( debugExtraItemIDs[i], false );
				}
			}
		}
	}

	public void EraseGameProgress()
	{
		string dataPathToErase = null;
	
		Debug.Log("Debug : GameController : erasing game progress.");
		dataPathToErase = Application.persistentDataPath;
		if( Directory.Exists ( dataPathToErase ) )
		{
			try
			{
				File.Delete ( gameProgressFilePath );
			}
			catch( Exception e )
			{
				Debug.Log("Debug : GameController : game progress erasure failed. Exception message \""+e.Message+"\".");
			}
		}
	}

	public void ErasePlayerProgress()
	{
		string dataPathToErase = null;
	
		Debug.Log("Debug : GameController : erasing player progress.");
		dataPathToErase = Application.persistentDataPath;
		if( Directory.Exists ( dataPathToErase ) )
		{
			try
			{
				File.Delete ( playerProgressFilePath );
			}
			catch( Exception e )
			{
				Debug.Log("Debug : GameController : player progress erasure failed. Exception message \""+e.Message+"\".");
			}
		}
	}

	public bool ExistsRecordedGameProgress()
	{
		return File.Exists( gameProgressFilePath );
	}

	private void LoadFadeOverlay( bool dark, Vector3 rgb )
	{
		Color layerColor = Color.black;

		if( (interfaceCanvasObject != null) && (imageOverlayObject == null) && (imageOverlayModel != null) )
		{
			imageOverlayObject = Instantiate( imageOverlayModel ) as GameObject;
			imageOverlayObject.transform.SetParent( interfaceCanvasObject.transform, false );
			imageOverlayComponent = imageOverlayObject.GetComponent<Image>();
			if( imageOverlayComponent != null )
			{
				layerColor.r = rgb.x;
				layerColor.g = rgb.y;
				layerColor.b = rgb.z;
				if( dark )
				{
					imageOverlayComponent.color = layerColor;
					overlayDark = true;
					overlayClear = false;
				}
				else
				{
					layerColor.a = 0f;
					imageOverlayComponent.color = layerColor;
					overlayDark = false;
					overlayClear = true;
				}
			}
			else
			{
				Destroy( imageOverlayObject );
				imageOverlayObject = null;
			}
		}
	}

	private void UpdateFadeRequests()
	{
		if( stageComponent != null )
		{
			if( stageComponent.RequestingFadeOut( ref overlayRGB ) )
			{
				SetOverlayColor( overlayRGB );
				if( !overlayDark )
				{
					SetOverlayFadeSpeed( OVERLAY_FADE_SPEED_STAGES );
				}
				else
				{
					stageComponent.ConcludeFadeOut();
				}
			}
			else if( stageComponent.RequestingFadeIn() )
			{
				if( !overlayClear )
				{
					SetOverlayFadeSpeed( -OVERLAY_FADE_SPEED_STAGES );
				}
				else
				{
					stageComponent.ConcludeFadeIn();
				}
			}
		}
	}

	private bool SetOverlayColor( Vector3 newOverlayRgb )
	{
		Color layerColor = Color.black;

		if( imageOverlayComponent != null )
		{
			layerColor.r = newOverlayRgb.x;
			layerColor.g = newOverlayRgb.y;
			layerColor.b = newOverlayRgb.z;
			layerColor.a = imageOverlayComponent.color.a;
			imageOverlayComponent.color = layerColor;
			return true;
		}
		return false;
	}

	private void SetOverlayFadeSpeed( float newOverlayFadeSpeed )
	{
		if( imageOverlayComponent != null )
		{
			overlayFadeSpeed = newOverlayFadeSpeed;
		}
		else
		{
			if( newOverlayFadeSpeed > 0f )
			{
				/*halmeida - trying to turn the overlay opaque, fading out.*/
				overlayDark = true;
				overlayClear = false;
			}
			else if( newOverlayFadeSpeed < 0f )
			{
				/*halmeida - trying to turn the overlay transparent, fading in.*/
				overlayDark = false;
				overlayClear = true;
			}
			overlayFadeSpeed = 0f;
		}
	}

	private bool LoadTitle( int titleType )
	{
		Color titleColor = Color.black;
		GameObject titleModel = null;

		switch( titleType )
		{
			case TITLE_TYPE_STUDIO:
				titleModel = imageStudioTitleModel;
				break;
			case TITLE_TYPE_GAME:
				titleModel = imageGameTitleModel;
				break;
			default:
				titleModel = null;
				break;
		}
		if( (interfaceCanvasObject != null) && (imageTitleObject == null) && (titleModel != null) )
		{
			imageTitleObject = Instantiate( titleModel ) as GameObject;
			imageTitleComponent = imageTitleObject.GetComponent<Image>();
			if( imageTitleComponent != null )
			{
				titleColor = imageTitleComponent.color;
				titleColor.a = 0f;
				imageTitleComponent.color = titleColor;
				imageTitleObject.transform.SetParent( interfaceCanvasObject.transform, false );
				return true;
			}
			else
			{
				Destroy( imageTitleObject );
				imageTitleObject = null;
			}
		}
		return false;
	}

	private void UpdateRunningFades( float timeStep )
	{
		Color color = Color.black;
		float alpha = 0f;

		if( imageTitleComponent != null )
		{
			if( !titleFadeOver )
			{
				if( titleFadeSpeed != 0f )
				{
					color = imageTitleComponent.color;
					alpha = color.a;
					alpha += titleFadeSpeed * timeStep;
					if( titleFadeSpeed > 0f )
					{
						if( alpha >= 1f )
						{
							alpha = 1f;
							titleFadeOver = true;
						}
					}
					else
					{
						if( alpha <= 0f )
						{
							alpha = 0f;
							titleFadeOver = true;
						}
					}
					color.a = alpha;
					imageTitleComponent.color = color;
				}
			}
			else
			{
				if( titleDuration > 0f )
				{
					titleElapsed += timeStep;
				}
			}
		}
		if( imageOverlayComponent != null )
		{
			if( overlayFadeSpeed != 0f )
			{
				color = imageOverlayComponent.color;
				alpha = color.a;
				alpha += overlayFadeSpeed * timeStep;
				if( overlayFadeSpeed > 0f )
				{
					overlayClear = false;
					if( alpha >= 1f )
					{
						alpha = 1f;
						overlayDark = true;
						overlayFadeSpeed = 0f;
					}
				}
				else
				{
					overlayDark = false;
					if( alpha <= 0f )
					{
						alpha = 0f;
						overlayClear = true;
						overlayFadeSpeed = 0f;
					}
				}
				color.a = alpha;
				imageOverlayComponent.color = color;
			}
		}
	}

	private void LoadTextDatabase()
	{
		string pathFromResources = null;
		int indexOfFirstCharacter = -1;

		if( gameTextLanguage == INVALID_LANGUAGE_CODE )
		{
			gameTextLanguage = LANGUAGE_CODE_EN;
		}
		pathFromResources = DIRECTORY_PATH_GAME_TEXT;
		indexOfFirstCharacter = pathFromResources.LastIndexOf( "/Resources/", System.StringComparison.Ordinal );
		if( indexOfFirstCharacter > -1 )
		{
			pathFromResources = pathFromResources.Substring( indexOfFirstCharacter + 11 );
		}
		switch( gameTextLanguage )
		{
			case LANGUAGE_CODE_EN:
				pathFromResources += FILE_NAME_GAME_TEXT_EN;
				break;
			case LANGUAGE_CODE_PT:
				pathFromResources += FILE_NAME_GAME_TEXT_PT;
				break;
		}
		gameTextDatabase = GameTextDatabase.Instance;
		if( gameTextDatabase != null )
		{
			gameTextDatabase.LoadFromResource( pathFromResources );
		}
	}

	private void LoadAllChallengeData()
	{
		string pathFromResources = null;
		int indexOfFirstCharacter = -1;
		float oldTime = 0f;

		oldTime = Time.realtimeSinceStartup;
		pathFromResources = ChallengeEditor.DIRECTORY_PATH_CHALLENGES;
		indexOfFirstCharacter = pathFromResources.LastIndexOf( "/Resources/", System.StringComparison.Ordinal );
		if( indexOfFirstCharacter > -1 )
		{
			pathFromResources = pathFromResources.Substring( indexOfFirstCharacter + 11 );
		}
		pathFromResources += ChallengeEditor.DIRECTORY_NAME_CHALLENGES;
		Debug.Log("Debug : ChallengeGame : pathFromResources = \""+pathFromResources+"\".");
		Debug.Log("Debug : ChallengeGame : time to build correct path string = "+(Time.realtimeSinceStartup - oldTime)+".");
		oldTime = Time.realtimeSinceStartup;
		LoadAllChallengeTextAssets( pathFromResources );
		Debug.Log("Debug : ChallengeGame : time to load all challenge text assets = "+(Time.realtimeSinceStartup - oldTime)+".");
		oldTime = Time.realtimeSinceStartup;
		ExtractAllDataFromTextAssets();
		Debug.Log("Debug : ChallengeGame : time to extract data from text assets = "+(Time.realtimeSinceStartup - oldTime)+".");
		oldTime = Time.realtimeSinceStartup;
		UnloadAllChallengeTextAssets();
		Debug.Log("Debug : ChallengeGame : time to unload all challenge text assets = "+(Time.realtimeSinceStartup - oldTime)+".");
	}

	private void LoadAllChallengeTextAssets( string pathFromResources )
	{
		UnityEngine.Object[] assetsInFolder = null;
		int totalAssetsInFolder = 0;

		if( challengeTextAssets == null )
		{
			assetsInFolder = Resources.LoadAll( pathFromResources );
			if( assetsInFolder != null )
			{
				totalAssetsInFolder = assetsInFolder.Length;
				challengeTextAssets = new TextAsset[totalAssetsInFolder];
				for( int i=0; i<totalAssetsInFolder; i++ )
				{
					challengeTextAssets[i] = (TextAsset)assetsInFolder[i];
				}
			}
		}
	}

	private void ExtractAllDataFromTextAssets()
	{
		StructurePersistentData data = null;

		if( (challengeTextAssets != null) && (challengeData == null) )
		{
			for( int i=0; i<challengeTextAssets.Length; i++ )
			{
				data = ChallengeStructure.GetPersistentDataFromTextAsset( challengeTextAssets[i] );
				if( data != null )
				{
					UsefulFunctions.IncreaseArray<StructurePersistentData>( ref challengeData, data );
				}
			}
		}
	}

	public void UnloadAllChallengeTextAssets()
	{
		TextAsset challengeTextAsset = null;

		if( challengeTextAssets != null )
		{
			for( int i=0; i<challengeTextAssets.Length; i++ )
			{
				challengeTextAsset = challengeTextAssets[i];
				if( challengeTextAsset != null )
				{
					Resources.UnloadAsset( challengeTextAsset );
				}
				challengeTextAsset = null;
			}
			challengeTextAssets = null;
		}
	}

	private bool LoadChallengeEditor()
	{
		if( (challengeEditorModel != null) && (challengeEditorObject == null) && (challengeEditor == null) )
		{
			challengeEditorObject = Instantiate( challengeEditorModel, Vector3.zero, Quaternion.identity ) as GameObject;
			if( challengeEditorObject != null )
			{
				challengeEditor = challengeEditorObject.GetComponent<ChallengeEditor>();
				if( challengeEditor != null )
				{
					challengeEditor.SetCameraObject( cameraObject );
					challengeEditor.SetTileFactories( tileFactories );
					challengeEditor.SetInterfaceCanvas( interfaceCanvasObject );
					challengeEditor.SetInputManager( inputManager );
					challengeEditor.SetPlayerObjectModel( playerEditorModel );
					challengeEditor.SetItemDatabase( itemDatabase );
					challengeEditor.SetEnemyDatabase( enemyDatabase );
					return true;
				}
				Destroy( challengeEditorObject );
				challengeEditorObject = null;
			}
		}
		return false;
	}

	private void CalculateFPS( float newFrameDuration )
	{
		RectTransform rectTrans = null;
		float fpsAnchorX = 0f;
		float fpsAnchorY = 0f;

		fpsFrames++;
		fpsTime += newFrameDuration;
		if( fpsFrames >= AVERAGE_FRAMES )
		{
			if( (fpsObject == null) && (interfaceCanvasTrans != null) )
			{
				fpsObject = new GameObject("FPSDisplay", typeof(RectTransform));
				fpsSpritedString = fpsObject.AddComponent<SpritedStringUI>();
				fpsSpritedString.SetSymbolSource( fontFPS );
				fpsSpritedString.ToggleRaycastTargeting( false );
				fpsSpritedString.SetValue("FPS 00.0000");
				rectTrans = fpsObject.GetComponent<RectTransform>();
				if( rectTrans != null )
				{
					rectTrans.SetParent( interfaceCanvasTrans, false );
					fpsAnchorX = interfaceCanvasRect.width / 2f - rectTrans.rect.width / 2f;
					fpsAnchorY = -interfaceCanvasRect.height / 2f + rectTrans.rect.height / 2f;
					rectTrans.anchoredPosition = new Vector2( fpsAnchorX, fpsAnchorY );
				}
			}
			fps = 1f / (fpsTime / fpsFrames);
			if( fpsSpritedString != null )
			{
				fpsSpritedString.SetValue("FPS "+fps);
			}
			fpsFrames = 0;
			fpsTime = 0f;
		}
	}

	private void ExtractCanvasProperties()
	{
		if( (interfaceCanvasComponent != null) && (interfaceCanvasTrans == null) )
		{
			interfaceCanvasTrans = interfaceCanvasObject.GetComponent<RectTransform>();
			if( interfaceCanvasTrans != null )
			{
				interfaceCanvasRect = interfaceCanvasTrans.rect;
				Debug.Log("Debug : GameController : interfaceCanvasRect = ("+interfaceCanvasRect.width+", "+interfaceCanvasRect.height+").");
				interfaceCanvasScreenRect = interfaceCanvasComponent.pixelRect;
				Debug.Log("Debug : GameController : interfaceCanvasScreenRect = ("+interfaceCanvasScreenRect.width+", "+interfaceCanvasScreenRect.height+").");
			}
			else
			{
				interfaceCanvasComponent = null;
			}
		}
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
