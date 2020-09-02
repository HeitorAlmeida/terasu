using System.Collections;
using UnityEngine;

public class EventBase : MonoBehaviour
{
	public const int INVALID_EVENT_ID = -1;

	public const int INVALID_CHARACTER_COMMAND = -1;
	public const int CHARACTER_COMMAND_LEFT = 0;
	public const int CHARACTER_COMMAND_RIGHT = 1;
	public const int CHARACTER_COMMAND_JUMP = 2;
	public const int CHARACTER_COMMAND_BREAK = 3;
	public const int CHARACTER_COMMAND_FADE = 4;

	public int eventID;
	public int[] previousEventIDsA;
	public int[] previousEventIDsB;
	public int[] prohibitiveEventIDs;
	public int triggerNodeID;
	public bool blockInput;
	public bool saveConclusion;

	protected bool triggered;
	protected bool concluded;
	protected InputManager inputManager;
	protected bool[] eventConclusionsPreviousA;
	protected bool[] eventConclusionsPreviousB;
	protected int totalEventsPreviousA;
	protected int totalEventsPreviousB;
	protected int totalPendingEventsPreviousA;
	protected int totalPendingEventsPreviousB;
	protected int totalEventsProhibitive;
	protected bool prohibitiveEventConcluded;

	protected virtual void Awake()
	{
		triggered = false;
		concluded = false;
		inputManager = null;
		eventConclusionsPreviousA = null;
		eventConclusionsPreviousB = null;
		totalEventsPreviousA = 0;
		totalEventsPreviousB = 0;
		totalPendingEventsPreviousA = 0;
		totalPendingEventsPreviousB = 0;
		totalEventsProhibitive = 0;
		prohibitiveEventConcluded = false;
		if( previousEventIDsA != null )
		{
			totalEventsPreviousA = previousEventIDsA.Length;
			if( totalEventsPreviousA > 0 )
			{
				eventConclusionsPreviousA = new bool[totalEventsPreviousA];
				for( int i=0; i<totalEventsPreviousA; i++ )
				{
					eventConclusionsPreviousA[i] = false;
				}
				totalPendingEventsPreviousA = totalEventsPreviousA;
			}
		}
		if( previousEventIDsB != null )
		{
			totalEventsPreviousB = previousEventIDsB.Length;
			if( totalEventsPreviousB > 0 )
			{
				eventConclusionsPreviousB = new bool[totalEventsPreviousB];
				for( int i=0; i<totalEventsPreviousB; i++ )
				{
					eventConclusionsPreviousB[i] = false;
				}
				totalPendingEventsPreviousB = totalEventsPreviousB;
			}
		}
		if( prohibitiveEventIDs != null )
		{
			totalEventsProhibitive = prohibitiveEventIDs.Length;
		}
	}

	public bool IsTriggered()
	{
		return triggered;
	}

	public bool IsConcluded()
	{
		return concluded;
	}

	public virtual void ExtractInterests( ChallengeStage stage )
	{
		inputManager = null;
		if( stage != null )
		{
			inputManager = stage.GetInputManager();
		}
	}

	public bool Trigger( EventBase[] eventSet, int currentNodeID )
	{
		if( !triggered )
		{
			triggered = EvaluateTrigger( eventSet, currentNodeID );
			if( triggered )
			{
				PrepareForAction();
			}
		}
		return triggered;
	}

	protected virtual bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		if( triggerNodeID != GraphNode.NODE_ID_INVALID )
		{
			if( currentNodeID != triggerNodeID )
			{
				return false;
			}
		}
		UpdateEventConclusions( eventSet );
		if( prohibitiveEventConcluded )
		{
			return false;
		}
		if( (totalEventsPreviousA > 0) && (totalEventsPreviousB == 0) )
		{
			if( totalPendingEventsPreviousA > 0 )
			{
				return false;
			}
		}
		else if( (totalEventsPreviousA == 0) && (totalEventsPreviousB > 0) )
		{
			if( totalPendingEventsPreviousB > 0 )
			{
				return false;
			}
		}
		else if( (totalEventsPreviousA > 0) && (totalEventsPreviousB > 0) )
		{
			if( (totalPendingEventsPreviousA > 0) && (totalPendingEventsPreviousB > 0) )
			{
				return false;
			}
		}
		return true;
	}

	private void UpdateEventConclusions( EventBase[] eventSet )
	{
		EventBase otherEvent = null;
		int otherEventID = INVALID_EVENT_ID;
		bool otherEventConcluded = false;

		prohibitiveEventConcluded = false;
		totalPendingEventsPreviousA = totalEventsPreviousA;
		totalPendingEventsPreviousB = totalEventsPreviousB;
		if( (eventSet != null) && ((totalEventsPreviousA > 0) || (totalEventsPreviousB > 0) || (totalEventsProhibitive > 0)) )
		{
			for( int i=0; i<eventSet.Length; i++ )
			{
				otherEvent = eventSet[i];
				if( otherEvent != null )
				{
					otherEventID = otherEvent.eventID;
					otherEventConcluded = otherEvent.IsConcluded();
					if( !prohibitiveEventConcluded && otherEventConcluded && (totalEventsProhibitive > 0) )
					{
						for( int j=0; j<totalEventsProhibitive; j++ )
						{
							if( prohibitiveEventIDs[j] == otherEventID )
							{
								prohibitiveEventConcluded = true;
								break;
							}
						}
					}
					if( totalEventsPreviousA > 0 )
					{
						for( int j=0; j<totalEventsPreviousA; j++ )
						{
							if( previousEventIDsA[j] == otherEventID )
							{
								eventConclusionsPreviousA[j] = otherEventConcluded;
								if( otherEventConcluded )
								{
									totalPendingEventsPreviousA--;
								}
								break;
							}
						}
					}
					if( totalEventsPreviousB > 0 )
					{
						for( int j=0; j<totalEventsPreviousB; j++ )
						{
							if( previousEventIDsB[j] == otherEventID )
							{
								eventConclusionsPreviousB[j] = otherEventConcluded;
								if( otherEventConcluded )
								{
									totalPendingEventsPreviousB--;
								}
								break;
							}
						}
					}
				}
			}
		}
	}

	protected virtual void PrepareForAction()
	{
		//Debug.Log("Debug : EventBase : "+gameObject.name+" is preparing for action.");
		if( blockInput && (inputManager != null) )
		{
			inputManager.AddBlockerEvent( eventID );
		}
	}

	public void Progress( float timeStep )
	{
		if( triggered && !concluded )
		{
			PerformAction( timeStep );
			concluded = EvaluateConclusion();
			if( concluded )
			{
				RemovePreparation();
				AbandonInterests();
			}
		}
	}

	protected virtual void PerformAction( float timeStep )
	{
		/*halmeida - do nothing.*/
	}

	protected virtual bool EvaluateConclusion()
	{
		return true;
	}

	protected virtual void RemovePreparation()
	{
		//Debug.Log("Debug : EventBase : "+gameObject.name+" is removing preparation.");
		if( blockInput && (inputManager != null) )
		{
			inputManager.RemoveBlockerEvent( eventID );
		}
	}

	protected virtual void AbandonInterests()
	{
		inputManager = null;
	}

	public virtual void Cancel( bool forceConclusion, bool forceReset )
	{
		if( concluded )
		{
			concluded = false;
			triggered = false;
		}
		else if( triggered )
		{
			RemovePreparation();
			AbandonInterests();
			triggered = false;
		}
		if( !forceReset && forceConclusion )
		{
			concluded = true;
			triggered = true;
		}
	}
}
