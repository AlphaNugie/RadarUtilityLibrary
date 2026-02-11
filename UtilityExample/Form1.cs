using ArsLibrary.Core.Rhd;
using ArsLibrary.Model.Rhd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UtilityExample
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox_ProtocolVersion.SelectedIndex = 0;
        }

        private void Button_Parse_Click(object sender, EventArgs e)
        {
            string hex = richTextBox_Input.Text;
            //选择协议类型
            var verStr = comboBox_ProtocolVersion.SelectedItem.ToString();
            RadarParseConfig config = RadarParseConfig.Rhd;
            if (verStr.Contains("2.1.2"))
                config = RadarParseConfig.Fd;
            //RadarPacket radarPacket = RadarDataParser.Parse(hex);
            RadarPacket radarPacket = RadarDataParser.Parse(hex, out string pHex, null, config);
            richTextBox_RadarPacket.Text = radarPacket.ToString();
            richTextBox_Input.Text = pHex;
        }
    }
}
