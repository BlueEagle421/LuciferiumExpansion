
using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LuciferiumExpansion
{
    public class PlaceWorker_LuciferiumPump : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            base.DrawGhost(def, center, rot, ghostCol, thing);

            CompProperties_LuciferiumPump props = def.GetCompProperties<CompProperties_LuciferiumPump>();

            if (props == null)
                return;

            GenDraw.DrawRadiusRing(center, props.range);
        }
    }
    public class CompProperties_LuciferiumPump : CompProperties_Activable
    {
        public int baseUsages = 3;
        public int fuelConsuption = 12;
        public float range;
        public HediffDef hediffDefComa;
        public HediffDef hediffDefAddiction;
        public EffecterDef effecterDef;
        public int sustainEffectTicks = 100;
        [NoTranslate] public string selectionTexPath;

        public CompProperties_LuciferiumPump() => compClass = typeof(CompLuciferiumPump);
    }

    [StaticConstructorOnStartup]
    public class CompLuciferiumPump : CompActivable
    {
        private CompRefuelable _compRefuelable;
        private int _usagesLeft;
        private Corpse _selectedCorpse;
        public CompProperties_LuciferiumPump Props => (CompProperties_LuciferiumPump)props;

        [Unsaved(false)] private Texture2D selectionTex;

        public Texture2D SelectionUIIcon
        {
            get
            {
                if (selectionTex == null)
                    selectionTex = ContentFinder<Texture2D>.Get(Props.activateTexPath);

                return selectionTex;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            _usagesLeft = Props.baseUsages;
            _compRefuelable = parent.GetComp<CompRefuelable>();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            if (parent.Spawned)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "USH_LE_OrderCorspeSelection".Translate() + "...";
                command_Action.defaultDesc = "USH_LE_OrderCorspeSelectionDesc".Translate(parent.Named("THING"));
                command_Action.icon = SelectionUIIcon;
                command_Action.action = delegate
                {
                    SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                    BeginCorpseTargeting();
                };

                yield return command_Action;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (this == null)
                return string.Empty;

            stringBuilder.AppendLine("USH_LE_UsagesLeft".Translate(_usagesLeft));

            stringBuilder.AppendLine("USH_LE_CorspeSelected".Translate(CorspeName(_selectedCorpse)));

            return stringBuilder.ToString().TrimEnd();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref _usagesLeft, "USH_UsagesLeft");
        }

        public override AcceptanceReport CanActivate(Pawn activateBy = null)
        {
            AcceptanceReport result = base.CanActivate(activateBy);
            if (!result.Accepted)
                return result;

            if (!IsCorpseValid(_selectedCorpse))
                return "USH_LE_CorspeNotValid".Translate();

            return true;
        }

        public override void Activate()
        {
            base.Activate();

            TryToResurrect(_selectedCorpse);
        }

        protected override bool TryUse()
        {
            if (IsCorpseValid(_selectedCorpse))
                return false;

            return true;
        }

        private void BeginCorpseTargeting()
        {
            Find.Targeter.BeginTargeting(CorpseTargetingParameters(), delegate (LocalTargetInfo t)
            {
                if (IsCorpseValid((Corpse)t.Thing))
                    _selectedCorpse = (Corpse)t.Thing;
            });
        }

        private string CorspeName(Corpse corpse)
        {
            if (!IsCorpseValid(corpse))
                return "USH_LE_CorpseNone".Translate();

            return corpse.InnerPawn.Name.ToStringFull;
        }

        private bool IsCorpseValid(Corpse corpse)
        {
            if (corpse == null)
                return false;

            if (corpse.InnerPawn == null)
                return false;

            if (!corpse.InnerPawn.IsColonist)
                return false;

            return true;
        }

        public bool TryToResurrect(Corpse corpse)
        {
            if (!IsCorpseValid(corpse))
                return false;

            Resurrect(corpse.InnerPawn);
            return true;
        }

        private void Resurrect(Pawn pawn)
        {
            ResurrectionUtility.Resurrect(pawn);
            SpawnResurrectionEffect(pawn);
            HandleHediffs(pawn);

            _selectedCorpse = null;
            _compRefuelable.ConsumeFuel(Props.fuelConsuption);
            _usagesLeft--;
            CheckForSelfDeconstruction();
        }

        private void SpawnResurrectionEffect(Pawn pawn)
        {
            Map currentMap = parent.Map;
            Effecter spawnedEffecter = Props.effecterDef.Spawn(pawn, currentMap, 1f);
            currentMap.effecterMaintainer.AddEffecterToMaintain(spawnedEffecter, pawn.Position, Props.sustainEffectTicks);
        }

        public void HandleHediffs(Pawn pawn)
        {
            List<Hediff> allHediffs = new List<Hediff>();
            pawn.health.hediffSet.GetHediffs(ref allHediffs);

            Hediff addedComa = pawn.health.AddHediff(Props.hediffDefComa);
            Hediff foundAddiction = allHediffs.Find(x => x.def == Props.hediffDefAddiction);

            if (foundAddiction == null)
            {
                addedComa.Severity = 5;
                pawn.health.AddHediff(Props.hediffDefAddiction);
            }
            else
            {
                addedComa.Severity = 1;
                foundAddiction.Severity = 1;
            }
        }

        public void CheckForSelfDeconstruction()
        {
            if (_usagesLeft > 0)
                return;

            parent.Destroy(DestroyMode.Deconstruct);
        }

        private TargetingParameters CorpseTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = false,
                canTargetItems = true,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = (TargetInfo x) => x.Thing is Corpse && Distance(parent.Position, x.Thing.Position) < Props.range
            };
        }
        private float Distance(IntVec3 a, IntVec3 b)
        {
            float num = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            GenDraw.DrawRadiusRing(parent.Position, Props.range);
        }
    }
}
