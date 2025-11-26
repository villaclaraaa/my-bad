using Mybad.Core.DomainModels;
using Mybad.Storage.DB.Entities;

namespace Mybad.Storage.DB.Mappers
{
    internal static class HeroMatchupMapper
    {
        public static HeroMatchupEntity MapToEntity(this HeroMatchupModel model) =>
            new()
            {
                HeroId = model.HeroId,
                OtherHeroId = model.OtherHeroId,
                Wins = model.Wins,
                GamesPlayed = model.GamesPlayed,
            };

        public static HeroMatchupModel MapToModel(this HeroMatchupEntity entity) =>
            new()
            {
                HeroId = entity.HeroId,
                OtherHeroId = entity.OtherHeroId,
                Wins = entity.Wins,
                GamesPlayed = entity.GamesPlayed,
            };
    }
}
