using RimWorld;
using Verse;

namespace LuciferiumExpansion
{
    public class HediffCompProperties_USH_ScarletParalysis : HediffCompProperties
    {
        public HediffCompProperties_USH_ScarletParalysis() => this.compClass = typeof(HediffComp_USH_ScarletParalysis);
    }
    public class HediffComp_USH_ScarletParalysis : HediffComp
    {

        public HediffCompProperties_USH_ScarletParalysis Props => (HediffCompProperties_USH_ScarletParalysis)this.props;

        private Pawn p;

        public override void CompPostMake()
        {
            base.CompPostMake();
            p = this.parent.pawn;

            foreach (Thought_Memory memory in p.needs.mood.thoughts.memories.Memories)
            {
                if (memory.def == ThoughtDefOf.DebugBad)
                {
                    p.needs.mood.thoughts.memories.RemoveMemory(memory);
                }
            }

            foreach (Trait trait in p.story.traits.allTraits)
            {
                p.story.traits.RemoveTrait(trait);
            }

            Trait traitToGive = new Trait(TraitDef.Named("USH_ScarletParalysis"));
            p.story.traits.GainTrait(traitToGive);

            p.health.RemoveHediff(this.parent);

        }
    }
}