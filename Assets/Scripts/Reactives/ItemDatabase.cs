using UnityEngine;
using System.Collections;

public class ItemDatabase : MonoBehaviour
{
	public enum ItemDisplayContext { Field, Inventory }

	public GameObject[] itemModels;
	public ItemDisplayScheme[] itemDisplaySchemes;
	public ItemDisplayContext[] itemDisplayContexts;
	public int textIDInventoryTutorial;

	private int[] itemIDs;
	private ItemData[] itemDatas;
	private GameTextDatabase.TextPlatform currentPlatform;

	void Awake()
	{
		GameObject itemModel = null;
		ItemController itemController = null;
		ItemData itemData = null;
		bool idExtracted = false;

		itemIDs = null;
		itemDatas = null;
		if( itemModels != null )
		{
			for( int i=0; i<itemModels.Length; i++ )
			{
				idExtracted = false;
				itemModel = itemModels[i];
				if( itemModel != null )
				{
					itemController = itemModel.GetComponent<ItemController>();
					if( itemController != null )
					{
						itemData = itemController.itemData;
						if( itemData != null )
						{
							UsefulFunctions.IncreaseArray<int>( ref itemIDs, itemData.itemID );
							UsefulFunctions.IncreaseArray<ItemData>( ref itemDatas, itemData );
							idExtracted = true;
						}
					}
				}
				if( !idExtracted )
				{
					UsefulFunctions.IncreaseArray<int>( ref itemIDs, ItemData.INVALID_ITEM_ID );
					UsefulFunctions.IncreaseArray<ItemData>( ref itemDatas, null );
				}
			}
		}
		#if !UNITY_ANDROID || UNITY_EDITOR
			currentPlatform = GameTextDatabase.TextPlatform.PC;
		#else
			currentPlatform = GameTextDatabase.TextPlatform.Android;
		#endif
	}

	public GameObject GetItemModel( int itemID )
	{
		GameObject model = null;

		if( itemIDs != null )
		{
			for( int i=0; i<itemIDs.Length; i++ )
			{
				if( itemIDs[i] == itemID )
				{
					model = itemModels[i];
					break;
				}
			}
		}
		return model;
	}

	public ItemData GetItemData( int itemID )
	{
		ItemData itemData = null;

		if( itemIDs != null )
		{
			for( int i=0; i<itemIDs.Length; i++ )
			{
				if( itemIDs[i] == itemID )
				{
					itemData = itemDatas[i];
					break;
				}
			}
		}
		return itemData;
	}

	public int GetNextItemID( int currentItemID, ref ItemData nextItemData )
	{
		bool idFound = false;

		if( itemIDs != null )
		{
			if( currentItemID == ItemData.INVALID_ITEM_ID )
			{
				/*halmeida - if the method receives an invalid ID, I return the first valid ID.*/
				for( int i=0; i<itemIDs.Length; i++ )
				{
					if( itemIDs[i] != ItemData.INVALID_ITEM_ID )
					{
						nextItemData = itemDatas[i];
						return itemIDs[i];
					}
				}
			}
			else
			{
				/*halmeida - if the method receives a valid ID, I try to find the following valid ID.*/
				for( int i=0; i<itemIDs.Length; i++ )
				{
					if( !idFound )
					{
						if( itemIDs[i] == currentItemID )
						{
							idFound = true;
						}
					}
					else
					{
						if( itemIDs[i] != ItemData.INVALID_ITEM_ID )
						{
							nextItemData = itemDatas[i];
							return itemIDs[i];
						}
					}
				}
				/*halmeida - if either the received ID wasn't found or the following valid ID wasn't found,
				I return the first valid ID.*/
				return GetNextItemID( ItemData.INVALID_ITEM_ID, ref nextItemData );
			}
		}
		nextItemData = null;
		return ItemData.INVALID_ITEM_ID;
	}

	public int GetPreviousItemID( int currentItemID, ref ItemData previousItemData )
	{
		bool idFound = false;

		if( itemIDs != null )
		{
			if( currentItemID == ItemData.INVALID_ITEM_ID )
			{
				/*halmeida - if the method receives an invalid ID, I return the last valid ID.*/
				for( int i=itemIDs.Length-1; i>-1; i-- )
				{
					if( itemIDs[i] != ItemData.INVALID_ITEM_ID )
					{
						previousItemData = itemDatas[i];
						return itemIDs[i];
					}
				}
			}
			else
			{
				/*halmeida - if the method receives a valid ID, I try to find the following valid ID.*/
				for( int i=itemIDs.Length; i>-1; i-- )
				{
					if( !idFound )
					{
						if( itemIDs[i] == currentItemID )
						{
							idFound = true;
						}
					}
					else
					{
						if( itemIDs[i] != ItemData.INVALID_ITEM_ID )
						{
							previousItemData = itemDatas[i];
							return itemIDs[i];
						}
					}
				}
				/*halmeida - if either the received ID wasn't found or the following valid ID wasn't found,
				I return the first valid ID.*/
				return GetPreviousItemID( ItemData.INVALID_ITEM_ID, ref previousItemData );
			}
		}
		previousItemData = null;
		return ItemData.INVALID_ITEM_ID;
	}

	public bool DisplayItemInfo( ItemDisplayContext displayContext, ItemData itemData )
	{
		int itemID = ItemData.INVALID_ITEM_ID;
		GameTextDatabase textDatabase = null;
		TextBoxUIManager textBoxUIManager = null;
		bool boxCreated = false;
		bool displaySuccessful = false;
		ItemDisplayScheme displayScheme = null;
		ItemDisplayScheme.DisplayProperty[] schemeProperties = null;

		if( (itemData != null) && (itemDisplaySchemes != null) && (itemDisplayContexts != null) )
		{
			itemID = itemData.itemID;
			if( itemID != ItemData.INVALID_ITEM_ID )
			{
				textDatabase = GameTextDatabase.Instance;
				textBoxUIManager = TextBoxUIManager.Instance;
				for( int i=0; i<itemDisplayContexts.Length; i++ )
				{
					if( itemDisplayContexts[i] == displayContext )
					{
						if( itemDisplaySchemes.Length > i )
						{
							displayScheme = itemDisplaySchemes[i];
						}
					}
				}
				if( (displayScheme != null) && (textDatabase != null) && (textBoxUIManager != null) )
				{
					schemeProperties = displayScheme.properties;
					if( displayScheme.ConfiguredProperly() && (schemeProperties != null) )
					{
						string itemName = null;
						string itemDescription = null;
						string boxText = null;
						Sprite boxPicture = null;
						Vector2 boxPositionRates = Vector2.zero;
						Vector2 boxMaxSizeRates = Vector2.zero;
						bool toDiscard = false;
						for( int i=0; i<schemeProperties.Length; i++ )
						{
							boxText = null;
							boxPicture = null;
							switch( schemeProperties[i] )
							{
								case ItemDisplayScheme.DisplayProperty.Name:
									if( itemName == null )
									{
										itemDescription = textDatabase.GetItemDescription( itemID, ref itemName );
									}
									boxText = itemName;
									break;
								case ItemDisplayScheme.DisplayProperty.Description:
									if( itemDescription == null )
									{
										itemDescription = textDatabase.GetItemDescription( itemID, ref itemName );
									}
									boxText = itemDescription;
									break;
								case ItemDisplayScheme.DisplayProperty.Icon:
									boxPicture = itemData.itemIcon;
									break;
								case ItemDisplayScheme.DisplayProperty.Tutorial:
									boxText = textDatabase.GetPlatformText( textIDInventoryTutorial, currentPlatform, ref toDiscard );
									break;
							}
							boxPositionRates = displayScheme.positionRates[i];
							boxMaxSizeRates = displayScheme.maxSizeRates[i];
							boxCreated = textBoxUIManager.ShowDialogue( boxText, 0f, boxPicture, false, boxMaxSizeRates, boxPositionRates, null,
								itemData );
							if( !displaySuccessful )
							{
								displaySuccessful = boxCreated;
							}
						}
					}
				}
			}
		}
		return displaySuccessful;
	}
}
