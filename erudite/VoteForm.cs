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
    /// <summary>
    /// Форма для проведения голосования за слова, предложенные текущим игроком
    /// Позволяет другим игрокам проголосовать «за» или «против», определяет результат по большинству голосов
    /// </summary>
    public partial class VoteForm : Form
    {
        private List<Player> voters;
        private Dictionary<Player, bool> votes = new Dictionary<Player, bool>();
        private Button btnSubmit;
        public bool Accepted { get; private set; } // результат голосования

        /// <summary>
        /// Конструктор формы голосования
        /// Инициализирует интерфейс, отображает слова и очки, создаёт элементы для голосования каждого игрока
        /// </summary>
        /// <param name="currentPlayerName">Имя игрока, предложившего слова</param>
        /// <param name="words">Список слов, требующих голосования</param>
        /// <param name="score">Количество очков за предложенные слова</param>
        /// <param name="otherPlayers">Список игроков, которые будут голосовать</param>
        public VoteForm(string currentPlayerName, List<string> words, int score, List<Player> otherPlayers)
        {
            voters = otherPlayers;
            Text = "Голосование за слова";
            Size = new Size(450, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // создаём контейнер для размещения элементов интерфейса (таблица с 3 строками)
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3 };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Метка с информацией о словах и очках, предложенных для голосования
            var infoLabel = new Label
            {
                Text = $"Игрок {currentPlayerName} вставил слова:\n{string.Join(", ", words)}\nНа сумму {score} очков.\nПроголосуйте:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };
            layout.Controls.Add(infoLabel, 0, 0);

            // Панель для размещения групп голосования по каждому игроку
            var votesPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoScroll = true, Dock = DockStyle.Fill };
            foreach (var p in voters)
            {
                // Группа для голосования одного игрока (с именем в заголовке)
                var group = new GroupBox { Text = p.Name, Height = 50, Width = 300, AutoSize = true };
                // Радиокнопка «Принять» (по умолчанию выбрана)
                var rbYes = new RadioButton { Text = "Принять", Location = new Point(10, 20), Checked = true, AutoSize = true };
                // Радиокнопка «Отклонить»
                var rbNo = new RadioButton { Text = "Отклонить", Location = new Point(150, 20), AutoSize = true };
                rbYes.Tag = p; // Сохраняем ссылку на игрока в теге кнопки
                rbNo.Tag = p;
                // Обработчик изменения выбора «Принять»: устанавливаем голос игрока как true
                rbYes.CheckedChanged += (s, e) => { if (rbYes.Checked) votes[p] = true; };
                // Обработчик изменения выбора «Отклонить»: устанавливаем голос игрока как false
                rbNo.CheckedChanged += (s, e) => { if (rbNo.Checked) votes[p] = false; };
                group.Controls.Add(rbYes);
                group.Controls.Add(rbNo);
                votesPanel.Controls.Add(group);
                votes[p] = true; // По умолчанию все голоса «за»
            }
            layout.Controls.Add(votesPanel, 0, 1);

            // Кнопка подтверждения голосования
            btnSubmit = new Button { Text = "Завершить голосование", AutoSize = true, Anchor = AnchorStyles.None };
            btnSubmit.Click += BtnSubmit_Click;
            // Панель для размещения кнопки внизу формы
            var btnPanel = new Panel { Height = 50, Dock = DockStyle.Bottom };
            btnPanel.Controls.Add(btnSubmit);
            layout.Controls.Add(btnPanel, 0, 2);

            Controls.Add(layout);
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Завершить голосование»
        /// Подсчитывает голоса, определяет результат (большинство «за»/«против»), закрывает форму с результатом
        /// </summary>\
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            // Подсчёт голосов «за» (true)
            int yesCount = votes.Count(v => v.Value);
            // Общее количество голосующих игроков
            int total = voters.Count;
            // Слова принимаются, если голосов «за» больше половины
            Accepted = yesCount > total / 2;
            // Устанавливаем результат диалога и закрываем форму
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
