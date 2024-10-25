namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transfer
{
    [Key]
    public int Id { get; set; }

    public string? Reference { get; set; }
    public int TransferFrom { get; set; }
    public int TransferTo { get; set; }
    public string? TransferStatus { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; }

    public List<TransferItem> Items { get; set; } = new();
}

public class TransferItem
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("TransferId")]
    public Transfer? transfer { get; set; }
    public int TransferId { get; set; }

    [ForeignKey("ItemUid")]
    public Item? item { get; set; }
    public int ItemUid { get; set; }

    public int Amount { get; set; }
}
