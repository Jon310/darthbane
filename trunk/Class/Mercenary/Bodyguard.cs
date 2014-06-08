using Buddy.BehaviorTree;
using DarthBane.Helpers;
using DarthBane.Managers;
using Distance = DarthBane.Helpers.Global.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarthBane.Class.Mercenary
{
    class Bodyguard : RotationBase
    {
        public override SWTorSpec KeySpec
        {
            get { return SWTorSpec.MercenaryBodyguard; }
        }

        public override string Name
        {
            get { return "Bodyguard Healing"; }
        }

        public override string Revision
        {
            get { return ""; }
        }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    PVERotation,
                    Spell.Buff("Combat Support Cylinder"),
                    Spell.Buff("Hunter's Boon"),
                    Rest.HandleRest,
                    Rest.CompanionHandler());
            }
        }

        public override Composite Pulling
        {
            get
            {
                return new PrioritySelector(
                    CloseDistance(Distance.Ranged),

                    //Rotation
                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Unload"),
                    Spell.Cast("Power Shot", ret => Me.ResourceStat >= 70),
                    Spell.Cast("Rapid Shots"));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    HealingManager.AcquireHealTargets,
                    Spell.WaitForCast(),
                    MedPack.UseItem(ret => Me.HealthPercent < 40),
                    
                    Spell.Buff("Determination", ret => Me.IsStunned),
                    Spell.Buff("Supercharged Gas", ret => Me.BuffCount("Supercharge") == 30 
                        && Me.ResourcePercent() <= 80
                        && HealTarget != null && HealTarget.HealthPercent <= 80),
                    Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 70),
                    Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 40),
                    Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),

                    Spell.Cleanse("Cure"),

                    //Buff Kolto Residue
                    new Decorator(ctx => HealTarget != null,
                        Spell.CastOnGround("Kolto Missile", on => HealTarget.Position, ret => HealTarget.HealthPercent < 60 && !HealTarget.HasBuff("Kolto Residue"))),
                    
                    //Free, so use it!
                    Spell.Heal("Emergency Scan", 80),

                    //Important Buffs to take advantage of
                    new Decorator(ctx => Tank != null,
                        Spell.CastOnGround("Kolto Missile", on => Tank.Position, ret => Me.HasBuff("Supercharged Gas") && Me.InCombat && !Tank.HasBuff("Charge Screen"))),
                    Spell.Heal("Rapid Scan", 80, ret => Me.HasBuff("Critical Efficiency")),
                    Spell.HealGround("Kolto Missile", ret => Me.HasBuff("Supercharged Gas")),                    
                    Spell.Heal("Healing Scan", 80, ret => Me.HasBuff("Supercharged Gas")),                    

                    //Buff Tank
                    Spell.Heal("Kolto Shell", on => Tank, 100, ret => Tank != null && Me.InCombat && !Tank.HasBuff("Kolto Shell")),

                    //Single Target Priority
                    Spell.Heal("Healing Scan", 75),
                    Spell.Heal("Rapid Scan", 75),

                    //Filler
                    Spell.Heal("Rapid Shots", 95, ret => HealTarget != null && HealTarget.Name != Me.Name),
                    Spell.Heal("Rapid Shots", on => Tank, 100, ret => Tank != null && Me.InCombat),

                    CloseDistance(Distance.Ranged),

                    //Rotation
                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Unload"),
                    Spell.Cast("Power Shot", ret => Me.ResourceStat >= 70),
                    Spell.Cast("Rapid Shots"));
            }
        }
    }
}
