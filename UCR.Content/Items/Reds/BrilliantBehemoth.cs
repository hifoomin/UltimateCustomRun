using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class BrilliantBehemoth : ItemBase
    {
        public static float Damage;
        public static float Radius;
        public static float StackRadius;
        public override string Name => ":: Items ::: Reds :: Brilliant Behemoth";
        public override string InternalPickupToken => "behemoth";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "All your <style=cIsDamage>attacks explode</style> in a <style=cIsDamage>" + Radius + "m </style> <style=cStack>(+" + StackRadius + "m per stack)</style> radius for a bonus <style=cIsDamage>" + d(Damage) + "</style> TOTAL damage to nearby enemies.";

        public override void Init()
        {
            Damage = ConfigOption(0.6f, "Damage Increase", "Decimal. Vanilla is 0.6");
            ROSOption("Greens", 0f, 3f, 0.05f, "3");
            Radius = ConfigOption(4f, "Base Area of Effect", "Vanilla is 4");
            ROSOption("Greens", 0f, 20f, 0.5f, "3");
            StackRadius = ConfigOption(2.5f, "Stack Area of Effect", "Per Stack. Vanilla is 2.5");
            ROSOption("Greens", 0f, 20f, 0.5f, "3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitAll += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitAll += ChangeRadius;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.6f)
            );
            c.Next.Operand = Damage;
        }

        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f),
                x => x.MatchLdcR4(2.5f)
            );
            c.Next.Operand = Radius - StackRadius;
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }
    }
}