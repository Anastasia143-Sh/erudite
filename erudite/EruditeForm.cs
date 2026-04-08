using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace erudite
{
    public partial class EruditeForm : Form
    {
        private InitialForm _previousForm;
        private List<string> _playerNames;
        private int _playerCount;
        public EruditeForm(InitialForm previousForm, List<string> playerNames, int playerCount)
        {
            InitializeComponent();
            _previousForm = previousForm;
            _playerNames = playerNames;
            _playerCount = playerCount;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _previousForm.Show();
            this.Close();
        }
    }
}
