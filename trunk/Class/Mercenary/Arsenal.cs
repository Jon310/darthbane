using Distance = DarthBane.Helpers.Global.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.BehaviorTree;
using DarthBane.Helpers;

namespace DarthBane.Class.Mercenary
{
    class Arsenal : RotationBase
    {
        public override SWTorSpec KeySpec
        {
            get { return SWTorSpec.MercenaryArsenal; }
        }

        public override string Name
        {
            get { return "Arsenal"; }
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
                    Spell.Buff("High Velocity Gas Cylinder"),
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
                    //Move To Range
                    CloseDistance(Distance.Ranged),

                    Spell.Cast("Quell", ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= Distance.Melee),
                    Spell.Cast("Rail Shot", ret => Me.BuffCount("Tracer Lock") == 5),
                    Spell.Cast("Heatseeker Missiles", ret => Me.CurrentTarget.HasDebuff("Heat Signature")),
                    Spell.Cast("Electro Net"),
                    Spell.Cast("Unload", ret => Me.HasBuff("Barrage")),
                    Spell.Cast("Tracer Missile", ret => !Me.CurrentTarget.HasDebuff("Heat Signature") || Me.BuffCount("Tracer Lock") < 5),
                    Spell.Cast("Unload"),
                    Spell.Cast("Power Shot", ret => Me.ResourcePercent() < 40),
                    Spell.Cast("Rapid Shots"));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    Spell.WaitForCast(),
                    MedPack.UseItem(ret => Me.HealthPercent <= 50),
                    Spell.Buff("Determination", ret => Me.IsStunned),
                    Spell.Buff("Vent Heat", ret => Me.ResourcePercent() >= 50),
                    Spell.Buff("Energy Shield", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Kolto Overload", ret => Me.HealthPercent <= 30),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.Buff("Power Surge"),
                            Spell.CastOnGround("Death from Above", ret => Me.CurrentTarget.Distance > Distance.MeleeAoE),
                            Spell.Cast("Fusion Missile", ret => Me.ResourcePercent() <= 10 && Me.HasBuff("Power Surge")),
                            Spell.Cast("Flame Thrower", ret => Me.CurrentTarget.Distance <= 1f),
                            Spell.CastOnGround("Sweeping Blasters", ret => Me.ResourcePercent() <= 10))),

                    //Move To Range
                    CloseDistance(Distance.Ranged),

                    Spell.Cast("Quell", ret => Me.CurrentTarget.IsCasting && Me.CurrentTarget.Distance <= Distance.Melee),
                    Spell.Cast("Rail Shot", ret => Me.BuffCount("Tracer Lock") == 5),
                    Spell.Cast("Heatseeker Missiles", ret => Me.CurrentTarget.HasDebuff("Heat Signature")),
                    Spell.Cast("Electro Net"),
                    Spell.Cast("Unload", ret => Me.HasBuff("Barrage")),
                    Spell.Cast("Tracer Missile", ret => !Me.CurrentTarget.HasDebuff("Heat Signature") || Me.BuffCount("Tracer Lock") < 5),
                    Spell.Cast("Unload"),
                    Spell.Cast("Power Shot", ret => Me.ResourcePercent() < 40),
                    Spell.Cast("Rapid Shots"));
            }
        }
    }
}
