using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

namespace SG
{
    public class SaveFileDataWriter
    {
        public string savedDataDirectoryPath = "";
        public string saveFileName = "";

        //Before we create a new save file, we must check to see if one of this scharacter slot already exists
        public bool CheckToSeeIfFileExists() 
        {
            if (File.Exists(Path.Combine(savedDataDirectoryPath, saveFileName))) 
            {
                return true;
            }
            else 
            { 
                return false; 
            }
        }

        //Used to delete character save files
        public void DeleteSaveFile() 
        {
            File.Delete(Path.Combine(savedDataDirectoryPath, saveFileName));
        }

        //Used to create a save file upon starting a new game
        public void CreateNewCharacterSaveFile(CharacterSaveData characterData) 
        {
            //Make a path to save the file (a location on the computer)
            string savePath = Path.Combine(savedDataDirectoryPath, saveFileName);

            try
            {
                //Create the directory the file will be written to , if it does not already exist
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("Creating Save File, At save path: " + savePath);

                //Serialize the c# game data object into json
                string dataToStore = JsonUtility.ToJson(characterData, true);

                //Write the file to our system
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex) 
            {
                Debug.LogError("Error whilst trying to save character data, Game not saved" + savePath + "\n" + ex);
            }
        }

        //Used to load a save file upon Loading a previous game
        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterData = null;

            string loadPath = Path.Combine(savedDataDirectoryPath, saveFileName);

            if (File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    //Deserialize the data from Json back to unity
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception ex)
                {

                }
            }
             return characterData;
        }
    }
}
