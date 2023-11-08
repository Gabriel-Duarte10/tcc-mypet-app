using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace tcc_mypet_app.Services
{
    public class ApiService
    {
        private static readonly HttpClientHandler clientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        };
        private static readonly HttpClient client = new HttpClient(clientHandler);
        private const string BaseUrl = "https://192.168.1.169:3434/api/";
        //private const string BaseUrl = "https://projetomobileapi.azurewebsites.net/api/";

        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var response = await client.GetAsync(BaseUrl + url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                    return default(T);
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                throw;
            }
        }

        public async Task<TResult> PostAsync<TData, TResult>(string url, TData data)
        {
            try
            {
                if (data != null)
                {
                    var jsonData = JsonConvert.SerializeObject(data);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(BaseUrl + url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                        return default(TResult);
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TResult>(responseContent);
                }
                else
                {
                    var response = await client.PostAsync(BaseUrl + url, null);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                        return default(TResult);
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TResult>(responseContent);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                throw;
            }
        }

        public async Task<TResult> PutAsync<TData, TResult>(string url, TData data)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(BaseUrl + url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                    return default(TResult);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(responseContent);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                throw;
            }
        }


        public async Task<bool> DeleteAsync(string url)
        {
            try
            {
                var response = await client.DeleteAsync(BaseUrl + url);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                throw;
            }
        }
        public async Task<TResult> PostFormDataAsync<TResult>(string url, Dictionary<string, string> formData, List<Stream> streams)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            try
            {
                // Adiciona campos de formulário
                foreach (var field in formData)
                {
                    content.Add(new StringContent(field.Value), field.Key);
                }

                // Adiciona arquivos
                for (int i = 0; i < streams.Count; i++)
                {
                    var stream = streams[i];
                    stream.Position = 0;  // Certifica-se de que o stream está na posição inicial
                    var streamContent = new StreamContent(stream);
                    content.Add(streamContent, "Images", $"image{i}.jpg");  // Ajuste o nome do arquivo conforme necessário
                }

                var response = await client.PostAsync(BaseUrl + url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                    return default(TResult);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(responseContent);
                
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                throw;
            }
            finally
            {
                // Agora feche o conteúdo e todos os streams associados
                content.Dispose();
                foreach (var stream in streams)
                {
                    stream.Close();
                }
            }
        }
        public async Task<TResult> PutFormDataAsync<TResult>(string url, Dictionary<string, string> formData, List<Stream> streams)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            try
            {
                // Adiciona campos de formulário
                foreach (var field in formData)
                {
                    content.Add(new StringContent(field.Value), field.Key);
                }

                // Adiciona arquivos
                // Adiciona arquivos
                for (int i = 0; i < streams.Count; i++)
                {
                    var stream = streams[i];
                    stream.Position = 0;  // Certifica-se de que o stream está na posição inicial
                    var streamContent = new StreamContent(stream);
                    content.Add(streamContent, "Images", $"image{i}.jpg");  // Ajuste o nome do arquivo conforme necessário
                }

                var response = await client.PutAsync(BaseUrl + url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Error", error, "OK");
                    return default(TResult);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(responseContent);
                
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                throw;
            }
            finally
            {
                // Agora feche o conteúdo e todos os streams associados
                content.Dispose();
                foreach (var stream in streams)
                {
                    stream.Close();
                }
            }
        }
    }
}
