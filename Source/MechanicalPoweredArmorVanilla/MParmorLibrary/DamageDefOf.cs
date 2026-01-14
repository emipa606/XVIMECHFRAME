using RimWorld;
using Verse;

namespace MParmorLibrary;

[DefOf]
public static class DamageDefOf
{
    public static DamageDef MParmor_SlefDestructBomb;

    public static DamageDef MParmor_Black_BombDamage;

    public static DamageDef MParmor_FrostBullet;

    public static DamageDef MParmor_Bomb_IceSpike;

    public static DamageDef MParmor_Bomb_ColdWave;

    public static DamageDef XFUnused_BombWithDirection;

    static DamageDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(DamageDefOf));
    }
}