using Asphalt;
using Asphalt.Service;
using Asphalt.Storeable;
using Asphalt.Utils;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Components;
using Eco.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RecipeRemoverMod
{
    [AsphaltPlugin("RecipeRemoverMod")]
    public class RecipeRemoverMod : IModKitPlugin
    {
        [Inject]
        [StorageLocation("Config")]
        public static IStorage ConfigStorage { get; set; }

        public static void OnRecipesInitialized()
        {
            try
            {
                Dictionary<Type, Recipe[]> staticRecipes = (Dictionary<Type, Recipe[]>)typeof(CraftingComponent).GetFields(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name.Contains("staticRecipes")).GetValue(null);
                var keyTypes = staticRecipes.Values.SelectMany(r => r).Select(r => r.GetType());

                //       var allRecipes = typeof(Recipe).InstancesOfCreatableTypesParallel<Recipe>().Select(r => r.GetType()).ToArray();
                foreach (var type in keyTypes.ToArray())  //copy list because it will be modified from RemoveRecipe
                {
                    bool? config = ConfigStorage.Get<bool?>(type.FullName);

                    if (config == null)
                    {
                        ConfigStorage.Set<bool>(type.FullName, false);
                        continue;
                    }

                    if (config == true)
                        RecipeRemover.RemoveRecipe(type);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public string GetStatus()
        {
            return "running...";
        }
    }
}
