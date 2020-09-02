using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCamera : EventBase
{
	public int targetAgentID;
	public bool targetMidway;
	public bool targetInOneStep;
	public bool withinParticularNode;
	public int targetNodeID;
	public bool waitTillReached;

	private CameraController cameraController;
	private BaseAgent targetAgent;
	private bool targetAssigned;
	private bool targetReached;

	protected override void Awake()
	{
		base.Awake();
		if( !withinParticularNode )
		{
			targetNodeID = GraphNode.NODE_ID_INVALID;
		}
		cameraController = null;
		targetAgent = null;
		targetAssigned = false;
		targetReached = false;
	}

	public override void ExtractInterests( ChallengeStage stage )
	{
		base.ExtractInterests( stage );
		if( stage != null )
		{
			cameraController = stage.GetCameraController();
			targetAgent = (targetAgentID == BaseAgent.INVALID_AGENT_ID) ? null : stage.GetAgent( targetAgentID, targetNodeID );
		}
	}

	protected override bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		if( base.EvaluateTrigger( eventSet, currentNodeID ) )
		{
			return (cameraController != null);
		}
		return false;
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
		if( cameraController != null )
		{
			cameraController.SetTargetNPC( targetAgent );
			if( targetMidway )
			{
				cameraController.SetTargetMidway( true );
			}
			if( targetInOneStep )
			{
				cameraController.ApproachTargetInstantly();
			}
			targetAssigned = true;
		}
	}

	protected override void PerformAction( float timeStep )
	{
		base.PerformAction( timeStep );
		if( !targetReached && (cameraController != null) )
		{
			targetReached = cameraController.ReachedTarget();
		}
	}

	protected override bool EvaluateConclusion()
	{
		if( !waitTillReached )
		{
			return targetAssigned;
		}
		else
		{
			return (targetAssigned && targetReached);
		}
	}

	protected override void RemovePreparation()
	{
		base.RemovePreparation();
	}

	protected override void AbandonInterests()
	{
		base.AbandonInterests();
		cameraController = null;
		targetAgent = null;
	}

	public override void Cancel( bool forceConclusion, bool forceReset )
	{
		base.Cancel( forceConclusion, forceReset );
		targetAssigned = false;
		targetReached = false;
	}
}
