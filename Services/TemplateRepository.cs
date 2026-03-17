using System.Collections.Generic;
using MinecraftJsonGenerator.Models;

namespace MinecraftJsonGenerator.Services
{
    public static class TemplateRepository
    {
        public static BlockTemplateDefinition Get(BlockTemplateType type)
        {
            switch (type)
            {
                case BlockTemplateType.Block:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "all" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "", Parent = "minecraft:block/cube_all", TextureKeys = new List<string> { "all" } }
                        }
                    };

                case BlockTemplateType.Slab:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "bottom", "top", "side" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "", Parent = "minecraft:block/slab", TextureKeys = new List<string> { "bottom", "top", "side" } },
                            new ModelFileTemplate { FileSuffix = "_top", Parent = "minecraft:block/slab_top", TextureKeys = new List<string> { "bottom", "top", "side" } }
                        }
                    };

                case BlockTemplateType.Stairs:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "bottom", "top", "side" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "", Parent = "minecraft:block/stairs", TextureKeys = new List<string> { "bottom", "top", "side" } },
                            new ModelFileTemplate { FileSuffix = "_inner", Parent = "minecraft:block/inner_stairs", TextureKeys = new List<string> { "bottom", "top", "side" } },
                            new ModelFileTemplate { FileSuffix = "_outer", Parent = "minecraft:block/outer_stairs", TextureKeys = new List<string> { "bottom", "top", "side" } }
                        }
                    };

                case BlockTemplateType.Wall:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "_inventory",
                        ManualTextureKeys = new List<string> { "wall" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "_inventory", Parent = "minecraft:block/wall_inventory", TextureKeys = new List<string> { "wall" } },
                            new ModelFileTemplate { FileSuffix = "_post", Parent = "minecraft:block/template_wall_post", TextureKeys = new List<string> { "wall" } },
                            new ModelFileTemplate { FileSuffix = "_side", Parent = "minecraft:block/template_wall_side", TextureKeys = new List<string> { "wall" } },
                            new ModelFileTemplate { FileSuffix = "_side_tall", Parent = "minecraft:block/template_wall_side_tall", TextureKeys = new List<string> { "wall" } }
                        }
                    };

                case BlockTemplateType.Door:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "_bottom_left",
                        ManualTextureKeys = new List<string> { "top", "bottom" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "_bottom_left", Parent = "minecraft:block/door_bottom_left", TextureKeys = new List<string> { "top", "bottom" } },
                            new ModelFileTemplate { FileSuffix = "_bottom_left_open", Parent = "minecraft:block/door_bottom_left_open", TextureKeys = new List<string> { "top", "bottom" } },
                            new ModelFileTemplate { FileSuffix = "_bottom_right", Parent = "minecraft:block/door_bottom_right", TextureKeys = new List<string> { "top", "bottom" } },
                            new ModelFileTemplate { FileSuffix = "_bottom_right_open", Parent = "minecraft:block/door_bottom_right_open", TextureKeys = new List<string> { "top", "bottom" } },
                            new ModelFileTemplate { FileSuffix = "_top_left", Parent = "minecraft:block/door_top_left", TextureKeys = new List<string> { "top", "bottom" } },
                            new ModelFileTemplate { FileSuffix = "_top_left_open", Parent = "minecraft:block/door_top_left_open", TextureKeys = new List<string> { "top", "bottom" } },
                            new ModelFileTemplate { FileSuffix = "_top_right", Parent = "minecraft:block/door_top_right", TextureKeys = new List<string> { "top", "bottom" } },
                            new ModelFileTemplate { FileSuffix = "_top_right_open", Parent = "minecraft:block/door_top_right_open", TextureKeys = new List<string> { "top", "bottom" } }
                        }
                    };

                case BlockTemplateType.Leaves:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "all" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "", Parent = "minecraft:block/leaves", TextureKeys = new List<string> { "all" } }
                        }
                    };

                case BlockTemplateType.Trapdoor:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "_bottom",
                        ManualTextureKeys = new List<string> { "texture" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "_bottom", Parent = "minecraft:block/template_trapdoor_bottom", TextureKeys = new List<string> { "texture" } },
                            new ModelFileTemplate { FileSuffix = "_open", Parent = "minecraft:block/template_trapdoor_open", TextureKeys = new List<string> { "texture" } },
                            new ModelFileTemplate { FileSuffix = "_top", Parent = "minecraft:block/template_trapdoor_top", TextureKeys = new List<string> { "texture" } }
                        }
                    };

                case BlockTemplateType.Column:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "end", "side" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "", Parent = "minecraft:block/cube_column", TextureKeys = new List<string> { "end", "side" } }
                        }
                    };

                case BlockTemplateType.Column2:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "end", "side" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "", Parent = "minecraft:block/cube_column", TextureKeys = new List<string> { "end", "side" } },
                            new ModelFileTemplate { FileSuffix = "_horizontal", Parent = "minecraft:block/cube_column_horizontal", TextureKeys = new List<string> { "end", "side" } }
                        }
                    };

                case BlockTemplateType.FenceGate:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "texture" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "", Parent = "minecraft:block/template_fence_gate", TextureKeys = new List<string> { "texture" } },
                            new ModelFileTemplate { FileSuffix = "_open", Parent = "minecraft:block/template_fence_gate_open", TextureKeys = new List<string> { "texture" } },
                            new ModelFileTemplate { FileSuffix = "_wall", Parent = "minecraft:block/template_fence_gate_wall", TextureKeys = new List<string> { "texture" } },
                            new ModelFileTemplate { FileSuffix = "_wall_open", Parent = "minecraft:block/template_fence_gate_wall_open", TextureKeys = new List<string> { "texture" } }
                        }
                    };

                case BlockTemplateType.Fence:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "_inventory",
                        ManualTextureKeys = new List<string> { "texture" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate { FileSuffix = "_inventory", Parent = "minecraft:block/fence_inventory", TextureKeys = new List<string> { "texture" } },
                            new ModelFileTemplate { FileSuffix = "_post", Parent = "minecraft:block/fence_post", TextureKeys = new List<string> { "texture" } },
                            new ModelFileTemplate { FileSuffix = "_side", Parent = "minecraft:block/fence_side", TextureKeys = new List<string> { "texture" } }
                        }
                    };

                case BlockTemplateType.Cross:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "cross" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate
                            {
                                FileSuffix = "",
                                Parent = "minecraft:block/cross",
                                TextureKeys = new List<string> { "cross" }
                            }
                        }
                    };

                case BlockTemplateType.Carpet:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "wool" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate
                            {
                                FileSuffix = "",
                                Parent = "minecraft:block/carpet",
                                TextureKeys = new List<string> { "wool" }
                            }
                        }
                    };

                case BlockTemplateType.Torch:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "",
                        ManualTextureKeys = new List<string> { "torch" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate
                            {
                                FileSuffix = "",
                                Parent = "minecraft:block/template_torch",
                                TextureKeys = new List<string> { "torch" }
                            },
                            new ModelFileTemplate
                            {
                                FileSuffix = "_wall",
                                Parent = "minecraft:block/template_torch_wall",
                                TextureKeys = new List<string> { "torch" }
                            }
                        }
                    };

                case BlockTemplateType.Pane:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "_post",
                        ManualTextureKeys = new List<string> { "pane", "edge" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate
                            {
                                FileSuffix = "_post",
                                Parent = "minecraft:block/template_glass_pane_post",
                                TextureKeys = new List<string> { "pane", "edge" }
                            },
                            new ModelFileTemplate
                            {
                                FileSuffix = "_side",
                                Parent = "minecraft:block/template_glass_pane_side",
                                TextureKeys = new List<string> { "pane", "edge" }
                            },
                            new ModelFileTemplate
                            {
                                FileSuffix = "_side_alt",
                                Parent = "minecraft:block/template_glass_pane_side_alt",
                                TextureKeys = new List<string> { "pane", "edge" }
                            },
                            new ModelFileTemplate
                            {
                                FileSuffix = "_noside",
                                Parent = "minecraft:block/template_glass_pane_noside",
                                TextureKeys = new List<string> { "pane", "edge" }
                            },
                            new ModelFileTemplate
                            {
                                FileSuffix = "_noside_alt",
                                Parent = "minecraft:block/template_glass_pane_noside_alt",
                                TextureKeys = new List<string> { "pane", "edge" }
                            }
                        }
                    };

                case BlockTemplateType.Rail:
                    return new BlockTemplateDefinition
                    {
                        ItemModelSuffix = "_flat",
                        ManualTextureKeys = new List<string> { "rail" },
                        ModelFiles = new List<ModelFileTemplate>
                        {
                            new ModelFileTemplate
                            {
                                FileSuffix = "_flat",
                                Parent = "minecraft:block/rail_flat",
                                TextureKeys = new List<string> { "rail" }
                            },
                            new ModelFileTemplate
                            {
                                FileSuffix = "_curved",
                                Parent = "minecraft:block/rail_curved",
                                TextureKeys = new List<string> { "rail" }
                            },
                            new ModelFileTemplate
                            {
                                FileSuffix = "_raised_ne",
                                Parent = "minecraft:block/template_rail_raised_ne",
                                TextureKeys = new List<string> { "rail" }
                            },
                            new ModelFileTemplate
                            {
                                FileSuffix = "_raised_sw",
                                Parent = "minecraft:block/template_rail_raised_sw",
                                TextureKeys = new List<string> { "rail" }
                            }
                        }
                    };

                default:
                    return Get(BlockTemplateType.Block);
            }
        }

        public static List<string> GetComboItems()
        {
            return new List<string>
            {
                "Block",
                "Slab",
                "Stairs",
                "Wall",
                "Door",
                "Leaves",
                "Trapdoor",
                "Column",
                "Column 2",
                "Fence Gate",
                "Fence",

                "Cross",
                "Carpet",
                "Torch",
                "Pane",
                "Rail",
            };
        }
    }
}
