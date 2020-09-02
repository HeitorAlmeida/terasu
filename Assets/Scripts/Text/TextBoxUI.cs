using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxUI : MonoBehaviour
{
	public enum TextAlignment { AlignLeft, AlignMiddle, AlignRight }

	public enum ReactionType { ReactAnywhere, ReactPrecisely, NotReact }

	public const float DISTANCE_BETWEEN_LINES_FACTOR = 1.0f;
	public const float AUTO_CLOSE_LINE_TIME = 1f;
	public const float AUTO_CLOSE_PICTURE_TIME = 3f;

	protected RectTransform ownTransform;
	protected RectTransform canvasTransform;
	protected SymbolDatabase symbolDatabase;
	protected float maxBoxWidth;
	protected float maxBoxHeight;
	protected bool boxSizeLimitsSet;
	protected float maxTextWidth;
	protected float maxTextHeight;
	protected Sprite boxSprite;
	protected GameObject boxObject;
	protected RectTransform boxTransform;
	protected Image boxImage;
	protected Rect boxArea;
	protected Rect textArea;
	protected float margin;
	protected float marginDouble;
	protected float lineHeight;
	protected float maxLineWidth;
	protected int maxLinesPerPage;
	protected int pageFirstLineIndex;
	protected int pageLastLineIndex;
	protected bool built;
	protected bool open;
	protected float openAlpha;
	protected Vector2 openScale;
	protected bool opening;
	protected bool written;
	protected bool closing;
	protected bool closed;
	protected float closedAlpha;
	protected Vector2 closedScale;
	protected bool cleared;
	protected bool instantCreation;
	protected TextAlignment textAlignment;
	protected ReactionType reactionType;
	protected string text;
	protected Sprite picture;

	private string[] textLines;
	private GameObject[] textLineObjects;
	private RectTransform[] textLineTransforms;
	private SpritedStringUI[] textLineComponents;
	private float textSpeed;
	private SpritedStringUI currentLine;
	private int currentLineIndex;
	private int currentSymbolIndex;
	private int currentLineLength;
	private float symbolDuration;
	private float symbolElapsed;
	private bool writeImmediately;
	private bool autoClose;
	private float autoCloseDuration;
	private float autoCloseElapsed;
	private float openingSpeed;
	private GameObject pictureObject;
	private RectTransform pictureTrans;
	private Image pictureImage;

	protected virtual void Awake()
	{
		ownTransform = GetComponent<RectTransform>();
		cleared = true;
		ResetAllClassFields();
	}

	/*halmeida - carefull when using this method. If any memory is currently allocated, it will not be freed.*/
	protected virtual void ResetAllClassFields()
	{
		if( cleared )
		{
			canvasTransform = null;
			symbolDatabase = null;
			maxBoxWidth = 0f;
			maxBoxHeight = 0f;
			boxSizeLimitsSet = false;
			maxTextWidth = 0f;
			maxTextHeight = 0f;
			boxSprite = null;
			boxObject = null;
			boxTransform = null;
			boxImage = null;
			boxArea = new Rect( 0f, 0f, 0f, 0f );
			textArea = new Rect( 0f, 0f, 0f, 0f );
			margin = 0f;
			marginDouble = 0f;
			lineHeight = 0f;
			maxLineWidth = 0f;
			maxLinesPerPage = 0;
			pageFirstLineIndex = -1;
			pageLastLineIndex = -1;
			built = false;
			open = false;
			openAlpha = 1f;
			openScale = Vector2.one;
			opening = false;
			written = false;
			closing = false;
			closed = false;
			closedAlpha = 0f;
			closedScale = new Vector2( 0.1f, 0.1f );
			cleared = false;
			instantCreation = false;
			reactionType = ReactionType.ReactAnywhere;
			textAlignment = TextAlignment.AlignLeft;
			text = null;
			textLines = null;
			textLineObjects = null;
			textLineTransforms = null;
			textLineComponents = null;
			textSpeed = 0f;
			currentLine = null;
			currentLineIndex = -1;
			currentSymbolIndex = -1;
			currentLineLength = 0;
			symbolDuration = 0f;
			symbolElapsed = 0f;
			writeImmediately = false;
			autoClose = false;
			autoCloseDuration = 0f;
			autoCloseElapsed = 0f;
			openingSpeed = 1f;
			picture = null;
			pictureObject = null;
			pictureTrans = null;
			pictureImage = null;
		}
	}

	public void SetInterfaceCanvasTransform( RectTransform interfaceCanvasTransform )
	{
		canvasTransform = interfaceCanvasTransform;
	}

	public void SetBoxSizeLimitRates( float maxBoxWidthRate, float maxBoxHeightRate )
	{
		if( (canvasTransform != null) && !built && !cleared && (maxBoxWidthRate > 0f) && (maxBoxHeightRate > 0f) )
		{
			maxBoxWidth = maxBoxWidthRate * canvasTransform.rect.width;
			maxBoxHeight = maxBoxHeightRate * canvasTransform.rect.height;
			boxSizeLimitsSet = true;
		}
	}

	public void SetEssentials( SymbolDatabase newSymbolDatabase, Sprite newBoxSprite, string newText, float newTextSpeed,
		TextAlignment newTextAlignment, Sprite newPicture, bool newAutoClose, float boxOriginAlpha, float boxTargetAlpha,
		Vector2 boxOriginScale, Vector2 boxTargetScale, float boxOpeningSpeed )
	{
		if( !built && !cleared )
		{
			symbolDatabase = newSymbolDatabase;
			boxSprite = newBoxSprite;
			text = newText;
			textSpeed = (newTextSpeed >= 0f) ? newTextSpeed : textSpeed;
			symbolDuration = (textSpeed > 0f) ? (1f / textSpeed) : 0f;
			textAlignment = newTextAlignment;
			picture = newPicture;
			autoClose = newAutoClose;
			closedAlpha = boxOriginAlpha;
			openAlpha = boxTargetAlpha;
			closedScale = boxOriginScale;
			openScale = boxTargetScale;
			openingSpeed = boxOpeningSpeed;
		}
	}

	public void ChangeBoxBody( Sprite newBoxBodySprite )
	{
		boxSprite = newBoxBodySprite;
		if( boxImage != null )
		{
			boxImage.sprite = boxSprite;
		}
	}

	public void Progress( float timeStep )
	{
		if( !cleared )
		{
			if( built )
			{
				if( !open )
				{
					if( opening )
					{
						ProgressOpening( false, timeStep );
					}
					else
					{
						if( !closed )
						{
							if( closing )
							{
								ProgressClosing( false, timeStep );
							}
						}
						else
						{
							Clear();
						}
					}
				}
				else
				{
					if( !written )
					{
						ProgressWriting( false, timeStep );
					}
					else
					{
						if( autoCloseDuration > 0f )
						{
							autoCloseElapsed += timeStep;
							if( autoCloseElapsed > autoCloseDuration )
							{
								AdvancePage();
								autoCloseElapsed = 0f;
							}
						}
					}
				}
			}
		}
	}

	protected virtual void PrepareForOpening()
	{
		Color boxColor = Color.black;

		if( ownTransform != null )
		{
			ownTransform.localScale = new Vector3( closedScale.x, closedScale.y, 1f );
			if( boxImage != null )
			{
				boxImage.enabled = false;
				boxColor = boxImage.color;
				boxColor.a = closedAlpha;
				boxImage.color = boxColor;
			}
			if( pictureImage != null )
			{
				pictureImage.enabled = false;
				boxColor = pictureImage.color;
				boxColor.a = closedAlpha;
				pictureImage.color = boxColor;
			}
		}
	}

	public virtual void Open()
	{
		if( built && !open && !closing && !closed && (ownTransform != null) )
		{
			if( boxImage != null )
			{
				boxImage.enabled = true;
			}
			if( pictureImage != null )
			{
				pictureImage.enabled = true;
			}
			opening = true;
		}
	}

	public virtual void Close()
	{
		if( built && (open || opening) )
		{
			RemovePageLines();
			opening = false;
			open = false;
			closing = true;
		}
	}

	protected virtual void ProgressOpening( bool immediately, float timeStep )
	{
		float step = 0f;
		Vector3 newScale = Vector3.zero;
		Color newColor = Color.black;
		float newAlpha = 0f;
		bool xScalingOver = false;
		bool yScalingOver = false;
		bool boxFadingOver = true;
		bool pictureFadingOver = true;

		if( ownTransform != null )
		{
			if( !immediately )
			{
				step = openingSpeed * timeStep;
				newScale = ownTransform.localScale;
				newScale.x += step;
				if( newScale.x > openScale.x )
				{
					newScale.x = openScale.x;
					xScalingOver = true;
				}
				newScale.y += step;
				if( newScale.y > openScale.y )
				{
					newScale.y = openScale.y;
					yScalingOver = true;
				}
				ownTransform.localScale = newScale;
				if( boxImage != null )
				{
					boxFadingOver = false;
					newColor = boxImage.color;
					newAlpha = newColor.a;
					newAlpha += step;
					if( newAlpha > openAlpha )
					{
						newAlpha = openAlpha;
						boxFadingOver = true;
					}
					newColor.a = newAlpha;
					boxImage.color = newColor;
					if( pictureImage != null )
					{
						if( openAlpha > 0f )
						{
							newColor = pictureImage.color;
							newColor.a = newAlpha / openAlpha;
							pictureImage.color = newColor;
						}
						else
						{
							newColor = pictureImage.color;
							newColor.a = 1f;
							pictureImage.color = newColor;
						}
					}
				}
				else
				{
					if( pictureImage != null )
					{
						pictureFadingOver = false;
						newColor = pictureImage.color;
						newAlpha = newColor.a;
						newAlpha += step;
						if( newAlpha > 1f )
						{
							newAlpha = 1f;
							pictureFadingOver = true;
						}
						newColor.a = newAlpha;
						pictureImage.color = newColor;
					}
				}
				open = (xScalingOver && yScalingOver && boxFadingOver && pictureFadingOver);
			}
			else
			{
				ownTransform.localScale = new Vector3( openScale.x, openScale.y, 1f );
				if( boxImage != null )
				{
					newColor = boxImage.color;
					newColor.a = openAlpha;
					boxImage.color = newColor;
				}
				if( pictureImage != null )
				{
					newColor = pictureImage.color;
					newColor.a = 1f;
					pictureImage.color = newColor;
				}
				open = true;
			}
			opening = !open;
		}
	}

	protected virtual void ProgressWriting( bool immediately, float timeStep )
	{
		SpritedStringUI textLineComponent = null;
		int lineLength = 0;
		int totalToReveal = 0;

		if( pictureObject != null )
		{
			written = true;
			writeImmediately = false;
			if( autoCloseDuration > 0f )
			{
				autoCloseElapsed = 0f;
			}
		}
		else if( textLineComponents != null )
		{
			if( currentLineIndex < 0 )
			{
				currentLineIndex = pageFirstLineIndex;
			}
			if( immediately )
			{
				writeImmediately = true;
			}
			if( (textSpeed == 0f) || writeImmediately )
			{
				if( currentLineIndex > -1 )
				{
					for( int i=currentLineIndex; i<pageLastLineIndex+1; i++ )
					{
						if( textLineComponents.Length > i )
						{
							textLineComponent = textLineComponents[i];
							if( textLineComponent != null )
							{
								textLineComponent.ToggleAllSymbolVisuals( true );
							}
						}
					}
				}
				written = true;
				writeImmediately = false;
			}
			else
			{
				if( currentLine == null )
				{
					if( (currentLineIndex > -1) && (currentLineIndex <= pageLastLineIndex) && (textLineComponents.Length > currentLineIndex) )
					{
						textLineComponent = textLineComponents[currentLineIndex];
						if( textLineComponent != null )
						{
							lineLength = textLineComponent.GetTotalSymbols();
							if( lineLength > 0 )
							{
								currentLine = textLineComponent;
								currentSymbolIndex = 0;
								currentLineLength = lineLength;
								textLineComponent.ToggleSymbolVisual( currentSymbolIndex, true );
								symbolElapsed = 0f;
							}
						}
					}
					if( currentLine == null )
					{
						written = true;
					}
				}
				else
				{
					if( ShouldShowImmediately( currentLineIndex ) )
					{
						totalToReveal = currentLineLength;
					}
					else
					{
						symbolElapsed += textSpeed * timeStep;
						totalToReveal = (int)(symbolElapsed / symbolDuration);
					}
					if( totalToReveal > 0 )
					{
						for( int i=totalToReveal; i>0; i-- )
						{
							currentSymbolIndex++;
							if( currentSymbolIndex < currentLineLength )
							{
								currentLine.ToggleSymbolVisual( currentSymbolIndex, true );
							}
							else
							{
								currentLineIndex++;
								currentLine = null;
								break;
							}
						}
						symbolElapsed = 0f;
					}
				}
			}
			if( written )
			{
				if( autoCloseDuration > 0f )
				{
					autoCloseElapsed = 0f;
				}
			}
		}
	}

	protected virtual bool ShouldShowImmediately( int lineIndex )
	{
		return false;
	}

	protected virtual void ProgressClosing( bool immediately, float timeStep )
	{
		float step = 0f;
		Vector3 newScale = Vector3.zero;
		Color newColor = Color.black;
		float newAlpha = 0f;
		bool xScalingOver = false;
		bool yScalingOver = false;
		bool boxFadingOver = true;
		bool pictureFadingOver = true;

		if( ownTransform != null )
		{
			if( !immediately )
			{
				step = openingSpeed * timeStep;
				newScale = ownTransform.localScale;
				newScale.x -= step;
				if( newScale.x < closedScale.x )
				{
					newScale.x = closedScale.x;
					xScalingOver = true;
				}
				newScale.y -= step;
				if( newScale.y < closedScale.y )
				{
					newScale.y = closedScale.y;
					yScalingOver = true;
				}
				ownTransform.localScale = newScale;
				if( boxImage != null )
				{
					boxFadingOver = false;
					newColor = boxImage.color;
					newAlpha = newColor.a;
					newAlpha -= step;
					if( newAlpha < closedAlpha )
					{
						newAlpha = closedAlpha;
						boxFadingOver = true;
					}
					newColor.a = newAlpha;
					boxImage.color = newColor;
					if( pictureImage != null )
					{
						if( openAlpha > 0f )
						{
							newColor = pictureImage.color;
							newColor.a = newAlpha / openAlpha;
							pictureImage.color = newColor;
						}
						else
						{
							newColor = pictureImage.color;
							newColor.a = closedAlpha;
							pictureImage.color = newColor;
						}
					}
				}
				else
				{
					if( pictureImage != null )
					{
						pictureFadingOver = false;
						newColor = pictureImage.color;
						newAlpha = newColor.a;
						newAlpha -= step;
						if( newAlpha < closedAlpha )
						{
							newAlpha = closedAlpha;
							pictureFadingOver = true;
						}
						newColor.a = newAlpha;
						pictureImage.color = newColor;
					}
				}
				closed = (xScalingOver && yScalingOver && boxFadingOver && pictureFadingOver);
			}
			else
			{
				ownTransform.localScale = new Vector3( closedScale.x, closedScale.y, 1f );
				if( boxImage != null )
				{
					newColor = boxImage.color;
					newColor.a = closedAlpha;
					boxImage.color = newColor;
				}
				if( pictureImage != null )
				{
					newColor = pictureImage.color;
					newColor.a = closedAlpha;
					pictureImage.color = newColor;
				}
				closed = true;
			}
			closing = !closed;
		}
	}

	public bool Build()
	{
		if( !built && !cleared && ((text != null) || (picture != null)) && (symbolDatabase != null) && (ownTransform != null) &&
			(canvasTransform != null) && boxSizeLimitsSet )
		{
			lineHeight = symbolDatabase.GetStandardSymbolUIHeight();
			margin = 1.5f * lineHeight;
			marginDouble = 2f * margin;
			AdjustBoxSizeLimits();
			DefineTextAreaSizeLimits();
			AdjustTextContent();
			maxLineWidth = 0f;
			SplitTextIntoLines();
			AdjustTextLines();
			DefineTextAreaSize();
			DefineBoxArea();
			boxObject = new GameObject("UITextBoxContainer", typeof(RectTransform));
			boxTransform = boxObject.GetComponent<RectTransform>();
			boxTransform.SetParent( ownTransform, false );
			boxTransform.anchoredPosition = Vector2.zero;
			boxImage = boxObject.AddComponent<Image>();
			boxImage.type = Image.Type.Tiled;
			boxImage.sprite = boxSprite;
			boxTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, boxArea.width );
			boxTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, boxArea.height );
			DefineTextAreaOffset();
			PlaceOrnaments();
			PlacePageLines();
			if( !instantCreation )
			{
				PrepareForOpening();
			}
			built = true;
			if( instantCreation )
			{
				ProgressOpening( true, 0f );
				ProgressWriting( true, 0f );
				instantCreation = false;
			}
			return true;
		}
		return false;
	}

	protected virtual void AdjustBoxSizeLimits()
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void DefineTextAreaSizeLimits()
	{
		maxTextWidth = maxBoxWidth - marginDouble;
		maxTextHeight = maxBoxHeight - marginDouble;
	}

	protected virtual void AdjustTextContent()
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void AdjustTextLines()
	{
		/*halmeida - reserved for subclasses.*/
	}

	protected virtual void DefineTextAreaSize()
	{
		float textHeight = 0f;

		if( textLines != null )
		{
			for( int i=0; i<textLines.Length; i++ )
			{
				if( textLines[i] != null )
				{
					if( i != 0 )
					{
						textHeight += DISTANCE_BETWEEN_LINES_FACTOR * lineHeight;
					}
					textHeight += lineHeight;
					if( textHeight > maxTextHeight )
					{
						/*halmeida - too many lines within a single page, gotta push lines to next page.
						If it is still the first line, we forcibly accept it cause the limit is just too small.*/
						if( i != 0 )
						{
							textHeight -= (DISTANCE_BETWEEN_LINES_FACTOR + 1.0f) * lineHeight;
							break;
						}
					}
					maxLinesPerPage++;
					if( i == 0 )
					{
						pageFirstLineIndex = 0;
						pageLastLineIndex = 0;
					}
					else
					{
						pageLastLineIndex++;
					}
				}
			}
			textArea.width = maxLineWidth;
			textArea.height = textHeight;
		}
		else
		{
			textArea.width = maxTextWidth;
			textArea.height = maxTextHeight;
		}
	}

	protected virtual void DefineBoxArea()
	{
		if( boxObject == null )
		{
			boxArea.width = textArea.width + marginDouble;
			boxArea.height = textArea.height + marginDouble;
			/*halmeida - the area is top-left anchored.*/
			boxArea.x = -boxArea.width / 2f;
			boxArea.y = boxArea.height / 2f;
		}
	}

	protected virtual void DefineTextAreaOffset()
	{
		textArea.x = boxArea.x + margin;
		textArea.y = boxArea.y - margin;
	}

	protected virtual void PlaceOrnaments()
	{
		float widthFactor = 1f;
		float heightFactor = 1f;
		float scaleChange = 1f;

		if( picture != null )
		{
			pictureObject = new GameObject("TextBoxUIPicture", typeof(RectTransform));
			pictureTrans = pictureObject.GetComponent<RectTransform>();
			pictureTrans.SetParent( ownTransform, false );
			pictureTrans.anchoredPosition = Vector2.zero;
			pictureImage = pictureObject.AddComponent<Image>();
			pictureImage.sprite = picture;
			pictureImage.SetNativeSize();
			heightFactor = textArea.height / pictureTrans.rect.height;
			widthFactor = textArea.width / pictureTrans.rect.width;
			scaleChange = (heightFactor < widthFactor) ? heightFactor : widthFactor;
			pictureTrans.localScale = new Vector3( scaleChange, scaleChange, 1f );
			if( autoClose )
			{
				autoCloseDuration = AUTO_CLOSE_PICTURE_TIME;
				autoCloseElapsed = 0f;
			}
		}
	}

	private void PlacePageLines()
	{
		Vector2 insertionPosition = Vector2.zero;
		Vector2 linePosition = Vector2.zero;
		GameObject textLineObject = null;
		RectTransform textLineTrans = null;
		SpritedStringUI textLineComponent = null;
		float textLineWidth = 0f;
		int linesOnPage = 0;
		float insertionX = 0f;

		if( (pageFirstLineIndex > -1) && (pageLastLineIndex > -1) && (textLineObjects != null) && (pictureObject == null) )
		{
			switch( textAlignment )
			{
				case TextAlignment.AlignLeft:
					insertionX = textArea.x;
					break;
				case TextAlignment.AlignMiddle:
					insertionX = textArea.x + textArea.width / 2f;
					break;
				case TextAlignment.AlignRight:
					insertionX = textArea.x + textArea.width;
					break;
			}
			insertionPosition = new Vector2( insertionX, textArea.y - lineHeight / 2f );
			for( int i=pageFirstLineIndex; i<pageLastLineIndex+1; i++ )
			{
				/*halmeida - relying on the coherence of all the "textLine" arrays.*/
				textLineObject = textLineObjects[i];
				textLineTrans = textLineTransforms[i];
				textLineComponent = textLineComponents[i];
				if( (textLineObject != null) && (textLineTrans != null) && (textLineComponent != null) && (ownTransform != null) )
				{
					textLineWidth = textLineComponent.GetUIDimensions().x;
					linePosition = insertionPosition;
					switch( textAlignment )
					{
						case TextAlignment.AlignLeft:
							linePosition.x += textLineWidth / 2f;
							break;
						case TextAlignment.AlignRight:
							linePosition.x -= textLineWidth / 2f;
							break;
					}
					textLineTrans.SetParent( ownTransform, false );
					textLineTrans.anchoredPosition = linePosition;
					insertionPosition.y -= (DISTANCE_BETWEEN_LINES_FACTOR + 1.0f) * lineHeight;
				}
			}
			if( autoClose )
			{
				linesOnPage = pageLastLineIndex - pageFirstLineIndex + 1;
				autoCloseDuration = linesOnPage * AUTO_CLOSE_LINE_TIME;
				autoCloseElapsed = 0f;
			}
		}
	}

	private void RemovePageLines()
	{
		RectTransform textLineTrans = null;
		SpritedStringUI textLineComponent = null;

		if( (pageFirstLineIndex > -1) && (pageLastLineIndex > -1) && (textLineTransforms != null) && (textLineComponents != null) )
		{
			for( int i=pageFirstLineIndex; i<pageLastLineIndex+1; i++ )
			{
				/*halmeida - relying on the coherence of all the "textLine" arrays.*/
				textLineTrans = textLineTransforms[i];
				textLineComponent = textLineComponents[i];
				if( (textLineTrans != null) && (textLineComponent != null) )
				{
					textLineTrans.SetParent( null );
					textLineComponent.ToggleAllSymbolVisuals( false );
				}
			}
		}
	}

	private void AdvancePage()
	{
		if( pictureObject != null )
		{
			if( textLines != null )
			{
				pictureImage = null;
				pictureTrans = null;
				Destroy( pictureObject );
				pictureObject = null;
				PlacePageLines();
				written = false;
			}
			else
			{
				open = false;
				closing = true;
			}
		}
		else if( textLines != null )
		{
			RemovePageLines();
			pageFirstLineIndex = pageLastLineIndex + 1;
			if( pageFirstLineIndex < textLines.Length )
			{
				pageLastLineIndex = pageFirstLineIndex + maxLinesPerPage - 1;
				if( pageLastLineIndex >= textLines.Length )
				{
					pageLastLineIndex = textLines.Length - 1;
				}
				PlacePageLines();
				written = false;
				currentLineIndex = -1;
				currentLine = null;
				currentSymbolIndex = -1;
			}
			else
			{
				open = false;
				closing = true;
			}
		}
	}

	public void BlockReactions()
	{
		reactionType = ReactionType.NotReact;
	}

	public void AllowReactions( bool precise )
	{
		reactionType = precise ? ReactionType.ReactPrecisely : ReactionType.ReactAnywhere;
	}

	public virtual bool ReactToPointerDown( Vector2 canvasPosition )
	{
		bool react = false;
		Vector2 topLeft = Vector2.zero;

		if( reactionType == ReactionType.NotReact )
		{
			return false;
		}
		if( open && !autoClose && (ownTransform != null) )
		{
			if( reactionType == ReactionType.ReactPrecisely )
			{
				/*halmeida - the boxArea is top-left anchored, so it already has the offsets from the center to the left and
				from the center to the top.*/
				topLeft.x = ownTransform.anchoredPosition.x + boxArea.x;
				topLeft.y = ownTransform.anchoredPosition.y + boxArea.y;
				react = UsefulFunctions.AreaContainsPoint( topLeft.x, topLeft.y, boxArea.width, boxArea.height, canvasPosition.x, canvasPosition.y );
			}
			else
			{
				react = true;
			}
			if( react )
			{
				ReactToInteraction();
				return true;
			}
		}
		return false;
	}

	public virtual bool ReactToActionPress()
	{
		if( reactionType == ReactionType.NotReact )
		{
			return false;
		}
		if( open && !autoClose && (reactionType == ReactionType.ReactAnywhere) )
		{
			ReactToInteraction();
			return true;
		}
		return false;
	}

	protected virtual void ReactToInteraction()
	{
		if( written )
		{
			AdvancePage();
		}
		else
		{
			writeImmediately = true;
		}
	}

	public virtual void Clear()
	{
		SpritedStringUI textLineComponent = null;
		GameObject textLineObject = null;

		if( textLineComponents != null )
		{
			for( int i=0; i<textLineComponents.Length; i++ )
			{
				textLineComponent = textLineComponents[i];
				if( textLineComponent != null )
				{
					textLineComponent.Clear();
					textLineComponents[i] = null;
				}
			}
			textLineComponents = null;
		}
		if( textLineTransforms != null )
		{
			for( int i=0; i<textLineTransforms.Length; i++ )
			{
				textLineTransforms[i] = null;
			}
			textLineTransforms = null;
		}
		if( textLineObjects != null )
		{
			for( int i=0; i<textLineObjects.Length; i++ )
			{
				textLineObject = textLineObjects[i];
				if( textLineObject != null )
				{
					Destroy( textLineObject );
					textLineObjects[i] = null;
				}
			}
			textLineObjects = null;
		}
		if( textLines != null )
		{
			for( int i=0; i<textLines.Length; i++ )
			{
				textLines[i] = null;
			}
			textLines = null;
		}
		text = null;
		if( pictureObject != null )
		{
			pictureImage = null;
			pictureTrans = null;
			Destroy( pictureObject );
			pictureObject = null;
		}
		picture = null;
		boxSprite = null;
		boxImage = null;
		boxTransform = null;
		if( boxObject != null )
		{
			Destroy( boxObject );
			boxObject = null;
		}
		symbolDatabase = null;
		built = false;
		cleared = true;
	}

	public void ResetForReconstruction()
	{
		Clear();
		ResetAllClassFields();
		instantCreation = true;
	}

	private void SplitTextIntoLines()
	{
		GameObject textLineObject = null;
		RectTransform textLineTrans = null;
		SpritedStringUI textLineComponent = null;
		string textLine = null;
		int textLineLength = 0;
		int textLineIndex = -1;
		float textLineWidth = 0f;
		float interval = 0f;
		float position = 0f;
		int lineFirstIndex = -1;
		int lineLastIndex = -1;
		int lineLastIndexValid = -1;
		int lineLastIndexNext = -1;
		int lineLastIndexPrevious = -1;
		bool lineReady = false;
		bool allLinesReady = false;
		bool spaceFound = false;

		/*halmeida - to get one line I gotta create a long enough SpritedStringUI that fits
		exactly into the maxTextWidth. If I don't have enough characters to fill the width,
		I will just make a shorter line. The problem is that I'm able to see the width of a
		sprited string only after I set its value with all the characters it is supposed to
		have. Adding character by character to the SpritedString would be too slow, so I use
		a sort of bynary search, looking for the maximum valid size for each line.*/
		if( (text != null) && (textLines == null) && (ownTransform != null) )
		{
			textLineIndex = 0;
			lineFirstIndex = 0;
			lineLastIndex = text.Length-1;
			while( !allLinesReady )
			{
				lineLastIndexValid = -1;
				interval = lineLastIndex - lineFirstIndex + 1;
				position = lineFirstIndex + interval;
				while( !lineReady )
				{
					if( textLineObject == null )
					{
						textLineObject = new GameObject("TextBoxUILine"+textLineIndex, typeof(RectTransform));
						textLineTrans = textLineObject.GetComponent<RectTransform>();
						/*halmeida - we should not add the lines as children of the object yet because the box
						image itself has not yet been added as a child of the object. The lines would get behind
						it if they were added now.*/
						textLineComponent = textLineObject.AddComponent<SpritedStringUI>();
						textLineComponent.SetSymbolSource( symbolDatabase );
					}
					textLineLength = lineLastIndex - lineFirstIndex + 1;
					textLine = text.Substring( lineFirstIndex, textLineLength );
					textLineWidth = symbolDatabase.GetStringWidthUI( textLine );
					//Debug.Log("Debug : TextBoxUI : attempting text line width "+textLineWidth+".");
					interval = interval / 2f;
					if( textLineWidth > maxTextWidth )
					{
						position -= interval;
					}
					else
					{
						lineLastIndexValid = lineLastIndex;
						position += interval;
					}
					lineLastIndexNext = (int)position;
					/*halmeida - the position value itself should never be the same, but since it is rounded
					to an integer index, we may end up falling back to a previously checked index. When that
					happens, it means the interval has become small enough to stop the search.*/
					if( (lineLastIndexNext == lineLastIndexPrevious) || (lineLastIndexNext == lineLastIndex) ||
						(lineLastIndexNext > text.Length-1) )
					{
						if( lineLastIndexValid == -1 )
						{
							/*halmeida - after all the searching, no valid size was found. This probably means
							the maxTextWidth is just too small to fit even one character. Even so we will
							forcibly accept a one character wide line.*/
							lineLastIndexValid = lineFirstIndex;
						}
						if( lineLastIndexValid > lineFirstIndex )
						{
							/*halmeida - if there is more than one character in the line, we can check for
							word integrity. We cannot break a word into two lines. This means that the last
							character in a line that is not the last line has to be an empty space or the
							space has to be the first character in the next line.*/
							if( (lineLastIndexValid + 1) < text.Length )
							{
								spaceFound = false;
								for( int i=(lineLastIndexValid+1); i>lineFirstIndex; i-- )
								{
									if( text[i] == ' ' )
									{
										lineLastIndexValid = i-1;
										spaceFound = true;
									}
									else
									{
										if( spaceFound )
										{
											break;
										}
									}
								}
							}
						}
						lineReady = true;
						/*halmeida - we didn't necessarily end the search at a valid size, but the last valid
						size found is the biggest possible one. So we use that value to build the line.*/
						textLineLength = lineLastIndexValid - lineFirstIndex + 1;
						textLine = text.Substring( lineFirstIndex, textLineLength );
						textLineComponent.SetValue( textLine );
						textLineComponent.ToggleAllSymbolVisuals( false );
						//Debug.Log("Debug : TextBoxUI : line "+textLineIndex+" is \""+textLine+"\".");
						textLineWidth = textLineComponent.GetUIDimensions().x;
						//Debug.Log("Debug : TextBoxUI : final text line width "+textLineWidth+".");
						if( textLineWidth > maxLineWidth )
						{
							maxLineWidth = textLineWidth;
						}
					}
					else
					{
						lineLastIndexPrevious = lineLastIndex;
						lineLastIndex = lineLastIndexNext;
					}
				}
				UsefulFunctions.IncreaseArray<string>( ref textLines, textLine );
				UsefulFunctions.IncreaseArray<GameObject>( ref textLineObjects, textLineObject );
				UsefulFunctions.IncreaseArray<RectTransform>( ref textLineTransforms, textLineTrans );
				UsefulFunctions.IncreaseArray<SpritedStringUI>( ref textLineComponents, textLineComponent );
				textLine = null;
				textLineObject = null;
				textLineTrans = null;
				textLineComponent = null;
				if( lineLastIndexValid == (text.Length-1) )
				{
					allLinesReady = true;
				}
				else
				{
					textLineIndex++;
					lineFirstIndex = lineLastIndexValid+1;
					for( int i=lineFirstIndex; i<text.Length; i++ )
					{
						if( text[i] == ' ' )
						{
							lineFirstIndex++;
							if( lineFirstIndex == text.Length )
							{
								allLinesReady = true;
							}
						}
						else
						{
							break;
						}
					}
					lineLastIndex = text.Length-1;
					lineReady = false;
				}
			}
		}
	}

	public Vector2 GetBoxUIDimensions()
	{
		return new Vector2( boxArea.width, boxArea.height );
	}

	public bool IsOpen()
	{
		return open;
	}

	public bool IsWritten()
	{
		return written;
	}

	public bool IsCleared()
	{
		return cleared;
	}

	public bool AutoCloses()
	{
		return autoClose;
	}
}
