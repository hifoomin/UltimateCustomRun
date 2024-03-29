﻿using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class GhorsTome : ItemBase
    {
        public static float Chance;
        public static int Reward;

        public override string Name => ":: Items :: Greens :: Ghors Tome";
        public override string InternalPickupToken => "bonusGoldPackOnKill";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>" + Chance + "%</style> <style=cStack>(+" + Chance + "% on stack)</style> Chance on kill to drop a treasure worth <style=cIsUtility>$" + Reward + "</style>. <style=cIsUtility>Scales over time.</style>";

        public override void Init()
        {
            Chance = ConfigOption(4f, "Chance", "Per Stack. Vanilla is 4");
            Reward = ConfigOption(25, "Gold Gain", "Vanilla is 25");
            base.Init();
        }

        public override void Hooks()
        {
            ChangeReward();
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeChance;
        }

        public static void ChangeReward()
        {
            var gtc = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/BonusMoneyPack").GetComponentInChildren<MoneyPickup>();
            gtc.baseGoldReward = Reward;
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBle(out _),
                    x => x.MatchLdcR4(4f)))
            {
                c.Index += 1;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Ghor's Tome Chance hook");
            }
        }
    }
}