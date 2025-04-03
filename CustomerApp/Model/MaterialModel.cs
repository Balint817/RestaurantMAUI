using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CustomerApp.Model
{
#pragma warning disable CS8618
    public class MaterialModel
    {
        public static MaterialModel Unknown { get; } = new()
        {
            _id = null!,
            name = "<name>",
            englishName = "<name>",
            unit = "<unit>",
            inStock = 0,
            isEnough = true,
            usageOneWeekAgo = 0,
        };
        public string _id { get; set; }
        public string name { get; set; }
        public string englishName { get; set; }
        public string unit { get; set; }
        public int inStock { get; set; }
        public bool isEnough { get; set; }
        public int usageOneWeekAgo { get; set; }

        public class Quantized
        {
            public static Quantized Unknown { get; } = new()
            {
                Material = MaterialModel.Unknown,
                Count = 0,
            };
            public MaterialModel Material { get; set; }
            public int Count { get; set; }
            public override string ToString()
            {
                return $"{Count} {Material.unit} {Material.name}";
            }
        }

        public Quantized AddCount(int count)
        {
            return new Quantized { Count = count, Material = this };
        }
    }
#pragma warning restore CS8618 
}
