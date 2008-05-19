using System;
using System.Xml;


namespace qInfer.qInferConsole
{
	class qInfer
	{
        enum OPERATION
        {
            WRITE_QUESTION_TYPES = 0,
            WRITE_ANSWERS_TYPES = 1,
            WRITE_ANSWERS = 2
        }

		[STAThread]
		static void Main(string[] args)
		{
            if(args.Length == 0)
            {
                Console.WriteLine("One parameter is required:");
                Console.WriteLine("0 - write questions types");
                Console.WriteLine("1 - write questions answers");
                return;
            }

            const string qFileName = "intrebari.txt";

		    var operationType = (OPERATION) Enum.Parse(typeof (OPERATION), args[0]);
		    switch(operationType)
            {
                case OPERATION.WRITE_QUESTION_TYPES:
                    var resultOperations = new AnalysisResult();
                    resultOperations.WriteQuestions(qFileName);
                    resultOperations.WriteQuestionsTypes();
                    break;
                case OPERATION.WRITE_ANSWERS_TYPES:
                    new AnalysisResult().WriteAnswersTypes();
                    break;
                case OPERATION.WRITE_ANSWERS:
                    new AnalysisResult().WriteQuestionsAnswers();
                    break;
            }
		    
		}

        

        
	}
}

