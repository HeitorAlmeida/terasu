using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObstacle : EventBase
{
	public int obstacleCode;
	public int placementNodeID;
	public Vector2 offsetWithinNode;

	private ChallengeStage stageComponent;
	private GameObject obstacleModel;
	private ObstacleController obstacleComponent;
	private Vector2 obstaclePosition;
	private bool obstacleCreated;
	private bool obstacleDestroyed;
	private bool obstacleOver;

	protected override void Awake()
	{
		base.Awake();
		stageComponent = null;
		obstacleModel = null;
		obstacleComponent = null;
		obstaclePosition = Vector2.zero;
		obstacleCreated = false;
		obstacleDestroyed = false;
		obstacleOver = false;
	}

	public override void ExtractInterests( ChallengeStage stage )
	{
		ObstacleDatabase obstacleDatabase = null;
		bool extractionSuccess = false;

		base.ExtractInterests( stage );
		stageComponent = stage;
		if( stage != null )
		{
			obstacleDatabase = stage.GetObstacleDatabase();
			if( obstacleDatabase != null )
			{
				obstacleModel = obstacleDatabase.GetObstacleModel( obstacleCode );
				if( (obstacleModel != null) && stage.GetNodeStructurePosition( placementNodeID, ref obstaclePosition) )
				{
					obstaclePosition += offsetWithinNode;
					extractionSuccess = true;
				}
			}
		}
		if( !extractionSuccess )
		{
			stage = null;
			obstacleModel = null;
			obstaclePosition = Vector2.zero;
		}
	}

	protected override bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		if( base.EvaluateTrigger( eventSet, currentNodeID ) )
		{
			return ((stageComponent != null) && (obstacleModel != null));
		}
		return false;
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
		if( stageComponent != null )
		{
			obstacleComponent = stageComponent.CreateEventObstacle( obstacleModel, obstaclePosition, eventID );
			obstacleCreated = (obstacleComponent != null);
		}
	}

	protected override void PerformAction( float timeStep )
	{
		base.PerformAction( timeStep );
		/*if( !obstacleCreated )
		{
			if( stageComponent != null )
			{
				obstacleComponent = stageComponent.CreateEventObstacle( obstacleModel, obstaclePosition, eventID );
				obstacleCreated = (obstacleComponent != null);
			}
		}*/
		if( obstacleComponent != null )
		{
			/*halmeida - the moment in which an obstacle is destroyed is different from the moment
			in which the obstacle finishes its disappearing animation.*/
			if( !obstacleDestroyed )
			{
				obstacleDestroyed = obstacleComponent.IsDestroyed();
			}
			if( !obstacleOver )
			{
				obstacleOver = obstacleComponent.IsOver();
			}
		}
	}

	protected override bool EvaluateConclusion()
	{
		/*halmeida - if the event is concluded, the preparation will be removed and the obstacle will
		be removed. We have to wait until the full disappearance of the obstacle to remove it.*/
		return ( obstacleCreated && obstacleDestroyed && obstacleOver );
	}

	protected override void RemovePreparation()
	{
		base.RemovePreparation();
		obstacleComponent = null;
		if( stageComponent != null )
		{
			stageComponent.RemoveEventObstacle( eventID );
		}
	}

	protected override void AbandonInterests()
	{
		base.AbandonInterests();
		stageComponent = null;
		obstacleModel = null;
	}

	public override void Cancel( bool forceConclusion, bool forceReset )
	{
		bool shouldConclude = false;

		/*halmeida - if the event is canceled after the player has destroyed the obstacle and before the obstacle
		finishes its disappearing animation, the character will have destroyed the obstacle and the event will not yet
		have concluded. Canceling events like that would discard the player's effort to destroy the obstacle, since
		the event would create the obstacle all over again when it got triggered the next time. To prevent that, any
		cancelation of the event that takes place after the player has destroyed the obstacle has to consider the event
		as concluded.*/
		if( !forceReset )
		{
			shouldConclude = (forceConclusion || obstacleDestroyed);
		}
		base.Cancel( shouldConclude, forceReset );
		obstacleCreated = false;
		obstacleDestroyed = false;
		obstacleOver = false;
	}
}
