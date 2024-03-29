﻿using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class BundleOfFireworks : ItemBase
    {
        public static int Count;
        public static int StackCount;
        public static float Damage;
        public static float ProcCoefficient;
        public static float AoE;

        public override string Name => ":: Items : Whites :: Bundle of Fireworks";
        public override string InternalPickupToken => "firework";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Activating an interactable <style=cIsDamage>launches " + Count + " <style=cStack>(+" + StackCount + " per stack)</style> fireworks</style> that deal <style=cIsDamage>" + d(Damage) + "</style> base damage.";

        public override void Init()
        {
            Count = ConfigOption(8, "Base Count", "Vanilla is 8");
            StackCount = ConfigOption(4, "Stack Count", "Per Stack. Vanilla is 4");
            Damage = ConfigOption(3f, "Damage", "Decimal. Vanilla is 3");
            ProcCoefficient = ConfigOption(0.2f, "Proc Coefficient", "Vanilla is 0.2");
            AoE = ConfigOption(5f, "Area of Effect", "Vanilla is 5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnInteractionBegin += ChangeCount;
            Changes();
        }

        public static void ChangeCount(ILContext il)
        {
            ILCursor c = new(il);
            //c.GotoNext(MoveType.Before,
            //x => x.MatchLdcI4(4),
            //x => x.MatchLdloc(out _),
            //x => x.MatchLdcI4(4)
            if (c.TryGotoNext(x => x.MatchStfld<RoR2.FireworkLauncher>("remaining")))
            {
                c.EmitDelegate<Func<int, int>>((val) =>
                {
                    return Count - StackCount + ((val - 4) / 4) * StackCount;
                });
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Bundle Of Fireworks Count hook");
            }
            //c.Next.Operand = FireworksCount.Value;
            //c.Index += 2;
            //c.Next.Operand = FireworksCountStack.Value;
        }

        public static void Changes()
        {
            var croppa = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/FireworkProjectile");
            var msm = croppa.GetComponent<ProjectileImpactExplosion>();
            var skm = croppa.GetComponent<ProjectileController>();
            msm.blastDamageCoefficient = Damage / 3f;
            skm.procCoefficient = ProcCoefficient;
            msm.blastRadius = AoE;
            // this is probably the wrong way of doing this but i cant figure out another
        }
    }
}