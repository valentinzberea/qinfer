using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Xml;
using System.Text.RegularExpressions;
using System.Web.Configuration;

namespace QuestionServices
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class Service : System.Web.Services.WebService
    {

        static XmlDocument _QTypesXmlDocument;

        [WebMethod]
        public string GetQuestionType(string questionText)
        {
            if (_QTypesXmlDocument == null)
            {
                _QTypesXmlDocument = new XmlDocument();
                _QTypesXmlDocument.Load(WebConfigurationManager.AppSettings["QuestionTypesFile"]);
            }

            foreach (XmlNode typeNode in _QTypesXmlDocument.SelectNodes("/Q_TYPES/Type"))
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
