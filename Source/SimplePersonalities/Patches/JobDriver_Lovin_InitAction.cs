﻿using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using VanillaSocialInteractionsExpanded;
using Verse;
using Verse.AI;

namespace SPM2.Patches
{
    [HarmonyPatch]
    static class JobDriver_Lovin_InitAction_Vanilla
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.GetDeclaredMethods(typeof(JobDriver_Lovin)).FirstOrDefault(x => x.Name.Contains("<MakeNewToils>") && x.ReturnType == typeof(void));
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var ticksLeftField = AccessTools.Field(typeof(JobDriver_Lovin), "ticksLeft");
            foreach (var code in instructions)
            {
                yield return code;
                if (!found && code.StoresField(ticksLeftField))
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JobDriver_Lovin_InitAction_Vanilla), nameof(ModifyLovinTicks)));
                }
            }
        }

        [TweakValue("0SimplePersonality", 0, 2)] public static float chanceOfPassionateLovin = 0.1f;
        [TweakValue("0SimplePersonality", 0, 2)] public static float passionateLovinDurationMultiplier = 2f;
        private static void ModifyLovinTicks(JobDriver_Lovin jobDriver)
        {
            var pawn = jobDriver.pawn;
            var parther = jobDriver.Partner;
            var interaction = PersonalityComparer.Compare(pawn, parther);
            if (interaction == PersonalityInteraction.Complementary && Rand.Chance(chanceOfPassionateLovin))
            {
                jobDriver.collideWithPawns = true; // we treat it as a bool to indicate that it's a passionate lovin
                parther.jobs.curDriver.collideWithPawns = true;
                jobDriver.ticksLeft *= 2;
            }
        }
    }

    [HarmonyPatch]
    static class JobDriver_Lovin_InitAction_VSIE
    {
        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.GetDeclaredMethods(typeof(JobDriver_LovinOneNightStand)).FirstOrDefault(x => x.Name.Contains("<MakeNewToils>") && x.ReturnType == typeof(void));
        }
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            var ticksLeftField = AccessTools.Field(typeof(JobDriver_LovinOneNightStand), "ticksLeft");
            foreach (var code in instructions)
            {
                yield return code;
                if (!found && code.StoresField(ticksLeftField))
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JobDriver_Lovin_InitAction_VSIE), nameof(ModifyLovinTicks)));
                }
            }
        }

        [TweakValue("0SimplePersonality", 0, 2)] public static float chanceOfPassionateLovin = 0.1f;
        [TweakValue("0SimplePersonality", 0, 2)] public static float passionateLovinDurationMultiplier = 2f;
        private static void ModifyLovinTicks(JobDriver_LovinOneNightStand jobDriver)
        {
            var pawn = jobDriver.pawn;
            var parther = jobDriver.Partner;
            var interaction = PersonalityComparer.Compare(pawn, parther);
            if (interaction == PersonalityInteraction.Complementary && Rand.Chance(chanceOfPassionateLovin))
            {
                jobDriver.collideWithPawns = true; // we treat it as a bool to indicate that it's a passionate lovin
                parther.jobs.curDriver.collideWithPawns = true;
                jobDriver.ticksLeft *= 2;
            }
        }
    }
}
