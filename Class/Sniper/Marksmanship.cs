using Buddy.BehaviorTree;
using DarthBane.Helpers;
using Distance = DarthBane.Helpers.Global.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarthBane.Class.Sniper
{
    class Marksmanship : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.SniperMarksmanship; } }

        public override string Name
        {
            get { return "Pew Pew Sniper Marksman"; }
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
                    Spell.Buff("Coordination"),
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

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.CastOnGround("Orbital Strike"),
                            Spell.Cast("Fragmentation Grenade"),
                            Spell.CastOnGround("Suppressive Fire"))),

                    //Rotation
                    Spell.Buff("Take Cover"),
                    Spell.Cast("Shatter Shot", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced")),
                    Spell.Cast("Followthrough"),
                    Spell.Cast("Takedown", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Explosive Probe"),
                    Spell.Cast("Rifle Shot", ret => Me.EnergyPercent <= 70),
                    Spell.Cast("Series of Shots", ret => Me.IsInCover()),
                    Spell.Cast("Ambush"),
                    Spell.Cast("Snipe"),
                    Spell.Cast("Rifle Shot"));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    Spell.WaitForCast(),
                    MedPack.UseItem(ret => Me.HealthPercent < 40),

                    Spell.Buff("Escape"),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 50),
                    Spell.Buff("Laze Target"),
                    Spell.Buff("Target Acquired"),
                    

                    CloseDistance(Distance.Ranged),
                    
                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.CastOnGround("Orbital Strike"),
                            Spell.Cast("Fragmentation Grenade"),
                            Spell.CastOnGround("Suppressive Fire"))),

                    Spell.Cast("Distraction", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Debilitate", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Flash Bang", ret => Me.CurrentTarget.IsCasting),

                    Spell.Buff("Crouch", ret => !Me.IsInCover() && !Me.IsMoving),
                    Spell.Cast("Cover Pulse", ret => Me.CurrentTarget.Distance <= Distance.Melee),
                    Spell.Cast("Leg Shot", ret => Me.CurrentTarget.HasDebuff("Immobilized (Tech)")),
                    Spell.Cast("Entrench"),
                    Spell.Cast("Diversion"),
                    Spell.Cast("Shatter Shot", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced")),
                    Spell.Cast("Followthrough"),
                    Spell.Cast("Takedown"),
                    Spell.Cast("Explosive Probe"),
                    Spell.Cast("Rifle Shot", ret => Me.EnergyPercent <= 60),
                    Spell.Cast("Series of Shots"),
                    Spell.Cast("Ambush"),
                    Spell.Cast("Snipe"),
                    Spell.Cast("Rifle Shot"));
            }
        }

    }
}
