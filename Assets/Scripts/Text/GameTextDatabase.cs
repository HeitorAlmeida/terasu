using System.Xml.Serialization;
using System.IO;  //to use StringReader
using System;  //to use Exception
using UnityEngine;  //to use TextAsset

public class GameTextDatabase
{
	public enum TextType { Platform, System, Dialogue, Item }

	public enum TextPlatform { Android, PC }

	/*halmeida - these IDs are used for extraction of information from the text file. They should not be
	changed. If they are changed, they also have to be changed within the text file, so as to not break
	the communication.*/
	public const int INVALID_TEXT_ID = -1;
	public const int TEXT_ID_YES = 0;
	public const int TEXT_ID_NO = 1;
	public const int TEXT_ID_SAVE_ENUNCIATE = 2;
	public const int TEXT_ID_SAVE_OVERWRITE_ENUNCIATE = 3;
	public const int TEXT_ID_SAVE_CONFIRMATION = 4;
	public const int TEXT_ID_SAVE_CANCELATION = 5;
	public const int TEXT_ID_SAVE_FAILURE = 6;
	public const int TEXT_ID_TRANSACTION_INVEST_TITLE = 7;
	public const int TEXT_ID_TRANSACTION_INVEST_UNIT = 8;
	public const int TEXT_ID_TRANSACTION_DRAW_TITLE = 9;
	public const int TEXT_ID_TRANSACTION_DRAW_UNIT = 10;
	public const int TEXT_ID_PLAYER_LEVEL = 11;
	public const int TEXT_ID_EQUIP_ATTR_ACCEL = 12;
	public const int TEXT_ID_EQUIP_ATTR_MAXSPEED = 13;
	public const int TEXT_ID_EQUIP_ATTR_MAXHP = 14;
	public const int TEXT_ID_EQUIP_ATTR_MAXMP = 15;
	public const int TEXT_ID_EQUIP_ATTR_ATTACK = 16;
	public const int TEXT_ID_EQUIP_ATTR_MAXFREQUENCY = 17;
	public const int TEXT_ID_EQUIP_PART_HEAD = 18;
	public const int TEXT_ID_EQUIP_PART_NECK = 19;
	public const int TEXT_ID_EQUIP_PART_TORSO = 20;
	public const int TEXT_ID_EQUIP_PART_ARMS = 21;
	public const int TEXT_ID_EQUIP_PART_LEGS = 22;
	public const int TEXT_ID_NON_EQUIPPABLE_ITEMS = 23;
	public const int TEXT_ID_MENU_OPTION_PAUSE = 24;
	public const int TEXT_ID_MENU_OPTION_UNPAUSE = 25;
	public const int TEXT_ID_MENU_OPTION_CLOSE_MENU = 26;
	public const int TEXT_ID_MENU_OPTION_OPEN_MAP = 27;
	public const int TEXT_ID_MENU_OPTION_OPEN_INVENTORY = 28;
	public const int TEXT_ID_MENU_OPTION_QUIT_GAME = 29;
	public const int TEXT_ID_MENU_OPTION_EQUIP = 30;
	public const int TEXT_ID_MENU_OPTION_UNEQUIP = 31;
	public const int TEXT_ID_MENU_OPTION_USE = 32;

	private const int INVALID_PLATFORM_ID = -1;
	private const int TEXT_PLATFORM_ID_ANDROID = 0;
	private const int TEXT_PLATFORM_ID_PC = 1;

	private static GameTextDatabase instance;

	public static GameTextDatabase Instance
	{
		get
		{
			if( instance == null )
			{
				instance = new GameTextDatabase();
			}
			return instance;
		}
	}

	private GameText gameText;

	private GameTextDatabase()
	{
		gameText = null;
	}

	public bool LoadFromResource( string resourcePath )
	{
		TextAsset textAsset = null;
		string textContent = null;

		if( gameText != null )
		{
			gameText.Clear();
			gameText = null;
		}
		if( !String.IsNullOrEmpty( resourcePath ) )
		{
			textAsset = Resources.Load<TextAsset>( resourcePath );
			if( textAsset != null )
			{
				textContent = textAsset.text;
				LoadFromXmlString( textContent );
			}
			Resources.UnloadAsset( textAsset );
		}
		return (gameText != null);
	}

	private void LoadFromXmlString( string xmlFileText )
	{
		XmlSerializer serializer = null;
		StringReader reader = null;

		if( !String.IsNullOrEmpty( xmlFileText ) )
		{
			try
			{
				serializer = new XmlSerializer(typeof(GameText));
				reader = new StringReader( xmlFileText );
				gameText = (GameText)serializer.Deserialize( reader );
			}
			catch( Exception e )
			{
				Debug.Log("Debug : GameTextDatabase : load from xml file text failed. Exception message \""+e.Message+"\".");
				if( gameText != null )
				{
					gameText.Clear();
					gameText = null;
				}
			}
		}
	}

	public string GetPlatformText( int textID, TextPlatform platform, ref bool autoClose )
	{
		PlatformText[] psourceSet = null;
		PlatformText psource = null;
		int platformId = INVALID_PLATFORM_ID;

		if( gameText != null )
		{
			psourceSet = gameText.Platform;
			if( psourceSet != null )
			{
				switch( platform )
				{
					case TextPlatform.Android:
						platformId = TEXT_PLATFORM_ID_ANDROID;
						break;
					case TextPlatform.PC:
						platformId = TEXT_PLATFORM_ID_PC;
						break;
				}
				if( platformId != INVALID_PLATFORM_ID )
				{
					for( int i=0; i<psourceSet.Length; i++ )
					{
						psource = psourceSet[i];
						if( psource != null )
						{
							if( (psource.Id == textID) && (psource.PlatformId == platformId) )
							{
								autoClose = psource.AutoClose;
								return psource.Text;
							}
						}
					}
				}
			}
		}
		return null;
	}

	public string GetSystemText( int textID, ref bool autoClose )
	{
		SystemText[] ssourceSet = null;
		SystemText ssource = null;

		if( gameText != null )
		{
			ssourceSet = gameText.System;
			if( ssourceSet != null )
			{
				for( int i=0; i<ssourceSet.Length; i++ )
				{
					ssource = ssourceSet[i];
					if( ssource != null )
					{
						if( ssource.Id == textID )
						{
							autoClose = ssource.AutoClose;
							return ssource.Text;
						}
					}
				}
			}
		}
		return null;
	}

	public string GetDialogueText( int textID, ref string speakerName, ref bool tryAbove, ref bool autoClose )
	{
		DialogueText[] dsourceSet = null;
		DialogueText dsource = null;

		if( gameText != null )
		{
			dsourceSet = gameText.Dialogue;
			if( dsourceSet != null )
			{
				for( int i=0; i<dsourceSet.Length; i++ )
				{
					dsource = dsourceSet[i];
					if( dsource != null )
					{
						if( dsource.Id == textID )
						{
							speakerName = dsource.SpeakerName;
							tryAbove = dsource.TryAbove;
							autoClose = dsource.AutoClose;
							return dsource.Text;
						}
					}
				}
			}
		}
		return null;
	}

	public string GetItemDescription( int itemID, ref string itemName )
	{
		ItemText[] isourceSet = null;
		ItemText isource = null;

		if( gameText != null )
		{
			isourceSet = gameText.Items;
			if( isourceSet != null )
			{
				for( int i=0; i<isourceSet.Length; i++ )
				{
					isource = isourceSet[i];
					if( isource != null )
					{
						if( isource.Id == itemID )
						{
							itemName = isource.Name;
							return isource.Description;
						}
					}
				}
			}
		}
		return null;
	}

	public string GetEquipAttributeName( PlayerAgent.EquipAttribute attribute )
	{
		SystemText[] ssourceSet = null;
		SystemText ssource = null;
		int textID = INVALID_TEXT_ID;

		if( gameText != null )
		{
			ssourceSet = gameText.System;
			if( ssourceSet != null )
			{
				switch( attribute )
				{
					case PlayerAgent.EquipAttribute.Accel:
						textID = TEXT_ID_EQUIP_ATTR_ACCEL;
						break;
					case PlayerAgent.EquipAttribute.MaxSpeed:
						textID = TEXT_ID_EQUIP_ATTR_MAXSPEED;
						break;
					case PlayerAgent.EquipAttribute.MaxHP:
						textID = TEXT_ID_EQUIP_ATTR_MAXHP;
						break;
					case PlayerAgent.EquipAttribute.MaxMP:
						textID = TEXT_ID_EQUIP_ATTR_MAXMP;
						break;
					case PlayerAgent.EquipAttribute.Attack:
						textID = TEXT_ID_EQUIP_ATTR_ATTACK;
						break;
					case PlayerAgent.EquipAttribute.MaxFrequency:
						textID = TEXT_ID_EQUIP_ATTR_MAXFREQUENCY;
						break;
				}
				if( textID != INVALID_TEXT_ID )
				{
					for( int i=0; i<ssourceSet.Length; i++ )
					{
						ssource = ssourceSet[i];
						if( ssource != null )
						{
							if( ssource.Id == textID )
							{
								return ssource.Text;
							}
						}
					}
				}
			}
		}
		return null;
	}

	public string GetEquipBodyPartName( PlayerAgent.EquipBodyPart bodyPart )
	{
		SystemText[] ssourceSet = null;
		SystemText ssource = null;
		int textID = INVALID_TEXT_ID;

		if( gameText != null )
		{
			ssourceSet = gameText.System;
			if( ssourceSet != null )
			{
				switch( bodyPart )
				{
					case PlayerAgent.EquipBodyPart.Head:
						textID = TEXT_ID_EQUIP_PART_HEAD;
						break;
					case PlayerAgent.EquipBodyPart.Neck:
						textID = TEXT_ID_EQUIP_PART_NECK;
						break;
					case PlayerAgent.EquipBodyPart.Torso:
						textID = TEXT_ID_EQUIP_PART_TORSO;
						break;
					case PlayerAgent.EquipBodyPart.Arms:
						textID = TEXT_ID_EQUIP_PART_ARMS;
						break;
					case PlayerAgent.EquipBodyPart.Legs:
						textID = TEXT_ID_EQUIP_PART_LEGS;
						break;
				}
				if( textID != INVALID_TEXT_ID )
				{
					for( int i=0; i<ssourceSet.Length; i++ )
					{
						ssource = ssourceSet[i];
						if( ssource != null )
						{
							if( ssource.Id == textID )
							{
								return ssource.Text;
							}
						}
					}
				}
			}
		}
		return null;
	}

	public string GetMenuOptionText( MenuSimple.ChoiceEffect optionEffect )
	{
		SystemText[] ssourceSet = null;
		SystemText ssource = null;
		int textID = INVALID_TEXT_ID;

		if( gameText != null )
		{
			ssourceSet = gameText.System;
			if( ssourceSet != null )
			{
				switch( optionEffect )
				{
					case MenuSimple.ChoiceEffect.Pause:
						textID = TEXT_ID_MENU_OPTION_PAUSE;
						break;
					case MenuSimple.ChoiceEffect.Unpause:
						textID = TEXT_ID_MENU_OPTION_UNPAUSE;
						break;
					case MenuSimple.ChoiceEffect.CloseMenu:
						textID = TEXT_ID_MENU_OPTION_CLOSE_MENU;
						break;
					case MenuSimple.ChoiceEffect.OpenMap:
						textID = TEXT_ID_MENU_OPTION_OPEN_MAP;
						break;
					case MenuSimple.ChoiceEffect.OpenInventory:
						textID = TEXT_ID_MENU_OPTION_OPEN_INVENTORY;
						break;
					case MenuSimple.ChoiceEffect.QuitGame:
						textID = TEXT_ID_MENU_OPTION_QUIT_GAME;
						break;
					case MenuSimple.ChoiceEffect.Equip:
						textID = TEXT_ID_MENU_OPTION_EQUIP;
						break;
					case MenuSimple.ChoiceEffect.Unequip:
						textID = TEXT_ID_MENU_OPTION_UNEQUIP;
						break;
					case MenuSimple.ChoiceEffect.Use:
						textID = TEXT_ID_MENU_OPTION_USE;
						break;
				}
				if( textID != INVALID_TEXT_ID )
				{
					for( int i=0; i<ssourceSet.Length; i++ )
					{
						ssource = ssourceSet[i];
						if( ssource != null )
						{
							if( ssource.Id == textID )
							{
								return ssource.Text;
							}
						}
					}
				}
			}
		}
		return null;
	}
}
