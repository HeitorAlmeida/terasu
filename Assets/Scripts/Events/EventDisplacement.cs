using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDisplacement : EventBase
{
	public int agentID;
	public int[] waypointIDs;

	private RoutedAgent agentComponent;
	private ChallengeStage stageComponent;
	private Waypoint[] waypoints;
	private bool routeAssigned;
	private bool routeTraveled;

	protected override void Awake()
	{
		base.Awake();
		agentComponent = null;
		stageComponent = null;
		waypoints = null;
		routeAssigned = false;
		routeTraveled = false;
	}

	public override void ExtractInterests( ChallengeStage stage )
	{
		base.ExtractInterests( stage );
		stageComponent = stage;
		if( stageComponent != null )
		{
			agentComponent = stageComponent.GetAgent( agentID );
			waypoints = stageComponent.GetAllWaypoints();
		}
	}

	protected override bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		if( base.EvaluateTrigger( eventSet, currentNodeID ) )
		{
			return ((agentComponent != null) && (waypoints != null));
		}
		return false;
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
		if( agentComponent != null )
		{
			agentComponent.repeatRoute = false;
			agentComponent.SetRoutePointStayDuration( 0f );
			agentComponent.SetAllRouteWaypointIndexes( null );
			agentComponent.SetAllRouteWaypointIDs( waypointIDs );
			agentComponent.ExtractRoutePositions( waypoints );
			routeAssigned = agentComponent.HasRoute();
		}
	}

	protected override void PerformAction( float timeStep )
	{
		base.PerformAction( timeStep );
		if( agentComponent != null )
		{
			/*halmeida - if the route wasn't assigned, it means none of the required waypoints were loaded yet.
			Maybe now they are loaded.*/
			if( !routeAssigned && (stageComponent != null))
			{
				waypoints = stageComponent.GetAllWaypoints();
				agentComponent.ExtractRoutePositions( waypoints );
				routeAssigned = agentComponent.HasRoute();
			}
		}
	}

	protected override bool EvaluateConclusion()
	{
		if( routeAssigned && (agentComponent != null) )
		{
			routeTraveled = agentComponent.CompletedRoute();
		}
		return routeTraveled;
	}

	protected override void RemovePreparation()
	{
		base.RemovePreparation();
	}

	protected override void AbandonInterests()
	{
		base.AbandonInterests();
		stageComponent = null;
		agentComponent = null;
		waypoints = null;
	}

	public override void Cancel( bool forceConclusion, bool forceReset )
	{
		base.Cancel( forceConclusion, forceReset );
		routeAssigned = false;
		routeTraveled = false;
	}
}
