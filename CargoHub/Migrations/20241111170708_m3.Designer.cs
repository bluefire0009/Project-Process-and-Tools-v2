﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Diagnostics.CodeAnalysis;


#nullable disable

namespace CargoHub.Migrations
{
    [ExcludeFromCodeCoverage]
    [DbContext(typeof(DatabaseContext))]
    [Migration("20241111170708_m3")]
    partial class m3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("CargoHub.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactEmail")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactPhone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("CargoHub.Models.Inventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ItemReference")
                        .HasColumnType("TEXT");

                    b.Property<int>("total_allocated")
                        .HasColumnType("INTEGER");

                    b.Property<int>("total_available")
                        .HasColumnType("INTEGER");

                    b.Property<int>("total_expected")
                        .HasColumnType("INTEGER");

                    b.Property<int>("total_on_hand")
                        .HasColumnType("INTEGER");

                    b.Property<int>("total_ordered")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("CargoHub.Models.InventoryLocation", b =>
                {
                    b.Property<int>("InventoryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LocationId")
                        .HasColumnType("INTEGER");

                    b.HasKey("InventoryId", "LocationId");

                    b.HasIndex("LocationId");

                    b.ToTable("InventoryLocations");
                });

            modelBuilder.Entity("CargoHub.Models.Item", b =>
                {
                    b.Property<string>("Uid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<string>("CommodityCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<int>("ItemGroup")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemLine")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ModelNumber")
                        .HasColumnType("TEXT");

                    b.Property<int>("PackOrderQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ShortDescription")
                        .HasColumnType("TEXT");

                    b.Property<int>("SupplierCode")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SupplierId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SupplierPartNumber")
                        .HasColumnType("TEXT");

                    b.Property<int>("UnitOrderQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UnitPurchaseQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UpcCode")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Uid");

                    b.HasIndex("ItemGroup");

                    b.HasIndex("ItemLine");

                    b.HasIndex("ItemType");

                    b.HasIndex("SupplierId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("CargoHub.Models.ItemGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ItemGroups");
                });

            modelBuilder.Entity("CargoHub.Models.ItemLine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ItemLines");
                });

            modelBuilder.Entity("CargoHub.Models.ItemType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ItemTypes");
                });

            modelBuilder.Entity("CargoHub.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("WareHouseId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WareHouseId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("CargoHub.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BillTo")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderStatus")
                        .HasColumnType("TEXT");

                    b.Property<string>("PickingNotes")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ShipTo")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ShipmentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ShippingNotes")
                        .HasColumnType("TEXT");

                    b.Property<int>("SourceId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("TotalAmount")
                        .HasColumnType("REAL");

                    b.Property<float>("TotalDiscount")
                        .HasColumnType("REAL");

                    b.Property<float>("TotalSurcharge")
                        .HasColumnType("REAL");

                    b.Property<float>("TotalTax")
                        .HasColumnType("REAL");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("WareHouseId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BillTo");

                    b.HasIndex("ShipTo");

                    b.HasIndex("WareHouseId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("CargoHub.Models.OrderItems", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Amount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemUid")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OrderId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ItemUid");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("CargoHub.Models.Shipment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CarrierCode")
                        .HasColumnType("TEXT");

                    b.Property<string>("CarrierDescription")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("OrderId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PaymentType")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServiceCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ShipmentDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShipmentStatus")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShipmentType")
                        .HasColumnType("TEXT");

                    b.Property<int>("SourceId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalPackageCount")
                        .HasColumnType("INTEGER");

                    b.Property<float>("TotalPackageWeight")
                        .HasColumnType("REAL");

                    b.Property<string>("TransferMode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Shipments");
                });

            modelBuilder.Entity("CargoHub.Models.ShipmentItems", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Amount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemUid")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ShipmentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ItemUid");

                    b.HasIndex("ShipmentId");

                    b.ToTable("ShipmentItems");
                });

            modelBuilder.Entity("CargoHub.Models.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("AddressExtra")
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("Province")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ZipCode")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("CargoHub.Models.Transfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .HasColumnType("TEXT");

                    b.Property<int>("TransferFrom")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TransferStatus")
                        .HasColumnType("TEXT");

                    b.Property<int>("TransferTo")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TransferFrom");

                    b.HasIndex("TransferTo");

                    b.ToTable("Transfers");
                });

            modelBuilder.Entity("CargoHub.Models.TransferItem", b =>
                {
                    b.Property<int>("TransferId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemUid")
                        .HasColumnType("TEXT");

                    b.Property<int>("Amount")
                        .HasColumnType("INTEGER");

                    b.HasKey("TransferId", "ItemUid");

                    b.HasIndex("ItemUid");

                    b.ToTable("TransferItems");
                });

            modelBuilder.Entity("CargoHub.Models.Warehouse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .HasColumnType("TEXT");

                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactEmail")
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactPhone")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Province")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Zip")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Warehouses");
                });

            modelBuilder.Entity("CargoHub.Models.Inventory", b =>
                {
                    b.HasOne("CargoHub.Models.Item", "item")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.Navigation("item");
                });

            modelBuilder.Entity("CargoHub.Models.InventoryLocation", b =>
                {
                    b.HasOne("CargoHub.Models.Inventory", "inventory")
                        .WithMany("InventoryLocations")
                        .HasForeignKey("InventoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CargoHub.Models.Location", "location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("inventory");

                    b.Navigation("location");
                });

            modelBuilder.Entity("CargoHub.Models.Item", b =>
                {
                    b.HasOne("CargoHub.Models.ItemGroup", "itemGroup")
                        .WithMany()
                        .HasForeignKey("ItemGroup")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CargoHub.Models.ItemLine", "itemLine")
                        .WithMany()
                        .HasForeignKey("ItemLine")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CargoHub.Models.ItemType", "itemType")
                        .WithMany()
                        .HasForeignKey("ItemType")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CargoHub.Models.Supplier", "SupplierById")
                        .WithMany()
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SupplierById");

                    b.Navigation("itemGroup");

                    b.Navigation("itemLine");

                    b.Navigation("itemType");
                });

            modelBuilder.Entity("CargoHub.Models.Location", b =>
                {
                    b.HasOne("CargoHub.Models.Warehouse", "wareHouse")
                        .WithMany()
                        .HasForeignKey("WareHouseId");

                    b.Navigation("wareHouse");
                });

            modelBuilder.Entity("CargoHub.Models.Order", b =>
                {
                    b.HasOne("CargoHub.Models.Client", "client")
                        .WithMany()
                        .HasForeignKey("BillTo");

                    b.HasOne("CargoHub.Models.Location", "location")
                        .WithMany()
                        .HasForeignKey("ShipTo");

                    b.HasOne("CargoHub.Models.Warehouse", "wareHouse")
                        .WithMany()
                        .HasForeignKey("WareHouseId");

                    b.Navigation("client");

                    b.Navigation("location");

                    b.Navigation("wareHouse");
                });

            modelBuilder.Entity("CargoHub.Models.OrderItems", b =>
                {
                    b.HasOne("CargoHub.Models.Item", "item")
                        .WithMany()
                        .HasForeignKey("ItemUid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CargoHub.Models.Order", "order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("item");

                    b.Navigation("order");
                });

            modelBuilder.Entity("CargoHub.Models.Shipment", b =>
                {
                    b.HasOne("CargoHub.Models.Order", "order")
                        .WithOne("ShipmentById")
                        .HasForeignKey("CargoHub.Models.Shipment", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("order");
                });

            modelBuilder.Entity("CargoHub.Models.ShipmentItems", b =>
                {
                    b.HasOne("CargoHub.Models.Item", "item")
                        .WithMany()
                        .HasForeignKey("ItemUid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CargoHub.Models.Shipment", "shipment")
                        .WithMany("Items")
                        .HasForeignKey("ShipmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("item");

                    b.Navigation("shipment");
                });

            modelBuilder.Entity("CargoHub.Models.Transfer", b =>
                {
                    b.HasOne("CargoHub.Models.Location", "LocationFrom")
                        .WithMany()
                        .HasForeignKey("TransferFrom")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CargoHub.Models.Location", "LocationTo")
                        .WithMany()
                        .HasForeignKey("TransferTo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LocationFrom");

                    b.Navigation("LocationTo");
                });

            modelBuilder.Entity("CargoHub.Models.TransferItem", b =>
                {
                    b.HasOne("CargoHub.Models.Item", "item")
                        .WithMany()
                        .HasForeignKey("ItemUid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CargoHub.Models.Transfer", "transfer")
                        .WithMany("Items")
                        .HasForeignKey("TransferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("item");

                    b.Navigation("transfer");
                });

            modelBuilder.Entity("CargoHub.Models.Inventory", b =>
                {
                    b.Navigation("InventoryLocations");
                });

            modelBuilder.Entity("CargoHub.Models.Order", b =>
                {
                    b.Navigation("Items");

                    b.Navigation("ShipmentById");
                });

            modelBuilder.Entity("CargoHub.Models.Shipment", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("CargoHub.Models.Transfer", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
