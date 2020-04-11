using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalOrientedBehavior : MonoBehaviour
{
    Goal[] initialGoals;
    Action[] detailedActionArray;

    private int numberOfTimeChanges = 0;
    public float timeIncrementAmount = 3f;

    public Slider hungerSlider;
    public Slider tiredSlider;
    public Slider bladderSlider;
    public Text hungerCounter;
    public Text tiredCounter;
    public Text bladderCounter;
    public Text timeCount;
    public Text actionText;

    private void Start()
    {
        Behave();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoAction();
        }

        //Update slider values to reflect prescience
        hungerSlider.value = initialGoals[0].value;
        tiredSlider.value = initialGoals[1].value;
        bladderSlider.value = initialGoals[2].value;

        //Update slider text
        hungerCounter.text = hungerSlider.value.ToString();
        tiredCounter.text = tiredSlider.value.ToString();
        bladderCounter.text = bladderSlider.value.ToString();
    }

    void DoAction()
    {
        //Get action to match most pressing goal
        Action bestChoice = chooseAction(detailedActionArray, initialGoals);
        actionText.text = "You chose to " + bestChoice.name;
        
        for (int i = 0; i < initialGoals.Length; i++)
        {
            initialGoals[i].value += bestChoice.getGoalChange(initialGoals[i]);
        }
        //PrintGoalStatus();
    }

    Action chooseAction(Action[] actions, Goal[] goals)
    {
        //Find action leading to lowest discontentment
        Action bestAction = null;
        float bestValue = Mathf.Infinity;

        foreach (Action action in actions)
        {
            float thisValue = discontentment(action, goals);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = action;
                //Debug.Log("Better discontent found in " + action.name);
            }
        }
        return bestAction;
    }

    float discontentment(Action action, Goal[] goals)
    {
        float discontentment = 0;

        foreach (Goal goal in goals)
        {
            float newValue = goal.value + action.getGoalChange(goal);
            //Debug.Log("newValue for " + goal.name + " is " + newValue);

            //newValue += action.getDuration() * goal.getChange();
            discontentment += goal.getDiscontentment(newValue);
        }
        Debug.Log("Discontentment for " + action.name + " is " + discontentment);
        return discontentment;
    }

    void Behave()
    {
        //Set up inital goals to work toward
        initialGoals = new Goal[3];
        //Fill this array with new goals
        initialGoals[0] = new Goal("educate", 50f, 2f);
        initialGoals[1] = new Goal("entertain", 40f, 1f);
        initialGoals[2] = new Goal("outside", 30f, 2f);

        //Define my available actions
        Action eatSnackAction = new Action("Stare at AI Homework");
        Action sleepInBedAction = new Action("Look at phone");
        Action goToBathroomAction = new Action("Go outside");

        //Fill array with actions
        detailedActionArray = new Action[3];
        //Initialize Actions
        detailedActionArray[0] = eatSnackAction;
        detailedActionArray[1] = sleepInBedAction;
        detailedActionArray[2] = goToBathroomAction;
        //Set up related goals and weights
        //Snack relations
        detailedActionArray[0].goals.Add(new Goal("educate", -2f));
        detailedActionArray[0].goals.Add(new Goal("entertain", 1f));
        detailedActionArray[0].goals.Add(new Goal("outside", 2f));
        //Sleep relations
        detailedActionArray[1].goals.Add(new Goal("entertain", -2f));
        detailedActionArray[1].goals.Add(new Goal("educate", 1f));
        //Bathroom relations
        detailedActionArray[2].goals.Add(new Goal("outside", -2f));
        detailedActionArray[2].goals.Add(new Goal("educate", 1f));

        PrintGoalStatus();

        //Now represent the passing of time with a function that repeats every second
        InvokeRepeating("PassTime", 0f, timeIncrementAmount);
    }

    void PassTime()
    {
        //Go through the initial goals and add to their value to update their importance
        for (int i = 0; i < initialGoals.Length; i++)
        {
            //updates value internally
            initialGoals[i].change();
        }
        numberOfTimeChanges++;
        timeCount.text = numberOfTimeChanges.ToString();
        //Debug.Log("Time Has Passed for the " + numberOfTimeChanges + " time");
    }

    void PrintGoalStatus()
    {
        Debug.Log("Goals Update: ");
        for (int i = 0; i < initialGoals.Length; i++)
        {
            Debug.Log(initialGoals[i].name + " has an urgency value of: " + initialGoals[i].value);
        }
    }
}
