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
        public List<int> SelectedChoiceIdList { get; private set; } // seçilen seçeneklerin listesi

        public Choice SelectedChoice { get; private set; }
        public Question Question { get; set; }
        private Button nextQuestionButton;



        public QuestionForm()
        {
            InitializeComponent();
            ChoiceRadioButtons = new List<RadioButton>();
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown; // radiobuttonları dikey olarak sıralamak için
            SelectedChoiceIdList = new List<int>();
        }

        public string QuestionText
        {
            get { return questionLabel.Text; }  //soruyu bu labela yazdırıyor
            set { questionLabel.Text = value; }
        }


        public List<int> SelectedChoiceIds 
        {
            get
            {
                return SelectedChoiceIdList;
            }
        }

        public void LoadChoices(List<Choice> choices) //seçenekler sınıfından liste halinde seçenekleri yüklemek için
        {
            //önceki soruyu temizle
            flowLayoutPanel1.Controls.Clear(); 

            foreach (var choice in choices)  // seçenekler arasından seçenek getir
            {
                RadioButton radioButton = new RadioButton(); //yeni radiobutton oluştr

                radioButton.Text = choice.content; //radio button içeriği seçenek içeriği olsun
                radioButton.Tag = choice.id; //tag'ı ID olsun
                radioButton.AutoSize = true;

                radioButton.CheckedChanged += (sender, e) => // seçenek seçildiğinde olayı tetiklemek için
                {
                    if (radioButton.Checked) // eğer seçildiyse
                    {
                        int choiceId = (int)radioButton.Tag;
                        if (!SelectedChoiceIdList.Contains(choiceId)) // eğer bu Id'de bir ıd yoksa
                        {
                            SelectedChoiceIdList.Add(choiceId); // ekle
                        }
                    }
                };

                
                Label choiceLabel = new Label 
                {
                    Text = choice.content,  //soru içeriğini radiobutona ekle
                    AutoSize = true
                };

                flowLayoutPanel1.Controls.Add(radioButton); // panele radiobutton ekle
            }
        }


        private void NextQuestionButton_Click(object sender, EventArgs e) //yeni soruya geç
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

}