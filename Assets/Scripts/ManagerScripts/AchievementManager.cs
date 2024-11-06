using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public struct achievement
    {
        public int achievementId;
        public string achievementName;
        public string subText;
        public float progression;
        public int target;
        public bool isGotten;
    }

    List<achievement> achievements;

    string path = "";

    [SerializeField]
    GameObject listItem;
    [SerializeField]
    GameObject contentManager;

    // Start is called before the first frame update
    public void setAchievements()
    {
        path = Application.persistentDataPath + "PlayerAchievements.txt";

        achievements = new List<achievement>();

        //Check if file exists. If not, create the file.
        if (!File.Exists(path))
        {
            createAchievementFile(path);
        }

        //Go through and set achievement array.
        StreamReader re = new StreamReader(path);

        string currentLine = "";
        while((currentLine = re.ReadLine()) != null)
        {
            //Go through and add the current line to the achievements list.
            string[] line = currentLine.Split(":");

            achievement a = new achievement();

            a.achievementId = int.Parse(line[0]);
            a.achievementName = line[1];
            a.subText = line[2];
            a.progression = float.Parse(line[3]);
            a.target = int.Parse(line[4]);
            a.isGotten = line[5] == "True";


            //Get the last line, which shows the true or false of the achievement.
            achievements.Add(a);
        }

        re.Close();
    }

    public void changeAchievement(string name)
    {
        for(int i = 0; i < achievements.Count; i++)
        {
            if(string.Equals(name, achievements[i].achievementName))
            {
                //Only make a change if achievement was false.
                if (!achievements[i].isGotten)
                {
                    //Achievement has been triggered. So set to true.
                    achievement a = achievements[i];

                    a.isGotten = true;
                    //Remove and replace at achievements.
                    achievements.RemoveAt(i);
                    achievements.Insert(i, a);
                    updateAchievementFile();
                }
            }
        }
    }

    public void changeAchievement(string name, float progress, bool persistant)
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            if (string.Equals(name, achievements[i].achievementName))
            {
                //Only make a change if achievement was false.
                if (!achievements[i].isGotten)
                {
                    //Achievement has been triggered. So set to true.
                    achievement a = achievements[i];

                    //Test for progression.
                    if (a.target > 0)
                    {
                        if (a.progression < a.target)
                        {
                            a.progression += progress;
                        }

                        if(a.progression >= a.target)
                        {
                            a.isGotten = true;
                        }
                    }

                    //Remove and replace at achievements.
                    achievements.RemoveAt(i);
                    achievements.Insert(i, a);

                    if (persistant)
                    {
                        updateAchievementFile();
                    } else
                    {
                        if (a.isGotten)
                        {
                            updateAchievementFile();
                        }
                    }
                }
            }
        }
    }

    //A function to create a file used for achievements.
    void createAchievementFile(string p)
    {
        //This hard codes all current achievements.
        StreamWriter wr = new StreamWriter(p, false);

        //Let's start with finishing the game.
        wr.WriteLine("1:Escape?:Get away from the cabin:0:0:false");
        wr.WriteLine("2:Just Chillin':Find a cosy place to chill for a while:0:0:false");
        wr.WriteLine("3:Good listener:Follow the rules:0:0:false");
        wr.WriteLine("4:A taste for death:Die 5 times:0:5:false");
        wr.WriteLine("5:Cleaner:Clean up after yourself:0:10:false");

        wr.Close();
    }

    //Update the file that keeps achievement data.
    void updateAchievementFile()
    {
        StreamWriter wr = new StreamWriter(path, false);

        foreach(achievement a in achievements)
        {
            wr.WriteLine(a.achievementId + ":" + a.achievementName + ":" + a.subText + ":" + a.progression + ":" + a.target + ":" + a.isGotten);

        }

        wr.Close();
    }

    //Change a menu list to fill with the given achievements
    //Set them to be locked or unlocked.
    public void updateAchievementMenu()
    {
        //Only run if an achievement menu is added.
        if (listItem && contentManager)
        {
            for(int i = 2; i < contentManager.transform.childCount; i++)
            {
                Destroy(contentManager.transform.GetChild(i).gameObject);
            }

            foreach(achievement a in achievements)
            {
                GameObject currentItem = Instantiate(listItem, contentManager.transform);

                //Get the text files in this item.
                TextMeshProUGUI[] txt = currentItem.GetComponentsInChildren<TextMeshProUGUI>();

                txt[0].text = a.achievementName;
                txt[1].text = a.subText;

                //Update boolean.
                if (a.isGotten)
                {
                    currentItem.transform.GetChild(1).gameObject.SetActive(true);
                    currentItem.transform.GetChild(2).gameObject.SetActive(false);
                } else
                {
                    currentItem.transform.GetChild(2).gameObject.SetActive(true);
                    currentItem.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }
}
