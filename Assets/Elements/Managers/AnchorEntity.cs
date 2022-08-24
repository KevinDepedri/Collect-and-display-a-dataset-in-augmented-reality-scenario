using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorEntity : MonoBehaviour
{
    public List<string> anchorIdList = new List<string>();
    public int photoCounter = 0;
    public List<bool> verticalList = new List<bool>();
    
    // FUNCTION USED TO SAVE THE CLASS AS A JSON FILE
    public string ToJson()
    {
        ARDebugManager.Instance.LogWarning(JsonUtility.ToJson(this), false);
        return JsonUtility.ToJson(this);
    }
    
    // FUNCTION USED TO LOAD THE CLASS FROM A JSON FILE
    public void LoadFromJson(string id)
    {
        JsonUtility.FromJsonOverwrite(id, this);
    }
}
