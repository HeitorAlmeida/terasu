using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPositioning : EventBase
{
	public int agentID;
	public int waypointID;

	private BaseAgent agentComponent;
	private Waypoint waypointComponent;
	private Vector2 waypointPosition;
	private bool agentPositioned;

	protected override void Awake()
	{
		base.Awake();
		agentComponent = null;
		waypointComponent = null;
		waypointPosition = Vector2.zero;
		agentPositioned = false;
	}

	public override void ExtractInterests( ChallengeStage stage )
	{
		base.ExtractInterests( stage );
		if( stage != null )
		{
			agentComponent = stage.GetAgent( agentID );
			/*halmeida - A positioning event can only place the agent at a position within the currently
			loaded structure (current room), because if the position change went through any doors, the
			crossing of the door would not be registered and the loading of the scenery would break.*/
			waypointComponent = stage.GetWaypointFromCurrentStruct( waypointID );
		}
	}

	protected override bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		if( base.EvaluateTrigger( eventSet, currentNodeID ) )
		{
			return ((agentComponent != null) && (waypointComponent != null));
		}
		return false;
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
		if( waypointComponent != null )
		{
			waypointPosition = waypointComponent.GetPosition();
		}
	}

	protected override void PerformAction( float timeStep )
	{
		GameObject agentObject = null;
		Vector3 localPosition = Vector3.zero;

		base.PerformAction( timeStep );
		if( agentComponent != null )
		{
			agentObject = agentComponent.gameObject;
			localPosition = agentObject.transform.localPosition;
			localPosition.x = waypointPosition.x;
			localPosition.y = waypointPosition.y;
			agentObject.transform.localPosition = localPosition;
			agentPositioned = true;
		}
	}

	protected override bool EvaluateConclusion()
	{
		return agentPositioned;
	}

	protected override void RemovePreparation()
	{
		base.RemovePreparation();
	}

	protected override void AbandonInterests()
	{
		base.AbandonInterests();
		agentComponent = null;
		waypointComponent = null;
	}

	public override void Cancel( bool forceConclusion, bool forceReset )
	{
		base.Cancel( forceConclusion, forceReset );
		agentPositioned = false;
	}
}
