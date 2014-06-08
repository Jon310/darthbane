using Buddy.BehaviorTree;
using Buddy.Navigation;
using Buddy.Swtor;
using DarthBane.Helpers;
using Distance = DarthBane.Helpers.Global.Distance;
using Action = Buddy.BehaviorTree.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarthBane.Class.Sentinel
{
    class Watchman : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.SentinelWatchman; } }
        public override string Name { get { return "Dual Wield Jedi"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Juyo Form"),
                    Spell.Buff("Force Might"),
                    Rest.HandleRest,
                    Rest.CompanionHandler(),
                    new Decorator(ret => Me.HealthPercent < 100 || Me.Companion.HealthPercent < 100,
                        new Action(ret => Movement.Stop(MovementDirection.Forward))));
            }
        }

        public override Composite Pulling
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Force Leap"),
                    Spell.Cast("Blade Storm", ret => Me.CurrentTarget.Distance <= 1f),

                    //Move To Range
                    CloseDistance(Distance.Melee),

                    //Rotation
                    Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting),
                    Spell.Buff("Zen", ret => Me.HasBuff("Centering") && Me.BuffCount("Centering") > 29),
                    Spell.Cast("Dispatch", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Buff("Overload Saber"),
                    Spell.Cast("Merciless Slash"),
                    Spell.Cast("Cauterize"),
                    Spell.Cast("Slash"),
                    Spell.Buff("Valorous Call", ret => Me.HasBuff("Centering") && Me.BuffCount("Centering") < 15),
                    Spell.Cast("Zealous Strike", ret => Me.Energy <= 5),
                    Spell.Cast("Master Strike"),
                    Spell.Cast("Force Leap"),
                    Spell.Cast("Strike"));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    Spell.WaitForCast(),
                    MedPack.UseItem(ret => Me.HealthPercent < 40),

                    Spell.Buff("Rebuke", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Guarded by the Force", ret => Me.HealthPercent <= 10),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 30),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.Cast("Force Sweep",ret => Me.CurrentTarget.Distance <= 0.5f),
                            Spell.Cast("Twin Saber Throw"),
                            Spell.Cast("Cyclone Slash"))),

                    Spell.Cast("Force Leap"),
                    Spell.Cast("Blade Storm", ret => Me.CurrentTarget.Distance <= 1f),

                    //Move To Range
                    CloseDistance(Distance.Melee),

                    //Rotation
                    Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting),
                    Spell.Buff("Zen", ret => Me.HasBuff("Centering") && Me.BuffCount("Centering") > 29),
                    Spell.Cast("Dispatch", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Buff("Overload Saber"),
                    Spell.Cast("Merciless Slash"),
                    Spell.Cast("Cauterize"),
                    Spell.Cast("Slash"),
                    Spell.Buff("Valorous Call", ret => Me.HasBuff("Centering") && Me.BuffCount("Centering") < 15),
                    Spell.Cast("Zealous Strike", ret => Me.Energy <= 5),
                    Spell.Cast("Master Strike"),
                    Spell.Cast("Force Leap"),
                    Spell.Cast("Strike"));
            }
        }
    }
}
