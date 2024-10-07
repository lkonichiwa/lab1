using System;
using System.Collections.Generic;
using System.IO;

namespace lab1
{
    public struct GeneticData
    {
        public string name;
        public string organism;
        public string formula;
    }

    class Program
    {
        static List<GeneticData> data = new List<GeneticData>();
        static string GetFormula(string proteinName)
        {
            foreach (GeneticData item in data)
            {
                if (item.name.Equals(proteinName)) return item.formula;
            }
            return null;
        }
        static void ReadGeneticData(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] fragments = line.Split('\t');
                GeneticData protein;
                protein.name = fragments[0];
                protein.organism = fragments[1];
                protein.formula = fragments[2];
                data.Add(protein);
            }
            reader.Close();
        }
        static void ReadHandleCommands(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            StreamWriter sw = new StreamWriter("answer.txt");
            sw.WriteLine("Pavel Sadovskiy \nGenetic Searching");
            sw.WriteLine("================================================");
            int counter = 0;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine(); counter++;
                string[] command = line.Split('\t');
                if (command[0].Equals("search"))
                {
                    sw.WriteLine($"{counter.ToString("D3")}   {"search"}   {Decoding(command[1])}");
                    int index = Search(command[1]);
                    if (index != -1)
                    {
                        sw.WriteLine($"{data[index].organism}    {data[index].name}");
                    }
                    else
                    {
                        sw.WriteLine("NOT FOUND");
                    }
                    sw.WriteLine("================================================");
                }
                if (command[0].Equals("diff"))
                {
                    sw.WriteLine($"{counter.ToString("D3")}   {"diff"}   {command[1]} \t {command[2]}");
                    int cou = Diff(command[1], command[2]);
                    if (cou != -1)
                    {
                      sw.WriteLine($"{"amino - acids difference:"} \n {cou}");
                    }
                    else
                    {
                        sw.WriteLine("NOT FOUND");
                    }
                    sw.WriteLine("================================================");
                }
                if (command[0].Equals("mode"))
                {
                    sw.WriteLine($"{counter.ToString("D3")}   {"mode"}   {Decoding(command[1])}");
                    Dictionary<char, int> dict = Mode(command[1], sw);
                    char res = '0';
                    int maxCount = 0;
                    foreach (var kvp in dict)
                    {
                        if (kvp.Value > maxCount)
                        {
                            res = kvp.Key;
                            maxCount = kvp.Value;
                        }
                    }

                    if (res != '0')
                    {
                        sw.WriteLine($"amino - acids occurs: \n {res} \t {maxCount}");
                    }
                    else
                    {
                        sw.WriteLine("NOT FOUND");
                    }
                    sw.WriteLine("================================================");
                }
            }
            reader.Close();
            sw.Close();
        }
        static bool IsValid(string formula)
        {
            List<char> letters = new List<char>() { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };
            foreach (char ch in formula)
            {
                if (!letters.Contains(ch)) return false;
            }
            return true;
        }
        static string Encoding(string formula)
        {
            string encoded = String.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                char ch = formula[i];
                int count = 1;
                while (i < formula.Length - 1 && formula[i + 1] == ch)
                {
                    count++;
                    i++;
                }
                if (count > 2) encoded = encoded + count + ch;
                if (count == 1) encoded = encoded + ch;
                if (count == 2) encoded = encoded + ch + ch;

            }
            return encoded;

        }
        static string Decoding(string formula)
        {
            string decoded = String.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]))
                {
                    char letter = formula[i + 1];
                    int conversion = formula[i] - '0';
                    for (int j = 0; j < conversion - 1; j++) decoded = decoded + letter;
                }
                else decoded = decoded + formula[i];
            }
            return decoded;
        }
        static int Search(string amino_acid)
        {
            string decoded = Decoding(amino_acid);
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded)) return i;
            }
            return -1;
        }
        static int Diff(string protein1, string protein2)
        {
            string decoded1 = Decoding(GetFormula(protein1));
            string decoded2 = Decoding(GetFormula(protein2));
            if (decoded1 is null || decoded2 is null)
            {
                return -1;
            }
            int maxLength;
            if (decoded1.Length > decoded2.Length)
            {
                maxLength = decoded1.Length;
            }
            else
            {
                maxLength= decoded2.Length;
            }
            int counter = 0;
            for (int i = 0; i < maxLength; i++)
            {
                char char1 = i < decoded1.Length ? decoded1[i] : 'f';
                char char2 = i < decoded2.Length ? decoded2[i] : 'f';
                if (char1 != char2)
                {
                    counter++;
                }
            }

            return counter;
        }

        static Dictionary<char, int> Mode(string protein, StreamWriter sw)
        {
            string decoded = Decoding(GetFormula(protein));
            char[] arr = new char[20] { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' };
            Dictionary<char, int> dict = new Dictionary<char, int>();
            foreach (char aminoсids in arr)
            {
                dict[aminoсids] = 0;
            }
            foreach (char ch in decoded)
            {
                if (dict.ContainsKey(ch))
                {
                    dict[ch]++;
                }
            }
            return dict;
        }

        static void Main(string[] args)
        {   
            ReadGeneticData("sequences.0.txt");
            ReadHandleCommands("commands.0.txt");
            Console.WriteLine("Данные успешно загружены в файл");
        }
    }
}

