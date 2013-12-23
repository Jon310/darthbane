using DarthBane.Managers;
using Distance = DarthBane.Helpers.Global.Distance;
using Buddy.BehaviorTree;
using DarthBane.Helpers;

namespace DarthBane.Class.Sorcerer
{
    class Corruption : RotationBase
    {
        public override SWTorSpec KeySpec { get { return SWTorSpec.SorcererCorruption; } }
        public override string Name { get { return "Corruption Sorc"; } }
        public override string Revision { get { return ""; } }

        public override Composite PreCombat
        {
            get 
            {
                return new PrioritySelector(
                    PVERotation,
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
                    //Movement
                    CloseDistance(Distance.Ranged),

                    //Rotation
                    Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction (Force)")),
                    Spell.Cast("Crushing Darkness", ret => !Me.CurrentTarget.HasDebuff("Crushed (Force)")),
                    Spell.Cast("Force Lightning"),
                    Spell.Cast("Lightning Strike"),
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
                    Spell.Buff("Recklessness", ret => HealingManager.ShouldAOE),
                    Spell.Buff("Unnatural Preservation", ret => Me.HealthPercent < 50),

                    //Cleanse if needed
                    Spell.Cleanse("Purge"),

                    //Single Target Healing
                    Spell.Heal("Innervate", 80),
                    Spell.HoT("Static Barrier", 75, ret => HealTarget != null && !HealTarget.HasDebuff("Deionized")),     

                    //Buff Tank
                    Spell.HoT("Static Barrier", on => Tank, 100, ret => Tank != null && Tank.InCombat && !Tank.HasDebuff("Deionized")),
                    
                    //Use Force Bending
                    new Decorator(ret => Me.HasBuff("Force Bending"),
                        new PrioritySelector(
                            Spell.Heal("Innervate", 90),
                            Spell.Heal("Dark Infusion", 50))),
                        
                    //Build Force Bending
                    Spell.HoT("Resurgence", 80),
                    Spell.HoT("Resurgence", on => Tank, 100, ret => Tank != null && Tank.InCombat),
                    
                    //Force Regen
                    Spell.Cast("Consumption", on => Me, ret => Me.HasBuff("Force Surge") && Me.HealthPercent > 60 && Me.ForcePercent < 80),

                    //Aoe Heal
                    Spell.HealGround("Revivification"),

                    //Single Target Healing                  
                    Spell.Heal("Dark Heal", 35),
                    Spell.Heal("Dark Infusion", 80),
                    
                    //Movement
                    CloseDistance(Distance.Ranged),

                    //Rotation
                    Spell.Cast("Affliction", ret => !Me.CurrentTarget.HasDebuff("Affliction (Force)")),
                    Spell.Cast("Crushing Darkness", ret => !Me.CurrentTarget.HasDebuff("Crushed (Force)")),
                    Spell.Cast("Force Lightning"),
                    Spell.Cast("Lightning Strike"),
                    Spell.Cast("Shock"));
            }
        }
    }
}
