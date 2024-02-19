using System;

namespace JRiver
{
    public class JAutomationEvent
    {
        public string type { get; private set; }
        public string param1 { get; private set; }
        public string param2 { get; private set; }

        internal JAutomationEvent(string type, string arg1, string arg2)
        {
            this.type = type;
            param1 = arg1;
            param2 = arg2;
        }
    }
}
