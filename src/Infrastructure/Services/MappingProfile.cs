using Application.Features.Exercises.Get;
using Application.Features.Workouts.GetAll;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Workout, WorkoutDto>();
            CreateMap<Exercise, ExerciseDto>();
        }
    }
}
