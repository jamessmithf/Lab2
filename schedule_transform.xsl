<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" encoding="UTF-8"/>
	<xsl:template match="/Schedules">
		<html>
			<head>
				<title>Schedule Table</title>
				<style>
					table { border-collapse: collapse; width: 100%; }
					th, td { border: 1px solid black; padding: 8px; text-align: left; }
					th { background-color: #f2f2f2; }
				</style>
			</head>
			<body>
				<h2>Schedule Table</h2>
				<table>
					<tr>
						<th>Day</th>
						<th>Course Title</th>
						<th>Room</th>
						<th>Schedule Time</th>
						<th>Instructor</th>
						<th>Faculty</th>
						<th>Department</th>
						<th>Student Name</th>
						<th>Group</th>
					</tr>
					<xsl:for-each select="Course">
						<xsl:variable name="day" select="@Day"/>
						<xsl:variable name="title" select="@Title"/>
						<xsl:variable name="room" select="@Room"/>
						<xsl:variable name="scheduleTime" select="@ScheduleTime"/>
						<xsl:variable name="instructorName" select="Instructor/FullName"/>
						<xsl:variable name="faculty" select="Instructor/@Faculty"/>
						<xsl:variable name="department" select="Instructor/@Department"/>
						<xsl:for-each select="Students/Student">
							<tr>
								<td>
									<xsl:value-of select="$day"/>
								</td>
								<td>
									<xsl:value-of select="$title"/>
								</td>
								<td>
									<xsl:value-of select="$room"/>
								</td>
								<td>
									<xsl:value-of select="$scheduleTime"/>
								</td>
								<td>
									<xsl:value-of select="$instructorName"/>
								</td>
								<td>
									<xsl:value-of select="$faculty"/>
								</td>
								<td>
									<xsl:value-of select="$department"/>
								</td>
								<td>
									<xsl:value-of select="FullName"/>
								</td>
								<td>
									<xsl:value-of select="@Group"/>
								</td>
							</tr>
						</xsl:for-each>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
