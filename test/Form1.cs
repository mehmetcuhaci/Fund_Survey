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
        private List<Response> userResponses = new List<Response>(); // kullanıcı girişlerini liste halinde topla
        private static object https;
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string apiUrl = "https://dev.service.yancep.net/Survey/GetSurvey"; // anketi getirmek için kullanılan link
            string apiKey = "941bf440-4cc5-11ee-be56-0242ac120002"; // kullanabilmek için api key

            using (HttpClient client = new HttpClient())  // web servisinden veri alabilmek için metod
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);  // api key girişi

                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl); // apiUrlye giriş denemesi

                    if (response.IsSuccessStatusCode) // eğer giriş başarılıysa
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();  //gelen veri
                        Root root = JsonConvert.DeserializeObject<Root>(responseBody); // gelen veriyi jsona çevir

                        foreach (var question in root.data.questions)  //gelen data içinde soruları gez
                        {
                            using (QuestionForm questionForm = new QuestionForm())  // soru formuna gir
                            {
                                questionForm.QuestionText = $"{question.content}";  // soru formunda soru içeriğini al

                                questionForm.LoadChoices(question.choices); // seçenekleri al

                                DialogResult result = questionForm.ShowDialog(); // soru formunu getir

                                if (result == DialogResult.OK) // eğer form tamamlanırsa
                                {
                                    List<int> selectedChoiceIds = questionForm.SelectedChoiceIds;  // seçilen seçenekleri topla

                                    foreach (int choiceId in selectedChoiceIds) //seçilen seçenekleri idlerine ayır
                                    {
                                        userResponses.Add(new Response
                                        {
                                            questionId = question.id, // responsa soru ve seçenek idlerini kaydet
                                            choiceId = choiceId
                                        });
                                    }
                                }
                            }
                        }

                        string formattedResponses = FormatResponsesForMessageBox(userResponses); //test için soruları ve seçenekleri ekrana bastırdım
                        MessageBox.Show("Formatted Responses:\n\n" + formattedResponses);

                        await PostResponsesToWebService(userResponses);

                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode}");   
                    }
                }
                catch (Exception ex)                     /* Eğer bir sorun çıkarsa burada koduyla birlikte uyarısını vericek NotAllowed */
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

        }

        private async Task PostResponsesToWebService(List<Response> userResponses)  // toplanan anket verisini göndermek için
        {

            string apiUrl = "https://dev.service.yancep.net/Survey/GetSurveyResult";
            string apiKey = "941bf440-4cc5-11ee-be56-0242ac120002";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                try
                {
                    var postData = new  //postlanıcak json için 
                    {
                        results = userResponses,
                        surveyCode = "Suggest"
                    };

                    string postDataJson = JsonConvert.SerializeObject(postData);   // postDatayı jsona çevir

                    HttpResponseMessage response = await client.PostAsync(apiUrl, new StringContent(postDataJson, Encoding.UTF8, "application/json")); //application/jsona çevirdi

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(postDataJson); //test için göndermeden önce ekrana yazdırdı
                        MessageBox.Show("Anket gönderildi.");
                        var responseObject = JsonConvert.DeserializeObject<ResponseObject>(responseBody);

                        StringBuilder displayText = new StringBuilder();
                        // displayText.AppendLine("Response from the server:");
                        // displayText.AppendLine($"Suggestion ID: {responseObject.data.suggestion.id}");
                        displayText.AppendLine($"Yatırımcı Tipiniz : {responseObject.data.suggestion.name}");
                        displayText.AppendLine("Yatırım Yapabileceğiniz Fonlar :");

                        foreach (var suggestionFund in responseObject.data.suggestion.suggestionFunds)
                        {
                            displayText.AppendLine($"Önerilen Fon: {suggestionFund.name}");

                            foreach (var fund in suggestionFund.funds)
                            {
                                displayText.AppendLine($"Tefas Kodu: {fund.tefasCode}");
                                displayText.AppendLine($"Tefas Fon Başlığı: {fund.tefasFundDetail.fundTitle}");
                            }
                        }



                        MessageBox.Show(displayText.ToString());



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
                formattedResponses.AppendLine($"Question ID: {response.questionId}");  //test için ıdleri yazdırma
                formattedResponses.AppendLine($"Choice ID: {response.choiceId}");
                
            }
            return formattedResponses.ToString();
        }

        


        public class Result
        {
            public string content { get; set; }
            public object detail { get; set; }

        }

        public class DataResponse
        {
            public List<Result> results { get; set; }
        }

        public class RootResponse
        {
            public DataResponse data { get; set; }
            public bool success { get; set; }
            public object error { get; set; }
        }


        public class Response    /* Çekilen dataları sınıflara verildi gerekli olanlar sınıflardan liste halinde çekiliyor */
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
            public Suggestion suggestion { get; set; }
            public List<Answer> answers { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
            public bool success { get; set; }
            public object error { get; set; }
        }


        public class TefasFundDetail
        {
            public string fundTitle { get; set; }
            public long totalShareCount { get; set; }
            public int totalInvestorCount { get; set; }
            public double portfolioSize { get; set; }
        }

        public class Fund
        {
            public string tefasCode { get; set; }
            public TefasFundDetail tefasFundDetail { get; set; }
        }

        public class SuggestionFund
        {
            public string name { get; set; }
            public int weightPercentage { get; set; }
            public string color { get; set; }
            public List<Fund> funds { get; set; }
        }

        public class Suggestion
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<SuggestionFund> suggestionFunds { get; set; }
        }

        public class Answer
        {
            public string content { get; set; }
            public object detail { get; set; }
            public List<Choice> choices { get; set; }
        }

        public class ResponseObject
        {
            public Data data { get; set; }
            public bool success { get; set; }
            public object error { get; set; }
        }
    }

}