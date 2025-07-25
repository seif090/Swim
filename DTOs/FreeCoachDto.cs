﻿namespace SwimmingAcademy.DTOs
{
    /// <summary>
    /// Represents a coach who is currently available (not assigned to a school or team).
    /// </summary>
    public class FreeCoachDto
    {
        public int CoachId { get; set; }
        public string Name { get; set; }= string.Empty;

    }
}
