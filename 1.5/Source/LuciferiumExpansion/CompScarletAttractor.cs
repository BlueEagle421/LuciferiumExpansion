using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.Sound;

namespace LuciferiumExpansion
{
    public class CompProperties_ScarletAttractor : CompProperties_Interactable
    {
        public float scarletSludgeToAdd;
        public SoundDef soundDef;

        public CompProperties_ScarletAttractor() => compClass = typeof(CompScarletAttractor);
    }

    [StaticConstructorOnStartup]
    public class CompScarletAttractor : CompInteractable
    {
        internal List<IntVec3> _lumpCells;
        private Map _currentMap;

        public new CompProperties_ScarletAttractor Props => (CompProperties_ScarletAttractor)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            _currentMap = parent.Map;
        }

        protected override void OnInteracted(Pawn caster)
        {
            base.OnInteracted(caster);

            AttractScarletSludge();
        }

        protected override bool TryInteractTick() => true;

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("USH_LE_WillAttract".Translate(Props.scarletSludgeToAdd, Props.scarletSludgeToAdd.ConvertToLuciferium()));

            return stringBuilder.ToString().TrimEnd();
        }

        private void AttractScarletSludge()
        {
            PlayAttractionSound();
            SpawnAttractionBeam();

            ScarletSludgeManager.Instance.ScarletSludgeAmount += Props.scarletSludgeToAdd;
            parent.Destroy(DestroyMode.Vanish);
        }

        private void PlayAttractionSound()
        {
            Props.soundDef.PlayOneShot(new TargetInfo(parent.Position, parent.Map, false));
        }

        private void SpawnAttractionBeam()
        {
            Mote mote = (Mote)ThingMaker.MakeThing(ThingDef.Named("USH_ScarletBeam"), null);
            mote.exactPosition = parent.Position.ToVector3Shifted();
            mote.Scale = 65f;
            mote.rotationRate = 1.4f;
            GenSpawn.Spawn(mote, parent.Position, _currentMap, WipeMode.Vanish);
            FleckMaker.Static(parent.Position, _currentMap, FleckDefOf.PsycastAreaEffect, 10f);
        }
    }
}
