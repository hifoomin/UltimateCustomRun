﻿using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class Headstompers : ItemBase
    {
        public static float Cooldown;
        public static float MinimumRadius;
        public static float MaximumRadius;
        public static float MinimumDamage;
        public static float MaximumDamage;
        public static float JumpBoost;
        public override string Name => ":: Items ::: Reds :: H3AD-5T v2";
        public override string InternalPickupToken => "fallBoots";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Increase <style=cIsUtility>jump height</style>. Creates a <style=cIsDamage>" + MinimumRadius + "m-" + MaximumRadius + "m</style> radius <style=cIsDamage>kinetic explosion</style> on hitting the ground, dealing <style=cIsDamage>" + d(MinimumDamage) + "-" + d(MaximumDamage) + "</style> base damage that scales up with <style=cIsDamage>fall distance</style>. Recharges in <style=cIsDamage>" + Cooldown + "</style> <style=cStack>(-50% per stack)</style> seconds.";

        public override void Init()
        {
            Cooldown = ConfigOption(10f, "Cooldown", "Vanilla is 10");
            JumpBoost = ConfigOption(2f, "Jump Height Multiplier", "Vanilla is 2");
            MinimumRadius = ConfigOption(5f, "Minimum Range", "Vanilla is 5");
            MaximumRadius = ConfigOption(100f, "Maximum Range", "Vanilla is 100");
            MinimumDamage = ConfigOption(10f, "Minimum Damage", "Vanilla is 10");
            MaximumDamage = ConfigOption(100f, "Maximum Damage", "Vanilla is 100");
            base.Init();
        }

        public override void Hooks()
        {
            IL.EntityStates.Headstompers.HeadstompersIdle.FixedUpdateAuthority += ChangeJumpHeight;
            Changes();
        }

        public static void Changes()
        {
            On.EntityStates.Headstompers.HeadstompersCooldown.OnEnter += (orig, self) =>
            {
                EntityStates.Headstompers.HeadstompersCooldown.baseDuration = Cooldown;
                orig(self);
            };
            On.EntityStates.Headstompers.HeadstompersFall.OnEnter += (orig, self) =>
            {
                EntityStates.Headstompers.HeadstompersFall.minimumRadius = MinimumRadius;
                EntityStates.Headstompers.HeadstompersFall.maximumRadius = MaximumRadius;
                EntityStates.Headstompers.HeadstompersFall.minimumDamageCoefficient = MinimumDamage;
                EntityStates.Headstompers.HeadstompersFall.maximumDamageCoefficient = MaximumDamage;
                orig(self);
            };
        }

        public static void ChangeJumpHeight(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(2f)))
            {
                c.Next.Operand = JumpBoost;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply H3AD-5T v2 Jump Boost hook");
            }
        }
    }
}