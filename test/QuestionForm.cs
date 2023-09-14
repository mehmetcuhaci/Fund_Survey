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

       

        public Choice SelectedChoice { get; private set; }

        // Property to store the question
        public Question Question { get; set; }

        private Button nextQuestionButton;


        public QuestionForm()
        {
            InitializeComponent();
            ChoiceRadioButtons = new List<RadioButton>();
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            
        }

        public string QuestionText
        {
            get { return questionLabel.Text; }
            set { questionLabel.Text = value; }
        }


        public List<Choice> SelectedChoices
        {
            get
            {
                List<Choice> selectedChoices = new List<Choice>();
                foreach (var radioButton in ChoiceRadioButtons)
                {
                    if (radioButton.Checked)
                    {
                        selectedChoices.Add(new Choice
                        {
                            id = (int)radioButton.Tag,
                            content = radioButton.Text,
                            isSelected = true,
                            ChoiceId = (int)radioButton.Tag // Add ChoiceId to the selected choice
                        });
                    }
                }
                return selectedChoices;
            }
        }

        public void LoadChoices(List<Choice> choices)
        {
            // Clear existing controls
            flowLayoutPanel1.Controls.Clear();

            // int choiceIdCounter = 1; // Initialize the choice ID counter

            foreach (var choice in choices)
            {
                RadioButton radioButton = new RadioButton();
                //{
                //    Text = choice.content,
                //    Tag = choice.id,// choiceIdCounter, // Set the choice ID
                //    AutoSize = true,
                    
                //};

                radioButton.Text = choice.content;
                radioButton.Tag = choice.id;
                radioButton.AutoSize = true;


                radioButton.CheckedChanged += (sender, e) =>
                {
                    if (radioButton.Checked)
                    {
                        int choiceId = (int)radioButton.Tag;

                        // Use the Question property to access the question object
                        if (Question != null)
                        {
                            // Assign the question and choice IDs
                            choice.QuestionId = Question.id;
                            choice.ChoiceId = choice.id; // Set the ChoiceId when a choice is selected
                        }

                        Choice selectedChoice = choices.Find(c => c.id == choiceId);
                        if (selectedChoice != null)
                        {
                            if (!SelectedChoices.Any(c => c.id == choiceId))
                            {
                                SelectedChoices.Add(selectedChoice);
                            }
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
                flowLayoutPanel1.Controls.Add(choiceLabel); // Display the choice content

                // choiceIdCounter++; // Increment the choice ID counter
            }
        }


        private void NextQuestionButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

}