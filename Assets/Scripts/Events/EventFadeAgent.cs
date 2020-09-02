using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFadeAgent : EventBase
{
	public int agentID;
	public bool fadeToTransparency;
	public bool fadeImmediately;
	public float fadeSpeed;
	public bool waitTotalFade;

	private BaseAgent agentComponent;
	private bool fadeStarted;
	private bool fadeConcluded;

	protected override void Awake()
	{
		base.Awake();
		agentComponent = null;
		fadeStarted = false;
		fadeConcluded = false;
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
		if( fadeImmediately && (agentComponent != null) )
		{
			if( fadeToTransparency )
			{
				agentComponent.FadeToTransparency();
			}
			else
			{
				agentComponent.FadeToOpaque();
			}
			fadeStarted = true;
			fadeConcluded = true;
		}
	}

	protected override void PerformAction( float timeStep )
	{
		base.PerformAction( timeStep );
		if( !fadeStarted && (agentComponent != null) )
		{
			if( fadeToTransparency )
			{
				agentComponent.StartFadingToTransparency( fadeSpeed );
			}
			else
			{
				agentComponent.StartFadingToOpaque( fadeSpeed );
			}
			fadeStarted = true;
		}
	}

	protected override bool EvaluateConclusion()
	{
		if( !fadeConcluded && (agentComponent != null) )
		{
			if( fadeToTransparency )
			{
				fadeConcluded = agentComponent.IsTransparent();
			}
			else
			{
				fadeConcluded = agentComponent.IsOpaque();
			}
		}
		if( waitTotalFade )
		{
			return (fadeStarted && fadeConcluded);
		}
		else
		{
			return fadeStarted;
		}
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
		fadeStarted = false;
		fadeConcluded = false;
	}
}
