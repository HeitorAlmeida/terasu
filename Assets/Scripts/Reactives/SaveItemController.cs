using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveItemController : ItemController, IOptionsListener
{
	public const float QUESTION_DELAY = 1f;

	private bool askingToSave;
	private bool askingToOverwrite;
	private GameTextDatabase gameTextDatabase;
	private TextBoxManager textBoxManager;
	private bool delayingQuestion;
	private float delayDuration;
	private float delayElapsed;

	protected override void Awake()
	{
		base.Awake();
		askingToSave = false;
		askingToOverwrite = false;
		gameTextDatabase = GameTextDatabase.Instance;
		textBoxManager = TextBoxManager.Instance;
		delayingQuestion = false;
		delayDuration = QUESTION_DELAY;
		delayElapsed = 0f;
	}

	protected override void OnTriggerEnter2D( Collider2D otherCollider )
	{
		base.OnTriggerEnter2D( otherCollider );
		if( triggered )
		{
			if( !delayingQuestion )
			{
				delayingQuestion = true;
				delayElapsed = 0f;
			}

		}
	}

	protected override void OnTriggerExit2D (Collider2D otherCollider)
	{
		base.OnTriggerExit2D( otherCollider );
		if( !triggered )
		{
			if( delayingQuestion )
			{
				delayingQuestion = false;
			}
			if( triggerSourceComponent != null )
			{
				triggerSourceComponent.EnableSaving();
			}
		}
	}

	public override void Progress( float timeStep )
	{
		base.Progress( timeStep );
		if( delayingQuestion )
		{
			delayElapsed += timeStep;
			if( delayElapsed > delayDuration )
			{
				delayingQuestion = false;
				if( !askingToSave && !askingToOverwrite && (triggerSourceComponent != null) )
				{
					if( triggerSourceComponent.IsSavingEnabled() )
					{
						askingToSave = LoadSaveQuestion( false );
						if( askingToSave )
						{
							triggerSourceComponent.PauseGameForQuestion( this );
						}
					}
				}
			}
		}
	}

	private bool LoadSaveQuestion( bool overwriting )
	{
		int enunciateTextID = GameTextDatabase.INVALID_TEXT_ID;
		string textEnunciate = null;
		string textOptionZero = null;
		string textOptionOne = null;
		bool toDiscard = false;
		bool textCreated = false;

		if( (gameTextDatabase != null) && (textBoxManager != null) )
		{
			enunciateTextID = ( overwriting ? GameTextDatabase.TEXT_ID_SAVE_OVERWRITE_ENUNCIATE : GameTextDatabase.TEXT_ID_SAVE_ENUNCIATE );
			textEnunciate = gameTextDatabase.GetSystemText( enunciateTextID, ref toDiscard );
			textOptionZero = gameTextDatabase.GetSystemText( GameTextDatabase.TEXT_ID_YES, ref toDiscard );
			textOptionOne = gameTextDatabase.GetSystemText( GameTextDatabase.TEXT_ID_NO, ref toDiscard );
			if( (textEnunciate != null) && (textOptionZero != null) && (textOptionOne != null) )
			{
				textBoxManager.PrepareForQuestion( this, 2 );
				textCreated = textBoxManager.ShowDialogue( null, textEnunciate, true, false, null, TextBoxManager.QuestionRole.Enunciate );
				if( textCreated )
				{
					textCreated = textBoxManager.ShowDialogue( null, textOptionZero, true, false, null, TextBoxManager.QuestionRole.Option );
				}
				if( textCreated )
				{
					textCreated = textBoxManager.ShowDialogue( null, textOptionOne, true, false, null, TextBoxManager.QuestionRole.Option );
				}
				if( !textCreated )
				{
					textBoxManager.ClearAllDialogues();
				}
				else
				{
					/*halmeida - set option indexes for a generic confirmation and a generic cancellation commands, so that options
					don't actually need to be clicked to be chosen.*/
					textBoxManager.SetOptionIndexConfirm( 0 );
					textBoxManager.SetOptionIndexCancel( 1 );
				}
			}
		}
		return textCreated;
	}

	private void LoadSaveFeedback( bool tried, bool successful )
	{
		int feedbackTextID = GameTextDatabase.INVALID_TEXT_ID;
		string feedbackText = null;
		bool toDiscard = false;

		if( (gameTextDatabase != null) && (textBoxManager != null) )
		{
			if( tried )
			{
				Debug.Log("Debug : SaveItemController : tried to save. Success = "+successful+".");
				feedbackTextID = ( successful ? GameTextDatabase.TEXT_ID_SAVE_CONFIRMATION : GameTextDatabase.TEXT_ID_SAVE_FAILURE );
			}
			else
			{
				feedbackTextID = GameTextDatabase.TEXT_ID_SAVE_CANCELATION;
				Debug.Log("Debug : SaveItemController : did not try to save, cancelling the operation.");
			}
			feedbackText = gameTextDatabase.GetSystemText( feedbackTextID, ref toDiscard );
			textBoxManager.ShowDialogue( null, feedbackText, true, true );
		}
	}

	public void ChooseOption( int optionIndex )
	{
		bool saveSuccessful = false;

		if( askingToSave )
		{
			askingToSave = false;
			if( triggerSourceComponent != null )
			{
				if( optionIndex == 0 )
				{
					Debug.Log("Debug : SaveItemController : player chose to save progress.");
					if( triggerSourceComponent.SavedProgressExists() )
					{
						askingToOverwrite = LoadSaveQuestion( true );
						if( !askingToOverwrite )
						{
							Debug.Log("Debug : SaveItemController : could not load overwrite progress question.");
							triggerSourceComponent.UnpauseGameFromQuestion( this );
						}
					}
					else
					{
						saveSuccessful = triggerSourceComponent.SaveGameProgress();
						triggerSourceComponent.UnpauseGameFromQuestion( this );
						LoadSaveFeedback( true, saveSuccessful );
					}
				}
				else
				{
					Debug.Log("Debug : SaveItemController : player chose to not save progress.");
					triggerSourceComponent.UnpauseGameFromQuestion( this );
					LoadSaveFeedback( false, false );
				}
			}
		}
		else if( askingToOverwrite )
		{
			askingToOverwrite = false;
			if( triggerSourceComponent != null )
			{
				if( optionIndex == 0 )
				{
					saveSuccessful = triggerSourceComponent.SaveGameProgress();
					triggerSourceComponent.UnpauseGameFromQuestion( this );
					LoadSaveFeedback( true, saveSuccessful );
				}
				else
				{
					triggerSourceComponent.UnpauseGameFromQuestion( this );
					LoadSaveFeedback( false, false );
				}
			}
		}
	}

	public bool ListensFromUI()
	{
		return false;
	}
}
