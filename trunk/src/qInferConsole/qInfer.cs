using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;

namespace qInfer.qInferConsole
{
	class qInfer
	{
		[STAThread]
		static void Main(string[] args)
		{
            string qFileName = "intrebari.txt";

            List<string> qList = 
                new CsvParser { CsvFilePath = qFileName }.GetQuestions();

            XmlDocument resultXmlDocument = new XmlDocument();
            
            XmlNode parentNode = resultXmlDocument.AppendChild(
                    resultXmlDocument.CreateElement("QUESTIONS")
                    );
            foreach (string question in qList)
            {

                parentNode.AppendChild(CreateQuestionNode(resultXmlDocument, question));
                
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create("result.xml",settings ))
            {
                resultXmlDocument.WriteContentTo(writer);
            }
		}

        static XmlNode CreateQuestionNode(XmlDocument xmlDoc, string questionText)
        {
            string questionType = GetQuestionType(questionText);
            
            XmlNode qElement = xmlDoc.CreateElement("q");
            XmlAttribute qTypeAttr = xmlDoc.CreateAttribute("q_type");
            qTypeAttr.Value = questionType;
            qElement.Attributes.Append(qTypeAttr);

            XmlNode questionElement = xmlDoc.CreateElement("question");
            questionElement.AppendChild(xmlDoc.CreateTextNode(questionText));
            qElement.AppendChild(questionElement);

            return qElement;
        }

        static XmlDocument _QTypesXmlDocument;
        static string GetQuestionType(string questionText)
        {
            if (_QTypesXmlDocument == null)
            {
                _QTypesXmlDocument = new XmlDocument();
                _QTypesXmlDocument.Load("QTypesDef.xml");
            }

            foreach(XmlNode typeNode in _QTypesXmlDocument.SelectNodes("/Q_TYPES/Type"))
            {
                bool isMatch = Regex.Match(
                    questionText, 
                    typeNode.Attributes["Expression"].Value).Success;
                if (isMatch)
                {
                    return typeNode.Attributes["Id"].Value;
                }
            }

            return "INVALID";
        }
	}
}

