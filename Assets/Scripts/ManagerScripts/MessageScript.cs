using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageScript : MonoBehaviour
{
    //Script lists.
    List<eventClass> eventMessages;
    List<eventClass> storyEvents;

    [SerializeField]
    //A queue for all current messages that are set.
    List<eventClass> messageQueue;

    [SerializeField]
    List<string> messageQueueMessages;

    [SerializeField]
    int endingMessageId;
    [SerializeField]
    int deathMessageId;
    [SerializeField]
    int lastMessage;

    //Runs the messages.
    MenuManager promptManager;

    bool isUsingFirst;


    // Start is called before the first frame update
    void Start()
    {
        //Setup.
        promptManager = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MenuManager>();
        //lastMessage = -1;

        if(eventMessages == null || storyEvents == null)
        {
            setLists();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(messageQueue.Count);
        //Run the messages if a queue is present.
        if(!promptManager.isRunningMessage() && messageQueue.Count > 0)
        {
            //If this is the first update since populating the 
            if (isUsingFirst)
            {
                isUsingFirst = false;
            } else
            {
                //Remove from Queue so the next one can be played when this one is done.
                messageQueue.RemoveAt(0);
                if (messageQueueMessages.Count > 0)
                {
                    messageQueueMessages.RemoveAt(0);
                }
            }

            if(messageQueue.Count > 0 && messageQueue[0].getMessage() != "")
            {
                //Run the next message in the prompt manager.
                promptManager.updateText("MessagePrompt", messageQueue[0].getMessage(), messageQueue[0].getEventTime());

                //Ensure last message is only set if using a story message.
                if(messageQueue[0].messageType == 0 && lastMessage != endingMessageId)
                {
                    lastMessage = messageQueue[0].getID();
                }
            }
        }
    }

    //This will return if the queue is empty AND the ending message has been provided.
    public bool isMessagesFinished()
    {
        if(messageQueue == null)
        {
            messageQueue = new List<eventClass>();
        }
        return (messageQueue.Count > 0 ? false : true) && (lastMessage == endingMessageId || lastMessage == deathMessageId) && !promptManager.isRunningMessage();
    }

    public void clearMessageQueue()
    {

        messageQueue.Clear();
        //Even current message must be removed.
        promptManager.setisRunningMessage(false);
    }

    public void setLoadingOnPrompts()
    {
        promptManager.setToMenuGroup("LoadBlack");

        //Lock the menu so that the start menu cannot made on when loading black is on.
        promptManager.lockMenu = true;
    }

    public void setMessageOverride(int ind, int num)
    {
        storyEvents[ind].setMessageOverride(num);

        //Set overrides for the entire chain.
        //Add all chained messages to queue.
        int curStep = ind;

        while (storyEvents[curStep].nextMessage != -1)
        {
            curStep = storyEvents[curStep].nextMessage;

            storyEvents[curStep].setMessageOverride(num);
        }
    }

    public void resetLoadingOnPrompts()
    {
        promptManager.lockMenu = false;
        promptManager.setToMenuGroup("Stylus");
    }

    public int getDeathMessageID()
    {
        return deathMessageId;
    }

    public int getEndingMessageID()
    {
        return endingMessageId;
    }

    public int getLastStoryMessage()
    {
        return lastMessage;
    }

    //This populates the message queue with messages.
    //Call this when an event or story point occurs.
    public void runEventMessage(int id, EventScript.eventType t)
    {
        //Is using the first item. This allows you to not kick the message from queue until it is finished being used.
        isUsingFirst = true;

        //Populate the queue with a provided message / message chain.
        if(t == EventScript.eventType.storyEvent)
        {
            if (!string.Equals(storyEvents[id].getMessage(), ""))
            {
                int curStep = id;

                messageQueue.Add(storyEvents[curStep]);
                messageQueueMessages.Add(storyEvents[curStep].getMessage());

                //Add all chained messages to queue.
                while (storyEvents[curStep].nextMessage != -1)
                {
                    curStep = storyEvents[curStep].nextMessage;

                    messageQueue.Add(storyEvents[curStep]);
                    messageQueueMessages.Add(storyEvents[curStep].getMessage());
                }
            }
        } else if (t == EventScript.eventType.optionalEvent)
        {
            int curStep = id;

            //Run this event as an interruption event.
            if (messageQueue.Count > 0)
            {
                //If inserting, then assume this needs to be played NOW.
                promptManager.setisRunningMessage(false);

                messageQueue.Insert(0, eventMessages[curStep]);
                messageQueueMessages.Insert(0, eventMessages[curStep].getMessage());

                int i = 1;

                //Add all chained messages to queue.
                while (eventMessages[curStep].nextMessage != -1)
                {
                    curStep = eventMessages[curStep].nextMessage;

                    messageQueue.Insert(i, eventMessages[curStep]);
                    messageQueueMessages.Insert(i, eventMessages[curStep].getMessage());
                    i++;
                }
            } else
            {
                messageQueue.Add(eventMessages[curStep]);
                messageQueueMessages.Add(eventMessages[curStep].getMessage());

                //Add all chained messages to queue.
                while (eventMessages[curStep].nextMessage != -1)
                {
                    curStep = eventMessages[curStep].nextMessage;

                    messageQueue.Add(eventMessages[curStep]);
                    messageQueueMessages.Add(eventMessages[curStep].getMessage());
                }
            }
        }
    }

    public void setMessageQueue(Vector2[] messages, int initialPlayed)
    {
        if (eventMessages == null || storyEvents == null)
        {
            setLists();
        }

        //Get messages from save file and populate the queue.
        messageQueue = new List<eventClass>();

        isUsingFirst = false;

        for (int i = 0; i < messages.Length; i++)
        {
            if(messages[i].y == 0)
            {
                messageQueue.Add(storyEvents[(int)messages[i].x]);
                messageQueueMessages.Add(storyEvents[(int)messages[i].x].getMessage());
            } else if (messages[i].y == 1)
            {
                messageQueue.Add(eventMessages[(int)messages[i].x]);
                messageQueueMessages.Add(eventMessages[(int)messages[i].x].getMessage());
            }
        }

        if (initialPlayed <= 0)
        {
            //If initial is played, do nothing. If not, then play the initial.
            runEventMessage(1, EventScript.eventType.storyEvent);
        }
    }

    public Vector2[] getMessageQueue()
    {
        Vector2[] returnArr = new Vector2[1];
        if (messageQueue != null)
        {
            returnArr = new Vector2[messageQueue.Count];
            for (int i = 0; i < returnArr.Length; i++)
            {
                returnArr[i] = new Vector2(messageQueue[i].getID(), messageQueue[i].messageType);
            }
        }

        return returnArr;
    }

    private void setLists()
    {
        //Populate story and optional events.
        TextAsset ass = Resources.Load<TextAsset>("Dialogue_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        string eventResources = ass.text;
        string[] sections = eventResources.Split('\n');
        storyEvents = new List<eventClass>();
        int storyBeat = -1;

        for (int i = 0; i < sections.Length; i++)
        {
            string[] line = sections[i].Split(';');

            //Check if story beat is the same as the line before.
            if (storyBeat != int.Parse(line[0]))
            {
                //If not the same story beat, then add message to new beat.
                storyBeat = int.Parse(line[0]);

                if (line.Length == 4)
                {
                    storyEvents.Add(new eventClass(int.Parse(line[0]), float.Parse(line[2]), 0, int.Parse(line[3]), 0));
                }
                else
                {
                    storyEvents.Add(new eventClass(int.Parse(line[0]), float.Parse(line[2]), 0, 0));
                }

                storyEvents[storyEvents.Count - 1].addMessage(line[1]);
            }
            else
            {
                //Find the last story event made and add the next message.
                storyEvents[storyEvents.Count - 1].addMessage(line[1]);
            }
        }

        ass = Resources.Load<TextAsset>("OptionalDialogue_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        eventResources = ass.text;
        sections = eventResources.Split('\n');
        eventMessages = new List<eventClass>();
        storyBeat = -1;

        for (int i = 0; i < sections.Length; i++)
        {
            string[] line = sections[i].Split(';');

            //Check if story beat is the same as the line before.
            if (storyBeat != int.Parse(line[0]))
            {
                //If not the same story beat, then add message to new beat.
                storyBeat = int.Parse(line[0]);

                if (line.Length == 4)
                {
                    eventMessages.Add(new eventClass(int.Parse(line[0]), float.Parse(line[2]), 0, int.Parse(line[3]), 1));
                }
                else
                {
                    eventMessages.Add(new eventClass(int.Parse(line[0]), float.Parse(line[2]), 0, 1));
                }

                eventMessages[eventMessages.Count - 1].addMessage(line[1]);
            }
            else
            {
                //Find the last story event made and add the next message.
                eventMessages[eventMessages.Count - 1].addMessage(line[1]);
            }
        }
    }
}

public class eventClass
{
    int eventID;
    List<string> messages;
    float eventTime;
    int messageOverride;
    public int messageType { get; private set; }

    //Have the ability to change to the next message.
    public int nextMessage { get; private set; }

    public eventClass(int i, float f, int m, int t)
    {
        eventID = i;
        eventTime = f;
        messageOverride = m;
        messages = new List<string>();
        nextMessage = -1;
        messageType = t;
    }

    public eventClass(int i, float f, int m, int n, int t)
    {
        eventID = i;
        eventTime = f;
        messageOverride = m;
        messages = new List<string>();
        nextMessage = n;
        messageType = t;
    }

    public void addMessage(string s)
    {
        messages.Add(s);
    }

    public int getID()
    {
        return eventID;
    }

    public float getEventTime()
    {
        return eventTime;
    }

    public string getMessage()
    {
        if (messageOverride < messages.Count && messageOverride >= 0)
        {
            return messages[messageOverride];
        }

        //Say default dialogue.
        return messages[0];
    }

    public int getMessageOverride()
    {
        return messageOverride;
    }

    public void setMessageOverride(int i)
    {
        messageOverride = i;
    }
}
