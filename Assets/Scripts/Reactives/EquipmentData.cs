using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentDataNew", menuName = "Equipment Data")]
public class EquipmentData : ScriptableObject
{
	public PlayerAgent.EquipAttribute[] attributes;
	public int[] attributePercentages;
	public PlayerAgent.EquipBodyPart bodyPart;
}
