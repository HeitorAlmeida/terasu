using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //to create classes with [Serializable]

[Serializable]
public class PlayerPersistentData
{
	public int playerCoins;
	public bool playerSkillShoot;
	public int playerSkillBreak;
	public int playerExperiencePoints;
	public int[] playerInventoryItemIDs;
	public int[] playerInventoryItemUnits;
	public int[] playerEquippedItemIDs;
	public string playerCurrentName;
	public float playerOffsetXInNode;
	public float playerOffsetYInNode;
	public bool playerFacingSide;

	public PlayerPersistentData()
	{
		Clear();
	}
	
	public void Clear()
	{
		playerCoins = 0;
		playerSkillShoot = false;
		playerSkillBreak = -1;
		playerExperiencePoints = 0;
		playerInventoryItemIDs = null;
		playerInventoryItemUnits = null;
		playerEquippedItemIDs = null;
		playerCurrentName = null;
		playerOffsetXInNode = 0f;
		playerOffsetYInNode = 0f;
		playerFacingSide = true;
	}
}
