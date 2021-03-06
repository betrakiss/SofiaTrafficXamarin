﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TramlineFive.DataAccess.Entities;

namespace TramlineFive.DataAccess.Domain
{
    public class FavouriteDomain
    {
        public string Name { get; set; }
        public string StopCode { get; set; }

        public FavouriteDomain(Favourite entity)
        {
            Name = entity.Name;
            StopCode = entity.StopCode;
        }

        public static async Task<FavouriteDomain> AddAsync(string name, string stopCode)
        {
            if ((await TramlineFiveContext.FindFavouriteAsync(stopCode)) != null)
                return null;

            Favourite added = new Favourite
            {
                Name = name,
                StopCode = stopCode
            };

            await TramlineFiveContext.AddAsync(added);
            return new FavouriteDomain(added);
        }

        public static async Task<IEnumerable<FavouriteDomain>> TakeAsync()
        {
            return (await TramlineFiveContext.TakeAll<Favourite>()).Select(f => new FavouriteDomain(f));
        }

        public static async Task RemoveAsync(string stopCode)
        {
            await TramlineFiveContext.RemoveFavouriteAsync(stopCode);
        }
    }
}
