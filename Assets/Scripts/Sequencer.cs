using UnityEngine;
using UnityEngine.UI;
using static GameActions;
using static GameDirections;
using static GridManager;

public class Sequencer : MonoBehaviour
{
    public static Sequencer sequencer { get; private set; }

    //Image cardBackground;
    Image notNextCardCover;

    CardTrigger[] levelCards;

    [Header ("PART OF THE HUD")] 
    [SerializeField] GameObject cardsInLevel;

    public CardTrigger lastCard { get; private set; }
    public bool state { get; private set; }
    public NumberItem lastNumber { get; private set; }
    public Directions lastDirection { get; set; }

    public void Awake()
    {
        if (sequencer != null && sequencer != this)
        {
            Destroy(gameObject);
            return;
        }
        sequencer = this;
        FillCards();
    }

    public void FillCards()
    {
        levelCards = new CardTrigger[cardsInLevel.transform.childCount];

        for (int i = 0; i < levelCards.Length; i++)
        {
            levelCards[i] = cardsInLevel.transform.GetChild(i).GetComponent<CardTrigger>();
            levelCards[i].SetSlotNumber(i + 1);
            levelCards[i].Initialize();
        }
    }

    public void NextCard(int recievedValue)
    {
        for (int i = 0; i < levelCards.Length; i++)
        {
            notNextCardCover = levelCards[i].notNextCover;

            if (i == recievedValue - 1)
            {  // This is the next card
                levelCards[i].nextInSequence = true;
                notNextCardCover.gameObject.SetActive(false);
            }
            else
            {   // Not the next card
                levelCards[i].nextInSequence = false;

                if (levelCards[i].available)
                {
                    notNextCardCover.gameObject.SetActive(true);
                }
            }
        }
    }

    public void CommunicateAIActions(bool isBefore, Directions direction)
    {
        if (gridManager.AIActions != null)
        {
            gridManager.AIActions.direction = direction;
            gridManager.AIActions.action = AIActions.Move;
            lastDirection = direction;
            gridManager.AIActions.canMove = isBefore;
            gridManager.AIActions.isBefore = isBefore;
            gridManager.AIActions.isIAActive = true;
        }
    }

    public void ManageLockState(bool locked)
    {
        foreach (var card in levelCards)
        {
            card.Lockdown(locked);
        }
        if (locked == false) NextCard(lastNumber.value);
    }

    public void AICanMoveNow()
    {
        if (gridManager.AIActions != null
            && gridManager.AIActions.action != AIActions.Stay)
        {
            gridManager.AIActions.direction = lastDirection;
            gridManager.AIActions.canMove = true;
        }
    }
    public void CommunicateAction(NumberItem recievedNumber, Actions usedAction)
    {
        for (int i = 0; i < levelCards.Length; i++)
        {
            // Must match the usedAction AND be "inUse == true"
            if (usedAction == levelCards[i].cardAction && levelCards[i].isInUse == true)
            {
                lastCard = levelCards[i];
                lastNumber = recievedNumber;

                if (!levelCards[i].hasArrow)
                {
                    if (levelCards[i].cardAction == Actions.Enable) 
                    { 
                        levelCards[recievedNumber.value - 1].Enable();
                    }
                    else
                        levelCards[i].ExecuteAction(recievedNumber);

                    levelCards[i].Disable(recievedNumber);
                    NextCard(recievedNumber.value);
                    break;
                }
                else
                {
                    if (!levelCards[i].isAIBefore)
                    {
                        if (levelCards[i].cardAction == Actions.Enable)
                        {
                            levelCards[recievedNumber.value - 1].Enable();
                            AICanMoveNow();
                        }
                        else
                            levelCards[i].ExecuteAction(recievedNumber);

                        levelCards[i].Disable(recievedNumber);
                        NextCard(recievedNumber.value);
                        break;
                    }
                    else
                    {
                        // If isAIBefore == true
                        if (levelCards[i].cardAction == Actions.Enable)
                        {
                            AICanMoveNow();
                            levelCards[recievedNumber.value - 1].Enable();
                        }
                        levelCards[i].Disable(recievedNumber);
                        NextCard(recievedNumber.value);
                        break;
                    }
                }
            }
        }
    }

    public void PlayerAfterAction()
    {
        lastCard.ExecuteAction(lastNumber);
    }
}

