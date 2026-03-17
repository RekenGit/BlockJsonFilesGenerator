using System;
using System.Collections.Generic;

namespace MinecraftJsonGenerator.Services
{
    public static class VariantService
    {
        public static List<string> BuildVariantNames(string blockName, string variantPattern, int variantCount)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(blockName))
            {
                return result;
            }

            result.Add(blockName.Trim());

            var pattern = variantPattern ?? string.Empty;
            for (int i = 0; i < variantCount; i++)
            {
                var suffix = pattern.Replace("{variant}", i.ToString());
                result.Add(blockName.Trim() + suffix);
            }

            return result;
        }

        public static string NormalizeNamespace(string ns)
        {
            if (string.IsNullOrWhiteSpace(ns))
            {
                return "minecraft";
            }

            return ns.Trim().TrimEnd(':');
        }
    }
}
