﻿using System.Text.Json.Serialization;
using Gazillion;

namespace MHServerEmu.Billing.Catalogs
{
    public class CatalogEntryType
    {
        public string Name { get; set; }
        public int Order { get; set; }

        [JsonConstructor]
        public CatalogEntryType(string name, int order)
        {
            Name = name;
            Order = order;
        }

        public CatalogEntryType(MHCatalogEntryType catalogEntryType)
        {
            Name = catalogEntryType.Name;
            Order = catalogEntryType.Order;
        }

        public MHCatalogEntryType ToNetStruct()
        {
            return MHCatalogEntryType.CreateBuilder()
                .SetName(Name)
                .SetOrder(Order)
                .Build();
        }
    }
}
