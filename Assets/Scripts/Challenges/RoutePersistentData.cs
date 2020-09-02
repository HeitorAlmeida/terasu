using System.Collections;
using UnityEngine;
using System; //to use the [Serializable] class modifier.

[Serializable]
public class RoutePersistentData
{
	public int[] waypointIndexes;

	public RoutePersistentData()
	{
		Clear();
	}

	public void Clear()
	{
		waypointIndexes = null;
	}

	public void AddWaypointIndex( int newIndex )
	{
		UsefulFunctions.IncreaseArray<int>( ref waypointIndexes, newIndex );
	}

	public void RemoveLastWaypointIndex()
	{
		if( waypointIndexes != null )
		{
			UsefulFunctions.DecreaseArray<int>( ref waypointIndexes, (waypointIndexes.Length-1) );
		}
	}

	public void SetAllWaypointIndexes( int[] newWaypointIndexes )
	{
		waypointIndexes = newWaypointIndexes;
	}

	public int[] GetWaypointIndexesCopy()
	{
		int[] copy = null;
		int length = 0;

		if( waypointIndexes != null )
		{
			length = waypointIndexes.Length;
			copy = new int[length];
			for( int i=0; i<length; i++ )
			{
				copy[i] = waypointIndexes[i];
			}
		}
		return copy;
	}

	public bool IsEmpty()
	{
		return (waypointIndexes == null);
	}

	public void RemoveWaypointIndexAndUpdate( int waypointIndex )
	{
		int oldIndex = -1;

		if( waypointIndexes != null )
		{
			/*halmeida - we remove every occurrence of the index and update the bigger indexes to preserve path logic.*/
			for( int i=0; i<waypointIndexes.Length; i++ )
			{
				oldIndex = waypointIndexes[i];
				if( oldIndex == waypointIndex )
				{
					UsefulFunctions.DecreaseArray<int>( ref waypointIndexes, i );
					if( waypointIndexes == null )
					{
						break;
					}
					else
					{
						i--;
					}
				}
				else
				{
					if( oldIndex > waypointIndex )
					{
						waypointIndexes[i] = oldIndex - 1;
					}
				}
			}
		}
	}

	public void RemoveWaypointIndexSetAndUpdate( int firstIndexToRemove, int totalIndexesToRemove )
	{
		int index = -1;

		if( waypointIndexes != null )
		{
			int lastIndexToRemove = firstIndexToRemove + totalIndexesToRemove - 1;
			for( int i=0; i<waypointIndexes.Length; i++ )
			{
				index = waypointIndexes[i];
				if( (index >= firstIndexToRemove) && (index <= lastIndexToRemove) )
				{
					UsefulFunctions.DecreaseArray<int>( ref waypointIndexes, i );
					if( waypointIndexes == null )
					{
						break;
					}
					else
					{
						i--;
					}
				}
				else
				{
					if( index > lastIndexToRemove )
					{
						waypointIndexes[i] = index - totalIndexesToRemove;
					}
				}
			}
		}
	}
}
