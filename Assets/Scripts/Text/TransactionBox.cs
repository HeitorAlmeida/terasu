using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionBox : TextBox
{
	private int maxValue;
	private int currentValue;
	private int currentValueStep;
	private bool invest;
	private GameObject gaugeModel;
	private GameObject gaugeObject;
	private WorldSpaceGauge gaugeComponent;
	private Rect gaugeArea;
	private float marginQuad;
	private string stepLine;
	private GameObject stepLineObject;
	private SpritedString stepLineComponent;
	private Rect stepLineArea;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void ResetAllClassFields()
	{
		if( cleared )
		{
			maxValue = 0;
			currentValue = 0;
			currentValueStep = 0;
			invest = true;
			gaugeModel = null;
			gaugeObject = null;
			gaugeComponent = null;
			gaugeArea = new Rect( 0f, 0f, 0f, 0f );
			marginQuad = 0f;
			stepLine = null;
			stepLineObject = null;
			stepLineComponent = null;
			stepLineArea = new Rect( 0f, 0f, 0f, 0f );
		}
		base.ResetAllClassFields();
	}

	public override void SetOrnamentGauge( GameObject newGaugeModel )
	{
		gaugeModel = newGaugeModel;
	}

	public void SetParameters( int newMaxValue, int newCurrentValue, int newCurrentValueStep, bool investing )
	{
		maxValue = newMaxValue;
		maxValue *= (maxValue < 0) ? -1 : 1;
		currentValue = newCurrentValue;
		currentValue *= (currentValue < 0) ? -1 : 1;
		currentValueStep = newCurrentValueStep;
		currentValueStep *= (currentValueStep < 0) ? -1 : 1;
		invest = investing;
	}

	protected override void PrepareForOpening()
	{
		base.PrepareForOpening();
		if( gaugeComponent != null )
		{
			gaugeComponent.ToggleVisibility( false );
		}
		if( stepLineComponent != null )
		{
			stepLineComponent.ToggleAllSymbolVisuals( false );
		}
	}

	public override void Open()
	{
		base.Open();
		if( opening )
		{
			if( gaugeComponent != null )
			{
				gaugeComponent.ToggleVisibility( true );
			}
			if( stepLineComponent != null )
			{
				stepLineComponent.ToggleAllSymbolVisuals( true );
			}
		}
	}

	protected override void AdjustBoxSizeLimits()
	{
		float minimumWidth = 0f;
		float minimumHeight = 0f;

		marginQuad = 4f * margin;
		CreateGauge();
		CreateStepLine();
		minimumWidth = (stepLineArea.width > gaugeArea.width) ? stepLineArea.width : gaugeArea.width;
		minimumWidth += marginDouble;
		minimumHeight = gaugeArea.height + stepLineArea.height + marginQuad;
		if( maxBoxWidth < minimumWidth )
		{
			maxBoxWidth = minimumWidth;
		}
		if( maxBoxHeight < minimumHeight )
		{
			maxBoxHeight = minimumHeight;
		}
	}

	private void CreateGauge()
	{
		Vector2 gaugeSize = Vector2.zero;

		if( gaugeModel != null )
		{
			gaugeObject = Instantiate( gaugeModel, Vector3.zero, Quaternion.identity );
			gaugeComponent = gaugeObject.GetComponent<WorldSpaceGauge>();
			if( gaugeComponent == null )
			{
				Destroy( gaugeObject );
				gaugeObject = null;
			}
			else
			{
				gaugeComponent.Build( maxValue, 0f );
				gaugeComponent.SetCurrentValue( currentValue );
				gaugeSize = gaugeComponent.backgroundSize;
				gaugeArea.width = gaugeSize.x;
				gaugeArea.height = gaugeSize.y;
				gaugeSize = gaugeComponent.foregroundSize;
				gaugeArea.width = (gaugeArea.width > gaugeSize.x) ? gaugeArea.width : gaugeSize.x;
				gaugeArea.height = (gaugeArea.height > gaugeSize.y) ? gaugeArea.height : gaugeSize.y;
				gaugeObject.transform.SetParent( transform, false );
			}
		}
	}

	private void CreateStepLine()
	{
		int textID = GameTextDatabase.INVALID_TEXT_ID;
		GameTextDatabase gameTextDatabase = null;
		bool autoClose = false;
		Vector2 stepLineDimensions = Vector2.zero;

		gameTextDatabase = GameTextDatabase.Instance;
		if( (gameTextDatabase != null) && (symbolDatabase != null) && (stepLineObject == null) )
		{
			if( invest )
			{
				textID = GameTextDatabase.TEXT_ID_TRANSACTION_INVEST_UNIT;
			}
			else
			{
				textID = GameTextDatabase.TEXT_ID_TRANSACTION_DRAW_UNIT;
			}
			stepLine = gameTextDatabase.GetSystemText( textID, ref autoClose );
			stepLine = stepLine+"  "+currentValueStep;
			stepLineObject = new GameObject("StepLineObject");
			stepLineObject.transform.SetParent( transform, false );
			stepLineObject.transform.localPosition = Vector3.zero;
			stepLineComponent = stepLineObject.AddComponent<SpritedString>();
			stepLineComponent.SetSymbolSource( symbolDatabase );
			stepLineComponent.SetValue( stepLine );
			stepLineDimensions = stepLineComponent.GetWorldDimensions();
			stepLineArea.width = stepLineDimensions.x;
			stepLineArea.height = stepLineDimensions.y;
		}
	}

	protected override void DefineTextAreaSizeLimits()
	{
		maxTextWidth = maxBoxWidth - marginDouble;
		maxTextHeight = maxBoxHeight - marginQuad - gaugeArea.height - stepLineArea.height;
	}

	protected override void AdjustTextContent()
	{
		int totalDigitsMaxValue = 0;
		int totalDigitsCurrentValue = 0;
		int quocient = 0;
		int digitDifference = 0;

		quocient = maxValue;
		while( quocient > 0 )
		{
			quocient /= 10;
			totalDigitsMaxValue++;
		}
		totalDigitsMaxValue = (totalDigitsMaxValue < 1) ? 1 : totalDigitsMaxValue;
		quocient = currentValue;
		while( quocient > 0 )
		{
			quocient /= 10;
			totalDigitsCurrentValue++;
		}
		totalDigitsCurrentValue = (totalDigitsCurrentValue < 1) ? 1 : totalDigitsCurrentValue;
		digitDifference = totalDigitsMaxValue - totalDigitsCurrentValue;
		text = "";
		for( int i=0; i<digitDifference; i++ )
		{
			if( i == 0 )
			{
				text = "0";
			}
			else
			{
				text += "0";
			}
		}
		text += currentValue+"  /  "+maxValue;
	}

	protected override void DefineBoxArea()
	{
		float requiredWidth = 0f;

		if( boxObject == null )
		{
			/*halmeida - the textArea is limited by the box limits established earlier. These box limits
			ensure that the gauge and the stepLine fit inside the box, but there is no relation between
			the width of these two elements and the width of the final text. If the initial limits provided
			through the SetBoxLimits method are bigger than the width of the two elements, and the text
			demands that larger width, the text will be wider than them. On the other hand, if the initial
			limits provided through the SetBoxLimits method are narrower than the two elements, the text
			may have taken their width or may be slightly more narrow than them. At any rate, we cannot
			just take the textArea.width as the definitive width parameter.*/ 
			requiredWidth = textArea.width;
			if( gaugeArea.width > requiredWidth )
			{
				requiredWidth = gaugeArea.width;
			}
			if( stepLineArea.width > requiredWidth )
			{
				requiredWidth = stepLineArea.width;
			}
			boxArea.width = requiredWidth + marginDouble;
			boxArea.height = marginQuad + gaugeArea.height + stepLineArea.height + textArea.height;
			/*halmeida - the area is top-left anchored.*/
			boxArea.x = -boxArea.width / 2f;
			boxArea.y = boxArea.height / 2f;
		}
	}

	protected override void DefineTextAreaOffset()
	{
		textArea.x = -textArea.width / 2f;
		textArea.y = boxArea.y - margin - gaugeArea.height - marginDouble - stepLineArea.height;
	}

	protected override void PlaceOrnaments()
	{
		Vector3 objectPosition = Vector3.zero;

		/*halmeida - areas are top-left anchored.*/
		if( gaugeObject != null )
		{
			gaugeArea.x = -gaugeArea.width / 2f;
			gaugeArea.y = boxArea.y - margin;
			objectPosition.x = 0f;
			objectPosition.y = gaugeArea.y - gaugeArea.height / 2f;
			objectPosition.z = boxToTextDepth;
			gaugeObject.transform.localPosition = objectPosition;
		}
		if( stepLineObject != null )
		{
			stepLineArea.x = -stepLineArea.width / 2f;
			stepLineArea.y = boxArea.y - marginDouble - gaugeArea.height;
			objectPosition.x = 0f;
			objectPosition.y = stepLineArea.y - stepLineArea.height / 2f;
			objectPosition.z = boxToTextDepth;
			stepLineObject.transform.localPosition = objectPosition;
		}
	}

	public override bool ReactToPointerDown( float worldPointX, float worldPointY )
	{
		return open;
	}

	public override bool ReactToActionPress()
	{
		return open;
	}

	public override void Clear()
	{
		if( gaugeComponent != null )
		{
			gaugeComponent.Clear();
			gaugeComponent = null;
		}
		if( gaugeObject != null )
		{
			Destroy( gaugeObject );
			gaugeObject = null;
		}
		if( stepLineComponent != null )
		{
			stepLineComponent.Clear();
			stepLineComponent = null;
		}
		if( stepLineObject != null )
		{
			Destroy( stepLineObject );
			stepLineObject = null;
		}
		base.Clear();
	}
}
