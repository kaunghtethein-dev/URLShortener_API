using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Application.CustomExceptions
{
    public class CustomExceptions
    {
        public class AlreadyExistsException : Exception
        {
            public string EntityName { get; }
            public string PropertyName { get; }
            public object PropertyValue { get; }

            public AlreadyExistsException(string entityName, string propertyName, object propertyValue)
                : base($"{entityName} with {propertyName} '{propertyValue}' already exists.")
            {
                EntityName = entityName;
                PropertyName = propertyName;
                PropertyValue = propertyValue;
            }

            public AlreadyExistsException(string message) : base(message) { }
            public AlreadyExistsException(string message, Exception inner) : base(message, inner) { }
        }
    }
}
