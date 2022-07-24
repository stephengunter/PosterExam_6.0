using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Exceptions
{
    public class ExamQuestionDuplicated : Exception
    {
        public ExamQuestionDuplicated(string message = "") : base(message)
        {

        }
    }


    public class ExamNotRecruitQuestionSelected : Exception
    {
        
    }
}
