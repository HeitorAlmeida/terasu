using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNpcCreation : EventBase
{
	public GameObject npcModel;
	public int placementNodeID;
	public Vector2 offsetWithinNode;
	public bool facingRight;
	public bool isEnemy;

	private ChallengeStage stageComponent;
	private bool npcCreated;
	private bool sideToFace;

	protected override void Awake()
	{
		base.Awake();
		stageComponent = null;
		npcCreated = false;
		sideToFace = ( facingRight ? BaseAnimator.SIDE_RIGHT : BaseAnimator.SIDE_LEFT );
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
			return ((stageComponent != null) && (npcModel != null));
		}
		return false;
	}

	protected override void PrepareForAction()
	{
		PlayerAgent npcComponent = null;

		base.PrepareForAction();
		if( stageComponent != null )
		{
			npcComponent = stageComponent.CreateNonPlayableCharacter( npcModel, placementNodeID, offsetWithinNode, sideToFace, isEnemy );
			npcCreated = (npcComponent != null);
		}
	}

	protected override void PerformAction( float timeStep )
	{
		PlayerAgent npcComponent = null;

		base.PerformAction( timeStep );
		if( !npcCreated )
		{
			if( stageComponent != null )
			{
				npcComponent = stageComponent.CreateNonPlayableCharacter( npcModel, placementNodeID, offsetWithinNode, sideToFace, isEnemy );
				npcCreated = (npcComponent != null);
			}
		}
	}

	protected override bool EvaluateConclusion()
	{
		return npcCreated;
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
		npcCreated = false;
	}
}
