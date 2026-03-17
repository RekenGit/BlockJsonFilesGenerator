using System;

namespace MinecraftJsonGenerator
{
    public class TextureWeightRegistry
    {
        public event Action? WeightsChanged;
        public static Dictionary<string, int> Weights { get; private set; } = [];
        public static void AddWeight(string name, int weight)
        {
            if (Weights.ContainsKey(name))
                Weights[name] = weight;
            else
                Weights.Add(name, weight);
            MainForm.AllWeights?.WeightsChanged?.Invoke();
        }
        public static void ChangeWeight(string name, int weight)
        {
            Weights[name] = weight;
            MainForm.AllWeights?.WeightsChanged?.Invoke();
        }
        public static void RemoveWeight(string name)
        {
            Weights.Remove(name);
            MainForm.AllWeights?.WeightsChanged?.Invoke();
        }

        public static void RestartWeights()
        {
            Weights.Clear();
        }

        public static int GetPercentage(string name)
        {
            if (!Weights.ContainsKey(name))
                return 0;

            var totalWeight = Weights.Values.Sum();
            if (totalWeight == 0)
                return 0;

            return (int)Math.Round((double)Weights[name] / totalWeight * 100);
        }

        public static int GetValue(string name)
        {
            if (!Weights.ContainsKey(name))
                return 0;

            var totalWeight = Weights.Values.Sum();
            if (totalWeight == 0 || Weights[name] < 1)
                return 0;

            return Weights[name];
        }
    }

    public partial class textureWeightConfigControl : UserControl
    {
        public Image TextureImage { get; private set; }
        public string TextureName { get; private set; } = string.Empty;
        public int Weight { get; private set; } = 1;

        public textureWeightConfigControl(string variantName, int defaultWeight, Image img)
        {
            InitializeComponent();
            pixelArtPictureBox1.Image = TextureImage = img;
            label1.Text = TextureName = variantName;
            trackBar1.Value = Weight = defaultWeight;
            numericUpDown1.Value = defaultWeight;

            MainForm.AllWeights.WeightsChanged += UpdatePercentageLabel;
            TextureWeightRegistry.AddWeight(TextureName, Weight);
            this.Disposed += TextureWeightConfigControl_Disposed;
            //this.ControlRemoved += TextureWeightConfigControl_Disposed;
        }

        private void UpdatePercentageLabel()
        {
            _updatingWeight = true;
            int _weight = TextureWeightRegistry.GetValue(TextureName);
            Weight = _weight == 0 ? 1 : _weight;
            numericUpDown1.Value = trackBar1.Value = Weight;
            label2.Text = $"{TextureWeightRegistry.GetPercentage(TextureName)}%";
            _updatingWeight = false;
        }

        private bool _updatingWeight;
        private void WeightTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (_updatingWeight)
                return;

            try
            {
                _updatingWeight = true;
                Weight = trackBar1.Value;
                numericUpDown1.Value = Weight;
                TextureWeightRegistry.ChangeWeight(TextureName, Weight);
            }
            finally
            {
                _updatingWeight = false;
            }
        }

        private void WeightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_updatingWeight)
                return;

            try
            {
                _updatingWeight = true;
                Weight = (int)numericUpDown1.Value;
                trackBar1.Value = Weight;
                TextureWeightRegistry.ChangeWeight(TextureName, Weight);
            }
            finally
            {
                _updatingWeight = false;
            }
        }

        private void TextureWeightConfigControl_Disposed(object sender, EventArgs e)
        {
            TextureWeightRegistry.RemoveWeight(TextureName);
            MainForm.AllWeights.WeightsChanged -= UpdatePercentageLabel;
        }
    }
}
