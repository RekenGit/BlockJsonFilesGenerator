using System.Collections.Generic;

namespace MinecraftJsonGenerator.Models
{
    public class ModelFileTemplate
    {
        public string FileSuffix { get; set; }
        public string Parent { get; set; }
        public List<string> TextureKeys { get; set; }

        public ModelFileTemplate()
        {
            FileSuffix = string.Empty;
            Parent = string.Empty;
            TextureKeys = new List<string>();
        }
    }
}
