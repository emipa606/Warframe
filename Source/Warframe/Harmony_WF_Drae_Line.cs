using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace Warframe
{
    [HarmonyPatch(typeof(SelectionDrawer), "DrawSelectionOverlays", new Type[] { })]
    public static class Harmony_WF_Drae_Line
    {
        public static void Postfix()
        {
            foreach (var obj in Find.Selector.SelectedObjects)
            {
                if (obj is not Thing thing)
                {
                    continue;
                }

                if (thing is Pawn pawn)
                {
                    if (!WFModBase.Instance._WFcontrolstorage.BeControlerAndControlCell.ContainsKey(pawn))
                    {
                        continue;
                    }

                    var controler =
                        WFModBase.Instance._WFcontrolstorage.BeControlerAndControlCell.TryGetValue(pawn);
                    if (controler.Map == pawn.Map)
                    {
                        HighDrawLineBetween(pawn.TrueCenter(), controler.TrueCenter(),
                            MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent,
                                Color.magenta));
                    }
                }
                else if (thing is Building_ControlCell controler)
                {
                    if (!WFModBase.Instance._WFcontrolstorage.ControlCellAndBeControler.ContainsKey(
                        controler))
                    {
                        continue;
                    }

                    var becontroler =
                        WFModBase.Instance._WFcontrolstorage.ControlCellAndBeControler.TryGetValue(controler);
                    if (controler.Map == becontroler.Map)
                    {
                        HighDrawLineBetween(becontroler.TrueCenter(), controler.TrueCenter(),
                            MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent,
                                Color.magenta));
                    }
                }
            }
        }

        public static void HighDrawLineBetween(Vector3 A, Vector3 B, Material mat)
        {
            if (Mathf.Abs(A.x - B.x) < 0.01f && Mathf.Abs(A.z - B.z) < 0.01f)
            {
                return;
            }

            var pos = (A + B) / 2f;
            if (A == B)
            {
                return;
            }

            A.y = B.y;
            var z = (A - B).MagnitudeHorizontal();
            var q = Quaternion.LookRotation(A - B);
            var s = new Vector3(0.2f, 1f, z);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(pos + new Vector3(0, 3, 0), q, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 0);
        }
    }
}