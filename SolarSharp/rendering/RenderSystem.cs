﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    public static class RenderSystem
    {
        public static Device device;
        public static Context context;

        public static List<GraphicsShader> graphicsShaders = new List<GraphicsShader>();

        public static bool Initialize()
        {
            return true;
        }


    }
}