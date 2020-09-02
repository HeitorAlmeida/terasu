using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBoxManager
{
	public const float SYSTEM_TEXT_TOP = 0.8f;
	public const float ENUNCIATE_TEXT_TOP = 0.9f;
	public const float BOX_OPENING_SPEED = 3f;

	public enum QuestionRole { None, Enunciate, Option }

	/*halmeida - implementation of the Singleton design pattern with a property and a backing
	field for it. The backing field is private and static to hide a single instance of the
	data within the class. The property just defines accessors for that single instance.*/ 
	private static TextBoxManager instance;

	public static TextBoxManager Instance
	{
		get
		{
			if( instance == null )
			{
				instance = new TextBoxManager();
			}
			return instance;
		}
	}

	private GameObject textBoxParent;
	private GameObject[] boxObjects;
	private TextBox[] boxComponents;
	private EventDialogue[] boxMasterEvents;
	private QuestionRole[] boxQuestionRoles;
	private int[] boxOptionIndexes;
	private int[] boxSpeakerIDs;
	private bool[] boxTryAboves;
	private GameObject[] boxTailObjects;
	private SpriteRenderer[] boxTailRenderers;
	private SymbolDatabase symbolDatabase;
	private BoxAppearanceDatabase boxDatabase;
	private int boxAppearanceIndex;
	private Sprite bodySpriteSystem;
	private Sprite bodySpriteDialogue;
	private Sprite tailSpriteDialogue;
	private float tailWidth;
	private float tailHeight;
	private float tailHalfWidth;
	private float tailHalfHeight;
	private ChallengeStage stage;
	private GameObject cameraObject;
	private CameraController cameraController;
	private float cameraWidth;
	private float cameraHeight;
	private float cameraHalfWidth;
	private float cameraHalfHeight;
	private int cameraPixelWidth;
	private int cameraPixelHeight;
	private float boxDepth;
	private Vector2 systemBoxLimits;
	private float systemBoxTopOffset;
	private Vector2 dialogueBoxLimits;
	private IOptionsListener questionDelegate;
	private float enunciateBoxTopOffset;
	private TextBox enunciateBoxComponent;
	private int optionsToAllocate;
	private float optionAreaHeight;
	private float optionHalfAreaHeight;
	private float optionBoxTopOffset;
	private int optionAwaitedIndex;
	private int optionChosenIndex;
	private int optionIndexConfirm;
	private int optionIndexCancel;
	private TextBox optionComponentConfirm;
	private TextBox optionComponentCancel;
	private bool paused;

	private TextBoxManager()
	{
		textBoxParent = null;
		boxObjects = null;
		boxComponents = null;
		boxMasterEvents = null;
		boxQuestionRoles = null;
		boxOptionIndexes = null;
		boxSpeakerIDs = null;
		boxTryAboves = null;
		boxTailObjects = null;
		boxTailRenderers = null;
		symbolDatabase = null;
		boxDatabase = null;
		boxAppearanceIndex = -1;
		bodySpriteSystem = null;
		bodySpriteDialogue = null;
		tailSpriteDialogue = null;
		tailWidth = 0f;
		tailHeight = 0f;
		tailHalfWidth = 0f;
		tailHalfHeight = 0f;
		stage = null;
		cameraObject = null;
		cameraController = null;
		cameraWidth = 0f;
		cameraHeight = 0f;
		cameraHalfWidth = 0f;
		cameraHalfHeight = 0f;
		cameraPixelWidth = 0;
		cameraPixelHeight = 0;
		boxDepth = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_TEXT );
		systemBoxLimits = new Vector2( TextBox.DEFAULT_MAX_BOX_WIDTH, TextBox.DEFAULT_MAX_BOX_HEIGHT );
		systemBoxTopOffset = 0f;
		dialogueBoxLimits = new Vector2( DialogueBox.DEFAULT_MAX_D_BOX_WIDTH, DialogueBox.DEFAULT_MAX_D_BOX_HEIGHT );
		questionDelegate = null;
		enunciateBoxTopOffset = 0f;
		enunciateBoxComponent = null;
		optionsToAllocate = 0;
		optionAreaHeight = 0f;
		optionHalfAreaHeight = 0f;
		optionBoxTopOffset = 0f;
		optionAwaitedIndex = -1;
		optionChosenIndex = -1;
		optionIndexConfirm = -1;
		optionIndexCancel = -1;
		optionComponentConfirm = null;
		optionComponentCancel = null;
		paused = false;

		textBoxParent = new GameObject("TextBoxParent");
		textBoxParent.transform.position = new Vector3( 0f, 0f, boxDepth );
	}

	public void SetMaterials( SymbolDatabase newSymbolDatabase, BoxAppearanceDatabase newBoxDatabase )
	{
		Sprite bodySprite = null;
		Sprite tailSprite = null;

		symbolDatabase = newSymbolDatabase;
		boxDatabase = newBoxDatabase;
		if( boxDatabase != null )
		{
			if( !boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.System, 0, ref bodySprite, ref tailSprite ) )
			{
				boxAppearanceIndex = -1;
				bodySpriteSystem = null;
				bodySpriteDialogue = null;
				tailSpriteDialogue = null;
			}
			else
			{
				boxAppearanceIndex = 0;
				bodySpriteSystem = bodySprite;
				boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.Dialogue, 0, ref bodySprite, ref tailSprite );
				bodySpriteDialogue = bodySprite;
				tailSpriteDialogue = tailSprite;
			}
		}
		ExtractTailDimensions();
	}

	private void ExtractTailDimensions()
	{
		tailWidth = 0f;
		tailHeight = 0f;
		tailHalfWidth = 0f;
		tailHalfHeight = 0f;
		if( tailSpriteDialogue != null )
		{
			tailWidth = tailSpriteDialogue.bounds.size.x;
			tailHeight = tailSpriteDialogue.bounds.size.y;
			tailHalfWidth = tailWidth / 2f;
			tailHalfHeight = tailHeight / 2f;
		}
	}

	public void AdvanceBoxAppearance()
	{
		Sprite bodySprite = null;
		Sprite tailSprite = null;
		int newIndex = -1;
		int boxSpeakerID = BaseAgent.INVALID_AGENT_ID;
		TextBox boxComponent = null;
		SpriteRenderer tailRenderer = null;

		if( (boxDatabase != null) && (boxAppearanceIndex > -1) )
		{
			newIndex = boxAppearanceIndex++;
			if( !boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.System, newIndex, ref bodySprite, ref tailSprite ) )
			{
				newIndex = 0;
				boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.System, newIndex, ref bodySprite, ref tailSprite );
			}
			boxAppearanceIndex = newIndex;
			bodySpriteSystem = bodySprite;
			boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.Dialogue, newIndex, ref bodySprite, ref tailSprite );
			bodySpriteDialogue = bodySprite;
			tailSpriteDialogue = tailSprite;
			ExtractTailDimensions();
			if( boxComponents != null )
			{
				for( int i=0; i<boxComponents.Length; i++ )
				{
					boxComponent = boxComponents[i];
					if( boxComponent != null )
					{
						/*halmeida - relying on the coherence of all box related arrays.*/
						boxSpeakerID = boxSpeakerIDs[i];
						if( boxSpeakerID == BaseAgent.INVALID_AGENT_ID )
						{
							boxComponent.ChangeBoxBody( bodySpriteSystem );
						}
						else
						{
							boxComponent.ChangeBoxBody( bodySpriteDialogue );
						}
					}
				}
			}
			if( boxTailRenderers != null )
			{
				for( int i=0; i<boxTailRenderers.Length; i++ )
				{
					tailRenderer = boxTailRenderers[i];
					if( tailRenderer != null )
					{
						tailRenderer.sprite = tailSpriteDialogue;
					}
				}
			}
		}
	}

	public void SetReferences( ChallengeStage newStage, CameraController newCameraController )
	{
		stage = newStage;
		cameraController = newCameraController;
		if( cameraController != null )
		{
			if( cameraController.GetCameraOriginalWorldDimensions( ref cameraWidth, ref cameraHeight ) )
			{
				cameraHalfWidth = cameraWidth / 2f;
				cameraHalfHeight = cameraHeight / 2f;
				cameraController.GetCameraPixelDimensions( ref cameraPixelWidth, ref cameraPixelHeight );
				cameraObject = cameraController.gameObject;
				/*halmeida - the y of the system box offset will carry the y info that doesn't vary. In this case,
				it's the y of the top of the text box.*/
				systemBoxTopOffset = ( SYSTEM_TEXT_TOP - 0.5f ) * cameraHeight;
				enunciateBoxTopOffset = ( ENUNCIATE_TEXT_TOP - 0.5f ) * cameraHeight;
			}
		}
	}

	public void PrepareForQuestion( IOptionsListener newQuestionDelegate, int newOptionsToAllocate )
	{
		if( (newQuestionDelegate != null) && (newOptionsToAllocate > 0) )
		{
			questionDelegate = newQuestionDelegate;
			optionsToAllocate = newOptionsToAllocate;
		}
	}

	public bool ShowDialogue( string agentName, string dialogue, bool tryAbove, bool autoClose, EventDialogue masterEvent = null,
		QuestionRole questionRole = QuestionRole.None )
	{
		GameObject boxObject = null;
		TextBox boxComponent = null;
		int boxSpeakerID = BaseAgent.INVALID_AGENT_ID;
		Vector2 boxDimensions = Vector2.zero;
		Vector2 boxOffset = Vector2.zero;
		Vector3 boxPosition = Vector3.zero;
		bool boxPositionValid = false;
		BaseAgent agentComponent = null;
		GameObject boxTailObject = null;
		SpriteRenderer boxTailRenderer = null;
		Vector2 tailOffset = Vector2.zero;
		int optionCurrentIndex = -1;

		if( !QuestionRoleAvailable( questionRole ) )
		{
			return false;
		}
		else
		{
			if( questionRole != QuestionRole.None )
			{
				autoClose = false;
			}
		}
		if( (textBoxParent != null) && (symbolDatabase != null) && (dialogue != null) && (cameraObject != null) )
		{
			if( stage != null )
			{
				boxSpeakerID = stage.GetAgentID( agentName, ref agentComponent );
			}
			if( boxSpeakerID == BaseAgent.INVALID_AGENT_ID )
			{
				/*halmeida - this is not a character's dialogue text, so it is system info text.*/
				boxObject = new GameObject("SystemTextBox");
				boxComponent = boxObject.AddComponent<TextBox>();
				boxComponent.SetBoxLimits( systemBoxLimits.x, systemBoxLimits.y );
				boxComponent.SetEssentials( symbolDatabase, bodySpriteSystem, dialogue, 0f, TextBox.TextAlignment.AlignLeft, autoClose,
					BOX_OPENING_SPEED );
			}
			else
			{
				boxObject = new GameObject("DialogueTextBox");
				boxComponent = boxObject.AddComponent<DialogueBox>();
				boxComponent.SetBoxLimits( dialogueBoxLimits.x, dialogueBoxLimits.y );
				boxComponent.SetEssentials( symbolDatabase, bodySpriteDialogue, dialogue, TextBox.DEFAULT_TEXT_SPEED,
					TextBox.TextAlignment.AlignLeft, autoClose, BOX_OPENING_SPEED );
				if( agentComponent != null )
				{
					boxComponent.SetSpeakerName( agentComponent.GetCurrentName() );
					if( questionRole != QuestionRole.Option )
					{
						boxComponent.SetOrnamentPortrait( agentComponent.agentPortrait );
					}
				}
				if( questionRole == QuestionRole.None )
				{
					boxTailObject = new GameObject("DialogueBoxTail");
					boxTailRenderer = boxTailObject.AddComponent<SpriteRenderer>();
					boxTailRenderer.sprite = tailSpriteDialogue;
				}
			}
			boxComponent.Build();
			boxDimensions = boxComponent.GetBoxWorldDimensions();
			switch( questionRole )
			{
				case QuestionRole.None:
					boxPosition = GetBoxPositionForAgent( boxSpeakerID, boxDimensions, tryAbove, ref boxPositionValid, ref tailOffset );
					break;
				case QuestionRole.Enunciate:
					boxPosition = GetBoxPositionForEnunciate( boxDimensions, ref boxPositionValid );
					/*halmeida - with the dimensions of the enunciate set, I can already determine the area available
					for the options.*/
					optionAreaHeight = 2f * enunciateBoxTopOffset;
					optionAreaHeight *= ((enunciateBoxTopOffset < 0) ? -1f : 1f);
					optionAreaHeight -= boxDimensions.y;
					if( optionsToAllocate > 0 )
					{
						optionAreaHeight = optionAreaHeight / optionsToAllocate;
					}
					optionHalfAreaHeight = optionAreaHeight / 2f;
					optionBoxTopOffset = enunciateBoxTopOffset - boxDimensions.y;
					/*halmeida - enunciate text boxes cannot react to any input.*/
					boxComponent.BlockReactions();
					enunciateBoxComponent = boxComponent;
					optionAwaitedIndex = 0;
					break;
				case QuestionRole.Option:
					boxPosition = GetBoxPositionForOption( optionAwaitedIndex, boxDimensions, ref boxPositionValid );
					/*halmeida - option text boxes can only react to precise input over them.*/
					boxComponent.AllowReactions( true );
					optionCurrentIndex = optionAwaitedIndex;
					optionAwaitedIndex++;
					break;
			}
			if( boxPositionValid )
			{
				boxObject.transform.SetParent( textBoxParent.transform, false );
				/*halmeida - since the textBoxParent is at x = 0 and y = 0, I can use the local position of its
				children as if it was their position. Changing just the local position is faster.*/
				boxObject.transform.localPosition = boxPosition;
				if( boxTailObject != null )
				{
					boxTailObject.transform.SetParent( boxObject.transform, false );
					boxTailObject.transform.localPosition = new Vector3( tailOffset.x, tailOffset.y, boxComponent.GetBoxToTextDepth() );
					if( tailOffset.y > 0f )
					{
						boxTailObject.transform.rotation = Quaternion.Euler( 0f, 0f, 180f );
					}
				}
				boxComponent.Open();
				UsefulFunctions.IncreaseArray<GameObject>( ref boxObjects, boxObject );
				UsefulFunctions.IncreaseArray<TextBox>( ref boxComponents, boxComponent );
				UsefulFunctions.IncreaseArray<EventDialogue>( ref boxMasterEvents, masterEvent );
				UsefulFunctions.IncreaseArray<QuestionRole>( ref boxQuestionRoles, questionRole );
				UsefulFunctions.IncreaseArray<int>( ref boxOptionIndexes, optionCurrentIndex );
				UsefulFunctions.IncreaseArray<int>( ref boxSpeakerIDs, boxSpeakerID );
				UsefulFunctions.IncreaseArray<bool>( ref boxTryAboves, tryAbove );
				UsefulFunctions.IncreaseArray<GameObject>( ref boxTailObjects, boxTailObject );
				UsefulFunctions.IncreaseArray<SpriteRenderer>( ref boxTailRenderers, boxTailRenderer );
				return true;
			}
			boxComponent.Clear();
			GameObject.Destroy( boxObject );
			if( boxTailObject != null )
			{
				GameObject.Destroy( boxTailObject );
			}
		}
		return false;
	}

	private bool QuestionRoleAvailable( QuestionRole questionRole )
	{
		switch( questionRole )
		{
			case QuestionRole.None:
				return (questionDelegate == null);
			case QuestionRole.Enunciate:
				return ( (questionDelegate != null) && (enunciateBoxComponent == null) );
			case QuestionRole.Option:
				return ( (questionDelegate != null) && (enunciateBoxComponent != null) );
		}
		return false;
	}

	private Vector3 GetBoxPositionForAgent( int agentID, Vector2 boxDimensions, bool tryAbove, ref bool positionValid, ref Vector2 tailOffset )
	{
		Vector3 boxPosition = Vector3.zero;
		BaseAgent agent = null;
		Vector3 agentPosition = Vector3.zero;
		float agentHeight = 0f;
		float halfAgentHeight = 0f;
		bool fitsAbove = false;
		bool fitsBelow = false;
		Vector3 cameraPosition = Vector3.zero;
		float cameraTop = 0f;
		float cameraBottom = 0f;
		float cameraLeft = 0f;
		float cameraRight = 0f;
		float boxTop = 0f;
		float boxBottom = 0f;
		float halfBoxWidth = 0f;
		float halfBoxHeight = 0f;

		positionValid = false;
		if( cameraObject != null )
		{
			if( agentID == BaseAgent.INVALID_AGENT_ID )
			{
				/*halmeida - A box for system text display is always shown horizontally centered at the screen and with its
				top at a fixed, predetermined position.*/
				cameraPosition = cameraObject.transform.position;
				boxPosition.x = cameraPosition.x;
				boxPosition.y = cameraPosition.y + systemBoxTopOffset - boxDimensions.y / 2f;
				positionValid = true;
			}
			else
			{
				if( stage != null )
				{
					agent = stage.GetAgent( agentID );
					if( agent != null )
					{
						/*halmeida - gotta place the box at a position that relates to the agent's position, is within the
						camera, and tries to respect the above/bellow orientation.*/
						halfBoxHeight = boxDimensions.y / 2f;
						agentPosition = agent.gameObject.transform.position;
						agentHeight = agent.GetHeight();
						halfAgentHeight = agentHeight / 2f;
						cameraPosition = cameraObject.transform.position;
						cameraTop = cameraPosition.y + cameraHalfHeight;
						boxTop = agentPosition.y + halfAgentHeight + tailHeight + boxDimensions.y;
						fitsAbove = !( boxTop > cameraTop );
						cameraBottom = cameraPosition.y - cameraHalfHeight;
						boxBottom = agentPosition.y - halfAgentHeight - tailHeight - boxDimensions.y;
						fitsBelow = !( boxBottom < cameraBottom );
						if( fitsAbove && fitsBelow )
						{
							if( tryAbove )
							{
								boxPosition.y = boxTop - halfBoxHeight;
								tailOffset.y = -(halfBoxHeight + tailHalfHeight);
							}
							else
							{
								boxPosition.y = boxBottom + halfBoxHeight;
								tailOffset.y = halfBoxHeight + tailHalfHeight;
							}
						}
						else if( fitsAbove )
						{
							boxPosition.y = boxTop - halfBoxHeight;
							tailOffset.y = -(halfBoxHeight + tailHalfHeight);
						}
						else if( fitsBelow )
						{
							boxPosition.y = boxBottom + halfBoxHeight;
							tailOffset.y = halfBoxHeight + tailHalfHeight;
						}
						else
						{
							if( (cameraTop - agentPosition.y) >= (agentPosition.y - cameraBottom) )
							{
								boxPosition.y = cameraTop - halfBoxHeight;
								tailOffset.y = -(halfBoxHeight + tailHalfHeight);
							}
							else
							{
								boxPosition.y = cameraBottom + halfBoxHeight;
								tailOffset.y = halfBoxHeight + tailHalfHeight;
							}
						}
						/*halmeida - the y position is set, now we discover the x.*/
						boxPosition.x = agentPosition.x;
						halfBoxWidth = boxDimensions.x / 2f;
						cameraRight = cameraPosition.x + cameraHalfWidth;
						if( (agentPosition.x + halfBoxWidth) > cameraRight )
						{
							boxPosition.x = cameraRight - halfBoxWidth;
						}
						cameraLeft = cameraPosition.x - cameraHalfWidth;
						if( (agentPosition.x - halfBoxWidth) < cameraLeft )
						{
							boxPosition.x = cameraLeft + halfBoxWidth;
						}
						tailOffset.x = agentPosition.x - boxPosition.x;
						if( ((boxPosition.x + tailOffset.x) + tailHalfWidth) > cameraRight )
						{
							tailOffset.x = halfBoxWidth - tailHalfWidth;
						}
						if( ((boxPosition.x + tailOffset.x) - tailHalfWidth) < cameraLeft )
						{
							tailOffset.x = -halfBoxWidth + tailHalfWidth;
						}
						positionValid = true;
					}
				}
			}
		}
		return boxPosition;
	}

	private Vector3 GetBoxPositionForEnunciate( Vector2 boxDimensions, ref bool positionValid )
	{
		Vector3 boxPosition = Vector3.zero;
		Vector3 cameraPosition = Vector3.zero;

		positionValid = false;
		if( cameraObject != null )
		{
			/*halmeida - A box for question enunciate display is always shown horizontally centered at the screen and with its
			top at a fixed, predetermined position, regardless of being system message or character dialogue.*/
			cameraPosition = cameraObject.transform.position;
			boxPosition.x = cameraPosition.x;
			boxPosition.y = cameraPosition.y + enunciateBoxTopOffset - boxDimensions.y / 2f;
			positionValid = true;
		}
		return boxPosition;
	}

	private Vector3 GetBoxPositionForOption( int optionIndex, Vector2 boxDimensions, ref bool positionValid )
	{
		Vector3 boxPosition = Vector3.zero;
		Vector3 cameraPosition = Vector3.zero;
		float firstBoxTop = 0f;

		if( cameraObject != null )
		{
			cameraPosition = cameraObject.transform.position;
			boxPosition.x = cameraPosition.x;
			firstBoxTop = cameraPosition.y + optionBoxTopOffset;
			boxPosition.y = firstBoxTop - (optionIndex * optionAreaHeight) - optionHalfAreaHeight;
			positionValid = true;
		}
		return boxPosition;
	}

	public void SetOptionIndexConfirm( int newOptionIndexConfirm )
	{
		int boxOptionIndex = -1;

		if( (newOptionIndexConfirm > -1) && (newOptionIndexConfirm < optionAwaitedIndex) && (boxOptionIndexes != null) )
		{
			for( int i=0; i<boxOptionIndexes.Length; i++ )
			{
				boxOptionIndex = boxOptionIndexes[i];
				if( boxOptionIndex == newOptionIndexConfirm )
				{
					/*halmeida - relying on the coherence of all box related arrays.*/
					optionIndexConfirm = boxOptionIndex;
					optionComponentConfirm = boxComponents[i];
					break;
				}
			}
		}
	}

	public void SetOptionIndexCancel( int newOptionIndexCancel )
	{
		int boxOptionIndex = -1;

		if( (newOptionIndexCancel > -1) && (newOptionIndexCancel < optionAwaitedIndex) && (boxOptionIndexes != null) )
		{
			for( int i=0; i<boxOptionIndexes.Length; i++ )
			{
				boxOptionIndex = boxOptionIndexes[i];
				if( boxOptionIndex == newOptionIndexCancel )
				{
					/*halmeida - relying on the coherence of all box related arrays.*/
					optionIndexCancel = boxOptionIndex;
					optionComponentCancel = boxComponents[i];
					break;
				}
			}
		}
	}

	private void RemoveDialogue( int dialogueIndex, bool affectOptions )
	{
		GameObject boxTailObject = null;
		TextBox boxComponent = null;
		GameObject boxObject = null;
		EventDialogue boxMasterEvent = null;
		QuestionRole boxQuestionRole = QuestionRole.None;
		IOptionsListener savedQuestionDelegate = null;
		int savedOptionIndex = -1;

		if( boxObjects != null )
		{
			if( (dialogueIndex > -1) && (dialogueIndex < boxObjects.Length) )
			{
				/*halmeida - relying on the coherence of all box related arrays.*/
				boxTailObject = boxTailObjects[dialogueIndex];
				if( boxTailObject != null )
				{
					GameObject.Destroy( boxTailObject );
					boxTailObjects[dialogueIndex] = null;
				}
				boxComponent = boxComponents[dialogueIndex];
				if( boxComponent != null )
				{
					boxComponent.Clear();
					boxComponents[dialogueIndex] = null;
				}
				boxObject = boxObjects[dialogueIndex];
				if( boxObject != null )
				{
					GameObject.Destroy( boxObject );
					boxObjects[dialogueIndex] = null;
				}
				boxMasterEvent = boxMasterEvents[dialogueIndex];
				if( boxMasterEvent != null )
				{
					boxMasterEvent.ReactToDialogueEnd();
					boxMasterEvents[dialogueIndex] = null;
				}
				boxQuestionRole = boxQuestionRoles[dialogueIndex];
				UsefulFunctions.DecreaseArray<SpriteRenderer>( ref boxTailRenderers, dialogueIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref boxTailObjects, dialogueIndex );
				UsefulFunctions.DecreaseArray<bool>( ref boxTryAboves, dialogueIndex );
				UsefulFunctions.DecreaseArray<int>( ref boxSpeakerIDs, dialogueIndex );
				UsefulFunctions.DecreaseArray<int>( ref boxOptionIndexes, dialogueIndex );
				UsefulFunctions.DecreaseArray<QuestionRole>( ref boxQuestionRoles, dialogueIndex );
				UsefulFunctions.DecreaseArray<EventDialogue>( ref boxMasterEvents, dialogueIndex );
				UsefulFunctions.DecreaseArray<TextBox>( ref boxComponents, dialogueIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref boxObjects, dialogueIndex );
				if( (boxQuestionRole == QuestionRole.Option) && (questionDelegate != null) && affectOptions )
				{
					CloseQuestion();
				}
				else if( boxQuestionRole == QuestionRole.Enunciate )
				{
					/*halmeida - send choice to questionDelegate, reset variables to allow preparation for other questions.*/
					/*halmeida - we need to immediately remove the remaining options, because if a new question is loaded
					now, as we send the chosen index, and any remaining options of this closed question happen to be removed after
					that, the closure of the old option will trigger the closure of the new question entirely, before the player is
					given the chance of choosing anything or even the chance of seeing the new question.*/
					RemoveRemainingOptions();
					/*halmeida - when we send the choice, the questionDelegate may immediately start a new question, assigning
					itself or another questionDelegate to this TextBoxManager in the process. If we remove the question prep
					after sending the choice, we may be losing this new question's preparation. That's why we need to send
					the chosen choice only after the removal of the preparation of the current question, not before. To do
					that we need to save the values first.*/
					savedQuestionDelegate = questionDelegate;
					savedOptionIndex = optionChosenIndex;
					RemoveQuestionPreparation();
					if( savedQuestionDelegate != null )
					{
						savedQuestionDelegate.ChooseOption( savedOptionIndex );
					}
				}
			}
		}
	}

	private void CloseQuestion()
	{
		QuestionRole boxQuestionRole = QuestionRole.None;
		TextBox boxComponent = null;

		/*halmeida - start closure of the enunciate and all remaining options.*/ 
		if( boxQuestionRoles != null )
		{
			for( int i=0; i<boxQuestionRoles.Length; i++ )
			{
				boxQuestionRole = boxQuestionRoles[i];
				if( boxQuestionRole != QuestionRole.None )
				{
					/*halmeida - relying on the coherence of all box related arrays.*/
					boxComponent = boxComponents[i];
					if( boxComponent != null )
					{
						boxComponent.Close();
					}
				}
			}
		}
	}

	private void RemoveRemainingOptions()
	{
		QuestionRole boxQuestionRole = QuestionRole.None;

		if( boxQuestionRoles != null )
		{
			for( int i=0; i<boxQuestionRoles.Length; i++ )
			{
				boxQuestionRole = boxQuestionRoles[i];
				if( boxQuestionRole == QuestionRole.Option )
				{
					RemoveDialogue( i, false );
					if( boxQuestionRoles == null )
					{
						break;
					}
					else
					{
						i--;
					}
				}
			}
		}
	}

	private void RemoveQuestionPreparation()
	{
		questionDelegate = null;
		enunciateBoxComponent = null;
		optionsToAllocate = 0;
		optionAwaitedIndex = -1;
		optionChosenIndex = -1;
		optionIndexConfirm = -1;
		optionIndexCancel = -1;
		optionComponentConfirm = null;
		optionComponentCancel = null;
	}

	public void ClearAllDialogues()
	{
		GameObject boxObject = null;
		TextBox boxComponent = null;

		if( boxObjects != null )
		{
			for( int i=0; i<boxObjects.Length; i++ )
			{
				/*halmeida - relying on the coherence of all box related arrays.*/
				boxTailRenderers[i] = null;
				boxObject = boxTailObjects[i];
				if( boxObject != null )
				{
					GameObject.Destroy( boxObject );
					boxTailObjects[i] = null;
				}
				boxComponent = boxComponents[i];
				if( boxComponent != null )
				{
					boxComponent.Clear();
					boxComponents[i] = null;
				}
				boxObject = boxObjects[i];
				if( boxObject != null )
				{
					GameObject.Destroy( boxObject );
					boxObjects[i] = null;
				}
				boxMasterEvents[i] = null;
			}
			boxTailRenderers = null;
			boxTailObjects = null;
			boxTryAboves = null;
			boxSpeakerIDs = null;
			boxOptionIndexes = null;
			boxQuestionRoles = null;
			boxMasterEvents = null;
			boxComponents = null;
			boxObjects = null;
		}
		RemoveQuestionPreparation();
	}

	public void Progress( float timeStep )
	{
		TextBox boxComponent = null;
		GameObject boxObject = null;
		GameObject boxTailObject = null;
		int agentID = BaseAgent.INVALID_AGENT_ID;
		Vector2 boxDimensions = Vector2.zero;
		bool boxTryAbove = false;
		Vector2 tailOffset = Vector2.zero;
		Vector3 boxPosition = Vector3.zero;
		bool positionValid = false;
		Vector3 tailLocalPosition = Vector3.zero;
		QuestionRole boxQuestionRole = QuestionRole.None;
		int optionIndex = -1;

		if( !paused )
		{
			if( boxComponents != null )
			{
				for( int i=0; i<boxComponents.Length; i++ )
				{
					boxComponent = boxComponents[i];
					if( boxComponent != null )
					{
						boxComponent.Progress( timeStep );
						if( boxComponent.IsCleared() )
						{
							RemoveDialogue( i, true );
							if( boxComponents == null )
							{
								break;
							}
							else
							{
								i--;
							}
						}
						else
						{
							/*halmeida - relying on the coherence of all box related arrays.*/
							boxObject = boxObjects[i];
							agentID = boxSpeakerIDs[i];
							boxDimensions = boxComponent.GetBoxWorldDimensions();
							boxTryAbove = boxTryAboves[i];
							boxQuestionRole = boxQuestionRoles[i];
							switch( boxQuestionRole )
							{
								case QuestionRole.None:
									boxPosition = GetBoxPositionForAgent( agentID, boxDimensions, boxTryAbove, ref positionValid, ref tailOffset );
									break;
								case QuestionRole.Enunciate:
									boxPosition = GetBoxPositionForEnunciate( boxDimensions, ref positionValid );
									break;
								case QuestionRole.Option:
									optionIndex = boxOptionIndexes[i];
									boxPosition = GetBoxPositionForOption( optionIndex, boxDimensions, ref positionValid );
									break;
							}
							if( !positionValid )
							{
								RemoveDialogue( i, true );
								if( boxComponents == null )
								{
									break;
								}
								else
								{
									i--;
								}
							}
							else
							{
								boxObject.transform.localPosition = boxPosition;
								boxTailObject = boxTailObjects[i];
								if( boxTailObject != null )
								{
									tailLocalPosition = boxTailObject.transform.localPosition;
									tailLocalPosition.x = tailOffset.x;
									tailLocalPosition.y = tailOffset.y;
									boxTailObject.transform.localPosition = tailLocalPosition;
									if( tailOffset.y > 0f )
									{
										boxTailObject.transform.rotation = Quaternion.Euler( 0f, 0f, 180f );
									}
									else
									{
										boxTailObject.transform.rotation = Quaternion.Euler( 0f, 0f, 0f );
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public bool InterceptPointerDown( int screenPointX, int screenPointY )
	{
		Vector3 cameraPosition = Vector3.zero;
		TextBox boxComponent = null;
		bool intercepted = false;
		float screenRateX = 0f;
		float screenRateY = 0f;
		float worldPointX = 0f;
		float worldPointY = 0f;
		int optionIndex = -1;
		QuestionRole boxQuestionRole = QuestionRole.None;
		bool isOption = false;

		if( !paused )
		{
			if( (boxComponents != null) && (cameraObject != null) )
			{
				cameraPosition = cameraObject.transform.position;
				/*halmeida - take the bottom-left from the camera, within the world.*/
				cameraPosition.x -= cameraHalfWidth;
				cameraPosition.y -= cameraHalfHeight;
				/*halmeida - find the traveled rate of the point, using pixel coordinates.*/
				screenRateX = (float)screenPointX / (float)cameraPixelWidth;
				screenRateY = (float)screenPointY / (float)cameraPixelHeight;
				/*halmeida - use the traveled rate within the world.*/
				worldPointX = cameraPosition.x + screenRateX * cameraWidth;
				worldPointY = cameraPosition.y + screenRateY * cameraHeight;
				for( int i=0; i<boxComponents.Length; i++ )
				{
					boxComponent = boxComponents[i];
					if( boxComponent != null )
					{
						/*halmeida - relying on the coherence of all box related arrays.*/
						boxQuestionRole = boxQuestionRoles[i];
						isOption = false;
						if( boxQuestionRole == QuestionRole.Option )
						{
							optionIndex++;
							isOption = true;
						}
						if( boxComponent.ReactToPointerDown( worldPointX, worldPointY ) )
						{
							intercepted = true;
							if( isOption && (optionChosenIndex == -1) )
							{
								optionChosenIndex = optionIndex;
								optionIndexConfirm = -1;
								optionIndexCancel = -1;
								optionComponentConfirm = null;
								optionComponentCancel = null;
							}
							break;
						}
					}
				}
			}
		}
		return intercepted;
	}

	public bool InterceptActionPress()
	{
		TextBox boxComponent = null;
		bool intercepted = false;

		if( !paused )
		{
			if( boxComponents != null )
			{
				for( int i=0; i<boxComponents.Length; i++ )
				{
					boxComponent = boxComponents[i];
					if( boxComponent != null )
					{
						if( boxComponent.ReactToActionPress() )
						{
							intercepted = true;
						}
					}
				}
			}
		}
		return intercepted;
	}

	public bool InterceptConfirmation()
	{
		if( !paused )
		{
			if( optionComponentConfirm != null )
			{
				optionComponentConfirm.Close();
				optionChosenIndex = optionIndexConfirm;
				optionIndexConfirm = -1;
				optionIndexCancel = -1;
				optionComponentConfirm = null;
				optionComponentCancel = null;
				return true;
			}
		}
		return InterceptActionPress();
	}

	public bool InterceptCancellation()
	{
		if( !paused )
		{
			if( optionComponentCancel != null )
			{
				optionComponentCancel.Close();
				optionChosenIndex = optionIndexCancel;
				optionIndexConfirm = -1;
				optionIndexCancel = -1;
				optionComponentConfirm = null;
				optionComponentCancel = null;
				return true;
			}
		}
		return false;
	}

	public void TogglePause( bool pause )
	{
		paused = pause;
	}

	public bool IsPaused()
	{
		return paused;
	}
}
