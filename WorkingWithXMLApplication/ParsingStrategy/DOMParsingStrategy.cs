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
                var course = ParseCourseNode(courseNode);
                if (course != null)
                    schedule.Courses.Add(course);
            }

            return schedule;
        }

        private static Course? ParseCourseNode(XmlNode courseNode)
        {
            if (courseNode.Attributes == null) return null;

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
                        course.Instructor = ParseInstructorNode(childNode);
                        break;

                    case "Students":
                        ParseStudentsNode(childNode, course.Students);
                        break;
                }
            }

            return course;
        }

        private static Instructor? ParseInstructorNode(XmlNode instructorNode)
        {
            if (instructorNode.Attributes == null) return null;

            var instructor = new Instructor
            {
                Faculty = instructorNode.Attributes["Faculty"]?.Value.Trim(),
                Department = instructorNode.Attributes["Department"]?.Value.Trim()
            };

            foreach (XmlNode detailNode in instructorNode.ChildNodes)
            {
                if (detailNode.Name == "FullName")
                    instructor.FullName = detailNode.InnerText.Trim();
            }

            return instructor;
        }

        private static void ParseStudentsNode(XmlNode studentsNode, List<Student> students)
        {
            foreach (XmlNode studentNode in studentsNode.ChildNodes)
            {
                if (studentNode.Name != "Student" || studentNode.Attributes == null) continue;

                var student = new Student
                {
                    Group = studentNode.Attributes["Group"]?.Value.Trim()
                };

                foreach (XmlNode detailNode in studentNode.ChildNodes)
                {
                    if (detailNode.Name == "FullName")
                        student.FullName = detailNode.InnerText.Trim();
                }

                if (!string.IsNullOrWhiteSpace(student.FullName) && !string.IsNullOrWhiteSpace(student.Group))
                {
                    students.Add(student);
                }
            }
        }
    }
}
