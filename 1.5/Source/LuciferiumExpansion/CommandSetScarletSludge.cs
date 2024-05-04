using LuciferiumExpansion;
using UnityEngine;
using Verse;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class CommandSetScarletSludge : Command
    {
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);

            string textGetter(int x) => $"Set scarlet sludge amount: {x}, ({((float)x).ConvertToLuciferium()} luciferium) ({x / ScarletSludgeManager.Instance.DefaultScarletSludgeAmount() * 100f}%)";

            Dialog_Slider dialog_Slider = new Dialog_Slider(textGetter, 0, (int)LEUtils.SUSTAIN_ONE_YEAR * 10, delegate (int value)
            {
                ScarletSludgeManager.Instance.ScarletSludgeAmount = value;
            }, (int)ScarletSludgeManager.Instance.ScarletSludgeAmount, 1f);

            Find.WindowStack.Add(dialog_Slider);
        }
    }
}
