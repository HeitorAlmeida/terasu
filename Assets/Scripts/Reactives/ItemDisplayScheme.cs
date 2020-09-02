using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDisplaySchemeNew", menuName = "Item Display Scheme")]
public class ItemDisplayScheme : ScriptableObject
{
	public enum DisplayProperty { Name, Icon, Description, Tutorial }

	public DisplayProperty[] properties;
	public Vector2[] positionRates;
	public Vector2[] maxSizeRates;

	public bool ConfiguredProperly()
	{
		if( properties != null )
		{
			if( (positionRates != null) && (maxSizeRates != null) )
			{
				return ((properties.Length == positionRates.Length) && (positionRates.Length == maxSizeRates.Length));
			}
		}
		else
		{
			return ((positionRates == null) && (maxSizeRates == null)); 
		}
		return false;
	}
}
