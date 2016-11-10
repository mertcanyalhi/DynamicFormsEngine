using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicFormsEngine
{
    public class EntityNotFoundException : Exception
    {
        private string _message;
        public override string Message
        {
            get
            {
                return _message;
            }
        }

        public EntityNotFoundException(string message)
        {
            _message = message;
        }
    }

    class DuplicateException : Exception
    {
        private string _message;
        public override string Message
        {
            get
            {
                return _message;
            }
        }

        public DuplicateException(string message)
        {
            _message = message;
        }
    }
}
