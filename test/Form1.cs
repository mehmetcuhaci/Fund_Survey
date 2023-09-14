using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace test
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public partial class Form1 : Form
    {
        private List<Response> userResponses = new List<Response>();
        private static object https;
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string apiUrl = "https://dev.service.yancep.net/Survey/GetSurvey"; // web servis bağlantısı için string oluştur
            string apiKey = "941bf440-4cc5-11ee-be56-0242ac120002";  //api key'i stringe tanımla

            using (HttpClient client = new HttpClient())   // web servis bağlantısı için httpclient kütüphanesi kullan
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey); //web servisi kullanabilmek için api key gir

                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)  // eğer giriş başarılı ise
                    {
                        string responseBody = await response.Content.ReadAsStringAsync(); //response sınıfından verileri al

                        Root root = JsonConvert.DeserializeObject<Root>(responseBody);  // aldığın verileri json formatına dönüştür

                        foreach (var question in root.data.questions)
                        {
                            using (QuestionForm questionForm = new QuestionForm())
                            {
                                questionForm.QuestionText = $"{question.content}";

                                questionForm.Question = question;
                                questionForm.LoadChoices(question.choices);

                                DialogResult result = questionForm.ShowDialog();

                                if (result == DialogResult.OK)
                                {
                                    List<Choice> selectedChoices = questionForm.SelectedChoices;

                                    // Ensure that selected choice IDs are captured
                                    List<int> selectedChoiceIds = selectedChoices.Select(choice => choice.ChoiceId).ToList(); // Use ChoiceId

                                    // Create a new Response object and set the selected choice IDs
                                    userResponses.Add(new Response
                                    {
                                        QuestionID = question.id,
                                        Question = question.content,
                                        SelectedChoiceIds = selectedChoiceIds
                                    });
                                }
                            }
                        }


                        string formattedResponses = FormatResponsesForMessageBox(userResponses);
                        MessageBox.Show("Formatted Responses:\n\n" + formattedResponses);

                        
                        await PostResponsesToWebService(userResponses);
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

        }

        private async Task PostResponsesToWebService(List<Response> userResponses)
        {

            string apiUrl = "https://dev.service.yancep.net/Survey/GetSurveyResult";
            string apiKey = "941bf440-4cc5-11ee-be56-0242ac120002";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                try
                {
                    // Create a JSON object containing the responses
                    var postData = new
                    {
                        results = userResponses,
                        surveyCode = "Suggest"
                    };

                    string postDataJson = JsonConvert.SerializeObject(postData);

                    // Send a POST request to submit the responses
                    HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(postDataJson, System.Text.Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Responses submitted successfully.");
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

        }

        private string FormatResponsesForMessageBox(List<Response> responses)
        {
            StringBuilder formattedResponses = new StringBuilder();
            foreach (var response in responses)
            {
                formattedResponses.AppendLine($"Question ID: {response.QuestionID}");
                formattedResponses.AppendLine($"Question: {response.Question}");
                
                foreach (var choiceId in response.SelectedChoiceIds)
                {
                    formattedResponses.AppendLine($"Selected Choices:{choiceId}");
                    
                }
                formattedResponses.AppendLine();
            }
            return formattedResponses.ToString();
        }


        






        public class Response
        {
            public string QuestionID { get; set; }
            public string Question { get; set; }
            public List<int> SelectedChoiceIds { get; set; }
        }

        public class Choice
        {
            public int id { get; set; }
            public string content { get; set; }
            public bool isSelected { get; set; }
            public int riskScore { get; set; }
            public int futureTransactionScore { get; set; }
            public string QuestionId { get; set; }
            public int ChoiceId { get; set; }
        }

        public class Question
        {
            public string id { get; set; }
            public DateTime createdDate { get; set; }
            public DateTime updatedDate { get; set; }
            public string content { get; set; }
            public object detail { get; set; }
            public List<Choice> choices { get; set; }
        }

        public class Data
        {
            public string id { get; set; }
            public DateTime createdDate { get; set; }
            public DateTime updatedDate { get; set; }
            public string name { get; set; }
            public bool active { get; set; }
            public string code { get; set; }
            public List<Question> questions { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
            public bool success { get; set; }
            public object error { get; set; }
        }

    }

}