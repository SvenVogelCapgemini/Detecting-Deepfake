using System.Xml.Serialization;

namespace Worker_Node.Settings;

internal class SettingsSetup
{
    private static readonly object locker = new();
    private static SettingsSetup instance;

    private static readonly string _settingsDir = Environment.CurrentDirectory + "\\settings.xml";

    public Setting Setting;

    protected SettingsSetup()
    {
        // if the settings file exists read it otherwise create new settings
        if (File.Exists(_settingsDir))
            ReadSettings();
        else
            CreateSettings();
    }

    /// <summary>
    ///     Reads the settings file and saves it in the variable Settings
    /// </summary>
    public void ReadSettings()
    {
        var reader =
            new XmlSerializer(typeof(Setting));
        var file = new StreamReader(_settingsDir);
        var settings = (Setting)reader.Deserialize(file);
        file.Close();
        Setting = settings;
    }

    /// <summary>
    ///     Creates new settings with the option to save it to a file
    /// </summary>
    private void CreateSettings()
    {
        Setting = new Setting();
        Console.Clear();
        Console.WriteLine("What is the connection string of the Hub?");
        Setting.HubIp = Console.ReadLine().Trim();
        var incorrect = true;
        while (incorrect)
        {
            Console.WriteLine("Would you like to save these settings? y/n");
            var key = Console.ReadLine().Trim().ToLower();
            if (!key.Equals("y") && !key.Equals("n"))
                continue;
            incorrect = false;
            if (!key.Equals("y")) continue;
            WriteSettings();
            Console.WriteLine("Saved");
        }
    }

    /// <summary>
    ///     Writes the settings class to a file
    /// </summary>
    public void WriteSettings()
    {
        var writer =
            new XmlSerializer(typeof(Setting));

        var path = _settingsDir;
        var file = File.Create(path);

        writer.Serialize(file, Setting);
        file.Close();
    }

    public static SettingsSetup Instance()
    {
        if (instance == null)
            lock (locker)
            {
                if (instance == null) instance = new SettingsSetup();
            }

        return instance;
    }
}