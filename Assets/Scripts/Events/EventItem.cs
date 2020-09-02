using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventItem : EventBase
{
	public int itemCode;
	public int placementNodeID;
	public Vector2 offsetWithinNode;

	private ChallengeStage stageComponent;
	private GameObject itemModel;
	private ItemController itemComponent;
	private Vector2 itemPosition;
	private bool itemCreated;
	private bool itemObtained;
	private bool itemOver;

	protected override void Awake()
	{
		base.Awake();
		stageComponent = null;
		itemModel = null;
		itemComponent = null;
		itemPosition = Vector2.zero;
		itemCreated = false;
		itemObtained = false;
		itemOver = false;
	}

	public override void ExtractInterests( ChallengeStage stage )
	{
		ItemDatabase itemDatabase = null;
		bool extractionSuccess = false;

		base.ExtractInterests( stage );
		stageComponent = stage;
		if( stage != null )
		{
			itemDatabase = stage.GetItemDatabase();
			if( itemDatabase != null )
			{
				itemModel = itemDatabase.GetItemModel( itemCode );
				if( (itemModel != null) && stage.GetNodeStructurePosition( placementNodeID, ref itemPosition) )
				{
					itemPosition += offsetWithinNode;
					extractionSuccess = true;
				}
			}
		}
		if( !extractionSuccess )
		{
			stage = null;
			itemModel = null;
			itemPosition = Vector2.zero;
		}
	}

	protected override bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		if( base.EvaluateTrigger( eventSet, currentNodeID ) )
		{
			return ((stageComponent != null) && (itemModel != null));
		}
		return false;
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
		if( stageComponent != null )
		{
			itemComponent = stageComponent.CreateItem( itemModel, itemPosition, placementNodeID, eventID );
			itemCreated = (itemComponent != null);
		}
	}

	protected override void PerformAction( float timeStep )
	{
		base.PerformAction( timeStep );
		/*if( !itemCreated )
		{
			if( stageComponent != null )
			{
				itemComponent = stageComponent.CreateItem( itemModel, itemPosition, placementNodeID, eventID );
				itemCreated = (itemComponent != null);
			}
		}*/
		if( itemComponent != null )
		{
			/*halmeida - the moment in which a character obtains the item is different from the moment
			in which the item finishes its disappearing animation.*/
			if( !itemObtained )
			{
				itemObtained = itemComponent.IsTriggered();
			}
			if( !itemOver )
			{
				itemOver = itemComponent.IsOver();
			}
		}
	}

	protected override bool EvaluateConclusion()
	{
		/*halmeida - if the event is concluded, the preparation will be removed and the item will
		be removed. We have to wait until the full disappearance of the item to remove it.*/
		return ( itemCreated && itemObtained && itemOver );
	}

	protected override void RemovePreparation()
	{
		base.RemovePreparation();
		itemComponent = null;
		if( stageComponent != null )
		{
			stageComponent.RemoveEventItem( eventID );
		}
	}

	protected override void AbandonInterests()
	{
		base.AbandonInterests();
		stageComponent = null;
		itemModel = null;
	}

	public override void Cancel( bool forceConclusion, bool forceReset )
	{
		bool shouldConclude = false;

		/*halmeida - if the event is canceled after the player has obtained the item and before the item
		finishes its disappearing animation, the character will have the item and the event will not yet
		have concluded. Canceling events like that will allow the player to acquire the same item many
		times. To prevent that, any cancelation of the event that takes place after the player has obtained
		the item has to consider the event as concluded.*/
		if( !forceReset )
		{
			shouldConclude = (forceConclusion || itemObtained);
		}
		base.Cancel( shouldConclude, forceReset );
		itemCreated = false;
		itemObtained = false;
		itemOver = false;
	}
}
