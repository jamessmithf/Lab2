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
                var course = new Course { Students = new List<Student>() };

                foreach (XmlNode childNode in courseNode.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "Title":
                            course.Title = childNode.InnerText.Trim();
                            break;
                        case "Instructor":
                            var instructor = new Instructor();
                            foreach (XmlNode instructorNode in childNode.ChildNodes)
                            {
                                switch (instructorNode.Name)
                                {
                                    case "FullName":
                                        instructor.FullName = instructorNode.InnerText.Trim();
                                        break;
                                    case "Faculty":
                                        instructor.Faculty = instructorNode.InnerText.Trim();
                                        break;
                                    case "Department":
                                        instructor.Department = instructorNode.InnerText.Trim();
                                        break;
                                }
                            }
                            course.Instructor = instructor;
                            break;
                        case "Room":
                            course.Room = childNode.InnerText.Trim();
                            break;
                        case "ScheduleTime":
                            course.ScheduleTime = childNode.InnerText.Trim();
                            break;
                        case "Students":
                            foreach (XmlNode studentNode in childNode.ChildNodes)
                            {
                                if (studentNode.Name == "Student")
                                {
                                    var student = new Student();
                                    foreach (XmlNode studentDetailNode in studentNode.ChildNodes)
                                    {
                                        switch (studentDetailNode.Name)
                                        {
                                            case "FullName":
                                                student.FullName = studentDetailNode.InnerText.Trim();
                                                break;
                                            case "Group":
                                                student.Group = studentDetailNode.InnerText.Trim();
                                                break;
                                        }
                                    }
                                    // Додаємо лише, якщо студент містить дійсні дані
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
