using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.Shared.Cache;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using ExileCore.PoEMemory.MemoryObjects;
using System;
using ExileCore.PoEMemory;

namespace BlightHelper
{
    [SupportedOSPlatform("windows")]
    public class BlightHelper : BaseSettingsPlugin<BlightHelperSettings>
    {
        private const string FILTER_FILE = "Filter.txt";
        public List<string> FilterMods = new List<string>();
        public List<Element> DrawningList = new List<Element>();
        public CachedValue<List<(Entity item, Mods mods, Element element)>> _groundItems;



        public override bool Initialise()
        {
            _groundItems = new TimeCache<List<(Entity item, Mods mods, Element element)>>(GetItemsOnGround, Settings.CacheTime.Value);
            Name = "BlightHelper";
            Settings.RefreshFile.OnPressed += () => { ReadFilterFile(); };
            ReadFilterFile();

            return true;
        }

        private List<(Entity item, Mods mods, Element element)> GetItemsOnGround()
        {

            var labelsOnGround = GameController.IngameState.IngameUi.ItemsOnGroundLabelsVisible;
            var result = new List<(Entity item, Mods mods, Element element)>();
            foreach (var labelOnGround in labelsOnGround)
            {
                var item = labelOnGround.ItemOnGround;
                if (item.TryGetComponent<WorldItem>(out var worldItem) &&
                    worldItem.ItemEntity is { IsValid: true } groundItemEntity &&
                    groundItemEntity.TryGetComponent<Mods>(out var mods) &&
                    mods != null &&
                    (worldItem.ItemEntity.Metadata.Contains("Rings") ||
                    worldItem.ItemEntity.Metadata.Contains("Amulets") ||
                    worldItem.ItemEntity.Metadata.Contains("MiscellaneousObjects")))
                {

                    result.Add(new(groundItemEntity, mods, labelOnGround.Label));
                }
            }

            return result;
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

            foreach (var item in _groundItems.Value)
            {

                foreach (var mod in item.mods.ItemMods)
                {

                    if (FilterMods.Contains(mod.Name))
                    {
                        if (!DrawningList.Contains(item.Item3)) DrawningList.Add(item.Item3);
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


                    Graphics.DrawFrame(item.GetClientRectCache, Settings.BaseColor, Settings.FrameThickness);

                }

            }
        }

    }
}