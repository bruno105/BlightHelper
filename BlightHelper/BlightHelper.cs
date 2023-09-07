using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using GameOffsets.Components;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Vector2 = System.Numerics.Vector2;

namespace BlightHelper
{
    [SupportedOSPlatform("windows")]
    public class BlightHelper : BaseSettingsPlugin<BlightHelperSettings>
    {
        private const string FILTER_FILE = "Filter.txt";
        public List<string> FilterMods = new List<string>();
        public List<LabelOnGround> DrawningList = new List<LabelOnGround>();
        public override bool Initialise()
        {

            Name = "BlightHelper";
            Settings.RefreshFile.OnPressed += () => { ReadFilterFile(); };
            ReadFilterFile();

            return true;
        }

        private void ReadFilterFile()
        {


            var path = $"{DirectoryFullName}\\{FILTER_FILE}";
            if (File.Exists(path))
            {
                ReadFile();
            }
            else
                CreateFilterFile();
        }
        private void CreateFilterFile()
        {
            var path = $"{DirectoryFullName}\\{FILTER_FILE}";
            if (File.Exists(path)) return;
            using (var streamWriter = new StreamWriter(path, true))
            {
                streamWriter.WriteLine("//ModName|Description(Opitional)");
                streamWriter.WriteLine("BlightEnchantmentChillingTowerDamage");
                streamWriter.WriteLine("");
                streamWriter.Close();
            }
        }


        private void ReadFile()
        {
            DrawningList.Clear();
            string[] lines = System.IO.File.ReadAllLines($"{DirectoryFullName}\\{FILTER_FILE}");
            FilterMods.Clear();
            foreach (string line in lines)
            {
                if (line.Length <= 2) continue;
                if (line.Contains("/")) continue;
                string mod = line.Contains("|") ? line.Split('|')[0].Trim() : line.Trim();
                FilterMods.Add(mod);
            }


            Settings.FilterList.ListFilter.SetListValues(FilterMods);



        }
        public override void AreaChange(AreaInstance area)
        {
          
            DrawningList.Clear();
        }

        public override Job Tick()
        {
            if (GameController.Area.CurrentArea.IsHideout || GameController.Area.CurrentArea.IsTown || 
                GameController.IngameState.IngameUi == null || GameController.IngameState.IngameUi.ItemsOnGroundLabelsVisible == null)
            {
                return null;
            }


            DrawningList.Clear();


            var Items = GameController.IngameState.IngameUi.ItemsOnGroundLabelsVisible.Where(x => x.ItemOnGround.HasComponent<ExileCore.PoEMemory.Components.WorldItem>()
            && (x.ItemOnGround.GetComponent<ExileCore.PoEMemory.Components.WorldItem>().ItemEntity.Metadata.Contains("Rings")
            || x.ItemOnGround.GetComponent<ExileCore.PoEMemory.Components.WorldItem>().ItemEntity.Metadata.Contains("MiscellaneousObjects")
            || x.ItemOnGround.GetComponent<ExileCore.PoEMemory.Components.WorldItem>().ItemEntity.Metadata.Contains("Amulets"))).ToList();


            foreach (var label in Items)
            {
                
                var worlditem = label.ItemOnGround.GetComponent<ExileCore.PoEMemory.Components.WorldItem>();
                var moditems = worlditem.ItemEntity.GetComponent<Mods>();
                if (moditems == null) continue;


                foreach (var mod in moditems.ItemMods)
                {
                    if (worlditem.ItemEntity.Metadata.Contains("Rings"))
                    {
                        if (Settings.Debug2.Value == true) LogMessage($"Mod:{mod.Name}");

                        if (FilterMods.Contains(mod.Name))
                        {
                            if (!DrawningList.Contains(label)) DrawningList.Add(label);
                        }
                    }
                    else if (worlditem.ItemEntity.Metadata.Contains("Amulets"))
                    {
                        if (mod.Name == "GrantedPassive")
                        {

                            if (FilterMods.FirstOrDefault(x => x == mod.Values[0].ToString()) != null)
                            {
                                if (!DrawningList.Contains(label)) DrawningList.Add(label);
                            }
                        }
                    }

                }


            }
            return null;
        }

        public override void Render()
        {

            if (Settings.Debug2.Value == true) LogMessage($"Drawning:{DrawningList.Count}");
            if (DrawningList.Count > 0)
            {

                foreach (var item in DrawningList)
                {
                    if (item != null && item.IsVisible)
                    {
                        if (Settings.Debug2.Value == true) LogMessage($"Mod:{item.Label}");
                         Graphics.DrawFrame(item.Label.GetClientRectCache, Settings.BaseColor, Settings.FrameThickness);
                    }
                }

            }
        }

    }
}