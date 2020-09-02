using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCluster : MonoBehaviour
{
	public const int REWARD_CODE_INVALID = -1;
	public const int REWARD_CODE_HP_RECOVERY = 0;

	public const float DEFAULT_SCALE_CHANGE_SECOND_ITEM = 1.4f;
	public const float DEFAULT_SCALE_CHANGE_LAST_ITEM = 1.8f;

	public int rewardCode;
	public Vector3 scaleChangeSecondItem;
	public Vector3 scaleChangeLastItem;

	private ItemController[] itemControllers;
	private int totalItems;
	private bool[] triggeredItems;
	private int totalTriggered;
	private PlayerAgent targetPlayer;
	private bool rewardGiven;

	void Awake()
	{
		itemControllers = null;
		totalItems = 0;
		triggeredItems = null;
		totalTriggered = 0;
		targetPlayer = null;
		rewardGiven = false;
	}

	public void SetRewardTarget( PlayerAgent newTargetPlayer )
	{
		targetPlayer = newTargetPlayer;
	}

	public void IncludeItem( ItemController newItemController )
	{
		bool itemTriggered = false;

		if( newItemController != null )
		{
			UsefulFunctions.IncreaseArray<ItemController>( ref itemControllers, newItemController );
			totalItems++;
			itemTriggered = newItemController.IsTriggered();
			UsefulFunctions.IncreaseArray<bool>( ref triggeredItems, itemTriggered );
			if( itemTriggered )
			{
				totalTriggered++;
			}
		}
	}

	public void ExcludeItem( ItemController oldItemController )
	{
		if( oldItemController != null )
		{
			for( int i=0; i<totalItems; i++ )
			{
				if( itemControllers[i] == oldItemController )
				{
					UsefulFunctions.DecreaseArray<ItemController>( ref itemControllers, i );
					if( triggeredItems[i] )
					{
						totalTriggered--;
					}
					UsefulFunctions.DecreaseArray<bool>( ref triggeredItems, i );
					totalItems--;
					break;
				}
			}
		}
	}

	public void Progress( float timeStep )
	{
		int previousTotalTriggered = 0;
		ItemController itemController = null;

		if( (itemControllers != null) && !rewardGiven )
		{
			previousTotalTriggered = totalTriggered;
			for( int i=0; i<totalItems; i++ )
			{
				if( !triggeredItems[i] )
				{
					itemController = itemControllers[i];
					if( itemController.IsTriggered() )
					{
						triggeredItems[i] = true;
						totalTriggered++;
					}
				}
			}
			if( previousTotalTriggered != totalTriggered )
			{
				UpdateInfluenceOverItems();
			}
			if( totalTriggered == totalItems )
			{
				RewardPlayer();
			}
		}
	}

	private void UpdateInfluenceOverItems()
	{
		ItemController itemController = null;
		bool changeAppearances = false;
		Vector3 scaleChange = Vector3.one;

		if( totalTriggered == (totalItems-1) )
		{
			changeAppearances = true;
			scaleChange = scaleChangeLastItem;
		}
		else
		{
			if( totalTriggered == 1 )
			{
				changeAppearances = true;
				scaleChange = scaleChangeSecondItem;
			}
		}
		if( changeAppearances )
		{
			for( int i=0; i<totalItems; i++ )
			{
				if( !triggeredItems[i] )
				{
					itemController = itemControllers[i];
					itemController.RequestScaleChange( scaleChange, true );
					itemController.EvolveAppearance( scaleChange.x );
				}
			}
		}
	}

	private void RewardPlayer()
	{
		switch( rewardCode )
		{
			case REWARD_CODE_HP_RECOVERY:
				if( targetPlayer != null )
				{
					targetPlayer.FillHP( false );
					rewardGiven = true;
				}
				break;
		}
	}

	public bool GetRewardGiven()
	{
		return rewardGiven;
	}

	public int GetTotalItems()
	{
		return totalItems;
	}

	public void Clear()
	{
		itemControllers = null;
		triggeredItems = null;
		totalItems = 0;
		totalTriggered = 0;
		targetPlayer = null;
	}
}
