using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventWait : EventBase
{
	public float waitTime;

	private float waitDuration;
	private float waitElapsed;

	protected override void Awake()
	{
		base.Awake();
		waitDuration = waitTime;
		waitElapsed = 0f;
	}

	public override void ExtractInterests( ChallengeStage stage )
	{
		base.ExtractInterests( stage );
	}

	protected override bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		return base.EvaluateTrigger( eventSet, currentNodeID );
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
	}

	protected override void PerformAction( float timeStep )
	{
		base.PerformAction( timeStep );
		waitElapsed += timeStep;
	}

	protected override bool EvaluateConclusion()
	{
		return (waitElapsed > waitDuration);
	}

	protected override void RemovePreparation()
	{
		base.RemovePreparation();
	}

	protected override void AbandonInterests()
	{
		base.AbandonInterests();
	}

	public override void Cancel( bool forceConclusion, bool forceReset )
	{
		base.Cancel( forceConclusion, forceReset );
		waitElapsed = 0f;
	}
}
