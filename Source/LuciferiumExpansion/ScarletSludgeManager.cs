using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace LuciferiumExpansion
{
    public class ScarletSludgeManager : WorldComponent
    {
        public float ScarletSludgeAmount;
        public static ScarletSludgeManager Instance { get; private set; }

        public ScarletSludgeManager(World world) : base(world)
        {
            Instance = this;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ScarletSludgeAmount, "USH_ScarletSludgeAmount", DefaultScarletSludgeAmount());
        }

        private float DefaultScarletSludgeAmount()
        {
            return Random.Range(1f, 10f);
        }

    }
}
