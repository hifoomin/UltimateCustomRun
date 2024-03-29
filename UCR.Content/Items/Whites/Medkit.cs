﻿using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class Medkit : ItemBase
    {
        public static float FlatHealing;
        public static float PercentHealing;

        public override string Name => ":: Items : Whites :: Medkit";
        public override string InternalPickupToken => "medkit";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "2 seconds after getting hurt, <style=cIsHealing>heal</style> for <style=cIsHealing>" + FlatHealing + "</style> plus an additional <style=cIsHealing>" + d(PercentHealing) + " <style=cStack>(+" + d(PercentHealing) + " per stack)</style></style> of <style=cIsHealing>maximum health</style>.";

        public override void Init()
        {
            FlatHealing = ConfigOption(20f, "Flat Healing", "Vanilla is 20");
            PercentHealing = ConfigOption(0.05f, "Percent Healing", "Decimal. Per Stack. Vanilla is 0.05");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld(typeof(RoR2Content.Items), "Medkit"),
                    x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                    x => x.MatchStloc(0),
                    x => x.MatchLdcR4(20f)))
            {
                c.Index += 3;
                c.Next.Operand = FlatHealing;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Medkit Flat Healing hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdarg(0),
                    x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetMethod("get_maxHealth")),
                    x => x.MatchLdcR4(0.05f)))
            {
                c.Index += 2;
                c.Next.Operand = PercentHealing;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Medkit Percent Healing hook");
            }
        }
    }
}