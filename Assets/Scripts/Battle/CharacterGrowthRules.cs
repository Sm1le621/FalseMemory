using UnityEngine;

[CreateAssetMenu(menuName="FalseMemory/Character Growth Rules")]
public sealed class CharacterGrowthRules : ScriptableObject
{
    [Range(2,99)] public int maximumLevel=99;
    [Min(1)] public int firstLevelExperience=100;
    [Min(0)] public int experienceStep=50;
    [Min(0)] public int healthPerLevel=5, manaPerLevel=2;
    public ItemStatBonus statsPerLevel=new ItemStatBonus{attack=2,defense=1,magicAttack=1,magicDefense=1,speed=1};
    public int MaximumLevel => Mathf.Clamp(maximumLevel,2,99);
    public long TotalForLevel(int level)
    {
        long steps=Mathf.Clamp(level,1,MaximumLevel)-1;
        return steps*System.Math.Max(1,firstLevelExperience)+steps*(steps-1)/2*System.Math.Max(0,experienceStep);
    }
    public int LevelFor(int totalExperience,int currentLevel)
    {
        int level=System.Math.Max(1,currentLevel);
        while(level<MaximumLevel && System.Math.Max(0,totalExperience)>=TotalForLevel(level+1))level++;
        return level;
    }
    public int Remaining(int totalExperience,int level) => level>=MaximumLevel?0:
        (int)System.Math.Min(int.MaxValue,System.Math.Max(0,TotalForLevel(level+1)-System.Math.Max(0,totalExperience)));
    public int Growth(int perLevel,int level) => (int)System.Math.Min(int.MaxValue,
        (long)System.Math.Max(0,perLevel)*(Mathf.Clamp(level,1,MaximumLevel)-1));
    public ItemStatBonus Bonuses(int level) => new ItemStatBonus{
        attack=Growth(statsPerLevel.attack,level),defense=Growth(statsPerLevel.defense,level),
        magicAttack=Growth(statsPerLevel.magicAttack,level),magicDefense=Growth(statsPerLevel.magicDefense,level),speed=Growth(statsPerLevel.speed,level)};
}
