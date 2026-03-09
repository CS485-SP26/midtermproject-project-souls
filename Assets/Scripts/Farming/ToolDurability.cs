using UnityEngine;

public class ToolDurability : MonoBehaviour
{
    public int maxDurability = 50;
    public int currentDurability;
    public int useCost = 5;

    private bool isBroken = false;

    void Start()
    {
        currentDurability = maxDurability;
    }

    public bool UseTool()
    {
        if (isBroken)
        {
            Debug.Log("Tool is Broken");      
            return false;
        }

        if (currentDurability < useCost)
        {
            BreakTool();
            return false;
        }

        currentDurability -= useCost;
        return true;
    }

    void BreakTool()
    {
        isBroken = true;
        Debug.Log("Tool is Broken");
    }

    public void RepairTool()
    {
        currentDurability = maxDurability;
        isBroken = false;
    }
}