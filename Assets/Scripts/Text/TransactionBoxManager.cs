using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionBoxManager
{
	public const float BOX_OPENING_SPEED = 3f;

	private static TransactionBoxManager instance;

	public static TransactionBoxManager Instance
	{
		get
		{
			if( instance == null )
			{
				instance = new TransactionBoxManager();
			}
			return instance;
		}
	}

	private GameObject transactionBoxParent;
	private GameObject[] boxObjects;
	private TransactionBox[] boxComponents;
	private GameObject[] boxSourceObjects;
	private ITransactionListener[] boxSourceListeners;
	private float[] boxSourceDistances;
	private bool[] boxTryAboves;
	private GameObject[] boxTailObjects;
	private SpriteRenderer[] boxTailRenderers;
	private SymbolDatabase symbolDatabase;
	private BoxAppearanceDatabase boxDatabase;
	private int boxAppearanceIndex;
	private Sprite bodySprite;
	private Sprite tailSprite;
	private float tailWidth;
	private float tailHeight;
	private float tailHalfWidth;
	private float tailHalfHeight;
	private GameObject cameraObject;
	private CameraController cameraController;
	private float cameraWidth;
	private float cameraHeight;
	private float cameraHalfWidth;
	private float cameraHalfHeight;
	private float boxDepth;
	private Vector2 boxLimits;

	private TransactionBoxManager()
	{
		transactionBoxParent = null;
		boxObjects = null;
		boxComponents = null;
		boxSourceObjects = null;
		boxSourceListeners = null;
		boxSourceDistances = null;
		boxTryAboves = null;
		boxTailObjects = null;
		boxTailRenderers = null;
		symbolDatabase = null;
		boxDatabase = null;
		boxAppearanceIndex = -1;
		bodySprite = null;
		tailSprite = null;
		tailWidth = 0f;
		tailHeight = 0f;
		tailHalfWidth = 0f;
		tailHalfHeight = 0f;
		cameraObject = null;
		cameraController = null;
		cameraWidth = 0f;
		cameraHeight = 0f;
		cameraHalfWidth = 0f;
		cameraHalfHeight = 0f;
		boxDepth = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_TRANSACTION );
		boxLimits = new Vector2( TextBox.DEFAULT_MAX_BOX_WIDTH, TextBox.DEFAULT_MAX_BOX_HEIGHT );

		transactionBoxParent = new GameObject("TransactionBoxParent");
		transactionBoxParent.transform.position = new Vector3( 0f, 0f, boxDepth );
	}

	public void SetMaterials( SymbolDatabase newSymbolDatabase, BoxAppearanceDatabase newBoxDatabase )
	{
		symbolDatabase = newSymbolDatabase;
		boxDatabase = newBoxDatabase;
		if( boxDatabase != null )
		{
			if( !boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.Transaction, 0, ref bodySprite, ref tailSprite ) )
			{
				boxAppearanceIndex = -1;
				bodySprite = null;
				tailSprite = null;
			}
			else
			{
				boxAppearanceIndex = 0;
				ExtractTailDimensions();
			}
		}
	}

	private void ExtractTailDimensions()
	{
		tailWidth = 0f;
		tailHeight = 0f;
		tailHalfWidth = 0f;
		tailHalfHeight = 0f;
		if( tailSprite != null )
		{
			tailWidth = tailSprite.bounds.size.x;
			tailHeight = tailSprite.bounds.size.y;
			tailHalfWidth = tailWidth / 2f;
			tailHalfHeight = tailHeight / 2f;
		}
	}

	public void AdvanceBoxAppearance()
	{
		int newIndex = -1;
		TransactionBox boxComponent = null;
		SpriteRenderer tailRenderer = null;

		if( (boxDatabase != null) && (boxAppearanceIndex > -1) )
		{
			newIndex = boxAppearanceIndex++;
			if( !boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.System, newIndex, ref bodySprite, ref tailSprite ) )
			{
				newIndex = 0;
				boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.System, newIndex, ref bodySprite, ref tailSprite );
			}
			boxAppearanceIndex = newIndex;
			ExtractTailDimensions();
			if( boxComponents != null )
			{
				for( int i=0; i<boxComponents.Length; i++ )
				{
					boxComponent = boxComponents[i];
					if( boxComponent != null )
					{
						boxComponent.ChangeBoxBody( bodySprite );
					}
				}
			}
			if( boxTailRenderers != null )
			{
				for( int i=0; i<boxTailRenderers.Length; i++ )
				{
					tailRenderer = boxTailRenderers[i];
					if( tailRenderer != null )
					{
						tailRenderer.sprite = tailSprite;
					}
				}
			}
		}
	}

	public void SetCameraController( CameraController newCameraController )
	{
		cameraController = newCameraController;
		if( cameraController != null )
		{
			if( cameraController.GetCameraOriginalWorldDimensions( ref cameraWidth, ref cameraHeight ) )
			{
				cameraHalfWidth = cameraWidth / 2f;
				cameraHalfHeight = cameraHeight / 2f;
				cameraObject = cameraController.gameObject;
			}
		}
	}

	public bool CreateTransaction( GameObject sourceObject, ITransactionListener sourceListener, float sourceDistance,
		int maxValue, int currentValue, int currentValueStep, bool investing, bool tryAbove, GameObject wsGaugeModel )
	{
		GameObject boxObject = null;
		TransactionBox boxComponent = null;
		Vector2 boxDimensions = Vector2.zero;
		Vector2 boxOffset = Vector2.zero;
		Vector3 boxPosition = Vector3.zero;
		bool boxPositionValid = false;
		GameObject boxTailObject = null;
		SpriteRenderer boxTailRenderer = null;
		Vector2 tailOffset = Vector2.zero;

		/*halmeida - if there is already a transaction with the sourceObject, we remove it immediately, before
		creating a new one with the specified values.*/
		RemoveTransaction( sourceObject );
		if( (transactionBoxParent != null) && (symbolDatabase != null) && (sourceObject != null) && (sourceListener != null) )
		{
			boxObject = new GameObject("TransactionBox");
			boxComponent = boxObject.AddComponent<TransactionBox>();
			boxComponent.SetBoxLimits( boxLimits.x, boxLimits.y );
			boxComponent.SetEssentials( symbolDatabase, bodySprite, "TO REPLACE", 0f, TextBox.TextAlignment.AlignMiddle,
				false, BOX_OPENING_SPEED );
			boxComponent.SetOrnamentGauge( wsGaugeModel );
			boxComponent.SetParameters( maxValue, currentValue, currentValueStep, investing );
			boxTailObject = new GameObject("TransactionBoxTail");
			boxTailRenderer = boxTailObject.AddComponent<SpriteRenderer>();
			boxTailRenderer.sprite = tailSprite;

			boxComponent.Build();
			boxDimensions = boxComponent.GetBoxWorldDimensions();
			boxPosition = GetBoxPositionForSource( sourceObject, sourceDistance, boxDimensions, tryAbove, ref boxPositionValid, ref tailOffset );
			if( boxPositionValid )
			{
				boxObject.transform.SetParent( transactionBoxParent.transform, false );
				/*halmeida - since the transactionBoxParent is at x = 0 and y = 0, I can use the local position of its
				children as if it was their position. Changing just the local position is faster.*/
				boxObject.transform.localPosition = boxPosition;
				boxTailObject.transform.SetParent( boxObject.transform, false );
				boxTailObject.transform.localPosition = new Vector3( tailOffset.x, tailOffset.y, boxComponent.GetBoxToTextDepth() );
				if( tailOffset.y > 0f )
				{
					boxTailObject.transform.rotation = Quaternion.Euler( 0f, 0f, 180f );
				}
				boxComponent.Open();
				UsefulFunctions.IncreaseArray<GameObject>( ref boxObjects, boxObject );
				UsefulFunctions.IncreaseArray<TransactionBox>( ref boxComponents, boxComponent );
				UsefulFunctions.IncreaseArray<GameObject>( ref boxSourceObjects, sourceObject );
				UsefulFunctions.IncreaseArray<ITransactionListener>( ref boxSourceListeners, sourceListener );
				UsefulFunctions.IncreaseArray<float>( ref boxSourceDistances, sourceDistance );
				UsefulFunctions.IncreaseArray<bool>( ref boxTryAboves, tryAbove );
				UsefulFunctions.IncreaseArray<GameObject>( ref boxTailObjects, boxTailObject );
				UsefulFunctions.IncreaseArray<SpriteRenderer>( ref boxTailRenderers, boxTailRenderer );
				return true;
			}
			boxComponent.Clear();
			GameObject.Destroy( boxObject );
			GameObject.Destroy( boxTailObject );
		}
		return false;
	}

	private Vector3 GetBoxPositionForSource( GameObject sourceObject, float sourceDistance, Vector2 boxDimensions, bool tryAbove,
		ref bool positionValid, ref Vector2 tailOffset )
	{
		Vector3 boxPosition = Vector3.zero;
		Vector3 sourcePosition = Vector3.zero;
		bool fitsAbove = false;
		bool fitsBelow = false;
		Vector3 cameraPosition = Vector3.zero;
		float cameraTop = 0f;
		float cameraBottom = 0f;
		float cameraLeft = 0f;
		float cameraRight = 0f;
		float boxTop = 0f;
		float boxBottom = 0f;
		float halfBoxWidth = 0f;
		float halfBoxHeight = 0f;

		positionValid = false;
		if( (sourceObject != null) && (cameraObject != null) )
		{
			/*halmeida - gotta place the box at a position that relates to the source's position, is within the
			camera, and tries to respect the above/bellow orientation.*/
			halfBoxHeight = boxDimensions.y / 2f;
			sourcePosition = sourceObject.transform.position;
			cameraPosition = cameraObject.transform.position;
			cameraTop = cameraPosition.y + cameraHalfHeight;
			boxTop = sourcePosition.y + sourceDistance + tailHeight + boxDimensions.y;
			fitsAbove = !( boxTop > cameraTop );
			cameraBottom = cameraPosition.y - cameraHalfHeight;
			boxBottom = sourcePosition.y - sourceDistance - tailHeight - boxDimensions.y;
			fitsBelow = !( boxBottom < cameraBottom );
			if( fitsAbove && fitsBelow )
			{
				if( tryAbove )
				{
					boxPosition.y = boxTop - halfBoxHeight;
					tailOffset.y = -(halfBoxHeight + tailHalfHeight);
				}
				else
				{
					boxPosition.y = boxBottom + halfBoxHeight;
					tailOffset.y = halfBoxHeight + tailHalfHeight;
				}
			}
			else if( fitsAbove )
			{
				boxPosition.y = boxTop - halfBoxHeight;
				tailOffset.y = -(halfBoxHeight + tailHalfHeight);
			}
			else if( fitsBelow )
			{
				boxPosition.y = boxBottom + halfBoxHeight;
				tailOffset.y = halfBoxHeight + tailHalfHeight;
			}
			else
			{
				if( (cameraTop - sourcePosition.y) >= (sourcePosition.y - cameraBottom) )
				{
					boxPosition.y = cameraTop - halfBoxHeight;
					tailOffset.y = -(halfBoxHeight + tailHalfHeight);
				}
				else
				{
					boxPosition.y = cameraBottom + halfBoxHeight;
					tailOffset.y = halfBoxHeight + tailHalfHeight;
				}
			}
			/*halmeida - the y position is set, now we discover the x.*/
			boxPosition.x = sourcePosition.x;
			halfBoxWidth = boxDimensions.x / 2f;
			cameraRight = cameraPosition.x + cameraHalfWidth;
			if( (sourcePosition.x + halfBoxWidth) > cameraRight )
			{
				boxPosition.x = cameraRight - halfBoxWidth;
			}
			cameraLeft = cameraPosition.x - cameraHalfWidth;
			if( (sourcePosition.x - halfBoxWidth) < cameraLeft )
			{
				boxPosition.x = cameraLeft + halfBoxWidth;
			}
			tailOffset.x = sourcePosition.x - boxPosition.x;
			if( ((boxPosition.x + tailOffset.x) + tailHalfWidth) > cameraRight )
			{
				tailOffset.x = halfBoxWidth - tailHalfWidth;
			}
			if( ((boxPosition.x + tailOffset.x) - tailHalfWidth) < cameraLeft )
			{
				tailOffset.x = -halfBoxWidth + tailHalfWidth;
			}
			positionValid = true;
		}
		return boxPosition;
	}

	public void UpdateTransaction( GameObject sourceObject, int newMaxValue, int newCurrentValue, int newCurrentValueStep, bool investing,
		GameObject wsGaugeModel )
	{
		GameObject boxSourceObject = null;
		TransactionBox boxComponent = null;

		if( (sourceObject != null) && (boxSourceObjects != null) )
		{
			for( int i=0; i<boxSourceObjects.Length; i++ )
			{
				boxSourceObject = boxSourceObjects[i];
				if( boxSourceObject == sourceObject )
				{
					boxComponent = boxComponents[i];
					boxComponent.ResetForReconstruction();
					boxComponent.SetBoxLimits( boxLimits.x, boxLimits.y );
					boxComponent.SetEssentials( symbolDatabase, bodySprite, "TO REPLACE", 0f, TextBox.TextAlignment.AlignMiddle,
						false, BOX_OPENING_SPEED );
					boxComponent.SetOrnamentGauge( wsGaugeModel );
					boxComponent.SetParameters( newMaxValue, newCurrentValue, newCurrentValueStep, investing );
					boxComponent.Build();
					return;
				}
			}
		}
	}

	public void CloseTransaction( GameObject sourceObject )
	{
		GameObject boxSourceObject = null;
		TransactionBox boxComponent = null;

		if( (sourceObject != null) && (boxSourceObjects != null) )
		{
			for( int i=0; i<boxSourceObjects.Length; i++ )
			{
				boxSourceObject = boxSourceObjects[i];
				if( boxSourceObject == sourceObject )
				{
					boxComponent = boxComponents[i];
					if( boxComponent != null )
					{
						boxComponent.Close();
					}
					return;
				}
			}
		}
	}

	public void RemoveTransaction( GameObject sourceObject )
	{
		GameObject boxSourceObject = null;

		if( (sourceObject != null) && (boxSourceObjects != null) )
		{
			for( int i=0; i<boxSourceObjects.Length; i++ )
			{
				boxSourceObject = boxSourceObjects[i];
				if( boxSourceObject == sourceObject )
				{
					RemoveTransactionByIndex(i);
					return;
				}
			}
		}
	}

	private void RemoveTransactionByIndex( int transactionIndex )
	{
		GameObject boxTailObject = null;
		TextBox boxComponent = null;
		GameObject boxObject = null;

		if( boxObjects != null )
		{
			if( (transactionIndex > -1) && (transactionIndex < boxObjects.Length) )
			{
				boxTailObject = boxTailObjects[transactionIndex];
				if( boxTailObject != null )
				{
					GameObject.Destroy( boxTailObject );
					boxTailObjects[transactionIndex] = null;
				}
				boxComponent = boxComponents[transactionIndex];
				if( boxComponent != null )
				{
					boxComponent.Clear();
					boxComponents[transactionIndex] = null;
				}
				boxObject = boxObjects[transactionIndex];
				if( boxObject != null )
				{
					GameObject.Destroy( boxObject );
					boxObjects[transactionIndex] = null;
				}
				UsefulFunctions.DecreaseArray<SpriteRenderer>( ref boxTailRenderers, transactionIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref boxTailObjects, transactionIndex );
				UsefulFunctions.DecreaseArray<bool>( ref boxTryAboves, transactionIndex );
				UsefulFunctions.DecreaseArray<float>( ref boxSourceDistances, transactionIndex );
				UsefulFunctions.DecreaseArray<ITransactionListener>( ref boxSourceListeners, transactionIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref boxSourceObjects, transactionIndex );
				UsefulFunctions.DecreaseArray<TransactionBox>( ref boxComponents, transactionIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref boxObjects, transactionIndex );
			}
		}
	}

	public void Progress( float timeStep )
	{
		TransactionBox boxComponent = null;
		GameObject boxSourceObject = null;
		GameObject boxObject = null;
		GameObject boxTailObject = null;
		float boxSourceDistance = 0f;
		Vector2 boxDimensions = Vector2.zero;
		bool boxTryAbove = false;
		Vector2 tailOffset = Vector2.zero;
		Vector3 boxPosition = Vector3.zero;
		bool positionValid = false;
		Vector3 tailLocalPosition = Vector3.zero;

		if( boxComponents != null )
		{
			for( int i=0; i<boxComponents.Length; i++ )
			{
				boxComponent = boxComponents[i];
				if( boxComponent != null )
				{
					boxComponent.Progress( timeStep );
					if( boxComponent.IsCleared() )
					{
						RemoveTransactionByIndex(i);
						if( boxComponents == null )
						{
							break;
						}
						else
						{
							i--;
						}
					}
					else
					{
						/*halmeida - relying on the coherence of all box related arrays.*/
						boxSourceObject = boxSourceObjects[i];
						boxSourceDistance = boxSourceDistances[i];
						boxObject = boxObjects[i];
						boxDimensions = boxComponent.GetBoxWorldDimensions();
						boxTryAbove = boxTryAboves[i];
						boxPosition = GetBoxPositionForSource( boxSourceObject, boxSourceDistance, boxDimensions, boxTryAbove,
							ref positionValid, ref tailOffset );
						if( !positionValid )
						{
							RemoveTransactionByIndex(i);
							if( boxComponents == null )
							{
								break;
							}
							else
							{
								i--;
							}
						}
						else
						{
							boxObject.transform.localPosition = boxPosition;
							boxTailObject = boxTailObjects[i];
							if( boxTailObject != null )
							{
								tailLocalPosition = boxTailObject.transform.localPosition;
								tailLocalPosition.x = tailOffset.x;
								tailLocalPosition.y = tailOffset.y;
								boxTailObject.transform.localPosition = tailLocalPosition;
								if( tailOffset.y > 0f )
								{
									boxTailObject.transform.rotation = Quaternion.Euler( 0f, 0f, 180f );
								}
								else
								{
									boxTailObject.transform.rotation = Quaternion.Euler( 0f, 0f, 0f );
								}
							}
						}
					}
				}
			}
		}
	}

	public bool InterceptPointerDown( int screenPointX, int screenPointY )
	{
		TransactionBox boxComponent = null;
		bool intercepted = false;
		ITransactionListener listener = null;

		if( boxComponents != null )
		{
			for( int i=0; i<boxComponents.Length; i++ )
			{
				boxComponent = boxComponents[i];
				if( boxComponent != null )
				{
					if( boxComponent.ReactToPointerDown( screenPointX, screenPointY ) )
					{
						intercepted = true;
						listener = boxSourceListeners[i];
						if( listener != null )
						{
							listener.AdvanceTransaction();
						}
					}
				}
			}
		}
		return intercepted;
	}

	public bool InterceptActionPress()
	{
		TransactionBox boxComponent = null;
		bool intercepted = false;
		ITransactionListener listener = null;

		if( boxComponents != null )
		{
			for( int i=0; i<boxComponents.Length; i++ )
			{
				boxComponent = boxComponents[i];
				if( boxComponent != null )
				{
					if( boxComponent.ReactToActionPress() )
					{
						intercepted = true;
						listener = boxSourceListeners[i];
						if( listener != null )
						{
							listener.AdvanceTransaction();
						}
					}
				}
			}
		}
		return intercepted;
	}

	public bool InterceptConfirmation()
	{
		return InterceptActionPress();
	}
}
