using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;

namespace RemoveInteriorFogPatcher
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "RemoveInteriorFogPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var cellContext in state.LoadOrder.PriorityOrder.Cell().WinningContextOverrides(state.LinkCache))
            {
                var cell = cellContext.Record;
                if (!cell.Flags.HasFlag(Cell.Flag.IsInteriorCell)) continue;

                if (cell.Lighting == null) continue;
                if (cell.Lighting.FogNear != 0 || cell.Lighting.FogFar != 0)
                {
                    var modifiedCell = cellContext.GetOrAddAsOverride(state.PatchMod);
                    modifiedCell.Lighting!.FogNear = 0;
                    modifiedCell.Lighting.FogFar = 0;
                }
            }
        }
    }
}
