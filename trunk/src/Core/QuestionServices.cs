using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace qInfer.Core
{
    public class QuestionServices
    {
        static XmlDocument _QTypesXmlDocument;

        public string GetQuestionType(string questionText)
        {
            if (_QTypesXmlDocument == null)
            {
                _QTypesXmlDocument = new XmlDocument();
                _QTypesXmlDocument.Load("QTypesDef.xml");
            }

            foreach (XmlNode typeNode in _QTypesXmlDocument.SelectNodes("/Q_TYPES/Type"))
            {
                var isMatch = Regex.Match(
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
