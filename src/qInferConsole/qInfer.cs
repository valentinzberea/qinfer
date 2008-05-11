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
            const string qFileName = "intrebari.txt";

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

            var settings = new XmlWriterSettings {OmitXmlDeclaration = false, Indent = true};
		    using (var writer = XmlWriter.Create("result.xml",settings ))
            {
                resultXmlDocument.WriteContentTo(writer);
            }
		}

        static XmlNode CreateQuestionNode(XmlDocument xmlDoc,string questionId, string questionText)
        {
            var service = new QuestionServices();
            var questionType =  service.GetQuestionType(questionText);
            
            XmlNode qElement = xmlDoc.CreateElement("q");
            
            var qTypeAttr = xmlDoc.CreateAttribute("q_type");
            qTypeAttr.Value = questionType;
            qElement.Attributes.Append(qTypeAttr);

            var idAttr = xmlDoc.CreateAttribute("id");
            idAttr.Value = questionId;
            qElement.Attributes.Append(idAttr);

            XmlNode questionElement = xmlDoc.CreateElement("question");
            questionElement.AppendChild(xmlDoc.CreateTextNode(questionText));
            qElement.AppendChild(questionElement);

            return qElement;
        }

        
	}
}

