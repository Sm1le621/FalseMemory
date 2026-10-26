using System;

[Flags]
public enum CreatureCategory { None=0,Human=1,Beast=2,Undead=4,Unholy=8 }

public readonly struct BattleDamageResult
{
    public int BaseDamage {get;}
    public int HolyDamage {get;}
    public int Total => (int)Math.Min(int.MaxValue,(long)BaseDamage+HolyDamage);
    public BattleDamageResult(int basis,int holy){BaseDamage=Math.Max(0,basis);HolyDamage=Math.Max(0,holy);}
}

// Pure composition: called once after primary mitigation/critical/Defend, never from a damage event.
public static class BattleDamage
{
    public static BattleDamageResult Compose(int basis,double wisdomBonus,CreatureCategory target,float holyResistance,int intrinsicHoly=0,bool pureHoly=false)
    {
        basis=Math.Max(0,basis);
        bool eligible=(target&(CreatureCategory.Undead|CreatureCategory.Unholy))!=0;
        if(!eligible)return new BattleDamageResult(pureHoly?0:basis,0);
        if(double.IsNaN(wisdomBonus)||double.IsInfinity(wisdomBonus))wisdomBonus=0;
        double resistance=float.IsNaN(holyResistance)||float.IsInfinity(holyResistance)?0:Math.Max(0,Math.Min(1,holyResistance));
        double holy=(pureHoly?basis:basis*Math.Max(0,wisdomBonus))+Math.Max(0,intrinsicHoly);
        return new BattleDamageResult(pureHoly?0:basis,(int)Math.Min(int.MaxValue,Math.Round(holy*(1-resistance),MidpointRounding.AwayFromZero)));
    }
}
