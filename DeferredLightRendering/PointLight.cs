using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    class PointLight
    {
        public Vector3 Center;
        public float Radius;
        public Vector3 Color;
        public float Intensity;
        public bool Enabled = true;

        public PointLight(Vector3 center, float r, Vector3 col, float intensity)
        {
            Center = center;
            Radius = r;
            Color = col;
            Intensity = intensity;
        }
    }
}
