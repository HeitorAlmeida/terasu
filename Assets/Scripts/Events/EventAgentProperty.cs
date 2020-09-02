using System.Collections;
using UnityEngine;

public class EventAgentProperty : EventBase
{
	public enum AgentProperty { Name, Colliders, MaxSpeed, DamageAcceptance, FacingSide, TransactionsTrigger }

	public int agentID;
	public AgentProperty targetProperty;
	public bool newBoolValue;
	public float newFloatValue;
	public string newStringValue;
	public bool tryToCorrect;

	private bool newValueAssigned;
	private BaseAgent agentComponent;

	protected override void Awake()
	{
		base.Awake();
		newValueAssigned = false;
		agentComponent = null;
	}

	public override void ExtractInterests( ChallengeStage stage )
	{
		base.ExtractInterests( stage );
		if( stage != null )
		{
			agentComponent = stage.GetAgent( agentID );
		}
	}

	protected override bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		if( base.EvaluateTrigger( eventSet, currentNodeID ) )
		{
			return (agentComponent != null);
		}
		return false;
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
	}

	protected override void PerformAction( float timeStep )
	{
		base.PerformAction( timeStep );
		if( !newValueAssigned && (agentComponent != null) )
		{
			switch( targetProperty )
			{
				case AgentProperty.Name:
					agentComponent.SetCurrentName( newStringValue );
					newValueAssigned = true;
					break;
				case AgentProperty.Colliders:
					agentComponent.ToggleColliders( newBoolValue );
					newValueAssigned = true;
					break;
				case AgentProperty.MaxSpeed:
					if( !tryToCorrect )
					{
						agentComponent.ForceFinalMaxSpeedTo( newFloatValue );
					}
					else
					{
						agentComponent.CorrectFinalStats();
					}
					newValueAssigned = true;
					break;
				case AgentProperty.DamageAcceptance:
					agentComponent.ToggleDamageAcceptance( newBoolValue );
					newValueAssigned = true;
					break;
				case AgentProperty.FacingSide:
					agentComponent.SetFacingSide( newBoolValue );
					newValueAssigned = true;
					break;
				case AgentProperty.TransactionsTrigger:
					agentComponent.ToggleTransactionsTrigger( newBoolValue );
					newValueAssigned = true;
					break;
			}
		}
	}

	protected override bool EvaluateConclusion()
	{
		return newValueAssigned;
	}

	protected override void RemovePreparation()
	{
		base.RemovePreparation();
	}

	protected override void AbandonInterests()
	{
		base.AbandonInterests();
		agentComponent = null;
	}

	public override void Cancel( bool forceConclusion, bool forceReset )
	{
		base.Cancel( forceConclusion, forceReset );
		newValueAssigned = false;
	}
}
