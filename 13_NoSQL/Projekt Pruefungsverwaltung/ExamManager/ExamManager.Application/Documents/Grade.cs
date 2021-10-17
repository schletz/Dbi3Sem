using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Documents
{
    /// <summary>
    /// Eingebettetes Objekt für Noten. Hat keine ID, da es vom Student verwaltet wird.
    /// </summary>
    public class Grade
    {
        public Grade(int value, string subject)
        {
            Value = value;
            Subject = subject;
            Updated = DateTime.UtcNow;
        }

        public int Value { get; set; }
        public string Subject { get; set; }
        public DateTime Updated { get; set; }
    }
}