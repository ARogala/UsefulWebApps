﻿using UsefulWebApps.Models.MyHomePage;

namespace UsefulWebApps.Repository.IRepository
{
    public interface IQuickLinksRepository : IRepository<QuickLinks>
    {
        //any QuickLink specific database methods here
        Task<List<QuickLinks>> GetQuickLinksForUser(string userId);
    }
}
