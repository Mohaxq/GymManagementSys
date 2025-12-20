using GymManagementDAL.Data.Context;
using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GymManagementDAL.Data.DataSeeding
{
    public class GymDbContextSeeding
    {
        public static bool SeedData(GymDbContext dbContext, string contentRootPath)
        {
            try
            {
                bool HasCategories = dbContext.Categories.Any();
                bool HasPlans = dbContext.Plans.Any();
                if (HasCategories && HasPlans) return false;

                if (!HasCategories)
                {
                    var Categories = LoadDataFromJsonFile<Category>("categories.json");
                    dbContext.Categories.AddRange(Categories);
                }

                if (!HasPlans)
                {
                    var Planss = LoadDataFromJsonFile<Plan>("plans.json");
                    dbContext.Plans.AddRange(Planss);
                }

                int RowsAffected = dbContext.SaveChanges();
                return RowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding Failed : {ex}");
                return false;
            }
        }

        private static List<T> LoadDataFromJsonFile<T>(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", fileName);

            if (!File.Exists(filePath)) throw new FileNotFoundException();

            string Data = File.ReadAllText(filePath);
            var Options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            Options.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializer.Deserialize<List<T>>(Data, Options) ?? new List<T>();


        }
    }
}
