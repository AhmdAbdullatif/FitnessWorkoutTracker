using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Exercises.Get
{
    public class ExerciseDto
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public Instant CreatedAt { get; private set; }
    }
}
