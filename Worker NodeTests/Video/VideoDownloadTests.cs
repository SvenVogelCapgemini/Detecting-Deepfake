using NUnit.Framework;
using NUnit.Framework.Internal;
using System.IO;
using System.Threading.Tasks;

namespace Worker_Node.Video.Tests;

[TestFixture]
public class VideoDownloadTests
{
    string videopath = System.Environment.CurrentDirectory + @"\Videos\temp\-1.mp4";

    [TearDown]
    public void DeleteVideoAfterTest()
    {
        if (File.Exists(videopath))
        {
            File.Delete(videopath);
        }
    }

    [Test]
    public async Task TestYoutubeVideo()
    {
        await Video.GetVideo("https://www.youtube.com/watch?v=xcJtL7QggTI", -1);
        System.Console.WriteLine(videopath);
        Assert.IsTrue(File.Exists(videopath));
    }
}

