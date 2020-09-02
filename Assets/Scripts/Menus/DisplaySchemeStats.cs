using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DisplaySchemeStatsNew", menuName = "Stats Display Scheme")]
public class DisplaySchemeStats : ScriptableObject
{
	public bool displayLevel;
	public TextAlignment levelAnchorAlignment;
	public Vector2 levelPositionRates;
	public Vector2 levelScales;
	public TextAlignment attributeAnchorAlignment;
	public Vector2 attributeScales;
	public int attributeMaxDigits;
	public PlayerAgent.EquipAttribute[] attributes;
	public Vector2[] attributeNamePosRates;
	public Vector2[] attributeBasePosRates;
	public Vector2[] attributeEquipPosRates;
	public Vector2[] attributeTotalPosRates;

	public bool ConfiguredAllAspects()
	{
		int length = 0;

		if( attributes != null )
		{
			length = attributes.Length;
			if( (attributeNamePosRates != null) && (attributeBasePosRates != null) && (attributeEquipPosRates != null) &&
				(attributeTotalPosRates != null) )
			{
				return ( (attributeNamePosRates.Length == length) && (attributeBasePosRates.Length == length) &&
					(attributeEquipPosRates.Length == length) && (attributeTotalPosRates.Length == length) );
			}
		}
		else
		{
			return ( (attributeNamePosRates == null) && (attributeBasePosRates == null) && (attributeEquipPosRates == null) &&
				(attributeTotalPosRates == null) );
		}
		return false;
	}

	public bool ConfiguredAspect( MenuStatsDisplay.AttributeAspect aspect )
	{
		Vector2[] checkArray = null;

		switch( aspect )
		{
			case MenuStatsDisplay.AttributeAspect.Name:
				checkArray = attributeNamePosRates;
				break;
			case MenuStatsDisplay.AttributeAspect.Base:
				checkArray = attributeBasePosRates;
				break;
			case MenuStatsDisplay.AttributeAspect.Equip:
				checkArray = attributeEquipPosRates;
				break;
			case MenuStatsDisplay.AttributeAspect.Total:
				checkArray = attributeTotalPosRates;
				break;
		}
		if( attributes != null )
		{
			if( checkArray != null )
			{
				return (attributes.Length == checkArray.Length);
			}
		}
		else
		{
			return (checkArray == null);
		}
		return false;
	}
}
