using System;
using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using System.Diagnostics;
using System.Threading;
using Action = Buddy.BehaviorTree.Action;

namespace DarthBane.Helpers
{
    public static class Rest
    {
        private static TorPlayer Me { get { return BuddyTor.Me; } }

        public static int NormalizedResource()
        {
            if (Me.AdvancedClass == AdvancedClass.None)
            {
                switch (Me.Class)
                {
                    //Add cases needed for reverse basic classes
                    case CharacterClass.BountyHunter:
                        return 100 - (int)Me.ResourcePercent();
                    case CharacterClass.Warrior:
                        return 100;
                    case CharacterClass.Knight:
                        return 100;
                    default:
                        return (int)Me.ResourcePercent();
                }
            }
            else
            {
                switch (Me.AdvancedClass)
                {
                    //Add cases needed for reverse Advance classes
                    case AdvancedClass.Mercenary:
                        return 100 - (int)Me.ResourcePercent();
                    case AdvancedClass.Powertech:
                        return 100 - (int)Me.ResourcePercent();
                    case AdvancedClass.Juggernaut:
                        return 100;
                    case AdvancedClass.Marauder:
                        return 100;
                    case AdvancedClass.Guardian:
                        return 100;
                    case AdvancedClass.Sentinel:
                        return 100;
                    default:
                        return (int)Me.ResourcePercent();
                }
            }

        }

        private static string RestSpell()
        {
            if (Me.AdvancedClass == AdvancedClass.None)
            {
                switch (Me.Class)
                {
                    //Add cases needed for reverse basic classes
                    case CharacterClass.Agent:
                        return "Recuperate";
                    case CharacterClass.Smuggler:
                        return "Recuperate";
                    case CharacterClass.BountyHunter:
                        return "Recharge and Reload";
                    case CharacterClass.Trooper:
                        return "Recharge and Reload";
                    case CharacterClass.Warrior:
                        return "Channel Hatred";
                    case CharacterClass.Knight:
                        return "Introspection";
                    case CharacterClass.Inquisitor:
                        return "Seethe";
                    case CharacterClass.Consular:
                        return "Meditation";
                    default:
                        return "None";
                }
            }
            else
            {
                switch (Me.AdvancedClass)
                {
                    //Add cases needed for reverse Advance classes
                    case AdvancedClass.Operative:
                        return "Recuperate";
                    case AdvancedClass.Sniper:
                        return "Recuperate";
                    case AdvancedClass.Gunslinger:
                        return "Recuperate";
                    case AdvancedClass.Scoundrel:
                        return "Recuperate";
                    case AdvancedClass.Mercenary:
                        return "Recharge and Reload";
                    case AdvancedClass.Powertech:
                        return "Recharge and Reload";
                    case AdvancedClass.Commando:
                        return "Recharge and Reload";
                    case AdvancedClass.Vanguard:
                        return "Recharge and Reload";
                    case AdvancedClass.Juggernaut:
                        return "Channel Hatred";
                    case AdvancedClass.Marauder:
                        return "Channel Hatred";
                    case AdvancedClass.Guardian:
                        return "Introspection";
                    case AdvancedClass.Sentinel:
                        return "Introspection";
                    case AdvancedClass.Assassin:
                        return "Seethe";
                    case AdvancedClass.Sorcerer:
                        return "Seethe";
                    case AdvancedClass.Sage:
                        return "Meditation";
                    case AdvancedClass.Shadow:
                        return "Meditation";
                    default:
                        return "None";
                }
            }
        }

        public static bool NeedRest()
        {
            int resource = NormalizedResource();
            return !Me.InCombat && ((resource < 50 || Me.HealthPercent < 90)
                || (Me.Companion != null && !Me.Companion.IsDead && Me.Companion.HealthPercent < 90));
        }

        public static bool KeepResting()
        {
            int resource = NormalizedResource();
            return !Me.InCombat && ((resource < 100 || Me.HealthPercent < 100)
                || (Me.Companion != null && !Me.Companion.IsDead && Me.Companion.HealthPercent < 100));
        }

        private static long RehookLastMemSize;
        private static DateTime datLGC;
        public static Composite HandleRest
        {
            get
            {
                return new Action(delegate
                {
                    if (Process.GetCurrentProcess().PrivateMemorySize64 - RehookLastMemSize >= 50000000 && !Me.IsCasting)
                    {
                        try
                        {
                            datLGC = DateTime.Now;
                            Logging.Write("Starting GC/Rehook...");
                            GC.Collect();
                            GC.GetTotalMemory(true);
                            Thread.Sleep(3000);
                        }
                        catch
                        {
                            Thread.Sleep(4000);
                            Logging.Write("Error Occoured in Re-Hook");
                        }
                        RehookLastMemSize = Process.GetCurrentProcess().PrivateMemorySize64;
                        Logging.Write("Re-Hook/Reload Processed.");
                        BotMain.CurrentBot.Pulse();
                    }

                    if (NeedRest())
                    {
                        while (KeepResting())
                        {
                            if(!Me.IsCasting)
                                AbilityManager.Cast(RestSpell(), Me);

                            Thread.Sleep(100);
                        }

                        Movement.Move(MovementDirection.Forward, TimeSpan.FromMilliseconds(5));
                        return RunStatus.Success;
                    }

                    return RunStatus.Failure;
                });
            }
        }

        public static Composite CompanionHandler()
        {
            return new Decorator(ret => Me.Companion != null && Me.Companion.IsDead && BuddyTor.Me.CompanionUnlocked > 0,
                new PrioritySelector(
                    Spell.WaitForCast(),
                    Spell.Cast("Revive Companion", on => Me.Companion, when => Me.Companion.Distance <= 0.2f),
                    new Sleep(2500), // I don't give a damn. It's going to want to do other stuff while resting, so let's force him to wait a bit.
                    Class.RotationBase.MoveTo(ret => Me.Companion.Position, 0.2f)));
        }

    }
}
