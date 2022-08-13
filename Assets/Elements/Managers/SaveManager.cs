using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;
using System;
using System.IO;

public class SaveManager : Singleton<SaveManager>
{
    public bool WriteToFile(string fileName, string fileContents)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            File.WriteAllText(fullPath, fileContents);
            //ARDebugManager.Instance.LogInfo($"SUCCESSFULLY WRITTEN {fileName} to {fullPath}");
            return true;
        }
        catch (Exception e)
        {
            ARDebugManager.Instance.LogError($"FAILED TO WRITE TO {fullPath} WITH EXCEPTION {e}", false);
            return false;
        }
    }
    
    public bool LoadFromFile(string fileName, out string result)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            result = File.ReadAllText(fullPath);
            //ARDebugManager.Instance.LogInfo($"SUCCESSFULLY READ {fileName} from {fullPath}, value: {result}");
            return true;
        }
        catch (Exception e)
        {
            ARDebugManager.Instance.LogError($"FAILED TO READ FROM {fullPath} WITH EXCEPTION {e}", false);
            result = "";
            return false;
        }
    }
    
}
