using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDatabase : MonoBehaviour
{
	public const int ENEMY_CODE_INVALID = -1;
	public const int SMALLEST_ENEMY_CODE = 0;
	public const int ENEMY_CODE_QUIET = 0;
	public const int ENEMY_CODE_CRAWLER = 1;
	public const int BIGGEST_ENEMY_CODE = 1;

	public GameObject modelQuiet;
	public GameObject modelCrawler;

	void Awake()
	{
		/*halmeida - do nothing.*/
	}

	public GameObject GetEnemyModel( int enemyCode )
	{
		GameObject model = null;

		switch( enemyCode )
		{
			case ENEMY_CODE_QUIET:
				model = modelQuiet;
				break;
			case ENEMY_CODE_CRAWLER:
				model = modelCrawler;
				break;
		}
		return model;
	}

	public string GetEnemyName( int enemyCode )
	{
		string name = "Invalid enemy name";

		switch( enemyCode )
		{
			case ENEMY_CODE_QUIET:
				name = "Quiet Shadow";
				break;
			case ENEMY_CODE_CRAWLER:
				name = "Crawler Shadow";
				break;
		}
		return name;
	}
}
