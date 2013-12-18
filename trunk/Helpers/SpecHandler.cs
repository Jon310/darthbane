using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;

namespace DarthBane.Helpers
{
    static class SpecHandler
    {
        public static SkillTreeId PrimarySpec(this TorPlayer player)
        {
            Tuple<SkillTreeId,SkillTreeId,SkillTreeId> skillTrees = player.ClassSkillTrees();
            var pointsInTree1 = player.GetSkillPointsSpentInTree(skillTrees.Item1);
            var pointsInTree2 = player.GetSkillPointsSpentInTree(skillTrees.Item2);
            var pointsInTree3 = player.GetSkillPointsSpentInTree(skillTrees.Item3);

            Logging.Write(pointsInTree1.ToString());
            Logging.Write(pointsInTree2.ToString());
            Logging.Write(pointsInTree3.ToString());

            foreach(SkillTreeId s in BuddyTor.Me.SkillTrees){
                Logging.Write("*****" + s.ToString());
            }


            if (pointsInTree1 > pointsInTree2 && pointsInTree1 > pointsInTree3)
                { return (skillTrees.Item1); }

            else if (pointsInTree2 > pointsInTree1 && pointsInTree2 > pointsInTree3)
                { return (skillTrees.Item2); }

            else if (pointsInTree3 > pointsInTree1 && pointsInTree3 > pointsInTree2)
                { return (skillTrees.Item3); }

            return (SkillTreeId.None);
        }

        public static string TalentBuild(TorPlayer player)
        {
            Tuple<SkillTreeId,SkillTreeId,SkillTreeId>  classSkillTrees = player.ClassSkillTrees();
            string  advancedClassName   = player.AdvancedClass.ToString();
            string  className           = player.Class.ToString();
            int     pointsInTree1       = player.GetSkillPointsSpentInTree(classSkillTrees.Item1);
            int     pointsInTree2       = player.GetSkillPointsSpentInTree(classSkillTrees.Item2);
            int     pointsInTree3       = player.GetSkillPointsSpentInTree(classSkillTrees.Item3);

            if (player.AdvancedClass == AdvancedClass.None)
                { return ("None"); }

            else if ((pointsInTree1 <= 0) && (pointsInTree2 <= 0) && (pointsInTree3 <= 0))
                { return (string.Format("None ({0} is unspec'd)", player.AdvancedClass.ToString())); }

            return (string.Format("{0} {1} | {2} {3} | {4} {5}",
                        pointsInTree1,
                        classSkillTrees.Item1.ToString().Replace(className, "").Replace(advancedClassName, ""),
                        pointsInTree2,
                        classSkillTrees.Item2.ToString().Replace(className, "").Replace(advancedClassName, ""),
                        pointsInTree3,
                        classSkillTrees.Item3.ToString().Replace(className, "").Replace(advancedClassName, "")));
        }

        #region Private/Utility
        private static Tuple<SkillTreeId,SkillTreeId,SkillTreeId> ClassSkillTrees(this TorPlayer player)
        {
            Tuple<SkillTreeId,SkillTreeId,SkillTreeId> classSkillTrees;

            if (SkillTrees.TryGetValue(player.NormalizedClass(), out classSkillTrees))
                { return (classSkillTrees); }

            return (Tuple.Create(SkillTreeId.None, SkillTreeId.None, SkillTreeId.None));
        }

        private static Dictionary<CharacterClass, Tuple<SkillTreeId,SkillTreeId,SkillTreeId>> SkillTrees = new Dictionary<CharacterClass, Tuple<SkillTreeId,SkillTreeId,SkillTreeId>>()
        {
            // Basic Classes, Empire
            { CharacterClass.Warrior, Tuple.Create(SkillTreeId.None, SkillTreeId.None, SkillTreeId.None) },
            { CharacterClass.Inquisitor, Tuple.Create(SkillTreeId.None, SkillTreeId.None, SkillTreeId.None) },
            { CharacterClass.Agent, Tuple.Create(SkillTreeId.None, SkillTreeId.None, SkillTreeId.None) },
            { CharacterClass.BountyHunter, Tuple.Create(SkillTreeId.None, SkillTreeId.None, SkillTreeId.None) },

            // Advanced Classes, Empire
            { CharacterClass.Juggernaut, Tuple.Create(SkillTreeId.JuggernautImmortal, SkillTreeId.JuggernautRage, SkillTreeId.JuggernautVengeance) },
            { CharacterClass.Marauder, Tuple.Create(SkillTreeId.MarauderAnnihilation, SkillTreeId.MarauderCarnage, SkillTreeId.MarauderRage) },
            { CharacterClass.Assassin, Tuple.Create(SkillTreeId.AssassinDarkness, SkillTreeId.AssassinDeception, SkillTreeId.AssassinMadness) },
            { CharacterClass.Sorcerer, Tuple.Create(SkillTreeId.SorcererCorruption, SkillTreeId.SorcererLightning, SkillTreeId.SorcererMadness) },
            { CharacterClass.Sniper, Tuple.Create(SkillTreeId.SniperMarksmanship, SkillTreeId.SniperEngineering, SkillTreeId.SniperLethality) },
            { CharacterClass.Operative, Tuple.Create(SkillTreeId.OperativeMedic, SkillTreeId.OperativeConcealment, SkillTreeId.OperativeLethality) },
            { CharacterClass.Mercenary, Tuple.Create(SkillTreeId.MercenaryBodyguard, SkillTreeId.MercenaryArsenal, SkillTreeId.MercenaryFirebug) },
            { CharacterClass.Powertech, Tuple.Create(SkillTreeId.PowertechAdvanced, SkillTreeId.PowertechFirebug, SkillTreeId.PowertechShieldTech) },

            // Basic Classes, Republic
            { CharacterClass.Knight, Tuple.Create(SkillTreeId.None, SkillTreeId.None, SkillTreeId.None) },
            { CharacterClass.Consular, Tuple.Create(SkillTreeId.None, SkillTreeId.None, SkillTreeId.None) },
            { CharacterClass.Smuggler, Tuple.Create(SkillTreeId.None, SkillTreeId.None, SkillTreeId.None) },
            { CharacterClass.Trooper, Tuple.Create(SkillTreeId.None, SkillTreeId.None, SkillTreeId.None) },

            // Advanced Classes, Republic
            { CharacterClass.Guardian, Tuple.Create(SkillTreeId.GuardianDefense, SkillTreeId.GuardianFocus, SkillTreeId.GuardianVigilance) },
            { CharacterClass.Sentinel, Tuple.Create(SkillTreeId.SentinelCombat, SkillTreeId.SentinelFocus, SkillTreeId.SentinelWatchman) },
            { CharacterClass.Shadow, Tuple.Create(SkillTreeId.ShadowCombat, SkillTreeId.ShadowInfiltration, SkillTreeId.ShadowBalance) },
            { CharacterClass.Sage, Tuple.Create(SkillTreeId.SageBalance, SkillTreeId.SageSeer, SkillTreeId.SageTelekinetics) },
            { CharacterClass.Gunslinger, Tuple.Create(SkillTreeId.GunslingerDirtyFighting, SkillTreeId.GunslingerSaboteur, SkillTreeId.GunslingerSharpshooter) },
            { CharacterClass.Scoundrel, Tuple.Create(SkillTreeId.ScoundrelSawbones, SkillTreeId.ScoundrelScrapper, SkillTreeId.ScoundrelDirtyFighting) },
            { CharacterClass.Commando, Tuple.Create(SkillTreeId.CommandoCombatMedic, SkillTreeId.CommandoGunnery, SkillTreeId.CommandoAssaultSpecialist) },
            { CharacterClass.Vanguard, Tuple.Create(SkillTreeId.VanguardAssaultSpecialist, SkillTreeId.VanguardShieldSpecialist, SkillTreeId.VanguardTactics) }
        };

        public static SWTorSpec CurrentSpec
        {
            get
            {
                SkillTreeId tree1 = switchSpec(BuddyTor.Me).Item1;
                SkillTreeId tree2 = switchSpec(BuddyTor.Me).Item2;
                SkillTreeId tree3 = switchSpec(BuddyTor.Me).Item3;

                return calculateSpec(BuddyTor.Me, tree1, tree2, tree3);
            }
        }

        public static SWTorSpec GetSpec(TorPlayer Player)
        {

            SkillTreeId tree1 = switchSpec(Player).Item1;
            SkillTreeId tree2 = switchSpec(Player).Item2;
            SkillTreeId tree3 = switchSpec(Player).Item3;

            return calculateSpec(Player, tree1, tree2, tree3);
        }

        private static Tuple<SkillTreeId, SkillTreeId, SkillTreeId> switchSpec(TorPlayer player)
        {
            SkillTreeId tree1 = new SkillTreeId();
            SkillTreeId tree2 = new SkillTreeId();
            SkillTreeId tree3 = new SkillTreeId();

            switch (player.AdvancedClass)
            {
                case AdvancedClass.Assassin:
                    tree1 = SkillTreeId.AssassinDarkness;
                    tree2 = SkillTreeId.AssassinDeception;
                    tree3 = SkillTreeId.AssassinMadness;
                    break;

                case AdvancedClass.Sorcerer:
                    tree1 = SkillTreeId.SorcererCorruption;
                    tree2 = SkillTreeId.SorcererLightning;
                    tree3 = SkillTreeId.SorcererMadness;
                    break;

                case AdvancedClass.Juggernaut:
                    tree1 = SkillTreeId.JuggernautImmortal;
                    tree2 = SkillTreeId.JuggernautRage;
                    tree3 = SkillTreeId.JuggernautVengeance;
                    break;

                case AdvancedClass.Marauder:
                    tree1 = SkillTreeId.MarauderAnnihilation;
                    tree2 = SkillTreeId.MarauderCarnage;
                    tree3 = SkillTreeId.MarauderRage;
                    break;

                case AdvancedClass.Sniper:
                    tree1 = SkillTreeId.SniperEngineering;
                    tree2 = SkillTreeId.SniperLethality;
                    tree3 = SkillTreeId.SniperMarksmanship;
                    break;

                case AdvancedClass.Operative:
                    tree1 = SkillTreeId.OperativeConcealment;
                    tree2 = SkillTreeId.OperativeLethality;
                    tree3 = SkillTreeId.OperativeMedic;
                    break;

                case AdvancedClass.Mercenary:
                    tree1 = SkillTreeId.MercenaryBodyguard;
                    tree2 = SkillTreeId.MercenaryArsenal;
                    tree3 = SkillTreeId.MercenaryFirebug;
                    break;

                case AdvancedClass.Powertech:
                    tree1 = SkillTreeId.PowertechAdvanced;
                    tree2 = SkillTreeId.PowertechFirebug;
                    tree3 = SkillTreeId.PowertechShieldTech;
                    break;

                case AdvancedClass.Shadow:
                    tree1 = SkillTreeId.ShadowBalance;
                    tree2 = SkillTreeId.ShadowCombat;
                    tree3 = SkillTreeId.ShadowInfiltration;
                    break;

                case AdvancedClass.Sage:
                    tree1 = SkillTreeId.SageBalance;
                    tree2 = SkillTreeId.SageSeer;
                    tree3 = SkillTreeId.SageTelekinetics;
                    break;

                case AdvancedClass.Guardian:
                    tree1 = SkillTreeId.GuardianDefense;
                    tree2 = SkillTreeId.GuardianFocus;
                    tree3 = SkillTreeId.GuardianVigilance;
                    break;

                case AdvancedClass.Sentinel:
                    tree1 = SkillTreeId.SentinelCombat;
                    tree2 = SkillTreeId.SentinelFocus;
                    tree3 = SkillTreeId.SentinelWatchman;
                    break;

                case AdvancedClass.Gunslinger:
                    tree1 = SkillTreeId.GunslingerDirtyFighting;
                    tree2 = SkillTreeId.GunslingerSaboteur;
                    tree3 = SkillTreeId.GunslingerSharpshooter;
                    break;

                case AdvancedClass.Scoundrel:
                    tree1 = SkillTreeId.ScoundrelDirtyFighting;
                    tree2 = SkillTreeId.ScoundrelSawbones;
                    tree3 = SkillTreeId.ScoundrelScrapper;
                    break;

                case AdvancedClass.Commando:
                    tree1 = SkillTreeId.CommandoAssaultSpecialist;
                    tree2 = SkillTreeId.CommandoCombatMedic;
                    tree3 = SkillTreeId.CommandoGunnery;
                    break;

                case AdvancedClass.Vanguard:
                    tree1 = SkillTreeId.VanguardAssaultSpecialist;
                    tree2 = SkillTreeId.VanguardShieldSpecialist;
                    tree3 = SkillTreeId.VanguardTactics;
                    break;

                default:
                    break;


            }
            return new Tuple<SkillTreeId, SkillTreeId, SkillTreeId>(tree1, tree2, tree3);
        }

        private static SWTorSpec calculateSpec(TorPlayer p, SkillTreeId tree1, SkillTreeId tree2, SkillTreeId tree3)
        {
            var points1 = BuddyTor.Me.GetSkillPointsSpentInTree(tree1);
            var points2 = BuddyTor.Me.GetSkillPointsSpentInTree(tree2);
            var points3 = BuddyTor.Me.GetSkillPointsSpentInTree(tree3);
            Logging.Write("[SkillTree 1][" + points1 
                        +  "]  [SkillTree 2][" + points2 
                        +  "]  [SkillTree 3][" + points3 + "]");

            if (points1 > points2 && points1 > points3)
            {
                switch (p.AdvancedClass)
                {
                    case AdvancedClass.Assassin:
                        return SWTorSpec.AssassinDarkness;
                    case AdvancedClass.Commando:
                        return SWTorSpec.CommandoAssaultSpecialist;
                    case AdvancedClass.Guardian:
                        return SWTorSpec.GuardianDefense;
                    case AdvancedClass.Gunslinger:
                        return SWTorSpec.GunslingerDirtyFighting;
                    case AdvancedClass.Juggernaut:
                        return SWTorSpec.JuggernautImmortal;
                    case AdvancedClass.Marauder:
                        return SWTorSpec.MarauderAnnihilation;
                    case AdvancedClass.Mercenary:
                        return SWTorSpec.MercenaryBodyguard;
                    case AdvancedClass.Operative:
                        return SWTorSpec.OperativeConcealment;
                    case AdvancedClass.Powertech:
                        return SWTorSpec.PowertechAdvanced;
                    case AdvancedClass.Sage:
                        return SWTorSpec.SageBalance;
                    case AdvancedClass.Scoundrel:
                        return SWTorSpec.ScoundrelDirtyFighting;
                    case AdvancedClass.Sentinel:
                        return SWTorSpec.SentinelCombat;
                    case AdvancedClass.Shadow:
                        return SWTorSpec.ShadowBalance;
                    case AdvancedClass.Sniper:
                        return SWTorSpec.SniperEngineering;
                    case AdvancedClass.Sorcerer:
                        return SWTorSpec.SorcererCorruption;
                    case AdvancedClass.Vanguard:
                        return SWTorSpec.VanguardAssaultSpecialist;
                    default:
                        return SWTorSpec.None;
                }
            }
            else if (points2 > points1 && points2 > points3)
            {
                switch (BuddyTor.Me.AdvancedClass)
                {
                    case AdvancedClass.Assassin:
                        return SWTorSpec.AssassinDeception;
                    case AdvancedClass.Commando:
                        return SWTorSpec.CommandoCombatMedic;
                    case AdvancedClass.Guardian:
                        return SWTorSpec.GuardianFocus;
                    case AdvancedClass.Gunslinger:
                        return SWTorSpec.GunslingerSaboteur;
                    case AdvancedClass.Juggernaut:
                        return SWTorSpec.JuggernautRage;
                    case AdvancedClass.Marauder:
                        return SWTorSpec.MarauderCarnage;
                    case AdvancedClass.Mercenary:
                        return SWTorSpec.MercenaryArsenal;
                    case AdvancedClass.Operative:
                        return SWTorSpec.OperativeLethality;
                    case AdvancedClass.Powertech:
                        return SWTorSpec.PowertechFirebug;
                    case AdvancedClass.Sage:
                        return SWTorSpec.SageSeer;
                    case AdvancedClass.Scoundrel:
                        return SWTorSpec.ScoundrelSawbones;
                    case AdvancedClass.Sentinel:
                        return SWTorSpec.SentinelFocus;
                    case AdvancedClass.Shadow:
                        return SWTorSpec.ShadowCombat;
                    case AdvancedClass.Sniper:
                        return SWTorSpec.SniperLethality;
                    case AdvancedClass.Sorcerer:
                        return SWTorSpec.SorcererLightning;
                    case AdvancedClass.Vanguard:
                        return SWTorSpec.VanguardShieldSpecialist;
                    default:
                        return SWTorSpec.None;
                }
            }
            else if (points3 > points1 && points3 > points2)
            {
                switch (BuddyTor.Me.AdvancedClass)
                {
                    case AdvancedClass.Assassin:
                        return SWTorSpec.AssassinMadness;
                    case AdvancedClass.Commando:
                        return SWTorSpec.CommandoGunnery;
                    case AdvancedClass.Guardian:
                        return SWTorSpec.GuardianVigilance;
                    case AdvancedClass.Gunslinger:
                        return SWTorSpec.GunslingerSharpshooter;
                    case AdvancedClass.Juggernaut:
                        return SWTorSpec.JuggernautVengeance;
                    case AdvancedClass.Marauder:
                        return SWTorSpec.MarauderRage;
                    case AdvancedClass.Mercenary:
                        return SWTorSpec.MercenaryFirebug;
                    case AdvancedClass.Operative:
                        return SWTorSpec.OperativeMedic;
                    case AdvancedClass.Powertech:
                        return SWTorSpec.PowertechShieldTech;
                    case AdvancedClass.Sage:
                        return SWTorSpec.SageTelekinetics;
                    case AdvancedClass.Scoundrel:
                        return SWTorSpec.ScoundrelScrapper;
                    case AdvancedClass.Sentinel:
                        return SWTorSpec.SentinelWatchman;
                    case AdvancedClass.Shadow:
                        return SWTorSpec.ShadowInfiltration;
                    case AdvancedClass.Sniper:
                        return SWTorSpec.SniperMarksmanship;
                    case AdvancedClass.Sorcerer:
                        return SWTorSpec.SorcererMadness;
                    case AdvancedClass.Vanguard:
                        return SWTorSpec.VanguardTactics;
                    default:
                        return SWTorSpec.None;
                }
            }
            else
                return SWTorSpec.None;
        }
    }

    public enum SWTorSpec
    {
        AssassinDarkness,
        AssassinDeception,
        AssassinMadness,
        SorcererCorruption,
        SorcererLightning,
        SorcererMadness,
        JuggernautImmortal,
        JuggernautRage,
        JuggernautVengeance,
        MarauderAnnihilation,
        MarauderCarnage,
        MarauderRage,
        SniperEngineering,
        SniperLethality,
        SniperMarksmanship,
        OperativeConcealment,
        OperativeLethality,
        OperativeMedic,
        MercenaryArsenal,
        MercenaryBodyguard,
        MercenaryFirebug,
        PowertechAdvanced,
        PowertechFirebug,
        PowertechShieldTech,
        ShadowBalance,
        ShadowCombat,
        ShadowInfiltration,
        SageBalance,
        SageSeer,
        SageTelekinetics,
        GuardianDefense,
        GuardianFocus,
        GuardianVigilance,
        SentinelCombat,
        SentinelFocus,
        SentinelWatchman,
        GunslingerDirtyFighting,
        GunslingerSaboteur,
        GunslingerSharpshooter,
        ScoundrelDirtyFighting,
        ScoundrelSawbones,
        ScoundrelScrapper,
        CommandoAssaultSpecialist,
        CommandoCombatMedic,
        CommandoGunnery,
        VanguardAssaultSpecialist,
        VanguardShieldSpecialist,
        VanguardTactics,
        None
    }
#endregion
}