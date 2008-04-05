using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Reflection;
using System.IO;

namespace qInfer.qInferConsole
{
    public class CsvParser
    {
        public string CsvFilePath { private get; set; }

        public List<string> GetQuestions()
        {
            OleDbConnection connection = new OleDbConnection(
                string.Format(
                    @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=""{0}"";
                    Extended Properties=""text;HDR=No;FMT=Delimited"";",
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                    );
            OleDbCommand selCommand = new OleDbCommand(
                string.Format(@"select * from ""{0}""", CsvFilePath),
                connection);

            DataTable table = new DataTable();
            new OleDbDataAdapter(selCommand).Fill(table);

            List<string> qList = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                qList.Add(row[3].ToString());
            }
            return qList;
        }
    }
}
