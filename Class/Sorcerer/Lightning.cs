using DarthBane.Managers;
using Distance = DarthBane.Helpers.Global.Distance;
using System.Linq;
using Buddy.BehaviorTree;
using DarthBane.Helpers;

namespace DarthBane.Class.Sorcerer
{
    class Lightning : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.SorcererLightning; } }
        public override string Name { get { return "Corruption Sorc"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Mark of Power"),
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
                    Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction (Force)")),
                    Spell.Cast("Thundering Blast"),
                    Spell.Cast("Crushing Darkness", ret => !Me.CurrentTarget.HasDebuff("Crushed (Force)")),
                    Spell.Cast("Force Lightning", ret => Me.HasBuff("Lightning Barrage")),
                    Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
                    Spell.Cast("Lightning Strike", ret => Me.HasBuff("Subversion") && Me.Buffs.FirstOrDefault(B => B.Name == "Subversion").Stacks < 3 || Me.HasBuff("Subversion") && Me.Buffs.FirstOrDefault(B => B.Name == "Subversion").TimeLeft.Seconds < 8 || !Me.HasBuff("Subversion")),
                    Spell.Cast("Shock"));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    HealingManager.AcquireHealTargets,
                    Spell.WaitForCast(),

                    Spell.Buff("Recklessness"),
                    Spell.Buff("Polarity Shift"),
                    Spell.Buff("Static Barrier", ret => !Me.HasBuff("Deionized")),
                    Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent < 50),
                    Spell.Buff("Consumption", ret => Me.HealthPercent > 80 && Me.ForcePercent < 20),

                    Spell.HoT("Static Barrier", on => Tank, 100, ret => !Tank.HasDebuff("Deionized")),
                    Spell.HoT("Static Barrier", 95, ret => !HealTarget.HasDebuff("Deionized")),
                    Spell.Heal("Dark Heal", 35),
                    Spell.Heal("Dark Infusion", 70),

                    //Movement
                    CloseDistance(Distance.Ranged),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.Cast("Thundering Blast"),
                            Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
                            Spell.CastOnGround("Force Storm"))),

                    //Rotation
                    Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction (Force)")),
                    Spell.Cast("Thundering Blast"),
                    Spell.Cast("Crushing Darkness", ret => !Me.CurrentTarget.HasDebuff("Crushed (Force)")),
                    Spell.Cast("Force Lightning", ret => Me.HasBuff("Lightning Barrage")),
                    Spell.Cast("Chain Lightning", ret => Me.HasBuff("Lightning Storm")),
                    Spell.Cast("Lightning Strike", ret => Me.HasBuff("Subversion") && Me.Buffs.FirstOrDefault(B => B.Name == "Subversion").Stacks < 3 || Me.HasBuff("Subversion") && Me.Buffs.FirstOrDefault(B => B.Name == "Subversion").TimeLeft.Seconds < 8 || !Me.HasBuff("Subversion")),
                    Spell.Cast("Shock"));
            }
        }
    }
}
