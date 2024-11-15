using System.Xml;

namespace WorkingWithXMLApplication.ParsingStrategy
{
    public class DOMParsingStrategy : IParsingStrategy
    {
        public Schedule Parse(string selectedFilePath)
        {
            var schedule = new Schedule { Courses = new List<Course>() };
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(selectedFilePath);

            XmlNodeList courseNodes = xmlDoc.GetElementsByTagName("Course");
            foreach (XmlNode courseNode in courseNodes)
            {
                var course = new Course
                {
                    Students = new List<Student>(),
                    Day = courseNode.Attributes["Day"]?.Value.Trim(),
                    Title = courseNode.Attributes["Title"]?.Value.Trim(),
                    Room = courseNode.Attributes["Room"]?.Value.Trim(),
                    ScheduleTime = courseNode.Attributes["ScheduleTime"]?.Value.Trim()
                };

                foreach (XmlNode childNode in courseNode.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "Instructor":
                            var instructor = new Instructor
                            {
                                Faculty = childNode.Attributes["Faculty"]?.Value.Trim(),
                                Department = childNode.Attributes["Department"]?.Value.Trim()
                            };

                            foreach (XmlNode instructorDetail in childNode.ChildNodes)
                            {
                                if (instructorDetail.Name == "FullName")
                                    instructor.FullName = instructorDetail.InnerText.Trim();
                            }
                            course.Instructor = instructor;
                            break;

                        case "Students":
                            foreach (XmlNode studentNode in childNode.ChildNodes)
                            {
                                if (studentNode.Name == "Student")
                                {
                                    var student = new Student
                                    {
                                        Group = studentNode.Attributes["Group"]?.Value.Trim()
                                    };

                                    foreach (XmlNode studentDetail in studentNode.ChildNodes)
                                    {
                                        if (studentDetail.Name == "FullName")
                                            student.FullName = studentDetail.InnerText.Trim();
                                    }

                                    if (!string.IsNullOrWhiteSpace(student.FullName) && !string.IsNullOrWhiteSpace(student.Group))
                                    {
                                        course.Students.Add(student);
                                    }
                                }
                            }
                            break;
                    }
                }

                schedule.Courses.Add(course);
            }

            return schedule;
        }
    }
}
