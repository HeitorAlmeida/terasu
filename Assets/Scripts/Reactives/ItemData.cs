using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataNew", menuName = "Item Data")]
public class ItemData : ScriptableObject
{
	public const int INVALID_ITEM_ID = -1;

	public enum EffectType { Money, SkillUnlock, HP, MP, MaxHP, MaxMP }

	public int itemID;
	public Sprite itemIcon;
	public bool obtainable;
	public bool inventoryKept;
	public bool expendable;
	public EffectType effectType;
	public float effectValue;
	public EquipmentData equipment;
}
