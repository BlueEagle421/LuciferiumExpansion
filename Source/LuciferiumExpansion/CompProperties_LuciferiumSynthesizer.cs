using Verse;
using PipeSystem;

namespace LuciferiumExpansion
{
    public class CompProperties_LuciferiumSynthesizer : CompProperties
    {
        public ThingDef thing;
        public int thingCount;
        public CompProperties_LuciferiumSynthesizer() => this.compClass = typeof(CompLuciferiumSynthesizer);
    }
}
