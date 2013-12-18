using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Buddy.BehaviorTree;
using DarthBane.Managers;
using Buddy.Swtor;
using Buddy.Swtor.Objects;
using DarthBane.Helpers;
using Buddy.Common;
using Buddy.Common.Math;
using Buddy.CommonBot;
using Buddy.Navigation;


using Action = Buddy.BehaviorTree.Action;

namespace DarthBane.Class
{
    public abstract class RotationBase
    {
        protected static Buddy.Swtor.Objects.TorPlayer Me { get { return BuddyTor.Me; } }
        protected static TorCharacter Pet { get { return Me.Companion; } }
        protected static TorCharacter Tank { get { return HealingManager.Tank; } }
        protected static TorCharacter HealTarget { get { return HealingManager.HealTarget; } }
                
        protected static IManItem MedPack = new IManItem("Medpac", 90);

        public abstract string Revision { get; }
        public abstract SWTorSpec KeySpec { get; }
        public abstract string Name { get; }

        public abstract Composite PVERotation { get; }
        public abstract Composite PreCombat { get; }
        public abstract Composite Pulling { get; }

        public static bool ShouldAOE(int minMobs, float distance)
        {
            using (BuddyTor.Memory.AcquireFrame())
            {
                Vector3 center = Me.CurrentTarget.Position;

                List<Vector3> points = new List<Vector3>();

                foreach (TorCharacter c in ObjectManager.GetObjects<TorCharacter>())
                {
                    try
                    {
                        if (c != null && c.InCombat && c.IsHostile && !c.IsDead && !c.IsStunned)
                            points.Add(c.Position);
                        if (c != null && !c.InCombat && !c.IsHostile && c.IsDead && c.IsStunned)
                            points.Remove(c.Position);
                    }
                    catch
                    {
                        Logging.Write("An error with aoe count unit");
                        continue;
                    }
                }

                return points.Count(p => p.DistanceSqr(center) <= (distance * distance)) >= minMobs;
            }


        }

        public static Composite CloseDistance(float range)
        {
            return new Decorator(ret => Me.CurrentTarget != null,
                new PrioritySelector(
                    new Decorator(ret => Me.CurrentTarget.Distance < range,
                        new Action( delegate{
                            Navigator.MovementProvider.StopMovement();
                            return RunStatus.Failure;
                        })),
                    new Decorator(ret => Me.CurrentTarget.Distance >= range,
                        CommonBehaviors.MoveAndStop(location => Me.CurrentTarget.Position, range, true)),
                    new Action(delegate { return RunStatus.Failure; })));
        }


        public Composite StopResting
        {
            get
            {
                return new Decorator(ret =>
                    !Me.InCombat
                    && Me.IsCasting
                    && Me.HealthPercent > 90
                    && (Me.Companion != null && Me.Companion.HealthPercent > 90),
                    //&& DateTime.Now.Subtract(Me.CastTimeEnd).TotalSeconds > 5,
                    new Action(ret => Buddy.Swtor.Movement.Move(Buddy.Swtor.MovementDirection.Forward, System.TimeSpan.FromMilliseconds(400))));
            }
        }

        private static void StopMoving()
        {
            Buddy.Swtor.Movement.Stop(MovementDirection.Forward);
            Thread.Sleep(50);
            Buddy.CommonBot.CommonBehaviors.MoveStop();
            Thread.Sleep(50);
            Buddy.Swtor.Input.MoveStopAll();
            Buddy.CommonBot.CommonBehaviors.MoveStop();
        }

        private static DateTime datLCL;
        public static void MoveTo(TorCharacter theUnit, float dist, string Cast1 = "", string Cast2 = "", string Cast3 = "", string Cast4 = "")
        {
            if (theUnit == null)
            {
                Logger.Write("Null MoveTo:  Aborting.");
                return;
            }

            if (theUnit == null) theUnit = Me.CurrentTarget;
            if (theUnit == null || dist <= 0) return;
            if (theUnit.Distance <= dist && theUnit.InLineOfSight) return;

            try
            {

                StopMoving();           // To (try and) prevent 'The Chicken Dance'
                datLCL = DateTime.Now;

                if (theUnit.Distance > dist || !theUnit.InLineOfSight)
                {
                    Logger.Write("MoveTo: " + theUnit.Name + " Moving to within " + dist.ToString("0.0") + " from dist of " + theUnit.Distance.ToString("0.0") + " current LOS: " + theUnit.InLineOfSight.ToString());
                    MoveResult MR = MoveResult.Moved;
                    StopMoving();
                    while ((MR != MoveResult.Failed && MR != MoveResult.PathGenerationFailed) && theUnit != null && DateTime.Now.Subtract(datLCL).TotalSeconds <= 15 && (theUnit.Distance > dist || !theUnit.InLineOfSight))
                    {
                        if (Cast1 != "") Spell.Cast(Cast1);
                        if (Cast2 != "") Spell.Cast(Cast2);
                        if (Cast3 != "") Spell.Cast(Cast3);
                        if (Cast4 != "") Spell.Cast(Cast4);
                        if (theUnit.Distance > dist || !theUnit.InLineOfSight)
                        {
                            MR = Buddy.Navigation.Navigator.MoveTo(theUnit.Position);
                            Thread.Sleep(100);
                        }
                    }
                    if (MR == MoveResult.Failed && MR == MoveResult.PathGenerationFailed) Logger.Write("Move Result: " + MR.ToString());
                    StopMoving();
                    StopMoving();
                }
            }
            catch { }
        }

        public static Composite MoveTo(CommonBehaviors.Retrieval<Vector3> position, float range)
        {
            return CommonBehaviors.MoveAndStop(position, range, true, "Target Position");
        }
        
    }
}
