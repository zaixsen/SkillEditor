using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloderName
{
    Prefab, Model
}

public enum SkillRange
{
    Single, All, Range
}

public class SkillData
{
    public FloderName floderName;
    public int id;
    public string name;
    public string description;
    public string iconPath;
    public string modelPath;
    public int audioId;
    public int animationId;
    public TriggerType triggerType;
    public SkillRange skillRange;
    public List<BuffData> buffs = new List<BuffData>();
}
public enum TriggerType
{
    ClodDown, UseCount
}
public class BuffData
{
    public int id;
    public string name;
    public string description;
    public string modelPath;
    public float damage;
    public float recaver;  //÷Œ¡∆
    public float triggerTime;  
    public float Add;//‘ˆ“Ê
}