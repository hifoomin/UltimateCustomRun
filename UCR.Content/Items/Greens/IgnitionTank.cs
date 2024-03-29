﻿using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class IgnitionTank : ItemBase
    {
        public static int Damage;
        public static float BurnChance;
        public static bool StackBurnChance;
        public static float BurnChancePerStack;

        public override string Name => ":: Items :: Greens :: Ignition Tank";
        public override string InternalPickupToken => "strengthenBurn";
        public override bool NewPickup => true;

        public override string PickupText => (BurnChance > 0 ? "Gain " + BurnChance + "% chance to ignite enemies on hit. " : "") +
                                             "Your ignite effects deal +" + d(Damage) + "% damage.";

        public override string DescText => (BurnChance > 0 ? "Gain " + BurnChance + "%" + (StackBurnChance ? " <style=cStack>(+" + BurnChancePerStack + "% per stack)</style>" : "") + " chance to <style=cIsDamage>ignite</style> enemies on hit. " : "") +
                                           "Ignite effects deal <style=cIsDamage>+" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> more damage over time.";

        public override void Init()
        {
            Damage = ConfigOption(3, "Burn Damage", "Decimal. Per Stack. Vanilla is 3");
            BurnChance = ConfigOption(0f, "Base Burn Chance", "Vanilla is 0");
            BurnChancePerStack = ConfigOption(0f, "Stack Burn Chance", "Per Stack. Vanilla is 0");
            StackBurnChance = ConfigOption(false, "Make Burn Chance Stack?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.StrengthenBurnUtils.CheckDotForUpgrade += ChangeDamage;
            On.RoR2.GlobalEventManager.OnHitEnemy += AddBurn;
        }

        public void AddBurn(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (damageInfo.attacker && damageInfo.attacker.GetComponent<CharacterBody>() && damageInfo.attacker.GetComponent<CharacterMaster>().inventory)
            {
                var inv = damageInfo.attacker.GetComponent<CharacterMaster>().inventory;
                var stack = inv.GetItemCount(DLC1Content.Items.StrengthenBurn);
                if (stack > 0)
                {
                    switch (StackBurnChance)
                    {
                        default:
                            if (Util.CheckRoll(BurnChance * damageInfo.procCoefficient + BurnChance * damageInfo.procCoefficient * (stack - 1), damageInfo.attacker.GetComponent<CharacterBody>().master.luck))
                            {
                                InflictDotInfo blaze = new()
                                {
                                    attackerObject = damageInfo.attacker,
                                    victimObject = victim,
                                    dotIndex = DotController.DotIndex.Burn,
                                    damageMultiplier = 1f
                                };
                                DotController.InflictDot(ref blaze);
                            }
                            break;

                        case false:
                            if (Util.CheckRoll(BurnChance * damageInfo.procCoefficient, damageInfo.attacker.GetComponent<CharacterBody>().master.luck))
                            {
                                InflictDotInfo blaze = new()
                                {
                                    attackerObject = damageInfo.attacker,
                                    victimObject = victim,
                                    dotIndex = DotController.DotIndex.Burn,
                                    damageMultiplier = 1f
                                };
                                DotController.InflictDot(ref blaze);
                            }
                            break;
                    }
                }
            }
            orig(self, damageInfo, victim);
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(3)))
            {
                c.Index += 1;
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Ignition Tank Burn Damage hook");
            }
        }
    }
}