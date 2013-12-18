using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarthBane.Helpers;

namespace DarthBane.Class.Gunslinger
{
    class Sharpshooter : RotationBase
    {
        public override Helpers.SWTorSpec KeySpec
        {
            get { return SWTorSpec.GunslingerSharpshooter; }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override string Revision
        {
            get { throw new NotImplementedException(); }
        }

        public override Buddy.BehaviorTree.Composite PreCombat
        {
            get { throw new NotImplementedException(); }
        }

        public override Buddy.BehaviorTree.Composite Pulling
        {
            get { throw new NotImplementedException(); }
        }

        public override Buddy.BehaviorTree.Composite PVERotation
        {
            get { throw new NotImplementedException(); }
        }
    }
}
