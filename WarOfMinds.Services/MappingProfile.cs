using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;

namespace WarOfMinds.Services
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<GameDTO, Game>().ReverseMap();
            CreateMap<PlayerDTO, Player>().ReverseMap();
            CreateMap<PlayerRatingBySubjectDTO, PlayerRatingBySubject>().ReverseMap();
            CreateMap<SubjectDTO, Subject>().ReverseMap();
        }
    }
}
