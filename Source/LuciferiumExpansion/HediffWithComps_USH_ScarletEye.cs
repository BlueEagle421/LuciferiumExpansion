using RimWorld;
using Verse;

namespace LuciferiumExpansion
{
    public class HediffWithComps_USH_ScarletEye : HediffWithComps
    {
        private float xpToForget = 1500f;
        public override void PostMake()
        {
            base.PostMake();
            SkillRecord intellectual = this.pawn.skills.GetSkill(SkillDefOf.Intellectual);
            if (intellectual != null)
            {
                if (intellectual.levelInt == 0 & intellectual.xpSinceLastLevel <= xpToForget)
                {
                    intellectual.xpSinceLastLevel = 0;
                }
                else
                {
                    if (intellectual.xpSinceLastLevel < xpToForget)
                    {
                        float XPsinceLastLvl = intellectual.xpSinceLastLevel;
                        intellectual.levelInt--;
                        intellectual.xpSinceLastLevel = intellectual.XpRequiredForLevelUp - (xpToForget - XPsinceLastLvl);
                    }
                    else
                    {
                        intellectual.xpSinceLastLevel = this.pawn.skills.GetSkill(SkillDefOf.Intellectual).xpSinceLastLevel - xpToForget;
                    }
                }
            }

        }
    }
}