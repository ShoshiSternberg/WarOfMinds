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
            //CreateMap<GameDTO, Game>().ReverseMap().ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players)); 
            //CreateMap<PlayerDTO, Player>().ReverseMap().ForMember(dest => dest.Games, opt => opt.MapFrom(src => src.Games));            
            CreateMap<SubjectDTO, Subject>().ReverseMap();


            CreateMap<Game, GameDTO>()
      .ForMember(d => d.Players, opt => opt.MapFrom(s => s.Players.Select(c => c.GPlayer)));
      //.ReverseMap()
      //.ForMember(d => d.Players, opt => opt.MapFrom(s => s.Players
      //    .Select(c => new GamePlayer { GameId = s.GameID, PlayerId = c.PlayerID })));
           
            
            CreateMap<Player, PlayerDTO>()
      .ForMember(d => d.Games, opt => opt.MapFrom(s => s.Games.Select(c => c.PGame)))
      .ReverseMap()
      .ForMember(d => d.Games, opt => opt.MapFrom(s => s.Games
          .Select(c => new GamePlayer { GameId = c.GameID, PlayerId = s.PlayerID})));




            CreateMap<GameDTO, Game>()
    .ForMember(dest => dest.Subject, opt => opt.Ignore())
    .ForMember(dest => dest.Players, opt => opt.Ignore())
    .AfterMap((src, dest, context) => {
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

        }
    }
}
