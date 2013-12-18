using Distance = DarthBane.Helpers.Global.Distance;
using System;
using Buddy.BehaviorTree;
using DarthBane.Helpers;
using DarthBane.Managers;

namespace DarthBane.Class.Commando
{
    class CombatMedic : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.CommandoCombatMedic; } }
        public override string Name { get { return "Medic"; } }
        public override string Revision { get { return ""; } }

        public override Buddy.BehaviorTree.Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Fortification"),
                    Spell.Buff("Combat Support Cell"),
                    Rest.HandleRest,
                    Rest.CompanionHandler());
            }
        }

        public override Buddy.BehaviorTree.Composite Pulling
        {
            get { throw new NotImplementedException(); }
        }

        public override Buddy.BehaviorTree.Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    HealingManager.AcquireHealTargets,
                    Spell.WaitForCast(),
                    
                    Spell.Buff("Tenacity"),
                    Spell.Buff("Supercharge Cells", ret => Me.ResourcePercent() >= 20 && HealTarget != null && HealTarget.HealthPercent <= 80),
                    Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 60),
                    Spell.Buff("Recharge Cells", ret => Me.ResourceStat <= 50),
                    Spell.Cast("Tech Override", ret => Tank != null && Tank.HealthPercent <= 50),

                    new Decorator(ret => Me.HasBuff("Supercharge Cells"),
                        new PrioritySelector(
                            new Decorator(ctx => Tank != null,
                                Spell.CastOnGround("Kolto Bomb", on => Tank.Position, ret => !Tank.HasBuff("Kolto Residue"))),
                            Spell.Heal("Bacta Infusion", 60),
                            Spell.Heal("Advanced Medical Probe", 85))),

                    Spell.Cleanse("Field Aid"),
                    Spell.Heal("Trauma Probe", on => Tank, 100, ret => Tank != null && Tank.BuffCount("Trauma Probe") <= 1),
                    new Decorator(ctx => Tank != null,
                        Spell.CastOnGround("Kolto Bomb", on => Tank.Position, ret => !Tank.HasBuff("Kolto Residue"))),
                    Spell.Heal("Bacta Infusion", 80),
                    Spell.Heal("Medical Probe", 80, ret => Me.HasBuff("Field Triage")),
                    Spell.Heal("Advanced Medical Probe", 75),
                    Spell.Heal("Medical Probe", 50),
                    Spell.Heal("Hammer Shot", on => Tank, 100, ret => Tank != null && Me.InCombat),

                    //Movement
                    CloseDistance(Distance.Ranged),

                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("High Impact Bolt"),
                    Spell.Cast("Full Auto"),
                    Spell.Cast("Charged Bolts", ret => Me.ResourceStat >= 70));
            }
        }
    }
}
