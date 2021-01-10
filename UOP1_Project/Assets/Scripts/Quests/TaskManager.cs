﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this script needs to be put on the actor, and takes care of the current task to accomplish.
//the task contains a dialogue and maybe an event.

public class TaskManager : MonoBehaviour
{
	[Header("Data")]
	[SerializeField] private ActorSO _actor=default;
	[SerializeField] private DialogueDataSO _defaultDialogue = default;

	[Header("Listening to channels")]
	[SerializeField] private TaskChannelSO _startTaskEvent = default;
	[SerializeField] private VoidEventChannelSO _endDialogueEvent = default;
	[SerializeField] private DialogueActorChannelSo _interactionEvent = default;
	[SerializeField] private VoidEventChannelSO _winDialogueEvent = default;
	[SerializeField] private VoidEventChannelSO _loseDialogueEvent = default;
	[SerializeField] private TaskChannelSO _endTaskEvent = default;

	[Header("Broadcasting on channels")]
	[SerializeField] private VoidEventChannelSO _checkTaskValidityEvent = default;
	[SerializeField] private DialogueDataChannelSo _startDialogueEvent = default;

	//check if character is actif. An actif character is the character concerned by the task.
	private bool _hasActifTask;
	private TaskSO _currentTask;
	private DialogueDataSO _currentDialogue;

	private void Start()
	{
		if (_endDialogueEvent != null)
		{ _endDialogueEvent.OnEventRaised += EndDialogue; }
		if (_startTaskEvent != null)
		{ _startTaskEvent.OnEventRaised += CheckTaskInvolvment; }
		if (_interactionEvent != null)
		{ _interactionEvent.OnEventRaised += InteractWithCharacter; }
		if (_winDialogueEvent != null)
		{ _winDialogueEvent.OnEventRaised += PlayWinDialogue; }
		if (_loseDialogueEvent != null)
		{ _loseDialogueEvent.OnEventRaised += PlayLoseDialogue; }
		if (_endTaskEvent != null)
		{ _endTaskEvent.OnEventRaised += EndTask; }

	}
	//play default dialogue if no task
	void PlayDefaultDialogue()
	{
		if (_defaultDialogue != null)
		{
			_currentDialogue = _defaultDialogue; 
		    StartDialogue();
		}

	}
	void CheckTaskInvolvment(TaskSO task)
	{
		Debug.Log("check involvment"); 
		if(_actor == task.Actor)
		{
			RegisterTask(task); 
		}

	}
	//register a task
     void RegisterTask(TaskSO task)
	{
		_currentTask = task;
		_hasActifTask = true;
		
	}
	//start a dialogue when interaction
	//some tasks need to be instantanious. And do not need the interact button.
	//when interaction again, restart same dialogue.
	 void InteractWithCharacter(ActorSO actorToInteractWith)
	{
		if (actorToInteractWith == _actor)
		{
			if (_hasActifTask)
			{
				StartTask();

			}
			else
			{
				PlayDefaultDialogue();
			}
		}
	}
	public void InteractWithCharacter()
	{
		if (_hasActifTask)
			{
				StartTask();

			}
			else
			{
				PlayDefaultDialogue();
			}
		
	}
	void StartTask() {
        if(_currentTask!=null)
		if (_currentTask.DialogueBeforeTask != null)
		{
			_currentDialogue = _currentTask.DialogueBeforeTask;
			StartDialogue();
		}
		else
		{
				Debug.LogError("Task without dialogue registring not implemented."); 
		}
	}
	 void StartDialogue()
	{
		if (_startDialogueEvent != null)
		{
			Debug.Log("Start Dialogue ");
			_startDialogueEvent.RaiseEvent(_currentDialogue);
		}


	}
	void PlayLoseDialogue() {
		
		if (_currentTask != null)
			if (_currentTask.LoseDialogue != null)
			{
				Debug.Log("Play lose Dialogue ");
				_currentDialogue = _currentTask.LoseDialogue;
				StartDialogue();
			}
		
	}
	void PlayWinDialogue()
	{
		Debug.Log("Play Win Dialogue" + _currentTask.WinDialogue);
		if (_currentTask != null)
			if (_currentTask.WinDialogue != null)
			{
				_currentDialogue = _currentTask.WinDialogue;
				StartDialogue();
			}

	}
	//End dialogue
	 void EndDialogue()
	{
		
		//depending on the dialogue that ended, do something 
			switch (_currentDialogue.DialogueType)
			{
				case dialogueType.startDialogue:
					//Check the validity of the task
					CheckTaskValidity();
					break;
				case dialogueType.winDialogue:
					//After playing the win dialogue close Dialogue and end Task
					break;
				case dialogueType.loseDialogue:
					//closeDialogue
					//replay start Dialogue if the lose Dialogue ended
					if (_currentTask.DialogueBeforeTask != null)
					{
						_currentDialogue = _currentTask.DialogueBeforeTask;

					}
					break;
				case dialogueType.defaultDialogue:
					//close Dialogue
					//nothing happens if it's the default dialogue
					break;
				default:
					break;
			
		}
		


	}
	void CheckTaskValidity()
	{
		if(_checkTaskValidityEvent!=null)
		{
			_checkTaskValidityEvent.RaiseEvent(); 
		}


	}
	 void EndTask(TaskSO taskToFinish)
	{
		Debug.Log("End Task " + taskToFinish.name); 
		if(taskToFinish==_currentTask)
		    UnregisterTask();
		else
		{
			StartTask(); 
		}

	}
	void EndTask()
	{
	
			UnregisterTask();

	}
	//unregister a task when it ends.
	void UnregisterTask()
	{
		_currentTask = null;
		_hasActifTask = false;
		_currentDialogue = _defaultDialogue; 

		
	}
}
