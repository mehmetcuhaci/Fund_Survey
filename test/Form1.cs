using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            string apiUrl = "https://dev.service.yancep.net/Survey/GetSurvey";
            string apiKey = "941bf440-4cc5-11ee-be56-0242ac120002";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Root root = JsonConvert.DeserializeObject<Root>(responseBody);

                        foreach (var question in root.data.questions)
                        {
                            using (QuestionForm questionForm = new QuestionForm())
                            {
                                questionForm.QuestionText = $"{question.content}";

                                questionForm.LoadChoices(question.choices);

                                DialogResult result = questionForm.ShowDialog();

                                if (result == DialogResult.OK)
                                {
                                    List<int> selectedChoiceIds = questionForm.SelectedChoiceIds;

                                    foreach (int choiceId in selectedChoiceIds)
                                    {
                                        userResponses.Add(new Response
                                        {
                                            questionId = question.id,
                                            choiceId = choiceId
                                        });
                                    }
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
                    var postData = new
                    {
                        results = userResponses,
                        surveyCode = "Suggest"
                    };

                    string postDataJson = JsonConvert.SerializeObject(postData);

                    HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(postDataJson, Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show(postDataJson);
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
                formattedResponses.AppendLine($"Question ID: {response.questionId}");
                formattedResponses.AppendLine($"Choice ID: {response.choiceId}");
                formattedResponses.AppendLine();
            }
            return formattedResponses.ToString();
        }



        public class Response
        {
            public string questionId { get; set; }
            public int choiceId { get; set; }
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