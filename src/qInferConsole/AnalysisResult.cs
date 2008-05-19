using System;
using System.Xml;
using qInfer.Core;
using System.Diagnostics;

namespace qInfer.qInferConsole
{
    public class AnalysisResult
    {
        public const string RESULT_FILE_NAME = "result.xml";

        #region WriteQuestions
        public void WriteQuestions(string qFileName)
        {
            var qDictionary =
                new QuestionsParser { CsvFilePath = qFileName }.GetQuestions();

            var resultXmlDocument = new XmlDocument();

            var parentNode = resultXmlDocument.AppendChild(
                    resultXmlDocument.CreateElement("QUESTIONS")
                    );
            foreach (var questionId in qDictionary.Keys)
            {

                parentNode.AppendChild(
                    CreateQuestionNode(resultXmlDocument, questionId, qDictionary[questionId]));

            }

            SaveResultXmlDocument(resultXmlDocument);
        }
        #endregion

        #region WriteQuestionsTypes
        public void WriteQuestionsTypes()
        {
            var resultDoc = new XmlDocument();
            resultDoc.Load(RESULT_FILE_NAME);

            try
            {
                foreach(XmlNode questionNode in resultDoc.SelectNodes("/QUESTIONS/q"))
                {
                    var questionText = questionNode.SelectSingleNode("question").InnerText;
                    var questionType = new QuestionServices().GetQuestionType(questionText);
                    questionNode.Attributes.Append(
                        GetAttribute(resultDoc, "q_type", questionType));
                        
                }
                SaveResultXmlDocument(resultDoc);
            }
            catch { }

        }
        #endregion

        #region WriteAnswersTypes
        public void WriteAnswersTypes()
        {
            var resultDoc = new XmlDocument();
            resultDoc.Load(RESULT_FILE_NAME);

            //Debugger.Launch();
            try
            {
                foreach (XmlNode questionNode in resultDoc.SelectNodes("/QUESTIONS/q"))
                {
                    var questionText = questionNode.SelectSingleNode("question").InnerText;
                    var answerType = new QuestionServices().GetAnswerType(questionText);
                    questionNode.Attributes.Append(
                        GetAttribute(resultDoc, "q_exp_ans_type", answerType));

                }
                SaveResultXmlDocument(resultDoc);
            }
            catch { }

        }
        #endregion

        #region WriteQuestionsAnswers
        public void WriteQuestionsAnswers()
        {
            var resultDoc = new XmlDocument();
            resultDoc.Load(RESULT_FILE_NAME);
            
            try
            {
                foreach (XmlNode questionNode in resultDoc.SelectNodes("/QUESTIONS/q"))
                {
                    var questionText = questionNode.SelectSingleNode("question").InnerText;
                    var questionId = questionNode.SelectSingleNode("@id").Value;

                    var answer = new QuestionServices().GetAnswerToQuestion(questionText);
                    questionNode.AppendChild(
                        CreateAnswerNode(resultDoc, answer, questionId));
                }
                SaveResultXmlDocument(resultDoc);
            }
            catch { }
        }
        #endregion

        #region CreateQuestionNode
        private XmlNode CreateQuestionNode(
            XmlDocument xmlDoc, 
            string questionId, string questionText)
        {
            XmlNode qElement = xmlDoc.CreateElement("q");

            qElement.Attributes.Append(GetAttribute(xmlDoc,"id",questionId));

            XmlNode questionElement = xmlDoc.CreateElement("question");
            questionElement.AppendChild(xmlDoc.CreateTextNode(questionText));
            qElement.AppendChild(questionElement);

            return qElement;
        }
        #endregion

        #region CreateAnswerNode
        private XmlNode CreateAnswerNode(XmlDocument xmlDoc, QuestionServices.Answer answer, string baseId)
        {
            var answerNode = xmlDoc.CreateElement("answer");

            answerNode.Attributes.Append(GetAttribute(xmlDoc,"a_id",baseId + "_a"));

            answerNode.Attributes.Append(GetAttribute(xmlDoc, "a_support",answer.SnippetFileName));

            var answerTextElement = xmlDoc.CreateElement("a_string");
            answerTextElement.AppendChild(xmlDoc.CreateTextNode(answer.AnswerText));
            answerNode.AppendChild(answerTextElement);

            answerNode.AppendChild(CreateSnippetElement(
                xmlDoc,
                baseId + "_s", 
                answer.SnippetFileName, 
                answer.SnippetText));

            return answerNode;
        }

        private XmlNode CreateSnippetElement(XmlDocument xmlDoc, 
                string snippetId,string snippetFile,string snippetText)
        {
            var snippetElement = xmlDoc.CreateElement("snippet");

            snippetElement.Attributes.Append(
                GetAttribute(xmlDoc, "s_id", snippetId));
            snippetElement.Attributes.Append(
                GetAttribute(xmlDoc, "s_support", snippetFile));
            snippetElement.AppendChild(
                xmlDoc.CreateTextNode(snippetText));

            return snippetElement;
        }
        #endregion

        #region SaveResultXmlDocument
        private void SaveResultXmlDocument(XmlDocument xmlDoc)
        {
            var settings = new XmlWriterSettings { OmitXmlDeclaration = false, Indent = true };
            using (var writer = XmlWriter.Create(RESULT_FILE_NAME, settings))
            {
                xmlDoc.WriteContentTo(writer);
            }
        }
        #endregion

        private XmlAttribute GetAttribute(XmlDocument xmlDoc, string attributeName, string attributeValue)
        {
            var attr = xmlDoc.CreateAttribute(attributeName);
            attr.Value = attributeValue;
            return attr;
        }
    }
}