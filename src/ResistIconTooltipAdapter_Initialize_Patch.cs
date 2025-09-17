using AsmResolver.Collections;
using HarmonyLib;
using MGSC;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static MGSC.TooltipProperty;

namespace CompareDelta
{
    [HarmonyPatch(typeof(ResistIconTooltipAdapter), nameof(ResistIconTooltipAdapter.Initialize),
        new Type[] {
            typeof(string),
            typeof(System.Collections.Generic.List<MGSC.DmgResist>),
            typeof(System.Collections.Generic.List<string>),
            typeof(System.Collections.Generic.List<MGSC.DmgResist>)
    })]
    public static class ResistIconTooltipAdapter_Initialize_Patch
    {
        public static void Postfix(ResistIconTooltipAdapter __instance, string resist,
            List<DmgResist> currentResists, List<DmgResist> equippedResists = null)
        {


            //Copy Warning:  This replicates logic that is in ResistIconTooltipAdapter.Initialize
            //  It is very close to the original code.

            //---- The logical copy copy starts here.

            if (equippedResists == null) return;

            int currentResist = Mathf.RoundToInt(currentResists.FirstOrDefault((DmgResist r) => r.damage == resist).resistPercent);
            int equippedResist = Mathf.RoundToInt(equippedResists.FirstOrDefault((DmgResist r) => r.damage == resist).resistPercent);

            ComprasionType comparisonType = ComprasionType.None;

            if (currentResist < equippedResist)
            {
                comparisonType = ComprasionType.Lower;
            }
            else if (currentResist > equippedResist)
            {
                comparisonType = ComprasionType.Higher;
            }

            //---- Effectively a copy ends here.

            SetDiffText(__instance, comparisonType, currentResist, equippedResist);

        }

        private static void SetDiffText(ResistIconTooltipAdapter instance,
            ComprasionType comprasionType, int currentResist, int equippedResist)
        {

            int delta = currentResist - equippedResist;
            string difference = delta == 0 ? "" : $" ({(delta).ToString("+#;-#")})";

            switch (comprasionType)
            {
                case TooltipProperty.ComprasionType.Lower:
                    difference = difference.WrapInColor(Colors.LightRed);
                    break;
                case TooltipProperty.ComprasionType.Higher:
                    difference = difference.WrapInColor(Colors.Green);
                    break;
                    //default:
                    //Ignore
            }

            instance._equippedValue.text = $"<size=5>{equippedResist}{difference}</size>";
        }
    }
}
