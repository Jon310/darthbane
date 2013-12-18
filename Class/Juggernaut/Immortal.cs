using Buddy.BehaviorTree;
using DarthBane.Helpers;
using Distance = DarthBane.Helpers.Global.Distance;

namespace DarthBane.Class.Juggernaut
{
    class Immortal : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.JuggernautImmortal; } }
        public override string Name { get { return "Jugg Tank"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Soresu Form"),
                    Spell.Buff("Unnatural Might"),
                    Rest.HandleRest,
                    Rest.CompanionHandler());
            }
        }

        public override Composite PVERotation
        {
            get
            { 
                return new PrioritySelector(
                    Spell.WaitForCast(),

                    Spell.Buff("Unleash", ret => Me.IsStunned),
                    MedPack.UseItem(ret => Me.HealthPercent <= 30),
                    Spell.Buff("Saber Reflect", ret => Me.HealthPercent <= 90),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Invincible", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Endure Pain", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Enrage", ret => Me.ActionPoints <= 6),

                    new Decorator(ret => Me.CurrentTarget.Distance <= Distance.Melee && ShouldAOE(3, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.Cast("Crushing Blow", ret => Me.CurrentTarget.HasDebuff("Armor Reduced")),
                            Spell.Cast("Smash", ret => Me.CurrentTarget.Distance <= 0.5f),
                            Spell.Cast("Sweeping Slash", ret => Me.CurrentTarget.Distance <= 0.5f))),

                    Spell.Cast("Saber Throw"),
                    Spell.Cast("Force Charge"),

                    //Move To Range
                    CloseDistance(Distance.Melee),

                    //Guard Companion
                    Spell.Cast("Guard", ret => Me.Companion, ret => Me.Companion != null && !Me.Companion.IsDead && !Me.Companion.HasBuff("Guard")),

                    //Interupts
                    Spell.Cast("Disruption", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Itimidating Roar", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("Force Choke", ret => Me.CurrentTarget.IsCasting),

                    //Rotation
                    Spell.Cast("Sundering Assault", ret => !Me.CurrentTarget.HasDebuff("Armor Reduced") || Me.ActionPoints <= 7),
                    Spell.Cast("Force Scream"),
                    Spell.Cast("Crushing Blow"),
                    Spell.Cast("Backhand", ret => !Me.CurrentTarget.IsStunned),
                    Spell.Cast("Vicious Throw", ret => Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Ravage"),
                    Spell.Cast("Vicious Slash", ret => Me.ActionPoints >= 11),
                    Spell.Cast("Assault"),
                    Spell.Cast("Retaliation"));
            }
        }

        public override Composite Pulling
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Force Charge"),
                    Spell.Cast("Saber Throw"),
                    CloseDistance(Distance.Melee));
            }
        }
    }
}
