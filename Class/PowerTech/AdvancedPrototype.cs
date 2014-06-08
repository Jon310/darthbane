using Buddy.CommonBot;
using Distance = DarthBane.Helpers.Global.Distance;
using Action = Buddy.BehaviorTree.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.BehaviorTree;
using DarthBane.Helpers;

namespace DarthBane.Class.PowerTech
{
    class AdvancedPrototype : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.PowertechAdvanced; } }
        public override string Name { get { return "MeleeDPS - Hybrid Pyro Prototype"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Combustable Gas Cylinder"),
                    Spell.Buff("Hunter's Boon"),
                    Spell.Cast("Guard", ret => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard")),
                    Rest.HandleRest,
                    Rest.CompanionHandler());
            }
        }

        public override Composite Pulling
        {
            get
            {
                return new PrioritySelector(
                    //Move To Range
                    CloseDistance(Distance.MeleeAoE),

                    //Rotation
                    //Spell.Cast("Neural Dart"),
                    Spell.Cast("Quell", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Heat Blast", ret => Me.BuffCount("Heat Screen") >= 3),
                    Spell.Cast("Rocket Punch"),
                    Spell.Cast("Rail Shot"),
                    Spell.Cast("Explosive Dart"),
                    Spell.Cast("Rapid Shots", ret => Me.ResourcePercent() >= 30),
                    Spell.Cast("Flame Burst"),
                    Spell.Cast("Rapid Shots"));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    Spell.WaitForCast(),
                    MedPack.UseItem(ret => Me.HealthPercent < 40),

                    Spell.Buff("Explosive Fuel"),
                    Spell.Buff("Determination", ret => Me.IsStunned),
                    Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 50),
                    Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 40),
                    Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.CastOnGround("Death from Above", ret => Me.CurrentTarget.Distance > Distance.MeleeAoE),
                            Spell.Cast("Explosive Dart"),
                            new Decorator(ret => Me.CurrentTarget.Distance <= Distance.MeleeAoE,
                                new PrioritySelector(
                                    Spell.Cast("Flame Thrower"),
                                    Spell.Cast("Flame Sweep"))))),

                    //Move To Range
                    CloseDistance(Distance.Melee),

                    //Rotation
                    //Spell.Cast("Neural Dart"),
                    Spell.Cast("Quell", ret => Me.CurrentTarget.IsCasting),

                    Spell.Cast("Retractable Blade", ret => !Me.CurrentTarget.HasDebuff("Bleeding")),
                    Spell.Cast("Flame Thrower", ret => Me.ResourcePercent() > 15 && Me.CurrentTarget.HasDebuff("Bleeding")),
                    Spell.Cast("Rail Shot"),
                    Spell.Cast("Rocket Punch"),
                    Spell.Cast("Rapid Shots", ret => Me.ResourcePercent() >= 30),
                    Spell.Cast("Flame Burst"),
                    Spell.Cast("Incendiary Missile", ret => (Me.CurrentTarget.HasDebuff("Bleeding") && Me.ResourcePercent() > 25) || 
                                                            (!Me.CurrentTarget.HasDebuff("Bleeding") && Me.ResourcePercent() > 15)),
                    Spell.Cast("Rapid Shots"));
            }
        }
    }
}
