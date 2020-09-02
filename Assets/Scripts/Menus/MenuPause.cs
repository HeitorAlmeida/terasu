using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPause : MenuStatsDisplay
{
	public int currencyItemID;
	public TextAlignment currencyAnchorAlignment;
	public Vector2 currencyPositionRates;
	public Vector2 currencyTextScales;
	public Vector2 currencyIconScales;

	protected ItemDatabase itemDatabase;
	protected GameObject currencyImageObject;
	protected RectTransform currencyImageTrans;
	protected Image currencyImage;
	protected GameObject currencyTextObject;
	protected RectTransform currencyTextTrans;
	protected SpritedStringUI currencyText;

	protected override void Awake()
	{
		base.Awake();
		itemDatabase = null;
		currencyImageObject = null;
		currencyImageTrans = null;
		currencyImage = null;
		currencyTextObject = null;
		currencyTextTrans = null;
		currencyText = null;
	}

	public void SetItemDatabase( ItemDatabase newItemDatabase )
	{
		itemDatabase = newItemDatabase;
	}

	protected override bool CreateStatsDisplay()
	{
		ItemData itemData = null;
		Sprite itemIcon = null;
		Vector2 iconPosition = Vector2.zero;

		if( base.CreateStatsDisplay() )
		{
			if( currencyTextObject == null )
			{
				CreateTextDisplayObject( "CurrencyValue", player.GetCurrentCoins().ToString(), currencyTextScales, currencyPositionRates,
					currencyAnchorAlignment, ref currencyTextObject, ref currencyTextTrans, ref currencyText );
				if( (itemDatabase != null) && (currencyImageObject == null) )
				{
					itemData = itemDatabase.GetItemData( currencyItemID );
					if( itemData != null )
					{
						itemIcon = itemData.itemIcon;
						if( itemIcon != null )
						{
							currencyImageObject = new GameObject("CurrencyIcon", typeof(RectTransform));
							currencyImageTrans = currencyImageObject.GetComponent<RectTransform>();
							currencyImage = currencyImageObject.AddComponent<Image>();
							currencyImage.sprite = itemIcon;
							currencyImage.SetNativeSize();
							currencyImageTrans.SetParent( interfaceCanvasTrans, false );
							currencyImageTrans.localScale = new Vector3( currencyIconScales.x, currencyIconScales.y, 1f );
							if( currencyTextTrans != null )
							{
								iconPosition.x = currencyTextTrans.anchoredPosition.x;
								iconPosition.x -= currencyText.GetUIDimensions().x * currencyTextScales.x / 2f;
								iconPosition.x -= currencyImageTrans.rect.width * currencyIconScales.x;
								iconPosition.y = currencyTextTrans.anchoredPosition.y;
								currencyImageTrans.anchoredPosition = iconPosition;
							}
						}
					}
				}
			}
			return true;
		}
		return false;
	}

	protected override bool UpdateStatsDisplay()
	{
		Vector2 iconPosition = Vector2.zero;

		if( base.UpdateStatsDisplay() )
		{
			if( currencyTextObject != null )
			{
				UpdateTextDisplayObject( player.GetCurrentCoins().ToString(), currencyTextScales, currencyPositionRates, currencyAnchorAlignment,
					currencyTextTrans, currencyText );
				if( currencyImageTrans != null )
				{
					iconPosition.x = currencyTextTrans.anchoredPosition.x;
					iconPosition.x -= currencyText.GetUIDimensions().x * currencyTextScales.x / 2f;
					iconPosition.x -= currencyImageTrans.rect.width * currencyIconScales.x;
					iconPosition.y = currencyTextTrans.anchoredPosition.y;
					currencyImageTrans.anchoredPosition = iconPosition;
				}
			}
			return true;
		}
		return false;
	}

	public override void DeselectOptionComponent()
	{
		base.DeselectOptionComponent();
		UpdateStatsDisplay();
	}

	protected override void ClearStatsDisplay()
	{
		if( currencyText != null )
		{
			currencyText.Clear();
			currencyText = null;
		}
		currencyTextTrans = null;
		if( currencyTextObject != null )
		{
			Destroy( currencyTextObject );
			currencyTextObject = null;
		}
		currencyImage = null;
		currencyImageTrans = null;
		if( currencyImageObject != null )
		{
			Destroy( currencyImageObject );
			currencyImageObject = null;
		}
		base.ClearStatsDisplay();
	}
}
