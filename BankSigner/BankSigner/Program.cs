namespace BankSigner
{
    using BankSigner.Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml.Linq;
    public static class Program
    {
        public static AccountNumber AANumber;
        public static string BankData;
        public static string BankName;
        public static StringBuilder Buffer;
        public static string FilePath;
        public static AccountNumber UANumber;

        public static XElement Alphabetize(XElement el)
        {
            Comparison<XElement> comparison = new Comparison<XElement>(Program.Compare);
            List<XAttribute> content = new List<XAttribute>(el.Attributes());
            List<XElement> list2 = new List<XElement>(el.Elements());
            content.Sort((x, y) => string.CompareOrdinal(x.Name.ToString(), y.Name.ToString()));
            list2.Sort(comparison);
            XElement element = new XElement(el.Name);
            element.Add(content);
            element.Add(list2);
            el.RemoveAttributes();
            el.RemoveNodes();
            el.Add(new object[] { content, list2 });
            if (el.HasElements)
            {
                foreach (XElement element2 in el.Elements())
                {
                    Alphabetize(element2);
                }
            }
            return el;
        }

        private static int Compare(XElement x, XElement y)
        {
            int num = x.Name.ToString().CompareTo(y.Name.ToString());
            if (((num == 0) && x.HasAttributes) && y.HasAttributes)
            {
                XAttribute attribute = x.Attribute("name");
                XAttribute attribute2 = y.Attribute("name");
                if ((attribute != null) && (attribute2 != null))
                {
                    num = string.CompareOrdinal(attribute.Value, attribute2.Value);
                }
            }
            return num;
        }

        public static string Format(XElement el)
        {
            string str = string.Empty;
            if ((el.Name != "Section") && (el.Name != "Key"))
            {
                str = str + el.Name;
            }
            if (el.HasAttributes)
            {
                foreach (XAttribute attribute in el.Attributes())
                {
                    if (attribute.Name != "name")
                    {
                        str = str + attribute.Name;
                    }
                    if (attribute.Name != "text")
                    {
                        str = str + attribute.Value;
                    }
                }
            }
            if (el.HasElements)
            {
                foreach (XElement element in el.Elements())
                {
                    str = str + Format(element);
                }
            }
            return str;
        }

        public static string FormatHash(byte[] Input)
        {
            char[] chArray = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            string str = string.Empty;
            foreach (byte num in Input)
            {
                str = str + chArray[num >> 4] + chArray[num & 15];
            }
            return str;
        }

        [DllImport("BankSign.dll", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        public static extern void GetCurrentSig(string BankData, StringBuilder Destination, int DestinationSize);
        [DllImport("BankSign.dll", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        public static extern void GetNewSig(string AANumber, string UANumber, string BankName, string BankData, StringBuilder Destination, int DestinationSize);
        [DllImport("BankSign.dll", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        public static extern void GetNewSigFromString(string SigInputString, StringBuilder Destination, int DestinationSize);
        [DllImport("BankSign.dll", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        public static extern void GetSigInputString(string AANumber, string UANumber, string BankName, string BankData, StringBuilder Destination, int DestinationSize);
        [STAThread]
        private static void Main()
        {
            if (Settings.Default.FilePath.Length == 0)
            {
                Settings.Default.FilePath = Application.ExecutablePath;
                Settings.Default.Save();
            }
            AANumber = new AccountNumber();
            UANumber = new AccountNumber();
            AANumber.Number = Settings.Default.AAText;
            UANumber.Number = Settings.Default.UAText;
            BankName = Settings.Default.BNText;
            BankData = Settings.Default.BDText;
            FilePath = Settings.Default.FilePath;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void OpenFile(string FileName)
        {
            string[] strArray = FileName.Split(new char[] { '\\' });
            BankName = strArray[strArray.Length - 1];
            if (strArray.Length >= 4)
            {
                string[] strArray2 = strArray[strArray.Length - 2].Split(new char[] { '-' });
                if ((strArray2.Length == 4) && (strArray2[1] == "S2"))
                {
                    AANumber.Number = strArray[strArray.Length - 2];
                }
                strArray2 = strArray[strArray.Length - 4].Split(new char[] { '-' });
                if ((strArray2.Length == 4) && (strArray2[1] == "S2"))
                {
                    UANumber.Number = strArray[strArray.Length - 4];
                }
            }
            try
            {
                StreamReader reader = new StreamReader(FileName);
                BankData = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("There was an error opening or reading the bank file: \n" + exception.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        public static void SaveFile(string FileName)
        {
            try
            {
                StreamWriter writer = new StreamWriter(FileName, false);
                writer.Write(BankData);
                writer.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("There was an error writing or saving the bank file: \n" + exception.Message, "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        public static string Sign(string AANumber, string UANumber, string BankName, string BankData)
        {
            byte[] buffer;
            BankData = BankData.Replace("\r ", "\r\n ").Replace("\r<", "\r\n<");
            XDocument document = XDocument.Parse(BankData);
            XElement root = document.Root;
            root.SetElementValue("Signature", null);
            Alphabetize(root);
            string s = AANumber + UANumber + BankName;
            foreach (XElement element2 in root.Elements())
            {
                s = s + Format(element2);
            }
            using (SHA1 sha = SHA1.Create())
            {
                buffer = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
            }
            XElement content = new XElement("Signature", new XAttribute("value", FormatHash(buffer)));
            document.Root.SetElementValue("Signature", null);
            document.Root.Add(content);
            return (document.Declaration.ToString() + "\r\n" + document.ToString()).Replace("\" />", "\"/>").Replace("          <", "                    <").Replace("        <", "                <").Replace("      <", "            <").Replace("    <", "        <").Replace("  <", "    <");
        }

        [DllImport("BankSign.dll", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        public static extern void Sign(string AANumber, string UANumber, string BankName, string BankData, StringBuilder Destination, int DestinationSize);
        [DllImport("BankSign.dll", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        public static extern void SignFromString(string SigInputString, string BankData, StringBuilder Destination, int DestinationSize);
    }
}

