using Distance = DarthBane.Helpers.Global.Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.BehaviorTree;
using DarthBane.Helpers;

namespace DarthBane.Class.Sniper
{
    class Engineering : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.SniperEngineering; } }
        public override string Name { get { return "Engineering Combat"; } }
        public override string Revision { get { return ""; } }

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
                    //Movement
                    CloseDistance(Distance.Ranged),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.Cast("Plasma Probe"),
                            Spell.Cast("Fragmentation Grenade"))),

                    //Rotation
                    Spell.Buff("Take Cover"),
                    Spell.Cast("Series of Shots"),
                    Spell.Cast("Shatter Shot", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced")),
                    Spell.Cast("Rifle Shot", ret => Me.EnergyPercent <= 50),
                    Spell.Cast("Snipe"),
                    Spell.Cast("Overload Shot", ret => !Me.HasBuff("Crouch") || !Me.HasBuff("Cover")),
                    Spell.Cast("Plasma Probe"),
                    Spell.Cast("Interrogation Probe"),
                    Spell.Cast("Explosive Probe"),
                    Spell.Cast("Corrosive Dart"),
                    Spell.Cast("Ambush"));
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
                    //Spell.Cast("Ballistic Shield", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Shield Probe", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Evasion", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Adrenaline Probe", ret => Me.EnergyPercent <= 50),
                    Spell.Buff("Laze Target"),
                    Spell.Cast("Target Acquired"),

                    //Movement
                    CloseDistance(Distance.Ranged),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.Cast("Plasma Probe"),
                            Spell.Cast("Fragmentation Grenade"))),

                    //Rotation
                    //Spell.Cast("Take Cover"),
                    Spell.Buff("Crouch", ret => !Me.IsInCover()),
                    Spell.Cast("Series of Shots"),
                    Spell.Cast("Shatter Shot", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced")),
                    Spell.Cast("Plasma Probe"),
                    Spell.Cast("Interrogation Probe", ret => !Me.CurrentTarget.HasDebuff("Interrogation Probe (Tech)")),
                    Spell.Cast("Rifle Shot", ret => Me.EnergyPercent <= 50),
                    Spell.Cast("Snipe"),
                    Spell.Cast("Overload Shot", ret => !Me.HasBuff("Crouch") || !Me.HasBuff("Cover")),
                    Spell.Cast("Explosive Probe"),
                    Spell.Cast("Corrosive Dart"),
                    Spell.Cast("Ambush"));
            }
        }
    }
}
