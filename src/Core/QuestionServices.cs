using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace qInfer.Core
{
    public class QuestionServices
    {
        #region GetQuestionType
        static XmlDocument _QTypesXmlDocument;
        static XmlDocument QTypesXmlDocument
        {
            get
            {
                if (_QTypesXmlDocument == null)
                {
                    _QTypesXmlDocument = new XmlDocument();
                    _QTypesXmlDocument.Load("QTypesDef.xml");
                }
                return _QTypesXmlDocument;
            }
        }

        public struct QuestionType
        {
            public string Type;
            public List<string> AnswerPatterns;
            public List<string> WordsFound;
        }

        public QuestionType GetQuestionType(string questionText)
        {
           
            try
            {
                foreach (XmlNode typeNode in QTypesXmlDocument.SelectNodes("/Q_TYPES/Type"))
                {
                    var expression = typeNode.SelectSingleNode("Expression").InnerText;
                    var match = Regex.Match(questionText,expression);
                    
                    if (match.Success)
                    {
                        var words = new List<string>();
                        for (int i = 1; i < match.Groups.Count; i++ )
                        {
                            words.Add(match.Groups[i].Value);
                        }

                        var answerPatterns = new List<string>();
                        var patterns = typeNode.SelectNodes("AnswerPatterns/Answer");
                        if (patterns!= null)
                        {
                            foreach (XmlNode patternNode in patterns)
                            {
                                var patternString = String.Format(CultureInfo.InvariantCulture,
                                    patternNode.InnerText,
                                    words.ToArray());
                                answerPatterns.Add(patternString);
                            }
                        }

                        return new QuestionType
                                   {
                                       Type = typeNode.Attributes["Id"].Value,
                                       AnswerPatterns = answerPatterns,
                                       WordsFound = words
                                   };
                        }
                    }
            }
            catch { }

            return new QuestionType
                       {
                           Type = "INVALID", 
                           AnswerPatterns = new List<string>(), 
                           WordsFound = new List<string>()
                       };
        }
        #endregion

        #region GetAnswerType
        static XmlDocument _ATypesXmlDocument;
        static XmlDocument ATypesXmlDocument
        {
            get
            {
                if (_ATypesXmlDocument == null)
                {
                    _ATypesXmlDocument = new XmlDocument();
                    _ATypesXmlDocument.Load("ATypesDef.xml");
                }
                return _ATypesXmlDocument;
            }
        }
        public string GetAnswerType(string questionText)
        {
           
            try
            {
                foreach (XmlNode typeNode in ATypesXmlDocument.SelectNodes("/A_TYPES/Type"))
                {
                    var expression = typeNode.InnerText;
                    var isMatch = Regex.Match(questionText,expression).Success;
                    if (isMatch)
                    {
                        return typeNode.Attributes["Id"].Value;
                    }
                }
            }
            catch { }

            return "INVALID";
        }
        #endregion

        #region GetAnswerToQuestion
        public struct Answer
        {
            public string AnswerText;

            public string SnippetFileName;
            public string SnippetText;
        }

        public Answer GetAnswerToQuestion(string wikiFilesFolder,string questionId,List<string> patternsList)
        {
            var answer = new Answer();

            var filesPath = Path.Combine(wikiFilesFolder, questionId);
            var dirInfo = new DirectoryInfo(filesPath);
            
            if(dirInfo.Exists)
            {
                foreach(var file in dirInfo.GetFiles())
                {
                    using(var reader = file.OpenText())
                    {
                        var textLine = reader.ReadLine();
                        while (textLine != null)
                        {
                            foreach (var pattern in patternsList)
                            {
                                var match = Regex.Match(textLine, pattern);
                                if (match.Success)
                                {
                                    answer.SnippetText = match.Groups[0].Value;
                                    answer.AnswerText = match.Groups["answer"].Value;

                                    string snippetFileName = "";
                                    try
                                    {
                                        snippetFileName = textLine.Substring(0, textLine.IndexOf(':', 5));
                                    }
                                    catch
                                    {
                                    }
                                    answer.SnippetFileName = snippetFileName;
                                    return answer;
                                }
                            }

                            textLine = reader.ReadLine();
                        }
                    }
                    
                }
            }

            return answer;
        }
        #endregion

    }
}
