using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloWarsTools;
using LearnOpenTK.Common;

namespace HaloWarsInspector.Rendering
{
    static class ShaderCompiler
    {
        private static LazyValueCache ShaderCache = new LazyValueCache();

        public static Shader DefaultShader => ShaderCache.Get(() => {
            var shader = new Shader("Rendering/Shaders/shader.vert", "Rendering/Shaders/shader.frag");
            shader.Texture = Texture.LoadFromFile("Rendering/Resources/container.png");
            shader.Use();
            shader.SetInt("texture0", 0); // Unsure what this does

            return shader;
        });
    }
}
