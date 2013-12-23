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
        protected static TorPlayer Me { get { return BuddyTor.Me; } }
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

        protected static void StopMoving()
        {
            Movement.Stop(MovementDirection.Forward);
            Thread.Sleep(50);
            CommonBehaviors.MoveStop();
            Thread.Sleep(50);
            Input.MoveStopAll();
            CommonBehaviors.MoveStop();
        }

        public static Composite MoveTo(CommonBehaviors.Retrieval<Vector3> position, float range)
        {
            return CommonBehaviors.MoveAndStop(position, range, true, "Target Position");
        }
        
    }
}
