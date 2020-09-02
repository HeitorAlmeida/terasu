using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaLockItemController : ItemController, ITransactionListener
{
	public const float VERTICAL_DISTANCE_TO_BOX = 0.5f;
	public const int PAYMENT_INITIAL_VALUE = 1;
	public const int PAYMENT_CHANGE_FACTOR_1 = 5;
	public const int PAYMENT_CHANGE_FACTOR_2 = 2;
	public const int PAYMENTS_TO_CHANGE = 5;
	public const float PAYMENT_DURATION = 2f;

	public int areaLockIndex;
	public GameObject worldSpaceGaugeModel;

	private Graph graph;
	private int cost;
	private int paid;
	private int payment;
	private int paymentInitialValue;
	private int paymentChangeFactor;
	private int paymentsToChange;
	private int paymentsDone;
	private float paymentDuration;
	private float paymentElapsed;
	private ShortLifeAnimator areaLockAnimator;
	private TransactionBoxManager transactionBoxManager;
	private PlayerAgent transactionAgent;
	private bool transactionCreated;
	private bool runningMiddleWave;

	protected override void Awake()
	{
		areaLockAnimator = null;
		/*halmeida - set the animator to null because the Awake of the base class will call Clear(), and
		the redefinition of Clear() in this class asks if the animator is null.*/
		base.Awake();
		graph = null;
		cost = 0;
		paid = 0;
		payment = 0;
		paymentInitialValue = PAYMENT_INITIAL_VALUE;
		paymentChangeFactor = PAYMENT_CHANGE_FACTOR_1;
		paymentsToChange = PAYMENTS_TO_CHANGE;
		paymentsDone = 0;
		paymentDuration = PAYMENT_DURATION;
		paymentElapsed = 0f;
		transactionBoxManager = TransactionBoxManager.Instance;
		transactionAgent = null;
		transactionCreated = false;
		runningMiddleWave = false;

		continuousEffect = true;
	}

	protected override void ExtractItemAnimator()
	{
		areaLockAnimator = GetComponent<ShortLifeAnimator>();
	}

	protected override void ExtractTriggers()
	{
		base.ExtractTriggers();
		graph = triggerSourceComponent.GetGraph();
		if( graph != null )
		{
			cost = graph.GetAreaUnlockCost( areaLockIndex );
			if( cost > -1 )
			{
				paid = graph.GetAreaUnlockPaid( areaLockIndex );
				if( paid >= cost )
				{
					/*halmeida - the area is already unlocked, update the animation.*/
					UnlockAnimators();
				}
			}
		}
	}

	/*halmeida - this function receives an index that is valid across all relevant arrays and denotes
	an element that is not null across all relevant arrays.*/
	protected override void Trigger( int triggerIndex )
	{
		if( transactionAgent == null )
		{
			transactionAgent = triggerComponents[triggerIndex];
			/*halmeida - the playerComponent is garanteed to be not null and not dead, because this was previously tested.*/
			payment = paymentInitialValue;
			paymentChangeFactor = PAYMENT_CHANGE_FACTOR_1;
			paymentsDone = 0;
			/*halmeida - open the transaction interface element.*/
			if( (transactionBoxManager != null) && transactionAgent.GetTransactionsTrigger() )
			{
				transactionCreated = transactionBoxManager.CreateTransaction( gameObject, this, VERTICAL_DISTANCE_TO_BOX, cost,
					paid, payment, true, true, worldSpaceGaugeModel );
			}
		}
		base.Trigger( triggerIndex );
	}

	protected override void ActivateAnimators()
	{
		/*halmeida - do nothing, because the triggering should not do anything to the animation.*/
	}

	protected override void RemoveTriggeringObject( int triggeringIndex )
	{
		PlayerAgent triggeringToRemove = null;

		if( transactionAgent != null )
		{
			triggeringToRemove = triggeringComponents[triggeringIndex];
			if( transactionAgent == triggeringToRemove )
			{
				if( (transactionBoxManager != null) && transactionCreated )
				{
					transactionBoxManager.CloseTransaction( gameObject );
					transactionCreated = false;
				}
				transactionAgent = null;
			}
		}
		base.RemoveTriggeringObject( triggeringIndex );
	}

	protected override void Untrigger()
	{
		/*halmeida - perform normal untrigger actions.*/
		base.Untrigger();
	}

	protected override void DeactivateAnimators()
	{
		/*halmeida - do nothing, because the untriggering should not do anything to the animation.*/
	}

	public override void Clear()
	{
		/*halmeida - remove the transaction box from the manager.*/
		if( transactionAgent != null )
		{
			if( (transactionBoxManager != null) && transactionCreated )
			{
				transactionBoxManager.RemoveTransaction( gameObject );
				transactionCreated = false;
			}
			transactionAgent = null;
		}
		runningMiddleWave = false;
		base.Clear();
	}

	protected override void ClearItemAnimator()
	{
		if( areaLockAnimator != null )
		{
			areaLockAnimator.Clear();
			areaLockAnimator = null;
		}
	}

	public override void CorrectDepthChain()
	{
		if( areaLockAnimator != null )
		{
			areaLockAnimator.StartDepthChain( gameObject.transform.position.z, true );
		}
	}

	private void UnlockAnimators()
	{
		if( areaLockAnimator != null )
		{
			areaLockAnimator.StartDying();
		}
		if( fireController != null )
		{
			fireController.SetActive( true );
		}
		ExtractWaveAnimator( 1 );
	}

	public void AdvanceTransaction()
	{
		int costLeft = 0;
		int agentCoins = 0;
		int actualPayment = 0;
		int change = 0;

		if( (transactionAgent != null) && transactionCreated && (paid < cost) )
		{
			costLeft = cost - paid;
			actualPayment = (payment > costLeft) ? costLeft : payment;
			agentCoins = transactionAgent.GetCurrentCoins();
			if( agentCoins > 0 )
			{
				actualPayment = (actualPayment > agentCoins) ? agentCoins : actualPayment;
				if( graph != null )
				{
					graph.PayToUnlockArea( areaLockIndex, actualPayment, ref change );
				}
				paid += actualPayment;
				if( paid >= cost )
				{
					UnlockAnimators();
				}
				transactionAgent.SetCurrentCoins( agentCoins - actualPayment );
				paymentsDone++;
				if( paymentsDone == paymentsToChange )
				{
					payment *= paymentChangeFactor;
					paymentChangeFactor = (paymentChangeFactor == PAYMENT_CHANGE_FACTOR_1) ? PAYMENT_CHANGE_FACTOR_2 : PAYMENT_CHANGE_FACTOR_1;
					paymentsDone = 0;
				}
				paymentElapsed = 0f;
				if( transactionBoxManager != null )
				{
					transactionBoxManager.UpdateTransaction( gameObject, cost, paid, payment, true, worldSpaceGaugeModel );
				}
			}
		}
	}

	public override void Progress( float timeStep )
	{
		if( transactionAgent != null )
		{
			if( !transactionCreated )
			{
				if( (transactionBoxManager != null) && transactionAgent.GetTransactionsTrigger() )
				{
					transactionCreated = transactionBoxManager.CreateTransaction( gameObject, this, VERTICAL_DISTANCE_TO_BOX, cost,
						paid, payment, true, true, worldSpaceGaugeModel );
				}
			}
			else
			{
				if( payment > paymentInitialValue )
				{
					paymentElapsed += timeStep;
					if( paymentElapsed > paymentDuration )
					{
						payment = paymentInitialValue;
						paymentChangeFactor = PAYMENT_CHANGE_FACTOR_1;
						paymentsDone = 0;
						if( transactionBoxManager != null )
						{
							transactionBoxManager.UpdateTransaction( gameObject, cost, paid, payment, true, worldSpaceGaugeModel );
						}
					}
				}
			}
		}
		base.Progress( timeStep );
	}

	protected override void ProgressItemAnimator( float timeStep )
	{
		if( areaLockAnimator != null )
		{
			areaLockAnimator.Progress( timeStep );
		}
	}

	protected override void ProgressWaveAnimator( float timeStep )
	{
		if( waveAnimator != null )
		{
			if( !waveAnimator.HasRunningWave() )
			{
				if( waveEmitterIndex == 1 )
				{
					if( !runningMiddleWave )
					{
						runningMiddleWave = true;
					}
					else
					{
						ExtractWaveAnimator( 2 );
						runningMiddleWave = false;
					}
				}
				/*halmeida - gotta check if they are null again cause the extraction might have nullified them.*/
				if( (waveAnimator != null) && (waveParams != null) )
				{
					waveAnimator.StartWave( waveParams.waveColor, waveAnimator.gameObject.transform.position, waveParams.waveLengths,
						waveParams.waveAlphas, waveParams.waveFrontLimit, waveParams.waveSpeed, waveParams.waveLengthVariation, 1f, 0f,
						waveParams.waveNoise, waveParams.waveNoiseScale, waveParams.waveDelay );
				}
			}
			/*halmeida - gotta check if it's null again cause the extraction might have nullified it.*/
			if( waveAnimator != null )
			{
				waveAnimator.Progress( timeStep );
			}
		}
	}
}
