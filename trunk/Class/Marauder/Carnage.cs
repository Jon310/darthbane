using Distance = DarthBane.Helpers.Global.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.BehaviorTree;
using DarthBane.Helpers;

namespace DarthBane.Class.Marauder
{
    class Carnage : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.MarauderCarnage; } }
        public override string Name { get { return "Carnage"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Ataru Form"),
                    Spell.Buff("Unnatural Might"),
                    Rest.HandleRest,
                    Rest.CompanionHandler());
            }
        }

        public override Composite Pulling
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Saber Throw"),
                    Spell.Cast("Force Charge"),
                    Spell.Cast("Dual Saber Throw"),

                    //Move To Range
                    CloseDistance(Distance.Melee),

                    //Interupts
                    Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting),

                    Spell.Cast("Vicious Throw"),
                    Spell.Cast("Gore"),
                    Spell.Cast("Ravage", ret => Me.HasBuff("Gore")),
                    Spell.Cast("Force Scream", ret => Me.HasBuff("Execute")),
                    Spell.Cast("Massacre"),
                    Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 7),
                    Spell.Cast("Battering Assault", ret => Me.ActionPoints <= 7),
                    Spell.Cast("Assault", ret => Me.ActionPoints <= 9),
                    Spell.Cast("Retaliation"));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    Spell.WaitForCast(),
                    MedPack.UseItem(ret => Me.HealthPercent < 40),

                    Spell.Buff("Unleash"),
                    Spell.Buff("Cloak of Pain", ret => Me.HealthPercent <= 90),
                    Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Frenzy", ret => Me.BuffCount("Fury") < 5),
                    Spell.Buff("Berserk"),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.Cast("Smash", ret => Me.CurrentTarget.Distance <= 0.5f),
                            Spell.Cast("Sweeping Slash", ret => Me.CurrentTarget.Distance <= 0.5f))),

                    Spell.Cast("Saber Throw"),
                    Spell.Cast("Force Charge"),
                    Spell.Cast("Dual Saber Throw"),

                    //Move To Range
                    CloseDistance(Distance.Melee),

                    //Interupts
                    Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting),

                    Spell.Cast("Vicious Throw"),
                    Spell.Cast("Gore"),
                    Spell.Cast("Ravage", ret => Me.HasBuff("Gore")),
                    Spell.Cast("Force Scream", ret => Me.HasBuff("Execute")),
                    Spell.Cast("Massacre"),
                    Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 7),
                    Spell.Cast("Battering Assault", ret => Me.ActionPoints <= 7),
                    Spell.Cast("Assault", ret => Me.ActionPoints <= 9),
                    Spell.Cast("Retaliation"));
            }
        }
    }
}
