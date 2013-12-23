using Buddy.BehaviorTree;
using Distance = DarthBane.Helpers.Global.Distance;
using DarthBane.Helpers;

namespace DarthBane.Class.Gunslinger
{
    class Sharpshooter : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.GunslingerSharpshooter; } }
        public override string Name { get { return "Gunslinger Sharpshooter"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Lucky Shots"),
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

                    //Rotation
                    Spell.Buff("Crouch", ret => !Me.IsInCover()),
                    Spell.Cast("Flourish Shot", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced") && Me.CurrentTarget.RelativeDifficulty() > 1),
                    Spell.Cast("Trickshot"),
                    Spell.Cast("Quickdraw", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Sabotage Charge", ret => Me.CurrentTarget.RelativeDifficulty() > 1),
                    Spell.Cast("Flurry of Bolts", ret => Me.EnergyPercent <= 70),
                    Spell.Cast("Speed Shot", ret => Me.IsInCover()),
                    Spell.Cast("Aimed Shot", ret => Me.IsInCover()),
                    Spell.Cast("Charged Burst", ret => Me.IsInCover()),
                    Spell.Cast("Flurry of Bolts"));
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
                    Spell.Buff("Scrambling Field", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Defense Screen", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Dodge", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Cool Head", ret => Me.EnergyPercent <= 50),
                    Spell.Buff("Smuggler's Luck"),
                    Spell.Buff("Illegal Mods"),

                    new Decorator(ret => ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.CastOnGround("XS Freighter Flyby"),
                            Spell.Cast("Thermal Grenade"),
                            Spell.CastOnGround("Sweeping Gunfire"))),

                    //Movement
                    CloseDistance(Distance.Ranged),

                    //Rotation
                    Spell.Buff("Crouch", ret => !Me.IsInCover()),
                    Spell.Cast("Flourish Shot", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced") && Me.CurrentTarget.RelativeDifficulty() > 1),
                    Spell.Cast("Trickshot"),
                    Spell.Cast("Quickdraw", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Sabotage Charge", ret => Me.CurrentTarget.RelativeDifficulty() > 1),
                    Spell.Cast("Flurry of Bolts", ret => Me.EnergyPercent <= 70),
                    Spell.Cast("Speed Shot", ret => Me.IsInCover()),
                    Spell.Cast("Aimed Shot", ret => Me.IsInCover()),
                    Spell.Cast("Charged Burst", ret => Me.IsInCover()),
                    Spell.Cast("Flurry of Bolts"));
            }
        }
    }
}
