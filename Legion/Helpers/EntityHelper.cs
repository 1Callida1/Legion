using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legion.Helpers
{
    public static class EntityHelper
    {
        public static T DetachEntity<T>(T entity, ApplicationDbContext db) where T : class
        {
            db.Entry(entity).State = EntityState.Detached;
            if (entity.GetType().GetProperty("Id") != null)
            {
                entity.GetType().GetProperty("Id")?.SetValue(entity, 0);
            }
            return entity;
        }

        public static List<T> DetachEntities<T>(List<T> entities, ApplicationDbContext db) where T : class
        {
            foreach (var entity in entities)
            {
                DetachEntity(entity, db);
            }
            return entities;
        }
    }
}
