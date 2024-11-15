using System.Xml.Linq;

namespace WorkingWithXMLApplication.ParsingStrategy
{
    public class LINQParsingStrategy : IParsingStrategy
    {
        public Schedule Parse(string selectedFilePath)
        {
            XDocument xml = XDocument.Load(selectedFilePath);
            var schedule = new Schedule { Courses = new List<Course>() };

            foreach (var courseElement in xml.Descendants("Course"))
            {
                var course = new Course
                {
                    Day = (string)courseElement.Attribute("Day"),
                    Title = (string)courseElement.Attribute("Title"),
                    Room = (string)courseElement.Attribute("Room"),
                    ScheduleTime = (string)courseElement.Attribute("ScheduleTime"),
                    Instructor = new Instructor
                    {
                        FullName = (string)courseElement.Element("Instructor")?.Element("FullName"),
                        Faculty = (string)courseElement.Element("Instructor")?.Attribute("Faculty"),
                        Department = (string)courseElement.Element("Instructor")?.Attribute("Department")
                    },
                    Students = courseElement.Descendants("Student").Select(s => new Student
                    {
                        FullName = (string)s.Element("FullName"),
                        Group = (string)s.Attribute("Group")
                    }).ToList()
                };
                schedule.Courses.Add(course);
            }

            return schedule;
        }
    }
}
