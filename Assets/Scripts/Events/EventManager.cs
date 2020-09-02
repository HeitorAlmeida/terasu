using System.Collections;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	public GameObject[] eventModels;

	private GameObject[] eventObjects;
	private EventBase[] eventComponents;
	private int[] eventNodeIDs;
	private EventBase[][] eventsPerNode;
	private EventBase[] eventsWithoutNode;
	private int currentNodeID;
	private EventBase[] currentNodeEvents;
	private ChallengeStage stageComponent;

	void Awake()
	{
		GameObject eventModel = null;
		GameObject eventObject = null;
		EventBase eventComponent = null;
		int validEvents = 0;
		int insertionIndex = -1;
		bool nodeIDFound = false;
		int referencedNodes = 0;
		EventBase[] eventSet = null;

		eventObjects = null;
		eventComponents = null;
		eventNodeIDs = null;
		eventsPerNode = null;
		eventsWithoutNode = null;
		currentNodeID = GraphNode.NODE_ID_INVALID;
		currentNodeEvents = null;
		stageComponent = null;
		if( eventModels != null )
		{
			for( int i=0; i<eventModels.Length; i++ )
			{
				eventModel = eventModels[i];
				if( eventModel != null )
				{
					eventComponent = eventModel.GetComponent<EventBase>();
					if( eventComponent != null )
					{
						validEvents++;
						nodeIDFound = false;
						if( eventNodeIDs != null )
						{
							for( int j=0; j<eventNodeIDs.Length; j++ )
							{
								if( eventNodeIDs[j] == eventComponent.triggerNodeID )
								{
									nodeIDFound = true;
								}
							}
						}
						if( !nodeIDFound )
						{
							UsefulFunctions.IncreaseArray<int>( ref eventNodeIDs, eventComponent.triggerNodeID );
						}
					}
				}
			}
		}
		if( validEvents > 0 )
		{
			eventObjects = new GameObject[validEvents];
			eventComponents = new EventBase[validEvents];
			if( eventNodeIDs != null )
			{
				referencedNodes = eventNodeIDs.Length;
				eventsPerNode = new EventBase[referencedNodes][];
				for( int i=0; i<referencedNodes; i++ )
				{
					eventsPerNode[i] = null;
				}
			}
			insertionIndex = 0;
			for( int i=0; i<eventModels.Length; i++ )
			{
				eventModel = eventModels[i];
				if( eventModel != null )
				{
					eventComponent = eventModel.GetComponent<EventBase>();
					if( eventComponent != null )
					{
						eventObject = Instantiate( eventModel ) as GameObject;
						eventObject.transform.SetParent( transform );
						eventComponent = eventObject.GetComponent<EventBase>();
						eventObjects[insertionIndex] = eventObject;
						eventComponents[insertionIndex] = eventComponent;
						if( referencedNodes > 0 )
						{
							/*halmeida - this means eventNodeIDs and eventsPerNode are not null and have
							their lengths equal to referencedNodes.*/
							for( int j=0; j<referencedNodes; j++ )
							{
								if( eventNodeIDs[j] == eventComponent.triggerNodeID )
								{
									eventSet = eventsPerNode[j];
									UsefulFunctions.IncreaseArray<EventBase>( ref eventSet, eventComponent );
									eventsPerNode[j] = eventSet;
								}
							}
						}
						insertionIndex++;
					}
				}
			}
			/*halmeida - separate the events with an invalid trigger node ID value as events without node.*/
			if( referencedNodes > 0 )
			{
				for( int i=0; i<referencedNodes; i++ )
				{
					if( eventNodeIDs[i] == GraphNode.NODE_ID_INVALID )
					{
						eventsWithoutNode = eventsPerNode[i];
						break;
					}
				}
			}
		}
	}

	public void SetStage( ChallengeStage newStageComponent )
	{
		stageComponent = newStageComponent;
	}

	public void PrepareNodelessEvents()
	{
		EventBase nodelessEvent = null;

		if( (stageComponent != null) && (eventsWithoutNode != null) )
		{
			for( int i=0; i<eventsWithoutNode.Length; i++ )
			{
				nodelessEvent = eventsWithoutNode[i];
				if( !nodelessEvent.IsConcluded() )
				{
					nodelessEvent.ExtractInterests( stageComponent );
					nodelessEvent.Trigger( eventComponents, GraphNode.NODE_ID_INVALID );
				}
				/*
				if( nodelessEvent.IsTriggered() )
				{
					Debug.Log("Debug : EventManager : EVENT "+nodelessEvent.eventID+" IS TRIGGERED, NODE ID "+currentNodeID+".");
				}
				if( nodelessEvent.IsConcluded() )
				{
					Debug.Log("Debug : EventManager : EVENT "+nodelessEvent.eventID+" WAS ALREADY CONCLUDED.");
				}
				*/
			}
		}
	}

	public void PrepareNodeEvents( int loadedNodeID )
	{
		EventBase nodeEvent = null;

		currentNodeID = GraphNode.NODE_ID_INVALID;
		currentNodeEvents = null;
		if( (stageComponent != null) && (eventsPerNode != null) )
		{
			/*halmeida - if eventsPerNode is not null, it means eventNodeIDs is also not null and
			both have the same length.*/
			currentNodeID = loadedNodeID;
			for( int i=0; i<eventNodeIDs.Length; i++ )
			{
				if( eventNodeIDs[i] == currentNodeID )
				{
					currentNodeEvents = eventsPerNode[i];
					if( currentNodeEvents != null )
					{
						for( int j=0; j<currentNodeEvents.Length; j++ )
						{
							nodeEvent = currentNodeEvents[j];
							if( !nodeEvent.IsConcluded() )
							{
								nodeEvent.ExtractInterests( stageComponent );
								nodeEvent.Trigger( eventComponents, currentNodeID );
							}
							/*
							if( nodeEvent.IsTriggered() )
							{
								Debug.Log("Debug : EventManager : EVENT "+nodeEvent.eventID+" IS TRIGGERED, NODE ID "+currentNodeID+".");
							}
							if( nodeEvent.IsConcluded() )
							{
								Debug.Log("Debug : EventManager : EVENT "+nodeEvent.eventID+" WAS ALREADY CONCLUDED.");
							}
							*/
						}
					}
					break;
				}
			}
		}
	}

	public void Progress( float timeStep )
	{
		EventBase nodeEvent = null;
		EventBase nodelessEvent = null;
		bool eventConcluded = false;

		if( currentNodeEvents != null )
		{
			/*halmeida - since currentNodeEvents is just a subset of eventComponents, it
			means all of its elements are not null, for they were already validated during
			the Awake().*/ 
			for( int i=0; i<currentNodeEvents.Length; i++ )
			{
				nodeEvent = currentNodeEvents[i];
				if( !nodeEvent.IsConcluded() )
				{
					nodeEvent.Progress( timeStep );
					if( nodeEvent.IsConcluded() )
					{
						eventConcluded = true;
					}
				}
			}
		}
		if( eventsWithoutNode != null )
		{
			/*halmeida - since eventsWithoutNode is just a subset of eventComponents, it
			means all of its elements are not null, for they were already validated during
			the Awake().*/
			for( int i=0; i<eventsWithoutNode.Length; i++ )
			{
				nodelessEvent = eventsWithoutNode[i];
				if( !nodelessEvent.IsConcluded() )
				{
					nodelessEvent.Progress( timeStep );
					if( nodelessEvent.IsConcluded() )
					{
						eventConcluded = true;
					}
				}
			}
		}
		if( eventConcluded )
		/*halmeida - when an event is concluded, it may have made it possible for
		other events to trigger themselves. Furthermore, an event that is triggered
		might be immediately concluded if it, for example, takes no time. Since we
		cannot tell how much of the time of this processing (how much of the timeStep
		interval) was spent for the event to conclude, it would be very incorrect to
		just repeat the Progress call with the same timeStep. So, to allow at least
		the immediate events to trigger and conclude themselves, we repeat the Progress
		call with zero timeStep.*/
		{
			if( currentNodeEvents != null )
			{
				for( int i=0; i<currentNodeEvents.Length; i++ )
				{
					nodeEvent = currentNodeEvents[i];
					if( !nodeEvent.IsConcluded() )
					{
						nodeEvent.ExtractInterests( stageComponent );
						nodeEvent.Trigger( eventComponents, currentNodeID );
					}
				}
			}
			if( eventsWithoutNode != null )
			{
				for( int i=0; i<eventsWithoutNode.Length; i++ )
				{
					nodelessEvent = eventsWithoutNode[i];
					if( !nodelessEvent.IsConcluded() )
					{
						nodelessEvent.ExtractInterests( stageComponent );
						nodelessEvent.Trigger( eventComponents, currentNodeID );
					}
				}
			}
			Progress( 0f );
		}
	}

	public void CloseNodeEvents()
	{
		EventBase nodeEvent = null;

		if( currentNodeEvents != null )
		{
			/*halmeida - since currentNodeEvents is just a subset of eventComponents, it
			means all of its elements are not null, for they were already validated during
			the Awake().*/ 
			for( int i=0; i<currentNodeEvents.Length; i++ )
			{
				nodeEvent = currentNodeEvents[i];
				if( !nodeEvent.IsConcluded() )
				{
					nodeEvent.Cancel( false, false );
				}
			}
		}
	}

	public int[] GetAllEventIDs( ref bool[] eventConclusions )
	{
		EventBase eventComponent = null;
		int[] eventIDs = null;
		int totalEvents = 0;

		eventConclusions = null;
		if( eventComponents != null )
		{
			totalEvents = eventComponents.Length;
			eventIDs = new int[totalEvents];
			eventConclusions = new bool[totalEvents];
			for( int i=0; i<totalEvents; i++ )
			{
				eventComponent = eventComponents[i];
				/*halmeida - eventComponent is necessarily not null, for this was certified in Awake().*/
				eventIDs[i] = eventComponent.eventID;
				/*halmeida - some events, like the ones within the ending chain of events, have to be playable more than once.
				The only way we can do that without breaking the chain of events itself is to just not save the conclusion
				of these events to the save files. The conclusion will be effective until the player closes the game and opens
				it again, at which point the game will have forgotten that these events have ever been concluded.*/
				eventConclusions[i] = (eventComponent.IsConcluded() && eventComponent.saveConclusion);
			}
		}
		return eventIDs;
	}

	public bool[] GetEventConclusions( int[] eventIDs )
	{
		EventBase eventComponent = null;
		bool[] eventConclusions = null;
		int length = 0;
		int eventID = -1;

		if( eventIDs != null )
		{
			length = eventIDs.Length;
			if( length > 0 )
			{
				eventConclusions = new bool[length];
				for( int i=0; i<length; i++ )
				{
					eventConclusions[i] = false;
				}
				if( eventComponents != null )
				{
					for( int i=0; i<length; i++ )
					{
						eventID = eventIDs[i];
						for( int j=0; j<eventComponents.Length; j++ )
						{
							eventComponent = eventComponents[j];
							if( eventComponent != null )
							{
								if( eventID == eventComponent.eventID )
								{
									if( eventComponent.IsConcluded() )
									{
										eventConclusions[i] = true;
									}
									break;
								}
							}
						}
					}
				}
			}
		}
		return eventConclusions;
	}

	public void LoadEventConclusions( int[] eventIDs, bool[] eventConclusions )
	{
		EventBase eventComponent = null;
		int loadedID = -1;
		bool loadedConclusion = false;

		if( (eventComponents != null) && (eventIDs != null) && (eventConclusions != null) )
		{
			for( int i=0; i<eventIDs.Length; i++ )
			{
				if( eventConclusions.Length > i )
				{
					loadedID = eventIDs[i];
					loadedConclusion = eventConclusions[i];
					for( int j=0; j<eventComponents.Length; j++ )
					{
						eventComponent = eventComponents[j];
						/*halmeida - eventComponent is necessarily not null, for that was certified in Awake().*/
						if( eventComponent.eventID == loadedID )
						{
							if( eventComponent.saveConclusion )
							{
								eventComponent.Cancel( loadedConclusion, false );
							}
						}
					}
				}
			}
		}
	}

	public void Reset()
	{
		EventBase eventComponent = null;

		if( eventComponents != null )
		{
			for( int i=0; i<eventComponents.Length; i++ )
			{
				eventComponent = eventComponents[i];
				if( eventComponent != null )
				{
					eventComponent.Cancel( false, true );
				}
			}
		}
		currentNodeID = GraphNode.NODE_ID_INVALID;
		currentNodeEvents = null;
	}
}
