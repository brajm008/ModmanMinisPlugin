﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LordAshes;
using Newtonsoft.Json;
using ThunderMan.Manifests;
using UnityEngine;

namespace ThunderMan.ThunderManIntegration
{
    public partial class AssetsList : Form
    {
        private string AssetType;
        
        public AssetsList(string search)
        {
            AssetType = search;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var model = paths[listBox1.SelectedIndex];

            var modFolder = model.transformName.Substring(0, model.transformName.IndexOf("\\"));
            
            modManifest obj = JsonConvert.DeserializeObject<modManifest>(File.ReadAllText(pluginsFolder+"\\" + modFolder + "\\manifest.json"));

            // Don't bother loading json file until needed
            var author = modFolder.Substring(0,modFolder.IndexOf("-"));
            var mod_name = obj.name;
            var version = obj.version_number;
            
            model.Ror2mm = $"ror2mm://v1/install/talespire.thunderstore.io/{author}/{mod_name}/{version}/";
            if (AssetType == "Effects") model.transformName = $"#{model.transformName}";
            StatMessaging.SetInfo(new CreatureGuid(ThunderManPlugin.RadialTargetedMini), ThunderManPlugin.Guid, JsonConvert.SerializeObject(model));
            Close();
        }

        public List<LoadAsset> paths = new List<LoadAsset>();
        private string pluginsFolder;

        private void AssetsList_Load(object sender, EventArgs e)
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pluginsFolder = Directory.GetParent(assemblyFolder).FullName;

            ParallelQuery<FileInfo> bundles = ThunderManPlugin.GetMinis();

            Dictionary<string, LoadAsset> assets = new Dictionary<string, LoadAsset>();

            foreach (var path in bundles)
            {
                var relative = path.FullName.Replace(pluginsFolder+"\\", "");
                Debug.Log($"Relative: {relative}");
                Debug.Log($"Name: {path.Name}");

                var asset = new LoadAsset
                {
                    MiniName = path.Name,
                    transformName = relative
                };
                assets.Add(path.Name,asset);
                paths.Add(asset);
                listBox1.Items.Add(path.Name);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
