using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using System; //to use Int32.TryParse and String methods

public class ChallengeEditor : MonoBehaviour
{
	public const int SCREEN_INVALID = -1;
	public const int SCREEN_EDIT_CHALLENGE = 0;
	public const int SCREEN_SAVE_CHALLENGE = 1;
	public const int SCREEN_LOAD_CHALLENGE = 2;
	public const int SCREEN_EDIT_WAYPOINT = 3;

	public const string DIRECTORY_PATH_CHALLENGES = "Assets/Resources/";
	public const string DIRECTORY_NAME_CHALLENGES = "Challenges";

	public const float OVERLAY_COLOR_R = 1f;
	public const float OVERLAY_COLOR_G = 1f;
	public const float OVERLAY_COLOR_B = 1f;
	public const float OVERLAY_COLOR_A = 0.4f;

	public const float FAR_CAMERA_ORTHO_FACTOR = 1.8f;

	public GameObject tileDividerModel;
	public GameObject sizeMarkerModel;
	public GameObject spawnMarkerModel;
	public GameObject colliderMarkerModel;
	public GameObject waypointMarkerModel;
	public GameObject imagePrefabOverlay;
	public GameObject imagePrefabPanel;
	public GameObject imagePrefabPanelCodes;
	public GameObject imagePrefabPanelName;
	public GameObject inputPrefabID;
	public GameObject inputPrefabDifficulty;
	public GameObject textPrefabCurrentID;
	public GameObject textPrefabID;
	public GameObject textPrefabDifficulty;
	public GameObject buttonPrefabSave;
	public GameObject buttonPrefabLoad;
	public GameObject buttonPrefabConfirm;
	public GameObject buttonPrefabCancel;
	public GameObject buttonPrefabDividers;
	public GameObject buttonPrefabPurpose;
	public GameObject buttonPrefabDecrease;
	public GameObject buttonPrefabIncrease;
	public GameObject togglePrefabMirror;
	public GameObject togglePrefabDelete;
	public int indexTileFactoryFront;
	public int indexTileFactoryBack;

	private GameObject playerObjectModel;
	private TileFactory[] tileFactories;
	private int factoryIndexFront;
	private int factoryIndexBack;
	private string challengesDirectory;
	private GameObject interfaceCanvasObject;
	private GameObject imageObjectOverlay;
	private Image imageComponentOverlay;
	private GameObject imageObjectPanel;
	private GameObject imageObjectPanelCodes;
	private GameObject imageObjectPanelName;
	private Text textComponentPanelID;
	private Text textComponentPanelIDSufix;
	private Text textComponentPanelDifficulty;
	private Text textComponentPanelCodes;
	private Text textComponentPanelName;
	private GameObject textObjectCurrentID;
	private GameObject textObjectID;
	private GameObject textObjectDifficulty;
	private Text textComponentCurrentID;
	private Text textComponentID;
	private Text textComponentDifficulty;
	private GameObject inputObjectID;
	private GameObject inputObjectDifficulty;
	private InputField inputComponentID;
	private InputField inputComponentDifficulty;
	private GameObject buttonObjectSave;
	private GameObject buttonObjectLoad;
	private GameObject buttonObjectConfirm;
	private GameObject buttonObjectCancel;
	private GameObject buttonObjectDividers;
	private GameObject buttonObjectPurpose;
	private GameObject buttonObjectDecrease;
	private GameObject buttonObjectIncrease;
	private Button buttonComponentSave;
	private Button buttonComponentLoad;
	private Button buttonComponentConfirm;
	private Button buttonComponentCancel;
	private Button buttonComponentDividers;
	private Button buttonComponentPurpose;
	private Button buttonComponentDecrease;
	private Button buttonComponentIncrease;
	private GameObject toggleObjectMirror;
	private GameObject toggleObjectDelete;
	private Toggle toggleComponentMirror;
	private Toggle toggleComponentDelete;
	private UnityAction actionSave;
	private UnityAction actionLoad;
	private UnityAction actionConfirm;
	private UnityAction actionCancel;
	private UnityAction actionDividers;
	private UnityAction actionPurpose;
	private UnityAction actionDecrease;
	private UnityAction actionIncrease;
	private UnityAction<bool> actionToggleMirror;
	private UnityAction<bool> actionToggleDelete;
	private bool createMirror;
	private bool deleteObject;
	private int currentScreen;
	private GameObject cameraObject;
	private ChallengeCanvas challengeCanvas;
	private InputManager inputManager;
	private Color overlayColor;
	private bool panelDataLoaded;
	private GameTextDatabase gameTextDatabase;
	private ItemDatabase itemDatabase;
	private EnemyDatabase enemyDatabase;
	private Text textComponentButtonPurpose;
	private bool farCamera;
	private int challengeID;
	private int challengeIDSufix;
	private int challengeDifficulty;

	void Awake()
	{
		playerObjectModel = null;
		tileFactories = null;
		factoryIndexFront = indexTileFactoryFront;
		factoryIndexBack = indexTileFactoryBack;
		challengesDirectory = DIRECTORY_PATH_CHALLENGES + DIRECTORY_NAME_CHALLENGES;
		interfaceCanvasObject = null;
		imageObjectOverlay = null;
		imageComponentOverlay = null;
		imageObjectPanel = null;
		imageObjectPanelCodes = null;
		imageObjectPanelName = null;
		textComponentPanelID = null;
		textComponentPanelIDSufix = null;
		textComponentPanelDifficulty = null;
		textComponentPanelCodes = null;
		textComponentPanelName = null;
		textObjectCurrentID = null;
		textObjectID = null;
		textObjectDifficulty = null;
		textComponentCurrentID = null;
		textComponentID = null;
		textComponentDifficulty = null;
		inputObjectID = null;
		inputObjectDifficulty = null;
		inputComponentID = null;
		inputComponentDifficulty = null;
		buttonObjectSave = null;
		buttonObjectLoad = null;
		buttonObjectConfirm = null;
		buttonObjectCancel = null;
		buttonObjectDividers = null;
		buttonObjectPurpose = null;
		buttonObjectDecrease = null;
		buttonObjectIncrease = null;
		buttonComponentSave = null;
		buttonComponentLoad = null;
		buttonComponentConfirm = null;
		buttonComponentCancel = null;
		buttonComponentDividers = null;
		buttonComponentPurpose = null;
		buttonComponentDecrease = null;
		buttonComponentIncrease = null;
		toggleObjectMirror = null;
		toggleObjectDelete = null;
		toggleComponentMirror = null;
		toggleComponentDelete = null;
		actionSave = null;
		actionLoad = null;
		actionConfirm = null;
		actionCancel = null;
		actionDividers = null;
		actionPurpose = null;
		actionDecrease = null;
		actionIncrease = null;
		actionToggleMirror = null;
		actionToggleDelete = null;
		createMirror = false;
		deleteObject = false;
		currentScreen = SCREEN_INVALID;
		cameraObject = null;
		challengeCanvas = null;
		inputManager = null;
		overlayColor = new Color( OVERLAY_COLOR_R, OVERLAY_COLOR_G, OVERLAY_COLOR_B, OVERLAY_COLOR_A );
		panelDataLoaded = false;
		gameTextDatabase = GameTextDatabase.Instance;
		itemDatabase = null;
		enemyDatabase = null;
		textComponentButtonPurpose = null;
		farCamera = false;
		challengeID = ChallengeStructure.STRUCTURE_ID_INVALID;
		challengeIDSufix = -1;
		challengeDifficulty = ChallengeStructure.UNDEFINED_DIFFICULTY;
	}

	public void SetCameraObject( GameObject newCameraObject )
	{
		cameraObject =  newCameraObject;
	}

	public void SetTileFactories( TileFactory[] newTileFactories )
	{
		tileFactories = newTileFactories;
		if( tileFactories != null )
		{
			if( factoryIndexFront >= tileFactories.Length )
			{
				factoryIndexFront = 0;
			}
			if( factoryIndexBack >= tileFactories.Length )
			{
				if( tileFactories.Length > 1 )
				{
					factoryIndexBack = 1;
				}
				else
				{
					factoryIndexBack = 0;
				}
			}
		}
		else
		{
			factoryIndexFront = -1;
			factoryIndexBack = -1;
		}
	}

	public void SetInterfaceCanvas( GameObject newInterfaceCanvasObject )
	{
		interfaceCanvasObject = newInterfaceCanvasObject;
	}

	public void SetInputManager( InputManager newInputManager )
	{
		inputManager = newInputManager;
		if( inputManager != null )
		{
			inputManager.SetChallengeEditor( this );
		}
	}

	public void SetPlayerObjectModel( GameObject newPlayerModel )
	{
		playerObjectModel = newPlayerModel;
	}

	public void SetItemDatabase( ItemDatabase newItemDatabase )
	{
		itemDatabase = newItemDatabase;
	}

	public void SetEnemyDatabase( EnemyDatabase newEnemyDatabase )
	{
		enemyDatabase = newEnemyDatabase;
	}

	public void Progress( float timeStep )
	{
		switch( currentScreen )
		{
			case SCREEN_INVALID:
				LoadScreenEditChallenge();
				currentScreen = SCREEN_EDIT_CHALLENGE;
				break;
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					challengeCanvas.Progress( timeStep );
					if( !panelDataLoaded )
					{
						if( challengeCanvas.GetDataFromLoadedStructure( ref challengeID, false, ref challengeIDSufix, ref challengeDifficulty ) )
						{
							if( textComponentPanelID != null )
							{
								textComponentPanelID.text = "ID "+challengeID;
							}
							if( textComponentPanelIDSufix != null )
							{
								textComponentPanelIDSufix.text = "SUF "+challengeIDSufix;
							}
							if( textComponentPanelDifficulty != null )
							{
								textComponentPanelDifficulty.text = "DIF "+challengeDifficulty;
							}
							panelDataLoaded = true;
						}
					}
				}
				break;
			default:
				break;
		}
	}

	public void FixedProgress( float timeStep )
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					challengeCanvas.FixedProgress( timeStep );
				}
				break;
		}
	}

	public void OnClickSave()
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				LoadScreenSaveChallenge();
				currentScreen = SCREEN_SAVE_CHALLENGE;
				break;
			default:
				break;
		}
	}

	public void OnClickLoad()
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				LoadScreenLoadChallenge();
				currentScreen = SCREEN_LOAD_CHALLENGE;
				break;
			default:
				break;
		}
	}

	public void OnClickWaypoint( int waypointCurrentID )
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				LoadScreenEditWaypoint( waypointCurrentID );
				currentScreen = SCREEN_EDIT_WAYPOINT;
				break;
			default:
				break;
		}
	}

	public void OnClickConfirm()
	{
		int tempChallengeID = ChallengeStructure.STRUCTURE_ID_INVALID;
		int tempChallengeIDSufix = -1;
		int tempChallengeDifficulty = ChallengeStructure.UNDEFINED_DIFFICULTY;
		string challengeIDString = null;
		string challengeDifficultyString = null;
		string waypointIDString = null;
		int waypointIDInt = Waypoint.INVALID_WAYPOINT_ID;

		switch( currentScreen )
		{
			case SCREEN_SAVE_CHALLENGE:
				if( (inputComponentID != null) && (inputComponentDifficulty != null) )
				{
					if( inputComponentID.text != null )
					{
						challengeIDString = inputComponentID.text.Trim();
						if( !String.IsNullOrEmpty( challengeIDString ) )
						{
							if( Int32.TryParse( challengeIDString, out tempChallengeID ) )
							{
								if( inputComponentDifficulty.text != null )
								{
									challengeDifficultyString = inputComponentDifficulty.text.Trim();
									if( !String.IsNullOrEmpty( challengeDifficultyString ) )
									{
										if( !Int32.TryParse( challengeDifficultyString, out tempChallengeDifficulty ) )
										{
											Debug.Log("Debug : ChallengeEditor : Difficulty value inserted is invalid.");
											tempChallengeDifficulty = ChallengeStructure.UNDEFINED_DIFFICULTY;
										}
									}
								}
								if( !Directory.Exists( challengesDirectory ) )
								{
									Directory.CreateDirectory( challengesDirectory );
								}
								if( challengeCanvas != null )
								{
									if( challengeCanvas.SaveChallengeStructure( challengesDirectory, tempChallengeID,
										tempChallengeDifficulty, createMirror ) )
									{
										LoadScreenSaveChallenge( false );
										currentScreen = SCREEN_EDIT_CHALLENGE;
										if(	challengeCanvas.GetDataFromSavedStructure( ref tempChallengeID, false, ref tempChallengeIDSufix,
											ref tempChallengeDifficulty ) )
										{
											challengeID = tempChallengeID;
											challengeIDSufix = tempChallengeIDSufix;
											challengeDifficulty = tempChallengeDifficulty;
											if( textComponentPanelID != null )
											{
												textComponentPanelID.text = "ID "+challengeID;
											}
											if( textComponentPanelIDSufix != null )
											{
												textComponentPanelIDSufix.text = "SUF "+challengeIDSufix;
											}
											if( textComponentPanelDifficulty != null )
											{
												textComponentPanelDifficulty.text = "DIF "+challengeDifficulty;
											}
										}
									}
								}
							}
							else
							{
								Debug.Log("Debug : ChallengeEditor : ID value inserted is invalid.");
							}
						}
					}
				}
				break;
			case SCREEN_LOAD_CHALLENGE:
				if( inputComponentID != null )
				{
					if( inputComponentID.text != null )
					{
						challengeIDString = inputComponentID.text.Trim();
						if( !String.IsNullOrEmpty( challengeIDString ) )
						{
							if( challengeCanvas != null )
							{
								if( challengeCanvas.LoadChallengeStructure( challengesDirectory+"/"+challengeIDString ) )
								{
									LoadScreenLoadChallenge( false );
									currentScreen = SCREEN_EDIT_CHALLENGE;
									if( challengeCanvas.GetDataFromLoadedStructure( ref tempChallengeID, false, ref tempChallengeIDSufix,
										ref tempChallengeDifficulty ) )
									{
										challengeID = tempChallengeID;
										challengeIDSufix = tempChallengeIDSufix;
										challengeDifficulty = tempChallengeDifficulty;
										if( textComponentPanelID != null )
										{
											textComponentPanelID.text = "ID "+challengeID;
										}
										if( textComponentPanelIDSufix != null )
										{
											textComponentPanelIDSufix.text = "SUF "+challengeIDSufix;
										}
										if( textComponentPanelDifficulty != null )
										{
											textComponentPanelDifficulty.text = "DIF "+challengeDifficulty;
										}
										panelDataLoaded = true;
									}
									else
									{
										panelDataLoaded = false;
									}
								}
							}
						}
					}
				}
				break;
			case SCREEN_EDIT_WAYPOINT:
				if( deleteObject )
				{
					if( challengeCanvas != null )
					{
						challengeCanvas.EndWaypointAlteration( true, true, waypointIDInt );
						LoadScreenEditWaypoint( -1, false );
						currentScreen = SCREEN_EDIT_CHALLENGE;
					}
				}
				else
				{
					if( inputComponentID != null )
					{
						if( inputComponentID.text != null )
						{
							waypointIDString = inputComponentID.text.Trim();
							if( !String.IsNullOrEmpty( waypointIDString ) )
							{
								if( Int32.TryParse( waypointIDString, out waypointIDInt ) )
								{
									if( challengeCanvas != null )
									{
										/*halmeida - pass the value to the challengeCanvas to give it to the selected waypoint.*/
										challengeCanvas.EndWaypointAlteration( true, false, waypointIDInt );
										LoadScreenEditWaypoint( -1, false );
										currentScreen = SCREEN_EDIT_CHALLENGE;
									}
								}
							}
						}
					}
				}
				break;
			default:
				break;
		}
	}

	public void OnClickCancel()
	{
		switch( currentScreen )
		{
			case SCREEN_SAVE_CHALLENGE:
				LoadScreenSaveChallenge( false );
				currentScreen = SCREEN_EDIT_CHALLENGE;
				break;
			case SCREEN_LOAD_CHALLENGE:
				LoadScreenLoadChallenge( false );
				currentScreen = SCREEN_EDIT_CHALLENGE;
				break;
			case SCREEN_EDIT_WAYPOINT:
				if( challengeCanvas != null )
				{
					challengeCanvas.EndWaypointAlteration( false, false, -1 );
				}
				LoadScreenEditWaypoint( -1, false );
				currentScreen = SCREEN_EDIT_CHALLENGE;
				break;
			default:
				break;
		}
	}

	public void OnClickDividers()
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					challengeCanvas.ToggleTileDividers();
				}
				break;
			default:
				break;
		}
	}

	public void OnClickPurpose()
	{
		int currentPurpose = ChallengeCanvas.PURPOSE_INVALID;
		string itemName = null;

		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					challengeCanvas.AdvancePurpose();
					currentPurpose = challengeCanvas.GetCurrentPurpose();
					if( textComponentButtonPurpose != null )
					{
						textComponentButtonPurpose.text = ChallengeCanvas.GetPurposeInitial( currentPurpose );
					}
					if( imageObjectPanelName != null )
					{
						imageObjectPanelName.SetActive( (currentPurpose == ChallengeCanvas.PURPOSE_EDIT_ITEMS) ||
							(currentPurpose == ChallengeCanvas.PURPOSE_EDIT_ENEMIES) );
					}
					switch( currentPurpose )
					{
						case ChallengeCanvas.PURPOSE_EDIT_TILES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Var. "+challengeCanvas.GetCurrentTileVariation();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_BACK_TILES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Var. "+challengeCanvas.GetCurrentTileVariation();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ITEMS:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentItemID()+
									" Cluster "+challengeCanvas.GetCurrentItemClusterIndex();
							}
							if( textComponentPanelName != null )
							{
								if( gameTextDatabase != null )
								{
									gameTextDatabase.GetItemDescription( challengeCanvas.GetCurrentItemID(), ref itemName );
									textComponentPanelName.text = itemName;
								}
								else
								{
									textComponentPanelName.text = null;
								}
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ITEM_CLUSTERS:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentItemID()+
									" Cluster "+challengeCanvas.GetCurrentItemClusterIndex();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ENEMIES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentEnemyCode();
							}
							if( !challengeCanvas.IsEditingEnemyRoute() )
							{
								if( (textComponentPanelName != null) && (enemyDatabase != null) )
								{
									textComponentPanelName.text = enemyDatabase.GetEnemyName( challengeCanvas.GetCurrentEnemyCode() );
								}
							}
							else
							{
								if( textComponentPanelName != null )
								{
									textComponentPanelName.text = "-empty route-";
								}
							}
							break;
						default:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "---";
							}
							break;
					}
				}
				break;
			default:
				break;
		}
	}

	public void OnClickDecrease()
	{
		int currentPurpose = ChallengeCanvas.PURPOSE_INVALID;
		string itemName = null;

		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					currentPurpose = challengeCanvas.GetCurrentPurpose();
					challengeCanvas.ChangeCurrentCode( false );
					switch( currentPurpose )
					{
						case ChallengeCanvas.PURPOSE_EDIT_TILES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Var. "+challengeCanvas.GetCurrentTileVariation();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_BACK_TILES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Var. "+challengeCanvas.GetCurrentTileVariation();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ITEMS:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentItemID()+
									" Cluster "+challengeCanvas.GetCurrentItemClusterIndex();
							}
							if( textComponentPanelName != null )
							{
								if( gameTextDatabase != null )
								{
									gameTextDatabase.GetItemDescription( challengeCanvas.GetCurrentItemID(), ref itemName );
									textComponentPanelName.text = itemName;
								}
								else
								{
									textComponentPanelName.text = null;
								}
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ITEM_CLUSTERS:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentItemID()+
									" Cluster "+challengeCanvas.GetCurrentItemClusterIndex();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ENEMIES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentEnemyCode();
							}
							if( !challengeCanvas.IsEditingEnemyRoute() )
							{
								if( (textComponentPanelName != null) && (enemyDatabase != null) )
								{
									textComponentPanelName.text = enemyDatabase.GetEnemyName( challengeCanvas.GetCurrentEnemyCode() );
								}
							}
							break;
						default:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "---";
							}
							break;
					}
				}
				break;
			default:
				break;
		}
	}

	public void OnClickIncrease()
	{
		int currentPurpose = ChallengeCanvas.PURPOSE_INVALID;
		string itemName = null;

		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					currentPurpose = challengeCanvas.GetCurrentPurpose();
					challengeCanvas.ChangeCurrentCode( true );
					switch( currentPurpose )
					{
						case ChallengeCanvas.PURPOSE_EDIT_TILES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Var. "+challengeCanvas.GetCurrentTileVariation();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_BACK_TILES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Var. "+challengeCanvas.GetCurrentTileVariation();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ITEMS:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentItemID()+
									" Cluster "+challengeCanvas.GetCurrentItemClusterIndex();
							}
							if( textComponentPanelName != null )
							{
								if( gameTextDatabase != null )
								{
									gameTextDatabase.GetItemDescription( challengeCanvas.GetCurrentItemID(), ref itemName );
									textComponentPanelName.text = itemName;
								}
								else
								{
									textComponentPanelName.text = null;
								}
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ITEM_CLUSTERS:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentItemID()+
									" Cluster "+challengeCanvas.GetCurrentItemClusterIndex();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ENEMIES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentEnemyCode();
							}
							if( !challengeCanvas.IsEditingEnemyRoute() )
							{
								if( (textComponentPanelName != null) && (enemyDatabase != null) )
								{
									textComponentPanelName.text = enemyDatabase.GetEnemyName( challengeCanvas.GetCurrentEnemyCode() );
								}
							}
							break;
						default:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "---";
							}
							break;
					}
				}
				break;
			default:
				break;
		}
	}

	public void OnEditRoute( int[] waypointIndexes, bool closeRouteEdition )
	{
		if( (currentScreen == SCREEN_EDIT_CHALLENGE) && (challengeCanvas != null) )
		{
			if( challengeCanvas.GetCurrentPurpose() == ChallengeCanvas.PURPOSE_EDIT_ENEMIES )
			{
				if( !closeRouteEdition )
				{
					if( challengeCanvas.IsEditingEnemyRoute() )
					{
						if( textComponentPanelName != null )
						{
							textComponentPanelName.text = "Route "+UsefulFunctions.StringFromArray<int>(", ", waypointIndexes);
						}
					}
				}
				else
				{
					if( textComponentPanelCodes != null )
					{
						textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentEnemyCode();
					}
					if( !challengeCanvas.IsEditingEnemyRoute() )
					{
						if( (textComponentPanelName != null) && (enemyDatabase != null) )
						{
							textComponentPanelName.text = enemyDatabase.GetEnemyName( challengeCanvas.GetCurrentEnemyCode() );
						}
					}
				}
			}
		}
	}

	public void OnToggleMirror( bool value )
	{
		switch( currentScreen )
		{
			case SCREEN_SAVE_CHALLENGE:
				createMirror = !createMirror;
				break;
			default:
				break;
		}
	}

	public void OnToggleDelete( bool value )
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_WAYPOINT:
				deleteObject = !deleteObject;
				break;
			default:
				break;
		}
	}

	public void ReactToSaveKey()
	{
		OnClickSave();
	}

	public void ReactToLoadKey()
	{
		OnClickLoad();
	}

	public void ReactToReturnKeyDown()
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					challengeCanvas.ReactToReturnKeyDown();
				}
				break;
			case SCREEN_SAVE_CHALLENGE:
				OnClickConfirm();
				break;
			case SCREEN_LOAD_CHALLENGE:
				OnClickConfirm();
				break;
			case SCREEN_EDIT_WAYPOINT:
				OnClickConfirm();
				break;
			default:
				break;
		}
	}

	public void ReactToEscapeKeyDown()
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					challengeCanvas.ReactToEscapeKeyDown();
				}
				break;
			case SCREEN_SAVE_CHALLENGE:
				OnClickCancel();
				break;
			case SCREEN_LOAD_CHALLENGE:
				OnClickCancel();
				break;
			case SCREEN_EDIT_WAYPOINT:
				OnClickCancel();
				break;
			default:
				break;
		}
	}

	public void ReactToSpaceKey( bool down )
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					challengeCanvas.ReactToSpaceKey( down );
				}
				break;
			default:
				break;
		}
	}

	public void ReactToFarCameraKey( bool down )
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( down )
				{
					farCamera = !farCamera;
					if( farCamera )
					{
						if( challengeCanvas != null )
						{
							challengeCanvas.SetCameraFurther( true, FAR_CAMERA_ORTHO_FACTOR );
						}
						if( inputManager != null )
						{
							inputManager.AlterInputArea( FAR_CAMERA_ORTHO_FACTOR, FAR_CAMERA_ORTHO_FACTOR );
						}
					}
					else
					{
						if( challengeCanvas != null )
						{
							challengeCanvas.SetCameraFurther( false );
						}
						if( inputManager != null )
						{
							inputManager.ResetInputArea();
						}
					}
				}
				break;
			default:
				break;
		}
	}

	public void ReactToPointerDown( int screenPointX, int screenPointY )
	{
		switch( currentScreen )
		{
			case SCREEN_EDIT_CHALLENGE:
				if( challengeCanvas != null )
				{
					challengeCanvas.ReactToPointerDown( screenPointX, screenPointY );
					if( challengeCanvas.GetCurrentPurpose() == ChallengeCanvas.PURPOSE_EDIT_ITEM_CLUSTERS )
					{
						if( textComponentPanelCodes != null )
						{
							textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentItemID()+
								" Cluster "+challengeCanvas.GetCurrentItemClusterIndex();
						}
					}
				}
				break;
			default:
				break;
		}
	}

	public void ReactToPointerUp()
	{
		if( challengeCanvas != null )
		{
			challengeCanvas.ReactToPointerUp();
		}
	}

	private void LoadScreenEditChallenge( bool load = true )
	{
		Image tempImageComponent = null;
		Transform tempTextTransform = null;
		GameObject tempTextObject = null;
		int currentPurpose = ChallengeCanvas.PURPOSE_INVALID;
		Navigation navigation = default( Navigation );
		string itemName = null;

		if( load )
		{
			/*halmeida - we will set the navigation mode to none in every button, so that the focus is not kept at the
			button after we click it. Without this setting, after you click a button, any press of the spacebar or the
			enter button triggers the clicked button again.*/
			navigation = new Navigation();
			navigation.mode = Navigation.Mode.None;
			if( UICreator.CreateButton( buttonPrefabSave, ref buttonObjectSave, ref buttonComponentSave, interfaceCanvasObject ) )
			{
				actionSave = new UnityAction( OnClickSave );
				buttonComponentSave.onClick.AddListener( actionSave );
			}
			if( UICreator.CreateButton( buttonPrefabLoad, ref buttonObjectLoad, ref buttonComponentLoad, interfaceCanvasObject ) )
			{
				actionLoad = new UnityAction( OnClickLoad );
				buttonComponentLoad.onClick.AddListener( actionLoad );
			}
			if( UICreator.CreateButton( buttonPrefabDividers, ref buttonObjectDividers, ref buttonComponentDividers, interfaceCanvasObject ) )
			{
				actionDividers = new UnityAction( OnClickDividers );
				buttonComponentDividers.onClick.AddListener( actionDividers );
				buttonComponentDividers.navigation = navigation;
			}
			if( UICreator.CreateImage( imagePrefabPanel, ref imageObjectPanel, ref tempImageComponent, interfaceCanvasObject ) )
			{
				tempTextTransform = imageObjectPanel.transform.Find( "TextPanelID" );
				if( tempTextTransform != null )
				{
					tempTextObject = tempTextTransform.gameObject;
					textComponentPanelID = tempTextObject.GetComponent<Text>();
					if( textComponentPanelID != null )
					{
						textComponentPanelID.text = "ID -";
					}
				}
				tempTextTransform = imageObjectPanel.transform.Find( "TextPanelIDSufix" );
				if( tempTextTransform != null )
				{
					tempTextObject = tempTextTransform.gameObject;
					textComponentPanelIDSufix = tempTextObject.GetComponent<Text>();
					if( textComponentPanelIDSufix != null )
					{
						textComponentPanelIDSufix.text = "SUF -";
					}
				}
				tempTextTransform = imageObjectPanel.transform.Find( "TextPanelDifficulty" );
				if( tempTextTransform != null )
				{
					tempTextObject = tempTextTransform.gameObject;
					textComponentPanelDifficulty = tempTextObject.GetComponent<Text>();
					if( textComponentPanelDifficulty != null )
					{
						textComponentPanelDifficulty.text = "DIF -";
					}
				}
			}
			if( UICreator.CreateButton( buttonPrefabPurpose, ref buttonObjectPurpose, ref buttonComponentPurpose, interfaceCanvasObject ) )
			{
				actionPurpose = new UnityAction( OnClickPurpose );
				buttonComponentPurpose.onClick.AddListener( actionPurpose );
				buttonComponentPurpose.navigation = navigation;
				tempTextTransform = buttonObjectPurpose.transform.Find( "TextPurposeInitial" );
				if( tempTextTransform != null )
				{
					tempTextObject = tempTextTransform.gameObject;
					textComponentButtonPurpose = tempTextObject.GetComponent<Text>();
					if( textComponentButtonPurpose != null )
					{
						textComponentButtonPurpose.text = "-";
					}
				}
			}
			if( UICreator.CreateButton( buttonPrefabDecrease, ref buttonObjectDecrease, ref buttonComponentDecrease, interfaceCanvasObject ) )
			{
				actionDecrease = new UnityAction( OnClickDecrease );
				buttonComponentDecrease.onClick.AddListener( actionDecrease );
				buttonComponentDecrease.navigation = navigation;
			}
			if( UICreator.CreateButton( buttonPrefabIncrease, ref buttonObjectIncrease, ref buttonComponentIncrease, interfaceCanvasObject ) )
			{
				actionIncrease = new UnityAction( OnClickIncrease );
				buttonComponentIncrease.onClick.AddListener( actionIncrease );
				buttonComponentIncrease.navigation = navigation;
			}
			if( UICreator.CreateImage( imagePrefabPanelCodes, ref imageObjectPanelCodes, ref tempImageComponent, interfaceCanvasObject ) )
			{
				tempTextTransform = imageObjectPanelCodes.transform.Find( "TextPanelCodes" );
				if( tempTextTransform != null )
				{
					tempTextObject = tempTextTransform.gameObject;
					textComponentPanelCodes = tempTextObject.GetComponent<Text>();
					if( textComponentPanelCodes != null )
					{
						textComponentPanelCodes.text = "---";
					}
				}
			}
			if( UICreator.CreateImage( imagePrefabPanelName, ref imageObjectPanelName, ref tempImageComponent, interfaceCanvasObject ) )
			{
				tempTextTransform = imageObjectPanelName.transform.Find( "TextPanelName" );
				if( tempTextTransform != null )
				{
					tempTextObject = tempTextTransform.gameObject;
					textComponentPanelName = tempTextObject.GetComponent<Text>();
					if( textComponentPanelName != null )
					{
						textComponentPanelName.text = "---";
					}
				}
				imageObjectPanelName.SetActive( false );
			}
			if( challengeCanvas == null )
			{
				challengeCanvas = gameObject.AddComponent<ChallengeCanvas>();
				if( challengeCanvas != null )
				{
					challengeCanvas.SetCameraObject( cameraObject, interfaceCanvasObject );
					challengeCanvas.SetModels( tileDividerModel, sizeMarkerModel, spawnMarkerModel, playerObjectModel,
						colliderMarkerModel, waypointMarkerModel );
					challengeCanvas.SetTileFactories( tileFactories, factoryIndexFront, factoryIndexBack );
					challengeCanvas.SetItemDatabase( itemDatabase );
					challengeCanvas.SetEnemyDatabase( enemyDatabase );
					challengeCanvas.SetChallengeEditor( this );
					currentPurpose = challengeCanvas.GetCurrentPurpose();
					if( textComponentButtonPurpose != null )
					{
						textComponentButtonPurpose.text = ChallengeCanvas.GetPurposeInitial( currentPurpose );
					}
					switch( currentPurpose )
					{
						case ChallengeCanvas.PURPOSE_EDIT_TILES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Var. "+challengeCanvas.GetCurrentTileVariation();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_BACK_TILES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Var. "+challengeCanvas.GetCurrentTileVariation();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ITEMS:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentItemID()+
									" Cluster "+challengeCanvas.GetCurrentItemClusterIndex();
							}
							if( imageObjectPanelName != null )
							{
								imageObjectPanelName.SetActive( true );
							}
							if( textComponentPanelName != null )
							{
								if( gameTextDatabase != null )
								{
									gameTextDatabase.GetItemDescription( challengeCanvas.GetCurrentItemID(), ref itemName );
									textComponentPanelName.text = itemName;
								}
								else
								{
									textComponentPanelName.text = null;
								}
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ITEM_CLUSTERS:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentItemID()+
									" Cluster "+challengeCanvas.GetCurrentItemClusterIndex();
							}
							break;
						case ChallengeCanvas.PURPOSE_EDIT_ENEMIES:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "Code "+challengeCanvas.GetCurrentEnemyCode();
							}
							if( imageObjectPanelName != null )
							{
								imageObjectPanelName.SetActive( true );
							}
							if( (textComponentPanelName != null) && (enemyDatabase != null) )
							{
								textComponentPanelName.text = enemyDatabase.GetEnemyName( challengeCanvas.GetCurrentEnemyCode() );
							}
							break;
						default:
							if( textComponentPanelCodes != null )
							{
								textComponentPanelCodes.text = "---";
							}
							break;
					}
				}
			}
			if( inputManager != null )
			{
				inputManager.SetChallengeCanvas( challengeCanvas );
			}
		}
		else
		{
			UICreator.DestroyButton( ref buttonObjectSave, ref buttonComponentSave, ref actionSave );
			UICreator.DestroyButton( ref buttonObjectLoad, ref buttonComponentLoad, ref actionLoad );
			UICreator.DestroyButton( ref buttonObjectDividers, ref buttonComponentDividers, ref actionDividers );
			textComponentPanelID = null;
			textComponentPanelIDSufix = null;
			textComponentPanelDifficulty = null;
			UICreator.DestroyImage( ref imageObjectPanel, ref tempImageComponent );
			textComponentButtonPurpose = null;
			UICreator.DestroyButton( ref buttonObjectPurpose, ref buttonComponentPurpose, ref actionPurpose );
			UICreator.DestroyButton( ref buttonObjectDecrease, ref buttonComponentDecrease, ref actionDecrease );
			UICreator.DestroyButton( ref buttonObjectIncrease, ref buttonComponentIncrease, ref actionIncrease );
			textComponentPanelCodes = null;
			UICreator.DestroyImage( ref imageObjectPanelCodes, ref tempImageComponent );
			textComponentPanelName = null;
			UICreator.DestroyImage( ref imageObjectPanelName, ref tempImageComponent );
		}
	}

	private void LoadScreenSaveChallenge( bool load = true )
	{
		if( load )
		{
			if( UICreator.CreateImage( imagePrefabOverlay, ref imageObjectOverlay, ref imageComponentOverlay, interfaceCanvasObject ) )
			{
				imageComponentOverlay.color = overlayColor;
			}
			UICreator.CreateText( textPrefabID, ref textObjectID, ref textComponentID, interfaceCanvasObject );
			UICreator.CreateText( textPrefabDifficulty, ref textObjectDifficulty, ref textComponentDifficulty, interfaceCanvasObject );
			if( UICreator.CreateInputField( inputPrefabID, ref inputObjectID, ref inputComponentID, interfaceCanvasObject ) )
			{
				inputComponentID.text = ""+challengeID;
			}
			if( UICreator.CreateInputField( inputPrefabDifficulty, ref inputObjectDifficulty, ref inputComponentDifficulty, interfaceCanvasObject ) )
			{
				inputComponentDifficulty.text = "0";
			}
			if( UICreator.CreateToggle( togglePrefabMirror, ref toggleObjectMirror, ref toggleComponentMirror, interfaceCanvasObject ) )
			{
				createMirror = toggleComponentMirror.isOn;
				actionToggleMirror = new UnityAction<bool>( OnToggleMirror );
				toggleComponentMirror.onValueChanged.AddListener( actionToggleMirror );
			}
			if( UICreator.CreateButton( buttonPrefabConfirm, ref buttonObjectConfirm, ref buttonComponentConfirm, interfaceCanvasObject ) )
			{
				actionConfirm = new UnityAction( OnClickConfirm );
				buttonComponentConfirm.onClick.AddListener( actionConfirm );
			}
			if( UICreator.CreateButton( buttonPrefabCancel, ref buttonObjectCancel, ref buttonComponentCancel, interfaceCanvasObject ) )
			{
				actionCancel = new UnityAction( OnClickCancel );
				buttonComponentCancel.onClick.AddListener( actionCancel );
			}
			if( challengeCanvas != null )
			{
				challengeCanvas.SetInputActive( false );
			}
			if( inputComponentID != null )
			{
				inputComponentID.ActivateInputField();
				inputComponentID.Select();
			}
		}
		else
		{
			UICreator.DestroyImage( ref imageObjectOverlay, ref imageComponentOverlay );
			UICreator.DestroyText( ref textObjectID, ref textComponentID );
			UICreator.DestroyText( ref textObjectDifficulty, ref textComponentDifficulty );
			UICreator.DestroyInputField( ref inputObjectID, ref inputComponentID );
			UICreator.DestroyInputField( ref inputObjectDifficulty, ref inputComponentDifficulty );
			UICreator.DestroyToggle( ref toggleObjectMirror, ref toggleComponentMirror, ref actionToggleMirror );
			UICreator.DestroyButton( ref buttonObjectConfirm, ref buttonComponentConfirm, ref actionConfirm );
			UICreator.DestroyButton( ref buttonObjectCancel, ref buttonComponentCancel, ref actionCancel );
			if( challengeCanvas != null )
			{
				challengeCanvas.SetInputActive( true );
			}
		}
	}

	private void LoadScreenLoadChallenge( bool load = true )
	{
		if( load )
		{
			if( UICreator.CreateImage( imagePrefabOverlay, ref imageObjectOverlay, ref imageComponentOverlay, interfaceCanvasObject ) )
			{
				imageComponentOverlay.color = overlayColor;
			}
			UICreator.CreateText( textPrefabID, ref textObjectID, ref textComponentID, interfaceCanvasObject );
			UICreator.CreateInputField( inputPrefabID, ref inputObjectID, ref inputComponentID, interfaceCanvasObject );
			if( UICreator.CreateButton( buttonPrefabConfirm, ref buttonObjectConfirm, ref buttonComponentConfirm, interfaceCanvasObject ) )
			{
				actionConfirm = new UnityAction( OnClickConfirm );
				buttonComponentConfirm.onClick.AddListener( actionConfirm );
			}
			if( UICreator.CreateButton( buttonPrefabCancel, ref buttonObjectCancel, ref buttonComponentCancel, interfaceCanvasObject ) )
			{
				actionCancel = new UnityAction( OnClickCancel );
				buttonComponentCancel.onClick.AddListener( actionCancel );
			}
			if( challengeCanvas != null )
			{
				challengeCanvas.SetInputActive( false );
			}
			if( inputComponentID != null )
			{
				inputComponentID.ActivateInputField();
				inputComponentID.Select();
			}
		}
		else
		{
			UICreator.DestroyImage( ref imageObjectOverlay, ref imageComponentOverlay );
			UICreator.DestroyText( ref textObjectID, ref textComponentID );
			UICreator.DestroyInputField( ref inputObjectID, ref inputComponentID );
			UICreator.DestroyButton( ref buttonObjectConfirm, ref buttonComponentConfirm, ref actionConfirm );
			UICreator.DestroyButton( ref buttonObjectCancel, ref buttonComponentCancel, ref actionCancel );
			if( challengeCanvas != null )
			{
				challengeCanvas.SetInputActive( true );
			}
		}
	}

	private void LoadScreenEditWaypoint( int waypointCurrentID, bool load = true )
	{
		if( load )
		{
			if( UICreator.CreateImage( imagePrefabOverlay, ref imageObjectOverlay, ref imageComponentOverlay, interfaceCanvasObject ) )
			{
				imageComponentOverlay.color = overlayColor;
			}
			UICreator.CreateText( textPrefabCurrentID, ref textObjectCurrentID, ref textComponentCurrentID, interfaceCanvasObject );
			if( textComponentCurrentID != null )
			{
				textComponentCurrentID.text = textComponentCurrentID.text + " " + waypointCurrentID;
			}
			UICreator.CreateText( textPrefabID, ref textObjectID, ref textComponentID, interfaceCanvasObject );
			UICreator.CreateInputField( inputPrefabID, ref inputObjectID, ref inputComponentID, interfaceCanvasObject );
			if( UICreator.CreateToggle( togglePrefabDelete, ref toggleObjectDelete, ref toggleComponentDelete, interfaceCanvasObject ) )
			{
				deleteObject = toggleComponentDelete.isOn;
				actionToggleDelete = new UnityAction<bool>( OnToggleDelete );
				toggleComponentDelete.onValueChanged.AddListener( actionToggleDelete );
			}
			if( UICreator.CreateButton( buttonPrefabConfirm, ref buttonObjectConfirm, ref buttonComponentConfirm, interfaceCanvasObject ) )
			{
				actionConfirm = new UnityAction( OnClickConfirm );
				buttonComponentConfirm.onClick.AddListener( actionConfirm );
			}
			if( UICreator.CreateButton( buttonPrefabCancel, ref buttonObjectCancel, ref buttonComponentCancel, interfaceCanvasObject ) )
			{
				actionCancel = new UnityAction( OnClickCancel );
				buttonComponentCancel.onClick.AddListener( actionCancel );
			}
			if( challengeCanvas != null )
			{
				challengeCanvas.SetInputActive( false );
			}
			if( inputComponentID != null )
			{
				inputComponentID.ActivateInputField();
				inputComponentID.Select();
			}
		}
		else
		{
			UICreator.DestroyImage( ref imageObjectOverlay, ref imageComponentOverlay );
			UICreator.DestroyText( ref textObjectCurrentID,  ref textComponentCurrentID );
			UICreator.DestroyText( ref textObjectID, ref textComponentID );
			UICreator.DestroyInputField( ref inputObjectID, ref inputComponentID );
			UICreator.DestroyToggle( ref toggleObjectDelete, ref toggleComponentDelete, ref actionToggleDelete );
			UICreator.DestroyButton( ref buttonObjectConfirm, ref buttonComponentConfirm, ref actionConfirm );
			UICreator.DestroyButton( ref buttonObjectCancel, ref buttonComponentCancel, ref actionCancel );
			if( challengeCanvas != null )
			{
				challengeCanvas.SetInputActive( true );
			}
		}
	}
}
