using System;
using System.Linq;
using DilmerGames.Core.Singletons;
using TMPro;
using UnityEngine;

public class ARDebugManager : Singleton<ARDebugManager>
{   
    [SerializeField] private TextMeshProUGUI debugAreaText = null;

    [SerializeField] private bool enableDebug = true;

    [SerializeField] private int maxLines = 8;

    [SerializeField] private bool onlyCriticalMessages = false;

    void OnEnable() 
    {
        debugAreaText.enabled = enableDebug;
        enabled = enableDebug;
    }

    public void LogInfo(string message, bool critical)
    {
        ClearLines();
        // For the full date format please use: $"{DateTime.Now.ToString("yyyy-dd-M HH:mm:ss"
        if (onlyCriticalMessages)
        {
            if (critical) debugAreaText.text += $"{DateTime.Now:HH:mm:ss}: <color=\"white\">{message}</color>\n";
        }
        else
        {
            debugAreaText.text += $"{DateTime.Now:HH:mm:ss}: <color=\"white\">{message}</color>\n";
        }
    }

    public void LogError(string message, bool critical)
    {
        ClearLines();
        if (onlyCriticalMessages)
        {
            if (critical) debugAreaText.text += $"{DateTime.Now:HH:mm:ss}: <color=\"red\">{message}</color>\n";
        }
        else
        {
            debugAreaText.text += $"{DateTime.Now:HH:mm:ss}: <color=\"red\">{message}</color>\n";
        }
    }

    public void LogWarning(string message, bool critical)
    {
        ClearLines();
        if (onlyCriticalMessages)
        {
            if (critical) debugAreaText.text += $"{DateTime.Now:HH:mm:ss}: <color=\"yellow\">{message}</color>\n";
        }
        else
        {
            debugAreaText.text += $"{DateTime.Now:HH:mm:ss}: <color=\"yellow\">{message}</color>\n";
        }
    }
    
    public void LogSuccess(string message, bool critical)
    {
        ClearLines();
        if (onlyCriticalMessages)
        {
            if (critical) debugAreaText.text += $"{DateTime.Now:HH:mm:ss}: <color=\"green\">{message}</color>\n";
        }
        else
        {
            debugAreaText.text += $"{DateTime.Now:HH:mm:ss}: <color=\"green\">{message}</color>\n";
        }
    }
    
    private void ClearLines()
    {
        if(debugAreaText.text.Split('\n').Count() >= maxLines)
        {
            debugAreaText.text = string.Empty;
        }
    }
}