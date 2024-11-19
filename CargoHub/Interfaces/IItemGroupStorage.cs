using System.Collections;
using CargoHub.Models;

public interface IItemGroupStorage
{
    Task<IEnumerable<ItemGroup>> getItemGroups();
    Task<IEnumerable<ItemGroup>> GetItemGroupsInPagination(int offset, int limit);
    Task<ItemGroup?> getItemGroup(int id);
    Task<bool> addItemGroup(ItemGroup itemGroup);
    Task<bool> deleteItemGroup(int id);
    Task<bool> updateItemGroup(int id, ItemGroup? itemGroup);
    IEnumerable<Item>? getItemGroupItems(int itemGroupId);
}