using ExileCore;
using ExileCore.PoEMemory.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BlightHelper
{
   

    public class BlightHelperCore : BaseSettingsPlugin<Settings>
    {

        private const string FILTER_FILE = "Filter.txt";
        public List<string> FilterMods = new List<string>();   

        public override bool Initialise()
        {
            Name = "BlightHelper";
            Settings.RefreshFile.OnPressed += () => { ReadFilterFile(); };
            ReadFilterFile();
            return base.Initialise();
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
            
            string[] lines = System.IO.File.ReadAllLines($"{DirectoryFullName}\\{FILTER_FILE}");
            FilterMods.Clear();
            foreach (string line in lines)
            {
                if (line.Contains("/")) continue;
                string mod = line.Contains("|") ? line.Split('|')[0].Trim() : line.Trim();
                FilterMods.Add(mod);
            }



        }
        
        public override void Render()
        {

            if (GameController.Area.CurrentArea.IsHideout ||
                GameController.Area.CurrentArea.IsTown || GameController.IngameState.IngameUi == null || GameController.IngameState.IngameUi.ItemsOnGroundLabelsVisible == null)
            {
                return;
            }
           





            foreach (var label in GameController.IngameState.IngameUi.ItemsOnGroundLabelsVisible)
            {
            

                var worlditem = label.ItemOnGround.GetComponent<WorldItem>();
                if (worlditem == null) continue;
                
                if (worlditem.ItemEntity.Type != ExileCore.Shared.Enums.EntityType.Item && !worlditem.ItemEntity.Metadata.Contains("Rings") && !worlditem.ItemEntity.Metadata.Contains("MiscellaneousObjects")) continue;

               
                var moditems = worlditem.ItemEntity.GetComponent<Mods>();
               
                if (moditems == null) continue;

          
                foreach (var mod in moditems.ItemMods)
                {
                    if (worlditem.ItemEntity.Metadata.Contains("Rings"))
                    {
                        if (FilterMods.Contains(mod.Name))
                        {
                            Graphics.DrawFrame(label.Label.GetClientRectCache, Settings.BaseColor, Settings.FrameThickness);
                        }
                    }
                    else if (worlditem.ItemEntity.Metadata.Contains("Amulets"))
                    {

                        
                        if (mod.Name == "GrantedPassive")
                        {
                            
                            if (FilterMods.FirstOrDefault(x=> x == mod.Values[0].ToString()) != null) Graphics.DrawFrame(label.Label.GetClientRectCache, Settings.BaseColor, Settings.FrameThickness);
                        }
                    }

                }
                
                    



            }
            
            //base.Render();
        }

        public override Job Tick()
        {

            return base.Tick();
        }


        


    }
}
