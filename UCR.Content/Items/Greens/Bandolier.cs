using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class Bandolier : ItemBase
    {
        public static float Base;
        public static float Exponent;

        public override string Name => ":: Items :: Greens :: Bandolier";
        public override string InternalPickupToken => "bandolier";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>" + Mathf.Round((1f - 1f / Mathf.Pow((1f + Base), Exponent)) * 100f) + "%</style> <style=cStack>(+" + (Mathf.Round((1f - 1f / Mathf.Pow((2f + Base), Exponent)) * 100f) - Mathf.Round((1f - 1f / Mathf.Pow((1f + Base), Exponent)) * 100f)) + "% on stack)</style> Chance on kill to drop an ammo pack that <style=cIsUtility>resets all skill cooldowns</style>.";

        public override void Init()
        {
            Base = ConfigOption(1f, "Base", "Vanilla is 1\nFormula:\n1 - 1 / (stack + Base)^Exponent) * 100\nIf you want to make Stacking better, decrease the Base and increase the Exponent.");
            ROSOption("Greens", 0f, 3f, 0.03f, "2");
            Exponent = ConfigOption(0.33f, "Exponent", "Decimal. Vanilla is 0.33");
            ROSOption("Greens", 0f, 3f, 0.03f, "2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeBase;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeExponent;
        }

        public static void
        ChangeBase(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(1)
            );
            c.Index += 3;
            c.Next.Operand = Base;
        }

        public static void ChangeExponent(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchAdd(),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.33f)
            );
            c.Index += 2;
            c.Next.Operand = Exponent;
        }
    }
}