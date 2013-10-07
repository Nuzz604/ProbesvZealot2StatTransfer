using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NuzzProbesvZealot2Restorer
{
    public class BankFile
    {
        internal static string PathBase = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\StarCraft II\Accounts\";
        
        /// <summary>
        /// Array containing the parts of each path in the file, starting from the root folder of StarCraft II\Accounts
        /// </summary>
        private string[] PathParts { get; set; }

        public string Path { get; private set; }

        public string AccountHandle 
        {
            get
            {
                return PathParts[1];
            }
        }

        public DateTime ModDate 
        {
            get
            {
                return File.GetLastWriteTimeUtc(Path);
            }
        }

        public string AccountName { get; private set; }

        public MapVersion MapVersion { get; private set; }

        public string MapHandle
        {
            get
            {
                return PathParts[3];
            }
        }

        private string RelativePath
        {
            get { return Path.Substring(PathBase.Length); }
        }

        public string RawFile { get; set; }

        private dynamic Xml { get; set; }

        public BankFile(string path)
        {
            
            Path = path;
            PathParts = RelativePath.Split(System.IO.Path.DirectorySeparatorChar);
            
            if (PathParts[2] != "Banks")
                throw new InvalidOperationException("Invalid bankfile path: " + path);

            RawFile = File.ReadAllText(Path);
            Xml = (dynamic)DynamicXml.Parse(RawFile);

            AccountName = GetAccountName();
            MapVersion = GetMapVersion();
            
        }

        private string GetAccountName()
        {
            try
            {
                Func<dynamic, bool> l1 = x => x.name == "PlayerInfo";
                Func<dynamic, bool> l2 = x => x.name == "PlayerName";

                var section = new List<dynamic>(Xml.Section);
                section =  new List<dynamic>(section.Where(l1).Single().Key);
                var item = section.Where(l2).Single();
                var name = item.Value.text;
                return name;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private MapVersion GetMapVersion()
        {
            if (MapHandle == "1-S2-1-2970949")
                return NuzzProbesvZealot2Restorer.MapVersion.PvZ2KorvinOfficial;
            if (MapHandle == "1-S2-1-4130515")
                return NuzzProbesvZealot2Restorer.MapVersion.PvZ2CoastOfficial;
            return NuzzProbesvZealot2Restorer.MapVersion.Unknown;
        }
    }

    public enum MapVersion
    {
        Unknown,
        PvZ2KorvinOfficial,
        PvZ2CoastOfficial
    }
}
