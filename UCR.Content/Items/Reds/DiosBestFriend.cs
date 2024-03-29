﻿using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun.Items.Reds
{
    public class DiosBestFriend : ItemBase
    {
        public static float Invincibility;
        public static float Delay;

        public override string Name => ":: Items ::: Reds :: Dios Best Friend";
        public override string InternalPickupToken => "extraLife";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>Upon death</style>, this item will be <style=cIsUtility>consumed</style> and you will <style=cIsHealing>return to life</style> with <style=cIsHealing>" + Invincibility + " seconds of invulnerability</style>.";

        public override void Init()
        {
            Invincibility = ConfigOption(3f, "Post-Revival Invincibility Duration", "Vanilla is 3");
            Delay = ConfigOption(2f, "Revival Delay", "Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterMaster.OnBodyDeath += ChangeDelay;
            IL.RoR2.CharacterMaster.RespawnExtraLife += ChangeInvinc;
        }

        private void ChangeInvinc(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f)))
            {
                c.Next.Operand = Invincibility;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Dios Best Friend Invincibility hook");
            }
        }

        private void ChangeDelay(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "ExtraLife"),
                x => x.MatchLdcI4(1)))
            {
                c.Index += 5;
                c.Next.Operand = Delay;
                c.Index += 4;
                c.Next.Operand = Mathf.Min(0.1f, Delay - 1);
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Dios Best Friend Delay hook");
            }
        }
    }
}