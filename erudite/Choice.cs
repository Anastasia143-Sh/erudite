using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace erudite
{
    public partial class Choice : Form
    {
        private InitialForm _previousForm;
        private List<string> _playerNames;
        private int _playerCount;

        public Choice(InitialForm previousForm, List<string> playerNames, int playerCount)
        {
            InitializeComponent();
            _previousForm = previousForm;
            _playerNames = playerNames;
            _playerCount = playerCount;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _previousForm.Show();
            this.Close();
        }

        private void btnCoice_Click(object sender, EventArgs e)
        {
            EruditeForm newForm = new EruditeForm(_previousForm, _playerNames, _playerCount);
            newForm.Show();
            this.Close();
        }
    }
}
