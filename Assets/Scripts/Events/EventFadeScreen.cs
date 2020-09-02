using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFadeScreen : EventBase
{
	public bool fadeOut;
	public Color fadeOutColor;
	public bool waitTillFaded;

	private bool fadeRequested;
	private bool fadeCompleted;
	private ChallengeStage stageComponent;

	protected override void Awake()
	{
		base.Awake();
		fadeRequested = false;
		fadeCompleted = false;
		stageComponent = null;
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
	}

	protected override void PerformAction( float timeStep )
	{
		base.PerformAction( timeStep );
		if( (stageComponent != null) && !fadeRequested )
		{
			if( fadeOut )
			{
				stageComponent.RequestFadeOut( new Vector3( fadeOutColor.r, fadeOutColor.g, fadeOutColor.b ) );
			}
			else
			{
				stageComponent.RequestFadeIn();
			}
			fadeRequested = true;
		}
	}

	protected override bool EvaluateConclusion()
	{
		Vector3 fadeRGB = Vector3.zero;

		if( !waitTillFaded )
		{
			return fadeRequested;
		}
		else
		{
			if( fadeRequested && (stageComponent != null) )
			{
				if( fadeOut )
				{
					fadeCompleted = !stageComponent.RequestingFadeOut( ref fadeRGB );
				}
				else
				{
					fadeCompleted = !stageComponent.RequestingFadeIn();
				}
			}
		}
		return fadeCompleted;
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
		fadeRequested = false;
		fadeCompleted = false;
	}
}
