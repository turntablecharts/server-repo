using System;
using CsvHelper.Configuration;

namespace Presentation.ViewModels
{
    public class SubscribersList
    {
        public string EmailAddress { get; set; }
    }

    public class SubscribersListMapper : ClassMap<SubscribersList>
    {
        public SubscribersListMapper()
        {
            Map(m => m.EmailAddress).Name("Email Address");
        }
    }
}
