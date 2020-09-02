using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGauge : MonoBehaviour
{
	public Color backgroundColor;
	public Vector2 backgroundSizeRates;
	public Color foregroundColor;
	public Vector2 foregroundSizeRates;
	public Vector2 positionRates;
	public bool reverseSink;

	private RectTransform gaugeTrans;
	private GameObject bgObject;
	private RectTransform bgTrans;
	private Image bgImage;
	private GameObject fgObject;
	private RectTransform fgTrans;
	private Image fgImage;
	private float fgMaxWidth;
	private float fgMaxHeight;
	private RectTransform interfaceCanvasTrans;
	private Rect interfaceCanvasRect;
	private float maxValue;
	private float minValue;
	private float currentValue;
	private bool horizontal;
	private Vector2 gaugeAnchoredPos;
	private bool built;

	void Awake()
	{
		gaugeTrans = gameObject.GetComponent<RectTransform>();
		bgObject = null;
		bgTrans = null;
		bgImage = null;
		fgObject = null;
		fgTrans = null;
		fgImage = null;
		fgMaxWidth = 0f;
		fgMaxHeight = 0f;
		interfaceCanvasTrans = null;
		interfaceCanvasRect = new Rect( 0f, 0f, 0f, 0f );
		maxValue = 0f;
		minValue = 0f;
		currentValue = 0f;
		horizontal = false;
		gaugeAnchoredPos = Vector2.zero;
		built = false;
	}

	public void Build( RectTransform newInterfaceCanvasTrans, float maxDisplayableValue, float minDisplayableValue )
	{
		if( (gaugeTrans != null) && (newInterfaceCanvasTrans != null) && (maxDisplayableValue > minDisplayableValue) &&
			(bgObject == null) && (fgObject == null) )
		{
			interfaceCanvasTrans = newInterfaceCanvasTrans;
			gaugeTrans.SetParent( interfaceCanvasTrans, false );
			interfaceCanvasRect = interfaceCanvasTrans.rect;
			gaugeAnchoredPos = positionRates;
			/*halmeida - to obtain the position within the canvas where the gauge will be, first we
			ensure we are dealing with screen rates.*/
			gaugeAnchoredPos.x *= ( gaugeAnchoredPos.x < 0f ? -1f : 1f );
			gaugeAnchoredPos.x = ( gaugeAnchoredPos.x > 1f ? 1f : gaugeAnchoredPos.x );
			gaugeAnchoredPos.y *= ( gaugeAnchoredPos.y < 0f ? -1f : 1f );
			gaugeAnchoredPos.y = ( gaugeAnchoredPos.y > 1f ? 1f : gaugeAnchoredPos.y );
			/*halmeida - then we turn screen rates into canvas coordinates. To clarify: with screen rates
			(0, 0) we are at the bottom left of the screen, while with canvas coordinates (0, 0) we are
			at the exact middle of the screen.*/
			if( gaugeAnchoredPos.x >= 0.5f )
			{
				gaugeAnchoredPos.x = 2f * (gaugeAnchoredPos.x - 0.5f);
				gaugeAnchoredPos.x = (interfaceCanvasRect.width / 2f) * gaugeAnchoredPos.x;
			}
			else
			{
				gaugeAnchoredPos.x = 2f * (0.5f - gaugeAnchoredPos.x);
				gaugeAnchoredPos.x = (interfaceCanvasRect.width / 2f) * gaugeAnchoredPos.x * -1;
			}
			if( gaugeAnchoredPos.y >= 0.5f )
			{
				gaugeAnchoredPos.y = 2f * (gaugeAnchoredPos.y - 0.5f);
				gaugeAnchoredPos.y = (interfaceCanvasRect.height / 2f) * gaugeAnchoredPos.y;
			}
			else
			{
				gaugeAnchoredPos.y = 2f * (0.5f - gaugeAnchoredPos.y);
				gaugeAnchoredPos.y = (interfaceCanvasRect.height / 2f) * gaugeAnchoredPos.y * -1;
			}
			gaugeTrans.anchoredPosition = gaugeAnchoredPos;
			CreateBackground();
			CreateForeground();
			maxValue = maxDisplayableValue;
			minValue = minDisplayableValue;
			currentValue = maxValue;
			built = true;
		}
	}

	private void CreateBackground()
	{
		bgObject = new GameObject("UIGaugeBackground", typeof(RectTransform));
		bgTrans = bgObject.GetComponent<RectTransform>();
		bgTrans.SetParent( gaugeTrans, false );
		bgTrans.anchoredPosition = Vector2.zero;
		bgTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, backgroundSizeRates.x * interfaceCanvasRect.width );
		bgTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, backgroundSizeRates.y * interfaceCanvasRect.height );
		bgImage = bgObject.AddComponent<Image>();
		bgImage.color = backgroundColor;
	}

	private void CreateForeground()
	{
		fgObject = new GameObject("UIGaugeForeground", typeof(RectTransform));
		fgTrans = fgObject.GetComponent<RectTransform>();
		fgTrans.SetParent( gaugeTrans, false );
		fgTrans.anchoredPosition = Vector2.zero;
		fgMaxWidth = foregroundSizeRates.x * interfaceCanvasRect.width;
		fgMaxHeight = foregroundSizeRates.y * interfaceCanvasRect.height;
		fgTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, fgMaxWidth );
		fgTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, fgMaxHeight );
		fgImage = fgObject.AddComponent<Image>();
		fgImage.color = foregroundColor;
		horizontal = (fgMaxWidth >= fgMaxHeight);
	}

	public float GetCurrentValue()
	{
		return currentValue;
	}

	public void SetCurrentValue( float newValue )
	{
		currentValue = newValue;
		if( currentValue > maxValue )
		{
			currentValue = maxValue;
		}
		if( currentValue < minValue )
		{
			currentValue = minValue;
		}
		UpdateGaugeForeground();
	}

	private void UpdateGaugeForeground()
	{
		float currentRate = 0f;
		float currentDimension = 0f;
		float offsetToSink = 0f;

		if( fgTrans != null )
		{
			currentRate = currentValue / maxValue;
			if( horizontal )
			{
				currentDimension = currentRate * fgMaxWidth;
				fgTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, currentDimension );
				offsetToSink = (fgMaxWidth - currentDimension) / 2f;
				if( reverseSink )
				{
					offsetToSink *= -1;
				}
				fgTrans.anchoredPosition = new Vector2( -offsetToSink, 0f );
			}
			else
			{
				currentDimension = currentRate * fgMaxHeight;
				fgTrans.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, currentDimension );
				offsetToSink = (fgMaxHeight - currentDimension) / 2f;
				if( reverseSink )
				{
					offsetToSink *= -1;
				}
				fgTrans.anchoredPosition = new Vector2( 0f, -offsetToSink );
			}
		}
	}

	public void ToggleVisibility( bool enable )
	{
		if( bgImage != null )
		{
			bgImage.enabled = enable;
		}
		if( fgImage != null )
		{
			fgImage.enabled = enable;
		}
	}

	public void Clear()
	{
		fgImage = null;
		fgTrans = null;
		if( fgObject != null )
		{
			Destroy( fgObject );
			fgObject = null;
		}
		bgImage = null;
		bgTrans = null;
		if( bgObject != null )
		{
			Destroy( bgObject );
			bgObject = null;
		}
		interfaceCanvasTrans = null;
		gaugeTrans = null;
	}

	public bool WasBuilt()
	{
		return built;
	}
}
