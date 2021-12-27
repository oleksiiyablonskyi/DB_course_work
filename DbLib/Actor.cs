using System;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace progbase3
{
    [XmlType(TypeName="actor")]
    public class Actor
    {
        public long id;
        public string fullName;
        public int age;
        public string gender;
        public List<Movie> movies;

        public Actor(long id, string fullName, int age, string gender)
        {
            this.id = id;
            this.fullName = fullName;
            this.age = age;
            this.gender = gender;
        }
        public Actor()
        {
            
        }
        public Actor(string fullName, int age, string gender)
        {
            this.fullName = fullName;
            this.age = age;
            this.gender = gender;
        }
        public override string ToString()
        {
            return $"[{id}] {fullName}, {gender}, {age} y.o.";
        }
    }
}
