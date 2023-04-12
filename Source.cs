using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DownloadPlatformTools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string installPath = "C:\\platform-tools";

            // Apagar a pasta de destino, se ela existir
            if (Directory.Exists(installPath))
            {
                Console.WriteLine($"Removendo pasta existente em {installPath}...");
                Directory.Delete(installPath, true);
            }

            // Fazer o download da página de destino
            string url = "https://androidsdkmanager.azurewebsites.net/Platformtools";
            string pageContent = await DownloadPageContent(url);

            // Extrair o link para a versão mais recente do Windows do Platform Tools
            string linkPattern = @"https://dl.google.com/android/repository/platform-tools_r\d+.\d+.\d+-windows.zip";
            string platformToolsLink = ExtractLink(pageContent, linkPattern);

            // Fazer o download do arquivo ZIP
            string downloadPath = "C:\\platform-tools.zip";
            await DownloadFile(platformToolsLink, downloadPath);

            // Extrair o arquivo ZIP para a pasta de destino
            string extractPath = "C:\\";
            ExtractZip(downloadPath, extractPath);

            // Limpar o arquivo ZIP
            File.Delete(downloadPath);

            Console.WriteLine($"Platform Tools instalado em {installPath}");

            for (int i = 5; i > 0; i--)
            {
                Console.WriteLine($"Fechando em {i} segundos...");
                await Task.Delay(1000);
            }

            Console.WriteLine("Fechando...");
        }

        static async Task<string> DownloadPageContent(string url)
        {
            string pageContent = "";

            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(url);
                pageContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return pageContent;
        }

        static string ExtractLink(string pageContent, string linkPattern)
        {
            string link = "";

            try
            {
                Match match = Regex.Match(pageContent, linkPattern);
                link = match.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return link;
        }

        static async Task DownloadFile(string fileUrl, string downloadPath)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(fileUrl);
                byte[] content = await response.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(downloadPath, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ExtractZip(string zipPath, string extractPath)
        {
            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
