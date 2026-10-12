using System;
using System.Data.Entity;
using System.Linq;

namespace WebApplication5.Models
{
    public class EventInitializer : CreateDatabaseIfNotExists<EventContext>
    {
        private static readonly string[] LegacySeedTitles =
        {
            "Технологии будущего",
            "Дизайн интерфейсов",
            "Вечер джаза",
            "Город и свет",
            "Научный форум",
            "Керамика: основы",
            "Симфонический вечер",
            "Архитектура сегодня",
            "Закрытая репетиция",
            "Новая конференция"
        };

        public override void InitializeDatabase(EventContext context)
        {
            base.InitializeDatabase(context);

            var existingEvents = context.Events.ToList();
            if (!existingEvents.Any())
            {
                SeedData(context);
                return;
            }

            if (existingEvents.Count != LegacySeedTitles.Length ||
                existingEvents.Any(item => !LegacySeedTitles.Contains(item.Title)))
                return;

            context.Events.RemoveRange(existingEvents);
            context.SaveChanges();
            context.Categories.RemoveRange(context.Categories.ToList());
            context.SaveChanges();
            SeedData(context);
        }

        protected override void Seed(EventContext context)
        {
            SeedData(context);
            base.Seed(context);
        }

        private static void SeedData(EventContext context)
        {
            var categoryNames = new[]
            {
                "Спектакли",
                "Выставки",
                "События",
                "Игры",
                "Концерты",
                "Мастер-классы"
            };

            foreach (var name in categoryNames)
            {
                if (!context.Categories.Any(category => category.Name == name))
                    context.Categories.Add(new Category { Name = name });
            }
            context.SaveChanges();

            var categories = context.Categories
                .Where(category => categoryNames.Contains(category.Name))
                .ToDictionary(category => category.Name, category => category.Id);

            context.Events.Add(new Event { Title = "Спектакль «Женитьба»", Description = "Гастрольная постановка комедии Гоголя о попытках героев устроить личную жизнь.", StartDate = new DateTime(2026, 10, 12, 14, 0, 0), Price = 500m, Seats = 150, IsPublished = true, CategoryId = categories["Спектакли"] });
            context.Events.Add(new Event { Title = "Выставка «Тюмень деревянная: история и современность»", Description = "Предметы быта, деревянные детали зданий и фотографии рассказывают о городской жизни начала XX века.", StartDate = new DateTime(2026, 10, 12, 10, 0, 0), Price = 120m, Seats = 80, IsPublished = true, CategoryId = categories["Выставки"] });
            context.Events.Add(new Event { Title = "Выставка «Увертюра. Врата Сибири»", Description = "Экспозиция об истории Тюменского края, природе, культуре и освоении Сибири.", StartDate = new DateTime(2026, 10, 12, 10, 0, 0), Price = 350m, Seats = 200, IsPublished = true, CategoryId = categories["Выставки"] });
            context.Events.Add(new Event { Title = "Медиапроект «Больше, чем блогер» о путешествиях по России", Description = "Обучающий курс о съёмке, текстах и рассказах о поездках с конкурсом публикаций.", StartDate = new DateTime(2026, 10, 12, 10, 0, 0), Price = 0m, Seats = 60, IsPublished = false, CategoryId = categories["События"] });
            context.Events.Add(new Event { Title = "Школьный семейный кинофестиваль", Description = "Кинопоказы, обсуждения фильмов, конкурс афиш и встречи в школе № 45.", StartDate = new DateTime(2026, 10, 12, 10, 0, 0), Price = 0m, Seats = 120, IsPublished = false, CategoryId = categories["События"] });
            context.Events.Add(new Event { Title = "Экскурсия «Бабушкин сундук»", Description = "Знакомство с бытом старой Тюмени через предметы интерьера, истории и старинные наряды.", StartDate = new DateTime(2026, 10, 12, 18, 0, 0), Price = 500m, Seats = 30, IsPublished = true, CategoryId = categories["Игры"] });
            context.Events.Add(new Event { Title = "Рок-квартирник «Эта музыка будет вечной»", Description = "Живые рок-хиты в камерном формате с музыкантами и поющими актёрами.", StartDate = new DateTime(2026, 10, 14, 20, 0, 0), Price = 1500m, Seats = 90, IsPublished = true, CategoryId = categories["Концерты"] });
            context.Events.Add(new Event { Title = "Экскурсия с мастер-классом по резьбе", Description = "Прогулка по центру деревянного зодчества с рассказом о резных деталях и созданием сувенира.", StartDate = new DateTime(2026, 10, 14, 11, 0, 0), Price = 700m, Seats = 30, IsPublished = true, CategoryId = categories["Мастер-классы"] });
            context.Events.Add(new Event { Title = "Интерактивная игра «Бабушкин сундук»", Description = "Семейная игра о повседневной жизни рубежа XIX–XX веков с предметами и костюмами той эпохи.", StartDate = new DateTime(2026, 10, 15, 18, 0, 0), Price = 500m, Seats = 30, IsPublished = true, CategoryId = categories["Игры"] });
            context.Events.Add(new Event { Title = "«Концерт за кулисами. Музыка Германии»", Description = "Камерная программа на сцене филармонии с произведениями немецких и австрийских композиторов.", StartDate = new DateTime(2026, 10, 16, 19, 0, 0), Price = 1500m, Seats = 80, IsPublished = true, CategoryId = categories["Концерты"] });
            context.Events.Add(new Event { Title = "Мастер-класс «Валенки-малютки»", Description = "Практическое занятие по мокрому валянию: участники сделают миниатюрный сувенир из шерсти.", StartDate = new DateTime(2026, 10, 16, 15, 0, 0), Price = 550m, Seats = 24, IsPublished = true, CategoryId = categories["Мастер-классы"] });

            context.SaveChanges();
        }
    }
}