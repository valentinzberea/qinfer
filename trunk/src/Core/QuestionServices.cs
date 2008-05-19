using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

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

        public string GetQuestionType(string questionText)
        {
            
            try
            {
                foreach (XmlNode typeNode in QTypesXmlDocument.SelectNodes("/Q_TYPES/Type"))
                {
                    var isMatch = Regex.Match(
                        questionText,
                        typeNode.Attributes["Expression"].Value).Success;
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
                    var isMatch = Regex.Match(
                        questionText,
                        typeNode.Attributes["Expression"].Value).Success;
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

        public Answer GetAnswerToQuestion(string questionText)
        {
            var answer = new Answer();



            return answer;
        }
        #endregion

    }
}
