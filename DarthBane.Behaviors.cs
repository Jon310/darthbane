using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Buddy.BehaviorTree;
using Buddy.Common;
using Buddy.Swtor;
using DarthBane.Class;
using DarthBane.Helpers;

namespace DarthBane
{
    partial class DarthBane
    {
        private Composite _combatBehavior, _preCombatBehavior, _pullBehavior;
        public override Composite OutOfCombat { get { return _preCombatBehavior; } }
        public override Composite Combat { get { return _combatBehavior; } }
        public override Composite Pull { get { return _pullBehavior; } }
        private Composite PreCombat { get { return new Decorator(ret => !Me.IsDead && !BuddyTor.Me.IsMounted, _currentRotation.PreCombat); } }
        private Composite Rotation { get { return _currentRotation.PVERotation; } }
        private Composite Pulling { get { return _currentRotation.Pulling; } }
        private RotationBase _currentRotation; // the current Rotation
        private List<RotationBase> _rotations;

        public bool RebuildBehaviors()
        {
            Logging.Write("RebuildBehaviors called.");

            _currentRotation = null; // clear current rotation

            GetRotations();

            SetRotation(); // set the new rotation

            if (_combatBehavior != null)
                _combatBehavior = new PrioritySelector(Rotation);
            if (_preCombatBehavior != null)
                _preCombatBehavior = new PrioritySelector(PreCombat);
            if (_pullBehavior != null)
                _pullBehavior = new PrioritySelector(Pulling);

            return true;
        }

        /// <summary>Set the Current Rotation</summary>
        private void SetRotation()
        {
            try
            {
                if (_rotations != null && _rotations.Count > 0)
                {
                    //Logging.Write(" We have rotations so lets use the best one...");
                    foreach (var rotation in _rotations)
                    {
                        if (rotation != null && rotation.KeySpec == SpecHandler.CurrentSpec)
                        {
                            Logging.Write(" Using " + rotation.Name + " rotation based on Character Spec ");
                            _currentRotation = rotation;
                        }
                        else
                        {
                            if (rotation != null)
                            {
                                //Logging.Write(" Skipping " + rotation.Name + " rotation. Character is not in " + rotation.KeySpec);
                            }
                        }
                    }
                }
                else
                {
                    Logging.Write(" We have no rotations -  calling GetRotations");
                    GetRotations();
                }
            }
            catch (Exception ex)
            {
                Logging.Write(" Failed to Set Rotation " + ex);
            }
        }

        /// <summary>Get & Set the Current Rotation</summary>
        private void GetRotations()
        {
            try
            {
                _rotations = new List<RotationBase>();
                _rotations.AddRange(new TypeLoader<RotationBase>());

                foreach (var rotation in _rotations)
                {
                    if (rotation != null && rotation.KeySpec == SpecHandler.CurrentSpec)
                    {
                        Logging.Write(" Using " + rotation.Name + " rotation based on Character Spec ");
                        _currentRotation = rotation;
                    }
                    else
                    {
                        if (rotation != null)
                        {
                            //Logging.Write(" Skipping " + rotation.Name + " rotation. Character is not in " + rotation.KeySpec);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException)
                    {
                        var exFileNotFound = exSub as FileNotFoundException;
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("DarthBane Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                Logging.Write(" Woops, we could not set the rotation.");
                Logging.Write(errorMessage);
            }
        }
    }
}
