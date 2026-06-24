using Google.Cloud.Vision.V1;

namespace Licenta.Services
{
    public class OCRService
    {
        public async Task<string> ReadTextAsync(string imagePath)
        {
            var client = await ImageAnnotatorClient.CreateAsync();
            var image = Image.FromFile(imagePath);
            var response = await client.DetectTextAsync(image);

            if (response.Count == 0)
                return "";

            return response[0].Description;
        }
    }
}