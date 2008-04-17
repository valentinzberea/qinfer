using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace qInfer.Core
{
    public class QuestionsParser
    {
        public string CsvFilePath { private get; set; }

        private const int QUESTION_ID_INDEX = 2;
        private const int QUESTION_TEXT_INDEX = 3;

        public Dictionary<string,string> GetQuestions()
        {
            Dictionary<string, string> qDictionary = new Dictionary<string, string>();

            string[] lines = File.ReadAllLines(CsvFilePath, Encoding.UTF7);

            foreach (string questionLine in lines)
            { 
                string[] tokens = questionLine.Split('\t');
                if(QUESTION_ID_INDEX<tokens.Length & QUESTION_TEXT_INDEX<tokens.Length)
                {
                    string id = tokens[QUESTION_ID_INDEX];
                    string text = tokens[QUESTION_TEXT_INDEX];
                    qDictionary.Add(id, text);
                }
            }

            return qDictionary;
        }
    }
}
