using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/***
 * 
 * This script functions to find which level the player is currently on in the mind chapter.
 * NOTE: Each level will keep its own world state in the world state manager. This does not keep the state of each world.
 * 
 * **/
public class LevelManager : MonoBehaviour
{
    string levelFilePath = "";

    public string retrieveLevel(string levelName)
    {
        setLevelPath();

        //Go through and set achievement array.
        StreamReader re = new StreamReader(levelFilePath);

        string currentLine = "";
        while ((currentLine = re.ReadLine()) != null)
        {
            //Go through and find the current level and return the progression.
            string[] line = currentLine.Split(":");

            if(line.Length == 4)
            {
                if (string.Equals(line[1], levelName))
                {
                    re.Close();
                    return levelName + " " + line[2];
                }
            }
        }

        re.Close();

        ///Debug.LogWarning("Level " + levelName + " was not found. Return to main menu...");

        return levelName;
    }

    private void createLevelFile(string p)
    {
        //This hard codes all current levels.
        StreamWriter wr = new StreamWriter(p, false);

        //Let's start with finishing the game.
        wr.WriteLine("1:Mind Chapter:1:4");

        wr.Close();
    }

    public void updateLevelFile(string name, int updateOffset)
    {
        setLevelPath();

        //Collect a complete string of this system.
        StreamReader re = new StreamReader(levelFilePath);

        string[] fullFile = re.ReadToEnd().Split("/n");

        re.Close();

        //Update the file.
        for(int i = 0; i < fullFile.Length; i++)
        {
            string[] curLine = fullFile[i].Split(':');

            //Only adjust the level to change.
            if(curLine.Length == 4 && string.Equals(curLine[1], name))
            {
                int newProgress = (int.Parse(curLine[2]) + updateOffset);
                if (newProgress > int.Parse(curLine[3]) || newProgress < 1)
                {
                    newProgress = 1;
                }

                fullFile[i] = curLine[0] + ":" + name + ":" + newProgress + ":" + curLine[3];
            }
        }

        //Save to new file.
        StreamWriter wr = new StreamWriter(levelFilePath, false);

        for(int i = 0; i < fullFile.Length; i++)
        {
            wr.WriteLine(fullFile[i]);
        }

        wr.Close();
    }

    private void setLevelPath()
    {
        levelFilePath = Application.persistentDataPath + " PlayerLevels.txt";

        //Check if file exists. If not, create the file.
        if (!File.Exists(levelFilePath))
        {
            createLevelFile(levelFilePath);
        }
    }
}
