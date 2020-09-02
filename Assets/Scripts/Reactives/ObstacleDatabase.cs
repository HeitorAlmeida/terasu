using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDatabase : MonoBehaviour
{
	public const int OBSTACLE_CODE_INVALID = -1;
	public const int SMALLEST_OBSTACLE_CODE = 0;
	public const int OBSTACLE_CODE_FRAIL_HORI = 0;
	public const int OBSTACLE_CODE_FRAIL_VERT = 1;
	public const int BIGGEST_OBSTACLE_CODE = 1;

	public GameObject modelFrailHorizontal;
	public GameObject modelFrailVertical;

	void Awake()
	{
		/*halmeida - do nothing.*/
	}

	public GameObject GetObstacleModel( int obstacleCode )
	{
		GameObject model = null;

		switch( obstacleCode )
		{
			case OBSTACLE_CODE_FRAIL_HORI:
				model = modelFrailHorizontal;
				break;
			case OBSTACLE_CODE_FRAIL_VERT:
				model = modelFrailVertical;
				break;
		}
		return model;
	}

	public string GetObstacleName( int obstacleCode )
	{
		string name = "Invalid obstacle name";

		switch( obstacleCode )
		{
			case OBSTACLE_CODE_FRAIL_HORI:
				name = "Frail Horizontal";
				break;
			case OBSTACLE_CODE_FRAIL_VERT:
				name = "Frail Vertical";
				break;
		}
		return name;
	}
}
