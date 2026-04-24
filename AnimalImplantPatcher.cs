using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Nanotech
{
    [StaticConstructorOnStartup]
    public static class AnimalImplantPatcher
    {
        static AnimalImplantPatcher()
        {
            LongEventHandler.ExecuteWhenFinished(PatchAnimalRecipes);
        }

        private static void PatchAnimalRecipes()
        {
            var recipeNames = new[]
            {
                "InstallNT_NanoserumImplant", //nanoserum padrão né
                "InstallNT_HermesImplant", //hermes GOTTA GO FAST
                "InstallNT_FeelGoodImplant", //brave new world simulator
                "Install_SFI", //hmmmm hora de PROCRIAR
                "InstallRestlessImplant", //no more napping
                "InstallNullBiteImplant", //poxa mas eu amo tanto comer...

                // remover
                "RemoveNT_NanoSerum",
                "RemoveNT_HermesImplant",
                "RemoveNT_FeelGoodImplant",
                "Remove_SFI",
                "RemoveRestlessImplant",
                "RemoveNullBiteImplant",
                // adiciona outros aqui futuramente se precisar
            };

            foreach (var name in recipeNames)
            {
                var recipe = DefDatabase<RecipeDef>.GetNamedSilentFail(name);
                if (recipe == null)
                {
                    Log.Warning($"[Nanotech] Recipe '{name}' not found during animal patch.");
                    continue;
                }

                recipe.recipeUsers ??= new List<ThingDef>();

                foreach (var def in DefDatabase<ThingDef>.AllDefsListForReading)
                {
                    if (def.race == null || def.race.Humanlike) continue;
                    if (!recipe.recipeUsers.Contains(def))
                        recipe.recipeUsers.Add(def);
                }
            }
        }
    }
}