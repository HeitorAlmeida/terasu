using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAppearanceDatabase : MonoBehaviour
{
	public enum BoxPurpose { System, Dialogue, Transaction, UI }
	
	public Sprite[] boxBodiesSystem;
	public Sprite[] boxBodiesDialogue;
	public Sprite[] boxTailsDialogue;
	public Sprite[] boxBodiesTransaction;
	public Sprite[] boxTailsTransaction;
	public Sprite[] boxBodiesUI;

	void Awake()
	{
	}

	public bool GetBoxAppearance( BoxPurpose boxPurpose, int variation, ref Sprite boxBody, ref Sprite boxTail )
	{
		Sprite[] bodySource = null;
		Sprite[] tailSource = null;

		boxBody = null;
		boxTail = null;
		switch( boxPurpose )
		{
			case BoxPurpose.System:
				bodySource = boxBodiesSystem;
				break;
			case BoxPurpose.Dialogue:
				bodySource = boxBodiesDialogue;
				tailSource = boxTailsDialogue;
				break;
			case BoxPurpose.Transaction:
				bodySource = boxBodiesTransaction;
				tailSource = boxTailsTransaction;
				break;
			case BoxPurpose.UI:
				bodySource = boxBodiesUI;
				break;
		}
		if( bodySource != null )
		{
			if( (variation > -1) && (variation < bodySource.Length) )
			{
				boxBody = bodySource[variation];
				if( tailSource != null )
				{
					if( variation < tailSource.Length )
					{
						boxTail = tailSource[variation];
					}
				}
				return true;
			}
		}
		return false;
	}
}
