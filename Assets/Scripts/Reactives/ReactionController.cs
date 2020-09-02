using UnityEngine;
using System.Collections;

public class ReactionController : MonoBehaviour
{
	public bool continuousEffect;

	protected GameObject triggerSourceObject;
	protected GameObject[] triggerObjects;
	protected GameObject[] triggeringObjects;
	protected GameObject targetObject;
	protected bool triggered;

	protected virtual void Awake()
	{
		Clear();
	}

	public bool SetTriggerSource( GameObject candidateSource )
	{
		ClearTriggers();
		if( SourceCompatible( candidateSource ) )
		{
			ExtractTriggers();
			return true;
		}
		ClearTriggers();
		return false;
	}

	protected virtual bool SourceCompatible( GameObject candidateSource )
	{
		if( candidateSource != null )
		{
			triggerSourceObject = candidateSource;
			return true;
		}
		return false;
	}

	protected virtual void ExtractTriggers()
	{
		triggerObjects = null;
	}

	public bool SetTarget( GameObject candidateTarget )
	{
		ClearTarget();
		return TargetCompatible( candidateTarget );
	}

	protected virtual bool TargetCompatible( GameObject candidateTarget )
	{
		if( candidateTarget != null )
		{
			targetObject = candidateTarget;
			return true;
		}
		return false;
	}

	public bool TryToTrigger( GameObject possibleTrigger )
	{
		int triggerIndex = -1;

		if( continuousEffect || !triggered )
		{
			triggerIndex = GetTriggerIndex( possibleTrigger );
			if( triggerIndex > -1 )
			{
				Trigger( triggerIndex );
				if( continuousEffect )
				{
					if( GetTriggeringIndex( possibleTrigger ) == -1 )
					{
						AddTriggeringObject( triggerIndex );
					}
				}
				return true;
			}
		}
		return false;
	}

	protected virtual int GetTriggerIndex( GameObject possibleTrigger )
	{
		if( (triggerObjects != null) && (possibleTrigger != null) )
		{
			for( int i=0; i<triggerObjects.Length; i++ )
			{
				if( triggerObjects[i] == possibleTrigger )
				{
					return i;
				}
			}
		}
		return -1;
	}

	protected virtual void Trigger( int triggerIndex )
	{
		triggered = true;
	}

	protected virtual void AddTriggeringObject( int triggerIndex )
	{
		UsefulFunctions.IncreaseArray<GameObject>( ref triggeringObjects, triggerObjects[triggerIndex] );
	}

	protected virtual int GetTriggeringIndex( GameObject possibleTrigger )
	{
		if( (triggeringObjects != null) && (possibleTrigger != null) )
		{
			for( int i=0; i<triggeringObjects.Length; i++ )
			{
				if( triggeringObjects[i] == possibleTrigger )
				{
					return i;
				}
			}
		}
		return -1;
	}

	public void TryToUntrigger( GameObject possibleTrigger )
	{
		int triggeringIndex = -1;

		if( continuousEffect && triggered )
		{
			triggeringIndex = GetTriggeringIndex( possibleTrigger );
			if( triggeringIndex > -1 )
			{
				RemoveTriggeringObject( triggeringIndex );
				if( triggeringObjects == null )
				{
					Untrigger();
				}
			}
		}
	}

	protected virtual void RemoveTriggeringObject( int triggeringIndex )
	{
		UsefulFunctions.DecreaseArray<GameObject>( ref triggeringObjects, triggeringIndex );
	}

	protected virtual void Untrigger()
	{
		triggered = false;
	}

	public bool IsTriggered()
	{
		return triggered;
	}

	public virtual void Clear()
	{
		ClearTriggers();
		ClearTarget();
	}

	protected virtual void ClearTriggers()
	{
		triggerSourceObject = null;
		triggerObjects = null;
		triggeringObjects = null;
		triggered = false;
	}

	protected virtual void ClearTarget()
	{
		targetObject = null;
	}
}
