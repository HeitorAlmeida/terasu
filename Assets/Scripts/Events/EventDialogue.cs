using System.Collections;
using UnityEngine;

public class EventDialogue : EventBase
{
	public GameTextDatabase.TextType dialogueType;
	public int[] dialogueIDs;

	private int dialogueIndex;
	private int dialogueID;
	private GameTextDatabase.TextPlatform dialoguePlatform;
	private bool dialogueCreated;
	private bool dialogueCleared;
	private GameTextDatabase gameTextDatabase;
	private TextBoxManager textBoxManager;

	protected override void Awake()
	{
		base.Awake();
		dialogueIndex = -1;
		dialogueID = GameTextDatabase.INVALID_TEXT_ID;
		#if !UNITY_ANDROID || UNITY_EDITOR
			dialoguePlatform = GameTextDatabase.TextPlatform.PC;
		#else
			dialoguePlatform = GameTextDatabase.TextPlatform.Android;
		#endif
		dialogueCreated = false;
		dialogueCleared = false;
		gameTextDatabase = null;
		textBoxManager = null;
	}

	public override void ExtractInterests( ChallengeStage stage )
	{
		base.ExtractInterests( stage );
		gameTextDatabase = GameTextDatabase.Instance;
		textBoxManager = TextBoxManager.Instance;
	}

	protected override bool EvaluateTrigger( EventBase[] eventSet, int currentNodeID )
	{
		if( base.EvaluateTrigger( eventSet, currentNodeID ) )
		{
			return ( (gameTextDatabase != null) && (textBoxManager != null) );
		}
		return false;
	}

	protected override void PrepareForAction()
	{
		base.PrepareForAction();
		AdvanceDialogueIndex();
	}

	private void AdvanceDialogueIndex()
	{
		if( dialogueIDs != null )
		{
			if( dialogueIndex < 0 )
			{
				if( !dialogueCleared )
				{
					dialogueIndex = 0;
					dialogueID = dialogueIDs[0];
					dialogueCreated = false;
				}
			}
			else
			{
				dialogueIndex++;
				if( dialogueIndex < dialogueIDs.Length )
				{
					dialogueID = dialogueIDs[dialogueIndex];
					dialogueCreated = false;
					dialogueCleared = false;
				}
				else
				{
					dialogueIndex = -1;
					dialogueID = GameTextDatabase.INVALID_TEXT_ID;
				}
			}
		}
	}

	protected override void PerformAction( float timeStep )
	{
		string text = null;
		string speakerName = null;
		bool tryAbove = false;
		bool autoClose = false;

		base.PerformAction( timeStep );
		if( !dialogueCreated )
		{
			if( (gameTextDatabase != null) && (textBoxManager != null) )
			{
				switch( dialogueType )
				{
					case GameTextDatabase.TextType.Platform:
						text = gameTextDatabase.GetPlatformText( dialogueID, dialoguePlatform, ref autoClose );
						dialogueCreated = textBoxManager.ShowDialogue( null, text, false, autoClose, this );
						break;
					case GameTextDatabase.TextType.System:
						text = gameTextDatabase.GetSystemText( dialogueID, ref autoClose );
						dialogueCreated = textBoxManager.ShowDialogue( null, text, false, autoClose, this );
						break;
					case GameTextDatabase.TextType.Dialogue:
						text = gameTextDatabase.GetDialogueText( dialogueID, ref speakerName, ref tryAbove, ref autoClose );
						dialogueCreated = textBoxManager.ShowDialogue( speakerName, text, tryAbove, autoClose, this );
						break;
					case GameTextDatabase.TextType.Item:
						text = gameTextDatabase.GetItemDescription( dialogueID, ref speakerName );
						dialogueCreated = textBoxManager.ShowDialogue( speakerName, text, false, false, this );
						break;
				}
			}
		}
	}

	public void ReactToDialogueEnd()
	{
		if( dialogueCreated && !dialogueCleared )
		{
			dialogueCleared = true;
			AdvanceDialogueIndex();
		}
	}

	protected override bool EvaluateConclusion()
	{
		return (dialogueCleared && (dialogueIndex == -1));
	}

	protected override void RemovePreparation()
	{
		base.RemovePreparation();
	}

	protected override void AbandonInterests()
	{
		base.AbandonInterests();
		gameTextDatabase = null;
		textBoxManager = null;
	}

	public override void Cancel( bool forceConclusion, bool forceReset )
	{
		base.Cancel( forceConclusion, forceReset );
		dialogueIndex = -1;
		dialogueID = GameTextDatabase.INVALID_TEXT_ID;
		dialogueCreated = false;
		dialogueCleared = false;
	}
}
