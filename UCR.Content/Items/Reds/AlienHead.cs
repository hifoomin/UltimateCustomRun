using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Reds
{
    public class AlienHead : ItemBase
    {
        public static float CooldownReduction;
        public static float FlatCDR;
        public static bool StackFlatCDR;

        public override string Name => ":: Items ::: Reds :: Alien Head";
        public override string InternalPickupToken => "alienHead";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>Reduce skill cooldowns</style> by <style=cIsUtility>" + d(CooldownReduction) + "</style> <style=cStack>(+" + d(CooldownReduction) + " per stack)</style>" +
                                           (FlatCDR != 0f ? " and <style=cIsUtility>" + FlatCDR + "</style> second" +
                                           (StackFlatCDR ? "<style=cStack>(+" + FlatCDR + " per stack)</style>" : ".") : ".");

        public override void Init()
        {
            CooldownReduction = ConfigOption(0.25f, "Percent Cooldown Reduction", "Decimal. Per Stack. Vanilla is 0.25");
            ROSOption("Greens", 0f, 1f, 0.05f, "3");
            FlatCDR = ConfigOption(0f, "Flat Cooldown Reduction", "Vanilla is 0");
            ROSOption("Greens", 0f, 10f, 0.25f, "3");
            StackFlatCDR = ConfigOption(false, "Stack Flat Cooldown Reduction?", "Vanilla is false");
            ROSOption("Greens", 0f, 10f, 1f, "3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCDR;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void ChangeCDR(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.75f),
                x => x.MatchMul()
            );
            c.Next.Operand = 1f - CooldownReduction;
        }

        // PLEASE HELP TO FIX
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.AlienHead);
                if (stack > 0)
                {
                    args.cooldownReductionAdd += StackFlatCDR ? FlatCDR * stack : FlatCDR;
                }
            }
        }

        // PLEASE HELP TO FIX
    }
}