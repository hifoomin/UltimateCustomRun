﻿using RoR2;
using RoR2.Projectile;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    static class StickyBomb
    {
        public static void Changes()
        {
            var nowthatssticky = Resources.Load<GameObject>("prefabs/projectiles/stickybomb");
            var s = nowthatssticky.GetComponent<ProjectileImpactExplosion>();
            s.blastDamageCoefficient = Main.StickyBombDamage.Value / 1.8f;
            // weird
            s.blastRadius = Main.StickyBombRadius.Value;
            s.lifetime = Main.StickyBombDelay.Value;
            if (Main.StickyBombFalloff.Value)
            {
                s.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            }
            else
            {
                s.falloffModel = BlastAttack.FalloffModel.None;
            }
        }
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(5)
            );
            c.Index += 1;
            c.Next.Operand = Main.StickyBombChance.Value;
        }
    }
}