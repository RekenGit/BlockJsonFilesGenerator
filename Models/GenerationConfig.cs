using System.Collections.Generic;

namespace MinecraftJsonGenerator.Models
{
    public class GenerationConfig
    {
        public string BlockName { get; set; }
        public string AlternativeBlockName { get; set; }
        public string VariantPattern { get; set; }
        public int VariantCount { get; set; }
        public string Namespace { get; set; }
        public BlockTemplateType BlockType { get; set; }
        public string OutputPath { get; set; }
        public bool UseSingleTexture { get; set; }
        public Dictionary<string, string> TextureInputs { get; set; }
        public Dictionary<string, int> Weights { get; set; }
        public string SelectedItemVariant { get; set; } = "";
        public bool MultipleBlocks { get; set; }
        public string SelectedItemVariant2 { get; set; } = "";
        public string SelectedBlockTypePath { get; set; } = "";

        public GenerationConfig()
        {
            BlockName = string.Empty;
            VariantPattern = string.Empty;
            Namespace = "minecraft";
            OutputPath = string.Empty;
            TextureInputs = new Dictionary<string, string>();
            Weights = new Dictionary<string, int>();
        }
    }
}
