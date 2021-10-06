using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.Application.Documents
{
    public class Teacher
    {
        public Teacher(string shortname, string firstname, string lastname)
        {
            Shortname = shortname;
            Firstname = firstname;
            Lastname = lastname;
        }

        [BsonId]
        public string Shortname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? Email { get; set; }
    }
}
