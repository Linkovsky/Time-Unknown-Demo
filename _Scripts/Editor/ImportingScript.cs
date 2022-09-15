using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportingScript : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        TextureImporter importer = (TextureImporter)assetImporter;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.textureType = TextureImporterType.Sprite;
        importer.filterMode = FilterMode.Point;
        importer.alphaIsTransparency = true;

        importer.SaveAndReimport();
    }
}
