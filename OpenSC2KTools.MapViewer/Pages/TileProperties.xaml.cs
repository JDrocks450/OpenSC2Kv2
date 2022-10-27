using OpenSC2Kv2.API.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenSC2KTools.MapViewer.Pages
{
    /// <summary>
    /// Interaction logic for TileProperties.xaml
    /// </summary>
    public partial class TileProperties : Window
    {
        public TileProperties()
        {
            InitializeComponent();
        }
        public TileProperties(OpenSC2Kv2.API.World.OverridePhysicalPropertiesDefinition EditingDef) : this()
        {
            this.EditingDef = EditingDef;
            ShowDef();
        }

        public OverridePhysicalPropertiesDefinition EditingDef { get; private set; }

        private void ShowDef()
        {
            if (EditingDef == null) EditingDef = new OverridePhysicalPropertiesDefinition(); 
            XValBox.Text = EditingDef.ManualOffsetX.ToString();
            YValBox.Text = EditingDef.ManualOffsetY.ToString();
        }

        private bool AutoCorrect()
        {
            if (!int.TryParse(XValBox.Text, out var XVal) || !int.TryParse(YValBox.Text, out var YVal))
                return false;
            EditingDef.ManualOffsetX = XVal;
            EditingDef.ManualOffsetY = YVal;
            return true;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (!AutoCorrect())
            {
                MessageBox.Show("Whatever you entered is not right. Review your inputted values " +
                    "and try again.", "Invalid Submission");
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}
