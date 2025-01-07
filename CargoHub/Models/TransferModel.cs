namespace CargoHub.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transfer : IEquatable<Transfer>
{
    [Key]
    public int Id { get; set; }

    public string? Reference { get; set; }

    public int TransferFrom { get; set; }
    public Location? LocationFrom;
    public int TransferTo { get; set; }

    public Location? LocationTo;
    public string? TransferStatus { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; }

    public List<TransferItem> Items { get; set; } = new();
    public bool IsDeleted { get; set; } = false;

    public bool Equals(Transfer? other)
    {
        if (other is null)
            return false;

        // Compare all relevant properties except foreign keys
        return this.Id == other.Id &&
                this.Reference == other.Reference &&
                this.TransferFrom == other.TransferFrom &&
                this.TransferTo == other.TransferTo &&
                this.TransferStatus == other.TransferStatus &&
                this.CreatedAt == other.CreatedAt &&
                this.Items.SequenceEqual(other.Items);
    }
}

public class TransferItem : IEquatable<TransferItem>
{
    [ForeignKey("TransferId")]
    public Transfer? transfer { get; set; }
    public int TransferId { get; set; }

    [ForeignKey("ItemUid")]
    public Item? item { get; set; }
    public string ItemUid { get; set; }

    public int Amount { get; set; }

    public bool Equals(TransferItem? other)
    {
        if (other is null)
            return false;

        // Compare all relevant properties except foreign keys
        return this.TransferId == other.TransferId &&
                this.ItemUid == other.ItemUid &&
                this.Amount == other.Amount;
    }
}
