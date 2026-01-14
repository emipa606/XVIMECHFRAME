using UnityEngine;
using Verse;

namespace MParmorLibrary;

[StaticConstructorOnStartup]
public static class Colors
{
    public static readonly Texture2D machineBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color32(byte.MaxValue, 120, 0, byte.MaxValue));

    public static readonly Texture2D machineHurtBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color32(191, 0, 0, byte.MaxValue));

    public static readonly Texture2D machineEmptyBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color32(64, 0, 0, byte.MaxValue));

    public static readonly Texture2D shieldBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color32(0, 125, byte.MaxValue, byte.MaxValue));

    public static readonly Texture2D shieldHurtBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color32(0, 0, 190, byte.MaxValue));

    public static readonly Texture2D shieldEmptyBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color32(0, 0, 61, byte.MaxValue));

    public static readonly Texture2D shieldCDBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color32(0, 38, byte.MaxValue, byte.MaxValue));

    public static readonly Texture2D supplyBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color32(0, 128, 14, byte.MaxValue));

    public static readonly Texture2D powerBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color32(41, 251, 251, byte.MaxValue));

    public static readonly Material powerBarTex_Material =
        SolidColorMaterials.SimpleSolidColorMaterial(new Color32(41, 251, 251, byte.MaxValue));

    public static readonly Texture2D aqua = SolidColorMaterials.NewSolidColorTexture(new Color(0f, 1f, 1f));

    public static readonly Texture2D gray = SolidColorMaterials.NewSolidColorTexture(new Color(0.5f, 0.5f, 0.5f));

    public static readonly Texture2D labelMachineBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f / 3f, 0f, 0.65f));

    public static readonly Texture2D labelShieldBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0f, 25f / 51f, 1f, 0.65f));

    public static readonly Texture2D label_RedSkillB_FilledMatT =
        SolidColorMaterials.NewSolidColorTexture(new Color(1f, 0.05f, 0.05f, 0.65f));

    public static readonly Texture2D labelUnfilledMat =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.3f, 0.3f, 0.3f, 0.65f));

    public static readonly Texture2D labelUnfilledMatDark =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.1f, 0.1f, 1f));

    public static readonly Texture2D labelUnfilledMatBlack =
        SolidColorMaterials.NewSolidColorTexture(new Color(0.02f, 0.02f, 0.02f, 1f));
}