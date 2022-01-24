﻿using RoR2.Skills;
using UnityEngine;

namespace UltimateCustomRun
{
    public static class StridesOfHeresy
    {
        public static void StridesChanges()
        {
            var ss = Resources.Load<SkillDef>("skilldefs/lunarreplacements/lunarutilityreplacement");
            ss.baseMaxStock = Main.StridesCharges.Value;
            ss.baseRechargeInterval = Main.StridesCooldown.Value;
            ss.rechargeStock = Main.StridesCharges.Value;
        }
        // very unfinished
    }
}
