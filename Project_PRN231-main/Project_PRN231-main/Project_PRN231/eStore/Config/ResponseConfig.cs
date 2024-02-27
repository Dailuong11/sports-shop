using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace eStore.Config
{
    public class ResponseConfig
    {
        private static readonly string BaseUrl = "https://localhost:7177/";

        public static async Task<HttpResponseMessage> GetData(string targetAddress, string? cookies)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(cookies))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cookies);
            }
            var Response = await client.GetAsync(targetAddress);
            return Response;
        }

        public static async Task<HttpResponseMessage> PostData(string targetAddress, string content, string? cookies)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(cookies))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cookies);
            }
            var Response = await client.PostAsync(targetAddress, new StringContent(content, Encoding.UTF8, "application/json"));
            return Response;
        }

        public static async Task<HttpResponseMessage> UploadData(string targerAddress, byte[] bytes, string fileName, string? cookies)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var multiPartFromDataContent = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(bytes);
            multiPartFromDataContent.Add(fileContent, "file", fileName);
            if (!string.IsNullOrEmpty(cookies))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cookies);
            }
            var Response = await client.PostAsync(targerAddress, multiPartFromDataContent);
            return Response;
        }

        public static async Task<HttpResponseMessage> PutData(string targetAddress, string content, string? cookies)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(cookies))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cookies);
            }
            var Response = await client.PutAsync(targetAddress, new StringContent(content, Encoding.UTF8, "application/json"));
            return Response;
        }

        public static async Task<HttpResponseMessage> DeleteData(string targetAddress, string? cookies)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(cookies))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cookies);
            }
            var Response = await client.DeleteAsync(targetAddress);
            return Response;
        }
    }
}
