using UnityEngine;
using System.Collections;
using UnityEngine.UI; //to use Image component.

public class MenuSimple : MonoBehaviour
{
	public enum ChoiceEffect { None, Pause, Unpause, CloseMenu, OpenMap, OpenInventory, QuitGame, Equip, Unequip, Use,
		OpenShop, OpenCredits, DeselectOption, BuyWithVirtualMoney, BuyWithRealMoney }
	
	public GameObject titleModel;
	public Vector2 titlePositionRates;
	public Vector2 titleScale;
	public GameObject quitModel;
	public Vector2 quitPositionRates;
	public Vector2 quitScale;
	public GameObject[] optionModels;
	public Vector2[] optionButtonScales;
	public ChoiceEffect[] optionChoiceEffects;
	public GameObject[] optionEventRequirements;
	public float optionCenterXRate;
	public float highestOptionTopRate;
	public float optionDistanceRate;
	public float backgroundMaxAlpha;
	public float backgroundAlphaSpeed;
	public bool useOptionEffectAsText;
	public ChoiceEffect titleTextEffect;
	public ChoiceEffect quitTextEffect;

	protected GameController gameController;
	protected SymbolDatabase font;
	protected GameObject interfaceCanvasObject;
	protected RectTransform interfaceCanvasTrans;
	protected Canvas interfaceCanvasComponent;
	protected Rect interfaceCanvasRect;
	protected Rect interfaceCanvasScreenRect;
	protected GameObject optionsParent;
	protected RectTransform optionsParentTransform;
	protected float maxScreenOffsetLeft;
	protected float maxScreenOffsetRight;
	protected float maxScreenOffsetUp;
	protected float maxScreenOffsetDown;
	protected ChoiceEffect selectedChoiceEffect;
	protected ButtonAnimator selectedQuitComponent;
	protected ButtonAnimator selectedOptionComponent;
	protected bool touchingOverlay;
	protected GameObject imageOverlayModel;
	protected GameObject imageOverlayObject;
	protected Image imageOverlayComponent;
	protected GameObject[] optionObjects;
	protected RectTransform[] optionTransforms;
	protected ButtonAnimator[] optionComponents;
	protected ChoiceEffect[] optionEffects;
	protected GameObject[] optionTextObjects;
	protected SpritedStringUI[] optionTexts;
	protected float highestOptionTopY;
	protected float hiddenOptionHeight;
	protected bool requiringProgress;
	protected float overlayFadeSpeed;
	protected float overlayFadeSpeedRef;
	protected float overlayMaxAlpha;
	protected bool allowQuitOption;

	private GameObject titleObject;
	private RectTransform titleTransform;
	private ButtonAnimator titleComponent;
	private GameObject titleTextObject;
	private SpritedStringUI titleText;
	private GameObject quitObject;
	private RectTransform quitTransform;
	private ButtonAnimator quitComponent;
	private GameObject quitTextObject;
	private SpritedStringUI quitText;
	private int totalValidModels;
	private GameObject[] totalValidOptionModels;
	private Vector2[] totalValidOptionScales;
	private ChoiceEffect[] totalValidOptionChoiceEffects;
	private int[] totalValidOptionEventIDs;
	private int currentValidModels;
	private GameObject[] currentValidOptionModels;
	private Vector2[] currentValidOptionScales;
	private ChoiceEffect[] currentValidOptionChoiceEffects;
	private int[] currentValidOptionEventIDs;
	private bool overlayDark;
	private bool overlayClear;
	private bool graphicsReady;
	private bool graphicsCleared;
	private bool draggingMenu;
	private float lastDragCanvasX;
	private float lastDragCanvasY;
	private bool clearBeforeEffect;
	private EventManager eventManager;
	private MenuSimple previousMenu;

	protected virtual void Awake()
	{
		GameObject optionModel = null;
		RectTransform optionTrans = null;
		ButtonAnimator optionComponent = null;
		ChoiceEffect optionChoiceEffect = ChoiceEffect.None;
		GameObject optionEventObject = null;
		EventBase optionEventComponent = null;
		int optionEventID = EventBase.INVALID_EVENT_ID;
		Vector2 optionScale = Vector2.one;

		gameController = null;
		font = null;
		interfaceCanvasObject = null;
		interfaceCanvasTrans = null;
		interfaceCanvasComponent = null;
		interfaceCanvasRect = new Rect( 0f, 0f, 0f, 0f );
		interfaceCanvasScreenRect = new Rect( 0f, 0f, 0f, 0f );
		imageOverlayModel = null;
		imageOverlayObject = null;
		imageOverlayComponent = null;
		titleObject = null;
		titleTransform = null;
		titleComponent = null;
		titleTextObject = null;
		titleText = null;
		quitObject = null;
		quitTransform = null;
		quitComponent = null;
		quitTextObject = null;
		quitText = null;
		allowQuitOption = true;
		optionsParent = null;
		optionsParentTransform = null;
		totalValidModels = 0;
		totalValidOptionModels = null;
		totalValidOptionScales = null;
		totalValidOptionChoiceEffects = null;
		totalValidOptionEventIDs = null;
		currentValidModels = 0;
		currentValidOptionModels = null;
		currentValidOptionScales = null;
		currentValidOptionChoiceEffects = null;
		currentValidOptionEventIDs = null;
		optionObjects = null;
		optionTransforms = null;
		optionComponents = null;
		optionEffects = null;
		optionTextObjects = null;
		optionTexts = null;
		selectedChoiceEffect = ChoiceEffect.None;
		selectedQuitComponent = null;
		selectedOptionComponent = null;
		overlayFadeSpeed = 0f;
		overlayFadeSpeedRef = (backgroundAlphaSpeed > 0f) ? backgroundAlphaSpeed : 1f;
		overlayMaxAlpha = backgroundMaxAlpha;
		overlayDark = false;
		overlayClear = false;
		graphicsReady = false;
		graphicsCleared = false;
		touchingOverlay = false;
		draggingMenu = false;
		lastDragCanvasX = 0f;
		lastDragCanvasY = 0f;
		highestOptionTopY = 0f;
		hiddenOptionHeight = 0f;
		maxScreenOffsetLeft = 0f;
		maxScreenOffsetRight = 0f;
		maxScreenOffsetUp = 0f;
		maxScreenOffsetDown = 0f;
		requiringProgress = false;
		clearBeforeEffect = true;
		eventManager = null;
		previousMenu = null;
		if( optionModels != null )
		{
			for( int i=0; i<optionModels.Length; i++ )
			{
				optionModel = optionModels[i];
				if( optionModel != null )
				{
					optionTrans = optionModel.GetComponent<RectTransform>();
					optionComponent = optionModel.GetComponent<ButtonAnimator>();
					if( (optionTrans != null) && (optionComponent != null) )
					{
						optionScale = Vector2.one;
						optionChoiceEffect = ChoiceEffect.None;
						optionEventID = EventBase.INVALID_EVENT_ID;
						if( optionButtonScales != null )
						{
							if( optionButtonScales.Length > i )
							{
								optionScale = optionButtonScales[i];
							}
						}
						if( optionChoiceEffects != null )
						{
							if( optionChoiceEffects.Length > i )
							{
								optionChoiceEffect = optionChoiceEffects[i];
							}
						}
						if( optionEventRequirements != null )
						{
							if( optionEventRequirements.Length > i )
							{
								optionEventObject = optionEventRequirements[i];
								if( optionEventObject != null )
								{
									optionEventComponent = optionEventObject.GetComponent<EventBase>();
									if( optionEventComponent != null )
									{
										optionEventID = optionEventComponent.eventID;
									}
								}
							}
						}
						totalValidModels++;
						UsefulFunctions.IncreaseArray<GameObject>( ref totalValidOptionModels, optionModel );
						UsefulFunctions.IncreaseArray<Vector2>( ref totalValidOptionScales, optionScale );
						UsefulFunctions.IncreaseArray<ChoiceEffect>( ref totalValidOptionChoiceEffects, optionChoiceEffect );
						UsefulFunctions.IncreaseArray<int>( ref totalValidOptionEventIDs, optionEventID );
					}
				}
			}
		}
	}

	public virtual void SetGameController( GameController newGameController, Canvas newInterfaceCanvas, GameObject newImageOverlayModel,
		EventManager newEventManager )
	{
		gameController = newGameController;
		if( gameController != null )
		{
			font = gameController.GetSymbolDatabaseUI();
		}
		interfaceCanvasObject = null;
		interfaceCanvasTrans = null;
		interfaceCanvasComponent = newInterfaceCanvas;
		if( interfaceCanvasComponent != null )
		{
			interfaceCanvasObject = interfaceCanvasComponent.gameObject;
			interfaceCanvasTrans = interfaceCanvasObject.GetComponent<RectTransform>();
			if( interfaceCanvasTrans != null )
			{
				interfaceCanvasRect = interfaceCanvasTrans.rect;
				interfaceCanvasScreenRect = interfaceCanvasComponent.pixelRect;
				//Debug.Log("Debug : MenuSimple : interface canvas reference rect = "+interfaceCanvasRect+".");
				//Debug.Log("Debug : MenuSimple : interface canvas actual screen Rect = "+interfaceCanvasScreenRect+".");
			}
			else
			{
				interfaceCanvasComponent = null;
				interfaceCanvasObject = null;
			}
		}
		imageOverlayModel = newImageOverlayModel;
		eventManager = newEventManager;
	}

	public void SetClosureBeforeEffect( bool newClosure )
	{
		clearBeforeEffect = newClosure;
	}

	public void SetPreviousMenu( MenuSimple previousSimpleMenu )
	{
		previousMenu = previousSimpleMenu;
	}

	public MenuSimple GetPreviousMenu()
	{
		return previousMenu;
	}

	public void BuildVisualRepresentation( bool withQuitOption = true )
	{
		if( graphicsReady && graphicsCleared )
		{
			graphicsReady = false;
			graphicsCleared = false;
		}
		if( !graphicsReady )
		{
			if( !overlayDark )
			{
				if( imageOverlayComponent == null )
				{
					CreateBackground( false, imageOverlayModel, ref imageOverlayObject, ref imageOverlayComponent, ref overlayDark,
						ref overlayClear );
				}
				if( imageOverlayComponent != null )
				{
					overlayFadeSpeed = overlayFadeSpeedRef;
				}
				allowQuitOption = withQuitOption;
				requiringProgress = true;
			}
			else
			{
				CreateHeader();
				BuildOptions();
				SetMaxScreenOffsets();
				ScrollForUserConvenience();
				graphicsReady = true;
			}
		}
	}

	protected void CreateBackground( bool alreadyOpaque, GameObject newOverlayModel, ref GameObject newOverlayObject, ref Image newOverlayImage,
		ref bool isOpaque, ref bool isInvisible )
	{
		Color color = Color.black;
		RectTransform rectTrans = null;

		if( interfaceCanvasObject != null )
		{
			if( (newOverlayObject == null) && (newOverlayImage == null) )
			{
				if( newOverlayModel != null )
				{
					newOverlayObject = Instantiate( newOverlayModel ) as GameObject;
					rectTrans = newOverlayObject.GetComponent<RectTransform>();
					newOverlayImage = newOverlayObject.GetComponent<Image>();
					if( (rectTrans != null) && (newOverlayImage != null) )
					{
						color = newOverlayImage.color;
						color.a = alreadyOpaque ? overlayMaxAlpha : 0f;
						newOverlayImage.color = color;
						rectTrans.SetParent( interfaceCanvasTrans, false );
						isInvisible = !alreadyOpaque;
						isOpaque = alreadyOpaque;
					}
					else
					{
						newOverlayImage = null;
						Destroy( newOverlayObject );
						newOverlayObject = null;
					}
				}
				if( newOverlayObject == null )
				{
					isOpaque = true;
					isInvisible = false;
				}
			}
			if( optionsParent == null )
			{
				optionsParent = new GameObject("OptionsParent", typeof(RectTransform));
				optionsParentTransform = optionsParent.GetComponent<RectTransform>();
				if( optionsParentTransform != null )
				{
					optionsParentTransform.SetParent( interfaceCanvasTrans, false );
					optionsParentTransform.anchoredPosition = Vector2.zero;
				}
			}
		}
	}

	private bool CreateHeader()
	{
		string buttonCaption = null;
		RectTransform toDiscard = null;
		Vector2 anchoredPosition = Vector2.zero;

		if( interfaceCanvasObject != null )
		{
			GameTextDatabase gameTextDatabase = GameTextDatabase.Instance;
			if( (titleModel != null) && (titleObject == null) )
			{
				titleObject = Instantiate( titleModel ) as GameObject;
				titleTransform = titleObject.GetComponent<RectTransform>(); 
				titleComponent = titleObject.GetComponent<ButtonAnimator>();
				if( (titleTransform != null) && (titleComponent != null) )
				{
					titleTransform.SetParent( interfaceCanvasTrans, false );
					anchoredPosition.x = (titlePositionRates.x - 0.5f) * interfaceCanvasRect.width;
					anchoredPosition.y = (titlePositionRates.y - 0.5f) * interfaceCanvasRect.height;
					titleTransform.anchoredPosition = anchoredPosition;
					titleTransform.localScale = new Vector3( titleScale.x, titleScale.y, 1f );
					if( gameTextDatabase != null )
					{
						buttonCaption = gameTextDatabase.GetMenuOptionText( titleTextEffect );
						CreateButtonText( titleTransform, buttonCaption, ref titleTextObject, ref toDiscard, ref titleText, true );
					}
					titleComponent.SetPressed( true );
				}
				else
				{
					titleTransform = null;
					if( titleComponent != null )
					{
						titleComponent.Clear();
						titleComponent = null;
					}
					Destroy( titleObject );
					titleObject = null;
				}
			}
			if( allowQuitOption && (quitModel != null) && (quitObject == null) )
			{
				quitObject = Instantiate( quitModel ) as GameObject;
				quitTransform = quitObject.GetComponent<RectTransform>();
				quitComponent = quitObject.GetComponent<ButtonAnimator>();
				if( (quitTransform != null) && (quitComponent != null) )
				{
					quitTransform.SetParent( interfaceCanvasTrans, false );
					anchoredPosition.x = (quitPositionRates.x - 0.5f) * interfaceCanvasRect.width;
					anchoredPosition.y = (quitPositionRates.y - 0.5f) * interfaceCanvasRect.height;
					quitTransform.anchoredPosition = anchoredPosition;
					quitTransform.localScale = new Vector3( quitScale.x, quitScale.y, 1f );
					if( gameTextDatabase != null )
					{
						buttonCaption = gameTextDatabase.GetMenuOptionText( quitTextEffect );
						CreateButtonText( quitTransform, buttonCaption, ref quitTextObject, ref toDiscard, ref quitText, true );
					}
				}
				else
				{
					quitTransform = null;
					if( quitComponent != null )
					{
						quitComponent.Clear();
						quitComponent = null;
					}
					Destroy( quitObject );
					quitObject = null;
				}
			}
			return ( (quitObject != null) || (titleObject != null) );
		}
		return false;
	}

	protected void CreateButtonText( RectTransform buttonTransform, string textContent, ref GameObject textObject, ref RectTransform textTransform,
		ref SpritedStringUI textComponent, bool replicateScale )
	{
		RectTransform areaParentTrans = null;
		if( (buttonTransform != null) && (textContent != null) )
		{
			areaParentTrans = (RectTransform)buttonTransform.parent;
			if( areaParentTrans != null )
			{
				textObject = new GameObject("ButtonText", typeof(RectTransform));
				textTransform = textObject.GetComponent<RectTransform>();
				textTransform.SetParent( areaParentTrans, false );
				textComponent = textObject.AddComponent<SpritedStringUI>();
				textComponent.SetSymbolSource( font );
				textComponent.SetValue( textContent );
				textComponent.ToggleRaycastTargeting( false );
				textTransform.anchoredPosition = buttonTransform.anchoredPosition;
				if( replicateScale )
				{
					textTransform.localScale = buttonTransform.localScale;
				}
			}
		}
	}

	protected virtual void BuildOptions()
	{
		if( (interfaceCanvasComponent == null) || (optionsParentTransform == null) )
		{
			return;
		}
		SeparateCurrentlyValidOptions();
		if( (currentValidOptionModels == null) || (currentValidOptionChoiceEffects == null) )
		{
			return;
		}
		optionObjects = new GameObject[currentValidModels];
		optionTransforms = new RectTransform[currentValidModels];
		optionComponents = new ButtonAnimator[currentValidModels];
		optionEffects = new ChoiceEffect[currentValidModels];
		optionTextObjects = new GameObject[currentValidModels];
		optionTexts = new SpritedStringUI[currentValidModels];
		GameObject optionModel = null;
		GameObject newOptionObject = null;
		RectTransform newOptionTrans = null;
		ButtonAnimator newOptionComponent = null;
		Vector2 newOptionScale = Vector2.one;
		ChoiceEffect newOptionEffect = ChoiceEffect.None;
		GameObject newOptionTextObject = null;
		RectTransform toDiscard = null;
		SpritedStringUI newOptionText = null;
		/*halmeida - discover the canvas X coordinate where all options should be placed.*/
		float newAnchoredX = (optionCenterXRate - 0.5f) * interfaceCanvasRect.width;
		/*halmeida - discover the canvas Y coordinate where the top of the first option should be placed.*/
		highestOptionTopY = (highestOptionTopRate - 0.5f) * interfaceCanvasRect.height;
		float newAnchoredY = highestOptionTopY;
		/*halmeida - discover the vertical distance from one option to the next.*/
		float optionDistance = optionDistanceRate * interfaceCanvasRect.height;
		float halfOptionHeight = 0f;
		GameTextDatabase gameTextDatabase = GameTextDatabase.Instance;
		string buttonCaption = null;
		for( int i=0; i<currentValidModels; i++ )
		{
			optionModel = currentValidOptionModels[i];
			newOptionObject = Instantiate( optionModel ) as GameObject;
			newOptionTrans = newOptionObject.GetComponent<RectTransform>();
			newOptionComponent = newOptionObject.GetComponent<ButtonAnimator>();
			newOptionScale = currentValidOptionScales[i];
			newOptionEffect = currentValidOptionChoiceEffects[i];
			newOptionTrans.SetParent( optionsParentTransform, false );
			newOptionTrans.localScale = new Vector3( newOptionScale.x, newOptionScale.y, 1f );
			halfOptionHeight = (newOptionTrans.rect.height / 2f) * newOptionScale.y;
			newAnchoredY -= halfOptionHeight;
			newOptionTrans.anchoredPosition = new Vector2( newAnchoredX, newAnchoredY );
			newAnchoredY -= halfOptionHeight + optionDistance;
			newOptionTextObject = null;
			newOptionText = null;
			if( useOptionEffectAsText && (gameTextDatabase != null) )
			{
				buttonCaption = gameTextDatabase.GetMenuOptionText( newOptionEffect );
				CreateButtonText( newOptionTrans, buttonCaption, ref newOptionTextObject, ref toDiscard, ref newOptionText, false );
			}
			optionObjects[i] = newOptionObject;
			optionTransforms[i] = newOptionTrans;
			optionComponents[i] = newOptionComponent;
			optionEffects[i] = newOptionEffect;
			optionTextObjects[i] = newOptionTextObject;
			optionTexts[i] = newOptionText;
		}
		hiddenOptionHeight = 0f;
		if( newAnchoredY < -interfaceCanvasRect.height/2f )
		{
			hiddenOptionHeight = -newAnchoredY - interfaceCanvasRect.height/2f;
		}
	}

	private void SeparateCurrentlyValidOptions()
	{
		bool[] requirementsMet = null;

		currentValidModels = 0;
		currentValidOptionModels = null;
		currentValidOptionScales = null;
		currentValidOptionChoiceEffects = null;
		currentValidOptionEventIDs = null;
		if( totalValidOptionModels == null )
		{
			return;
		}
		requirementsMet = EvaluateValidOptionRequirements();
		if( requirementsMet != null )
		{
			for( int i=0; i<totalValidModels; i++ )
			{
				if( requirementsMet[i] )
				{
					currentValidModels++;
					UsefulFunctions.IncreaseArray<GameObject>( ref currentValidOptionModels, totalValidOptionModels[i] );
					UsefulFunctions.IncreaseArray<Vector2>( ref currentValidOptionScales, totalValidOptionScales[i] );
					UsefulFunctions.IncreaseArray<ChoiceEffect>( ref currentValidOptionChoiceEffects, totalValidOptionChoiceEffects[i] );
					UsefulFunctions.IncreaseArray<int>( ref currentValidOptionEventIDs, totalValidOptionEventIDs[i] );
				}
			}
		}
	}

	protected virtual bool[] EvaluateValidOptionRequirements()
	{
		bool[] requirementsMet = null;
		bool[] eventConclusions = null;
		int requirementID = EventBase.INVALID_EVENT_ID;

		requirementsMet = new bool[totalValidModels];
		for( int i=0; i<totalValidModels; i++ )
		{
			requirementsMet[i] = true;
		}
		if( (totalValidOptionEventIDs != null) && (eventManager != null) )
		{
			/*halmeida - if totalValidOptionEventIDs is not null, we already know its length is totalValidModels.*/
			eventConclusions = eventManager.GetEventConclusions( totalValidOptionEventIDs );
			if( eventConclusions != null )
			{
				if( eventConclusions.Length == totalValidModels )
				{
					for( int i=0; i<totalValidModels; i++ )
					{
						requirementID = totalValidOptionEventIDs[i];
						if( requirementID != EventBase.INVALID_EVENT_ID )
						{
							requirementsMet[i] = eventConclusions[i];
						}
					}
				}
			}
		}
		return requirementsMet;
	}

	protected virtual void SetMaxScreenOffsets()
	{
		/*halmeida - the simple menu is not allowed to be dragged sideways. It can only be dragged up or
		down and only as much as necessary to allow contemplation of all its options.*/
		/*halmeida - all offsets are supposed to be positive.*/
		maxScreenOffsetLeft = 0f;
		maxScreenOffsetRight = 0f;
		maxScreenOffsetUp = hiddenOptionHeight;
		maxScreenOffsetDown = 0f;
	}

	protected virtual void ScrollForUserConvenience()
	{
		/*halmeida - no scrolling required for this kind of menu.*/
	}

	public virtual void Progress( float timeStep )
	{
		bool clearMenu = false;

		ProgressOverlay( timeStep );
		if( titleComponent != null )
		{
			titleComponent.Progress( timeStep );
		}
		if( quitComponent != null )
		{
			quitComponent.Progress( timeStep );
		}
		if( !graphicsReady )
		{
			if( overlayDark )
			{
				BuildVisualRepresentation();
			}
		}
		else
		{
			if( !graphicsCleared )
			{
				ProgressOptionComponents( timeStep );
				if( selectedChoiceEffect != ChoiceEffect.None )
				{
					clearMenu = true;
					if( selectedQuitComponent != null )
					{
						if( selectedQuitComponent.GetCurrentStatus() != ButtonAnimator.STATUS_ACTIVATED )
						{
							clearMenu = false;
						}
					}
					if( selectedOptionComponent != null )
					{
						if( selectedOptionComponent.GetCurrentStatus() != ButtonAnimator.STATUS_ACTIVATED )
						{
							clearMenu = false;
						}
					}
					if( clearMenu )
					{
						if( !clearBeforeEffect )
						{
							PerformChoiceEffect();
							selectedChoiceEffect = ChoiceEffect.None;
							/*halmeida - in this case the menu will not clear its visual representation by itself.
							It will be waiting for a call to ClearVisualRepresentation to start the process. We can
							call this a manual closure.*/
						}
						else
						{
							ClearVisualRepresentation( false );
							/*halmeida - in this case the menu will clear its visual representation before passing
							on the choice that was made. Since the selection effect value is not yet lost, this
							instruction will be called again at every Process() call, until the graphics are actually
							cleared and the choice effect is performed.*/
						}
					}
				}
				else if( overlayClear )
				{
					/*halmeida - this is needed for the manual closure process. The outside call to clear the visual
					representation removes the options and starts the overlay fading into clear. To trully complete
					the closure we gotta remove the overlay, and this is done by calling the clearing function with
					the overlay already at a clear color.*/
					ClearVisualRepresentation( false );
				}
			}
			else
			{
				requiringProgress = false;
				if( (selectedChoiceEffect != ChoiceEffect.None) && clearBeforeEffect )
				{
					PerformChoiceEffect();
					selectedChoiceEffect = ChoiceEffect.None;
				}
			}
		}
	}

	protected virtual void ProgressOverlay( float timeStep )
	{
		Color color = Color.black;
		float alpha = 0f;
		
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
					if( alpha > overlayMaxAlpha )
					{
						alpha = overlayMaxAlpha;
						overlayDark = true;
						overlayFadeSpeed = 0f;
					}
				}
				else
				{
					overlayDark = false;
					if( alpha < 0f )
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

	protected virtual void ProgressOptionComponents( float timeStep )
	{
		ButtonAnimator optionComponent = null;

		if( optionComponents != null )
		{
			for( int i=0; i<optionComponents.Length; i++ )
			{
				optionComponent = optionComponents[i];
				if( optionComponent != null )
				{
					optionComponent.Progress( timeStep );
				}
			}
		}
	}

	public virtual void DeselectOptionComponent()
	{
		if( selectedOptionComponent != null )
		{
			selectedOptionComponent.SetPressed( false );
			selectedOptionComponent = null;
		}
	}

	public void ClearVisualRepresentation( bool immediately )
	{
		if( !graphicsCleared )
		{
			if( overlayDark || immediately )
			{
				if( titleText != null )
				{
					titleText.Clear();
					titleText = null;
				}
				if( titleTextObject != null )
				{
					Destroy( titleTextObject );
					titleTextObject = null;
				}
				if( titleComponent != null )
				{
					titleComponent.Clear();
					titleComponent = null;
				}
				titleTransform = null;
				if( titleObject != null )
				{
					Destroy( titleObject );
					titleObject = null;
				}
				selectedQuitComponent = null;
				if( quitText != null )
				{
					quitText.Clear();
					quitText = null;
				}
				if( quitTextObject != null )
				{
					Destroy( quitTextObject );
					quitTextObject = null;
				}
				if( quitComponent != null )
				{
					quitComponent.Clear();
					quitComponent = null;
				}
				quitTransform = null;
				if( quitObject != null )
				{
					Destroy( quitObject );
					quitObject = null;
				}
				ClearOptions();
				if( optionsParent != null )
				{
					optionsParentTransform = null;
					Destroy( optionsParent );
					optionsParent = null;
				}
				if( !immediately && (imageOverlayComponent != null) )
				{
					overlayFadeSpeed = -overlayFadeSpeedRef;
				}
				else
				{
					overlayDark = false;
					overlayClear = true;
				}
			}
			if( overlayClear )
			{
				if( imageOverlayObject != null )
				{
					imageOverlayComponent = null;
					Destroy( imageOverlayObject );
					imageOverlayObject = null;
				}
				graphicsCleared = true;
				if( immediately )
				{
					requiringProgress = false;
					selectedChoiceEffect = ChoiceEffect.None;
				}
			}
		}
	}

	protected virtual void ClearOptions()
	{
		ButtonAnimator optionComponent = null;
		GameObject optionObject = null;
		SpritedStringUI optionText = null;

		selectedOptionComponent = null;
		optionEffects = null;
		if( optionComponents != null )
		{
			for( int i=0; i<optionComponents.Length; i++ )
			{
				optionComponent = optionComponents[i];
				if( optionComponent != null )
				{
					optionComponent.Clear();
					optionComponents[i] = null;
				}
			}
			optionComponents = null;
		}
		if( optionTransforms != null )
		{
			for( int i=0; i<optionTransforms.Length; i++ )
			{
				optionTransforms[i] = null;
			}
			optionTransforms = null;
		}
		if( optionObjects != null )
		{
			for( int i=0; i<optionObjects.Length; i++ )
			{
				optionObject = optionObjects[i];
				if( optionObject != null )
				{
					Destroy( optionObject );
					optionObjects[i] = null;
				}
			}
			optionObjects = null;
		}
		if( optionTexts != null )
		{
			for( int i=0; i<optionTexts.Length; i++ )
			{
				optionText = optionTexts[i];
				if( optionText != null )
				{
					optionText.Clear();
					optionTexts[i] = null;
				}
			}
			optionTexts = null;
		}
		if( optionTextObjects != null )
		{
			for( int i=0; i<optionTextObjects.Length; i++ )
			{
				optionObject = optionTextObjects[i];
				if( optionObject != null )
				{
					Destroy( optionObject );
					optionTextObjects[i] = null;
				}
			}
			optionTextObjects = null;
		}
		currentValidModels = 0;
		currentValidOptionModels = null;
		currentValidOptionScales = null;
		currentValidOptionChoiceEffects = null;
		currentValidOptionEventIDs = null;
	}

	public virtual void Clear()
	{
		ClearVisualRepresentation( true );
		totalValidModels = 0;				
		totalValidOptionModels = null;
		totalValidOptionScales = null;
		totalValidOptionChoiceEffects = null;
		totalValidOptionEventIDs = null;
		font = null;
	}

	protected virtual void PerformChoiceEffect()
	{
		switch( selectedChoiceEffect )
		{
			case ChoiceEffect.Unpause:
				if( gameController != null )
				{
					gameController.CloseMenu();
				}
				break;
			case ChoiceEffect.CloseMenu:
				if( gameController != null )
				{
					gameController.CloseMenu();
				}
				break;
			case ChoiceEffect.OpenMap:
				if( gameController != null )
				{
					if( !gameController.OpenMap() )
					{
						DeselectOptionComponent();
					}
				}
				break;
			case ChoiceEffect.OpenInventory:
				if( gameController != null )
				{
					if( !gameController.OpenInventory() )
					{
						DeselectOptionComponent();
					}
				}
				break;
			case ChoiceEffect.DeselectOption:
				DeselectOptionComponent();
				break;
			case ChoiceEffect.QuitGame:
				if( gameController != null )
				{
					gameController.QuitGame();
				}
				break;
			case ChoiceEffect.OpenShop:
				if( gameController != null )
				{
					/*if( !gameController.OpenShop() )
					{
						DeselectOptionComponent();
					}*/
				}
				break;
			case ChoiceEffect.OpenCredits:
				if( gameController != null )
				{
					/*if( !gameController.OpenCredits() )
					{
						DeselectOptionComponent();
					}*/
				}
				break;
		}
	}

	public virtual bool ReactToQuitRequest( bool ignoreQuitAvailability )
	{
		if( !ignoreQuitAvailability )
		{
			if( quitComponent != null )
			{
				selectedQuitComponent = quitComponent;
				selectedQuitComponent.SetPressed( true );
			}
			else
			{
				return false;
			}
		}
		selectedChoiceEffect = ChoiceEffect.CloseMenu;
		return true;
	}

	public virtual void ReactToPointerDown( Vector2 canvasPoint )
	{
		bool intercepted = false;

		//Debug.Log("Debug : MenuSimple : pointer down : position = "+screenPointX+","+screenPointY+".");
		if( selectedChoiceEffect == ChoiceEffect.None )
		{
			intercepted = CheckQuitHit( canvasPoint );
			if( !intercepted )
			{
				intercepted = CheckOptionsHit( canvasPoint );
			}
			if( !intercepted )
			{
				touchingOverlay = true;
			}
		}
	}

	private bool CheckQuitHit( Vector2 canvasPoint )
	{
		if( quitTransform != null )
		{
			float realWidth = quitTransform.rect.width * quitTransform.localScale.x;
			float realHeight = quitTransform.rect.height * quitTransform.localScale.y;
			float xMin = quitTransform.anchoredPosition.x - realWidth/2f;
			float yMax = quitTransform.anchoredPosition.y + realHeight/2f;
			if( UsefulFunctions.AreaContainsPoint( xMin, yMax, realWidth, realHeight, canvasPoint.x, canvasPoint.y ) )
			{
				selectedQuitComponent = quitComponent;
				selectedQuitComponent.SetPressed( true );
				ReactToQuitRequest( true );
				return true;
			}
		}
		return false;
	}

	protected virtual bool CheckOptionsHit( Vector2 canvasPoint )
	{
		RectTransform optionTransform = null;
		ButtonAnimator optionComponent = null;
		float realWidth = 0f;
		float realHeight = 0f;
		float xMin = 0f;
		float yMax = 0f;

		if( optionTransforms != null )
		{
			for( int i=0; i<optionTransforms.Length; i++ )
			{
				optionTransform = optionTransforms[i];
				realWidth = optionTransform.rect.width * optionTransform.localScale.x;
				realHeight = optionTransform.rect.height * optionTransform.localScale.y;
				xMin = optionTransform.anchoredPosition.x - realWidth/2f;
				yMax = optionTransform.anchoredPosition.y + realHeight/2f;
				if( optionsParentTransform != null )
				{
					xMin += optionsParentTransform.anchoredPosition.x;
					yMax += optionsParentTransform.anchoredPosition.y;
				}
				if( UsefulFunctions.AreaContainsPoint( xMin, yMax, realWidth, realHeight, canvasPoint.x, canvasPoint.y ) )
				{
					selectedOptionComponent = null;
					if( optionComponents != null )
					{
						if( optionComponents.Length > i )
						{
							optionComponent = optionComponents[i];
							if( optionComponent != null )
							{
								selectedOptionComponent = optionComponent;
								selectedOptionComponent.SetPressed( true );
							}
						}
					}
					selectedChoiceEffect = ChoiceEffect.None;
					if( optionEffects != null )
					{
						if( optionEffects.Length > i )
						{
							selectedChoiceEffect = optionEffects[i];
						}
					}
					return true;
				}
			}
		}
		return false;
	}

	public void ReactToPointerUp()
	{
		//Debug.Log("Debug : MenuSimple : pointer up.");
		touchingOverlay = false;
	}

	public void ReactToBeginDrag( Vector2 canvasPoint )
	{
		//Debug.Log("Debug : MenuSimple : begin drag : position = "+screenPointX+","+screenPointY+".");
		if( selectedChoiceEffect == ChoiceEffect.None )
		{
			if( touchingOverlay )
			{
				draggingMenu = true;
				lastDragCanvasX = canvasPoint.x;
				lastDragCanvasY = canvasPoint.y;
			}
		}
	}

	public void ReactToDrag( Vector2 canvasPoint )
	{
		float canvasOffsetX = 0f;
		float canvasOffsetY = 0f;
		float newOptionsX = 0f;
		float newOptionsY = 0f;

		Debug.Log("Debug : MenuSimple : drag : UI position = "+canvasPoint.x+","+canvasPoint.y+".");
		if( touchingOverlay && draggingMenu && (optionsParentTransform != null) )
		{
			canvasOffsetX = canvasPoint.x - lastDragCanvasX;
			canvasOffsetY = canvasPoint.y - lastDragCanvasY;
			lastDragCanvasX = canvasPoint.x;
			lastDragCanvasY = canvasPoint.y;
			newOptionsX = optionsParentTransform.anchoredPosition.x + canvasOffsetX;
			newOptionsX = (newOptionsX > maxScreenOffsetRight) ? maxScreenOffsetRight : newOptionsX;
			newOptionsX = (newOptionsX < -maxScreenOffsetLeft) ? -maxScreenOffsetLeft : newOptionsX;
			newOptionsY = optionsParentTransform.anchoredPosition.y + canvasOffsetY;
			newOptionsY = (newOptionsY > maxScreenOffsetUp) ? maxScreenOffsetUp : newOptionsY;
			newOptionsY = (newOptionsY < -maxScreenOffsetDown) ? -maxScreenOffsetDown : newOptionsY;
			optionsParentTransform.anchoredPosition = new Vector2( newOptionsX, newOptionsY );
		}
	}

	public void ReactToEndDrag( Vector2 canvasPoint )
	{
		Debug.Log("Debug : MenuSimple : end drag : UI position = "+canvasPoint.x+","+canvasPoint.y+".");
		draggingMenu = false;
	}

	public bool RequiresProgress()
	{
		return requiringProgress;
	}

	protected Rect ExtractCorrespondingScreenRect( RectTransform rectTrans )
	{
		Vector2 offsetFromAnchor = Vector2.zero;
		float startingX = 0f;
		float startingY = 0f;
		float elementWidth = 0f;
		float elementHeight = 0f;
		float elementScreenWidth = 0f;
		float elementScreenHeight = 0f;
		float correctedX = 0f;
		float correctedY = 0f;
		float traveledRate = 0f;

		/*halmeida - this is not a very complete way of calculating the screen rect of a UI element.
		It does only work with elements that have the pivot at their center and have their anchors
		at a single point and only if that parent element is the canvas itself.*/
		if( (rectTrans != null) && (interfaceCanvasComponent != null) )
		{
			if( rectTrans.anchorMax == rectTrans.anchorMin )
			{
				startingX = rectTrans.anchorMax.x * interfaceCanvasRect.width;
				startingY = rectTrans.anchorMax.y * interfaceCanvasRect.height;
				offsetFromAnchor = rectTrans.anchoredPosition;
				correctedX = startingX + offsetFromAnchor.x;
				correctedY = startingY + offsetFromAnchor.y;
				elementWidth = rectTrans.rect.width * rectTrans.localScale.x;
				elementHeight = rectTrans.rect.height * rectTrans.localScale.y;
				correctedX -= elementWidth / 2f;
				correctedY -= elementHeight / 2f;
				/*halmeida - now we have the position of the left bottom corner. But they
				are still in canvas coordinates, not screen coordinates.*/
				traveledRate = correctedX / interfaceCanvasRect.width;
				correctedX = traveledRate * interfaceCanvasScreenRect.width;
				traveledRate = correctedY / interfaceCanvasRect.height;
				correctedY = traveledRate * interfaceCanvasScreenRect.height;
				elementScreenWidth = elementWidth * interfaceCanvasComponent.scaleFactor;
				elementScreenHeight = elementHeight * interfaceCanvasComponent.scaleFactor;
			}
		}
		return new Rect( correctedX, correctedY, elementScreenWidth, elementScreenHeight );
	}
}
