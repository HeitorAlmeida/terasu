using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBox : MonoBehaviour
{
	public const float DEFAULT_MAX_BOX_WIDTH = 8f;
	public const float DEFAULT_MAX_BOX_HEIGHT = 3f;
	public const float DISTANCE_BETWEEN_LINES_FACTOR = 1.0f;
	public const float DEFAULT_TEXT_SPEED = 4f;
	public const float REDUCED_SCALE = 0.1f;
	public const float AUTO_CLOSE_LINE_TIME = 1f;

	public enum TextAlignment { AlignLeft, AlignMiddle, AlignRight }

	public enum ReactionType { ReactAnywhere, ReactPrecisely, NotReact }

	protected SymbolDatabase symbolDatabase;
	protected float maxBoxWidth;
	protected float maxBoxHeight;
	protected float minBoxHeight;
	protected float maxTextWidth;
	protected float maxTextHeight;
	protected Sprite boxSprite;
	protected GameObject boxObject;
	protected SpriteRenderer boxRenderer;
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
	protected bool opening;
	protected bool written;
	protected bool closing;
	protected bool closed;
	protected bool cleared;
	protected bool instantCreation;
	protected ReactionType reactionType;
	protected float boxToTextDepth;
	protected TextAlignment textAlignment;
	protected string text;

	private string[] textLines;
	private GameObject[] textLineObjects;
	private SpritedString[] textLineComponents;
	private bool textLinesReady;
	private float textSpeed;
	private SpritedString currentLine;
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

	protected virtual void Awake()
	{
		cleared = true;
		ResetAllClassFields();
	}

	/*halmeida - carefull when using this method. If any memory is currently allocated, it will not be freed.*/
	protected virtual void ResetAllClassFields()
	{
		if( cleared )
		{
			symbolDatabase = null;
			maxBoxWidth = DEFAULT_MAX_BOX_WIDTH;
			maxBoxHeight = DEFAULT_MAX_BOX_HEIGHT;
			minBoxHeight = 0f;
			maxTextWidth = 0f;
			maxTextHeight = 0f;
			boxSprite = null;
			boxObject = null;
			boxRenderer = null;
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
			opening = false;
			written = false;
			closing = false;
			closed = false;
			cleared = false;
			instantCreation = false;
			reactionType = ReactionType.ReactAnywhere;
			boxToTextDepth = -DisplayDepthManager.ELEMENT_TO_ELEMENT_OFFSET / 100f;
			textAlignment = TextAlignment.AlignLeft;
			text = null;
			textLines = null;
			textLineObjects = null;
			textLineComponents = null;
			textLinesReady = false;
			textSpeed = DEFAULT_TEXT_SPEED;
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
		}
	}

	public void SetBoxLimits( float newMaxBoxWidth, float newMaxBoxHeight )
	{
		if( !built && !cleared )
		{
			maxBoxWidth = (newMaxBoxWidth > 0f) ? newMaxBoxWidth : maxBoxWidth;
			maxBoxHeight = (newMaxBoxHeight > 0f) ? newMaxBoxHeight : maxBoxHeight;
		}
	}

	public void SetEssentials( SymbolDatabase newSymbolDatabase, Sprite newBoxSprite, string newText, float newTextSpeed,
		TextAlignment newTextAlignment, bool newAutoClose, float boxOpeningSpeed )
	{
		if( !built && !cleared )
		{
			symbolDatabase = newSymbolDatabase;
			boxSprite = newBoxSprite;
			text = newText;
			textSpeed = (newTextSpeed >= 0f) ? newTextSpeed : textSpeed;
			symbolDuration = (textSpeed > 0f) ? (1f / textSpeed) : (1f / DEFAULT_TEXT_SPEED);
			textAlignment = newTextAlignment;
			autoClose = newAutoClose;
			openingSpeed = boxOpeningSpeed;
		}
	}

	public virtual void SetSpeakerName( string newSpeakerName )
	{
		/*halmeida - reserved for subclasses.*/
	}

	public virtual void SetOrnamentPortrait( Sprite newPortraitSprite )
	{
		/*halmeida - reserved for subclasses.*/
	}

	public virtual void SetOrnamentGauge( GameObject gaugeObject )
	{
		/*halmeida - reserved for subclasses.*/
	}

	public void ChangeBoxBody( Sprite newBoxBodySprite )
	{
		boxSprite = newBoxBodySprite;
		if( boxRenderer != null )
		{
			boxRenderer.sprite = boxSprite;
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
		transform.localScale = new Vector3( REDUCED_SCALE, REDUCED_SCALE, 1f );
		if( boxRenderer != null )
		{
			boxRenderer.enabled = false;
		}
	}

	public virtual void Open()
	{
		if( built && !open && !closing && !closed )
		{
			if( boxRenderer != null )
			{
				boxRenderer.enabled = true;
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
		Vector3 newScale = Vector3.zero;
		float step = 0f;
		bool xScalingOver = false;
		bool yScalingOver = false;

		if( !immediately )
		{
			newScale = transform.localScale;
			step = openingSpeed * timeStep;
			newScale.x += step;
			if( newScale.x > 1f )
			{
				newScale.x = 1f;
				xScalingOver = true;
			}
			newScale.y += step;
			if( newScale.y > 1f )
			{
				newScale.y = 1f;
				yScalingOver = true;
			}
			transform.localScale = newScale;
			open = (xScalingOver && yScalingOver);
		}
		else
		{
			transform.localScale = new Vector3( 1f, 1f, 1f );
			open = true;
		}
		opening = !open;
	}

	protected virtual void ProgressWriting( bool immediately, float timeStep )
	{
		SpritedString textLineComponent = null;
		int lineLength = 0;
		int totalToReveal = 0;

		if( textLineComponents != null )
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
		Vector3 newScale = Vector3.zero;
		float step = 0f;
		bool xScalingOver = false;
		bool yScalingOver = false;

		if( !immediately )
		{
			newScale = transform.localScale;
			step = openingSpeed * timeStep;
			newScale.x -= step;
			if( newScale.x < REDUCED_SCALE )
			{
				newScale.x = REDUCED_SCALE;
				xScalingOver = true;
			}
			newScale.y -= step;
			if( newScale.y < REDUCED_SCALE )
			{
				newScale.y = REDUCED_SCALE;
				yScalingOver = true;
			}
			transform.localScale = newScale;
			closed = (xScalingOver && yScalingOver);
		}
		else
		{
			transform.localScale = new Vector3( REDUCED_SCALE, REDUCED_SCALE, 1f );
			closed = true;
		}
		closing = !closed;
	}

	public bool Build()
	{
		if( !built && !cleared && (text != null) && (symbolDatabase != null) )
		{
			lineHeight = symbolDatabase.GetStandardSymbolWorldHeight();
			margin = 1.5f * lineHeight;
			marginDouble = 2f * margin;
			AdjustBoxSizeLimits();
			DefineTextAreaSizeLimits();
			AdjustTextContent();
			maxLineWidth = 0f;
			SplitTextIntoLines();
			AdjustTextLines();
			textLinesReady = true;
			DefineTextAreaSize();
			DefineBoxArea();
			boxObject = new GameObject("TextBoxContainer");
			boxObject.transform.SetParent( transform, false );
			boxObject.transform.localPosition = Vector3.zero;
			boxRenderer = boxObject.AddComponent<SpriteRenderer>();
			boxRenderer.drawMode = SpriteDrawMode.Tiled;
			boxRenderer.sprite = boxSprite;
			boxRenderer.size = new Vector2( boxArea.width, boxArea.height );
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
		}
		textArea.width = maxLineWidth;
		textArea.height = textHeight;
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
		/*halmeida - reserved for subclasses.*/
	}

	private void PlacePageLines()
	{
		Vector3 insertionPosition = Vector3.zero;
		Vector3 linePosition = Vector3.zero;
		GameObject textLineObject = null;
		SpritedString textLineComponent = null;
		float textLineWidth = 0f;
		int linesOnPage = 0;
		float insertionX = 0f;

		if( (pageFirstLineIndex > -1) && (pageLastLineIndex > -1) && (textLineObjects != null) )
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
			insertionPosition = new Vector3( insertionX, textArea.y - lineHeight / 2f, boxToTextDepth );
			for( int i=pageFirstLineIndex; i<pageLastLineIndex+1; i++ )
			{
				/*halmeida - relying on the coherence of all the "textLine" arrays.*/
				textLineObject = textLineObjects[i];
				textLineComponent = textLineComponents[i];
				if( (textLineObject != null) && (textLineComponent != null) )
				{
					textLineWidth = textLineComponent.GetWorldDimensions().x;
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
					textLineObject.transform.localPosition = linePosition;
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
		GameObject textLineObject = null;
		SpritedString textLineComponent = null;

		if( (pageFirstLineIndex > -1) && (pageLastLineIndex > -1) && (textLineObjects != null) && (textLineComponents != null) )
		{
			for( int i=pageFirstLineIndex; i<pageLastLineIndex+1; i++ )
			{
				/*halmeida - relying on the coherence of all the "textLine" arrays.*/
				textLineObject = textLineObjects[i];
				textLineComponent = textLineComponents[i];
				if( (textLineObject != null) && (textLineComponent != null) )
				{
					textLineObject.transform.localPosition = Vector3.zero;
					textLineComponent.ToggleAllSymbolVisuals( false );
				}
			}
		}
	}

	private void AdvancePage()
	{
		if( textLines != null )
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
		reactionType = (precise ? ReactionType.ReactPrecisely : ReactionType.ReactAnywhere);
	}

	public virtual bool ReactToPointerDown( float worldPointX, float worldPointY )
	{
		bool react = false;
		Vector3 centerPosition = Vector3.zero;
		Vector2 topLeft = Vector2.zero;

		if( reactionType == ReactionType.NotReact )
		{
			return false;
		}
		if( open && !autoClose )
		{
			if( reactionType == ReactionType.ReactPrecisely )
			{
				centerPosition = transform.position;
				/*halmeida - the boxArea is top-left anchored, so it already has the offsets from the center to the left and
				from the center to the top.*/
				topLeft.x = centerPosition.x + boxArea.x;
				topLeft.y = centerPosition.y + boxArea.y;
				react = UsefulFunctions.AreaContainsPoint( topLeft.x, topLeft.y, boxArea.width, boxArea.height, worldPointX, worldPointY );
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
		SpritedString textLineComponent = null;
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
		textLinesReady = false;
		text = null;
		boxSprite = null;
		boxRenderer = null;
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
		SpritedString textLineComponent = null;
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

		/*halmeida - to get one line I gotta create a long enough SpritedString that fits
		exactly into the maxTextWidth. If I don't have enough characters to fill the width,
		I will just make a shorter line. Adding character by character to the SpritedString
		would be too slow, so I use a sort of bynary search, looking for the maximum valid
		size for each line.*/
		if( (text != null) && (textLines == null) )
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
						textLineObject = new GameObject("TextLineObject"+textLineIndex);
						textLineObject.transform.SetParent( transform, false );
						textLineObject.transform.localPosition = Vector3.zero;
						textLineComponent = textLineObject.AddComponent<SpritedString>();
						textLineComponent.SetSymbolSource( symbolDatabase );
					}
					textLineLength = lineLastIndex - lineFirstIndex + 1;
					textLine = text.Substring( lineFirstIndex, textLineLength );
					textLineWidth = symbolDatabase.GetStringWidthWorldSpace( textLine );
					//Debug.Log("Debug : TextBox : attempting text line width "+textLineWidth+".");
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
						//Debug.Log("Debug : TextBox : line "+textLineIndex+" is \""+textLine+"\".");
						textLineWidth = textLineComponent.GetWorldDimensions().x;
						//Debug.Log("Debug : TextBox : final text line width "+textLineWidth+".");
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
				UsefulFunctions.IncreaseArray<SpritedString>( ref textLineComponents, textLineComponent );
				textLine = null;
				textLineObject = null;
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

	/*halmeida - meant to be called after the splitting of the text into lines but
	before the definition of the text area size, during the building of the box.*/
	protected bool AddExtraTextLine( SpritedString newTextLineComponent, bool atFront )
	{
		string newTextLine = null;
		GameObject newTextLineObject = null;
		int length = 0;
		float textLineWidth = 0f;

		if( (newTextLineComponent != null) && !textLinesReady && !cleared )
		{
			newTextLine = newTextLineComponent.GetValue();
			newTextLineObject = newTextLineComponent.gameObject;
			UsefulFunctions.IncreaseArray<string>( ref textLines, newTextLine );
			UsefulFunctions.IncreaseArray<GameObject>( ref textLineObjects, newTextLineObject );
			UsefulFunctions.IncreaseArray<SpritedString>( ref textLineComponents, newTextLineComponent );
			if( atFront )
			{
				length = textLines.Length;
				for( int i=(length-1); i>0; i-- )
				{
					/*halmeida - relying on the coherence of all the "textLine" arrays.*/
					textLines[i] = textLines[i-1];
					textLineObjects[i] = textLineObjects[i-1];
					textLineComponents[i] = textLineComponents[i-1];
				}
				textLines[0] = newTextLine;
				textLineObjects[0] = newTextLineObject;
				textLineComponents[0] = newTextLineComponent;
			}
			newTextLineObject.transform.SetParent( transform, false );
			newTextLineObject.transform.localPosition = Vector3.zero;
			newTextLineComponent.ToggleAllSymbolVisuals( false );
			textLineWidth = newTextLineComponent.GetWorldDimensions().x;
			if( textLineWidth > maxLineWidth )
			{
				maxLineWidth = textLineWidth;
			}
			return true;
		}
		return false;
	}

	public Vector2 GetBoxWorldDimensions()
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

	public float GetBoxToTextDepth()
	{
		return boxToTextDepth;
	}
}
