using System.Collections;
using UnityEngine;
using System; //to create classes with [Serializable]

[Serializable]
public class StructurePersistentData
{
	public int structureID;
	public int[] blueprintFront;
	public int[] blueprintBack;
	public int rows;
	public int columns;
	public int[] gapUpColumns;
	public int[] gapLeftRows;
	public int[] gapDownColumns;
	public int[] gapRightRows;
	public int difficulty;
	public int[] itemCodes;
	public float[] itemOffsetsX;
	public float[] itemOffsetsY;
	public int[] itemToItemClusters;
	public int[] blueprintToOuterBox;
	public int[] blueprintToOuterTri;
	public int[] outerBoxFirstRows;
	public int[] outerBoxFirstColumns;
	public int[] outerBoxLastRows;
	public int[] outerBoxLastColumns;
	public int[] outerTriFirstRows;
	public int[] outerTriFirstColumns;
	public int[] outerTriLastRows;
	public int[] outerTriLastColumns;
	public int[] outerTriCorners;
	public int[] itemClusterRewardCodes;
	public float[] waypointOffsetsX;
	public float[] waypointOffsetsY;
	public int[] waypointIDs;
	public int[] enemyCodes;
	public float[] enemyOffsetsX;
	public float[] enemyOffsetsY;
	public RoutePersistentData[] enemyRoutes;

	public StructurePersistentData()
	{
		Initialize();
	}

	public void Initialize()
	{
		structureID = 0;
		blueprintFront = null;
		blueprintBack = null;
		rows = 0;
		columns = 0;
		gapUpColumns = null;
		gapLeftRows = null;
		gapDownColumns = null;
		gapRightRows = null;
		difficulty = 0;
		itemCodes = null;
		itemOffsetsX = null;
		itemOffsetsY = null;
		itemToItemClusters = null;
		blueprintToOuterBox = null;
		blueprintToOuterTri = null;
		outerBoxFirstRows = null;
		outerBoxFirstColumns = null;
		outerBoxLastRows = null;
		outerBoxLastColumns = null;
		outerTriFirstRows = null;
		outerTriFirstColumns = null;
		outerTriLastRows = null;
		outerTriLastColumns = null;
		outerTriCorners = null;
		itemClusterRewardCodes = null;
		waypointOffsetsX = null;
		waypointOffsetsY = null;
		waypointIDs = null;
		enemyCodes = null;
		enemyOffsetsX = null;
		enemyOffsetsY = null;
		enemyRoutes = null;
	}

	public void Clear()
	{
		ClearEnemyRoutes();
		Initialize();
	}

	public void ClearEnemyRoutes()
	{
		RoutePersistentData routeData = null;

		if( enemyRoutes != null )
		{
			for( int i=0; i<enemyRoutes.Length; i++ )
			{
				routeData = enemyRoutes[i];
				if( routeData != null )
				{
					routeData.Clear();
					enemyRoutes[i] = null;
				}
			}
			enemyRoutes = null;
		}
	}

	public bool SetBlueprint( int newRows, int newColumns, int[][] newBlueprintFront, int[][] newBlueprintToOuterBox, int[][] newBlueprintToOuterTri,
		int[][] newBlueprintBack )
	{
		int[] newBlueprintRowFront = null;
		int[] newBlueprintRowBack = null;
		int[] newBlueprintToBoxRow = null;
		int[] newBlueprintToTriRow = null;
		bool clearRow = false;
		int linearIndex = -1;
		int totalSlots = 0;

		if( (newBlueprintFront != null) && (newRows > 0) && (newColumns > 0) )
		{
			totalSlots = newRows * newColumns;
			blueprintFront = new int[totalSlots];
			blueprintToOuterBox = new int[totalSlots];
			blueprintToOuterTri = new int[totalSlots];
			blueprintBack = new int[totalSlots];
			for( int i=0; i<newRows; i++ )
			{
				if( i < newBlueprintFront.Length )
				{
					newBlueprintRowFront = newBlueprintFront[i];
					if( newBlueprintRowFront != null )
					{
						newBlueprintToBoxRow = null;
						if( newBlueprintToOuterBox != null )
						{
							if( newBlueprintToOuterBox.Length > i )
							{
								newBlueprintToBoxRow = newBlueprintToOuterBox[i];
							}
						}
						newBlueprintToTriRow = null;
						if( newBlueprintToOuterTri != null )
						{
							if( newBlueprintToOuterTri.Length > i )
							{
								newBlueprintToTriRow = newBlueprintToOuterTri[i];
							}
						}
						newBlueprintRowBack = null;
						if( newBlueprintBack != null )
						{
							if( newBlueprintBack.Length > i )
							{
								newBlueprintRowBack = newBlueprintBack[i];
							}
						}
						for( int j=0; j<newColumns; j++ )
						{
							linearIndex = i * newColumns + j;
							if( j < newBlueprintRowFront.Length )
							{
								blueprintFront[linearIndex] = newBlueprintRowFront[j];
							}
							else
							{
								blueprintFront[linearIndex] = ChallengeStructure.BLUEPRINT_CODE_NO_TILE;
							}
							blueprintToOuterBox[linearIndex] = -1;
							if( newBlueprintToBoxRow != null )
							{
								if( newBlueprintToBoxRow.Length > j )
								{
									blueprintToOuterBox[linearIndex] = newBlueprintToBoxRow[j];
								}
							}
							blueprintToOuterTri[linearIndex] = -1;
							if( newBlueprintToTriRow != null )
							{
								if( newBlueprintToTriRow.Length > j )
								{
									blueprintToOuterTri[linearIndex] = newBlueprintToTriRow[j];
								}
							}
							blueprintBack[linearIndex] = ChallengeStructure.BLUEPRINT_CODE_NO_TILE;
							if( newBlueprintRowBack != null )
							{
								if( newBlueprintRowBack.Length > j )
								{
									blueprintBack[linearIndex] = newBlueprintRowBack[j];
								}
							}
							if( blueprintFront[linearIndex] == ChallengeStructure.BLUEPRINT_CODE_NO_TILE )
							{
								if( i == 0 )
								{
									UsefulFunctions.IncreaseArray<int>( ref gapUpColumns, j );
								}
								else
								{
									if( i == (newRows-1) )
									{
										UsefulFunctions.IncreaseArray<int>( ref gapDownColumns, j );
									}
								}
								if( j == 0 )
								{
									UsefulFunctions.IncreaseArray<int>( ref gapLeftRows, i );
								}
								else
								{
									if( j == (newColumns-1) )
									{
										UsefulFunctions.IncreaseArray<int>( ref gapRightRows, i );
									}
								}
							}
						}
					}
					else
					{
						clearRow = true;
					}
				}
				else
				{
					clearRow = true;
				}
				if( clearRow )
				{
					for( int j=0; j<newColumns; j++ )
					{
						linearIndex = i * newColumns + j;
						blueprintFront[linearIndex] = ChallengeStructure.BLUEPRINT_CODE_NO_TILE;
						blueprintToOuterBox[linearIndex] = -1;
						blueprintToOuterTri[linearIndex] = -1;
						blueprintBack[linearIndex] = ChallengeStructure.BLUEPRINT_CODE_NO_TILE;
						if( i == 0 )
						{
							UsefulFunctions.IncreaseArray<int>( ref gapUpColumns, j );
						}
						else
						{
							if( i == (newRows-1) )
							{
								UsefulFunctions.IncreaseArray<int>( ref gapDownColumns, j );
							}
						}
						if( j == 0 )
						{
							UsefulFunctions.IncreaseArray<int>( ref gapLeftRows, i );
						}
						else
						{
							if( j == (newColumns-1) )
							{
								UsefulFunctions.IncreaseArray<int>( ref gapRightRows, i );
							}
						}
					}
				}
			}
			rows = newRows;
			columns = newColumns;
			return true;
		}
		return false;
	}

	public void SetItemData( int[] newItemCodes, Vector2[] newItemOffsets )
	{
		int length = 0;
		Vector2 offset = Vector2.zero;

		itemCodes = null;
		itemOffsetsX = null;
		itemOffsetsY = null;
		itemToItemClusters = null;
		if( (newItemCodes != null) && (newItemOffsets != null) )
		{
			length = newItemCodes.Length;
			if( newItemOffsets.Length == length )
			{
				itemCodes = new int[length];
				itemOffsetsX = new float[length];
				itemOffsetsY = new float[length];
				itemToItemClusters = new int[length];
				for( int i=0; i<length; i++ )
				{
					itemCodes[i] = newItemCodes[i];
					offset = newItemOffsets[i];
					itemOffsetsX[i] = offset.x;
					itemOffsetsY[i] = offset.y;
					itemToItemClusters[i] = -1;
				}
			}
		}
	}

	public void SetItemClusterData( int[] newItemClusterRewardCodes, int[] newItemToItemClusters )
	{
		int length = 0;

		itemClusterRewardCodes = null;
		if( newItemClusterRewardCodes != null )
		{
			length = newItemClusterRewardCodes.Length;
			itemClusterRewardCodes = new int[length];
			for( int i=0; i<length; i++ )
			{
				itemClusterRewardCodes[i] = newItemClusterRewardCodes[i];
			}
		}
		if( (newItemToItemClusters != null) && (itemToItemClusters != null) )
		{
			for( int i=0; i<newItemToItemClusters.Length; i++ )
			{
				if( i < itemToItemClusters.Length )
				{
					itemToItemClusters[i] = newItemToItemClusters[i];
				}
			}
		}
	}

	public void SetOuterBoxLimits( int[] newBoxFirstRows, int[] newBoxFirstColumns, int[] newBoxLastRows, int[] newBoxLastColumns )
	{
		outerBoxFirstRows = newBoxFirstRows;
		outerBoxFirstColumns = newBoxFirstColumns;
		outerBoxLastRows = newBoxLastRows;
		outerBoxLastColumns = newBoxLastColumns;
	}

	public void SetOuterTriLimits( int[] newTriFirstRows, int[] newTriFirstColumns, int[] newTriLastRows, int[] newTriLastColumns,
		int[] newTriCorners )
	{
		outerTriFirstRows = newTriFirstRows;
		outerTriFirstColumns = newTriFirstColumns;
		outerTriLastRows = newTriLastRows;
		outerTriLastColumns = newTriLastColumns;
		outerTriCorners = newTriCorners;
	}

	public void SetWaypointData( Vector2[] newWaypointOffsets, int[] newWaypointIDs )
	{
		int length = 0;
		Vector2 offset = Vector2.zero;

		waypointOffsetsX = null;
		waypointOffsetsY = null;
		waypointIDs = null;
		if( (newWaypointOffsets != null) && (newWaypointIDs != null) )
		{
			length = newWaypointOffsets.Length;
			if( newWaypointIDs.Length == length )
			{
				waypointOffsetsX = new float[length];
				waypointOffsetsY = new float[length];
				waypointIDs = new int[length];
				for( int i=0; i<length; i++ )
				{
					offset = newWaypointOffsets[i];
					waypointOffsetsX[i] = offset.x;
					waypointOffsetsY[i] = offset.y;
					waypointIDs[i] = newWaypointIDs[i];
				}
			}
		}
	}

	public void SetEnemyData( int[] newEnemyCodes, Vector2[] newEnemyOffsets )
	{
		int length = 0;
		Vector2 offset = Vector2.zero;

		ClearEnemyRoutes();
		enemyCodes = null;
		enemyOffsetsX = null;
		enemyOffsetsY = null;
		if( (newEnemyCodes != null) && (newEnemyOffsets != null) )
		{
			length = newEnemyCodes.Length;
			if( newEnemyOffsets.Length == length )
			{
				enemyCodes = new int[length];
				enemyOffsetsX = new float[length];
				enemyOffsetsY = new float[length];
				enemyRoutes = new RoutePersistentData[length];
				for( int i=0; i<length; i++ )
				{
					enemyCodes[i] = newEnemyCodes[i];
					offset = newEnemyOffsets[i];
					enemyOffsetsX[i] = offset.x;
					enemyOffsetsY[i] = offset.y;
					enemyRoutes[i] = null;
				}
			}
		}
	}

	public void SetEnemyRoute( int enemyIndex, RoutePersistentData newEnemyRoute )
	{
		RoutePersistentData oldEnemyRoute = null;

		if( enemyRoutes != null )
		{
			if( (enemyIndex > -1) && (enemyIndex < enemyRoutes.Length) )
			{
				oldEnemyRoute = enemyRoutes[enemyIndex];
				if( oldEnemyRoute != null )
				{
					oldEnemyRoute.Clear();
				}
				enemyRoutes[enemyIndex] = newEnemyRoute;
			}
		}
	}

	public RoutePersistentData[] GetEnemyRoutesCopy()
	{
		RoutePersistentData[] enemyRoutesCopy = null;
		RoutePersistentData originalEnemyRoute = null;
		RoutePersistentData newEnemyRoute = null;
		int length = 0;

		if( enemyRoutes != null )
		{
			length = enemyRoutes.Length;
			if( length > 0 )
			{
				enemyRoutesCopy = new RoutePersistentData[length];
				for( int i=0; i<length; i++ )
				{
					originalEnemyRoute = enemyRoutes[i];
					if( originalEnemyRoute != null )
					{
						newEnemyRoute = new RoutePersistentData();
						newEnemyRoute.SetAllWaypointIndexes( originalEnemyRoute.GetWaypointIndexesCopy() );
					}
					else
					{
						newEnemyRoute = null;
					}
					enemyRoutesCopy[i] = newEnemyRoute;
				}
			}
		}
		return enemyRoutesCopy;
	}

	public void PrintEnemyRoutes()
	{
		RoutePersistentData route = null;

		if( enemyRoutes != null )
		{
			for( int i=0; i<enemyRoutes.Length; i++ )
			{
				route = enemyRoutes[i];
				Debug.Log("Debug : StructurePersistentData : Route "+i+":");
				if( route != null )
				{
					Debug.Log("Debug : StructurePersistentData : "+UsefulFunctions.StringFromArray<int>(", ", route.GetWaypointIndexesCopy()));
				}
			}
		}
	}
}
