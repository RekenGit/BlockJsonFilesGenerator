using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MinecraftJsonGenerator.Models;

namespace MinecraftJsonGenerator.Services
{
    public class GenerationService
    {
        public void GenerateAll(GenerationConfig config)
        {
            GenerateModels(config);
            GenerateItems(config);
            GenerateBlockstates(config);
        }

        public void GenerateModels(GenerationConfig config)
        {
            Validate(config);

            var ns = VariantService.NormalizeNamespace(config.Namespace);
            var textureVariants = VariantService.BuildVariantNames(config.BlockName, config.VariantPattern, config.VariantCount);
            var fileNameVariants = VariantService.BuildVariantNames(config.AlternativeBlockName, config.VariantPattern, config.VariantCount);
            var template = TemplateRepository.Get(config.BlockType);
            var basePath = Path.Combine(config.OutputPath, "assets", ns, "models", "block", config.BlockName, config.SelectedBlockTypePath);

            Directory.CreateDirectory(basePath);

            foreach (var variant in textureVariants.Select((value, i) => new { i, value}))
            {
                foreach (var model in template.ModelFiles)
                {
                    var fileName = fileNameVariants[variant.i] + model.FileSuffix + ".json";
                    var filePath = Path.Combine(basePath, fileName);
                    var json = BuildModelJson(ns, variant.value, config, model);
                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }
            }
        }

        public void GenerateItems(GenerationConfig config)
        {
            Validate(config);

            string ns = VariantService.NormalizeNamespace(config.Namespace);
            string itemsDir = Path.Combine(config.OutputPath, "assets", ns, "items");
            Directory.CreateDirectory(itemsDir);

            var template = TemplateRepository.Get(config.BlockType);

            string selectedVariant1 = string.IsNullOrWhiteSpace(config.SelectedItemVariant)
                ? config.AlternativeBlockName
                : config.SelectedItemVariant;

            if (!string.IsNullOrEmpty(config.SelectedBlockTypePath))
                selectedVariant1 = config.SelectedBlockTypePath + "/" + selectedVariant1;

            string modelPath1 = $"{ns}:block/{config.BlockName}/{selectedVariant1}{template.ItemModelSuffix}";
            string itemFilePath1 = Path.Combine(itemsDir, $"{config.AlternativeBlockName}.json");
            File.WriteAllText(itemFilePath1, BuildItemJson(modelPath1), Encoding.UTF8);

            if (config.MultipleBlocks)
            {
                string selectedVariant2 = string.IsNullOrWhiteSpace(config.SelectedItemVariant2)
                    ? config.AlternativeBlockName
                    : config.SelectedItemVariant2;

                if (!string.IsNullOrEmpty(config.SelectedBlockTypePath))
                    selectedVariant2 = config.SelectedBlockTypePath + "/" + selectedVariant2;

                string secondaryBlockName = GetSecondaryBlockName(config);
                string modelPath2 = $"{ns}:block/{config.BlockName}/{selectedVariant2}{template.ItemModelSuffix}";
                string itemFilePath2 = Path.Combine(itemsDir, $"{secondaryBlockName}.json");

                File.WriteAllText(itemFilePath2, BuildItemJson(modelPath2), Encoding.UTF8);
            }
        }

        private string BuildItemJson(string modelRef)
        {
            return "{\r\n" +
                   "  \"model\": {\r\n" +
                   "    \"type\": \"minecraft:model\",\r\n" +
                   "    \"model\": \"" + modelRef + "\"\r\n" +
                   "  }\r\n" +
                   "}";
        }

        public void GenerateBlockstates(GenerationConfig config)
        {
            Validate(config);

            var ns = VariantService.NormalizeNamespace(config.Namespace);
            string _path = string.IsNullOrWhiteSpace(config.SelectedBlockTypePath) ? config.AlternativeBlockName : config.SelectedBlockTypePath + "/" + config.AlternativeBlockName;
            var variants = VariantService.BuildVariantNames(_path, config.VariantPattern, config.VariantCount);
            var fullBlockVariants = VariantService.BuildVariantNames(config.BlockName, config.VariantPattern, config.VariantCount);
            var blockstatesPath = Path.Combine(config.OutputPath, "assets", ns, "blockstates");
            Directory.CreateDirectory(blockstatesPath);

            if (!config.MultipleBlocks)
            {
                var json = BuildBlockstateJson(config, ns, variants, fullBlockVariants);
                File.WriteAllText(Path.Combine(blockstatesPath, config.AlternativeBlockName + ".json"), json, Encoding.UTF8);
                return;
            }

            string baseJson = BuildSingleModelBlockstate(config, ns, config.OutputPath + config.BlockName);
            File.WriteAllText(Path.Combine(blockstatesPath, config.AlternativeBlockName + ".json"), baseJson, Encoding.UTF8);

            string secondaryBlockName = GetSecondaryBlockName(config);
            string variantsJson = BuildBlockstateJson(config, ns, variants, fullBlockVariants);
            File.WriteAllText(Path.Combine(blockstatesPath, secondaryBlockName + ".json"), variantsJson, Encoding.UTF8);
        }

        private void Validate(GenerationConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.BlockName))
                throw new InvalidOperationException("Nazwa bloku jest wymagana.");

            if (string.IsNullOrWhiteSpace(config.OutputPath))
                throw new InvalidOperationException("Folder wyjściowy jest wymagany.");
        }

        private string BuildModelJson(string ns, string variant, GenerationConfig config, ModelFileTemplate model)
        {
            var textures = ResolveTextures(ns, variant, config, model.TextureKeys);
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"parent\": \"" + model.Parent + "\",");
            sb.AppendLine("  \"textures\": {");

            for (int i = 0; i < textures.Count; i++)
            {
                var kv = textures.ElementAt(i);
                var comma = i < textures.Count - 1 ? "," : string.Empty;
                sb.AppendLine("    \"" + kv.Key + "\": \"" + kv.Value + "\"" + comma);
            }

            sb.AppendLine("  }");
            sb.Append("}");
            return sb.ToString();
        }

        private Dictionary<string, string> ResolveTextures(string ns, string variant, GenerationConfig config, List<string> keys)
        {
            var result = new Dictionary<string, string>();

            if (config.UseSingleTexture)
            {
                var baseTexture = GetInput(config, "all");
                if (string.IsNullOrWhiteSpace(baseTexture))
                    baseTexture = variant;

                var textureRef = NormalizeTextureRef(ns, baseTexture);
                foreach (var key in keys)
                    result[key] = textureRef;

                return result;
            }

            foreach (var key in keys)
            {
                var value = GetInput(config, key);
                if (string.IsNullOrWhiteSpace(value))
                    value = variant;

                result[key] = NormalizeTextureRef(ns, value);
            }

            return result;
        }

        private string GetInput(GenerationConfig config, string key)
        {
            if (config.TextureInputs == null)
                return null;

            return config.TextureInputs.ContainsKey(key) ? config.TextureInputs[key] : null;
        }

        private string NormalizeTextureRef(string ns, string value)
        {
            var trimmed = (value ?? string.Empty).Trim();
            if (trimmed.Contains(":"))
                return trimmed;

            if (trimmed.StartsWith("block/") || trimmed.StartsWith("item/"))
                return ns + ":" + trimmed;

            return ns + ":block/" + trimmed;
        }

        private string BuildBlockstateJson(GenerationConfig config, string ns, List<string> variants, List<string> variantsFullBLock = null)
        {
            switch (config.BlockType)
            {
                case BlockTemplateType.Block:
                case BlockTemplateType.Leaves:
                case BlockTemplateType.Column:
                case BlockTemplateType.Column2:
                case BlockTemplateType.Cross:
                case BlockTemplateType.Carpet:
                    return BuildSimpleVariantsBlockstate(config, ns, variants);
                case BlockTemplateType.Slab:
                    return BuildSlabBlockstate(config, ns, variants, variantsFullBLock);
                case BlockTemplateType.Stairs:
                    return BuildStairsBlockstate(config, ns, variants);
                case BlockTemplateType.Wall:
                    return BuildWallBlockstate(config, ns, variants);
                case BlockTemplateType.Door:
                    return BuildDoorBlockstate(config, ns, variants);
                case BlockTemplateType.Trapdoor:
                    return BuildTrapdoorBlockstate(config, ns, variants);
                case BlockTemplateType.FenceGate:
                    return BuildFenceGateBlockstate(config, ns, variants);
                case BlockTemplateType.Fence:
                    return BuildFenceBlockstate(config, ns, variants);
                case BlockTemplateType.Torch:
                    return BuildTorchBlockstate(config, ns, variants);

                case BlockTemplateType.Pane:
                    return BuildPaneBlockstate(config, ns, variants);

                case BlockTemplateType.Rail:
                    return BuildRailBlockstate(config, ns, variants);
                default:
                    return BuildSimpleVariantsBlockstate(config, ns, variants);
            }
        }

        private string BuildSimpleVariantsBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"variants\": {");
            sb.AppendLine("    \"\": [");
            AppendWeightedModels(sb, config, ns, variants, "", 6);
            sb.AppendLine("    ]");
            sb.AppendLine("  }");
            sb.Append("}");
            return sb.ToString();
        }

        private string BuildSlabBlockstate(GenerationConfig config, string ns, List<string> variants, List<string> variantsFullBLock)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"variants\": {");
            sb.AppendLine("    \"type=bottom\": [");
            AppendWeightedModels(sb, config, ns, variants, "", 6);
            sb.AppendLine("    ],");
            sb.AppendLine("    \"type=top\": [");
            AppendWeightedModels(sb, config, ns, variants, "_top", 6);
            sb.AppendLine("    ],");
            sb.AppendLine("    \"type=double\": [");
            AppendWeightedModels(sb, config, ns, variantsFullBLock, "", 6);
            sb.AppendLine("    ]");
            sb.AppendLine("  }");
            sb.Append("}");
            return sb.ToString();
        }

        private string BuildStairsBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            var map = new Dictionary<string, Tuple<string, int, int, bool>>
            {
                { "facing=east,half=bottom,shape=inner_left", Tuple.Create("_inner", 0, 270, true) },
                { "facing=east,half=bottom,shape=inner_right", Tuple.Create("_inner", 0, 0, false) },
                { "facing=east,half=bottom,shape=outer_left", Tuple.Create("_outer", 0, 270, true) },
                { "facing=east,half=bottom,shape=outer_right", Tuple.Create("_outer", 0, 0, false) },
                { "facing=east,half=bottom,shape=straight", Tuple.Create("", 0, 0, false) },

                { "facing=east,half=top,shape=inner_left", Tuple.Create("_inner", 180, 0, true) },
                { "facing=east,half=top,shape=inner_right", Tuple.Create("_inner", 180, 90, true) },
                { "facing=east,half=top,shape=outer_left", Tuple.Create("_outer", 180, 0, true) },
                { "facing=east,half=top,shape=outer_right", Tuple.Create("_outer", 180, 90, true) },
                { "facing=east,half=top,shape=straight", Tuple.Create("", 180, 0, true) },


                { "facing=west,half=bottom,shape=inner_left", Tuple.Create("_inner", 0, 90, true) },
                { "facing=west,half=bottom,shape=inner_right", Tuple.Create("_inner", 0, 180, true) },
                { "facing=west,half=bottom,shape=outer_left", Tuple.Create("_outer", 0, 90, true) },
                { "facing=west,half=bottom,shape=outer_right", Tuple.Create("_outer", 0, 180, true) },
                { "facing=west,half=bottom,shape=straight", Tuple.Create("", 0, 180, true) },

                { "facing=west,half=top,shape=inner_left", Tuple.Create("_inner", 180, 180, true) },
                { "facing=west,half=top,shape=inner_right", Tuple.Create("_inner", 180, 270, true) },
                { "facing=west,half=top,shape=outer_left", Tuple.Create("_outer", 180, 180, true) },
                { "facing=west,half=top,shape=outer_right", Tuple.Create("_outer", 180, 270, true) },
                { "facing=west,half=top,shape=straight", Tuple.Create("", 180, 180, true) },


                { "facing=south,half=bottom,shape=inner_left", Tuple.Create("_inner", 0, 0, false) },
                { "facing=south,half=bottom,shape=inner_right", Tuple.Create("_inner", 0, 90, true) },
                { "facing=south,half=bottom,shape=outer_left", Tuple.Create("_outer", 0, 0, false) },
                { "facing=south,half=bottom,shape=outer_right", Tuple.Create("_outer", 0, 90, true) },
                { "facing=south,half=bottom,shape=straight", Tuple.Create("", 0, 90, true) },

                { "facing=south,half=top,shape=inner_left", Tuple.Create("_inner", 180, 90, true) },
                { "facing=south,half=top,shape=inner_right", Tuple.Create("_inner", 180, 180, true) },
                { "facing=south,half=top,shape=outer_left", Tuple.Create("_outer", 180, 90, true) },
                { "facing=south,half=top,shape=outer_right", Tuple.Create("_outer", 180, 180, true) },
                { "facing=south,half=top,shape=straight", Tuple.Create("", 180, 90, true) },


                { "facing=north,half=bottom,shape=inner_left", Tuple.Create("_inner", 0, 180, true) },
                { "facing=north,half=bottom,shape=inner_right", Tuple.Create("_inner", 0, 270, true) },
                { "facing=north,half=bottom,shape=outer_left", Tuple.Create("_outer", 0, 180, true) },
                { "facing=north,half=bottom,shape=outer_right", Tuple.Create("_outer", 0, 270, true) },
                { "facing=north,half=bottom,shape=straight", Tuple.Create("", 0, 270, true) },

                { "facing=north,half=top,shape=inner_left", Tuple.Create("_inner", 180, 270, true) },
                { "facing=north,half=top,shape=inner_right", Tuple.Create("_inner", 180, 0, true) },
                { "facing=north,half=top,shape=outer_left", Tuple.Create("_outer", 180, 270, true) },
                { "facing=north,half=top,shape=outer_right", Tuple.Create("_outer", 180, 0, true) },
                { "facing=north,half=top,shape=straight", Tuple.Create("", 180, 270, true) }
            };

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"variants\": {");
            int index = 0;
            foreach (var kv in map)
            {
                var comma = index < map.Count - 1 ? "," : string.Empty;
                sb.AppendLine("    \"" + kv.Key + "\": [");
                AppendWeightedModels(sb, config, ns, variants, kv.Value.Item1, 6, kv.Value.Item3, kv.Value.Item2, kv.Value.Item4);
                sb.AppendLine("    ]" + comma);
                index++;
            }
            sb.AppendLine("  }");
            sb.Append("}");
            return sb.ToString();
        }

        private string BuildWallBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"multipart\": [");

            sb.AppendLine("    {");
            sb.AppendLine("      \"when\": { \"up\": \"true\" },");
            sb.AppendLine("      \"apply\": [");
            AppendWeightedModels(sb, config, ns, variants, "_post", 8);
            sb.AppendLine("      ]");
            sb.AppendLine("    },");

            AppendWallSide(sb, config, ns, variants, "north", 0, false, true);
            sb.AppendLine(",");
            AppendWallSide(sb, config, ns, variants, "east", 90, false, true);
            sb.AppendLine(",");
            AppendWallSide(sb, config, ns, variants, "south", 180, false, true);
            sb.AppendLine(",");
            AppendWallSide(sb, config, ns, variants, "west", 270, false, true);
            sb.AppendLine(",");
            AppendWallSide(sb, config, ns, variants, "north", 0, true, false);
            sb.AppendLine(",");
            AppendWallSide(sb, config, ns, variants, "east", 90, true, false);
            sb.AppendLine(",");
            AppendWallSide(sb, config, ns, variants, "south", 180, true, false);
            sb.AppendLine(",");
            AppendWallSide(sb, config, ns, variants, "west", 270, true, false);
            sb.AppendLine();

            sb.AppendLine("  ]");
            sb.Append("}");
            return sb.ToString();
        }

        private void AppendWallSide(StringBuilder sb, GenerationConfig config, string ns, List<string> variants, string dir, int y, bool tall, bool includeClosingBrace)
        {
            var state = tall ? "tall" : "low";
            var suffix = tall ? "_side_tall" : "_side";
            sb.AppendLine("    {");
            sb.AppendLine("      \"when\": { \"" + dir + "\": \"" + state + "\" },");
            sb.AppendLine("      \"apply\": [");
            AppendWeightedModels(sb, config, ns, variants, suffix, 8, y, null, false);
            sb.AppendLine("      ]");
            sb.Append("    }");
            if (includeClosingBrace)
            {
                // just keep signature stable
            }
        }

        private string BuildDoorBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            var faces = new Dictionary<string, int>
            {
                { "east", 0 }, { "south", 90 }, { "west", 180 }, { "north", 270 }
            };

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"variants\": {");

            var entries = new List<string>();
            foreach (var half in new[] { "lower", "upper" })
            {
                foreach (var hinge in new[] { "left", "right" })
                {
                    foreach (var open in new[] { "false", "true" })
                    {
                        foreach (var facing in faces)
                        {
                            var suffix = GetDoorSuffix(half, hinge, open == "true");
                            var rotation = GetDoorY(facing.Key, hinge, open == "true", facing.Value);
                            var key = "facing=" + facing.Key + ",half=" + half + ",hinge=" + hinge + ",open=" + open;
                            entries.Add(BuildVariantEntry(config, ns, variants, key, suffix, rotation, null, false));
                        }
                    }
                }
            }

            sb.Append(string.Join(",\r\n", entries));
            sb.AppendLine();
            sb.AppendLine("  }");
            sb.Append("}");
            return sb.ToString();
        }

        private string BuildTrapdoorBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            var facings = new Dictionary<string, int>
            {
                { "north", 0 }, { "south", 180 }, { "west", 270 }, { "east", 90 }
            };

            var entries = new List<string>();
            foreach (var open in new[] { false, true })
            {
                foreach (var half in new[] { "bottom", "top" })
                {
                    foreach (var facing in facings)
                    {
                        var suffix = open ? "_open" : (half == "top" ? "_top" : "_bottom");
                        int y = facing.Value;
                        int? x = null;
                        bool uv = false;
                        if (open)
                        {
                            x = 90;
                            uv = true;
                        }
                        var key = "facing=" + facing.Key + ",half=" + half + ",open=" + open.ToString().ToLowerInvariant();
                        entries.Add(BuildVariantEntry(config, ns, variants, key, suffix, y, x, uv));
                    }
                }
            }

            return "{\r\n  \"variants\": {\r\n" + string.Join(",\r\n", entries) + "\r\n  }\r\n}";
        }

        private string BuildFenceGateBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            var facings = new Dictionary<string, int>
            {
                { "south", 0 }, { "west", 90 }, { "north", 180 }, { "east", 270 }
            };

            var entries = new List<string>();
            foreach (var facing in facings)
            {
                foreach (var open in new[] { false, true })
                {
                    foreach (var inWall in new[] { false, true })
                    {
                        var suffix = inWall ? (open ? "_wall_open" : "_wall") : (open ? "_open" : "");
                        var key = "facing=" + facing.Key + ",in_wall=" + inWall.ToString().ToLowerInvariant() + ",open=" + open.ToString().ToLowerInvariant();
                        entries.Add(BuildVariantEntry(config, ns, variants, key, suffix, facing.Value, null, false));
                    }
                }
            }

            return "{\r\n  \"variants\": {\r\n" + string.Join(",\r\n", entries) + "\r\n  }\r\n}";
        }

        private string BuildFenceBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"multipart\": [");
            sb.AppendLine("    {");
            sb.AppendLine("      \"apply\": [");
            AppendWeightedModels(sb, config, ns, variants, "_post", 8);
            sb.AppendLine("      ]");
            sb.AppendLine("    },");

            AppendFenceSide(sb, config, ns, variants, "north", 0);
            sb.AppendLine(",");
            AppendFenceSide(sb, config, ns, variants, "east", 90);
            sb.AppendLine(",");
            AppendFenceSide(sb, config, ns, variants, "south", 180);
            sb.AppendLine(",");
            AppendFenceSide(sb, config, ns, variants, "west", 270);
            sb.AppendLine();
            sb.AppendLine("  ]");
            sb.Append("}");
            return sb.ToString();
        }

        private void AppendFenceSide(StringBuilder sb, GenerationConfig config, string ns, List<string> variants, string dir, int y)
        {
            sb.AppendLine("    {");
            sb.AppendLine("      \"when\": { \"" + dir + "\": \"true\" },");
            sb.AppendLine("      \"apply\": [");
            AppendWeightedModels(sb, config, ns, variants, "_side", 8, y, null, false);
            sb.AppendLine("      ]");
            sb.Append("    }");
        }

        private string BuildVariantEntry(GenerationConfig config, string ns, List<string> variants, string key, string suffix, int y, int? x, bool uvlock)
        {
            var sb = new StringBuilder();
            sb.AppendLine("    \"" + key + "\": [");
            AppendWeightedModels(sb, config, ns, variants, suffix, 6, y, x, uvlock);
            sb.Append("    ]");
            return sb.ToString();
        }

        private string BuildTorchBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            return "{\r\n  \"variants\": {\r\n" +
                   "    \"\": [\r\n" +
                   string.Join(",\r\n", variants.Select((variant, i) =>
                   {
                       var modelRef = ns + ":block/" + config.BlockName + "/" + variant;
                       var weight = config.Weights != null && config.Weights.ContainsKey(variant) ? config.Weights[variant] : 1;
                       var comma = i < variants.Count - 1 ? "," : string.Empty;
                       return $"      {{ \"model\": \"{modelRef}\", \"weight\": {weight} }}{comma}";
                   })) +
                   "\r\n    ],\r\n" +
                   "    \"facing=east\": [\r\n" +
                   string.Join(",\r\n", variants.Select((variant, i) =>
                   {
                       var modelRef = ns + ":block/" + config.BlockName + "/" + variant + "_wall";
                       var weight = config.Weights != null && config.Weights.ContainsKey(variant) ? config.Weights[variant] : 1;
                       var comma = i < variants.Count - 1 ? "," : string.Empty;
                       return $"      {{ \"model\": \"{modelRef}\", \"y\": 90, \"uvlock\": true, \"weight\": {weight} }}{comma}";
                   })) +
                   "\r\n    ],\r\n" +
                   "    \"facing=south\": [\r\n" +
                   string.Join(",\r\n", variants.Select((variant, i) =>
                   {
                       var modelRef = ns + ":block/" + config.BlockName + "/" + variant + "_wall";
                       var weight = config.Weights != null && config.Weights.ContainsKey(variant) ? config.Weights[variant] : 1;
                       var comma = i < variants.Count - 1 ? "," : string.Empty;
                       return $"      {{ \"model\": \"{modelRef}\", \"y\": 180, \"uvlock\": true, \"weight\": {weight} }}{comma}";
                   })) +
                   "\r\n    ],\r\n" +
                   "    \"facing=west\": [\r\n" +
                   string.Join(",\r\n", variants.Select((variant, i) =>
                   {
                       var modelRef = ns + ":block/" + config.BlockName + "/" + variant + "_wall";
                       var weight = config.Weights != null && config.Weights.ContainsKey(variant) ? config.Weights[variant] : 1;
                       var comma = i < variants.Count - 1 ? "," : string.Empty;
                       return $"      {{ \"model\": \"{modelRef}\", \"y\": 270, \"uvlock\": true, \"weight\": {weight} }}{comma}";
                   })) +
                   "\r\n    ],\r\n" +
                   "    \"facing=north\": [\r\n" +
                   string.Join(",\r\n", variants.Select((variant, i) =>
                   {
                       var modelRef = ns + ":block/" + config.BlockName + "/" + variant + "_wall";
                       var weight = config.Weights != null && config.Weights.ContainsKey(variant) ? config.Weights[variant] : 1;
                       var comma = i < variants.Count - 1 ? "," : string.Empty;
                       return $"      {{ \"model\": \"{modelRef}\", \"y\": 0, \"uvlock\": true, \"weight\": {weight} }}{comma}";
                   })) +
                   "\r\n    ]\r\n  }\r\n}";
        }

        private string BuildPaneBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"multipart\": [");

            sb.AppendLine("    {");
            sb.AppendLine("      \"apply\": [");
            AppendWeightedModels(sb, config, ns, variants, "_post", 8);
            sb.AppendLine("      ]");
            sb.AppendLine("    },");

            AppendPaneSide(sb, config, ns, variants, "north", 0, false);
            sb.AppendLine(",");
            AppendPaneSide(sb, config, ns, variants, "east", 90, false);
            sb.AppendLine(",");
            AppendPaneSide(sb, config, ns, variants, "south", 180, false);
            sb.AppendLine(",");
            AppendPaneSide(sb, config, ns, variants, "west", 270, false);
            sb.AppendLine(",");
            AppendPaneSide(sb, config, ns, variants, "north", 0, true);
            sb.AppendLine(",");
            AppendPaneSide(sb, config, ns, variants, "east", 90, true);
            sb.AppendLine(",");
            AppendPaneSide(sb, config, ns, variants, "south", 180, true);
            sb.AppendLine(",");
            AppendPaneSide(sb, config, ns, variants, "west", 270, true);
            sb.AppendLine();

            sb.AppendLine("  ]");
            sb.Append("}");
            return sb.ToString();
        }

        private void AppendPaneSide(StringBuilder sb, GenerationConfig config, string ns, List<string> variants, string dir, int y, bool alt)
        {
            string suffix = alt ? "_side_alt" : "_side";

            sb.AppendLine("    {");
            sb.AppendLine("      \"when\": { \"" + dir + "\": \"true\" },");
            sb.AppendLine("      \"apply\": [");
            AppendWeightedModels(sb, config, ns, variants, suffix, 8, y, null, true);
            sb.AppendLine("      ]");
            sb.Append("    }");
        }

        private string BuildRailBlockstate(GenerationConfig config, string ns, List<string> variants)
        {
            var entries = new List<string>
    {
        BuildVariantEntry(config, ns, variants, "shape=north_south", "_flat", 0, null, false),
        BuildVariantEntry(config, ns, variants, "shape=east_west", "_flat", 90, null, false),

        BuildVariantEntry(config, ns, variants, "shape=ascending_east", "_raised_ne", 90, null, false),
        BuildVariantEntry(config, ns, variants, "shape=ascending_west", "_raised_sw", 90, null, false),
        BuildVariantEntry(config, ns, variants, "shape=ascending_north", "_raised_ne", 0, null, false),
        BuildVariantEntry(config, ns, variants, "shape=ascending_south", "_raised_sw", 0, null, false),

        BuildVariantEntry(config, ns, variants, "shape=south_east", "_curved", 0, null, false),
        BuildVariantEntry(config, ns, variants, "shape=south_west", "_curved", 90, null, false),
        BuildVariantEntry(config, ns, variants, "shape=north_west", "_curved", 180, null, false),
        BuildVariantEntry(config, ns, variants, "shape=north_east", "_curved", 270, null, false)
    };

            return "{\r\n  \"variants\": {\r\n" + string.Join(",\r\n", entries) + "\r\n  }\r\n}";
        }

        private string GetDoorSuffix(string half, string hinge, bool open)
        {
            var halfPrefix = half == "lower" ? "_bottom" : "_top";
            var hingePart = hinge == "left" ? "_left" : "_right";
            return halfPrefix + hingePart + (open ? "_open" : "");
        }

        private int GetDoorY(string facing, string hinge, bool open, int closedY)
        {
            if (!open)
                return closedY;

            switch (facing)
            {
                case "east": return hinge == "left" ? 90 : 270;
                case "south": return hinge == "left" ? 180 : 0;
                case "west": return hinge == "left" ? 270 : 90;
                case "north": return hinge == "left" ? 0 : 180;
                default: return closedY;
            }
        }

        private void AppendWeightedModels(StringBuilder sb, GenerationConfig config, string ns, List<string> variants, string suffix, int indentSpaces, int? y = null, int? x = null, bool uvlock = false)
        {
            var indent = new string(' ', indentSpaces);
            for (int i = 0; i < variants.Count; i++)
            {
                var variant = variants[i];
                var modelRef = ns + ":block/" + config.BlockName + "/" + variant + suffix;
                var weight = 1;
                if (config.Weights != null && config.Weights.ContainsKey(variant))
                    weight = config.Weights[variant];

                var parts = new List<string>();
                parts.Add("\"model\": \"" + modelRef + "\"");
                if (x.HasValue) parts.Add("\"x\": " + x.Value);
                if (y.HasValue) parts.Add("\"y\": " + y.Value);
                if (uvlock) parts.Add("\"uvlock\": true");
                parts.Add("\"weight\": " + weight);

                var comma = i < variants.Count - 1 ? "," : string.Empty;
                sb.AppendLine(indent + "{ " + string.Join(", ", parts) + " }" + comma);
            }
        }

        private string GetSecondaryBlockName(GenerationConfig config)
        {
            string suffix = (config.VariantPattern ?? string.Empty).Replace("{variant}", "");
            return config.BlockName + suffix;
        }

        private string BuildSingleModelBlockstate(GenerationConfig config, string ns, string modelVariant)
        {
            string modelRef = $"{ns}:block/{config.BlockName}/{modelVariant}";

            return "{\r\n" +
                   "  \"variants\": {\r\n" +
                   "    \"\": [\r\n" +
                   $"      {{ \"model\": \"{modelRef}\", \"weight\": 1 }}\r\n" +
                   "    ]\r\n" +
                   "  }\r\n" +
                   "}";
        }
    }
}
