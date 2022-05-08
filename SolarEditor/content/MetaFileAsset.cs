﻿using SolarSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SolarEditor
{
    public enum AssetType
    {
        INVALID = 0,
        MODEL,
        TEXTURE,
        SHADER,
        RENDER_GRAPH,
        SCENE
    }

    public class MetaFileAsset
    {
        public AssetType AssetType { get; set; }
        public Guid Guid { get; set; }

        public static MetaFileAsset GetOrCreateMetaFileAsset(string path)
        {
            path = Path.ChangeExtension(path, ".slo");
            if (File.Exists(path)) {                
                return JsonSerializer.Deserialize<MetaFileAsset>(File.ReadAllText(path));
            }

            MetaFileAsset metaFileAsset = new MetaFileAsset();
            metaFileAsset.Guid = Guid.NewGuid();

            Logger.Trace($"Creating meta file, {path}");
            string json = JsonSerializer.Serialize(metaFileAsset);
            File.WriteAllText(path, json);

            return metaFileAsset;
        }
    }


}
