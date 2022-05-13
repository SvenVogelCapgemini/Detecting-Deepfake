using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Worker_Node.Python.Tests;

[TestFixture]
public class PythonScriptsTests
{
    private PythonScripts _scripts = new PythonScripts();


    [Test]
    public void PythonReadRandomArgs(
        [Random(0, 99, 10)] int args)
    {
        var random = Randomizer.CreateRandomizer();
        var input = random.GetString(args);
        var output = _scripts.Run(PythonScripts.ScriptType.test, input);
        Assert.AreEqual(input, output);
    }
}