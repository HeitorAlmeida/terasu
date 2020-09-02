using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class InputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public const float STICK_ACTIVE_RADIUS = 10f;
	public const float STICK_MAX_RADIUS = 130f;
	
	public GameObject stickBackgroundModel;
	public GameObject stickForegroundModel;
	public GameObject buttonPrefabPause;
	public bool moveStickBase;

	private GameObject cameraObject;
	private Camera cameraComponent;
	private CameraController cameraController;
	private BoxCollider2D inputCollider;
	private float inputColliderOriWidth;
	private float inputColliderOriHeight;
	private float inputColliderAltWidth;
	private float inputColliderAltHeight;
	private bool inputColliderAltered;
	private bool colliderValid;
	private GameController gameController;
	private ChallengeEditor challengeEditor;
	private ChallengeCanvas challengeCanvas;
	private ChallengeStage challengeStage;
	private MenuSimple menu;
	private TextBoxUIManager textBoxUIManager;
	private TextBoxManager textBoxManager;
	private TransactionBoxManager transactionBoxManager;
	private bool loadVirtualKeypad;
	private GameObject interfaceCanvasObject;
	private Canvas interfaceCanvas;
	private Rect interfaceCanvasScreenRect;
	private float halfCanvasScreenWidth;
	private float halfCanvasScreenHeight;
	private float interfaceCanvasFactor;
	private GameObject stickLeftBackgroundObject;
	private GameObject stickLeftForegroundObject;
	private GameObject stickRightBackgroundObject;
	private GameObject stickRightForegroundObject;
	private RectTransform stickLeftBackgroundTrans;
	private RectTransform stickLeftForegroundTrans;
	private RectTransform stickRightBackgroundTrans;
	private RectTransform stickRightForegroundTrans;
	private Image stickLeftBackgroundImage;
	private Image stickLeftForegroundImage;
	private Image stickRightBackgroundImage;
	private Image stickRightForegroundImage;
	private GameObject buttonObjectPause;
	private Button buttonComponentPause;
	private EventTrigger buttonTriggerPause;
	private Image buttonImagePause;
	private UnityAction<BaseEventData> actionPressPause;
	private bool visualBlockRequested;
	private bool functionalBlockRequested;
	private int[] blockerEventIDs;
	private bool controlsEnabled;
	private bool virtualKeypadVisible;
	private int cameraPixelWidth;
	private int cameraPixelHeight;
	private bool draggingLeftStick;
	private bool draggingRightStick;
	private Vector2 stickLeftNeutralPos;
	private Vector2 stickLeftTiltedPos;
	private Vector2 stickLeftDirection;
	private Vector2 stickRightNeutralPos;
	private Vector2 stickRightTiltedPos;
	private Vector2 stickRightDirection;
	private float stickActiveRadius;
	private float stickActiveRadiusSqr;
	private float stickMaxRadius;
	private float stickMaxRadiusSqr;

	void Awake()
	{
		cameraObject = null;
		cameraComponent = null;
		cameraController = null;
		inputCollider = gameObject.GetComponent<BoxCollider2D>();
		inputColliderOriWidth = 0f;
		inputColliderOriHeight = 0f;
		inputColliderAltWidth = 0f;
		inputColliderAltHeight = 0f;
		inputColliderAltered = false;
		colliderValid = false;
		gameController = null;
		challengeEditor = null;
		challengeCanvas = null;
		challengeStage = null;
		menu = null;
		textBoxUIManager = null;
		textBoxManager = null;
		transactionBoxManager = null;
		loadVirtualKeypad = false;
		#if UNITY_ANDROID && !UNITY_EDITOR
			loadVirtualKeypad = true;
		#endif
		interfaceCanvasObject = null;
		interfaceCanvas = null;
		interfaceCanvasScreenRect = new Rect( 0f, 0f, 0f, 0f );
		halfCanvasScreenWidth = 0f;
		halfCanvasScreenHeight = 0f;
		interfaceCanvasFactor = 0f;
		stickLeftBackgroundObject = null;
		stickLeftForegroundObject = null;
		stickRightBackgroundObject = null;
		stickRightForegroundObject = null;
		stickLeftBackgroundTrans = null;
		stickLeftForegroundTrans = null;
		stickRightBackgroundTrans = null;
		stickRightForegroundTrans = null;
		stickLeftBackgroundImage = null;
		stickLeftForegroundImage = null;
		stickRightBackgroundImage = null;
		stickRightForegroundImage = null;
		buttonObjectPause = null;
		buttonComponentPause = null;
		buttonTriggerPause = null;
		buttonImagePause = null;
		actionPressPause = null;
		visualBlockRequested = false;
		functionalBlockRequested = false;
		blockerEventIDs = null;
		controlsEnabled = false;
		virtualKeypadVisible = false;
		cameraPixelWidth = 0;
		cameraPixelHeight = 0;
		draggingLeftStick = false;
		draggingRightStick = false;
		stickLeftNeutralPos = Vector2.zero;
		stickLeftTiltedPos = Vector2.zero;
		stickLeftDirection = Vector2.zero;
		stickRightNeutralPos = Vector2.zero;
		stickRightTiltedPos = Vector2.zero;
		stickRightDirection = Vector2.zero;
		stickActiveRadius = STICK_ACTIVE_RADIUS;
		stickActiveRadiusSqr = stickActiveRadius * stickActiveRadius;
		stickMaxRadius = STICK_MAX_RADIUS;
		if( stickMaxRadius < stickActiveRadius )
		{
			stickMaxRadius = stickActiveRadius;
		}
		stickMaxRadiusSqr = stickMaxRadius * stickMaxRadius;
	}

	public bool SetCameraObject( GameObject newCameraObject )
	{
		float cameraMaxWorldWidth = 0f;
		float cameraMaxWorldHeight = 0f;

		if( newCameraObject != null )
		{
			cameraObject = newCameraObject;
			cameraComponent = cameraObject.GetComponent<Camera>();
			cameraController = cameraObject.GetComponent<CameraController>();
			if( (cameraComponent != null) && (cameraController != null) && (inputCollider != null) )
			{
				cameraController.GetCameraPixelDimensions( ref cameraPixelWidth, ref cameraPixelHeight );
				if( cameraController.GetCameraMaxWorldDimensions( ref cameraMaxWorldWidth, ref cameraMaxWorldHeight ) )
				{
					if( (cameraMaxWorldWidth > 0f) && (cameraMaxWorldHeight > 0f) )
					{
						inputColliderOriWidth = cameraMaxWorldWidth;
						inputColliderOriHeight = cameraMaxWorldHeight;
						inputCollider.size = new Vector2( inputColliderOriWidth, inputColliderOriHeight );
						gameObject.transform.SetParent( cameraObject.transform );
						gameObject.transform.localPosition = new Vector3( 0f, 0f, cameraComponent.nearClipPlane + 1f );
						colliderValid = true;
						return true;
					}
				}
			}
		}
		Debug.Log("Debug : InputManager : dimensions not ready for copy.");
		cameraController = null;
		cameraComponent = null;
		cameraObject = null;
		return false;
	}

	public void SetGameController( GameController newGameController, Canvas newInterfaceCanvas )
	{
		RectTransform interfaceCanvasTransform = null;

		gameController = newGameController;
		interfaceCanvas = newInterfaceCanvas;
		if( interfaceCanvas != null )
		{
			interfaceCanvasObject = interfaceCanvas.gameObject;
			interfaceCanvasTransform = interfaceCanvasObject.GetComponent<RectTransform>();
			if( interfaceCanvasTransform != null )
			{
				interfaceCanvasScreenRect = interfaceCanvas.pixelRect;
				halfCanvasScreenWidth = interfaceCanvasScreenRect.width / 2f;
				halfCanvasScreenHeight = interfaceCanvasScreenRect.height / 2f;
				interfaceCanvasFactor = interfaceCanvas.scaleFactor;
				if( interfaceCanvasFactor > 0f )
				{
					return;
				}
			}
		}
		interfaceCanvasObject = null;
		interfaceCanvasTransform = null;
		interfaceCanvas = null;
	}

	public void SetChallengeEditor( ChallengeEditor newChallengeEditor )
	{
		challengeEditor = newChallengeEditor;
	}

	public void SetChallengeCanvas( ChallengeCanvas newChallengeCanvas )
	{
		challengeCanvas = newChallengeCanvas;
	}

	public void SetChallengeStage( ChallengeStage newChallengeStage )
	{
		challengeStage = newChallengeStage;
	}

	public void SetMenu( MenuSimple newMenu )
	{
		menu = newMenu;
	}

	public void SetBoxManagers( TextBoxUIManager newTextBoxUIManager, TextBoxManager newTextBoxManager, TransactionBoxManager newTransactionBoxManager )
	{
		textBoxUIManager = newTextBoxUIManager;
		textBoxManager = newTextBoxManager;
		transactionBoxManager = newTransactionBoxManager;
	}

	public void CreateControls()
	{
		if( interfaceCanvasObject != null )
		{
			if( loadVirtualKeypad )
			{
				/*if( (buttonPrefabAction != null) && (buttonObjectAction == null) )
				{
					if( UICreator.CreateButton( buttonPrefabAction, ref buttonObjectAction, ref buttonComponentAction, interfaceCanvasObject ) )
					{
						actionPressAction = new UnityAction<BaseEventData>( OnPressAction );
						AddListenerToObject( buttonObjectAction, ref buttonTriggerAction, EventTriggerType.PointerDown, actionPressAction );
						actionReleaseAction = new UnityAction<BaseEventData>( OnReleaseAction );
						AddListenerToObject( buttonObjectAction, ref buttonTriggerAction, EventTriggerType.PointerUp, actionReleaseAction );
						AddListenerToObject( buttonObjectAction, ref buttonTriggerAction, EventTriggerType.PointerExit, actionReleaseAction );
						buttonImageAction = buttonObjectAction.GetComponent<Image>();
					}
				}*/
				if( (buttonPrefabPause != null) && (buttonObjectPause == null) )
				{
					if( UICreator.CreateButton( buttonPrefabPause, ref buttonObjectPause, ref buttonComponentPause, interfaceCanvasObject ) )
					{
						actionPressPause = new UnityAction<BaseEventData>( OnPressPause );
						AddListenerToObject( buttonObjectPause, ref buttonTriggerPause, EventTriggerType.PointerDown, actionPressPause );
						AddListenerToObject( buttonObjectPause, ref buttonTriggerPause, EventTriggerType.PointerEnter, actionPressPause );
						buttonImagePause = buttonObjectPause.GetComponent<Image>();
					}
				}
			}
			/*halmeida - if there are no blocking conditions, enable the keypad.*/
			ToggleControls( true, true );
		}
	}

	private void AddListenerToObject( GameObject target, ref EventTrigger triggerComponent, EventTriggerType triggerType,
	UnityAction<BaseEventData> action )
	{
		EventTrigger.Entry entry = null;

		if( (target != null) && (action != null) )
		{
			if( triggerComponent == null )
			{
				triggerComponent = target.GetComponent<EventTrigger>();
			}
			if( triggerComponent != null )
			{
				entry = new EventTrigger.Entry();
				entry.eventID = triggerType;
				entry.callback.AddListener( action );
				triggerComponent.triggers.Add( entry );
			}
		}
	}

	public void ToggleControls( bool functionality, bool visibility )
	{
		functionalBlockRequested = !functionality;
		UpdateFunctionalBlock();
		ApplyFunctionalBlock();
		visualBlockRequested = !visibility;
		UpdateVisualBlock();
		ApplyVisualBlock();
	}

	public void AddBlockerEvent( int eventID )
	{
		if( blockerEventIDs != null )
		{
			for( int i=0; i<blockerEventIDs.Length; i++ )
			{
				if( blockerEventIDs[i] == eventID )
				{
					return;
				}
			}
		}
		UsefulFunctions.IncreaseArray<int>( ref blockerEventIDs, eventID );
		UpdateFunctionalBlock();
		UpdateVisualBlock();
		ApplyFunctionalBlock();
		ApplyVisualBlock();
	}

	public void RemoveBlockerEvent( int eventID )
	{
		if( blockerEventIDs != null )
		{
			for( int i=0; i<blockerEventIDs.Length; i++ )
			{
				if( blockerEventIDs[i] == eventID )
				{
					UsefulFunctions.DecreaseArray<int>( ref blockerEventIDs, i );
					UpdateFunctionalBlock();
					UpdateVisualBlock();
					ApplyFunctionalBlock();
					ApplyVisualBlock();
					break;
				}
			}
		}
	}

	private void UpdateFunctionalBlock()
	{
		controlsEnabled = !functionalBlockRequested;
		if( controlsEnabled && (blockerEventIDs != null) )
		{
			controlsEnabled = !(blockerEventIDs.Length > 0);
		}
	}

	private void UpdateVisualBlock()
	{
		virtualKeypadVisible = !visualBlockRequested;
		if( virtualKeypadVisible && (blockerEventIDs != null) )
		{
			virtualKeypadVisible = !(blockerEventIDs.Length > 0);
		}
	}

	private void ApplyFunctionalBlock()
	{
		if( !controlsEnabled )
		{
			if( buttonTriggerPause != null )
			{
				buttonTriggerPause.enabled = false;
			}
			if( buttonImagePause != null )
			{
				buttonImagePause.raycastTarget = false;
			}
			if( challengeStage != null )
			{
				//Debug.Log("Debug : InputManager : ApplyFunctionalBlock, releasing all keys.");
				ReleaseLeftStick();
				ReleaseRightStick();
				challengeStage.ReleaseAllKeys( -1 );
			}
		}
		else
		{
			if( buttonTriggerPause != null )
			{
				buttonTriggerPause.enabled = true;
			}
			if( buttonImagePause != null )
			{
				buttonImagePause.raycastTarget = true;
			}
		}
	}

	private void ApplyVisualBlock()
	{
		if( !virtualKeypadVisible )
		{
			if( buttonImagePause != null )
			{
				buttonImagePause.enabled = false;
			}
			if( challengeStage != null )
			{
				challengeStage.ToggleGaugeVisibility( -1, false );
			}
		}
		else
		{
			if( buttonImagePause != null )
			{
				buttonImagePause.enabled = true;
			}
			if( challengeStage != null )
			{
				challengeStage.ToggleGaugeVisibility( -1, true );
			}
		}
	}

	public void OnPointerDown( PointerEventData eventData )
	{
		float offsetX = 0f;
		float offsetY = 0f;
		bool intercepted = false;
		Vector2 canvasPosition = Vector2.zero;
		int touchX = 0;
		int touchY = 0;
		bool rightHalf = false;

		if( colliderValid && (eventData != null) )
		{
			Debug.Log("Debug : InputManager : OnPointerDown : position = "+eventData.position+".");
			touchX = (int)eventData.position.x;
			touchY = (int)eventData.position.y;
			if( challengeEditor != null )
			{
				challengeEditor.ReactToPointerDown( touchX, touchY );
			}
			else
			{
				/*halmeida - some elements are expecting the input coordinates in screen space while others are
				expecting them in canvas space. "eventData.position" contains the screen space coordinates that
				we will use to calculate the canvas space ones.*/
				if( interfaceCanvas != null )
				{
					offsetX = eventData.position.x - halfCanvasScreenWidth;
					rightHalf = (offsetX > 0f);
					offsetY = eventData.position.y - halfCanvasScreenHeight;
					offsetX /= interfaceCanvasFactor;
					offsetY /= interfaceCanvasFactor;
					canvasPosition = new Vector2( offsetX, offsetY );
				}
				if( menu != null )
				{
					if( interfaceCanvas != null )
					{
						menu.ReactToPointerDown( canvasPosition );
					}
				}
				else
				{
					if( textBoxUIManager != null )
					{
						if( interfaceCanvas != null )
						{
							intercepted = textBoxUIManager.InterceptPointerDown( canvasPosition );
						}
					}
					if( !intercepted && (textBoxManager != null) )
					{
						intercepted = textBoxManager.InterceptPointerDown( touchX, touchY );
					}
					if( !intercepted )
					{
						if( (interfaceCanvas != null) && controlsEnabled )
						{
							if( !rightHalf )
							{
								if( loadVirtualKeypad )
								{
									ResetLeftStick( canvasPosition );
								}
							}
							else
							{
								if( transactionBoxManager != null )
								{
									intercepted = transactionBoxManager.InterceptPointerDown( touchX, touchY );
								}
								if( !intercepted )
								{
									if( loadVirtualKeypad )
									{
										ResetRightStick( canvasPosition );
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private void ResetLeftStick( Vector2 canvasPosition )
	{
		stickLeftNeutralPos = canvasPosition;
		stickLeftTiltedPos = stickLeftNeutralPos;
		if( stickLeftBackgroundObject == null )
		{
			if( UICreator.CreateImage( stickBackgroundModel, ref stickLeftBackgroundObject, ref stickLeftBackgroundImage,
				interfaceCanvasObject ) )
			{
				stickLeftBackgroundTrans = stickLeftBackgroundObject.GetComponent<RectTransform>();
				if( stickLeftBackgroundTrans == null )
				{
					stickLeftBackgroundImage = null;
					Destroy( stickLeftBackgroundObject );
					stickLeftBackgroundObject = null;
				}
			}
		}
		if( stickLeftForegroundObject == null )
		{
			if( UICreator.CreateImage( stickForegroundModel, ref stickLeftForegroundObject, ref stickLeftForegroundImage,
				interfaceCanvasObject ) )
			{
				stickLeftForegroundTrans = stickLeftForegroundObject.GetComponent<RectTransform>();
				if( stickLeftForegroundTrans == null )
				{
					stickLeftForegroundImage = null;
					Destroy( stickLeftForegroundObject );
					stickLeftForegroundObject = null;
				}
			}
		}
		if( stickLeftBackgroundTrans != null )
		{
			stickLeftBackgroundTrans.anchoredPosition = stickLeftNeutralPos;
		}
		if( stickLeftForegroundTrans != null )
		{
			stickLeftForegroundTrans.anchoredPosition = stickLeftTiltedPos;
		}
		draggingLeftStick = true;
	}

	private void ResetRightStick( Vector2 canvasPosition )
	{
		stickRightNeutralPos = canvasPosition;
		stickRightTiltedPos = stickRightNeutralPos;
		if( stickRightBackgroundObject == null )
		{
			if( UICreator.CreateImage( stickBackgroundModel, ref stickRightBackgroundObject, ref stickRightBackgroundImage,
				interfaceCanvasObject ) )
			{
				stickRightBackgroundTrans = stickRightBackgroundObject.GetComponent<RectTransform>();
				if( stickRightBackgroundTrans == null )
				{
					stickRightBackgroundImage = null;
					Destroy( stickRightBackgroundObject );
					stickRightBackgroundObject = null;
				}
			}
		}
		if( stickRightForegroundObject == null )
		{
			if( UICreator.CreateImage( stickForegroundModel, ref stickRightForegroundObject, ref stickRightForegroundImage,
				interfaceCanvasObject ) )
			{
				stickRightForegroundTrans = stickRightForegroundObject.GetComponent<RectTransform>();
				if( stickRightForegroundTrans == null )
				{
					stickRightForegroundImage = null;
					Destroy( stickRightForegroundObject );
					stickRightForegroundObject = null;
				}
			}
		}
		if( stickRightBackgroundTrans != null )
		{
			stickRightBackgroundTrans.anchoredPosition = stickRightNeutralPos;
		}
		if( stickRightForegroundTrans != null )
		{
			stickRightForegroundTrans.anchoredPosition = stickRightTiltedPos;
		}
		draggingRightStick = true;
	}

	public void OnPointerUp( PointerEventData eventData )
	{
		float offsetX = 0f;

		if( challengeEditor != null )
		{
			challengeEditor.ReactToPointerUp();
		}
		else
		{
			if( menu != null )
			{
				menu.ReactToPointerUp();
			}
			else
			{
				if( (interfaceCanvas != null) && (eventData != null) && loadVirtualKeypad )
				{
					/*halmeida - only affect a given stick if the pointer up happens in its half of the screen.*/
					offsetX = eventData.position.x - halfCanvasScreenWidth;
					if( offsetX < 0f )
					{
						if( draggingLeftStick )
						{
							ReleaseLeftStick();
							ProvideInputDirection( true );
						}
					}
					else
					{
						if( draggingRightStick )
						{
							ReleaseRightStick();
							ProvideInputDirection( false );
						}
					}
				}
			}
		}
	}

	private void ReleaseLeftStick()
	{
		stickLeftBackgroundImage = null;
		stickLeftForegroundImage = null;
		stickLeftBackgroundTrans = null;
		stickLeftForegroundTrans = null;
		if( stickLeftBackgroundObject != null )
		{
			Destroy( stickLeftBackgroundObject );
			stickLeftBackgroundObject = null;
		}
		if( stickLeftForegroundObject != null )
		{
			Destroy( stickLeftForegroundObject );
			stickLeftForegroundObject = null;
		}
		stickLeftDirection = Vector2.zero;
		draggingLeftStick = false;
	}

	private void ReleaseRightStick()
	{
		stickRightBackgroundImage = null;
		stickRightForegroundImage = null;
		stickRightBackgroundTrans = null;
		stickRightForegroundTrans = null;
		if( stickRightBackgroundObject != null )
		{
			Destroy( stickRightBackgroundObject );
			stickRightBackgroundObject = null;
		}
		if( stickRightForegroundObject != null )
		{
			Destroy( stickRightForegroundObject );
			stickRightForegroundObject = null;
		}
		stickRightDirection = Vector2.zero;
		draggingRightStick = false;
	}

	public void OnBeginDrag( PointerEventData eventData )
	{
		float offsetX = 0f;
		float offsetY = 0f;
		Vector2 canvasPosition = Vector2.zero;

		if( colliderValid && (eventData != null) )
		{
			if( (menu != null) && (interfaceCanvas != null) )
			{
				offsetX = eventData.position.x - halfCanvasScreenWidth;
				offsetY = eventData.position.y - halfCanvasScreenHeight;
				offsetX /= interfaceCanvasFactor;
				offsetY /= interfaceCanvasFactor;
				canvasPosition = new Vector2( offsetX, offsetY );
				menu.ReactToBeginDrag( canvasPosition );
			}
		}
	}

	public void OnDrag( PointerEventData eventData )
	{
		float offsetX = 0f;
		float offsetY = 0f;
		bool rightHalf = false;
		Vector2 canvasPosition = Vector2.zero;
		Vector2 stickOffset = Vector2.zero;
		float stickOffsetXSqr = 0f;
		float stickOffsetYSqr = 0f;
		float hypotenuseSqr = 0f;

		if( colliderValid && (eventData != null) )
		{
			if( interfaceCanvas != null )
			{
				offsetX = eventData.position.x - halfCanvasScreenWidth;
				rightHalf = (offsetX > 0f);
				offsetY = eventData.position.y - halfCanvasScreenHeight;
				offsetX /= interfaceCanvasFactor;
				offsetY /= interfaceCanvasFactor;
				canvasPosition = new Vector2( offsetX, offsetY );
				if( menu != null )
				{
					menu.ReactToDrag( canvasPosition );
				}
				else
				{
					if( !rightHalf )
					{
						if( draggingLeftStick )
						{
							/*halmeida - gotta check if this offset, applied to the tilted position, makes the foreground stick
							get outside the max stick radius. If it does, gotta keep it within max radius by moving the whole thing.*/
							stickLeftTiltedPos.x = offsetX;
							stickLeftTiltedPos.y = offsetY;
							stickOffset = stickLeftTiltedPos - stickLeftNeutralPos;
							stickLeftDirection = stickOffset.normalized;
							stickOffsetXSqr = stickOffset.x * stickOffset.x;
							stickOffsetYSqr = stickOffset.y * stickOffset.y;
							hypotenuseSqr = stickOffsetXSqr + stickOffsetYSqr;
							if( hypotenuseSqr > stickMaxRadiusSqr )
							{
								/*halmeida - the tilted stick was brought further than the max radius.*/
								stickOffset = stickLeftDirection * stickMaxRadius;
								if( moveStickBase )
								{
									/*halmeida - Gotta bring the stick base to a propper position, closer to the tilted stick.*/
									stickLeftNeutralPos = stickLeftTiltedPos - stickOffset;
									if( stickLeftBackgroundTrans != null )
									{
										stickLeftBackgroundTrans.anchoredPosition = stickLeftNeutralPos;
									}
								}
								else
								{
									/*halmeida - Gotta keep the tilted stick within max radius.*/
									stickLeftTiltedPos = stickLeftNeutralPos + stickOffset;
								}
								hypotenuseSqr = stickMaxRadiusSqr;
							}
							if( stickLeftForegroundTrans != null )
							{
								stickLeftForegroundTrans.anchoredPosition = stickLeftTiltedPos;
							}
							if( hypotenuseSqr < stickActiveRadiusSqr )
							{
								stickLeftDirection = Vector2.zero;
							}
							ProvideInputDirection( true );
						}
					}
					else
					{
						if( draggingRightStick )
						{
							/*halmeida - gotta check if this offset, applied to the tilted position, makes the foreground stick
							get outside the max stick radius. If it does, gotta keep it within max radius by moving the whole thing.*/
							stickRightTiltedPos.x = offsetX;
							stickRightTiltedPos.y = offsetY;
							stickOffset = stickRightTiltedPos - stickRightNeutralPos;
							stickRightDirection = stickOffset.normalized;
							stickOffsetXSqr = stickOffset.x * stickOffset.x;
							stickOffsetYSqr = stickOffset.y * stickOffset.y;
							hypotenuseSqr = stickOffsetXSqr + stickOffsetYSqr;
							if( hypotenuseSqr > stickMaxRadiusSqr )
							{
								/*halmeida - the tilted stick was brought further than the max radius.*/
								stickOffset = stickRightDirection * stickMaxRadius;
								if( moveStickBase )
								{
									/*halmeida - Gotta bring the stick base to a propper position, closer to the tilted stick.*/
									stickRightNeutralPos = stickRightTiltedPos - stickOffset;
									if( stickRightBackgroundTrans != null )
									{
										stickRightBackgroundTrans.anchoredPosition = stickRightNeutralPos;
									}
								}
								else
								{
									/*halmeida - Gotta keep the tilted stick within max radius.*/
									stickRightTiltedPos = stickRightNeutralPos + stickOffset;
								}
								hypotenuseSqr = stickMaxRadiusSqr;
							}
							if( stickRightForegroundTrans != null )
							{
								stickRightForegroundTrans.anchoredPosition = stickRightTiltedPos;
							}
							if( hypotenuseSqr < stickActiveRadiusSqr )
							{
								stickRightDirection = Vector2.zero;
							}
							ProvideInputDirection( false );
						}
					}
				}
			}
		}
	}

	public void OnEndDrag( PointerEventData eventData )
	{
		float offsetX = 0f;
		float offsetY = 0f;
		Vector2 canvasPosition = Vector2.zero;

		if( colliderValid && (eventData != null) )
		{
			if( (menu != null) && (interfaceCanvas != null) )
			{
				offsetX = eventData.position.x - halfCanvasScreenWidth;
				offsetY = eventData.position.y - halfCanvasScreenHeight;
				offsetX /= interfaceCanvasFactor;
				offsetY /= interfaceCanvasFactor;
				canvasPosition = new Vector2( offsetX, offsetY );
				menu.ReactToEndDrag( canvasPosition );
			}
		}
	}

	private void ProvideInputDirection( bool leftSide )
	{
		Vector2 stickDirection = Vector2.zero;

		if( challengeStage != null )
		{
			stickDirection = (leftSide ? stickLeftDirection : stickRightDirection);
			challengeStage.ReactToInputDirection( -1, leftSide, stickDirection );
		}
	}

	public void OnPressSpace( BaseEventData baseEvent )
	{
		bool intercepted = false;

		if( textBoxUIManager != null )
		{
			intercepted = textBoxUIManager.InterceptActionPress();
		}
		if( !intercepted && (textBoxManager != null) )
		{
			intercepted = textBoxManager.InterceptActionPress();
		}
		if( !intercepted && (transactionBoxManager != null) )
		{
			transactionBoxManager.InterceptActionPress();
		}
	}

	public void OnPressReturn()
	{
		bool intercepted = false;

		if( textBoxUIManager != null )
		{
			intercepted = textBoxUIManager.InterceptConfirmation();
		}
		if( !intercepted && (textBoxManager != null) )
		{
			intercepted = textBoxManager.InterceptConfirmation();
		}
		if( !intercepted && (transactionBoxManager != null) )
		{
			transactionBoxManager.InterceptConfirmation();
		}
	}

	public void OnPressEscape()
	{
		bool intercepted = false;

		if( gameController != null )
		{
			if( !gameController.IsPaused() )
			{
				if( controlsEnabled )
				{
					gameController.OpenPauseMenu();
				}
			}
			else
			{
				if( menu != null )
				{
					intercepted = menu.ReactToQuitRequest( false );
				}
				if( !intercepted && (textBoxUIManager != null) )
				{
					intercepted = textBoxUIManager.InterceptCancellation();
				}
				if( !intercepted && (textBoxManager != null) )
				{
					intercepted = textBoxManager.InterceptCancellation();
				}
			}
		}
	}

	public void OnPressPause( BaseEventData baseEvent )
	{
		if( gameController != null )
		{
			gameController.OpenPauseMenu();
		}
	}

	public void AlterInputArea( float widthFactor, float heightFactor )
	{
		if( (inputCollider != null) && (cameraController != null) )
		{
			inputColliderAltWidth = inputColliderOriWidth * widthFactor;
			inputColliderAltHeight = inputColliderOriHeight * heightFactor;
			inputCollider.size = new Vector2( inputColliderAltWidth, inputColliderAltHeight );
			inputColliderAltered = true;
		}
	}

	public void ResetInputArea()
	{
		if( inputColliderAltered )
		{
			inputCollider.size = new Vector2( inputColliderOriWidth, inputColliderOriHeight );
			inputColliderAltered = false;
		}
	}

	public void Progress( float timeStep )
	{
		Touch touch = default( Touch );
		bool touchingRight = false;
		bool touchingLeft = false;

		if( gameController != null )
		{
			if( challengeStage != null )
			{
				if( controlsEnabled )
				{
					if( loadVirtualKeypad )
					{
						/*halmeida - there's a bug which causes the non-triggering of the OnPointerUp event callback.
						When the OnPointerUp is not triggered, the character keeps moving locked to a direction, even
						though no touch is happening at the movement half of the screen. It seems to happen when the
						movement and aiming touches are alternated fast. Causing it on purpose seems harder than it
						happening in the game. It happens a lot when playing normally on the phone. To correct this,
						we need to always check if there is a touch still happening to keep the movement or the
						aiming.*/
						if( Input.GetMouseButton(0) )
						{
							if( Input.mousePosition.x < halfCanvasScreenWidth )
							{
								touchingLeft = true;
							}
							else
							{
								touchingRight = true;
							}
						}
						for( int i=0; i<Input.touchCount; i++ )
						{
							touch = Input.GetTouch( i );
							if( touch.position.x < halfCanvasScreenWidth )
							{
								touchingLeft = true;
							}
							else
							{
								touchingRight = true;
							}
						}
						if( !touchingLeft )
						{
							ReleaseLeftStick();
							ProvideInputDirection( true );
						}
						if( !touchingRight )
						{
							ReleaseRightStick();
							ProvideInputDirection( false );
						}
					}
					else
					{
						if( Input.GetKeyDown( KeyCode.W ) )
						{
							challengeStage.ReactToUpKey( -1, true, true );
						}
						if( Input.GetKeyUp( KeyCode.W ) )
						{
							challengeStage.ReactToUpKey( -1, true, false );
						}
						if( Input.GetKeyDown( KeyCode.A ) )
						{
							challengeStage.ReactToLeftKey( -1, true, true );
						}
						if( Input.GetKeyUp( KeyCode.A ) )
						{
							challengeStage.ReactToLeftKey( -1, true, false );
						}
						if( Input.GetKeyDown( KeyCode.S ) )
						{
							challengeStage.ReactToDownKey( -1, true, true );
						}
						if( Input.GetKeyUp( KeyCode.S ) )
						{
							challengeStage.ReactToDownKey( -1, true, false );
						}
						if( Input.GetKeyDown( KeyCode.D ) )
						{
							challengeStage.ReactToRightKey( -1, true, true );
						}
						if( Input.GetKeyUp( KeyCode.D ) )
						{
							challengeStage.ReactToRightKey( -1, true, false );
						}
						if( Input.GetKeyDown( KeyCode.UpArrow ) )
						{
							challengeStage.ReactToUpKey( -1, false, true );
						}
						if( Input.GetKeyUp( KeyCode.UpArrow ) )
						{
							challengeStage.ReactToUpKey( -1, false, false );
						}
						if( Input.GetKeyDown( KeyCode.LeftArrow ) )
						{
							challengeStage.ReactToLeftKey( -1, false, true );
						}
						if( Input.GetKeyUp( KeyCode.LeftArrow ) )
						{
							challengeStage.ReactToLeftKey( -1, false, false );
						}
						if( Input.GetKeyDown( KeyCode.DownArrow ) )
						{
							challengeStage.ReactToDownKey( -1, false, true );
						}
						if( Input.GetKeyUp( KeyCode.DownArrow ) )
						{
							challengeStage.ReactToDownKey( -1, false, false );
						}
						if( Input.GetKeyDown( KeyCode.RightArrow ) )
						{
							challengeStage.ReactToRightKey( -1, false, true );
						}
						if( Input.GetKeyUp( KeyCode.RightArrow ) )
						{
							challengeStage.ReactToRightKey( -1, false, false );
						}
						if( Input.GetKeyDown( KeyCode.Space ) )
						{
							OnPressSpace( null );
						}
						if( Input.GetKeyDown( KeyCode.Return ) )
						{
							OnPressReturn();
						}
						if( Input.GetKeyDown( KeyCode.Escape ) )
						{
							OnPressEscape();
						}
					}
				}
				else
				{
					if( !loadVirtualKeypad )
					{
						if( Input.GetKeyDown( KeyCode.Return ) )
						{
							OnPressReturn();
						}
						if( Input.GetKeyDown( KeyCode.Escape ) )
						{
							OnPressEscape();
						}
					}
				}
			}
			else
			{
				if( challengeEditor != null )
				{
					if( Input.GetKeyDown( KeyCode.Return ) )
					{
						challengeEditor.ReactToReturnKeyDown();
					}
					if( Input.GetKeyDown( KeyCode.Escape ) )
					{
						challengeEditor.ReactToEscapeKeyDown();
					}
					if( Input.GetKeyDown( KeyCode.Space ) )
					{
						challengeEditor.ReactToSpaceKey( true );
					}
					if( Input.GetKeyUp( KeyCode.Space ) )
					{
						challengeEditor.ReactToSpaceKey( false );
					}
					if( Input.GetKeyDown( KeyCode.F ) )
					{
						challengeEditor.ReactToFarCameraKey( true );
					}
					if( Input.GetKeyUp( KeyCode.F ) )
					{
						challengeEditor.ReactToFarCameraKey( false );
					}
					if( Input.GetKeyDown( KeyCode.S ) )
					{
						challengeEditor.ReactToSaveKey();
					}
					if( Input.GetKeyDown( KeyCode.L ) )
					{
						challengeEditor.ReactToLoadKey();
					}
				}
				if( challengeCanvas != null )
				{
					if( Input.GetKeyDown( KeyCode.LeftArrow ) )
					{
						challengeCanvas.ReactToLeftKey( true );
					}
					if( Input.GetKeyUp( KeyCode.LeftArrow ) )
					{
						challengeCanvas.ReactToLeftKey( false );
					}
					if( Input.GetKeyDown( KeyCode.RightArrow ) )
					{
						challengeCanvas.ReactToRightKey( true );
					}
					if( Input.GetKeyUp( KeyCode.RightArrow ) )
					{
						challengeCanvas.ReactToRightKey( false );
					}
					if( Input.GetKeyDown( KeyCode.UpArrow ) )
					{
						challengeCanvas.ReactToUpKey( true );
					}
					if( Input.GetKeyUp( KeyCode.UpArrow ) )
					{
						challengeCanvas.ReactToUpKey( false );
					}
					if( Input.GetKeyDown( KeyCode.DownArrow ) )
					{
						challengeCanvas.ReactToDownKey( true );
					}
					if( Input.GetKeyUp( KeyCode.DownArrow ) )
					{
						challengeCanvas.ReactToDownKey( false );
					}
				}
			}
		}
	}
}
