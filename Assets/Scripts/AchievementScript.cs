using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementScript : MonoBehaviour
{
    [SerializeField]
    private string achievementName;
    [SerializeField]
    private float progressionAddition;

    [SerializeField]
    private bool persistent;

    private AchievementManager manager_;

    private bool complete = false;

    private void Start()
    {
        manager_ = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AchievementManager>();
    }

    public void triggerAchievement()
    {
        if (!complete)
        {
            if(progressionAddition > 0)
            {
                manager_.changeAchievement(achievementName, progressionAddition, persistent);
            } else
            {
                manager_.changeAchievement(achievementName);
            }

            //Ensure this can be reused if it is NOT persistent.
            if (persistent)
            {
                complete = true;
            }
        }
    }
}
