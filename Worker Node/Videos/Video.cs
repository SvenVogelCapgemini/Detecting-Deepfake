namespace Worker_Node;

internal class Video
{
    public async Task<string> GetVideo(string url, string id)
    {
        try
        {
            var filepath = $"{Environment.CurrentDirectory}\\Videos\\temp";
            Directory.CreateDirectory(filepath);
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStreamAsync();
            var ms = new MemoryStream();
            data.CopyTo(ms);
            File.WriteAllBytes($"{filepath}\\{id}.mp4", ms.ToArray());
            return $"{filepath}\\{id}.mp4";
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<bool> DeleteVideo(string id)
    {
        try
        {
            File.Delete(Environment.CurrentDirectory + $"\\videos\\temp\\{id}.mp4");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}