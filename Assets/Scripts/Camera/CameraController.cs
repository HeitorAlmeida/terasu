using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public const int TARGET_MODE_INVALID = -1;
	public const int SMALLEST_TARGET_MODE = 0;
	public const int TARGET_MODE_POSITION = 0;
	public const int TARGET_MODE_PLAYER = 1;
	public const int TARGET_MODE_NPC = 2;
	public const int TARGET_MODE_MIDWAY = 3;
	public const int TARGET_MODE_EDITOR = 4;
	public const int BIGGEST_TARGET_MODE = 4;

	public const int TARGET_APPROACH_INVALID = -1;
	public const int TARGET_APPROACH_SPEED = 0;
	public const int TARGET_APPROACH_LERP = 1;

	public const float DEFAULT_LERP_FACTOR = 2f;
	public const float SMOOTH_LERP_FACTOR = 5f;
	public const float TARGET_DISTANCE_TOLERANCE = 0.01f;
	public const float VELOCITY_TO_OFFSET_FACTOR = 0.6f;
	public const float VELOCITY_TO_ORTHO_FACTOR = 0.04f;

	public const float MAX_ORTHO_MULTIPLIER = 2f;

	public const int INVALID_TEXT_STAGE = -1;
	public const int TEXT_STAGE_SHOW = 0;
	public const int TEXT_STAGE_PLAIN = 1;
	public const int TEXT_STAGE_HIDE = 2;
	public const int TEXT_STAGE_OVER = 3;

	public const int INVALID_TEXT_EVOLUTION = -1;
	public const int TEXT_EVOLUTION_ASCENT = 0;
	public const int TEXT_EVOLUTION_STAY = 1;
	public const int TEXT_EVOLUTION_NONE_WAIT = 2;
	public const int TEXT_EVOLUTION_NONE_GO = 3;

	public const float TEXT_ASCENT_SPEED = 60f;
	public const float TEXT_ASCENT_DURATION_SHOW = 0.2f;
	public const float TEXT_ASCENT_DURATION_PLAIN = 1.3f;
	public const float TEXT_ASCENT_DURATION_HIDE = 0f;
	public const float TEXT_ASCENT_START_SCALE_X = 3f;
	public const float TEXT_ASCENT_START_SCALE_Y = 1f;
	public const float TEXT_STAY_DURATION_SHOW = 0.6f;
	public const float TEXT_STAY_DURATION_PLAIN = 1.3f;
	public const float TEXT_STAY_DURATION_HIDE = 0.5f;
	public const float TEXT_STAY_START_SCALE_X = 6f;
	public const float TEXT_STAY_START_SCALE_Y = 0.2f;
	public const float TEXT_NONE_DURATION_SHOW = 0.3f;
	public const float TEXT_NONE_DURATION_PLAIN = 0f;
	public const float TEXT_NONE_DURATION_HIDE = 0.3f;
	public const float TEXT_NONE_START_SCALE_X = 2f;
	public const float TEXT_NONE_START_SCALE_Y = 0.5f;

	private Camera cameraComponent;
	private int cameraPixelWidth;
	private int cameraPixelHeight;
	private float originalOrthographicSize;
	private float originalCameraWorldHeight;
	private float originalCameraWorldWidth;
	private float originalCameraUnitsPerPixel;
	private float maxOrthographicSize;
	private float maxCameraWorldWidth;
	private float maxCameraWorldHeight;
	private float maxCameraUnitsPerPixel;
	private bool dimensionsReady;
	private int targetMode;
	private int targetApproach;
	private bool targetReached;
	private float targetPositionX;
	private float targetPositionY;
	private PlayerAgent targetPlayer;
	private Transform targetPlayerTransform;
	private BaseAgent targetNPC;
	private Transform targetNPCTransform;
	private float speedX;
	private float speedY;
	private float lerpFactorPosition;
	private float lerpFactorOrthoSize;
	private bool farCamera;
	private float farCameraOrthoIncrease;
	private bool approachImmediately;
	private GameObject interfaceCanvasObject;
	private Canvas interfaceCanvasComponent;
	private RectTransform interfaceCanvasTrans;
	private Rect interfaceCanvasRect;
	private Rect interfaceCanvasScreenRect;
	private GameObject[] textObjects;
	private RectTransform[] textTransforms;
	private SpritedStringUI[] texts;
	private int[] textEvolutions;
	private Transform[] textOriginTransforms;
	private Vector2[] textOriginPositions;
	private Vector2[] textSpeeds;
	private Vector2[] textIncreasedScales;
	private float[] textDurationsShow;
	private float[] textDurationsPlain;
	private float[] textDurationsHide;
	private int[] textStages;
	private float[] textStageElapseds;
	private float[] textElapseds;
	private bool[] textPauseds;
	private bool paused;
	private bool endWaitingTexts;

	void Awake()
	{
		cameraComponent = null;
		cameraPixelWidth = 0;
		cameraPixelHeight = 0;
		originalOrthographicSize = 0f;
		originalCameraWorldHeight = 0f;
		originalCameraWorldWidth = 0f;
		originalCameraUnitsPerPixel = 0f;
		maxOrthographicSize = 0f;
		maxCameraWorldWidth = 0f;
		maxCameraWorldHeight = 0f;
		maxCameraUnitsPerPixel = 0f;
		cameraComponent = gameObject.GetComponent<Camera>();
		/*halmeida - no use capturing the dimensions of the camera now, for if this
		script is instantiated within an object at the scene from the start, the
		awake call may happen before the screen assumes its correct dimensions.
		The start call, however, happens after the screen changes. The change in
		dimensions happens at the editor, when play testing.*/
		dimensionsReady = false;
		targetMode = TARGET_MODE_INVALID;
		targetApproach = TARGET_APPROACH_INVALID;
		targetReached = false;
		targetPositionX = 0f;
		targetPositionY = 0f;
		targetPlayer = null;
		targetPlayerTransform = null;
		targetNPC = null;
		targetNPCTransform = null;
		speedX = 0f;
		speedY = 0f;
		lerpFactorPosition = 0f;
		lerpFactorOrthoSize = 0f;
		farCamera = false;
		farCameraOrthoIncrease = 0f;
		approachImmediately = false;
		interfaceCanvasObject = null;
		interfaceCanvasComponent = null;
		interfaceCanvasTrans = null;
		interfaceCanvasRect = new Rect( 0f, 0f, 0f, 0f );
		interfaceCanvasScreenRect = new Rect( 0f, 0f, 0f, 0f );
		textObjects = null;
		textTransforms = null;
		texts = null;
		textEvolutions = null;
		textOriginTransforms = null;
		textOriginPositions = null;
		textSpeeds = null;
		textIncreasedScales = null;
		textDurationsShow = null;
		textDurationsPlain = null;
		textDurationsHide = null;
		textStages = null;
		textStageElapseds = null;
		textElapseds = null;
		textPauseds = null;
		paused = false;
		endWaitingTexts = false;
		ResetTargetting();
	}

	public void ResetTargetting()
	{
		targetMode = TARGET_MODE_POSITION;
		targetPositionX = transform.position.x;
		targetPositionY = transform.position.y;
		targetReached = false;
		targetApproach = TARGET_APPROACH_LERP;
		lerpFactorPosition = DEFAULT_LERP_FACTOR;
		GetOrthoLerpFromPositionLerp();
		approachImmediately = false;
	}

	void Start ()
	{
		if( cameraComponent != null )
		{
			cameraPixelWidth = cameraComponent.pixelWidth;
			cameraPixelHeight = cameraComponent.pixelHeight;
			originalOrthographicSize = cameraComponent.orthographicSize;
			originalCameraWorldHeight = originalOrthographicSize * 2f;
			originalCameraUnitsPerPixel = originalCameraWorldHeight / cameraPixelHeight;
			originalCameraWorldWidth = originalCameraUnitsPerPixel * cameraPixelWidth;
			maxOrthographicSize = originalOrthographicSize * MAX_ORTHO_MULTIPLIER;
			maxCameraWorldHeight = maxOrthographicSize * 2f;
			maxCameraUnitsPerPixel = maxCameraWorldHeight / cameraPixelHeight;
			maxCameraWorldWidth = maxCameraUnitsPerPixel * cameraPixelWidth;
			Debug.Log("Debug : CameraController : dimensions captured.");
			dimensionsReady = true;
		}
	}

	public bool GetCameraPixelDimensions( ref int pixelWidth, ref int pixelHeight )
	{
		pixelWidth = cameraPixelWidth;
		pixelHeight = cameraPixelHeight;
		return dimensionsReady;
	}

	public bool GetCameraOriginalWorldDimensions( ref float worldWidth, ref float worldHeight )
	{
		worldWidth = originalCameraWorldWidth;
		worldHeight = originalCameraWorldHeight;
		return dimensionsReady;
	}

	public bool GetCameraMaxWorldDimensions( ref float maxWorldWidth, ref float maxWorldHeight )
	{
		maxWorldWidth = maxCameraWorldWidth;
		maxWorldHeight = maxCameraWorldHeight;
		return dimensionsReady;
	}

	public bool GetCameraOriginalUnitsPerPixel( ref float unitsPerPixel )
	{
		unitsPerPixel = originalCameraUnitsPerPixel;
		return dimensionsReady;
	}

	public bool GetCameraMaxUnitsPerPixel( ref float maxUnitsPerPixel )
	{
		maxUnitsPerPixel = maxCameraUnitsPerPixel;
		return dimensionsReady;
	}

	public void SetTargetPosition( Vector2 newTargetPosition, float newTravelTime, float newLerpFactor = 0f )
	{
		float offsetX = 0f;
		float offsetY = 0f;

		targetPositionX = newTargetPosition.x;
		targetPositionY = newTargetPosition.y;
		targetMode = TARGET_MODE_POSITION;
		targetReached = false;
		if( newTravelTime > 0f )
		{
			targetApproach = TARGET_APPROACH_SPEED;
			offsetX = targetPositionX - transform.position.x;
			offsetY = targetPositionY - transform.position.y;
			speedX = offsetX / newTravelTime;
			speedY = offsetY / newTravelTime;
		}
		else
		{
			targetApproach = TARGET_APPROACH_LERP;
			lerpFactorPosition = (newLerpFactor > 0f) ? newLerpFactor : DEFAULT_LERP_FACTOR;
			GetOrthoLerpFromPositionLerp();
		}
	}

	public bool ReachedTarget()
	{
		return targetReached;
	}

	public void SetTargetPlayer( PlayerAgent newTargetPlayer, float newLerpFactor = 0f )
	{
		targetPlayer = newTargetPlayer;
		if( targetPlayer != null )
		{
			targetPlayerTransform = targetPlayer.gameObject.transform;
			targetMode = TARGET_MODE_PLAYER;
			targetReached = false;
			targetApproach = TARGET_APPROACH_LERP;
			lerpFactorPosition = (newLerpFactor > 0f) ? newLerpFactor : DEFAULT_LERP_FACTOR;
			GetOrthoLerpFromPositionLerp();
		}
		else
		{
			ResetTargetting();
		}
	}

	public void SetTargetNPC( BaseAgent newTargetNPC, float newLerpFactor = 0f )
	{
		targetNPC = newTargetNPC;
		if( targetNPC != null )
		{
			targetNPCTransform = targetNPC.gameObject.transform;
			targetMode = TARGET_MODE_NPC;
			targetReached = false;
			targetApproach = TARGET_APPROACH_LERP;
			lerpFactorPosition = (newLerpFactor > 0f) ? newLerpFactor : DEFAULT_LERP_FACTOR;
			GetOrthoLerpFromPositionLerp();
		}
		else
		{
			if( targetPlayer != null )
			{
				SetTargetPlayer( targetPlayer, lerpFactorPosition );
			}
			else
			{
				ResetTargetting();
			}
		}
	}

	public bool SetTargetMidway( bool enable )
	{
		if( (targetPlayer != null) && (targetNPC != null) )
		{
			if( enable )
			{
				targetMode = TARGET_MODE_MIDWAY;
			}
			else
			{
				targetMode = TARGET_MODE_NPC;
			}
			targetReached = false;
			return true;
		}
		return false;
	}

	public void SetTargetEditor()
	{
		targetMode = TARGET_MODE_EDITOR;
		targetReached = false;
		targetApproach = TARGET_APPROACH_LERP;
		lerpFactorPosition = DEFAULT_LERP_FACTOR;
		GetOrthoLerpFromPositionLerp();
	}

	public Vector3 GetCameraObjectPosition()
	{
		return transform.position;
	}

	private void GetOrthoLerpFromPositionLerp()
	{
		lerpFactorOrthoSize = lerpFactorPosition / 2f;
	}

	public void ApproachTargetInstantly()
	{
		approachImmediately = true;
	}

	public void FixedProgress( float timeStep )
	{
		Vector2 actualTargetPosition = Vector3.zero;
		float actualTargetOrthoSize = 0f;

		/*halmeida - the camera will only do something if it has a valid target mode.*/
		/*halmeida - for each target mode, the actual target world position is obtained differently.*/
		if( GetActualTarget( ref actualTargetPosition, ref actualTargetOrthoSize ) )
		{
			if( approachImmediately )
			{
				transform.position = new Vector3( actualTargetPosition.x, actualTargetPosition.y, transform.position.z );
				if( cameraComponent != null )
				{
					cameraComponent.orthographicSize = actualTargetOrthoSize;
				}
				approachImmediately = false;
				targetReached = true;
			}
			else
			{
				targetReached = CheckIfReachedTargetPosition( actualTargetPosition );
				if( !targetReached )
				{
					MoveTowardsTargetPosition( actualTargetPosition, timeStep );
				}
				if( !CheckIfReachedTargetOrthoSize( actualTargetOrthoSize ) )
				{
					MoveTowardsTargetOrthoSize( actualTargetOrthoSize, timeStep );
				}
			}
		}
	}

	private bool GetActualTarget( ref Vector2 actualTargetPosition, ref float actualTargetOrthoSize )
	{
		bool targetValid = false;

		switch( targetMode )
		{
			case TARGET_MODE_POSITION:
				actualTargetPosition = new Vector2( targetPositionX, targetPositionY );
				actualTargetOrthoSize = originalOrthographicSize;
				targetValid = true;
				break;
			case TARGET_MODE_PLAYER:
				if( targetPlayerTransform != null )
				{
					actualTargetPosition = new Vector2( targetPlayerTransform.position.x, targetPlayerTransform.position.y );
					actualTargetPosition += targetPlayer.GetVelocity() * VELOCITY_TO_OFFSET_FACTOR;
					actualTargetOrthoSize = originalOrthographicSize + targetPlayer.GetVelocity().sqrMagnitude * VELOCITY_TO_ORTHO_FACTOR;
					if( actualTargetOrthoSize > maxOrthographicSize )
					{
						actualTargetOrthoSize = maxOrthographicSize;
					}
					targetValid = true;
				}
				break;
			case TARGET_MODE_NPC:
				if( targetNPCTransform != null )
				{
					actualTargetPosition = new Vector2( targetNPCTransform.position.x, targetNPCTransform.position.y );
					actualTargetPosition += targetNPC.GetVelocity() * VELOCITY_TO_OFFSET_FACTOR;
					actualTargetOrthoSize = originalOrthographicSize + targetNPC.GetVelocity().sqrMagnitude * VELOCITY_TO_ORTHO_FACTOR;
					if( actualTargetOrthoSize > maxOrthographicSize )
					{
						actualTargetOrthoSize = maxOrthographicSize;
					}
					targetValid = true;
				}
				break;
			case TARGET_MODE_MIDWAY:
				if( (targetPlayerTransform != null) && (targetNPCTransform != null) )
				{
					actualTargetPosition = new Vector2( (targetPlayerTransform.position.x + targetNPCTransform.position.x) / 2f,
						(targetPlayerTransform.position.y + targetNPCTransform.position.y) / 2f );
					actualTargetOrthoSize = originalOrthographicSize;
					targetValid = true;
				}
				break;
			case TARGET_MODE_EDITOR:
				actualTargetPosition = new Vector2( transform.position.x, transform.position.y );
				actualTargetOrthoSize = originalOrthographicSize;
				targetValid = true;
				break;
		}
		if( farCamera )
		{
			actualTargetOrthoSize += farCameraOrthoIncrease;
		}
		return targetValid;
	}

	private bool CheckIfReachedTargetPosition( Vector2 actualTargetPosition )
	{
		float offsetX = 0f;
		float offsetY = 0f;
		Vector3 currentPosition = transform.position;

		offsetX = actualTargetPosition.x - currentPosition.x;
		offsetY = actualTargetPosition.y - currentPosition.y;

		/*halmeida - since we will be using, in at least one of the approach modes, linear interpolation to
		come closer to the target, we will, mathematically, never reach it. That is why we need some sort
		of distance tolerance within which we will say that the target was reached.*/
		offsetX *= (offsetX < 0f) ? -1f : 1f;
		offsetY *= (offsetY < 0f) ? -1f : 1f;
		return !( (offsetX > TARGET_DISTANCE_TOLERANCE) || (offsetY > TARGET_DISTANCE_TOLERANCE) );
	}

	private bool CheckIfReachedTargetOrthoSize( float actualTargetOrthoSize )
	{
		float offset = 0f;

		if( cameraComponent != null )
		{
			offset = actualTargetOrthoSize - cameraComponent.orthographicSize;
			/*halmeida - since we will be using, in at least one of the approach modes, linear interpolation to
			come closer to the target, we will, mathematically, never reach it. That is why we need some sort
			of distance tolerance within which we will say that the target was reached.*/
			offset *= (offset < 0f) ? -1f : 1f;
			return !(offset > TARGET_DISTANCE_TOLERANCE);
		}
		return true;
	}

	private void MoveTowardsTargetPosition( Vector2 actualTargetPosition, float timeStep )
	{
		float finalValue = 0f;
		Vector3 finalPosition = Vector3.zero;
		Vector3 currentPosition = Vector3.zero;
		Vector2 current2DPosition = Vector2.zero;
		Vector2 currentVelocity = Vector2.zero;

		switch( targetApproach )
		{
			case TARGET_APPROACH_SPEED:
				currentPosition = transform.position;
				/*halmeida - move camera along the x axis.*/
				finalValue = currentPosition.x;
				if( speedX > 0f )
				{
					if( actualTargetPosition.x > currentPosition.x )
					{
						finalValue = currentPosition.x + (speedX * timeStep);
						if( finalValue > actualTargetPosition.x )
						{
							finalValue = actualTargetPosition.x;
						}
					}
					else
					{
						finalValue = actualTargetPosition.x;
					}
				}
				else if( speedX < 0f )
				{
					if( actualTargetPosition.x < currentPosition.x )
					{
						finalValue = currentPosition.x + (speedX * timeStep);
						if( finalValue < actualTargetPosition.x )
						{
							finalValue = actualTargetPosition.x;
						}
					}
					else
					{
						finalValue = actualTargetPosition.x;
					}
				}
				finalPosition.x = finalValue;
				/*halmeida - move camera along the y axis.*/
				finalValue = currentPosition.y;
				if( speedY > 0f )
				{
					if( actualTargetPosition.y > currentPosition.y )
					{
						finalValue = currentPosition.y + (speedY * timeStep);
						if( finalValue > actualTargetPosition.y )
						{
							finalValue = actualTargetPosition.y;
						}
					}
					else
					{
						finalValue = actualTargetPosition.y;
					}
				}
				else if( speedY < 0f )
				{
					if( actualTargetPosition.y < currentPosition.y )
					{
						finalValue = currentPosition.y + (speedY * timeStep);
						if( finalValue < actualTargetPosition.y )
						{
							finalValue = actualTargetPosition.y;
						}
					}
					else
					{
						finalValue = actualTargetPosition.y;
					}
				}
				finalPosition.y = finalValue;
				/*halmeida - keep the camera's z value unchanged.*/
				finalPosition.z = currentPosition.z;
				/*halmeida - set the camera at the obtained position.*/
				transform.position = finalPosition;
				break;
			case TARGET_APPROACH_LERP:
				currentPosition = transform.position;
				current2DPosition.x = currentPosition.x;
				current2DPosition.y = currentPosition.y;
				finalValue = lerpFactorPosition * timeStep;
				if( finalValue > 1f )
				{
					finalValue = 1;
				}
				current2DPosition = Vector2.Lerp( current2DPosition, actualTargetPosition, finalValue );
				/*halmeida - multiplying the lerpFactor by the timeStep does not garantee uniform
				movement across different framerates, but from some calculations I did, it comes
				quite close to that.*/
				finalPosition.x = current2DPosition.x;
				finalPosition.y = current2DPosition.y;
				finalPosition.z = currentPosition.z;
				transform.position = finalPosition;
				break;
		}
	}

	private void MoveTowardsTargetOrthoSize( float actualTargetOrthoSize, float timeStep )
	{
		float currentOrthoSize = 0f;
		float lerpAmount = 0f;

		if( cameraComponent != null )
		{
			switch( targetApproach )
			{
				case TARGET_APPROACH_SPEED:
					/*halmeida - this should never happen because the target ortho size should only be different
					from the original when dealing with speeding characters, and when the camera is following a
					character it is always in lerp approach mode.*/
					currentOrthoSize = cameraComponent.orthographicSize;
					if( currentOrthoSize < actualTargetOrthoSize )
					{
						currentOrthoSize += timeStep;
						if( currentOrthoSize > actualTargetOrthoSize )
						{
							currentOrthoSize = actualTargetOrthoSize;
						}
					}
					else
					{
						currentOrthoSize -= timeStep;
						if( currentOrthoSize < actualTargetOrthoSize )
						{
							currentOrthoSize = actualTargetOrthoSize;
						}
					}
					cameraComponent.orthographicSize = currentOrthoSize;
					break;
				case TARGET_APPROACH_LERP:
					currentOrthoSize = cameraComponent.orthographicSize;
					lerpAmount = lerpFactorOrthoSize * timeStep;
					if( lerpAmount > 1f )
					{
						lerpAmount = 1f;
					}
					cameraComponent.orthographicSize = Mathf.Lerp( currentOrthoSize, actualTargetOrthoSize, lerpAmount );
					break;
			}
		}
	}

	public float GetOriginalOrthographicSize()
	{
		return originalOrthographicSize;
	}

	public void AlterOrthoSize( float factor )
	{
		if( factor > 0f )
		{
			farCameraOrthoIncrease = (factor - 1f) * originalOrthographicSize;
			farCamera = true;
		}
	}

	public void ResetOrthoSize()
	{
		farCamera = false;
	}

	public void TogglePause( bool pause )
	{
		if( pause )
		{
			if( !paused )
			{
				paused = true;
			}
		}
		else
		{
			if( paused )
			{
				paused = false;
			}
		}
	}

	public void SetInterfaceCanvas( Canvas newInterfaceCanvasComponent )
	{
		bool canvasValid = false;

		interfaceCanvasComponent = newInterfaceCanvasComponent;
		if( interfaceCanvasComponent != null )
		{
			interfaceCanvasObject = interfaceCanvasComponent.gameObject;
			interfaceCanvasTrans = interfaceCanvasObject.GetComponent<RectTransform>();
			if( interfaceCanvasTrans != null )
			{
				interfaceCanvasRect = interfaceCanvasTrans.rect;
				Debug.Log("Debug : CameraController : canvasRect = ("+interfaceCanvasRect.width+", "+interfaceCanvasRect.height+").");
				interfaceCanvasScreenRect = interfaceCanvasComponent.pixelRect;
				Debug.Log("Debug : CameraController : canvasScreenRect = ("+interfaceCanvasScreenRect.width+", "+interfaceCanvasScreenRect.height+").");
				canvasValid = true;
			}
		}
		if( !canvasValid )
		{
			interfaceCanvasObject = null;
			interfaceCanvasComponent = null;
			interfaceCanvasTrans = null;
			interfaceCanvasRect = new Rect( 0f, 0f, 0f, 0f );
			interfaceCanvasScreenRect = new Rect( 0f, 0f, 0f, 0f );
		}
	}

	public RectTransform GetCanvasTransform()
	{
		return interfaceCanvasTrans;
	}

	public bool TransformWorldToCanvasPosition( Vector2 worldPos, ref Vector2 canvasPos )
	{
		float currentOrthographicSize = 0f;
		float currentWorldHeight = 0f;
		float currentUnitsPerPixel = 0f;
		float currentWorldWidth = 0f;
		Vector2 offsetFromCamera = Vector2.zero;
		Vector2 traveledRates = Vector2.zero;

		if( (cameraComponent != null) && (interfaceCanvasComponent != null) )
		{
			currentOrthographicSize = cameraComponent.orthographicSize;
			currentWorldHeight = currentOrthographicSize * 2f;
			currentUnitsPerPixel = currentWorldHeight / cameraPixelHeight;
			currentWorldWidth = currentUnitsPerPixel * cameraPixelWidth; 
			offsetFromCamera = worldPos - new Vector2( transform.position.x, transform.position.y );
			traveledRates = new Vector2( offsetFromCamera.x / currentWorldWidth, offsetFromCamera.y / currentWorldHeight );
			canvasPos = new Vector2( traveledRates.x * interfaceCanvasRect.width, traveledRates.y * interfaceCanvasRect.height );
			return true;
		}
		return false;
	}

	public bool AddInterfaceCanvasText( string textString, SymbolDatabase textFont, Color textColor, Vector4 textColorGradient, int textEvolution,
		Transform originTransform, Vector2 originTransformOffset, bool workDuringPause )
	{
		return AddCanvasText( textString, textFont, textColor, textColorGradient, textEvolution, originTransform, originTransformOffset,
			workDuringPause );
	}

	public bool AddInterfaceCanvasText( string textString, SymbolDatabase textFont, Color textColor, Vector4 textColorGradient, int textEvolution,
		Vector2 originWorldPosition, bool workDuringPause )
	{
		return AddCanvasText( textString, textFont, textColor, textColorGradient, textEvolution, null, originWorldPosition, workDuringPause );
	}

	private bool AddCanvasText( string textString, SymbolDatabase textFont, Color textColor, Vector4 textColorGradient, int textEvolution,
		Transform originTransform, Vector2 originWorldPosition, bool workDuringPause )
	{
		GameObject textObject = null;
		RectTransform textTrans = null;
		SpritedStringUI text = null;
		int textIndex = -1;
		Vector3 completeWorldPos = Vector3.zero;
		Vector2 currentWorldPos = Vector2.zero;
		Vector2 canvasPosition = Vector2.zero;
		Vector2 textSpeed = Vector2.zero;
		Vector2 textIncreasedScale = Vector2.zero;
		Color textGeneralColor = Color.white;
		float textDurationShow = 0f;
		float textDurationPlain = 0f;
		float textDurationHide = 0f;
		int textStage = INVALID_TEXT_STAGE;

		if( originTransform != null )
		{
			completeWorldPos = originTransform.position;
			currentWorldPos = new Vector2( completeWorldPos.x, completeWorldPos.y );
			/*halmeida - in case there is an origin transform, the originWorldPosition vector is interpreted as an offset
			from the transform's position.*/
			currentWorldPos += originWorldPosition;
		}
		else
		{
			currentWorldPos = originWorldPosition;
		}
		if( TransformWorldToCanvasPosition( currentWorldPos, ref canvasPosition ) )
		{
			textIndex = (textObjects == null) ? 0 : textObjects.Length;
			textObject = new GameObject("UIText"+textIndex, typeof(RectTransform));
			if( textObject != null )
			{
				textTrans = textObject.GetComponent<RectTransform>();
				text = textObject.AddComponent<SpritedStringUI>();
				if( text != null )
				{
					text.SetSymbolSource( textFont );
					text.ToggleRaycastTargeting( false );
					text.SetValue( textString );
					textTrans.SetParent( interfaceCanvasTrans, false );
					textTrans.anchoredPosition = canvasPosition;
					switch( textEvolution )
					{
						case TEXT_EVOLUTION_ASCENT:
							textSpeed.y = TEXT_ASCENT_SPEED;
							textGeneralColor = text.GetGeneralColor();
							textGeneralColor.a = 0f;
							text.SetGeneralColor( textGeneralColor );
							textStage = TEXT_STAGE_SHOW;
							textIncreasedScale = new Vector2( TEXT_ASCENT_START_SCALE_X, TEXT_ASCENT_START_SCALE_Y );
							textTrans.localScale = new Vector3( textIncreasedScale.x, textIncreasedScale.y, 1f );
							textDurationShow = TEXT_ASCENT_DURATION_SHOW;
							textDurationPlain = TEXT_ASCENT_DURATION_PLAIN;
							textDurationHide = TEXT_ASCENT_DURATION_HIDE;
							break;
						case TEXT_EVOLUTION_STAY:
							textGeneralColor = text.GetGeneralColor();
							textGeneralColor.a = 0f;
							text.SetGeneralColor( textGeneralColor );
							textStage = TEXT_STAGE_SHOW;
							textIncreasedScale = new Vector2( TEXT_STAY_START_SCALE_X, TEXT_STAY_START_SCALE_Y );
							textTrans.localScale = new Vector3( textIncreasedScale.x, textIncreasedScale.y, 1f );
							textDurationShow = TEXT_STAY_DURATION_SHOW;
							textDurationPlain = TEXT_STAY_DURATION_PLAIN;
							textDurationHide = TEXT_STAY_DURATION_HIDE;
							break;
						case TEXT_EVOLUTION_NONE_WAIT:
						case TEXT_EVOLUTION_NONE_GO:
							textGeneralColor = text.GetGeneralColor();
							textGeneralColor.a = 0f;
							text.SetGeneralColor( textGeneralColor );
							textStage = TEXT_STAGE_SHOW;
							textIncreasedScale = new Vector2( TEXT_NONE_START_SCALE_X, TEXT_NONE_START_SCALE_Y );
							textTrans.localScale = new Vector3( textIncreasedScale.x, textIncreasedScale.y, 1f );
							textDurationShow = TEXT_NONE_DURATION_SHOW;
							textDurationPlain = TEXT_NONE_DURATION_PLAIN;
							textDurationHide = TEXT_NONE_DURATION_HIDE;
							break;
					}
					text.SetColor( textColor, textColorGradient );
					UsefulFunctions.IncreaseArray<GameObject>( ref textObjects, textObject );
					UsefulFunctions.IncreaseArray<RectTransform>( ref textTransforms, textTrans );
					UsefulFunctions.IncreaseArray<SpritedStringUI>( ref texts, text );
					UsefulFunctions.IncreaseArray<int>( ref textEvolutions, textEvolution );
					UsefulFunctions.IncreaseArray<Transform>( ref textOriginTransforms, originTransform );
					UsefulFunctions.IncreaseArray<Vector2>( ref textOriginPositions, originWorldPosition );
					UsefulFunctions.IncreaseArray<Vector2>( ref textSpeeds, textSpeed );
					UsefulFunctions.IncreaseArray<Vector2>( ref textIncreasedScales, textIncreasedScale );
					UsefulFunctions.IncreaseArray<float>( ref textDurationsShow, textDurationShow );
					UsefulFunctions.IncreaseArray<float>( ref textDurationsPlain, textDurationPlain );
					UsefulFunctions.IncreaseArray<float>( ref textDurationsHide, textDurationHide );
					UsefulFunctions.IncreaseArray<int>( ref textStages, textStage );
					UsefulFunctions.IncreaseArray<float>( ref textStageElapseds, 0f );
					UsefulFunctions.IncreaseArray<float>( ref textElapseds, 0f );
					UsefulFunctions.IncreaseArray<bool>( ref textPauseds, workDuringPause );
					return true;
				}
			}
			textTrans = null;
			if( textObject != null )
			{
				Destroy( textObject );
			}
		}
		return false;
	}

	private void RemoveCanvasText( int textIndex )
	{
		if( texts != null )
		{
			if( (textIndex > -1) && (textIndex < texts.Length) )
			{
				texts[textIndex].Clear();
				texts[textIndex] = null;
				textOriginTransforms[textIndex] = null;
				textTransforms[textIndex] = null;
				Destroy( textObjects[textIndex] );
				textObjects[textIndex] = null;
				UsefulFunctions.DecreaseArray<bool>( ref textPauseds, textIndex );
				UsefulFunctions.DecreaseArray<float>( ref textElapseds, textIndex );
				UsefulFunctions.DecreaseArray<float>( ref textStageElapseds, textIndex );
				UsefulFunctions.DecreaseArray<int>( ref textStages, textIndex );
				UsefulFunctions.DecreaseArray<float>( ref textDurationsHide, textIndex );
				UsefulFunctions.DecreaseArray<float>( ref textDurationsPlain, textIndex );
				UsefulFunctions.DecreaseArray<float>( ref textDurationsShow, textIndex );
				UsefulFunctions.DecreaseArray<Vector2>( ref textIncreasedScales, textIndex );
				UsefulFunctions.DecreaseArray<Vector2>( ref textSpeeds, textIndex );
				UsefulFunctions.DecreaseArray<Vector2>( ref textOriginPositions, textIndex );
				UsefulFunctions.DecreaseArray<Transform>( ref textOriginTransforms, textIndex );
				UsefulFunctions.DecreaseArray<int>( ref textEvolutions, textIndex );
				UsefulFunctions.DecreaseArray<SpritedStringUI>( ref texts, textIndex );
				UsefulFunctions.DecreaseArray<RectTransform>( ref textTransforms, textIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref textObjects, textIndex );
			}
		}
	}

	public void ClearCanvasTexts()
	{
		if( texts != null )
		{
			for( int i=0; i<texts.Length; i++ )
			{
				texts[i].Clear();
				texts[i] = null;
				textOriginTransforms[i] = null;
				textTransforms[i] = null;
				Destroy( textObjects[i] );
				textObjects[i] = null;
			}
			textPauseds = null;
			textElapseds = null;
			textStageElapseds = null;
			textStages = null;
			textDurationsHide = null;
			textDurationsPlain = null;
			textDurationsShow = null;
			textIncreasedScales = null;
			textSpeeds = null;
			textOriginPositions = null;
			textOriginTransforms = null;
			textEvolutions = null;
			texts = null;
			textTransforms = null;
			textObjects = null;
		}
	}

	public bool AddCanvasGauge( UIGauge newGauge, float maxDisplayableValue, float minDisplayableValue )
	{
		if( newGauge != null )
		{
			newGauge.Build( interfaceCanvasTrans, maxDisplayableValue, minDisplayableValue );
			return newGauge.WasBuilt();
		}
		return false;
	}

	public void Progress( float timeStep )
	{
		RectTransform textTrans = null;
		bool textPaused = false;
		bool progressText = false;
		int textStage = INVALID_TEXT_STAGE;
		float stageDuration = 0f;
		float stageElapsed = 0f;
		float stageRate = 0f;
		bool keepStage = false;
		SpritedStringUI text = null;
		Color textColor = Color.white;
		Vector3 textScale = Vector3.one;
		Vector2 textIncreasedScale = Vector2.zero;
		bool textRemoved = false;
		int textEvolution = INVALID_TEXT_EVOLUTION;
		float textElapsed = 0f;
		Transform textOriginTrans = null;
		Vector2 textOriginWorld = Vector2.zero;
		Vector2 textOriginCanvas = Vector2.zero;
		Vector3 completeWorldPos = Vector3.zero;
		Vector2 currentOriginWorldPos = Vector2.zero;
		Vector2 textSpeed = Vector2.zero;

		if( textTransforms != null )
		{
			for( int i=0; i<textTransforms.Length; i++ )
			{
				textPaused = textPauseds[i];
				/*halmeida - if a text was added during a game pause, I will progress it during the pause and I will continue
				progressing it after the game is unpaused, because I don't want it to get stuck in the screen until the next pause.
				However, if a text was added when the game was unpaused, I will only progress it while the game is unpaused.*/
				progressText = (textPaused || (!textPaused && !paused));
				if( progressText )
				{
					textTrans = textTransforms[i];
					text = texts[i];
					textStage = textStages[i];
					stageRate = 0f;
					textRemoved = false;
					switch( textStage )
					{
						case TEXT_STAGE_SHOW:
							stageDuration = textDurationsShow[i];
							if( stageDuration <= 0f )
							{
								stageRate = 1f;
							}
							else
							{
								stageElapsed = textStageElapseds[i];
								stageElapsed += timeStep;
								stageRate = stageElapsed / stageDuration;
								stageRate = (stageRate > 1f) ? 1f : stageRate;
								textColor = text.GetGeneralColor();
								textColor.a = Mathf.Lerp( 0f, 1f, stageRate );
								text.SetGeneralColor( textColor );
								textIncreasedScale = textIncreasedScales[i];
								textScale = textTrans.localScale;
								textScale.x = Mathf.Lerp( textIncreasedScale.x, 1f, stageRate );
								textScale.y = Mathf.Lerp( textIncreasedScale.y, 1f, stageRate );
								textTrans.localScale = textScale;
							}
							break;
						case TEXT_STAGE_PLAIN:
							stageDuration = textDurationsPlain[i];
							if( stageDuration <= 0f )
							{
								stageRate = 1f;
							}
							else
							{
								stageElapsed = textStageElapseds[i];
								stageElapsed += timeStep;
								stageRate = stageElapsed / stageDuration;
								stageRate = (stageRate > 1f) ? 1f : stageRate;
							}
							break;
						case TEXT_STAGE_HIDE:
							stageDuration = textDurationsHide[i];
							if( stageDuration <= 0f )
							{
								stageRate = 1f;
							}
							else
							{
								stageElapsed = textStageElapseds[i];
								stageElapsed += timeStep;
								stageRate = stageElapsed / stageDuration;
								stageRate = (stageRate > 1f) ? 1f : stageRate;
								textColor = text.GetGeneralColor();
								textColor.a = Mathf.Lerp( 1f, 0f, stageRate );
								text.SetGeneralColor( textColor );
								textIncreasedScale = textIncreasedScales[i];
								textScale = textTrans.localScale;
								textScale.x = Mathf.Lerp( 1f, textIncreasedScale.x, stageRate );
								textScale.y = Mathf.Lerp( 1f, textIncreasedScale.y, stageRate );
								textTrans.localScale = textScale;
							}
							break;
						case TEXT_STAGE_OVER:
							RemoveCanvasText( i );
							textRemoved = true;
							break;
					}
					if( textRemoved )
					{
						if( textTransforms != null )
						{
							i--;
						}
						else
						{
							break;
						}
					}
					else
					{
						textEvolution = textEvolutions[i];
						if( (textEvolution == TEXT_EVOLUTION_NONE_WAIT) && endWaitingTexts )
						{
							textEvolutions[i] = TEXT_EVOLUTION_NONE_GO;
							textEvolution = TEXT_EVOLUTION_NONE_GO;
						}
						if( stageRate == 1f )
						{
							keepStage = ((textEvolution == TEXT_EVOLUTION_NONE_WAIT) && (textStage == TEXT_STAGE_PLAIN));
							if( !keepStage )
							{
								textStage++;
								textStages[i] = textStage;
								textStageElapseds[i] = 0f;
							}
							else
							{
								textStageElapseds[i] = stageElapsed;
							}
						}
						else
						{
							textStageElapseds[i] = stageElapsed;
						}
						textElapsed = textElapseds[i];
						textElapsed += timeStep;
						textElapseds[i] = textElapsed;
						textOriginTrans = textOriginTransforms[i];
						textOriginWorld = textOriginPositions[i];
						if( textOriginTrans != null )
						{
							completeWorldPos = textOriginTrans.position;
							currentOriginWorldPos = new Vector2( completeWorldPos.x, completeWorldPos.y );
							currentOriginWorldPos += textOriginWorld;
						}
						else
						{
							currentOriginWorldPos = textOriginWorld;
						}
						if( TransformWorldToCanvasPosition( currentOriginWorldPos, ref textOriginCanvas ) )
						{
							textSpeed = textSpeeds[i];
							textTrans.anchoredPosition = textOriginCanvas + (textElapsed * textSpeed);
						}
						text.FeedPositionToMaterial();
					}
				}
			}
		}
		endWaitingTexts = false;
	}

	public void AllowWaitingTextsToEnd()
	{
		endWaitingTexts = true;
	}
}
