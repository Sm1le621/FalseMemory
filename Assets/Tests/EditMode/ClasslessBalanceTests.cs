using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public sealed class ClasslessBalanceTests
{
    private const int Samples=64;
    private sealed class Build
    {
        public string name,weapon,armor,ability;
        public CharacterAttributeValues values;
        public Build(string name,string weapon,string armor,string ability,params int[] gains)
        {
            this.name=name;this.weapon=weapon;this.armor=armor;this.ability=ability;values=CharacterAttributeValues.Uniform(10);
            for(int i=0;i<gains.Length;i+=2){var stat=(CharacterAttribute)gains[i];values.Set(stat,10+gains[i+1]);}
        }
    }
    private sealed class Foe
    {
        public string name;public int hp,attack,defense,magicDefense,speed;public CreatureCategory category;public float resistance,magicChance;
        public Foe(string name,int hp,int attack,int defense,int magicDefense,int speed,CreatureCategory category,float resistance=0,float magicChance=0)
        {this.name=name;this.hp=hp;this.attack=attack;this.defense=defense;this.magicDefense=magicDefense;this.speed=speed;this.category=category;this.resistance=resistance;this.magicChance=magicChance;}
    }
    [Serializable] public sealed class Metric
    {
        public string build,enemy,weapon,armor;public int budget,hp,mp,physicalAttack,magicAttack,physicalDefense,magicDefense,initiative,basicHit,criticalHit,fireHit;
        public float criticalChance,evasion,enemyMagicChance;public double holyBonus,winPercent,meanDamagePerAttack,meanRounds,meanWinRounds,meanManaSpent,meanRemainingHealth;
    }
    [Serializable] private sealed class Report {public int samplesPerPair=Samples;public string method="Seeded formula simulation; fixed level-one baseline; 60 allocated points; no items; not real-time Play Mode.";public List<Metric> rows=new List<Metric>();}
    private sealed class Stats
    {
        public Build build;public ItemData weapon;public BattleSpell preferred;public int hp,mp,atk,matk,def,mdef,speed;public float crit,miss,evade;public double holy;
    }
    private static Build[] Builds()=>new[]{
        new Build("STR","Greatsword","HeavyPlateArmor","PowerStrike",0,40,3,20),
        new Build("DEX","Rapier","AssassinArmor","QuickStrike",2,30,1,20,4,10),
        new Build("MAG","MagicStaff","MageRobes","Fire",8,30,5,20,6,10),
        new Build("WIS","SacredBlade","HolyArmor","SacredStrike",6,40,0,10,4,10),
        new Build("STR_MAG","Longsword","BattlemageArmor","Fire",0,20,8,20,5,10,4,10),
        new Build("END_VIT","Longsword","KnightArmor","",3,30,4,30)};
    private static Foe[] Foes()=>new[]{
        new Foe("Skeleton fixture",240,28,20,12,12,CreatureCategory.Undead,0,.25f),
        new Foe("Zombie fixture",320,32,25,10,6,CreatureCategory.Undead,.15f),
        new Foe("Human fixture",260,30,24,18,15,CreatureCategory.Human,0,.5f),
        new Foe("Beast fixture",220,34,12,14,22,CreatureCategory.Beast),
        new Foe("Unholy fixture",300,32,18,24,14,CreatureCategory.Unholy,.35f,.65f)};
    private static T Load<T>(string path) where T:UnityEngine.Object
    {var asset=AssetDatabase.LoadAssetAtPath<T>(path);Assert.That(asset,Is.Not.Null,path);return asset;}
    private static Stats Calculate(Build build,CharacterAttributeRules rules)
    {
        var w=Load<ItemData>("Assets/Settings/Inventory/"+build.weapon+".asset");var a=Load<ItemData>("Assets/Settings/Inventory/"+build.armor+".asset");
        Assert.That(w.weapon.MeetsRequirements(build.values),Is.True,build.name);Assert.That(a.armor.MeetsRequirements(build.values),Is.True,build.name);
        var armor=a.armor.Bonuses(build.values,rules);bool magic=w.weapon.damageType==WeaponDamageType.Magic;
        var stats=new Stats{build=build,weapon=w,hp=100+rules.Adjustment(build.values,DerivedAttributeStat.MaximumHealth),mp=40+rules.Adjustment(build.values,DerivedAttributeStat.MaximumMana),
            atk=magic?15+rules.Adjustment(build.values,DerivedAttributeStat.Attack):w.weapon.Damage(build.values,rules),
            matk=magic?w.weapon.Damage(build.values,rules):12+rules.Adjustment(build.values,DerivedAttributeStat.MagicAttack),
            def=8+rules.Adjustment(build.values,DerivedAttributeStat.Defense)+armor.defense,mdef=8+rules.Adjustment(build.values,DerivedAttributeStat.MagicDefense)+armor.magicDefense,
            speed=10+rules.Adjustment(build.values,DerivedAttributeStat.Speed),crit=Mathf.Clamp(.1f+rules.CriticalBonus(build.values),0,rules.maximumCritical),
            miss=Mathf.Clamp01(.05f-rules.AccuracyBonus(build.values)),evade=Mathf.Clamp(rules.Evasion(build.values)+armor.evasion,0,rules.maximumEvasion),holy=rules.HolyBonus(build.values)};
        if(build.ability!="")stats.preferred=Load<BattleSpell>("Assets/Settings/Battle/Spells/"+build.ability+".asset");
        if(stats.preferred!=null && build.ability!="Fire")Assert.That(stats.preferred.MeetsRequirements(1,build.values),Is.True);
        return stats;
    }
    private static int Hit(Stats p,Foe foe,BattleRules battle,bool defending,IBattleRandom random,BattleSpell ability=null)
    {
        int basis,intrinsic=0;
        if(ability!=null && ability.effect==BattleEffect.MagicDamage)
            basis=BattleCalculator.MagicalDamage(p.matk,foe.magicDefense,ability.power,battle);
        else
        {
            bool magic=ability==null && p.weapon.weapon.damageType==WeaponDamageType.Magic;
            int power=ability==null?(magic?p.matk:p.atk):(int)Math.Round((p.atk+ability.power)*ability.attackMultiplier,MidpointRounding.AwayFromZero);
            var result=BattleCalculator.ResolvePhysical(power,magic?foe.magicDefense:foe.defense,defending,battle,p.crit,
                Mathf.Clamp01(p.miss+(ability==null?0:-ability.accuracyBonus)),random);
            if(result.Missed)return 0;basis=result.Damage;
            intrinsic=ability!=null?ability.holyPower:p.weapon.weapon.HolyDamage(p.build.values,Load<CharacterAttributeRules>("Assets/Settings/Progression/DefaultCharacterAttributes.asset"));
        }
        return BattleDamage.Compose(basis,p.holy,foe.category,foe.resistance,intrinsic).Total;
    }
    [Test] public void EqualBudgetBuildsProduceReproducibleCombatAndResourceReport()
    {
        var rules=Load<CharacterAttributeRules>("Assets/Settings/Progression/DefaultCharacterAttributes.asset");var battle=Load<BattleRules>("Assets/Settings/Battle/DefaultBattleRules.asset");
        var heal=Load<BattleSpell>("Assets/Settings/Battle/Spells/Heal.asset");var fire=Load<BattleSpell>("Assets/Settings/Battle/Spells/Fire.asset");var report=new Report();
        foreach(var build in Builds())
        {
            int budget=Enumerable.Range(0,10).Sum(i=>build.values.Get((CharacterAttribute)i)-10);Assert.That(budget,Is.EqualTo(60));
            var p=Calculate(build,rules);Assert.That(p.hp,Is.GreaterThan(0));Assert.That(p.mp,Is.GreaterThan(0));
            foreach(var foe in Foes())
            {
                double damageSum=0,attacks=0,roundSum=0,winRoundSum=0,mpSum=0,hpSum=0;int wins=0;
                for(int sample=0;sample<Samples;sample++)
                {
                    var random=new SeededBattleRandom(20261026+sample);int hp=p.hp,mp=p.mp,enemyHP=foe.hp,round=0;bool enemyDefending=false;
                    Action player=()=>{
                        if(hp<p.hp*.35 && mp>=rules.ManaCost(heal.mpCost,build.values))
                        {mp-=rules.ManaCost(heal.mpCost,build.values);hp=Math.Min(p.hp,hp+heal.power+rules.HealingBonus(build.values));return;}
                        BattleSpell ability=p.preferred!=null && mp>=rules.ManaCost(p.preferred.mpCost,build.values)?p.preferred:null;
                        if(ability!=null)mp-=rules.ManaCost(ability.mpCost,build.values);
                        int dealt=Hit(p,foe,battle,enemyDefending,random,ability);enemyHP=Math.Max(0,enemyHP-dealt);damageSum+=dealt;attacks++;
                    };
                    Action enemy=()=>{
                        enemyDefending=false;if(random.Next01()<.2f){enemyDefending=true;return;}
                        int damage=random.Next01()<foe.magicChance?BattleCalculator.MagicalDamage(foe.attack,p.mdef,10,battle):
                            BattleCalculator.ResolvePhysical(foe.attack,p.def,false,battle,.08f,.05f+p.evade,random).Damage;hp=Math.Max(0,hp-damage);
                    };
                    while(hp>0 && enemyHP>0 && round<100)
                    {
                        round++;if(p.speed>=foe.speed){player();if(enemyHP>0)enemy();}else{enemy();if(hp>0)player();}
                        Assert.That(mp,Is.InRange(0,p.mp));Assert.That(hp,Is.InRange(0,p.hp));
                    }
                    if(enemyHP==0){wins++;winRoundSum+=round;}roundSum+=round;mpSum+=p.mp-mp;hpSum+=hp;
                }
                var ordinary=BattleCalculator.ResolvePhysical(p.weapon.weapon.damageType==WeaponDamageType.Magic?p.matk:p.atk,
                    p.weapon.weapon.damageType==WeaponDamageType.Magic?foe.magicDefense:foe.defense,false,battle,0,0,new SeededBattleRandom(1));
                var critical=BattleCalculator.ResolvePhysical(p.weapon.weapon.damageType==WeaponDamageType.Magic?p.matk:p.atk,
                    p.weapon.weapon.damageType==WeaponDamageType.Magic?foe.magicDefense:foe.defense,false,battle,1,0,new SeededBattleRandom(1));
                int holy=p.weapon.weapon.HolyDamage(build.values,rules);
                report.rows.Add(new Metric{build=build.name,enemy=foe.name,weapon=build.weapon,armor=build.armor,budget=budget,hp=p.hp,mp=p.mp,
                    physicalAttack=p.atk,magicAttack=p.matk,physicalDefense=p.def,magicDefense=p.mdef,initiative=p.speed,criticalChance=p.crit,evasion=p.evade,holyBonus=p.holy,
                    basicHit=BattleDamage.Compose(ordinary.Damage,p.holy,foe.category,foe.resistance,holy).Total,criticalHit=BattleDamage.Compose(critical.Damage,p.holy,foe.category,foe.resistance,holy).Total,
                    fireHit=BattleDamage.Compose(BattleCalculator.MagicalDamage(p.matk,foe.magicDefense,fire.power,battle),p.holy,foe.category,foe.resistance).Total,
                    enemyMagicChance=foe.magicChance,winPercent=100d*wins/Samples,meanDamagePerAttack=damageSum/Math.Max(1,attacks),meanRounds=roundSum/Samples,meanWinRounds=wins==0?0:winRoundSum/wins,meanManaSpent=mpSum/Samples,meanRemainingHealth=hpSum/Samples});
            }
        }
        Assert.That(report.rows.Count,Is.EqualTo(30));Assert.That(report.rows.Where(r=>r.build=="MAG").First().mp,Is.GreaterThan(report.rows.First().mp));
        Assert.That(report.rows.Where(r=>r.build=="DEX").First().initiative,Is.GreaterThan(report.rows.First().initiative));
        Assert.That(report.rows.Where(r=>r.build=="END_VIT").First().hp,Is.GreaterThan(report.rows.First().hp));
        Directory.CreateDirectory("Temp/ClasslessBalance");File.WriteAllText("Temp/ClasslessBalance/Stage57.json",JsonUtility.ToJson(report,true));
        TestContext.WriteLine("30 pairs ×64 seeded simulations exported to Temp/ClasslessBalance/Stage57.json. Not a real-time playthrough.");
    }
    [Test] public void EveryArmorSupportsIndependentRemovalAndEveryGradeIsFiniteAtAllCaps()
    {
        var rules=Load<CharacterAttributeRules>("Assets/Settings/Progression/DefaultCharacterAttributes.asset");
        foreach(var build in Builds())
        {
            var armor=Load<ItemData>("Assets/Settings/Inventory/"+build.armor+".asset");var bag=new InventoryState();var gear=new EquipmentState();bag.TryAdd(armor,1);
            Assert.That(gear.TryEquip(bag,0,EquipmentSlot.Chest,1),Is.True);var before=gear.ScaledBonuses(build.values,rules);
            Assert.That(gear.ScaledBonuses(build.values,rules).defense,Is.EqualTo(before.defense));Assert.That(gear.TryUnequipTo(bag,EquipmentSlot.Chest,0,1),Is.True);
            Assert.That(gear.ScaledBonuses(build.values,rules).defense,Is.Zero);
        }
        foreach(ScalingGrade grade in Enum.GetValues(typeof(ScalingGrade)))for(int i=0;i<10;i++)
        {
            var stat=(CharacterAttribute)i;
            foreach(int cap in rules.Curve(stat).thresholds)foreach(int value in new[]{cap-1,cap,cap+1,99,120})
            {var values=CharacterAttributeValues.Uniform(value);int result=WeaponScalingData.Scale(20,values,new[]{new AttributeScalingEntry{attribute=stat,grade=grade}},rules);Assert.That(result,Is.InRange(20,50));}
        }
    }
}
