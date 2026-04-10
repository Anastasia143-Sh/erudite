using ClassLibrary;
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
    public partial class VoteForm : Form
    {
        private List<Player> voters;
        private Dictionary<Player, bool> votes = new Dictionary<Player, bool>();
        private Button btnSubmit;
        public bool Accepted { get; private set; }

        public VoteForm(string currentPlayerName, List<string> words, int score, List<Player> otherPlayers)
        {
            voters = otherPlayers;
            Text = "Голосование за слова";
            Size = new Size(400, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var infoLabel = new Label
            {
                Text = $"Игрок {currentPlayerName} вставил слова:\n{string.Join(", ", words)}\nНа сумму {score} очков.\nПроголосуйте:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };
            layout.Controls.Add(infoLabel, 0, 0);

            var votesPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoScroll = true, Dock = DockStyle.Fill };
            foreach (var p in voters)
            {
                var group = new GroupBox { Text = p.Name, Height = 60, Width = 250, AutoSize = false };
                var rbYes = new RadioButton { Text = "Принять", Location = new Point(10, 20), Checked = true };
                var rbNo = new RadioButton { Text = "Отклонить", Location = new Point(100, 20) };
                rbYes.Tag = p;
                rbNo.Tag = p;
                rbYes.CheckedChanged += (s, e) => { if (rbYes.Checked) votes[p] = true; };
                rbNo.CheckedChanged += (s, e) => { if (rbNo.Checked) votes[p] = false; };
                group.Controls.Add(rbYes);
                group.Controls.Add(rbNo);
                votesPanel.Controls.Add(group);
                votes[p] = true; // по умолчанию "за"
            }
            layout.Controls.Add(votesPanel, 0, 1);

            btnSubmit = new Button { Text = "Завершить голосование", Size = new Size(150, 30), Anchor = AnchorStyles.None };
            btnSubmit.Click += BtnSubmit_Click;
            var btnPanel = new Panel { Height = 50, Dock = DockStyle.Bottom };
            btnPanel.Controls.Add(btnSubmit);
            layout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(layout);
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            int yesCount = votes.Count(v => v.Value);
            int total = voters.Count;
            Accepted = yesCount > total / 2;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

