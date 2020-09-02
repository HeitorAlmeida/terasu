using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInventory : MenuStatsDisplay
{
	public DisplaySchemeMatrix sectionDisplayScheme;
	public PlayerAgent.EquipBodyPart[] firstSections;
	public DisplaySchemeMatrix itemDisplayScheme;
	public GameObject secondOverlayModel;
	public float secondOverlayAlpha;
	public GameObject thirdOverlayModel;
	public float thirdOverlayAlpha;
	public DisplaySchemeMatrix actionDisplayScheme;
	public ChoiceEffect[] itemActions;
	public Vector2 hypotheticTextOffset;
	public Color hypotheticColorIncrease;
	public Color hypotheticColorDecrease;
	public Color equippedTextColor;

	protected ItemDatabase itemDatabase;
	protected TextBoxUIManager textBoxUIManager;

	private GameObject sectionButtonModel;
	private GameObject itemButtonModel;
	private GameObject itemReturnButtonModel;
	private GameObject itemAdvanceButtonModel;
	private GameObject[] sectionButtonObjects;
	private RectTransform[] sectionButtonTransforms;
	private ButtonAnimator[] sectionButtons;
	private GameObject[] sectionTextObjects;
	private SpritedStringUI[] sectionTexts;
	private GameObject[] itemButtonObjects;
	private RectTransform[] itemButtonTransforms;
	private ButtonAnimator[] itemButtons;
	private GameObject[] itemTextObjects;
	private RectTransform[] itemTextTransforms;
	private SpritedStringUI[] itemTexts;
	private Vector2 itemTextOffsetRates;
	private GameObject[] itemAmountObjects;
	private RectTransform[] itemAmountTransforms;
	private SpritedStringUI[] itemAmountComponents;
	private Vector2 itemAmountOffsetRates;
	private GameObject itemReturnButtonObject;
	private RectTransform itemReturnButtonTransform;
	private ButtonAnimator itemReturnButton;
	private GameObject itemAdvanceButtonObject;
	private RectTransform itemAdvanceButtonTransform;
	private ButtonAnimator itemAdvanceButton;
	private int chosenSectionIndex;
	private int chosenItemIndex;
	private int checkingItemIndex;
	private int firstItemIndex;
	private int lastItemIndex;
	private List<ItemData> validItems;
	private List<int> validItemUnits;
	private GameObject secondOverlayObject;
	private Image secondOverlayImage;
	private GameObject thirdOverlayObject;
	private Image thirdOverlayImage;
	private GameObject[] hypotheticObjects;
	private SpritedStringUI[] hypotheticTexts;
	private GameObject actionButtonModel;
	private GameObject[] actionButtonObjects;
	private RectTransform[] actionButtonTransforms;
	private ButtonAnimator[] actionButtons;
	private GameObject[] actionTextObjects;
	private SpritedStringUI[] actionTexts;

	protected override void Awake()
	{
		base.Awake();
		itemDatabase = null;
		textBoxUIManager = null;
		sectionButtonModel = null;
		itemButtonModel = null;
		itemReturnButtonModel = null;
		itemAdvanceButtonModel = null;
		actionButtonModel = null;
		if( sectionDisplayScheme != null )
		{
			sectionButtonModel = CheckButtonValidity( sectionDisplayScheme.buttonElementModel );
		}
		Vector2[] itemContentOffsets = null;
		if( itemDisplayScheme != null )
		{
			itemButtonModel = CheckButtonValidity( itemDisplayScheme.buttonElementModel );
			itemReturnButtonModel = CheckButtonValidity( itemDisplayScheme.buttonReturnModel );
			itemAdvanceButtonModel = CheckButtonValidity( itemDisplayScheme.buttonAdvanceModel );
			itemContentOffsets = itemDisplayScheme.elementContentOffsetRates;
		}
		if( actionDisplayScheme != null )
		{
			actionButtonModel = CheckButtonValidity( actionDisplayScheme.buttonElementModel );
		}
		sectionButtonObjects = null;
		sectionButtonTransforms = null;
		sectionButtons = null;
		sectionTextObjects = null;
		sectionTexts = null;
		chosenSectionIndex = -1;
		itemButtonObjects = null;
		itemButtonTransforms = null;
		itemButtons = null;
		itemTextObjects = null;
		itemTextTransforms = null;
		itemTexts = null;
		itemTextOffsetRates = Vector2.zero;
		itemAmountObjects = null;
		itemAmountTransforms = null;
		itemAmountComponents = null;
		itemAmountOffsetRates = Vector2.zero;
		if( itemContentOffsets != null )
		{
			if( itemContentOffsets.Length > 0 )
			{
				itemTextOffsetRates = itemContentOffsets[0];
				if( itemContentOffsets.Length > 1 )
				{
					itemAmountOffsetRates = itemContentOffsets[1];
				}
			}
		}
		itemReturnButtonObject = null;
		itemReturnButtonTransform = null;
		itemReturnButton = null;
		itemAdvanceButtonObject = null;
		itemAdvanceButtonTransform = null;
		itemAdvanceButton = null;
		chosenItemIndex = -1;
		checkingItemIndex = -1;
		firstItemIndex = -1;
		lastItemIndex = -1;
		validItems = new List<ItemData>();
		validItemUnits = new List<int>();
		secondOverlayObject = null;
		secondOverlayImage = null;
		thirdOverlayObject = null;
		thirdOverlayImage = null;
		hypotheticObjects = null;
		hypotheticTexts = null;
		actionButtonObjects = null;
		actionButtonTransforms = null;
		actionButtons = null;
		actionTextObjects = null;
		actionTexts = null;
	}

	private GameObject CheckButtonValidity( GameObject testObject )
	{
		ButtonAnimator testAnimator = null;
		RectTransform testTransform = null;

		if( testObject != null )
		{
			testAnimator = testObject.GetComponent<ButtonAnimator>();
			testTransform = testObject.GetComponent<RectTransform>();
			if( (testAnimator != null) && (testTransform != null) )
			{
				return testObject;
			}
		}
		return null;
	}

	public void SetItemDatabase( ItemDatabase newItemDatabase )
	{
		itemDatabase = newItemDatabase;
	}

	protected override void BuildOptions()
	{
		base.BuildOptions();
		CreateSectionButtons();
		if( sectionButtons != null )
		{
			chosenSectionIndex = 0;
			sectionButtons[0].SetPressed( true );
			SeparateValidItems();
			CreateItemButtons();
		}
	}

	private void CreateSectionButtons()
	{
		bool toDiscard = false;
		GameObject buttonObject = null;
		RectTransform buttonTransform = null;
		ButtonAnimator buttonAnimator = null;
		GameObject buttonTextObject = null;
		RectTransform buttonTextTrans = null;
		SpritedStringUI buttonText = null;
		Vector2 buttonScale = Vector2.one;
		Vector2 buttonPositionRates = Vector2.zero;

		if( (sectionButtonModel != null) && (interfaceCanvasTrans != null) && (sectionButtons == null) )
		{
			GameTextDatabase gameTextDatabase = GameTextDatabase.Instance;
			int totalSections = 1;
			if( firstSections != null )
			{
				if( firstSections.Length > 0 )
				{
					totalSections += firstSections.Length;
				}
			}
			sectionButtonObjects = new GameObject[totalSections];
			sectionButtonTransforms = new RectTransform[totalSections];
			sectionButtons = new ButtonAnimator[totalSections];
			/*halmeida - the button objects and the text objects have to be separate objects so that they
			can have different scaling.*/
			sectionTextObjects = new GameObject[totalSections];
			sectionTexts = new SpritedStringUI[totalSections];
			buttonScale = sectionDisplayScheme.buttonElementScale;
			string sectionTitle = null;
			Vector2[] allPositionRates = sectionDisplayScheme.elementPositionRates;
			for( int i=0; i<totalSections; i++ )
			{
				if( gameTextDatabase != null )
				{
					if( i == (totalSections-1) )
					{
						sectionTitle = gameTextDatabase.GetSystemText( GameTextDatabase.TEXT_ID_NON_EQUIPPABLE_ITEMS, ref toDiscard );
					}
					else
					{
						sectionTitle = gameTextDatabase.GetEquipBodyPartName( firstSections[i] );
					}
				}
				if( allPositionRates != null )
				{
					buttonPositionRates = (allPositionRates.Length > i) ? allPositionRates[i] : new Vector2( 0.5f, 0.5f );
				}
				CreateButtonObject( sectionButtonModel, buttonScale, buttonPositionRates, TextAlignment.Center,
					ref buttonObject, ref buttonTransform, ref buttonAnimator );
				sectionButtonObjects[i] = buttonObject;
				sectionButtonTransforms[i] = buttonTransform;
				sectionButtons[i] = buttonAnimator;
				CreateTextDisplayObject( "SectionName", sectionTitle, Vector2.one, buttonPositionRates, TextAlignment.Center,
					ref buttonTextObject, ref buttonTextTrans, ref buttonText );
				sectionTextObjects[i] = buttonTextObject;
				sectionTexts[i] = buttonText;
			}
		}
	}

	private void SeparateValidItems()
	{
		if( (itemDatabase != null) && (player != null) && (sectionButtons != null) && (validItems != null) )
		{
			if( (chosenSectionIndex > -1) && (chosenSectionIndex < sectionButtons.Length) && (validItems.Count == 0) )
			{
				int[] itemIDs = player.GetInventoryItemIDsCopy();
				int[] itemUnits = player.GetInventoryItemUnitsCopy();
				ItemData itemData = null;
				EquipmentData equipmentData = null;
				if( itemIDs != null )
				{
					int itemID = ItemData.INVALID_ITEM_ID;
					for( int i=0; i<itemIDs.Length; i++ )
					{
						itemID = itemIDs[i];
						itemData = itemDatabase.GetItemData( itemID );
						if( itemData != null )
						{
							equipmentData = itemData.equipment;
							/*halmeida - I will evaluate itemData to see if it fits the current section of items.*/
							if( firstSections != null )
							{
								if( chosenSectionIndex < firstSections.Length )
								{
									if( equipmentData != null )
									{
										if( equipmentData.bodyPart == firstSections[chosenSectionIndex] )
										{
											validItems.Add( itemData );
											validItemUnits.Add( itemUnits[i] );
										}
									}
								}
								else
								{
									if( equipmentData == null )
									{
										validItems.Add( itemData );
										validItemUnits.Add( itemUnits[i] );
									}
								}
							}
							else
							{
								if( equipmentData == null )
								{
									validItems.Add( itemData );
									validItemUnits.Add( itemUnits[i] );
								}
							}
						}
					}
				}
			}
		}
	}

	private void CreateItemButtons()
	{
		chosenItemIndex = -1;
		checkingItemIndex = -1;
		if( (validItems != null) && (itemButtonModel != null) && (itemButtons == null) )
		{
			if( validItems.Count > 0 )
			{
				itemButtonObjects = new GameObject[validItems.Count];
				itemButtonTransforms = new RectTransform[validItems.Count];
				itemButtons = new ButtonAnimator[validItems.Count];
				itemTextObjects = new GameObject[validItems.Count];
				itemTextTransforms = new RectTransform[validItems.Count];
				itemTexts = new SpritedStringUI[validItems.Count];
				itemAmountObjects = new GameObject[validItems.Count];
				itemAmountTransforms = new RectTransform[validItems.Count];
				itemAmountComponents = new SpritedStringUI[validItems.Count];
				Vector2 buttonScale = itemDisplayScheme.buttonElementScale;
				Vector2[] allPositionRates = itemDisplayScheme.elementPositionRates;
				Vector2 buttonPositionRates = Vector2.zero;
				Vector2 textPositionRates = Vector2.zero;
				GameTextDatabase gameTextDatabase = GameTextDatabase.Instance;
				ItemData itemData = null;
				string itemName = null;
				bool itemHidden = false;
				GameObject itemButtonObject = null;
				RectTransform itemButtonTrans = null;
				ButtonAnimator itemButton = null;
				GameObject itemTextObject = null;
				RectTransform itemTextTrans = null;
				SpritedStringUI itemText = null;
				GameObject itemAmountObject = null;
				RectTransform itemAmountTransform = null;
				SpritedStringUI itemAmountComponent = null;
				bool needsScroll = false;
				for( int i=0; i<validItems.Count; i++ )
				{
					itemData = validItems[i];
					itemHidden = true;
					buttonPositionRates = new Vector2( 0.5f, 0.5f );
					gameTextDatabase.GetItemDescription( itemData.itemID, ref itemName );
					if( allPositionRates != null )
					{
						if( i < allPositionRates.Length )
						{
							itemHidden = false;
							buttonPositionRates = allPositionRates[i];
						}
						else
						{
							needsScroll = true;
						}
					}
					CreateButtonObject( itemButtonModel, buttonScale, buttonPositionRates, TextAlignment.Center, ref itemButtonObject,
						ref itemButtonTrans, ref itemButton );
					textPositionRates = buttonPositionRates + itemTextOffsetRates;
					CreateTextDisplayObject( "ItemName", itemName, Vector2.one, textPositionRates, TextAlignment.Left,
						ref itemTextObject, ref itemTextTrans, ref itemText );
					textPositionRates = buttonPositionRates + itemAmountOffsetRates;
					CreateTextDisplayObject( "ItemUnits", validItemUnits[i].ToString(), Vector2.one, textPositionRates,
						TextAlignment.Right, ref itemAmountObject, ref itemAmountTransform, ref itemAmountComponent );
					if( itemHidden )
					{
						lastItemIndex = i-1;
						itemButtonObject.SetActive( false );
						itemTextObject.SetActive( false );
						itemAmountObject.SetActive( false );
					}
					else
					{
						if( firstItemIndex == -1 )
						{
							firstItemIndex = i;
							lastItemIndex = i;
						}
					}
					itemButtonObjects[i] = itemButtonObject;
					itemButtonTransforms[i] = itemButtonTrans;
					itemButtons[i] = itemButton;
					itemTextObjects[i] = itemTextObject;
					itemTextTransforms[i] = itemTextTrans;
					itemTexts[i] = itemText;
					itemAmountObjects[i] = itemAmountObject;
					itemAmountTransforms[i] = itemAmountTransform;
					itemAmountComponents[i] = itemAmountComponent;
					if( (chosenItemIndex == -1) && (itemButton != null) )
					{
						if( player.IsEquippedWith( itemData.itemID ) )
						{
							chosenItemIndex = i;
							ChangeButtonState( true, itemButton, itemText, itemAmountComponent );
						}
					}
				}
				if( needsScroll )
				{
					buttonScale = itemDisplayScheme.buttonReturnScale;
					buttonPositionRates = itemDisplayScheme.returnPositionRates;
					CreateButtonObject( itemReturnButtonModel, buttonScale, buttonPositionRates, TextAlignment.Center,
						ref itemReturnButtonObject, ref itemReturnButtonTransform, ref itemReturnButton );
					buttonScale = itemDisplayScheme.buttonAdvanceScale;
					buttonPositionRates = itemDisplayScheme.advancePositionRates;
					CreateButtonObject( itemAdvanceButtonModel, buttonScale, buttonPositionRates, TextAlignment.Center,
						ref itemAdvanceButtonObject, ref itemAdvanceButtonTransform, ref itemAdvanceButton );
					if( itemReturnButton != null )
					{
						itemReturnButton.SetPressed( true );
					} 
				}
			}
			else
			{
				Debug.Log("Debug : MenuInventory : no items to show in section "+chosenSectionIndex+".");
			}
		}
	}

	protected void CreateButtonObject( GameObject buttonModel, Vector2 scaleChange, Vector2 positionRates, TextAlignment anchorAlign,
		ref GameObject newButtonObject, ref RectTransform newButtonTransform, ref ButtonAnimator newButtonAnimator )
	{
		if( buttonModel != null )
		{
			newButtonObject = Instantiate( buttonModel, interfaceCanvasTrans ) as GameObject;
			newButtonAnimator = newButtonObject.GetComponent<ButtonAnimator>();
			newButtonTransform = newButtonObject.GetComponent<RectTransform>();
			Vector2 originalUIDimensions = new Vector2( newButtonTransform.rect.width, newButtonTransform.rect.height );
			PlaceTransformProperly( newButtonTransform, originalUIDimensions, scaleChange, positionRates, anchorAlign );
		}
	}

	private void ChangeButtonState( bool pressedState, ButtonAnimator buttonAnimator, SpritedStringUI buttonName,
		SpritedStringUI buttonAmount )
	{
		if( buttonAnimator != null )
		{
			buttonAnimator.SetPressed( pressedState );
		}
		Color textColor = pressedState ? equippedTextColor : Color.white;
		if( buttonName != null )
		{
			buttonName.SetColor( textColor, Vector4.zero );
		}
		if( buttonAmount != null )
		{
			buttonAmount.SetColor( textColor, Vector4.zero );
		}
	}

	protected override void ProgressOptionComponents( float timeStep )
	{
		ButtonAnimator optionButton = null;

		if( sectionButtons != null )
		{
			for( int i=0; i<sectionButtons.Length; i++ )
			{
				optionButton = sectionButtons[i];
				if( optionButton != null )
				{
					optionButton.Progress( timeStep );
				}
			}
		}
		if( itemButtons != null )
		{
			for( int i=firstItemIndex; i<=lastItemIndex; i++ )
			{
				optionButton = itemButtons[i];
				if( optionButton != null )
				{
					optionButton.Progress( timeStep );
				}
			}
		}
		if( itemReturnButton != null )
		{
			itemReturnButton.Progress( timeStep );
		}
		if( itemAdvanceButton != null )
		{
			itemAdvanceButton.Progress( timeStep );
		}
		if( actionButtons != null )
		{
			for( int i=0; i<actionButtons.Length; i++ )
			{
				optionButton = actionButtons[i];
				if( optionButton != null )
				{
					optionButton.Progress( timeStep );
				}
			}
		}
		base.ProgressOptionComponents( timeStep );
	}

	protected override void ClearOptions()
	{
		if( textBoxUIManager != null )
		{
			textBoxUIManager.InterceptActionPress();
			textBoxUIManager = null;
		}
		ClearActionButtons();
		ClearHypotheticalStatsDisplay();
		ClearAdditionalOverlays();
		ClearItemButtons();
		ClearValidItems();
		ClearSectionButtons();
		base.ClearOptions();
	}

	private void ClearSectionButtons()
	{
		SpritedStringUI buttonText = null;
		ButtonAnimator buttonAnimator = null;
		GameObject buttonObject = null;

		if( sectionTexts != null )
		{
			for( int i=0; i<sectionTexts.Length; i++ )
			{
				buttonText = sectionTexts[i];
				if( buttonText != null )
				{
					buttonText.Clear();
					sectionTexts[i] = null;
				}
			}
			sectionTexts = null;
		}
		if( sectionTextObjects != null )
		{
			for( int i=0; i<sectionTextObjects.Length; i++ )
			{
				buttonObject = sectionTextObjects[i];
				if( buttonObject != null )
				{
					Destroy( buttonObject );
					sectionTextObjects[i] = null;
				}
			}
			sectionTextObjects = null;
		}
		if( sectionButtons != null )
		{
			for( int i=0; i<sectionButtons.Length; i++ )
			{
				buttonAnimator = sectionButtons[i];
				if( buttonAnimator != null )
				{
					buttonAnimator.Clear();
					sectionButtons[i] = null;
				}
			}
			sectionButtons = null;
		}
		sectionButtonTransforms = null;
		if( sectionButtonObjects != null )
		{
			for( int i=0; i<sectionButtonObjects.Length; i++ )
			{
				buttonObject = sectionButtonObjects[i];
				if( buttonObject != null )
				{
					Destroy( buttonObject );
					sectionButtonObjects[i] = null;
				}
			}
			sectionButtonObjects = null;
		}
		chosenSectionIndex = -1;
	}

	private void ClearItemButtons()
	{
		SpritedStringUI buttonText = null;
		ButtonAnimator buttonAnimator = null;
		GameObject buttonObject = null;

		if( itemReturnButton != null )
		{
			itemReturnButton.Clear();
			itemReturnButton = null;
		}
		itemReturnButtonTransform = null;
		if( itemReturnButtonObject != null )
		{
			Destroy( itemReturnButtonObject );
			itemReturnButtonObject = null;
		}
		if( itemAdvanceButton != null )
		{
			itemAdvanceButton.Clear();
			itemAdvanceButton = null;
		}
		itemAdvanceButtonTransform = null;
		if( itemAdvanceButtonObject != null )
		{
			Destroy( itemAdvanceButtonObject );
			itemAdvanceButtonObject = null;
		}
		if( itemAmountComponents != null )
		{
			for( int i=0; i<itemAmountComponents.Length; i++ )
			{
				buttonText = itemAmountComponents[i];
				if( buttonText != null )
				{
					buttonText.Clear();
					itemAmountComponents[i] = null;
				}
			}
			itemAmountComponents = null;
		}
		itemAmountTransforms = null;
		if( itemAmountObjects != null )
		{
			for( int i=0; i<itemAmountObjects.Length; i++ )
			{
				buttonObject = itemAmountObjects[i];
				if( buttonObject != null )
				{
					Destroy( buttonObject );
					itemAmountObjects[i] = null;
				}
			}
			itemAmountObjects = null;
		}
		if( itemTexts != null )
		{
			for( int i=0; i<itemTexts.Length; i++ )
			{
				buttonText = itemTexts[i];
				if( buttonText != null )
				{
					buttonText.Clear();
					itemTexts[i] = null;
				}
			}
			itemTexts = null;
		}
		itemTextTransforms = null;
		if( itemTextObjects != null )
		{
			for( int i=0; i<itemTextObjects.Length; i++ )
			{
				buttonObject = itemTextObjects[i];
				if( buttonObject != null )
				{
					Destroy( buttonObject );
					itemTextObjects[i] = null;
				}
			}
			itemTextObjects = null;
		}
		if( itemButtons != null )
		{
			for( int i=0; i<itemButtons.Length; i++ )
			{
				buttonAnimator = itemButtons[i];
				if( buttonAnimator != null )
				{
					buttonAnimator.Clear();
					itemButtons[i] = null;
				}
			}
			itemButtons = null;
		}
		itemButtonTransforms = null;
		if( itemButtonObjects != null )
		{
			for( int i=0; i<itemButtonObjects.Length; i++ )
			{
				buttonObject = itemButtonObjects[i];
				if( buttonObject != null )
				{
					Destroy( buttonObject );
					itemButtonObjects[i] = null;
				}
			}
			itemButtonObjects = null;
		}
		chosenItemIndex = -1;
		checkingItemIndex = -1;
		firstItemIndex = -1;
		lastItemIndex = -1;
	}

	private void ClearValidItems()
	{
		if( validItems != null )
		{
			validItems.Clear();
			validItemUnits.Clear();
		}
	}

	public override void Clear()
	{
		itemDatabase = null;
		sectionButtonModel = null;
		itemButtonModel = null;
		itemReturnButtonModel = null;
		itemAdvanceButtonModel = null;
		validItems = null;
		validItemUnits = null;
		base.Clear();
	}

	protected override bool CheckOptionsHit( Vector2 canvasPoint )
	{
		if( !base.CheckOptionsHit( canvasPoint ) )
		{
			if( (sectionButtonTransforms != null) && (actionButtonTransforms == null) )
			{
				for( int i=0; i<sectionButtonTransforms.Length; i++ )
				{
					if( CheckTransformPointCollision( sectionButtonTransforms[i], canvasPoint ) )
					{
						if( (chosenSectionIndex > -1) && (chosenSectionIndex != i) )
						{
							sectionButtons[chosenSectionIndex].SetPressed( false );
							ClearItemButtons();
							ClearValidItems();
							chosenSectionIndex = i;
							sectionButtons[i].SetPressed( true );
							SeparateValidItems();
							CreateItemButtons();
						}
						return true;
					}
				}
			}
			if( (itemButtonTransforms != null) && (actionButtonTransforms == null) )
			{
				for( int i=firstItemIndex; i<=lastItemIndex; i++ )
				{
					if( CheckTransformPointCollision( itemButtonTransforms[i], canvasPoint ) )
					{
						if( i != chosenItemIndex )
						{
							ChangeButtonState( true, itemButtons[i], itemTexts[i], itemAmountComponents[i] );
						}
						checkingItemIndex = i;
						CreateAdditionalOverlays();
						CreateHypotheticalStatsDisplay();
						CreateActionButtons();
						if( itemDatabase.DisplayItemInfo( ItemDatabase.ItemDisplayContext.Inventory, validItems[i] ) )
						{
							textBoxUIManager = TextBoxUIManager.Instance;
						}
						return true;
					}
				}
				if( CheckTransformPointCollision( itemAdvanceButtonTransform, canvasPoint ) )
				{
					if( (itemButtons.Length-1) > lastItemIndex )
					{
						firstItemIndex += itemDisplayScheme.elementsScrolled;
						lastItemIndex += itemDisplayScheme.elementsScrolled;
						if( lastItemIndex >= itemButtons.Length )
						{
							lastItemIndex = itemButtons.Length-1;
						}
						RepositionItemButtons();
						itemAdvanceButton.SetPressed( !((itemButtons.Length-1) > lastItemIndex) );
						itemReturnButton.SetPressed( !(firstItemIndex > 0) );
					}
					return true;
				}
				if( CheckTransformPointCollision( itemReturnButtonTransform, canvasPoint ) )
				{
					if( firstItemIndex > 0 )
					{
						firstItemIndex -= itemDisplayScheme.elementsScrolled;
						Vector2[] allElementPositions = itemDisplayScheme.elementPositionRates;
						lastItemIndex = allElementPositions.Length-1;
						RepositionItemButtons();
						itemReturnButton.SetPressed( !(firstItemIndex > 0) );
						itemAdvanceButton.SetPressed( !((itemButtons.Length-1) > lastItemIndex) );
					}
					return true;
				}
			}
			if( actionButtonTransforms != null )
			{
				for( int i=0; i<actionButtonTransforms.Length; i++ )
				{
					if( CheckTransformPointCollision( actionButtonTransforms[i], canvasPoint ) )
					{
						if( !actionButtons[i].IsPressed() )
						{
							switch( itemActions[i] )
							{
								case ChoiceEffect.Equip:
									player.EquipItem( validItems[checkingItemIndex] );
									UpdateStatsDisplay();
									if( chosenItemIndex > -1 )
									{
										ChangeButtonState( false, itemButtons[chosenItemIndex], itemTexts[chosenItemIndex],
											itemAmountComponents[chosenItemIndex] );
									}
									chosenItemIndex = checkingItemIndex;
									break;
								case ChoiceEffect.Unequip:
									player.UnequipItem( validItems[checkingItemIndex].itemID );
									UpdateStatsDisplay();
									ChangeButtonState( false, itemButtons[checkingItemIndex], itemTexts[checkingItemIndex],
										itemAmountComponents[checkingItemIndex] );
									checkingItemIndex = -1;
									chosenItemIndex = -1;
									break;
								case ChoiceEffect.Use:
									break;
								case ChoiceEffect.CloseMenu:
									if( checkingItemIndex != chosenItemIndex )
									{
										ChangeButtonState( false, itemButtons[checkingItemIndex], itemTexts[checkingItemIndex],
											itemAmountComponents[checkingItemIndex] );
									}
									break;
							}
							ClearActionButtons();
							ClearHypotheticalStatsDisplay();
							ClearAdditionalOverlays();
							if( textBoxUIManager != null )
							{
								textBoxUIManager.InterceptActionPress();
								textBoxUIManager = null;
							}
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool CheckTransformPointCollision( RectTransform rectTrans, Vector2 canvasPoint )
	{
		if( rectTrans != null )
		{
			float realWidth = rectTrans.rect.width * rectTrans.localScale.x;
			float realHeight = rectTrans.rect.height * rectTrans.localScale.y;
			float xMin = rectTrans.anchoredPosition.x - realWidth/2f;
			float yMax = rectTrans.anchoredPosition.y + realHeight/2f;
			return UsefulFunctions.AreaContainsPoint( xMin, yMax, realWidth, realHeight, canvasPoint.x, canvasPoint.y );
		}
		return false;
	}

	private void CreateAdditionalOverlays()
	{
		bool toDiscard = false;

		if( (secondOverlayObject == null) && (thirdOverlayObject == null) )
		{
			CreateBackground( true, secondOverlayModel, ref secondOverlayObject, ref secondOverlayImage, ref toDiscard, ref toDiscard );
			if( secondOverlayImage != null )
			{
				Color overlayColor = secondOverlayImage.color;
				overlayColor.a = secondOverlayAlpha;
				secondOverlayImage.color = overlayColor;
			}
			CreateBackground( true, thirdOverlayModel, ref thirdOverlayObject, ref thirdOverlayImage, ref toDiscard, ref toDiscard );
			if( thirdOverlayImage != null )
			{
				Color overlayColor = thirdOverlayImage.color;
				overlayColor.a = thirdOverlayAlpha;
				thirdOverlayImage.color = overlayColor;
			}
		}
	}

	private void ClearAdditionalOverlays()
	{
		thirdOverlayImage = null;
		if( thirdOverlayObject != null )
		{
			Destroy( thirdOverlayObject );
			thirdOverlayObject = null;
		}
		secondOverlayImage = null;
		if( secondOverlayObject != null )
		{
			Destroy( secondOverlayObject );
			secondOverlayObject = null;
		}
	}

	private void CreateHypotheticalStatsDisplay()
	{
		if( (validItems != null) && statsDisplayValid && (player != null) )
		{
			if( (checkingItemIndex > -1) && (checkingItemIndex < validItems.Count) )
			{
				PlayerAgent.EquipAttribute[] attributes = statsDisplay.attributes;
				if( attributes != null )
				{
					int totalAttributes = attributes.Length;
					float[] attributeValues = new float[totalAttributes];
					player.GetHypotheticalEquipResults( validItems[checkingItemIndex], attributes, ref attributeValues );
					hypotheticObjects = new GameObject[totalAttributes];
					hypotheticTexts = new SpritedStringUI[totalAttributes];
					TextAlignment textAnchor = statsDisplay.attributeAnchorAlignment;
					Vector2 textScale = statsDisplay.attributeScales;
					int textPercentage = 0;
					int textPercentageLimited = 0;
					Vector2[] allPositionRates = statsDisplay.attributeEquipPosRates;
					Vector2 textPositionRates = Vector2.zero;
					GameObject newTextObject = null;
					RectTransform newTextTransform = null;
					SpritedStringUI newText = null;
					int[] oneAttributeValues = null;
					for( int i=0; i<totalAttributes; i++ )
					{
						textPercentage = UsefulFunctions.GetPercentage( attributeValues[i] );
						textPercentageLimited = (textPercentage > percentageLimit) ? percentageLimit : textPercentage;
						textPositionRates = allPositionRates[i] + hypotheticTextOffset;
						CreateTextDisplayObject( "HypotheticStat", textPercentageLimited.ToString(), textScale, textPositionRates, textAnchor,
							ref newTextObject, ref newTextTransform, ref newText );
						hypotheticObjects[i] = newTextObject;
						hypotheticTexts[i] = newText;
						if( (statValues != null) && (aspects != null) && (newText != null) )
						{
							oneAttributeValues = statValues[i];
							for( int j=0; j<aspects.Length; j++ )
							{
								if( aspects[j] == AttributeAspect.Equip )
								{
									if( textPercentage > oneAttributeValues[j] )
									{
										newText.SetColor( hypotheticColorIncrease, Vector4.zero );
									}
									else if( textPercentage < oneAttributeValues[j] )
									{
										newText.SetColor( hypotheticColorDecrease, Vector4.zero );
									}
									break;
								}
							}
						}
					}
					attributes = null;
					attributeValues = null;
				}
			}
		}
	}

	private void ClearHypotheticalStatsDisplay()
	{
		SpritedStringUI hypotheticText = null;
		if( hypotheticTexts != null )
		{
			for( int i=0; i<hypotheticTexts.Length; i++ )
			{
				hypotheticText = hypotheticTexts[i];
				if( hypotheticText != null )
				{
					hypotheticText.Clear();
					hypotheticTexts[i] = null;
				}
			}
		}
		GameObject hypotheticObject = null;
		if( hypotheticObjects != null )
		{
			for( int i=0; i<hypotheticObjects.Length; i++ )
			{
				hypotheticObject = hypotheticObjects[i];
				if( hypotheticObject != null )
				{
					Destroy( hypotheticObject );
					hypotheticObjects[i] = null;
				}
			}
		}
	}

	private void CreateActionButtons()
	{
		if( (validItems != null) && (actionButtonModel != null) && (interfaceCanvasTrans != null) && (actionButtons == null) )
		{
			GameTextDatabase gameTextDatabase = GameTextDatabase.Instance;
			int totalActions = 0;
			if( itemActions != null )
			{
				totalActions = itemActions.Length;
			}
			if( (totalActions > 0) && (checkingItemIndex > -1) && (checkingItemIndex < validItems.Count) )
			{
				ItemData itemData = validItems[checkingItemIndex];
				bool isEquippable = (itemData.equipment != null);
				bool isEquipped = player.IsEquippedWith( itemData.itemID );
				bool isExpendable = itemData.expendable;
				actionButtonObjects = new GameObject[totalActions];
				actionButtonTransforms = new RectTransform[totalActions];
				actionButtons = new ButtonAnimator[totalActions];
				/*halmeida - the button objects and the text objects have to be separate objects so that they
				can have different scaling.*/
				actionTextObjects = new GameObject[totalActions];
				actionTexts = new SpritedStringUI[totalActions];
				Vector2 buttonScale = actionDisplayScheme.buttonElementScale;
				Vector2 buttonPositionRates = Vector2.zero;
				string actionName = null;
				Vector2[] allPositionRates = actionDisplayScheme.elementPositionRates;
				GameObject buttonObject = null;
				RectTransform buttonTransform = null;
				ButtonAnimator buttonAnimator = null;
				GameObject buttonTextObject = null;
				RectTransform buttonTextTrans = null;
				SpritedStringUI buttonText = null;
				for( int i=0; i<totalActions; i++ )
				{
					if( gameTextDatabase != null )
					{
						actionName = gameTextDatabase.GetMenuOptionText( itemActions[i] );
					}
					if( allPositionRates != null )
					{
						buttonPositionRates = (allPositionRates.Length > i) ? allPositionRates[i] : new Vector2( 0.5f, 0.5f );
					}
					CreateButtonObject( actionButtonModel, buttonScale, buttonPositionRates, TextAlignment.Center,
						ref buttonObject, ref buttonTransform, ref buttonAnimator );
					actionButtonObjects[i] = buttonObject;
					actionButtonTransforms[i] = buttonTransform;
					actionButtons[i] = buttonAnimator;
					CreateTextDisplayObject( "ActionName", actionName, Vector2.one, buttonPositionRates, TextAlignment.Center,
						ref buttonTextObject, ref buttonTextTrans, ref buttonText );
					actionTextObjects[i] = buttonTextObject;
					actionTexts[i] = buttonText;
					switch( itemActions[i] )
					{
						case ChoiceEffect.Equip:
							if( !isEquippable || isEquipped )
							{
								ChangeButtonState( true, buttonAnimator, buttonText, null );
							}
							break;
						case ChoiceEffect.Unequip:
							if( !isEquipped )
							{
								ChangeButtonState( true, buttonAnimator, buttonText, null );
							}
							break;
						case ChoiceEffect.Use:
							if( !isExpendable )
							{
								ChangeButtonState( true, buttonAnimator, buttonText, null );
							}
							break;
					}
				}
			}
		}
	}

	private void ClearActionButtons()
	{
		SpritedStringUI buttonText = null;
		ButtonAnimator buttonAnimator = null;
		GameObject buttonObject = null;

		if( actionTexts != null )
		{
			for( int i=0; i<actionTexts.Length; i++ )
			{
				buttonText = actionTexts[i];
				if( buttonText != null )
				{
					buttonText.Clear();
					actionTexts[i] = null;
				}
			}
			actionTexts = null;
		}
		if( actionTextObjects != null )
		{
			for( int i=0; i<actionTextObjects.Length; i++ )
			{
				buttonObject = actionTextObjects[i];
				if( buttonObject != null )
				{
					Destroy( buttonObject );
					actionTextObjects[i] = null;
				}
			}
			actionTextObjects = null;
		}
		if( actionButtons != null )
		{
			for( int i=0; i<actionButtons.Length; i++ )
			{
				buttonAnimator = actionButtons[i];
				if( buttonAnimator != null )
				{
					buttonAnimator.Clear();
					actionButtons[i] = null;
				}
			}
			actionButtons = null;
		}
		actionButtonTransforms = null;
		if( actionButtonObjects != null )
		{
			for( int i=0; i<actionButtonObjects.Length; i++ )
			{
				buttonObject = actionButtonObjects[i];
				if( buttonObject != null )
				{
					Destroy( buttonObject );
					actionButtonObjects[i] = null;
				}
			}
			actionButtonObjects = null;
		}
	}

	private void RepositionItemButtons()
	{
		Vector2 buttonScale = itemDisplayScheme.buttonElementScale;
		Vector2 buttonPositionRates = Vector2.zero;
		Vector2[] allElementPositions = itemDisplayScheme.elementPositionRates;
		Vector2 originalUIDimensions = Vector2.one;
		RectTransform elementTransform = null;
		Vector2 elementPositionRates = Vector2.zero;

		for( int i=0; i<itemButtons.Length; i++ )
		{
			if( (i >= firstItemIndex) && (i <= lastItemIndex) )
			{
				/*halmeida - button has a place among the buttons being shown.*/
				buttonPositionRates = allElementPositions[i-firstItemIndex];
				elementTransform = itemButtonTransforms[i];
				originalUIDimensions = new Vector2( elementTransform.rect.width, elementTransform.rect.height );
				PlaceTransformProperly( elementTransform, originalUIDimensions, buttonScale, buttonPositionRates, TextAlignment.Center );
				elementTransform = itemTextTransforms[i];
				elementPositionRates = buttonPositionRates + itemTextOffsetRates;
				originalUIDimensions = itemTexts[i].GetUIDimensions();
				PlaceTransformProperly( elementTransform, originalUIDimensions, Vector2.one, elementPositionRates, TextAlignment.Left );
				elementTransform = itemAmountTransforms[i];
				elementPositionRates = buttonPositionRates + itemAmountOffsetRates;
				originalUIDimensions = itemAmountComponents[i].GetUIDimensions();
				PlaceTransformProperly( elementTransform, originalUIDimensions, Vector2.one, elementPositionRates, TextAlignment.Right );
				itemTextObjects[i].SetActive( true );
				itemAmountObjects[i].SetActive( true );
				itemButtonObjects[i].SetActive( true );
			}
			else
			{
				/*halmeida - button should not be shown.*/
				itemTextObjects[i].SetActive( false );
				itemAmountObjects[i].SetActive( false );
				itemButtonObjects[i].SetActive( false );
			}
		}
	}
}
