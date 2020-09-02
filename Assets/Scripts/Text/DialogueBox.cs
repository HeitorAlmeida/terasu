using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : TextBox
{
	public const float DEFAULT_PORTRAIT_WIDTH = 1f;
	public const float DEFAULT_PORTRAIT_HEIGHT = 1f;
	public const float DEFAULT_MAX_D_BOX_WIDTH = 5.5f;
	public const float DEFAULT_MAX_D_BOX_HEIGHT = 1.5f;

	private string speakerName;
	private Sprite portraitSprite;
	private GameObject portraitObject;
	private SpriteRenderer portraitRenderer;
	private Rect portraitArea;
	private float marginTriple;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void ResetAllClassFields()
	{
		if( cleared )
		{
			speakerName = null;
			portraitSprite = null;
			portraitObject = null;
			portraitRenderer = null;
			portraitArea = new Rect( 0f, 0f, 0f, 0f );
			marginTriple = 0f;
		}
		base.ResetAllClassFields();
	}

	public override void SetSpeakerName (string newSpeakerName)
	{
		speakerName = newSpeakerName;
	}

	public override void SetOrnamentPortrait( Sprite newPortraitSprite )
	{
		portraitSprite = newPortraitSprite;
	}

	protected override void PrepareForOpening()
	{
		base.PrepareForOpening();
		if( portraitRenderer != null )
		{
			portraitRenderer.enabled = false;
		}
	}

	public override void Open()
	{
		base.Open();
		if( opening )
		{
			if( portraitRenderer != null )
			{
				portraitRenderer.enabled = true;
			}
		}
	}

	protected override bool ShouldShowImmediately( int lineIndex )
	{
		return (lineIndex == 0);
	}

	protected override void AdjustBoxSizeLimits()
	{
		marginTriple = 3f * margin;
		if( portraitSprite != null )
		{
			portraitArea.width = DEFAULT_PORTRAIT_WIDTH;
			portraitArea.height = DEFAULT_PORTRAIT_HEIGHT;
			if( maxBoxWidth < portraitArea.width + marginTriple )
			{
				maxBoxWidth = portraitArea.width + marginTriple;
			}
			if( maxBoxHeight < portraitArea.height + marginDouble )
			{
				maxBoxHeight = portraitArea.height + marginDouble;
			}
		}
	}

	protected override void DefineTextAreaSizeLimits()
	{
		if( portraitSprite != null )
		{
			maxTextWidth = maxBoxWidth - marginTriple - portraitArea.width;
			maxTextHeight = maxBoxHeight - marginDouble;
		}
		else
		{
			base.DefineTextAreaSizeLimits();
		}
	}

	protected override void AdjustTextLines()
	{
		GameObject textLineObject = null;
		SpritedString textLineComponent = null;

		if( speakerName != null )
		{
			textLineObject = new GameObject("NameLineObject");
			textLineComponent = textLineObject.AddComponent<SpritedString>();
			textLineComponent.SetSymbolSource( symbolDatabase );
			textLineComponent.SetValue( speakerName );
			textLineComponent.SetColor( new Color( 0.7f, 0f, 0f ) );
			if( !AddExtraTextLine( textLineComponent, true ) )
			{
				textLineComponent.Clear();
				Destroy( textLineObject );
			}
		}
	}

	protected override void DefineBoxArea()
	{
		float requiredHeight = 0f;

		if( boxObject == null )
		{
			boxArea.width = textArea.width + marginDouble;
			boxArea.height = textArea.height + marginDouble;
			if( portraitSprite != null )
			{
				boxArea.width += margin + portraitArea.width;
				requiredHeight = portraitArea.height + marginDouble;
				if( boxArea.height < requiredHeight )
				{
					boxArea.height = requiredHeight;
				} 
			}
			/*halmeida - the area is top-left anchored.*/
			boxArea.x = -boxArea.width / 2f;
			boxArea.y = boxArea.height / 2f;
		}
	}

	protected override void DefineTextAreaOffset()
	{
		textArea.x = boxArea.x + margin;
		textArea.y = boxArea.y - margin;
		if( portraitSprite != null )
		{
			portraitArea.x = textArea.x;
			textArea.x += portraitArea.width + margin;
			portraitArea.y = textArea.y;
		}
	}

	protected override void PlaceOrnaments()
	{
		float scaleFactorWidth = 0f;
		float scaleFactorHeight = 0f;
		Vector3 portraitOffset = Vector3.zero;

		if( (portraitSprite != null) && (portraitObject == null) )
		{
			portraitObject = new GameObject("PortraitObject");
			portraitRenderer = portraitObject.AddComponent<SpriteRenderer>();
			portraitRenderer.sprite = portraitSprite;
			scaleFactorWidth = portraitArea.width / portraitSprite.bounds.size.x;
			scaleFactorHeight = portraitArea.height / portraitSprite.bounds.size.y;
			portraitObject.transform.localScale = new Vector3( scaleFactorWidth, scaleFactorHeight, 1f );
			portraitObject.transform.SetParent( transform, false );
			portraitOffset.x = portraitArea.x + portraitArea.width / 2f;
			portraitOffset.y = portraitArea.y - portraitArea.height / 2f;
			portraitOffset.z = boxToTextDepth;
			portraitObject.transform.localPosition = portraitOffset;
		}
	}

	public override void Clear()
	{
		speakerName = null;
		portraitSprite = null;
		portraitRenderer = null;
		if( portraitObject != null )
		{
			Destroy( portraitObject );
			portraitObject = null;
		}
		base.Clear();
	}
}
