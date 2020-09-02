using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNpcRemoval : EventBase
{
	public int agentID;

	private ChallengeStage stageComponent;
	private bool npcRemoved;

	protected override void Awake()
	{
		base.Awake();
		stageComponent = null;
		npcRemoved = false;
	}

	public override void ExtractInterests( ChallengeStage stage )
	{
		base.ExtractInterests( stage );
		stageComponent = stage;
	}

	protected override bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		if( base.EvaluateTrigger( eventSet, currentNodeID ) )
		{
			return (stageComponent != null);
		}
		return false;
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
		if( stageComponent != null )
		{
			npcRemoved = stageComponent.RemoveNonPlayableCharacter( agentID );
		}
	}

	protected override void PerformAction( float timeStep )
	{
		base.PerformAction( timeStep );
		if( !npcRemoved )
		{
			if( stageComponent != null )
			{
				npcRemoved = stageComponent.RemoveNonPlayableCharacter( agentID );
			}
		}
	}

	protected override bool EvaluateConclusion()
	{
		return npcRemoved;
	}

	protected override void RemovePreparation()
	{
		base.RemovePreparation();
	}

	protected override void AbandonInterests()
	{
		base.AbandonInterests();
		stageComponent = null;
	}

	public override void Cancel( bool forceConclusion, bool forceReset )
	{
		base.Cancel( forceConclusion, forceReset );
		npcRemoved = false;
	}
}
