using Distance = DarthBane.Helpers.Global.Distance;
using Buddy.BehaviorTree;
using DarthBane.Helpers;


namespace DarthBane.Class.Commando
{
    class Gunnery : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.CommandoGunnery; } }
        public override string Name { get { return "Gunner"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Armor-piercing Cell"),
                    Spell.Buff("Fortification"),
                    Rest.HandleRest,
                    Rest.CompanionHandler());
            }
        }

        public override Composite Pulling
        {
            get
            {
                return new PrioritySelector(
                    Spell.WaitForCast(),

                    //Movement
                    CloseDistance(Distance.Ranged),

                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("High Impact Bolt", ret => Me.BuffCount("Charged Barrel") == 5),
                    Spell.Cast("Demolition Round", ret => Me.CurrentTarget.HasDebuff("Gravity Vortex")),
                    Spell.Cast("Electro Net"),
                    Spell.Cast("Full Auto", ret => Me.HasBuff("Curtain of Fire")),
                    Spell.Cast("Grav Round", ret => !Me.CurrentTarget.HasDebuff("Gravity Vortex") || Me.BuffCount("Charged Barrel") < 5),
                    Spell.Cast("Full Auto"),
                    Spell.Cast("Charged Bolts", ret => Me.ResourceStat > 60),
                    Spell.Cast("Hammer Shot"));
            }
        }

        public override Composite PVERotation
        {
            get
            {
                return new PrioritySelector(
                    Spell.WaitForCast(),

                    Spell.Buff("Tenacity"),
                    Spell.Buff("Reactive Shield", ret => Me.HealthPercent <= 70),
                    Spell.Buff("Adrenaline Rush", ret => Me.HealthPercent <= 30),
                    Spell.Buff("Recharge Cells", ret => Me.ResourceStat <= 40),
                    Spell.Buff("Reserve Powercell", ret => Me.ResourceStat <= 60),

                    //Movement
                    CloseDistance(Distance.Ranged),

                    new Decorator(ret => ShouldAOE(2, Distance.MeleeAoE),
                        new PrioritySelector(
                            Spell.Cast("Tech Override"),
                            Spell.CastOnGround("Mortar Volley", ret => Me.CurrentTarget.Distance > 0.5f),
                            Spell.Cast("Plasma Grenade", ret => Me.ResourceStat >= 90 && Me.HasBuff("Tech Override")),
                            Spell.Cast("Pulse Cannon", ret => Me.CurrentTarget.Distance <= 1f && Me.CurrentTarget.IsFacing),
                            Spell.CastOnGround("Hail of Bolts", ret => Me.ResourceStat >= 90))),

                    Spell.Cast("Disabling Shot", ret => Me.CurrentTarget.IsCasting),
                    Spell.Cast("High Impact Bolt", ret => Me.BuffCount("Charged Barrel") == 5),
                    Spell.Cast("Demolition Round", ret => Me.CurrentTarget.HasDebuff("Gravity Vortex")),
                    Spell.Cast("Electro Net"),
                    Spell.Cast("Full Auto", ret => Me.HasBuff("Curtain of Fire")),
                    Spell.Cast("Grav Round", ret => !Me.CurrentTarget.HasDebuff("Gravity Vortex") || Me.BuffCount("Charged Barrel") < 5),
                    Spell.Cast("Full Auto"),
                    Spell.Cast("Charged Bolts", ret => Me.ResourceStat > 60),
                    Spell.Cast("Hammer Shot"));
            }
        }
    }
}
