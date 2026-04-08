namespace erudite
{
    public partial class InitialForm : Form
    {
        public InitialForm()
        {
            InitializeComponent();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            Choice newForm = new Choice(this);
            newForm.Show();
            this.Hide();
        }
    }
}
