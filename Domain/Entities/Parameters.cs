using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Parameters : BaseEntity
    {
        public string Key { get; private set; } = string.Empty;

        public string Value { get; private set; } = string.Empty;

        public Parameters()
        {

        }

        public Parameters(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public void ChangeValue(string value)
        {
            Value = value;
        }
    }
}
