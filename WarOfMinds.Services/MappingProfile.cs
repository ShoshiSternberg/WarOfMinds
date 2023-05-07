using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarOfMinds.Common.DTO;
using WarOfMinds.Repositories.Entities;

namespace WarOfMinds.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<SubjectDTO, Subject>().ReverseMap();

            CreateMap<Player, PlayerDTO>()
                         .ForMember(d => d.Games, opt => opt.MapFrom(s => s.Games.Select(c => c.PGame)))
                         .ReverseMap()
                         .ForMember(d => d.Games, opt => opt.MapFrom(s => s.Games
                         .Select(c => new GamePlayer { GameId = c.GameID, PlayerId = s.PlayerID })));

            CreateMap<GameDTO, Game>()
            .ForMember(dest => dest.Subject, opt => opt.Ignore())
            .ForMember(dest => dest.Players, opt => opt.Ignore())
            .AfterMap((src, dest, context) =>
            {
                var subject = context.Mapper.Map<Subject>(src.Subject);
                dest.Subject = subject;

                var gamePlayers = src.Players.Select(p => new GamePlayer
                {
                    PlayerId = p.PlayerID,
                    GameId = dest.GameID,
                    GPlayer = context.Mapper.Map<Player>(p),
                    PGame = dest
                }); ;
                dest.Players = new HashSet<GamePlayer>(gamePlayers);
            });

            CreateMap<Game, GameDTO>()
            .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
            .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players.Select(gp => gp.GPlayer)))
            .AfterMap((src, dest, context) =>
            {
                var Players = src.Players.Select(gp => context.Mapper.Map<PlayerDTO>(gp.GPlayer)); 
                dest.Players = new HashSet<PlayerDTO>(Players);
            });
        }
    }
}
