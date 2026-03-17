using System.Collections.Generic;

namespace MinecraftJsonGenerator.Models
{
    public class BlockTemplateDefinition
    {
        public List<ModelFileTemplate> ModelFiles { get; set; }
        public List<string> ManualTextureKeys { get; set; }
        public string ItemModelSuffix { get; set; }

        public BlockTemplateDefinition()
        {
            ModelFiles = new List<ModelFileTemplate>();
            ManualTextureKeys = new List<string>();
            ItemModelSuffix = string.Empty;
        }
    }
}
