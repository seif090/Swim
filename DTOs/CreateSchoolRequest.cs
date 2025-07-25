﻿namespace SwimmingAcademy.DTOs
{
    /// <summary>
    /// Request DTO for creating a new school.
    /// </summary>
    public class CreateSchoolRequest
    {
        public short SchoolLevel { get; set; }
        public int CoachID { get; set; }
        public string? FirstDay { get; set; }
        public string? SecondDay { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public short Type { get; set; }
        public short Site { get; set; }
        public int UserId { get; set; }
    }
}
