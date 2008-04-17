using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;
using qInfer.Core;

namespace qInfer.qInferConsole
{
	class qInfer
	{
		[STAThread]
		static void Main(string[] args)
		{
            string qFileName = "intrebari.txt";

            Dictionary<string,string> qDictionary = 
                new QuestionsParser { CsvFilePath = qFileName }.GetQuestions();

            XmlDocument resultXmlDocument = new XmlDocument();
            
            XmlNode parentNode = resultXmlDocument.AppendChild(
                    resultXmlDocument.CreateElement("QUESTIONS")
                    );
            foreach (string questionId in qDictionary.Keys)
            {

                parentNode.AppendChild(
                    CreateQuestionNode(resultXmlDocument, questionId, qDictionary[questionId]));
                
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create("result.xml",settings ))
            {
                resultXmlDocument.WriteContentTo(writer);
            }
		}

        static XmlNode CreateQuestionNode(XmlDocument xmlDoc,string questionId, string questionText)
        {
            QuestionServices.Service service = new QuestionServices.Service();
            string questionType =  service.GetQuestionType(questionText);
            
            XmlNode qElement = xmlDoc.CreateElement("q");
            
            XmlAttribute qTypeAttr = xmlDoc.CreateAttribute("q_type");
            qTypeAttr.Value = questionType;
            qElement.Attributes.Append(qTypeAttr);

            XmlAttribute idAttr = xmlDoc.CreateAttribute("id");
            idAttr.Value = questionId;
            qElement.Attributes.Append(idAttr);

            XmlNode questionElement = xmlDoc.CreateElement("question");
            questionElement.AppendChild(xmlDoc.CreateTextNode(questionText));
            qElement.AppendChild(questionElement);

            return qElement;
        }

        
	}
}

