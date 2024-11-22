using System.Text.RegularExpressions;

namespace WorkingWithXMLApplication.ParsingStrategy
{
    public static class ScheduleFilter
    {
        /// <summary>
        /// Розбиває вхідний рядок на слова та розділові знаки.
        /// </summary>
        private static string[] SplitIntoWords(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Array.Empty<string>();

            var regex = new Regex(@"[\w'-]+|[^\w\s]+");
            return regex.Matches(input).Select(m => m.Value).ToArray();
        }

        /// <summary>
        /// Перевіряє, чи є часткові збіги між значенням атрибуту та фільтром.
        /// </summary>
        private static bool IsSimilarParts(string? userFilter, string? attributesValue)
        {
            var filterParts = SplitIntoWords(userFilter?.ToLower());
            var attributeParts = SplitIntoWords(attributesValue?.ToLower());

            return filterParts.Any(filter =>
                attributeParts.Any(attribute => attribute.StartsWith(filter)));
        }

        /// <summary>
        /// Перевіряє, чи час відповідає вказаному інтервалу.
        /// </summary>
        private static bool MatchesTime(string? time, TimeOnly[] timeInterval)
        {
            if (string.IsNullOrWhiteSpace(time))
                return true;

            if (!TimeOnly.TryParse(time.Trim(), out var parsedTime))
                return false;

            return timeInterval[0] <= parsedTime && timeInterval[1] >= parsedTime;
        }

        /// <summary>
        /// Відфільтровує розклад за заданими критеріями.
        /// </summary>
        public static Schedule FilterSchedule(Schedule schedule,
            string? instructorName = null,
            string? time = null,
            string? courseTitle = null,
            string? room = null,
            string? day = null)
        {
            // Якщо критерії не задані, повертаємо початковий розклад
            if (new[] { instructorName, time, courseTitle, room, day }.All(string.IsNullOrWhiteSpace))
            {
                return schedule;
            }

            var filteredCourses = schedule.Courses.Where(course =>
                (string.IsNullOrWhiteSpace(instructorName) || IsSimilarParts(instructorName, course.Instructor.FullName)) &&
                MatchesTime(time, course.TimeInterval) &&
                (string.IsNullOrWhiteSpace(courseTitle) || IsSimilarParts(courseTitle, course.Title)) &&
                (string.IsNullOrWhiteSpace(room) || IsSimilarParts(room, course.Room)) &&
                (string.IsNullOrWhiteSpace(day) || IsSimilarParts(day, course.Day))
            ).ToList();

            return new Schedule { Courses = filteredCourses };
        }
    }
}
