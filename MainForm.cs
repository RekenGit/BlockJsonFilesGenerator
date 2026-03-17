using MinecraftJsonGenerator.Models;
using MinecraftJsonGenerator.Services;

namespace MinecraftJsonGenerator
{
    public partial class MainForm : Form
    {
        private readonly GenerationService _generationService = new GenerationService();
        #region Components
        private ContextMenuStrip contextMenuStrip1;
        private System.ComponentModel.IContainer components;
        private GroupBox groupBox1;
        private Panel panel5;
        private Label lblBlockType;
        private Panel panel4;
        private Label lblNamespace;
        private TextBox txtNamespace;
        private Panel panel3;
        private Label lblVariantCount;
        private TextBox txtOutputPath;
        private Panel panel2;
        private Label lblVariantPattern;
        private TextBox txtVariantPattern;
        private Panel panel1;
        private Label lblBlockName;
        private TextBox txtBlockName;
        private ComboBox cmbBlockType;
        private NumericUpDown numVariantCount;
        private GroupBox grpTextures;
        private RadioButton rbManualTextures;
        private RadioButton rbSingleTexture;
        private GroupBox grpOutput;
        private Button btnBrowseOutput;
        private Panel panel6;
        private Label lblOutputPath;
        private GroupBox grpPreview;
        private GroupBox groupBox5;
        private GroupBox groupBox6;
        private Button btnGenerateBlockstates;
        private Button btnGenerateItems;
        private Button btnGenerateModels;
        private Button btnGenerateAll;
        private GroupBox grpWeights;
        private Panel pnlTextureInputs;
        private ListBox lstVariants;
        private FlowLayoutPanel flpWeights;
        private GroupBox groupBox2;
        private Panel panel7;
        private ComboBox cmbItemVariant;
        private Label lblItemVariant;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private GroupBox groupBox7;
        private GroupBox groupBox8;
        private Panel panel8;
        private ComboBox cmbItemVariant2;
        private Label lblItemVariant2;
        private CheckBox cbMultipleBlocks;
        private Panel panel9;
        private TextBox txtBlockNameAlternative;
        private Label lblBlockNameAlternative;
        private CheckBox cbAlternative;
        #endregion
        private readonly Dictionary<string, TextBox> _textureInputs = new Dictionary<string, TextBox>();
        public static TextureWeightRegistry AllWeights;
        public Dictionary<string, string> VariantLinks;

        private string groupBox5OriginalText;

        public MainForm()
        {
            InitializeComponent();
            AllWeights = new();
            VariantLinks = new();

            groupBox5OriginalText = groupBox5.Text;

            ConfigureForm();
            WireEvents();
            LoadBlockTypes();
            ApplyDefaults();
            RebuildTextureInputs();
            RefreshVariantPreview();
            LoadUserSettings();
        }

        private void ConfigureForm()
        {
            if (flpWeights != null)
                flpWeights.AutoScroll = flpWeights.WrapContents = true;
        }

        private void WireEvents()
        {
            txtBlockName.TextChanged += Input_Changed;
            txtVariantPattern.TextChanged += Input_Changed;
            txtNamespace.TextChanged += Input_Changed;
            txtOutputPath.TextChanged += Input_Changed;
            numVariantCount.ValueChanged += Input_Changed;
            cmbBlockType.SelectedIndexChanged += CmbBlockType_SelectedIndexChanged;
            rbSingleTexture.CheckedChanged += TextureMode_CheckedChanged;
            rbManualTextures.CheckedChanged += TextureMode_CheckedChanged;

            btnBrowseOutput.Click += BtnBrowseOutput_Click;
            btnGenerateModels.Click += BtnGenerateModels_Click;
            btnGenerateItems.Click += BtnGenerateItems_Click;
            btnGenerateBlockstates.Click += BtnGenerateBlockstates_Click;
            btnGenerateAll.Click += BtnGenerateAll_Click;
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) => SaveUserSettings();

        private void LoadBlockTypes()
        {
            cmbBlockType.Items.Clear();
            foreach (var item in TemplateRepository.GetComboItems())
                cmbBlockType.Items.Add(item);
        }

        private void ApplyDefaults()
        {
            if (string.IsNullOrWhiteSpace(txtVariantPattern.Text))
                txtVariantPattern.Text = "_broken{variant}";

            if (string.IsNullOrWhiteSpace(txtNamespace.Text))
                txtNamespace.Text = "minecraft";

            if (cmbBlockType.Items.Count > 0 && cmbBlockType.SelectedIndex < 0)
                cmbBlockType.SelectedIndex = 0;

            if (!rbSingleTexture.Checked && !rbManualTextures.Checked)
                rbSingleTexture.Checked = true;
        }

        private void Input_Changed(object sender, EventArgs e)
        {
            RefreshVariantPreview();
            txtBlockNameAlternative.Text = GetAlternativeBlockName(txtBlockName.Text);
        }

        private void CmbBlockType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RebuildTextureInputs();
            RefreshVariantPreview();
            txtBlockNameAlternative.Text = GetAlternativeBlockName(txtBlockName.Text);
        }

        private void TextureMode_CheckedChanged(object sender, EventArgs e) => RebuildTextureInputs();

        private void BtnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select resource directory";
                if (Directory.Exists(txtOutputPath.Text))
                    dialog.SelectedPath = txtOutputPath.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                    txtOutputPath.Text = dialog.SelectedPath;
            }
        }

        private void BtnGenerateModels_Click(object sender, EventArgs e) => GenerateFiles(_generationService.GenerateModels, "Model files were generated.");
        private void BtnGenerateItems_Click(object sender, EventArgs e) => GenerateFiles(_generationService.GenerateItems, "Item files were generated.");
        private void BtnGenerateBlockstates_Click(object sender, EventArgs e) => GenerateFiles(_generationService.GenerateBlockstates, "Blockstate files were generated.");
        private void BtnGenerateAll_Click(object sender, EventArgs e) => GenerateFiles(_generationService.GenerateAll, "All files were generated.");
        private void GenerateFiles(Action<GenerationConfig> generateAction, string successMessage)
        {
            try
            {
                var config = BuildConfig();
                generateAction(config);
                MessageBox.Show(successMessage, "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void RefreshVariantPreview()
        {
            int previousItemVariant = cmbItemVariant.SelectedIndex;
            int previousItemVariant2 = cmbItemVariant2.SelectedIndex;
            List<string> oldVariants = cmbItemVariant.Items.Cast<string>().ToList();

            cmbItemVariant.Items.Clear();
            cmbItemVariant2.Items.Clear();
            VariantLinks.Clear();

            List<string> variants = VariantService.BuildVariantNames(
                GetAlternativeBlockName(txtBlockName.Text),
                txtVariantPattern.Text,
                (int)numVariantCount.Value);

            List<string> variantTextures = VariantService.BuildVariantNames(
                txtBlockName.Text,
                txtVariantPattern.Text,
                (int)numVariantCount.Value);

            for (int i = 0; i < variants.Count; i++)
                VariantLinks.Add(variants[i], variantTextures[i]);

            foreach (var variant in variants)
                cmbItemVariant.Items.Add(variant);


            cmbItemVariant2.Items.AddRange(variants.ToArray());
            cmbItemVariant.SelectedIndex = previousItemVariant;
            cmbItemVariant2.SelectedIndex = previousItemVariant2;

            RefreshGeneratedFilesPreview(variants);
            RebuildWeightsPanel(variants, oldVariants);
        }

        private void RefreshGeneratedFilesPreview(List<string> variants)
        {
            lstVariants.Items.Clear();

            string blockName = txtBlockName.Text.Trim();
            string blockNameAlt = GetAlternativeBlockName(blockName);
            if (string.IsNullOrWhiteSpace(blockName))
                return;

            string ns = VariantService.NormalizeNamespace(txtNamespace.Text);
            var blockType = GetSelectedBlockType();
            var template = TemplateRepository.Get(blockType);

            int modelCount = variants.Count * template.ModelFiles.Count;

            lstVariants.Items.Add($"Models: {modelCount}");
            lstVariants.Items.Add("Items: " + (cbMultipleBlocks.Checked ? 2 : 1));
            lstVariants.Items.Add("Blockstates: " + (cbMultipleBlocks.Checked ? 2 : 1));
            lstVariants.Items.Add("--------------------------------");

            lstVariants.Items.Add("[MODELS]");
            foreach (var variant in variants)
            {
                foreach (var modelFile in template.ModelFiles)
                {
                    string fileName = variant + modelFile.FileSuffix + ".json";
                    string path = CombinePreviewPath(ns, "models", "block", blockName, GetSelectedBlockTypePath(), fileName);
                    lstVariants.Items.Add(path);
                }
            }

            lstVariants.Items.Add(string.Empty);
            lstVariants.Items.Add("[ITEM]");
            lstVariants.Items.Add(CombinePreviewPath(ns, "items", blockNameAlt + ".json"));
            if (cbMultipleBlocks.Checked)
                lstVariants.Items.Add(CombinePreviewPath(ns, "items", GetSecondaryBlockName() + ".json"));

            lstVariants.Items.Add(string.Empty);
            lstVariants.Items.Add("[BLOCKSTATE]");
            lstVariants.Items.Add(CombinePreviewPath(ns, "blockstates", blockNameAlt + ".json"));
            if (cbMultipleBlocks.Checked)
                lstVariants.Items.Add(CombinePreviewPath(ns, "blockstates", GetSecondaryBlockName() + ".json"));
        }

        private string CombinePreviewPath(params string[] parts)
        {
            return string.Join("/", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private string GetSecondaryBlockName()
        {
            string suffix = (txtVariantPattern.Text ?? string.Empty).Replace("{variant}", "");
            return GetAlternativeBlockName(txtBlockName.Text.Trim()) + suffix;
        }

        private void RebuildTextureInputs()
        {
            if (pnlTextureInputs == null)
                return;

            var oldValues = _textureInputs.ToDictionary(x => x.Key, x => x.Value.Text);
            _textureInputs.Clear();
            pnlTextureInputs.Controls.Clear();

            groupBox5.Text = groupBox5OriginalText + " " + cmbBlockType.Text;

            var type = GetSelectedBlockType();
            var template = TemplateRepository.Get(type);
            var keys = rbSingleTexture.Checked
                ? new List<string> { "all" }
                : template.ManualTextureKeys;

            int top = 10;
            foreach (var key in keys)
            {
                var lbl = new Label();
                lbl.AutoSize = true;
                lbl.Left = 10;
                lbl.Top = top + 4;
                lbl.Text = key + ":";

                var txt = new TextBox();
                txt.Name = "txtTexture_" + key;
                txt.Left = 90;
                txt.Top = top;
                txt.Width = Math.Max(180, pnlTextureInputs.Width - 110);
                txt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                txt.Text = oldValues.ContainsKey(key) ? oldValues[key] : string.Empty;
                txt.TextChanged += Input_Changed;

                pnlTextureInputs.Controls.Add(lbl);
                pnlTextureInputs.Controls.Add(txt);
                _textureInputs[key] = txt;
                top += 30;
            }
        }

        private void RebuildWeightsPanel(List<string> variants, List<string> oldVariants)
        {
            if (flpWeights == null)
                return;

            Dictionary<string, int> oldWeights = new(TextureWeightRegistry.Weights);

            foreach (Control ctrl in flpWeights.Controls)
                ctrl.Dispose();

            flpWeights.Controls.Clear();
            TextureWeightRegistry.RestartWeights();

            if (oldVariants.Count > 0)
            {
                for (int i = 0; i < variants.Count; i++)
                {
                    if (oldVariants.Count > i)
                        flpWeights.Controls.Add(BuildWeightCard(variants[i], oldWeights[oldVariants[i]]));
                    else
                        flpWeights.Controls.Add(BuildWeightCard(variants[i], 0));
                }
            }
            else
            {
                foreach (string variant in variants)
                {
                    flpWeights.Controls.Add(BuildWeightCard(variant, 0));
                }
            }
        }

        private textureWeightConfigControl BuildWeightCard(string variantName, int defaultWeight)
        {
            return new textureWeightConfigControl(variantName, Math.Max(1, Math.Min(100, defaultWeight)), LoadVariantPreview(variantName));
        }

        private Image LoadVariantPreview(string variantName)
        {
            try
            {
                var ns = VariantService.NormalizeNamespace(txtNamespace.Text);
                if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
                    return CreatePlaceholder();

                var texturePath = Path.Combine(txtOutputPath.Text, "assets", ns, "textures", "block", GetTextureNameFromVariantName(variantName) + ".png");
                if (!File.Exists(texturePath))
                    return CreatePlaceholder();

                using (var fs = new FileStream(texturePath, FileMode.Open, FileAccess.Read))
                using (var img = Image.FromStream(fs))
                {
                    return new Bitmap(img);
                }
            }
            catch
            {
                return CreatePlaceholder();
            }
        }

        private string GetTextureNameFromVariantName(string variantName)
        {
            string texture = "";
            VariantLinks.TryGetValue(variantName, out texture);
            return texture==null ? "" : texture;
        }

        private Image CreatePlaceholder()
        {
            var bmp = new Bitmap(64, 64);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.DrawRectangle(Pens.DarkGray, 0, 0, 63, 63);
                g.DrawLine(Pens.DarkGray, 0, 0, 63, 63);
                g.DrawLine(Pens.DarkGray, 63, 0, 0, 63);
            }
            return bmp;
        }

        private GenerationConfig BuildConfig()
        {
            if (cmbItemVariant.Items.Count == 0)
                throw new InvalidOperationException("No variants for block item.");

            if (string.IsNullOrWhiteSpace(txtBlockName.Text))
                throw new InvalidOperationException("Set block name.");

            if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
                throw new InvalidOperationException("Set directory path for resources.");

            var config = new GenerationConfig();
            config.BlockName = txtBlockName.Text.Trim();
            config.VariantPattern = txtVariantPattern.Text.Trim();
            config.VariantCount = (int)numVariantCount.Value;
            config.Namespace = VariantService.NormalizeNamespace(txtNamespace.Text);
            config.BlockType = GetSelectedBlockType();
            config.OutputPath = txtOutputPath.Text.Trim();
            config.UseSingleTexture = rbSingleTexture.Checked;
            config.SelectedItemVariant = cmbItemVariant.SelectedItem?.ToString() ?? config.BlockName;
            config.MultipleBlocks = cbMultipleBlocks.Checked;
            config.SelectedItemVariant2 = cmbItemVariant2.SelectedItem?.ToString() ?? "";
            config.AlternativeBlockName = GetAlternativeBlockName(config.BlockName);
            config.SelectedBlockTypePath = GetSelectedBlockTypePath();


            foreach (var entry in _textureInputs)
                config.TextureInputs[entry.Key] = entry.Value.Text.Trim();

            foreach (var entry in TextureWeightRegistry.Weights)
                config.Weights[entry.Key] = entry.Value;

            return config;
        }

        private BlockTemplateType GetSelectedBlockType()
        {
            switch ((cmbBlockType.Text ?? string.Empty).Trim())
            {
                case "Block": return BlockTemplateType.Block;
                case "Slab": return BlockTemplateType.Slab;
                case "Stairs": return BlockTemplateType.Stairs;
                case "Wall": return BlockTemplateType.Wall;
                case "Door": return BlockTemplateType.Door;
                case "Leaves": return BlockTemplateType.Leaves;
                case "Trapdoor": return BlockTemplateType.Trapdoor;
                case "Column": return BlockTemplateType.Column;
                case "Column 2": return BlockTemplateType.Column2;
                case "Fence Gate": return BlockTemplateType.FenceGate;
                case "Fence": return BlockTemplateType.Fence;

                case "Cross": return BlockTemplateType.Cross;
                case "Carpet": return BlockTemplateType.Carpet;
                case "Torch": return BlockTemplateType.Torch;
                case "Pane": return BlockTemplateType.Pane;
                case "Rail": return BlockTemplateType.Rail;

                default: return BlockTemplateType.Block;
            }
        }

        private void SaveUserSettings()
        {
            Properties.Settings.Default.LastBlockName = txtBlockName.Text.Trim();
            Properties.Settings.Default.LastVariantPattern = txtVariantPattern.Text.Trim();
            Properties.Settings.Default.LastVariantCount = (int)numVariantCount.Value;
            Properties.Settings.Default.LastNamespace = txtNamespace.Text.Trim();
            Properties.Settings.Default.LastOutputPath = txtOutputPath.Text.Trim();
            Properties.Settings.Default.LastMultipleBlocks = cbMultipleBlocks.Checked;
            Properties.Settings.Default.LastItemVariant = (int)cmbItemVariant.SelectedIndex;
            Properties.Settings.Default.LastAlternativeChecked = cbAlternative.Checked;
            Properties.Settings.Default.LastWeights = string.Join(";", TextureWeightRegistry.Weights.Select(kv => $"{kv.Value}"));
            Properties.Settings.Default.LastBlockType = cmbBlockType.SelectedIndex;

            Properties.Settings.Default.LastWindowSize = this.Size;

            Properties.Settings.Default.Save();
        }

        private void LoadUserSettings()
        {
            txtBlockName.Text = Properties.Settings.Default.LastBlockName;
            txtVariantPattern.Text = Properties.Settings.Default.LastVariantPattern;
            txtNamespace.Text = Properties.Settings.Default.LastNamespace;
            txtOutputPath.Text = Properties.Settings.Default.LastOutputPath;
            cbMultipleBlocks.Checked = Properties.Settings.Default.LastMultipleBlocks;
            cmbItemVariant.SelectedIndex = Properties.Settings.Default.LastItemVariant;
            cbAlternative.Checked = Properties.Settings.Default.LastAlternativeChecked;
            cmbBlockType.SelectedIndex = Properties.Settings.Default.LastBlockType;

            LoadWeightsAfterTime();

            int savedCount = Properties.Settings.Default.LastVariantCount;
            if (savedCount >= numVariantCount.Minimum && savedCount <= numVariantCount.Maximum)
                numVariantCount.Value = savedCount;

            this.Size = Properties.Settings.Default.LastWindowSize;
        }

        private static async void LoadWeightsAfterTime()
        {
            await Task.Delay(500);

            int i = 0;
            string[] weights = Properties.Settings.Default.LastWeights.Split(';');
            foreach (var value in TextureWeightRegistry.Weights)
            {
                if (int.TryParse(weights[i], out int weight))
                    TextureWeightRegistry.ChangeWeight(value.Key, weight);
                i++;
            }
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            contextMenuStrip1 = new ContextMenuStrip(components);
            groupBox1 = new GroupBox();
            panel9 = new Panel();
            cbAlternative = new CheckBox();
            txtBlockNameAlternative = new TextBox();
            lblBlockNameAlternative = new Label();
            cbMultipleBlocks = new CheckBox();
            panel5 = new Panel();
            cmbBlockType = new ComboBox();
            lblBlockType = new Label();
            panel4 = new Panel();
            txtNamespace = new TextBox();
            lblNamespace = new Label();
            panel3 = new Panel();
            numVariantCount = new NumericUpDown();
            lblVariantCount = new Label();
            panel2 = new Panel();
            txtVariantPattern = new TextBox();
            lblVariantPattern = new Label();
            panel1 = new Panel();
            txtBlockName = new TextBox();
            lblBlockName = new Label();
            grpTextures = new GroupBox();
            rbManualTextures = new RadioButton();
            rbSingleTexture = new RadioButton();
            grpOutput = new GroupBox();
            btnBrowseOutput = new Button();
            panel6 = new Panel();
            txtOutputPath = new TextBox();
            lblOutputPath = new Label();
            grpPreview = new GroupBox();
            lstVariants = new ListBox();
            groupBox5 = new GroupBox();
            pnlTextureInputs = new Panel();
            groupBox6 = new GroupBox();
            btnGenerateBlockstates = new Button();
            btnGenerateItems = new Button();
            btnGenerateModels = new Button();
            btnGenerateAll = new Button();
            grpWeights = new GroupBox();
            flpWeights = new FlowLayoutPanel();
            groupBox2 = new GroupBox();
            panel8 = new Panel();
            cmbItemVariant2 = new ComboBox();
            lblItemVariant2 = new Label();
            panel7 = new Panel();
            cmbItemVariant = new ComboBox();
            lblItemVariant = new Label();
            groupBox3 = new GroupBox();
            groupBox4 = new GroupBox();
            groupBox7 = new GroupBox();
            groupBox8 = new GroupBox();
            groupBox1.SuspendLayout();
            panel9.SuspendLayout();
            panel5.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numVariantCount).BeginInit();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            grpTextures.SuspendLayout();
            grpOutput.SuspendLayout();
            panel6.SuspendLayout();
            grpPreview.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            grpWeights.SuspendLayout();
            groupBox2.SuspendLayout();
            panel8.SuspendLayout();
            panel7.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox8.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.FromArgb(71, 80, 94);
            groupBox1.Controls.Add(panel9);
            groupBox1.Controls.Add(cbMultipleBlocks);
            groupBox1.Controls.Add(panel5);
            groupBox1.Controls.Add(panel4);
            groupBox1.Controls.Add(panel3);
            groupBox1.Controls.Add(panel2);
            groupBox1.Controls.Add(panel1);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.ForeColor = Color.White;
            groupBox1.Location = new Point(10, 26);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(10);
            groupBox1.Size = new Size(465, 214);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Block Settings";
            // 
            // panel9
            // 
            panel9.Controls.Add(cbAlternative);
            panel9.Controls.Add(txtBlockNameAlternative);
            panel9.Controls.Add(lblBlockNameAlternative);
            panel9.Dock = DockStyle.Top;
            panel9.Location = new Point(10, 175);
            panel9.Name = "panel9";
            panel9.Size = new Size(445, 23);
            panel9.TabIndex = 7;
            // 
            // cbAlternative
            // 
            cbAlternative.AutoSize = true;
            cbAlternative.Dock = DockStyle.Fill;
            cbAlternative.FlatStyle = FlatStyle.Flat;
            cbAlternative.Location = new Point(364, 0);
            cbAlternative.Name = "cbAlternative";
            cbAlternative.Padding = new Padding(2);
            cbAlternative.Size = new Size(81, 23);
            cbAlternative.TabIndex = 2;
            cbAlternative.Text = "Replace name?";
            cbAlternative.UseVisualStyleBackColor = true;
            // 
            // txtBlockNameAlternative
            // 
            txtBlockNameAlternative.BackColor = Color.FromArgb(132, 151, 181);
            txtBlockNameAlternative.BorderStyle = BorderStyle.FixedSingle;
            txtBlockNameAlternative.Dock = DockStyle.Left;
            txtBlockNameAlternative.Enabled = false;
            txtBlockNameAlternative.Location = new Point(120, 0);
            txtBlockNameAlternative.Name = "txtBlockNameAlternative";
            txtBlockNameAlternative.PlaceholderText = "test_block";
            txtBlockNameAlternative.Size = new Size(244, 23);
            txtBlockNameAlternative.TabIndex = 0;
            // 
            // lblBlockNameAlternative
            // 
            lblBlockNameAlternative.Dock = DockStyle.Left;
            lblBlockNameAlternative.Location = new Point(0, 0);
            lblBlockNameAlternative.Margin = new Padding(5, 0, 5, 0);
            lblBlockNameAlternative.Name = "lblBlockNameAlternative";
            lblBlockNameAlternative.Size = new Size(120, 23);
            lblBlockNameAlternative.TabIndex = 1;
            lblBlockNameAlternative.Text = "Second block name:";
            lblBlockNameAlternative.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbMultipleBlocks
            // 
            cbMultipleBlocks.Dock = DockStyle.Top;
            cbMultipleBlocks.FlatStyle = FlatStyle.Flat;
            cbMultipleBlocks.Location = new Point(10, 141);
            cbMultipleBlocks.Name = "cbMultipleBlocks";
            cbMultipleBlocks.Padding = new Padding(2);
            cbMultipleBlocks.Size = new Size(445, 34);
            cbMultipleBlocks.TabIndex = 6;
            cbMultipleBlocks.Text = "Generate a block set, a basic variant (one model), and a multi-model variant?";
            cbMultipleBlocks.UseVisualStyleBackColor = true;
            cbMultipleBlocks.CheckedChanged += cbMultipleBlocks_CheckedChanged;
            // 
            // panel5
            // 
            panel5.Controls.Add(cmbBlockType);
            panel5.Controls.Add(lblBlockType);
            panel5.Dock = DockStyle.Top;
            panel5.Location = new Point(10, 118);
            panel5.Name = "panel5";
            panel5.Size = new Size(445, 23);
            panel5.TabIndex = 4;
            // 
            // cmbBlockType
            // 
            cmbBlockType.BackColor = Color.FromArgb(132, 151, 181);
            cmbBlockType.Dock = DockStyle.Fill;
            cmbBlockType.FlatStyle = FlatStyle.Flat;
            cmbBlockType.FormattingEnabled = true;
            cmbBlockType.Location = new Point(78, 0);
            cmbBlockType.Name = "cmbBlockType";
            cmbBlockType.Size = new Size(367, 23);
            cmbBlockType.TabIndex = 2;
            // 
            // lblBlockType
            // 
            lblBlockType.Dock = DockStyle.Left;
            lblBlockType.Location = new Point(0, 0);
            lblBlockType.Margin = new Padding(5, 0, 5, 0);
            lblBlockType.Name = "lblBlockType";
            lblBlockType.Size = new Size(78, 23);
            lblBlockType.TabIndex = 1;
            lblBlockType.Text = "Block type:";
            lblBlockType.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panel4
            // 
            panel4.Controls.Add(txtNamespace);
            panel4.Controls.Add(lblNamespace);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(10, 95);
            panel4.Name = "panel4";
            panel4.Size = new Size(445, 23);
            panel4.TabIndex = 3;
            // 
            // txtNamespace
            // 
            txtNamespace.BackColor = Color.FromArgb(132, 151, 181);
            txtNamespace.BorderStyle = BorderStyle.FixedSingle;
            txtNamespace.Dock = DockStyle.Fill;
            txtNamespace.Location = new Point(78, 0);
            txtNamespace.Name = "txtNamespace";
            txtNamespace.PlaceholderText = "minecraft";
            txtNamespace.Size = new Size(367, 23);
            txtNamespace.TabIndex = 0;
            // 
            // lblNamespace
            // 
            lblNamespace.Dock = DockStyle.Left;
            lblNamespace.Location = new Point(0, 0);
            lblNamespace.Margin = new Padding(5, 0, 5, 0);
            lblNamespace.Name = "lblNamespace";
            lblNamespace.Size = new Size(78, 23);
            lblNamespace.TabIndex = 1;
            lblNamespace.Text = "Namespace:";
            lblNamespace.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panel3
            // 
            panel3.Controls.Add(numVariantCount);
            panel3.Controls.Add(lblVariantCount);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(10, 72);
            panel3.Name = "panel3";
            panel3.Size = new Size(445, 23);
            panel3.TabIndex = 2;
            // 
            // numVariantCount
            // 
            numVariantCount.BackColor = Color.FromArgb(132, 151, 181);
            numVariantCount.Dock = DockStyle.Fill;
            numVariantCount.Location = new Point(78, 0);
            numVariantCount.Name = "numVariantCount";
            numVariantCount.Size = new Size(367, 23);
            numVariantCount.TabIndex = 2;
            numVariantCount.ValueChanged += numVariantCount_ValueChanged;
            // 
            // lblVariantCount
            // 
            lblVariantCount.Dock = DockStyle.Left;
            lblVariantCount.Location = new Point(0, 0);
            lblVariantCount.Margin = new Padding(5, 0, 5, 0);
            lblVariantCount.Name = "lblVariantCount";
            lblVariantCount.Size = new Size(78, 23);
            lblVariantCount.TabIndex = 1;
            lblVariantCount.Text = "Varint count:";
            lblVariantCount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panel2
            // 
            panel2.Controls.Add(txtVariantPattern);
            panel2.Controls.Add(lblVariantPattern);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(10, 49);
            panel2.Name = "panel2";
            panel2.Size = new Size(445, 23);
            panel2.TabIndex = 1;
            // 
            // txtVariantPattern
            // 
            txtVariantPattern.BackColor = Color.FromArgb(132, 151, 181);
            txtVariantPattern.BorderStyle = BorderStyle.FixedSingle;
            txtVariantPattern.Dock = DockStyle.Fill;
            txtVariantPattern.Enabled = false;
            txtVariantPattern.Location = new Point(78, 0);
            txtVariantPattern.Name = "txtVariantPattern";
            txtVariantPattern.PlaceholderText = "_variant{variant}";
            txtVariantPattern.Size = new Size(367, 23);
            txtVariantPattern.TabIndex = 0;
            txtVariantPattern.TextChanged += txtVariantPattern_DataContextChanged;
            txtVariantPattern.DataContextChanged += BtnGenerateAll_Click;
            // 
            // lblVariantPattern
            // 
            lblVariantPattern.Dock = DockStyle.Left;
            lblVariantPattern.Enabled = false;
            lblVariantPattern.Location = new Point(0, 0);
            lblVariantPattern.Margin = new Padding(5, 0, 5, 0);
            lblVariantPattern.Name = "lblVariantPattern";
            lblVariantPattern.Size = new Size(78, 23);
            lblVariantPattern.TabIndex = 1;
            lblVariantPattern.Text = "Variant suffix:";
            lblVariantPattern.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            panel1.Controls.Add(txtBlockName);
            panel1.Controls.Add(lblBlockName);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(10, 26);
            panel1.Name = "panel1";
            panel1.Size = new Size(445, 23);
            panel1.TabIndex = 0;
            // 
            // txtBlockName
            // 
            txtBlockName.BackColor = Color.FromArgb(132, 151, 181);
            txtBlockName.BorderStyle = BorderStyle.FixedSingle;
            txtBlockName.Dock = DockStyle.Fill;
            txtBlockName.Location = new Point(78, 0);
            txtBlockName.Name = "txtBlockName";
            txtBlockName.PlaceholderText = "test_block";
            txtBlockName.Size = new Size(367, 23);
            txtBlockName.TabIndex = 0;
            // 
            // lblBlockName
            // 
            lblBlockName.Dock = DockStyle.Left;
            lblBlockName.Location = new Point(0, 0);
            lblBlockName.Margin = new Padding(5, 0, 5, 0);
            lblBlockName.Name = "lblBlockName";
            lblBlockName.Size = new Size(78, 23);
            lblBlockName.TabIndex = 1;
            lblBlockName.Text = "Block name:";
            lblBlockName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // grpTextures
            // 
            grpTextures.Controls.Add(rbManualTextures);
            grpTextures.Controls.Add(rbSingleTexture);
            grpTextures.Dock = DockStyle.Top;
            grpTextures.ForeColor = Color.White;
            grpTextures.Location = new Point(10, 26);
            grpTextures.Name = "grpTextures";
            grpTextures.Size = new Size(445, 69);
            grpTextures.TabIndex = 2;
            grpTextures.TabStop = false;
            grpTextures.Text = "Textures Set";
            // 
            // rbManualTextures
            // 
            rbManualTextures.AutoSize = true;
            rbManualTextures.Dock = DockStyle.Top;
            rbManualTextures.FlatStyle = FlatStyle.Flat;
            rbManualTextures.Location = new Point(3, 42);
            rbManualTextures.Name = "rbManualTextures";
            rbManualTextures.Padding = new Padding(2);
            rbManualTextures.Size = new Size(439, 23);
            rbManualTextures.TabIndex = 1;
            rbManualTextures.TabStop = true;
            rbManualTextures.Text = "Set each face texture individualy";
            rbManualTextures.UseVisualStyleBackColor = true;
            // 
            // rbSingleTexture
            // 
            rbSingleTexture.AutoSize = true;
            rbSingleTexture.Dock = DockStyle.Top;
            rbSingleTexture.FlatStyle = FlatStyle.Flat;
            rbSingleTexture.Location = new Point(3, 19);
            rbSingleTexture.Name = "rbSingleTexture";
            rbSingleTexture.Padding = new Padding(2);
            rbSingleTexture.Size = new Size(439, 23);
            rbSingleTexture.TabIndex = 0;
            rbSingleTexture.TabStop = true;
            rbSingleTexture.Text = "Use only one texture for all faces";
            rbSingleTexture.UseVisualStyleBackColor = true;
            // 
            // grpOutput
            // 
            grpOutput.BackColor = Color.FromArgb(71, 80, 94);
            grpOutput.Controls.Add(btnBrowseOutput);
            grpOutput.Controls.Add(panel6);
            grpOutput.Dock = DockStyle.Top;
            grpOutput.ForeColor = Color.White;
            grpOutput.Location = new Point(10, 240);
            grpOutput.Name = "grpOutput";
            grpOutput.Padding = new Padding(10);
            grpOutput.Size = new Size(465, 92);
            grpOutput.TabIndex = 3;
            grpOutput.TabStop = false;
            grpOutput.Text = "Resource Directory";
            // 
            // btnBrowseOutput
            // 
            btnBrowseOutput.Dock = DockStyle.Top;
            btnBrowseOutput.ForeColor = Color.Black;
            btnBrowseOutput.Location = new Point(10, 49);
            btnBrowseOutput.Name = "btnBrowseOutput";
            btnBrowseOutput.Size = new Size(445, 27);
            btnBrowseOutput.TabIndex = 2;
            btnBrowseOutput.Text = "Search...";
            btnBrowseOutput.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            panel6.Controls.Add(txtOutputPath);
            panel6.Controls.Add(lblOutputPath);
            panel6.Dock = DockStyle.Top;
            panel6.Location = new Point(10, 26);
            panel6.Name = "panel6";
            panel6.Size = new Size(445, 23);
            panel6.TabIndex = 1;
            // 
            // txtOutputPath
            // 
            txtOutputPath.BackColor = Color.FromArgb(132, 151, 181);
            txtOutputPath.BorderStyle = BorderStyle.FixedSingle;
            txtOutputPath.Dock = DockStyle.Fill;
            txtOutputPath.Location = new Point(44, 0);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.ReadOnly = true;
            txtOutputPath.Size = new Size(401, 23);
            txtOutputPath.TabIndex = 0;
            // 
            // lblOutputPath
            // 
            lblOutputPath.Dock = DockStyle.Left;
            lblOutputPath.Location = new Point(0, 0);
            lblOutputPath.Margin = new Padding(5, 0, 5, 0);
            lblOutputPath.Name = "lblOutputPath";
            lblOutputPath.Size = new Size(44, 23);
            lblOutputPath.TabIndex = 1;
            lblOutputPath.Text = "Path:";
            lblOutputPath.TextAlign = ContentAlignment.MiddleRight;
            // 
            // grpPreview
            // 
            grpPreview.BackColor = SystemColors.Control;
            grpPreview.Controls.Add(lstVariants);
            grpPreview.Dock = DockStyle.Fill;
            grpPreview.Location = new Point(10, 26);
            grpPreview.Name = "grpPreview";
            grpPreview.Size = new Size(364, 565);
            grpPreview.TabIndex = 1;
            grpPreview.TabStop = false;
            grpPreview.Text = "Generated Files Preview";
            // 
            // lstVariants
            // 
            lstVariants.BorderStyle = BorderStyle.None;
            lstVariants.Dock = DockStyle.Fill;
            lstVariants.Font = new Font("Consolas", 9F);
            lstVariants.FormattingEnabled = true;
            lstVariants.HorizontalScrollbar = true;
            lstVariants.ItemHeight = 14;
            lstVariants.Location = new Point(3, 19);
            lstVariants.Name = "lstVariants";
            lstVariants.Size = new Size(358, 543);
            lstVariants.TabIndex = 0;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(pnlTextureInputs);
            groupBox5.Dock = DockStyle.Fill;
            groupBox5.ForeColor = Color.White;
            groupBox5.Location = new Point(10, 95);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(445, 103);
            groupBox5.TabIndex = 5;
            groupBox5.TabStop = false;
            groupBox5.Text = "Faces of";
            // 
            // pnlTextureInputs
            // 
            pnlTextureInputs.Dock = DockStyle.Fill;
            pnlTextureInputs.Location = new Point(3, 19);
            pnlTextureInputs.Name = "pnlTextureInputs";
            pnlTextureInputs.Size = new Size(439, 81);
            pnlTextureInputs.TabIndex = 0;
            // 
            // groupBox6
            // 
            groupBox6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox6.BackColor = Color.FromArgb(21, 30, 43);
            groupBox6.Controls.Add(btnGenerateBlockstates);
            groupBox6.Controls.Add(btnGenerateItems);
            groupBox6.Controls.Add(btnGenerateModels);
            groupBox6.Controls.Add(btnGenerateAll);
            groupBox6.Dock = DockStyle.Bottom;
            groupBox6.ForeColor = Color.White;
            groupBox6.Location = new Point(10, 540);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(465, 51);
            groupBox6.TabIndex = 6;
            groupBox6.TabStop = false;
            groupBox6.Text = "Controlls";
            // 
            // btnGenerateBlockstates
            // 
            btnGenerateBlockstates.AutoSize = true;
            btnGenerateBlockstates.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGenerateBlockstates.BackColor = Color.FromArgb(176, 205, 247);
            btnGenerateBlockstates.Dock = DockStyle.Left;
            btnGenerateBlockstates.ForeColor = Color.Black;
            btnGenerateBlockstates.Location = new Point(298, 19);
            btnGenerateBlockstates.Name = "btnGenerateBlockstates";
            btnGenerateBlockstates.Padding = new Padding(2);
            btnGenerateBlockstates.Size = new Size(130, 29);
            btnGenerateBlockstates.TabIndex = 7;
            btnGenerateBlockstates.Text = "Generate Blockstates";
            btnGenerateBlockstates.UseVisualStyleBackColor = false;
            // 
            // btnGenerateItems
            // 
            btnGenerateItems.AutoSize = true;
            btnGenerateItems.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGenerateItems.BackColor = Color.FromArgb(176, 205, 247);
            btnGenerateItems.Dock = DockStyle.Left;
            btnGenerateItems.ForeColor = Color.Black;
            btnGenerateItems.Location = new Point(198, 19);
            btnGenerateItems.Name = "btnGenerateItems";
            btnGenerateItems.Padding = new Padding(2);
            btnGenerateItems.Size = new Size(100, 29);
            btnGenerateItems.TabIndex = 6;
            btnGenerateItems.Text = "Generate Items";
            btnGenerateItems.UseVisualStyleBackColor = false;
            // 
            // btnGenerateModels
            // 
            btnGenerateModels.AutoSize = true;
            btnGenerateModels.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGenerateModels.BackColor = Color.FromArgb(176, 205, 247);
            btnGenerateModels.Dock = DockStyle.Left;
            btnGenerateModels.ForeColor = Color.Black;
            btnGenerateModels.Location = new Point(88, 19);
            btnGenerateModels.Name = "btnGenerateModels";
            btnGenerateModels.Padding = new Padding(2);
            btnGenerateModels.Size = new Size(110, 29);
            btnGenerateModels.TabIndex = 5;
            btnGenerateModels.Text = "Generate Models";
            btnGenerateModels.UseVisualStyleBackColor = false;
            // 
            // btnGenerateAll
            // 
            btnGenerateAll.AutoSize = true;
            btnGenerateAll.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGenerateAll.BackColor = Color.FromArgb(176, 205, 247);
            btnGenerateAll.Dock = DockStyle.Left;
            btnGenerateAll.ForeColor = Color.Black;
            btnGenerateAll.Location = new Point(3, 19);
            btnGenerateAll.Name = "btnGenerateAll";
            btnGenerateAll.Padding = new Padding(2);
            btnGenerateAll.Size = new Size(85, 29);
            btnGenerateAll.TabIndex = 4;
            btnGenerateAll.Text = "Generate All";
            btnGenerateAll.UseVisualStyleBackColor = false;
            // 
            // grpWeights
            // 
            grpWeights.BackColor = SystemColors.Control;
            grpWeights.Controls.Add(flpWeights);
            grpWeights.Dock = DockStyle.Fill;
            grpWeights.Location = new Point(10, 110);
            grpWeights.Name = "grpWeights";
            grpWeights.Size = new Size(205, 481);
            grpWeights.TabIndex = 7;
            grpWeights.TabStop = false;
            grpWeights.Text = "Blockstate weights";
            // 
            // flpWeights
            // 
            flpWeights.Dock = DockStyle.Fill;
            flpWeights.ForeColor = Color.Black;
            flpWeights.Location = new Point(3, 19);
            flpWeights.Name = "flpWeights";
            flpWeights.Size = new Size(199, 459);
            flpWeights.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.BackColor = Color.FromArgb(71, 80, 94);
            groupBox2.Controls.Add(panel8);
            groupBox2.Controls.Add(panel7);
            groupBox2.Dock = DockStyle.Top;
            groupBox2.ForeColor = Color.White;
            groupBox2.Location = new Point(10, 26);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(10);
            groupBox2.Size = new Size(205, 84);
            groupBox2.TabIndex = 8;
            groupBox2.TabStop = false;
            groupBox2.Text = "Apparence of Block Item";
            // 
            // panel8
            // 
            panel8.Controls.Add(cmbItemVariant2);
            panel8.Controls.Add(lblItemVariant2);
            panel8.Dock = DockStyle.Top;
            panel8.Enabled = false;
            panel8.Location = new Point(10, 49);
            panel8.Name = "panel8";
            panel8.Size = new Size(185, 23);
            panel8.TabIndex = 2;
            // 
            // cmbItemVariant2
            // 
            cmbItemVariant2.BackColor = Color.FromArgb(132, 151, 181);
            cmbItemVariant2.Dock = DockStyle.Fill;
            cmbItemVariant2.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItemVariant2.FlatStyle = FlatStyle.Flat;
            cmbItemVariant2.ForeColor = Color.Black;
            cmbItemVariant2.FormattingEnabled = true;
            cmbItemVariant2.Location = new Point(77, 0);
            cmbItemVariant2.Name = "cmbItemVariant2";
            cmbItemVariant2.Size = new Size(108, 23);
            cmbItemVariant2.TabIndex = 3;
            // 
            // lblItemVariant2
            // 
            lblItemVariant2.Dock = DockStyle.Left;
            lblItemVariant2.ForeColor = Color.White;
            lblItemVariant2.Location = new Point(0, 0);
            lblItemVariant2.Margin = new Padding(5, 0, 5, 0);
            lblItemVariant2.Name = "lblItemVariant2";
            lblItemVariant2.Size = new Size(77, 23);
            lblItemVariant2.TabIndex = 1;
            lblItemVariant2.Text = "Mult. Variant:";
            lblItemVariant2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panel7
            // 
            panel7.Controls.Add(cmbItemVariant);
            panel7.Controls.Add(lblItemVariant);
            panel7.Dock = DockStyle.Top;
            panel7.Location = new Point(10, 26);
            panel7.Name = "panel7";
            panel7.Size = new Size(185, 23);
            panel7.TabIndex = 1;
            // 
            // cmbItemVariant
            // 
            cmbItemVariant.BackColor = Color.FromArgb(132, 151, 181);
            cmbItemVariant.Dock = DockStyle.Fill;
            cmbItemVariant.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItemVariant.FlatStyle = FlatStyle.Flat;
            cmbItemVariant.ForeColor = Color.Black;
            cmbItemVariant.FormattingEnabled = true;
            cmbItemVariant.Location = new Point(77, 0);
            cmbItemVariant.Name = "cmbItemVariant";
            cmbItemVariant.Size = new Size(108, 23);
            cmbItemVariant.TabIndex = 3;
            // 
            // lblItemVariant
            // 
            lblItemVariant.Dock = DockStyle.Left;
            lblItemVariant.ForeColor = Color.White;
            lblItemVariant.Location = new Point(0, 0);
            lblItemVariant.Margin = new Padding(5, 0, 5, 0);
            lblItemVariant.Name = "lblItemVariant";
            lblItemVariant.Size = new Size(77, 23);
            lblItemVariant.TabIndex = 1;
            lblItemVariant.Text = "Base:";
            lblItemVariant.TextAlign = ContentAlignment.MiddleRight;
            // 
            // groupBox3
            // 
            groupBox3.BackColor = Color.FromArgb(71, 80, 94);
            groupBox3.Controls.Add(groupBox5);
            groupBox3.Controls.Add(grpTextures);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.ForeColor = Color.White;
            groupBox3.Location = new Point(10, 332);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(10);
            groupBox3.Size = new Size(465, 208);
            groupBox3.TabIndex = 9;
            groupBox3.TabStop = false;
            groupBox3.Text = "Texture Settings";
            // 
            // groupBox4
            // 
            groupBox4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox4.BackColor = Color.FromArgb(71, 80, 94);
            groupBox4.Controls.Add(groupBox3);
            groupBox4.Controls.Add(grpOutput);
            groupBox4.Controls.Add(groupBox6);
            groupBox4.Controls.Add(groupBox1);
            groupBox4.Dock = DockStyle.Left;
            groupBox4.ForeColor = Color.White;
            groupBox4.Location = new Point(0, 0);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(10);
            groupBox4.Size = new Size(485, 601);
            groupBox4.TabIndex = 0;
            groupBox4.TabStop = false;
            groupBox4.Text = "Input";
            // 
            // groupBox7
            // 
            groupBox7.BackColor = Color.FromArgb(71, 80, 94);
            groupBox7.Controls.Add(grpPreview);
            groupBox7.Dock = DockStyle.Right;
            groupBox7.ForeColor = Color.White;
            groupBox7.Location = new Point(710, 0);
            groupBox7.Name = "groupBox7";
            groupBox7.Padding = new Padding(10);
            groupBox7.Size = new Size(384, 601);
            groupBox7.TabIndex = 13;
            groupBox7.TabStop = false;
            groupBox7.Text = "Output";
            // 
            // groupBox8
            // 
            groupBox8.BackColor = Color.FromArgb(71, 80, 94);
            groupBox8.Controls.Add(grpWeights);
            groupBox8.Controls.Add(groupBox2);
            groupBox8.Dock = DockStyle.Fill;
            groupBox8.ForeColor = Color.White;
            groupBox8.Location = new Point(485, 0);
            groupBox8.Name = "groupBox8";
            groupBox8.Padding = new Padding(10);
            groupBox8.Size = new Size(225, 601);
            groupBox8.TabIndex = 14;
            groupBox8.TabStop = false;
            groupBox8.Text = "Customize final result";
            // 
            // MainForm
            // 
            ClientSize = new Size(1094, 601);
            Controls.Add(groupBox8);
            Controls.Add(groupBox7);
            Controls.Add(groupBox4);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1110, 640);
            Name = "MainForm";
            Text = "Block File Generator for Minecraft";
            groupBox1.ResumeLayout(false);
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            panel5.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numVariantCount).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            grpTextures.ResumeLayout(false);
            grpTextures.PerformLayout();
            grpOutput.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            grpPreview.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            grpWeights.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            panel8.ResumeLayout(false);
            panel7.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox7.ResumeLayout(false);
            groupBox8.ResumeLayout(false);
            ResumeLayout(false);

        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void numVariantCount_ValueChanged(object sender, EventArgs e)
        {
            lblVariantPattern.Enabled = txtVariantPattern.Enabled = numVariantCount.Value > 0;
        }

        private void cbMultipleBlocks_CheckedChanged(object sender, EventArgs e)
        {
            panel8.Enabled = cbMultipleBlocks.Checked ? true : false;
            RefreshGeneratedFilesPreview(
                VariantService.BuildVariantNames(
                    txtBlockName.Text,
                    txtVariantPattern.Text,
                    (int)numVariantCount.Value));
        }

        private void txtVariantPattern_DataContextChanged(object sender, EventArgs e)
        {
            lblItemVariant2.Text = $"{txtVariantPattern.Text.Replace("{variant}", "")}:";
        }

        private string GetSelectedBlockTypePath()
        {
            switch ((cmbBlockType.Text ?? string.Empty).Trim())
            {
                case "Block": return "";
                case "Slab": return "slab";
                case "Stairs": return "stairs";
                case "Wall": return "wall";
                case "Door": return "door";
                case "Leaves": return "leaves";
                case "Trapdoor": return "trapdoor";
                case "Column": return "column";
                case "Column 2": return "column2";
                case "Fence Gate": return "fence_gate";
                case "Fence": return "fence";
                case "Cross": return "cross";
                case "Carpet": return "carpet";
                case "Torch": return "torch";
                case "Pane": return "pane";
                case "Rail": return "rail";
                default: return "";
            }
        }

        public string GetAlternativeBlockName(string name)
        {
            if (!cbAlternative.Checked)
                return name;

            name = name.Trim();
            switch ((cmbBlockType.Text ?? string.Empty).Trim())
            {
                case "Block": return name;
                case "Slab": return ReplaceNameCommon(name) + "_slab";
                case "Stairs": return ReplaceNameCommon(name) + "_stairs";
                case "Wall": return ReplaceNameCommon(name) + "_wall";
                case "Door": return ReplaceNameCommon(name) + "_door";
                case "Leaves": return ReplaceNameCommon(name) + "_leaves";
                case "Trapdoor": return ReplaceNameCommon(name) + "_trapdoor";
                case "Column": return ReplaceNameCommon(name) + "_column";
                case "Column 2": return ReplaceNameCommon(name) + "_column2";
                case "Fence Gate": return ReplaceNameCommon(name) + "_fence_gate";
                case "Fence": return ReplaceNameCommon(name) + "_fence";
                case "Cross": return ReplaceNameCommon(name) + "_cross";
                case "Carpet": return ReplaceNameCommon(name) + "_carpet";
                case "Torch": return ReplaceNameCommon(name) + "_torch";
                case "Pane": return ReplaceNameCommon(name) + "_pane";
                case "Rail": return ReplaceNameCommon(name) + "_rail";
                default: return "";
            }
        }

        private string ReplaceNameCommon(string name)
        {
            return name.Replace("_planks", "");
        }
    } 
}
