using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.CommonBot;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using Buddy.Navigation;
using Buddywing;
using Buddy.CommonBot.Settings;
using System.IO;
using System.Reflection;
using System.Text;
using DarthBane.Helpers;
using JetBrains.Annotations;


namespace DarthBane
{
    public partial class DarthBane : CombatRoutine
    {
        public override sealed string Name { get { return "Darth Bane"; } }
        public override CharacterClass Class { get { return Me.Class; } }
        public override Window ConfigWindow { get { return null; } }
        static TorPlayer Me { get { return BuddyTor.Me; } }
        private static DateTime datLLSl = DateTime.Now;
        private static long RehookLastMemSize;
        public override void Dispose(){}
        private static IntPtr SWTORHWnd;
        private static int SWTORPID;

        public override void Initialize()
        {
            Logging.Write("Initialize Behaviors");

            // Intialize Behaviors....
            if (_combatBehavior == null)
                _combatBehavior = new PrioritySelector();

            if (_preCombatBehavior == null)
                _preCombatBehavior = new PrioritySelector();

            if (_pullBehavior == null)
                _pullBehavior = new PrioritySelector();

            // Behaviors
            if (!RebuildBehaviors())
                return; 

            Logging.Write("Setting Processor Affinity");
            setAffinity();

            Logging.Write("Setting up Re-Hooks");
            RehookLastMemSize = Process.GetCurrentProcess().PrivateMemorySize64;

            Logging.Write("Init Complete!");
            
        }

        private void setAffinity()
        {
            int ProcessorCount = Environment.ProcessorCount;

            try
            {
                uint Affin = (uint)ProcessorCount;
                uint BWAffin = (uint) ((1 << (ProcessorCount - 1) | (1 << (ProcessorCount))));
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)BWAffin;
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
            }
            catch { Logging.Write("Failed setting process attributes on BuddyWing."); }

            int TorMem = 0;
            foreach (Process proc in Process.GetProcesses()) if (proc.ProcessName.Contains("swtor"))
                    try
                    {
                        uint Affin = ProcessorCount >= 4 ?
                            (uint)((1 << (ProcessorCount - 3) | (1 << (ProcessorCount - 4)))) :
                            (uint)(1 << (ProcessorCount - 2));
                        proc.PriorityClass = ProcessPriorityClass.High;
                        proc.ProcessorAffinity = (IntPtr)Affin;
                    }
                    catch (Exception ex)
                    {
                        Logging.Write("Failed setting process attributes on Star Wars (Main Window).");
                        Logging.Write("Error: " + ex.Message);
                    }
            foreach (Process proc in Process.GetProcesses()) if (proc.ProcessName.Contains("swtor") && proc.MainWindowTitle.Contains("Star Wars"))
                    if (proc.PrivateMemorySize64 > TorMem)
                    {
                        SWTORPID = proc.Id;
                        TorMem = (int)proc.NonpagedSystemMemorySize64;
                        SWTORHWnd = proc.MainWindowHandle;
                    }
        }
    }
}
