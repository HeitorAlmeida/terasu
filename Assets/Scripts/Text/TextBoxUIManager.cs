using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxUIManager
{

	public const float BOX_OPENING_SPEED = 2f;

	public enum QuestionRole { None, Enunciate, Option }

	private static TextBoxUIManager instance;

	public static TextBoxUIManager Instance
	{
		get
		{
			if( instance == null )
			{
				instance = new TextBoxUIManager();
			}
			return instance;
		}
	}

	private GameObject[] boxObjects;
	private TextBoxUI[] boxComponents;
	private EventDialogue[] boxMasterEvents;
	private ItemData[] boxMasterItems;
	private QuestionRole[] boxQuestionRoles;
	private int[] boxOptionIndexes;
	private SymbolDatabase symbolDatabase;
	private BoxAppearanceDatabase boxDatabase;
	private int boxAppearanceIndex;
	private Sprite bodySprite;
	private RectTransform uiCanvasTrans;
	private IOptionsListener questionDelegate;
	private TextBoxUI enunciateBoxComponent;
	private int optionAwaitedIndex;
	private int optionChosenIndex;
	private int optionIndexConfirm;
	private int optionIndexCancel;
	private TextBoxUI optionComponentConfirm;
	private TextBoxUI optionComponentCancel;
	private bool itemOnDisplay;

	private TextBoxUIManager()
	{
		boxObjects = null;
		boxComponents = null;
		boxMasterEvents = null;
		boxMasterItems = null;
		boxQuestionRoles = null;
		boxOptionIndexes = null;
		symbolDatabase = null;
		boxDatabase = null;
		boxAppearanceIndex = -1;
		bodySprite = null;
		uiCanvasTrans = null;
		questionDelegate = null;
		enunciateBoxComponent = null;
		optionAwaitedIndex = -1;
		optionChosenIndex = -1;
		optionIndexConfirm = -1;
		optionIndexCancel = -1;
		optionComponentConfirm = null;
		optionComponentCancel = null;
		itemOnDisplay = false;
	}

	public void SetMaterials( SymbolDatabase newSymbolDatabase, BoxAppearanceDatabase newBoxDatabase )
	{
		Sprite tailSprite = null;

		symbolDatabase = newSymbolDatabase;
		boxDatabase = newBoxDatabase;
		if( boxDatabase != null )
		{
			if( !boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.UI, 0, ref bodySprite, ref tailSprite ) )
			{
				boxAppearanceIndex = -1;
				bodySprite = null;
			}
			else
			{
				boxAppearanceIndex = 0;
			}
		}
	}

	public void AdvanceBoxAppearance()
	{
		Sprite tailSprite = null;
		int newIndex = -1;
		TextBoxUI boxComponent = null;

		if( (boxDatabase != null) && (boxAppearanceIndex > -1) )
		{
			newIndex = boxAppearanceIndex++;
			if( !boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.UI, newIndex, ref bodySprite, ref tailSprite ) )
			{
				newIndex = 0;
				boxDatabase.GetBoxAppearance( BoxAppearanceDatabase.BoxPurpose.UI, newIndex, ref bodySprite, ref tailSprite );
			}
			boxAppearanceIndex = newIndex;
			if( boxComponents != null )
			{
				for( int i=0; i<boxComponents.Length; i++ )
				{
					boxComponent = boxComponents[i];
					if( boxComponent != null )
					{
						boxComponent.ChangeBoxBody( bodySprite );
					}
				}
			}
		}
	}

	public void SetUICanvasTransform( RectTransform uiCanvasTransform )
	{
		uiCanvasTrans = uiCanvasTransform;
	}

	public void PrepareForQuestion( IOptionsListener newQuestionDelegate )
	{
		if( newQuestionDelegate != null )
		{
			questionDelegate = newQuestionDelegate;
			optionAwaitedIndex = 0;
		}
	}

	public bool ShowDialogue( string dialogueText, float dialogueTextSpeed, Sprite dialoguePicture, bool autoClose, Vector2 boxMaxSizeRates,
		Vector2 boxCenterPositionRates, EventDialogue masterEvent = null, ItemData masterItem = null, QuestionRole questionRole = QuestionRole.None )
	{
		GameObject boxObject = null;
		TextBoxUI boxComponent = null;
		RectTransform boxTransform = null;
		Vector2 boxPosition = Vector2.zero;
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
		if( uiCanvasTrans != null )
		{
			if( ((symbolDatabase != null) && (dialogueText != null)) || (dialoguePicture != null) )
			{
				boxObject = new GameObject("UITextBox", typeof(RectTransform));
				boxComponent = boxObject.AddComponent<TextBoxUI>();
				boxComponent.SetInterfaceCanvasTransform( uiCanvasTrans );
				boxComponent.SetBoxSizeLimitRates( boxMaxSizeRates.x, boxMaxSizeRates.y );
				boxComponent.SetEssentials( symbolDatabase, bodySprite, dialogueText, dialogueTextSpeed, TextBoxUI.TextAlignment.AlignMiddle,
					dialoguePicture, autoClose, 0f, 0.7f, new Vector2( 0.1f, 0.1f), Vector2.one, BOX_OPENING_SPEED );
				boxComponent.Build();
				switch( questionRole )
				{
					case QuestionRole.Enunciate:
						/*halmeida - enunciate text boxes cannot react to any input.*/
						boxComponent.BlockReactions();
						enunciateBoxComponent = boxComponent;
						break;
					case QuestionRole.Option:
						/*halmeida - option text boxes can only react to precise input over them.*/
						boxComponent.AllowReactions( true );
						optionCurrentIndex = optionAwaitedIndex;
						optionAwaitedIndex++;
						break;
				}
				boxTransform = boxObject.GetComponent<RectTransform>();
				boxTransform.SetParent( uiCanvasTrans, false );
				boxTransform.anchoredPosition = GetCanvasPositionFromScreenRates( boxCenterPositionRates );
				boxComponent.Open();
				UsefulFunctions.IncreaseArray<GameObject>( ref boxObjects, boxObject );
				UsefulFunctions.IncreaseArray<TextBoxUI>( ref boxComponents, boxComponent );
				UsefulFunctions.IncreaseArray<EventDialogue>( ref boxMasterEvents, masterEvent );
				UsefulFunctions.IncreaseArray<ItemData>( ref boxMasterItems, masterItem );
				UsefulFunctions.IncreaseArray<QuestionRole>( ref boxQuestionRoles, questionRole );
				UsefulFunctions.IncreaseArray<int>( ref boxOptionIndexes, optionCurrentIndex );
				if( masterItem != null )
				{
					itemOnDisplay = true;
				}
				return true;
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
				/*halmeida - differing from the normal world space TextBoxManager, this UI box manager does not
				require an enunciate for the acceptance of options. It only requires something to receive the
				choice when it is made.*/
				return (questionDelegate != null);
		}
		return false;
	}

	public Vector2 GetCanvasPositionFromScreenRates( Vector2 screenRatesPosition )
	{
		Vector2 canvasPosition = Vector2.zero;

		if( uiCanvasTrans != null )
		{
			canvasPosition.x = (screenRatesPosition.x - 0.5f) * uiCanvasTrans.rect.width;
			canvasPosition.y = (screenRatesPosition.y - 0.5f) * uiCanvasTrans.rect.height; 
		}
		return canvasPosition;
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
		TextBoxUI boxComponent = null;
		GameObject boxObject = null;
		EventDialogue boxMasterEvent = null;
		QuestionRole boxQuestionRole = QuestionRole.None;
		IOptionsListener savedQuestionDelegate = null;
		int savedOptionIndex = -1;
		bool lastOptionToClose = false;

		if( boxObjects != null )
		{
			if( (dialogueIndex > -1) && (dialogueIndex < boxObjects.Length) )
			{
				/*halmeida - relying on the coherence of all box related arrays.*/
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
				boxMasterItems[dialogueIndex] = null;
				boxQuestionRole = boxQuestionRoles[dialogueIndex];
				UsefulFunctions.DecreaseArray<int>( ref boxOptionIndexes, dialogueIndex );
				UsefulFunctions.DecreaseArray<QuestionRole>( ref boxQuestionRoles, dialogueIndex );
				UsefulFunctions.DecreaseArray<EventDialogue>( ref boxMasterEvents, dialogueIndex );
				UsefulFunctions.DecreaseArray<ItemData>( ref boxMasterItems, dialogueIndex );
				UsefulFunctions.DecreaseArray<TextBoxUI>( ref boxComponents, dialogueIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref boxObjects, dialogueIndex );
				/*halmeida - if there are no more items associated with dialogues, we update the related variable.*/
				itemOnDisplay = (boxMasterItems == null) ? false : itemOnDisplay;
				if( itemOnDisplay )
				{
					itemOnDisplay = false;
					for( int i=0; i<boxMasterItems.Length; i++ )
					{
						if( boxMasterItems[i] != null )
						{
							itemOnDisplay = true;
							break;
						}
					}
				}
				/*halmeida - the chosen option closes before the others, as a visual feedback to the player as to
				which option did he choose. If there was more than one available choice, we start closing them when
				the closure of the chosen one ends. We only send the chosen index to the delegate by the time all
				options have finished closing.*/
				if( (boxQuestionRole == QuestionRole.Option) && (questionDelegate != null) )
				{
					/*halmeida - since we don't really require an enunciate, I can't use the end of the closure of the
					enunciate as the trigger to send the chosen option and prepare for another question. I will use the
					end of the closure of the last option as this trigger.*/
					lastOptionToClose = (boxQuestionRoles == null);
					if( !lastOptionToClose )
					{
						lastOptionToClose = true;
						for( int i=0; i<boxQuestionRoles.Length; i++ )
						{
							if( boxQuestionRoles[i] == QuestionRole.Option )
							{
								lastOptionToClose = false;
							}
						}
					}
					if( lastOptionToClose )
					{
						/*halmeida - send choice to questionDelegate, reset variables to allow preparation for other questions.*/
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
					else
					{
						if( affectOptions )
						{
							CloseQuestion();
						}
					}
				}
			}
		}
	}

	private void CloseQuestion()
	{
		QuestionRole boxQuestionRole = QuestionRole.None;
		TextBoxUI boxComponent = null;

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

	private void RemoveQuestionPreparation()
	{
		questionDelegate = null;
		enunciateBoxComponent = null;
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
		TextBoxUI boxComponent = null;

		if( boxObjects != null )
		{
			for( int i=0; i<boxObjects.Length; i++ )
			{
				/*halmeida - relying on the coherence of all box related arrays.*/
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
				boxMasterItems[i] = null;
			}
			boxOptionIndexes = null;
			boxQuestionRoles = null;
			boxMasterItems = null;
			boxMasterEvents = null;
			boxComponents = null;
			boxObjects = null;
		}
		RemoveQuestionPreparation();
		itemOnDisplay = false;
	}

	public void Progress( float timeStep )
	{
		TextBoxUI boxComponent = null;

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
				}
			}
		}
	}

	public bool InterceptPointerDown( Vector2 canvasPosition )
	{
		TextBoxUI boxComponent = null;
		bool intercepted = false;
		int optionIndex = -1;
		QuestionRole boxQuestionRole = QuestionRole.None;
		bool isOption = false;

		if( boxComponents != null )
		{
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
					if( boxComponent.ReactToPointerDown( canvasPosition ) )
					{
						intercepted = true;
						if( isOption && (optionChosenIndex == -1) )
						{
							optionChosenIndex = optionIndex;
							optionIndexConfirm = -1;
							optionIndexCancel = -1;
							optionComponentConfirm = null;
							optionComponentCancel = null;
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
		TextBoxUI boxComponent = null;
		bool intercepted = false;

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
		return intercepted;
	}

	public bool InterceptConfirmation()
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
		return InterceptActionPress();
	}

	public bool InterceptCancellation()
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
		return false;
	}

	public bool IsDisplayingItem()
	{
		return itemOnDisplay;
	}
}
