using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static test.Form1;

namespace test
{
    public partial class QuestionForm : Form
    {


        public List<Choice> Choices { get; set; }
        public List<RadioButton> ChoiceRadioButtons { get; private set; }
        public List<int> SelectedChoiceIdList { get; private set; } // Değişen isim

        public Choice SelectedChoice { get; private set; }
        public Question Question { get; set; }
        private Button nextQuestionButton;



        public QuestionForm()
        {
            InitializeComponent();
            ChoiceRadioButtons = new List<RadioButton>();
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            SelectedChoiceIdList = new List<int>();
        }

        public string QuestionText
        {
            get { return questionLabel.Text; }
            set { questionLabel.Text = value; }
        }


        public List<int> SelectedChoiceIds // Değişen isim
        {
            get
            {
                return SelectedChoiceIdList;
            }
        }

        public void LoadChoices(List<Choice> choices)
        {
            // Clear existing controls
            flowLayoutPanel1.Controls.Clear();

            foreach (var choice in choices)
            {
                RadioButton radioButton = new RadioButton();

                radioButton.Text = choice.content;
                radioButton.Tag = choice.id;
                radioButton.AutoSize = true;

                radioButton.CheckedChanged += (sender, e) =>
                {
                    if (radioButton.Checked)
                    {
                        int choiceId = (int)radioButton.Tag;
                        if (!SelectedChoiceIdList.Contains(choiceId)) // Değişen isim
                        {
                            SelectedChoiceIdList.Add(choiceId); // Değişen isim
                        }
                    }
                };

                // Add the RadioButton and choice content as a label
                Label choiceLabel = new Label
                {
                    Text = choice.content,
                    AutoSize = true
                };

                flowLayoutPanel1.Controls.Add(radioButton);
            }
        }


        private void NextQuestionButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

}