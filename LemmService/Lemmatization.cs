using Core;
using LemmService;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace LemmatizationService
{
    public class Lemmatization : ILemmatization
    {
        const string Key1 = "4c5c170f760daf6b323dc61cd04c6d21980491e9";
        const string Key2 = "05653c23155ca40663a880980a9ee4cb716521ba";
        string key= Key1;        
        int counterBadResponseIspras;
        public Dictionary<string, int> GetDictFromLemms(string text)
        {
            if (counterBadResponseIspras<10)
            {
                var lemms = GetLemmas(text);
                Dictionary<string, int> DictFromLemms = new Dictionary<string, int>();
                if (lemms.Count() != 0)
                {
                    try
                    {
                        for (int i = 0; i < lemms.Count(); i++)
                        {
                            var annotations = lemms[i].annotations;

                            foreach (var lemma in annotations.lemma)
                            {
                                if (lemma.value != "")
                                {
                                    if (DictFromLemms.ContainsKey(lemma.value))
                                    {
                                        DictFromLemms[lemma.value] += 1;
                                    }
                                    else
                                    {
                                        DictFromLemms[lemma.value] = 1;
                                    }
                                }
                            }
                        }
                        return DictFromLemms;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"LemmatizationService.Lemmatization.GetDictFromLemms() worked with error {DateTime.Now}" +
                            $"{Environment.NewLine}{ex}{Environment.NewLine}");
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Environment.Exit(0);
                return null;
            }

        }

        private List<DesModel> GetLemmas(string text)
        {           
            var listStrings = SplitText1450character(text);
            List<DesModel> listDesModel = new List<DesModel>();
            try
            {
                foreach (var _string in listStrings)
                {
                    if (_string != "")
                    {
                        var lemmas = GetLemma(_string);
                        Thread.Sleep(500);
                        if (lemmas != null)
                        {
                            listDesModel.AddRange(lemmas);
                        }
                        else continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"LemmatizationService.Lemmatization.GetLemmas() worked with error {DateTime.Now}" +
                    $"{Environment.NewLine}{ex}{Environment.NewLine}");
            }
            return listDesModel;

        }
        private List<string> SplitText1450character(string text)
        {
            List<string> listStrings = new List<string>();
            try
            {
                if (text.Length >= 50000)
                {
                    Regex reg = new Regex(".{0,50000}");//the limitations of characters of Ispras request
                    var splitText = reg.Matches(text);
                    foreach (var item in splitText)
                    {
                        var cleanString = CleanText(item.ToString());
                        if (cleanString == "")
                        {
                            continue;
                        }
                        listStrings.Add(cleanString);
                    }
                }
                else
                {
                    listStrings.Add(text);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"LemmatizationService.Lemmatization.SplitText1450character() worked with error {DateTime.Now}" +
                    $"{Environment.NewLine}{ex}{Environment.NewLine}");
            }
            return listStrings;
        }

        private string GetKeyIspras() {return key = key == Key1 ? key = Key2 : key = Key1; }
       
        public List<DesModel> GetLemma(string splitStr)
        {            
            Log.Information($"LemmatizationService.Lemmatization.GetLemma() started at {DateTime.Now}{Environment.NewLine}");
            System.Diagnostics.Debug.WriteLine($"LemmatizationService.Lemmatization.GetLemma() started at {DateTime.Now}{Environment.NewLine}");
            string cont = $"[{{\"text\":\"{splitStr}\"}}]"; ;
            Task<string> responce = null;            
            List<DesModel> Lemms = new List<DesModel>();
            try
            {                
                int _counter = 1;
                link1:
                using (var client = new HttpClient())
                {
                   /* if (key==null)
                    {
                        key = GetKeyIspras();
                    }*/
                    
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"http://api.ispras.ru/texterra/v1/nlp?targetType=lemma&apikey={key}");
                    request.Content = new StringContent(cont, Encoding.UTF8, "application/json");//CONTENT-TYPE header
                    var x = client.SendAsync(request).Result;
                    responce = x.Content.ReadAsStringAsync();
                }
                List<DesModel> _Lemms = JsonConvert.DeserializeObject<List<DesModel>>(responce.Result);
                if (_Lemms != null)
                {
                    Lemms = _Lemms;
                   // Thread.Sleep(500);
                    counterBadResponseIspras = 0;
                }
                else
                {
                    if (_counter < 3)
                    {
                        _counter++;
                        Log.Information($"LemmatizationService.Lemmatization.GetLemma() worked with error {DateTime.Now}{Environment.NewLine}" +
                       $"Lemms from string {Environment.NewLine}" +
                       $"{splitStr}{Environment.NewLine} equally null{ Environment.NewLine}" +
                       $"Performing the {_counter} attempt of response  { Environment.NewLine}");
                        System.Diagnostics.Debug.WriteLine($"LemmatizationService.Lemmatization.GetLemma() worked with error {DateTime.Now}{Environment.NewLine}" +
                        $"Lemms from string {Environment.NewLine}" +
                        $"{splitStr}{Environment.NewLine} equally null{ Environment.NewLine}" +                        
                        $"key={key}{ Environment.NewLine}" +
                        $"Performing the {_counter} attempt of response  { Environment.NewLine}");
                        System.Diagnostics.Debug.WriteLine($"Thread.Sleep(30000)");
                        Thread.Sleep(30000);
                        
                        if (_counter == 3)
                        {
                            System.Diagnostics.Debug.WriteLine($"Thread.Sleep(60000)");
                            Thread.Sleep(30000);
                            Log.Information("Ups!!");
                            System.Diagnostics.Debug.WriteLine($"Thread.Sleep(60000)");
                            GetKeyIspras();
                        }
                        counterBadResponseIspras++;
                        Log.Information($"counterBadResponseIspras= {counterBadResponseIspras}");
                        System.Diagnostics.Debug.WriteLine($"counterBadResponseIspras= {counterBadResponseIspras}");
                        goto link1;//Trying sent request again                        
                    }

                    Log.Error($"LemmatizationService.Lemmatization.GetLemma() worked with error {DateTime.Now}{Environment.NewLine}" +
                    $"Lemms from string {Environment.NewLine}" +
                    $"{splitStr}{Environment.NewLine} equally null{ Environment.NewLine}");
                    System.Diagnostics.Debug.WriteLine($"LemmatizationService.Lemmatization.GetLemma() worked with error {DateTime.Now}{Environment.NewLine}" +
                    $"Lemms from string {Environment.NewLine}" +
                    $"{splitStr}{Environment.NewLine} equally null{ Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"LemmatizationService.Lemmatization.GetLemma() worked with error {DateTime.Now}" +
                    $"{Environment.NewLine}{ex}{Environment.NewLine}");
            }

            Log.Information($"LemmatizationService.Lemmatization.GetLemma() finished at {DateTime.Now}{Environment.NewLine}" +
                $"key={key}{Environment.NewLine}");
            System.Diagnostics.Debug.WriteLine($"LemmatizationService.Lemmatization.GetLemma() finished at {DateTime.Now}{Environment.NewLine} Lemms= {Lemms.Count}{Environment.NewLine}" +
                $"key={key}{Environment.NewLine}");
            return Lemms;
        }

        private string CleanText(string comment)
        {
            string cleanComment = "";
            try
            {
                string pattern1 = "[\t]+";
                string pattern2 = "[\n]+";
                string pattern3 = @"[^А-Яа-я\s]";
                string pattern4 = @"\s{2,}";
                string cleanComment1 = Regex.Replace(comment, pattern1, " ");
                string cleanComment2 = Regex.Replace(cleanComment1, pattern2, " ");
                string cleanComment3 = Regex.Replace(cleanComment2, pattern3, " ");
                cleanComment = Regex.Replace(cleanComment3, pattern4, " ");
            }
            catch (Exception ex)
            {
                Log.Error($"LemmatizationService.Lemmatization.CleanText() worked with error {DateTime.Now}" +
                    $"{Environment.NewLine}{ex}{Environment.NewLine}");
            }
            return cleanComment;
        }

    }
}

