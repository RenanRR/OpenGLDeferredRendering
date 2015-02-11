using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    class Box
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public Box(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Position = pos;
            Rotation = rot;
            Scale = scale;
        }
    }
}
