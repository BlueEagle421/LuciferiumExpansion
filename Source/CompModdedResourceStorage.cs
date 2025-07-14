
using PipeSystem;
using Verse;


namespace LuciferiumExpansion
{
    public class CompProperties_ModdedResourceStorage : CompProperties_ResourceStorage
    {
        public CompProperties_ModdedResourceStorage() => this.compClass = typeof(CompModdedResourceStorage);
    }

    [StaticConstructorOnStartup]
    public class CompModdedResourceStorage : CompResourceStorage
    {
        public override string CompInspectStringExtra()
        {
            return string.Empty;
        }
    }
}
