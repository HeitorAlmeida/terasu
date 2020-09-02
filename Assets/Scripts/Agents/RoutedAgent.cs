using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutedAgent : BaseAgent
{
	public const float TARGET_DISTANCE_TOLERANCE = 0.05f;

	public float waitAtRoutePoint;
	public bool repeatRoute;

	private RoutePersistentData routeIndexes;   //route composed of waypoint indexes.
	private int[] routeIDs;      //route composed of waypoint IDs.
	private Vector2[] routePositions;
	private int routeIndex;
	private float targetPosX;
	private float targetPosY;
	private bool targetReached;
	private bool routeReady;
	private bool routeCompleted;
	private float waitPosDuration;
	private float waitPosElapsed;

	protected override void Awake()
	{
		routeIndexes = null;
		routeIDs = null;
		routePositions = null;
		routeIndex = -1;
		targetPosX = 0f;
		targetPosY = 0f;
		targetReached = false;
		routeReady = false;
		routeCompleted = false;
		waitPosDuration = waitAtRoutePoint;
		waitPosDuration *= (waitPosDuration < 0f) ? -1f : 1f;
		waitPosElapsed = -1f;
		base.Awake();
	}

	public void AddRouteWaypointIndex( int newWaypointIndex )
	{
		if( routeIndexes == null )
		{
			routeIndexes = new RoutePersistentData();
		}
		routeIndexes.AddWaypointIndex( newWaypointIndex );
	}

	public void SetRoutePointStayDuration( float newStayDuration )
	{
		waitPosDuration = newStayDuration;
		waitPosDuration *= (waitPosDuration < 0f) ? -1f : 1f;
	}

	public void SetAllRouteWaypointIndexes( int[] newWaypointIndexes )
	{
		if( routeIndexes == null )
		{
			routeIndexes = new RoutePersistentData();
		}
		routeIndexes.SetAllWaypointIndexes( newWaypointIndexes );
		if( routeIndexes.IsEmpty() )
		{
			ClearRoute();
		}
	}

	public void SetAllRouteWaypointIDs( int[] newWaypointIDs )
	{
		routeIDs = newWaypointIDs;
	}

	public void ReduceWaypointIndexesRoute()
	{
		if( routeIndexes != null )
		{
			routeIndexes.RemoveLastWaypointIndex();
			if( routeIndexes.IsEmpty() )
			{
				ClearRoute();
			}
		}
	}

	public void RemoveWaypointIndexFromRoute( int waypointIndex )
	{
		if( routeIndexes != null )
		{
			routeIndexes.RemoveWaypointIndexAndUpdate( waypointIndex );
			if( routeIndexes.IsEmpty() )
			{
				ClearRoute();
			}
		}
	}

	public void RemoveWaypointIndexSetFromRoute( int firstIndexToRemove, int totalIndexesToRemove )
	{
		if( routeIndexes != null )
		{
			routeIndexes.RemoveWaypointIndexSetAndUpdate( firstIndexToRemove, totalIndexesToRemove );
			if( routeIndexes.IsEmpty() )
			{
				ClearRoute();
			}
		}
	}

	public RoutePersistentData GetWaypointIndexesRoute()
	{
		return routeIndexes;
	}

	public int[] GetWaypointIDsRoute()
	{
		return routeIDs;
	}

	public void ClearRoute()
	{
		if( routeIndexes != null )
		{
			routeIndexes.Clear();
			routeIndexes = null;
		}
		routeIDs = null;
		routePositions = null;
		routeReady = false;
		routeCompleted = false;
	}

	public override void Clear()
	{
		ClearRoute();
		base.Clear();
	}

	public void ExtractRoutePositions( Waypoint[] waypoints )
	{
		int[] waypointIndexes = null;
		int waypointIndex = -1;
		int waypointID = Waypoint.INVALID_WAYPOINT_ID;
		Waypoint waypoint = null;
		Vector2 routePosition = Vector2.zero;

		routePositions = null;
		if( waypoints != null )
		{
			if( routeIndexes != null )
			{
				waypointIndexes = routeIndexes.GetWaypointIndexesCopy();
				if( waypointIndexes != null )
				{
					for( int i=0; i<waypointIndexes.Length; i++ )
					{
						waypointIndex = waypointIndexes[i];
						if( (waypointIndex > -1) && (waypointIndex < waypoints.Length) )
						{
							waypoint = waypoints[waypointIndex];
							if( waypoint != null )
							{
								UsefulFunctions.IncreaseArray<Vector2>( ref routePositions, waypoint.GetPosition() );
							}
						}
					}
				}
			}
			else if( routeIDs != null )
			{
				for( int i=0; i<routeIDs.Length; i++ )
				{
					waypointID = routeIDs[i];
					for( int j=0; j<waypoints.Length; j++ )
					{
						waypoint = waypoints[j];
						if( waypoint != null )
						{
							if( waypointID == waypoint.ID )
							{
								UsefulFunctions.IncreaseArray<Vector2>( ref routePositions, waypoint.GetPosition() );
							}
						}
					}
				}
			}
		}
		routeReady = (routePositions != null);
		routeCompleted = false;
		routeIndex = -1;
		AdvanceRouteIndex();
	}

	protected override void ProgressAI( float timeStep )
	{
		Vector2 offsetToTarget = Vector2.zero;

		if( !routeCompleted && routeReady )
		{
			if( !targetReached )
			{
				targetReached = CheckIfReachedTargetPosition( ref offsetToTarget );
				if( targetReached )
				{
					if( waitPosDuration > 0f )
					{
						SetMovementDirection( Vector2.zero );
						waitPosElapsed = 0f;
					}
					else
					{
						while( targetReached )
						{
							AdvanceRouteIndex();
							/*halmeida - by advancing the route index, we might have concluded the route, setting the need
							to stop, or we might have acquired a new target, which may require a new direction.*/
							if( routeCompleted )
							{
								SetMovementDirection( Vector2.zero );
								break;
							}
							else
							{
								targetReached = CheckIfReachedTargetPosition( ref offsetToTarget );
							}
						}
					}
				}
				if( !targetReached )
				{
					SetMovementDirection( offsetToTarget.normalized );
				}
			}
			else
			{
				waitPosElapsed += timeStep;
				if( waitPosElapsed > waitPosDuration )
				{
					AdvanceRouteIndex();
				}
			}
		}
	}

	protected virtual bool CheckIfReachedTargetPosition( ref Vector2 offsetToTarget )
	{
		Vector3 currentPosition = Vector3.zero;
		float offsetX = 0f;
		float offsetY = 0f;

		currentPosition = transform.position;
		offsetX = targetPosX - currentPosition.x;
		offsetY = targetPosY - currentPosition.y;
		offsetToTarget = new Vector2( offsetX, offsetY );

		/*halmeida - since we will be moving towards the target with a speed that may be high valued and the
		position is sampled at intervals, we may never reach its exact position. That is why we need some sort
		of distance tolerance within which we will say that the target was reached.*/
		offsetX *= (offsetX < 0f) ? -1f : 1f;
		offsetY *= (offsetY < 0f) ? -1f : 1f;
		return !( (offsetX > TARGET_DISTANCE_TOLERANCE) || (offsetY > TARGET_DISTANCE_TOLERANCE) );
	}

	private void AdvanceRouteIndex()
	{
		Vector2 routePosition = Vector2.zero;

		if( routePositions != null )
		{
			routeIndex++;
			if( routeIndex >= routePositions.Length )
			{
				if( repeatRoute )
				{
					routeIndex = 0;
				}
				else
				{
					routeCompleted = true;
				}
			}
			if( !routeCompleted )
			{
				routePosition = routePositions[routeIndex];
				targetPosX = routePosition.x;
				targetPosY = routePosition.y;
				targetReached = false;
			}
		}
	}

	public bool ReachedCurrentTarget()
	{
		return targetReached;
	}

	public bool HasRoute()
	{
		return routeReady;
	}

	public bool CompletedRoute()
	{
		return routeCompleted;
	}
}
