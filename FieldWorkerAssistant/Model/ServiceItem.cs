using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldWorkerAssistant.Model
{
    public class ServiceItem
    {
        public int ServiceRequestID { get; set; }
        public string AssignedTo { get; set; }
        public string Type { get; set; }
        public string ProblemDescription { get; set; }
        public string Severity { get; set; }
        public string DateEntered { get; set; }
        public string DateResolved { get; set; }
        public string Status { get; set; }
        public string ActionTaken { get; set; }
    }
}
