﻿using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using static Dapper.SqlMapper;

namespace UsefulWebApps.Repository
{
    public class GroceryListRepository : Repository<GroceryList>, IGroceryListRepository
    {
        private readonly MySqlConnection _connection;
        public GroceryListRepository(MySqlConnection db) : base(db)
        {
            _connection = db;
        }

        //any GroceryList model specific database methods here
        public async Task<(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemsAndCategories()
        {
            string query = @"
                    SELECT * FROM grocery_list;
                    SELECT * FROM grocery_categories;
                ";
            GridReader gridReader = await _connection.QueryMultipleAsync(query);
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            await _connection.CloseAsync();
            return (groceryListItems, groceryCategoriesEnum);
        }

        public async Task<(GroceryList groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum)> GetGroceryListItemAndCategoriesAtId(int? id)
        {
            string query = @"
                SELECT * FROM grocery_list WHERE Id = @id;
                SELECT * FROM grocery_categories;
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(query, new { id });
            GroceryList groceryListItem = await gridReader.ReadFirstAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            await _connection.CloseAsync();
            return (groceryListItem, groceryCategoriesEnum);
        }


        public async Task GroceryListToggleComplete(int? id)
        {
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            string sql = "SELECT Complete FROM grocery_list WHERE Id = @id";
            bool isComplete = await _connection.QuerySingleAsync<bool>(sql, new { id }, transaction: txn);
            string sql2 = String.Empty;
            if (isComplete)
            {
                sql2 = "UPDATE grocery_list SET Complete = False WHERE Id = @id";
            }
            else
            {
                sql2 = "UPDATE grocery_list SET Complete = True WHERE Id = @id";
            }
            await _connection.ExecuteAsync(sql2, new { id }, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
        }
    }
}