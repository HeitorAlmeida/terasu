using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuStatsDisplay : MenuSimple
{
	public enum AttributeAspect { Name, Base, Equip, Total }

	public DisplaySchemeStats statsDisplay;
	public bool displayAllAspects;
	public AttributeAspect[] displayAspects;

	protected PlayerAgent player;
	protected GameObject levelObject;
	protected RectTransform levelTransform;
	protected SpritedStringUI levelText;
	protected bool statsDisplayValid;
	protected PlayerAgent.EquipAttribute[] stats;
	protected GameObject[][] statObjects;
	protected RectTransform[][] statTransforms;
	protected SpritedStringUI[][] statTexts;
	protected int[][] statValues;
	protected int percentageLimit;
	protected AttributeAspect[] aspects;


	protected override void Awake()
	{
		base.Awake();
		player = null;
		levelObject = null;
		levelTransform = null;
		levelText = null;
		statsDisplayValid = false;
		stats = null;
		statObjects = null;
		statTransforms = null;
		statTexts = null;
		statValues = null;
		percentageLimit = 1;
		aspects = null;
		if( statsDisplay != null )
		{
			for( int i=statsDisplay.attributeMaxDigits; i>0; i-- )
			{
				percentageLimit *= 10;
			}
			percentageLimit--;
			statsDisplayValid = EvaluateDisplaySchemeCompatibility();
			if( statsDisplayValid )
			{
				if( displayAllAspects )
				{
					aspects = (AttributeAspect[])Enum.GetValues(typeof(AttributeAspect));
				}
				else
				{
					aspects = displayAspects;
				}
			}
		}
	}

	private bool EvaluateDisplaySchemeCompatibility()
	{
		bool compatible = false;

		if( displayAllAspects )
		{
			return statsDisplay.ConfiguredAllAspects();
		}
		else if( displayAspects != null )
		{
			if( displayAspects.Length > 0 )
			{
				compatible = true;
				for( int i=0; i<displayAspects.Length; i++ )
				{
					if( !statsDisplay.ConfiguredAspect( displayAspects[i] ) )
					{
						compatible = false;
						break;
					}
				}
			}
		}
		return compatible;
	}

	public override void SetGameController( GameController newGameController, Canvas newInterfaceCanvas, GameObject newImageOverlayModel,
		EventManager newEventManager )
	{
		base.SetGameController( newGameController, newInterfaceCanvas, newImageOverlayModel, newEventManager );
		if( gameController != null )
		{
			player = gameController.GetAgentCharacter( -1 );
		}
	}

	protected override void BuildOptions()
	{
		base.BuildOptions();
		ExtractPlayer();
		CreateStatsDisplay();
	}

	protected void ExtractPlayer()
	{
		if( (player == null) && (gameController != null) )
		{
			player = gameController.GetAgentCharacter( -1 );
		}
	}

	protected virtual bool CreateStatsDisplay()
	{
		GameTextDatabase textDatabase = null;
		int totalStats = 0;
		string statName = null;
		string textName = null;
		string textValue = null;
		float statValueOriginal = 0f;
		float statValueBase = 0f;
		float statEquippedSum = 0f; 
		bool toDiscard = false;
		GameObject textObject = null;
		RectTransform textTransform = null;
		SpritedStringUI textComponent = null;
		Vector2 textDimensions = Vector2.zero;
		Vector2 textScales = Vector2.zero;
		Vector2 textPositionRates = Vector2.zero;
		TextAlignment textAnchor = TextAlignment.Center;

		textDatabase = GameTextDatabase.Instance;
		if( statsDisplayValid && (interfaceCanvasTrans != null) && (player != null) && (textDatabase != null) )
		{
			if( statsDisplay.displayLevel && (levelObject == null) )
			{
				statName = textDatabase.GetSystemText( GameTextDatabase.TEXT_ID_PLAYER_LEVEL, ref toDiscard );
				statName += " "+player.GetCurrentLevel();
				textScales = statsDisplay.levelScales;
				textPositionRates = statsDisplay.levelPositionRates;
				textAnchor = statsDisplay.levelAnchorAlignment;
				CreateTextDisplayObject( "LevelDisplay", statName, textScales, textPositionRates, textAnchor, ref levelObject,
					ref levelTransform, ref levelText ); 
			}
			if( statObjects == null )
			{
				stats = statsDisplay.attributes;
				if( stats != null )
				{
					totalStats = stats.Length;
					if( totalStats > 0 )
					{
						statObjects = new GameObject[totalStats][];
						statTransforms = new RectTransform[totalStats][];
						statTexts = new SpritedStringUI[totalStats][];
						statValues = new int[totalStats][];
						GameObject[] newStatObjects = null;
						RectTransform[] newStatTransforms = null;
						SpritedStringUI[] newStatTexts = null;
						int[] newStatValues = null;
						int statValue = -1;
						int percentageBase = 0;
						int percentageBaseLimited = 0;
						int percentageEquip = 0;
						int percentageEquipLimited = 0;
						int percentageTotal = 0;
						int percentageTotalLimited = 0;
						bool percentageBaseReady = false;
						bool percentageEquipReady = false;
						textAnchor = statsDisplay.attributeAnchorAlignment;
						textScales = statsDisplay.attributeScales;
						for( int i=0; i<stats.Length; i++ )
						{
							newStatObjects = new GameObject[aspects.Length];
							newStatTransforms = new RectTransform[aspects.Length];
							newStatTexts = new SpritedStringUI[aspects.Length];
							newStatValues = new int[aspects.Length];
							statName = textDatabase.GetEquipAttributeName( stats[i] );
							player.GetStatValueSegments( stats[i], ref statValueOriginal, ref statValueBase, ref statEquippedSum );
							percentageBaseReady = false;
							percentageEquipReady = false;
							for( int j=0; j<aspects.Length; j++ )
							{
								switch( aspects[j] )
								{
									case AttributeAspect.Name:
										textName = "StatName"+statName;
										textValue = statName;
										textPositionRates = statsDisplay.attributeNamePosRates[i];
										statValue = -1;
										break;
									case AttributeAspect.Base:
										textName = "StatBase"+statName;
										if( !percentageBaseReady )
										{
											percentageBase = UsefulFunctions.GetPercentage( statValueBase, statValueOriginal );
											statValue = percentageBase;
											percentageBaseLimited = (percentageBase > percentageLimit) ? percentageLimit : percentageBase;
											percentageBaseReady = true;
										}
										textValue = percentageBaseLimited.ToString();
										textPositionRates = statsDisplay.attributeBasePosRates[i];
										break;
									case AttributeAspect.Equip:
										textName = "StatEquip"+statName;
										if( !percentageEquipReady )
										{
											percentageEquip = UsefulFunctions.GetPercentage( statEquippedSum );
											statValue = percentageEquip;
											percentageEquipLimited = (percentageEquip > percentageLimit) ? percentageLimit : percentageEquip;
											percentageEquipReady = true;
										}
										textValue = percentageEquipLimited.ToString();
										textPositionRates = statsDisplay.attributeEquipPosRates[i];
										break;
									case AttributeAspect.Total:
										textName = "StatTotal"+statName;
										if( !percentageBaseReady )
										{
											percentageBase = UsefulFunctions.GetPercentage( statValueBase, statValueOriginal );
											percentageBaseLimited = (percentageBase > percentageLimit) ? percentageLimit : percentageBase;
											percentageBaseReady = true;
										}
										if( !percentageEquipReady )
										{
											percentageEquip = UsefulFunctions.GetPercentage( statEquippedSum );
											percentageEquipLimited = (percentageEquip > percentageLimit) ? percentageLimit : percentageEquip;
											percentageEquipReady = true;
										}
										percentageTotal = percentageBase + percentageEquip;
										statValue = percentageTotal;
										percentageTotalLimited = (percentageTotal > percentageLimit) ? percentageLimit : percentageTotal;
										textValue = percentageTotalLimited.ToString();
										textPositionRates = statsDisplay.attributeTotalPosRates[i];
										break;
								}
								CreateTextDisplayObject( textName, textValue, textScales, textPositionRates,
									textAnchor, ref textObject, ref textTransform, ref textComponent );
								newStatObjects[j] = textObject;
								newStatTransforms[j] = textTransform;
								newStatTexts[j] = textComponent;
								newStatValues[j] = statValue;
							}
							statObjects[i] = newStatObjects;
							statTransforms[i] = newStatTransforms;
							statTexts[i] = newStatTexts;
							statValues[i] = newStatValues;
						}
					}
				}
			}
			return true;
		}
		return false;
	}

	protected void CreateTextDisplayObject( string objectName, string objectContent, Vector2 scaleChange, Vector2 positionRates,
		TextAlignment anchorAlign, ref GameObject newObject, ref RectTransform newTransform, ref SpritedStringUI newText )
	{
		Vector2 originalUIDimensions = Vector2.zero;

		if( font != null )
		{
			newObject = new GameObject(objectName, typeof(RectTransform));
			newTransform = newObject.GetComponent<RectTransform>();
			newText = newObject.AddComponent<SpritedStringUI>();
			newText.SetSymbolSource( font );
			newText.SetValue( objectContent );
			newText.ToggleRaycastTargeting( false );
			originalUIDimensions = newText.GetUIDimensions();
			PlaceTransformProperly( newTransform, originalUIDimensions, scaleChange, positionRates, anchorAlign );
		}
	}

	protected void PlaceTransformProperly( RectTransform rectTrans, Vector2 originalUIDimensions, Vector2 scaleChange, Vector2 positionRates,
		TextAlignment anchorAlign )
	{
		Vector2 uiPosition = Vector2.zero;

		if( (rectTrans != null) && (interfaceCanvasTrans != null) )
		{
			uiPosition.x = (positionRates.x - 0.5f) * interfaceCanvasTrans.rect.width;
			uiPosition.y = (positionRates.y - 0.5f) * interfaceCanvasTrans.rect.height;
			if( rectTrans.parent != interfaceCanvasTrans )
			{
				rectTrans.SetParent( interfaceCanvasTrans, false );
			}
			rectTrans.localScale = new Vector3( scaleChange.x, scaleChange.y, 1f );
			originalUIDimensions.x *= scaleChange.x;
			originalUIDimensions.y *= scaleChange.y;
			if( anchorAlign == TextAlignment.Left )
			{
				uiPosition.x += originalUIDimensions.x / 2f;
			}
			else if( anchorAlign == TextAlignment.Right )
			{
				uiPosition.x -= originalUIDimensions.x / 2f;
			}
			rectTrans.anchoredPosition = uiPosition;
		}
	}

	protected virtual bool UpdateStatsDisplay()
	{
		GameTextDatabase textDatabase = null;
		string textValue = null;
		float statValueOriginal = 0f;
		float statValueBase = 0f;
		float statEquippedSum = 0f; 
		bool toDiscard = false;
		Vector2 textDimensions = Vector2.zero;
		Vector2 textScales = Vector2.zero;
		Vector2 textPositionRates = Vector2.zero;
		TextAlignment textAnchor = TextAlignment.Center;
		bool skipUpdate = false;

		textDatabase = GameTextDatabase.Instance;
		if( statsDisplayValid && (interfaceCanvasTrans != null) && (player != null) && (textDatabase != null) )
		{
			if( levelObject != null )
			{
				textValue = textDatabase.GetSystemText( GameTextDatabase.TEXT_ID_PLAYER_LEVEL, ref toDiscard );
				textValue += " "+player.GetCurrentLevel();
				textScales = statsDisplay.levelScales;
				textPositionRates = statsDisplay.levelPositionRates;
				textAnchor = statsDisplay.levelAnchorAlignment;
				UpdateTextDisplayObject( textValue, textScales, textPositionRates, textAnchor, levelTransform, levelText );
			}
			if( statObjects != null )
			{
				textAnchor = statsDisplay.attributeAnchorAlignment;
				textScales = statsDisplay.attributeScales;
				RectTransform[] oneStatTransforms = null;
				SpritedStringUI[] oneStatTexts = null;
				int[] oneStatValues = null;
				int percentageBase = 0;
				int percentageBaseLimited = 0;
				int percentageEquip = 0;
				int percentageEquipLimited = 0;
				int percentageTotal = 0;
				int percentageTotalLimited = 0;
				bool percentageBaseReady = false;
				bool percentageEquipReady = false;
				int statValue = -1;
				for( int i=0; i<stats.Length; i++ )
				{
					oneStatTransforms = statTransforms[i];
					oneStatTexts = statTexts[i];
					oneStatValues = statValues[i];
					player.GetStatValueSegments( stats[i], ref statValueOriginal, ref statValueBase, ref statEquippedSum );
					percentageBaseReady = false;
					percentageEquipReady = false;
					for( int j=0; j<aspects.Length; j++ )
					{
						skipUpdate = false;
						switch( aspects[j] )
						{
							case AttributeAspect.Name:
								skipUpdate = true;
								break;
							case AttributeAspect.Base:
								if( !percentageBaseReady )
								{
									percentageBase = UsefulFunctions.GetPercentage( statValueBase, statValueOriginal );
									statValue = percentageBase;
									percentageBaseLimited = (percentageBase > percentageLimit) ? percentageLimit : percentageBase;
									percentageBaseReady = true;
								}
								textValue = percentageBaseLimited.ToString();
								textPositionRates = statsDisplay.attributeBasePosRates[i];
								break;
							case AttributeAspect.Equip:
								if( !percentageEquipReady )
								{
									percentageEquip = UsefulFunctions.GetPercentage( statEquippedSum );
									statValue = percentageEquip;
									percentageEquipLimited = (percentageEquip > percentageLimit) ? percentageLimit : percentageEquip;
									percentageEquipReady = true;
								}
								textValue = percentageEquipLimited.ToString();
								textPositionRates = statsDisplay.attributeEquipPosRates[i];
								break;
							case AttributeAspect.Total:
								if( !percentageBaseReady )
								{
									percentageBase = UsefulFunctions.GetPercentage( statValueBase, statValueOriginal );
									percentageBaseLimited = (percentageBase > percentageLimit) ? percentageLimit : percentageBase;
									percentageBaseReady = true;
								}
								if( !percentageEquipReady )
								{
									percentageEquip = UsefulFunctions.GetPercentage( statEquippedSum );
									percentageEquipLimited = (percentageEquip > percentageLimit) ? percentageLimit : percentageEquip;
									percentageEquipReady = true;
								}
								percentageTotal = percentageBase + percentageEquip;
								statValue = percentageTotal;
								percentageTotalLimited = (percentageTotal > percentageLimit) ? percentageLimit : percentageTotal;
								textValue = percentageTotalLimited.ToString();
								textPositionRates = statsDisplay.attributeTotalPosRates[i];
								break;
						}
						if( !skipUpdate )
						{
							UpdateTextDisplayObject( textValue, textScales, textPositionRates, textAnchor, oneStatTransforms[j],
								oneStatTexts[j] );
							oneStatValues[j] = statValue;
						}
					}
				}
			}
			return true;
		}
		return false;
	}

	protected void UpdateTextDisplayObject( string objectContent, Vector2 scaleChange, Vector2 positionRates, TextAlignment anchorAlign,
		RectTransform readyTransform, SpritedStringUI readyText )
	{
		Vector2 originalUIDimensions = Vector2.zero;

		if( readyText != null )
		{
			readyText.SetValue( objectContent );
			originalUIDimensions = readyText.GetUIDimensions();
			PlaceTransformProperly( readyTransform, originalUIDimensions, scaleChange, positionRates, anchorAlign );
		}
	}

	protected override void ClearOptions()
	{
		ClearStatsDisplay();
		base.ClearOptions();
	}

	protected virtual void ClearStatsDisplay()
	{
		SpritedStringUI textComponent = null;
		SpritedStringUI[] textComponents = null;
		RectTransform[] textTransforms = null;
		GameObject textObject = null;
		GameObject[] textObjects = null;

		if( levelObject != null )
		{
			levelText.Clear();
			levelText = null;
			Destroy( levelObject );
			levelObject = null;
		}
		if( stats != null )
		{
			for( int i=0; i<stats.Length; i++ )
			{
				statValues[i] = null;
				textComponents = statTexts[i];
				if( textComponents != null )
				{
					for( int j=0; j<textComponents.Length; j++ )
					{
						textComponent = textComponents[j];
						if( textComponent != null )
						{
							textComponent.Clear();
							textComponents[j] = null;
						}
					}
					statTexts[i] = null;
				}
				textTransforms = statTransforms[i];
				if( textTransforms != null )
				{
					for( int j=0; j<textTransforms.Length; j++ )
					{
						textTransforms[j] = null;
					}
					statTransforms[i] = null;
				}
				textObjects = statObjects[i];
				if( textObjects != null )
				{
					for( int j=0; j<textObjects.Length; j++ )
					{
						textObject = textObjects[j];
						if( textObject != null )
						{
							Destroy( textObject );
							textObjects[j] = null;
						}
					}
					statObjects[i] = null;
				}
			}
			statValues = null;
			statTexts = null;
			statTransforms = null;
			statObjects = null;
			stats = null;
		}
	}

	public override void Clear()
	{
		player = null;
		aspects = null;
		statsDisplayValid = false;
		base.Clear();
	}
}
